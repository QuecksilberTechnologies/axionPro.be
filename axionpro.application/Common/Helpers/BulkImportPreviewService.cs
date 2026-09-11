using System.Text.Json;
using axionpro.application.Common.Enums;
using axionpro.application.Constants;
using axionpro.application.DTOs.Department;
using axionpro.application.DTOs.Designation;
using axionpro.application.DTOs.Role;
using axionpro.application.DTOS.Common;
using axionpro.application.Exceptions;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;

namespace axionpro.application.Common.Helpers;

/// <summary>Produces tenant-scoped master previews using existing repositories. Never persists data.</summary>
public sealed class BulkImportPreviewService(
    IUnitOfWork unitOfWork,
    ICommonRequestService commonRequestService)
{
    #region Trusted reads

    public async Task<BulkImportPreviewResponseDTO> PreviewAsync(
        BulkImportMaster master,
        BulkImportPreviewRequestDTO request,
        CancellationToken cancellationToken)
    {
        // The module-specific MediatR behavior runs before this service.
        var context = await commonRequestService.ValidateTenantUserRequestAsync();
        if (!context.Success || context.TenantId <= 0 ||
            context.LoggedInEmployeeId <= 0 || context.RoleId <= 0)
        {
            throw new UnauthorizedAccessException(AppConstants.ErrorMessages.Unauthorized);
        }

        var table = await BulkImportTableReader.ReadAsync(request, cancellationToken);
        var departments = new List<GetDepartmentResponseDTO>();
        var designations = new List<GetDesignationResponseDTO>();
        var roles = new List<GetRoleResponseDTO>();
        var employeeTypes = new List<global::axionpro.application.DTOs.EmployeeType.GetEmployeeTypeResponseDTO>();

        // Existing list repositories already enforce tenant ownership and exclude soft-deleted rows.
        if (master is BulkImportMaster.Department or BulkImportMaster.Designation)
        {
            departments = (await unitOfWork.DepartmentRepository.GetAsync(
                new GetDepartmentRequestDTO { PageNumber = 1, PageSize = int.MaxValue },
                context.TenantId,
                cancellationToken)).Data;
        }

        if (master == BulkImportMaster.Designation)
        {
            designations = (await unitOfWork.DesignationRepository.GetAsync(
                new GetDesignationRequestDTO { PageNumber = 1, PageSize = int.MaxValue },
                context.TenantId,
                cancellationToken)).Data;
        }

        if (master == BulkImportMaster.Role)
        {
            roles = (await unitOfWork.RoleRepository.GetAsync(
                context.TenantId,
                new GetRoleRequestDTO
                {
                    PageNumber = 1,
                    PageSize = int.MaxValue,
                    IsActive = false // Existing repository convention: include both states.
                })).Data;
        }

        if (master == BulkImportMaster.EmployeeType)
        {
            employeeTypes = await unitOfWork.EmployeeTypeRepository.GetAllAsync(context.TenantId, cancellationToken);
        }

        cancellationToken.ThrowIfCancellationRequested();
        return Build(master, request.ColumnMappingJson, table, departments, designations, roles, employeeTypes);
    }

    #endregion

    #region Mapping and row validation

    public static BulkImportPreviewResponseDTO Build(
        BulkImportMaster master,
        string? mappingJson,
        BulkImportTableDTO table,
        IReadOnlyList<GetDepartmentResponseDTO> departments,
        IReadOnlyList<GetDesignationResponseDTO> designations,
        IReadOnlyList<GetRoleResponseDTO> roles,
        IReadOnlyList<global::axionpro.application.DTOs.EmployeeType.GetEmployeeTypeResponseDTO>? employeeTypes = null)
    {
        var nameField = master switch
        {
            BulkImportMaster.Department => BulkImportConstants.DepartmentName,
            BulkImportMaster.Designation => BulkImportConstants.DesignationName,
            BulkImportMaster.Role => BulkImportConstants.RoleName,
            BulkImportMaster.EmployeeType => BulkImportConstants.TypeName,
            _ => throw new ValidationErrorException(AppConstants.ErrorMessages.InvalidRequest)
        };
        var allowed = new List<string> { nameField, BulkImportConstants.IsActive };
        allowed.Add(master == BulkImportMaster.Role ? BulkImportConstants.Remark : BulkImportConstants.Description);
        if (master is BulkImportMaster.Department or BulkImportMaster.EmployeeType)
        {
            allowed.Add(BulkImportConstants.Remark);
        }
        if (master == BulkImportMaster.Designation)
        {
            allowed.Add(BulkImportConstants.DepartmentName);
        }
        if (master == BulkImportMaster.Role)
        {
            allowed.Add(BulkImportConstants.RoleType);
        }

        var result = new BulkImportPreviewResponseDTO
        {
            Master = master,
            SourceColumns = table.Columns.ToList()
        };
        var mapping = ResolveColumnMapping(table, mappingJson, allowed);
        result.ColumnMapping = mapping;
        var required = new List<string> { nameField };
        if (master == BulkImportMaster.Designation)
        {
            required.Add(BulkImportConstants.DepartmentName);
        }
        if (master == BulkImportMaster.Role)
        {
            required.Add(BulkImportConstants.RoleType);
        }
        foreach (var field in required.Where(field => !mapping.ContainsKey(field)))
        {
            result.Errors.Add($"Map the required column {field}.");
        }

        foreach (var source in table.Rows)
        {
            var row = new BulkImportPreviewRowDTO
            {
                RowNumber = source.RowNumber,
                Values = mapping.ToDictionary(pair => pair.Key,
                    pair => source.Values[table.Columns.IndexOf(pair.Value)].Trim()),
                Status = BulkImportRowStatus.Ready
            };
            var name = row.Values.GetValueOrDefault(nameField, string.Empty);
            foreach (var field in required)
            {
                if (string.IsNullOrWhiteSpace(row.Values.GetValueOrDefault(field)))
                {
                    row.Errors.Add($"{field} is required.");
                }
            }
            if (name.Length > (master == BulkImportMaster.Role ? 100 : 255))
            {
                row.Errors.Add($"{nameField} exceeds the database field length.");
            }
            if (row.Values.GetValueOrDefault(BulkImportConstants.Description, string.Empty).Length > (master == BulkImportMaster.EmployeeType ? 255 : 500) ||
                row.Values.GetValueOrDefault(BulkImportConstants.Remark, string.Empty).Length > (master == BulkImportMaster.EmployeeType ? 255 : 200))
            {
                row.Errors.Add("Description or Remark exceeds the database field length.");
            }
            if (row.Values.TryGetValue(BulkImportConstants.IsActive, out var active) &&
                !string.IsNullOrWhiteSpace(active) && !bool.TryParse(active, out _))
            {
                row.Errors.Add("IsActive must be true or false.");
            }

            if (master == BulkImportMaster.Department)
            {
                var matches = departments.Where(item => Same(item.DepartmentName, name)).ToList();
                Match(row, matches.Select(item => (item.Id, item.IsActive)).ToList());
            }
            else if (master == BulkImportMaster.Designation)
            {
                var departmentName = row.Values.GetValueOrDefault(BulkImportConstants.DepartmentName);
                var parents = departments.Where(item => Same(item.DepartmentName, departmentName)).ToList();
                if (parents.Count != 1 || !parents[0].IsActive)
                {
                    row.Errors.Add("DepartmentName must match exactly one active department in this tenant; create missing departments first.");
                }
                else
                {
                    row.DepartmentId = parents[0].Id;
                    var matches = designations.Where(item =>
                        item.DepartmentId == row.DepartmentId && Same(item.DesignationName, name)).ToList();
                    Match(row, matches.Select(item => (item.Id, item.IsActive)).ToList());
                }
            }
            else if (master == BulkImportMaster.EmployeeType)
            {
                var matches = (employeeTypes ?? Array.Empty<global::axionpro.application.DTOs.EmployeeType.GetEmployeeTypeResponseDTO>())
                    .Where(item => Same(item.TypeName, name)).ToList();
                Match(row, matches.Select(item => (item.Id, item.IsActive == true)).ToList());
            }
            else
            {
                var rawType = row.Values.GetValueOrDefault(BulkImportConstants.RoleType);
                if (!int.TryParse(rawType, out var roleType) ||
                    (roleType != ConstantValues.RoleTypeAdmin &&
                     roleType != ConstantValues.RoleTypeManager &&
                     roleType != ConstantValues.RoleTypeEmployee))
                {
                    row.Errors.Add("RoleType must be an existing Admin, Manager or Employee type identifier.");
                }
                var matches = roles.Where(item => Same(item.RoleName, name)).ToList();
                Match(row, matches.Select(item => (item.Id, item.IsActive)).ToList());
                if (matches.Count == 1 && matches[0].RoleType != roleType)
                {
                    row.Errors.Add("Existing role has a different RoleType; import must not overwrite it.");
                }
            }

            result.Rows.Add(row);
        }

        var duplicates = result.Rows.GroupBy(row => (
                Name: row.Values.GetValueOrDefault(nameField, string.Empty).ToUpperInvariant(),
                Department: master == BulkImportMaster.Designation
                    ? row.Values.GetValueOrDefault(BulkImportConstants.DepartmentName, string.Empty).ToUpperInvariant()
                    : string.Empty))
            .Where(group => group.Count() > 1);
        foreach (var group in duplicates)
        {
            foreach (var row in group)
            {
                row.Errors.Add("Duplicate source rows: " + string.Join(", ", group.Select(item => item.RowNumber)));
            }
        }
        foreach (var row in result.Rows.Where(row => row.Errors.Count > 0))
        {
            row.Status = BulkImportRowStatus.Invalid;
        }

        return result;
    }

    private static void Match(BulkImportPreviewRowDTO row, List<(int Id, bool Active)> matches)
    {
        if (matches.Count > 1)
        {
            row.Errors.Add("Multiple existing matches require manual review.");
        }
        else if (matches.Count == 1)
        {
            row.ExistingId = matches[0].Id;
            row.Status = BulkImportRowStatus.Existing;
            if (!matches[0].Active)
            {
                row.Errors.Add("Existing record is inactive; import will not reactivate it.");
            }
        }
    }

    /// <summary>Resolves explicit or deterministic canonical headers for every import module.</summary>
    public static Dictionary<string, string> ResolveColumnMapping(
        BulkImportTableDTO table, string? mappingJson, IReadOnlyCollection<string> allowed)
    {
        Dictionary<string, string> mapping;
        try
        {
            mapping = string.IsNullOrWhiteSpace(mappingJson)
                ? new Dictionary<string, string>()
                : JsonSerializer.Deserialize<Dictionary<string, string>>(mappingJson) ?? throw new JsonException();
        }
        catch (JsonException)
        {
            throw new ValidationErrorException("ColumnMappingJson must be an object of target field to source header.");
        }
        foreach (var pair in mapping)
        {
            if (!allowed.Contains(pair.Key) || !table.Columns.Contains(pair.Value))
            {
                throw new ValidationErrorException("Column mapping contains an unknown target field or source header.");
            }
        }
        foreach (var field in allowed.Where(field => !mapping.ContainsKey(field)))
        {
            var matches = table.Columns.Where(column => HeaderKey(column) == HeaderKey(field)).ToList();
            if (matches.Count == 1)
            {
                mapping[field] = matches[0];
            }
        }
        if (mapping.Values.Distinct(StringComparer.OrdinalIgnoreCase).Count() != mapping.Count)
        {
            throw new ValidationErrorException("One source column cannot map to multiple target fields.");
        }
        return mapping;
    }

    private static string HeaderKey(string value)
    {
        return string.Concat(value.Where(char.IsLetterOrDigit)).ToUpperInvariant();
    }

    private static bool Same(string? first, string? second)
    {
        return string.Equals(first?.Trim(), second?.Trim(), StringComparison.OrdinalIgnoreCase);
    }

    #endregion
}
