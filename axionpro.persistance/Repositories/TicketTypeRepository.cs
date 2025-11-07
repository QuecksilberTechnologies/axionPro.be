using AutoMapper;
using axionpro.application.Constants;
using axionpro.application.DTOs.Leave;
using axionpro.application.DTOS.TicketDTO.TicketType;
using axionpro.application.Interfaces.IRepositories;
using axionpro.application.Wrappers;
using axionpro.domain.Entity;
using axionpro.persistance.Data.Context;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace axionpro.persistance.Repositories
{

    public class TicketTypeRepository : ITicketTypeRepository
    {
        #region Fields
        private readonly WorkforceDbContext _context;
        private readonly ILogger<TicketTypeRepository> _logger;
        private readonly IMapper _mapper;
        private IDbContextFactory<WorkforceDbContext> _contextFactory;
        #endregion



        #region Constructor
        public TicketTypeRepository(WorkforceDbContext context, ILogger<TicketTypeRepository> logger, IMapper mapper, IDbContextFactory<WorkforceDbContext> contextFactory
            )
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
        }


        #endregion

        #region Get Methods


        /// <summary>
        /// Retrieves all active Ticket Types for a given tenant, including their associated
        /// responsible role and primary responsible employee (based on IsPrimary = true).
        /// </summary>
        /// <param name="dTO">
        /// The request DTO containing tenant-specific filters such as <see cref="GetTicketTypeRequestDTO.TenantId"/> 
        /// and <see cref="GetTicketTypeRequestDTO.IsActive"/>.
        /// </param>
        /// <returns>
        public async Task<List<GetTicketTypeResponseDTO>> AllAsync(GetTicketTypeRequestDTO dTO)
        {
            try
            {
                if (dTO == null || dTO.TenantId <= 0)
                {
                    _logger.LogWarning("AllAsync called with invalid DTO or TenantId: {TenantId}", dTO?.TenantId);
                    return new List<GetTicketTypeResponseDTO>();
                }

                await using var context = await _contextFactory.CreateDbContextAsync();

                var query =
                    from t in context.TicketTypes.AsNoTracking()
                    join r in context.Roles.AsNoTracking() on t.ResponsibleRoleId equals r.Id into roleGroup
                    from r in roleGroup.DefaultIfEmpty()
                    join ur in context.UserRoles.AsNoTracking()
                        on r.Id equals ur.RoleId into userRoleGroup
                    from ur in userRoleGroup
                        .Where(x => x.IsPrimaryRole == true && x.IsActive == true)
                        .DefaultIfEmpty()
                    join e in context.Employees.AsNoTracking()
                        on ur.EmployeeId equals e.Id into empGroup
                    from e in empGroup
                        .Where(emp => emp.IsActive == true)
                        .DefaultIfEmpty()
                    where t.TenantId == dTO.TenantId
                          && t.IsActive == dTO.IsActive
                          && (t.IsSoftDeleted == false || t.IsSoftDeleted == null)
                    select new GetTicketTypeResponseDTO
                    {
                        Id = t.Id,
                        TicketTypeName = t.TicketTypeName,
                        TicketHeaderId = t.TicketHeaderId,
                        ResponsibleRoleId = r != null ? r.Id : null,
                        ResponsibleRoleName = r != null ? r.RoleName : null,
                        ResponsibleEmployeeId = e != null ? e.Id : null,
                        ResponsibleEmployeeName = e != null ? (e.FirstName + " " + e.LastName) : null,
                        ResponsibleEmployeeEmailId = e != null ? e.OfficialEmail : null,
                        TenantId = t.TenantId,
                        Description = t.Description,
                        IsActive = t.IsActive,
                        AddedById = t.AddedById,
                        AddedDateTime = t.AddedDateTime,
                        UpdatedById = t.UpdatedById,
                        UpdatedDateTime = t.UpdatedDateTime
                    };

                var ticketTypes = await query.ToListAsync();

                if (ticketTypes == null || !ticketTypes.Any())
                {
                    _logger.LogInformation("No TicketTypes found for TenantId {TenantId}", dTO.TenantId);
                    return new List<GetTicketTypeResponseDTO>();
                }

                _logger.LogInformation("Fetched {Count} TicketTypes for TenantId {TenantId}", ticketTypes.Count, dTO.TenantId);

                return ticketTypes;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching all TicketTypes for TenantId {TenantId}", dTO?.TenantId);
                throw;
            }
        }

        public async Task<List<GetTicketTypeResponseDTO>> AllByHeaderIdAsync(GetTicketTypeByHeaderIdRequestDTO dTO)
        {
            try
            {
                var ticketTypes = await _context.TicketTypes
                    .Where(t => t.TicketHeaderId == dTO.TicketHeaderId && t.TenantId == dTO.TenantId && t.IsActive && (t.IsSoftDeleted == false || t.IsSoftDeleted == null))
                    .ToListAsync();

                if (ticketTypes == null || !ticketTypes.Any())
                    return new List<GetTicketTypeResponseDTO>();

                return _mapper.Map<List<GetTicketTypeResponseDTO>>(ticketTypes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching all TicketTypes");
                throw;
            }
        }

        public async Task<GetTicketTypeResponseDTO?> GetByIdAsync(long id)
        {
            try
            {
                var entity = await _context.TicketTypes
                    .FirstOrDefaultAsync(t =>  t.Id == id && t.IsActive && (t.IsSoftDeleted == false || t.IsSoftDeleted == null));

                if (entity == null)
                {
                    _logger.LogWarning("TicketType with Id {Id} not found", id);
                    return null;
                }

                return _mapper.Map<GetTicketTypeResponseDTO>(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching TicketType by Id {Id}", id);
                throw;
            }
        }
        #endregion

        #region Add Method
        public async Task<List<GetTicketTypeResponseDTO>> AddAsync(AddTicketTypeRequestDTO dto)
        {
            try
            {
                var entity = new TicketType
                {
                    TicketTypeName = dto.TicketTypeName,
                    ResponsibleRoleId= dto.ResponsibleRoleId,
                    Description = dto.Description,
                    TenantId = dto.TenantId,
                    IsActive = true,
                    TicketHeaderId = dto.TicketHeaderId,
                    AddedDateTime = DateTime.UtcNow,
                    AddedById = dto.EmployeeId,
                };

                await _context.TicketTypes.AddAsync(entity);
                await _context.SaveChangesAsync();

                var ticketTypes = await _context.TicketTypes
                    .Where(t => t.IsActive && (t.IsSoftDeleted == false || t.IsSoftDeleted == null))
                    .ToListAsync();

                return _mapper.Map<List<GetTicketTypeResponseDTO>>(ticketTypes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding new TicketType");
                throw;
            }
        }

        #endregion

        #region Update Method
        /// <summary>
        /// Updates an existing TicketType record with new values if changed.
        /// </summary>
        /// <param name="dto">DTO containing updated fields</param>
        /// <returns>True if update was successful or no changes were needed; false if record not found</returns>
        /// <summary>
        /// Updates an existing TicketType record. Only non-null values from DTO will be updated.
        /// </summary>
        public async Task<bool> UpdateAsync(UpdateTicketTypeRequestDTO dto)
        {
            try
            {
                // 1️⃣ Fetch existing entity with IsActive check
                var entity = await _context.TicketTypes
                    .FirstOrDefaultAsync(t => t.Id == dto.Id && t.IsActive);

                if (entity == null)
                {
                    _logger.LogWarning("TicketType with Id {Id} not found or inactive", dto.Id);
                    return false;
                }

                bool isModified = false;

                // 2️⃣ Update only if DTO value is not null
                if (!string.IsNullOrWhiteSpace(dto.TicketTypeName) &&
                    !string.Equals(entity.TicketTypeName, dto.TicketTypeName, StringComparison.OrdinalIgnoreCase))
                {
                    entity.TicketTypeName = dto.TicketTypeName;
                    isModified = true;
                }


                if (dto.ResponsibleRoleId.HasValue && entity.ResponsibleRoleId != dto.ResponsibleRoleId.Value)
                {
                    entity.ResponsibleRoleId = dto.ResponsibleRoleId.Value;
                    isModified = true;
                }
              

                if (dto.IsActive.HasValue && entity.IsActive != dto.IsActive.Value)
                {
                    entity.IsActive = dto.IsActive.Value;
                    isModified = true;
                }

                if (!string.IsNullOrWhiteSpace(dto.Description) &&
                    !string.Equals(entity.Description, dto.Description, StringComparison.OrdinalIgnoreCase))
                {
                    entity.Description = dto.Description;
                    isModified = true;
                }

                if (!isModified)
                {
                  
                    _logger.LogInformation("No changes detected for TicketType with Id {Id}", dto.Id);
                    return true; // No update needed
                }
                entity.IsSoftDeleted = false;

                // 3️⃣ Set Updated metadata
                entity.UpdatedById = dto.EmployeeId;
                entity.UpdatedDateTime = DateTime.UtcNow;

                // 4️⃣ Save changes
                var result = await _context.SaveChangesAsync();
                _logger.LogInformation("TicketType with Id {Id} updated successfully", dto.Id);
                return result > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating TicketType with Id {Id}", dto.Id);
                throw;
            }
        }

        #endregion

        #region Delete Method
        public async Task<bool> DeleteAsync(long id, long employeeId)
        {
            try
            {
                var entity = await _context.TicketTypes
                    .FirstOrDefaultAsync(t => t.Id == id && t.IsActive);

                if (entity == null)
                {
                    _logger.LogWarning("TicketType with Id {Id} not found for delete", id);
                    return false;
                }

                entity.IsActive = ConstantValues.IsByDefaultFalse;
                entity.IsSoftDeleted = ConstantValues.IsByDefaultTrue;
                entity.SoftDeletedById = employeeId;
                entity.SoftDeletedTime = DateTime.UtcNow;
               

                var result = await _context.SaveChangesAsync();
                return result > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting TicketType with Id {Id}", id);
                throw;
            }
        }
        /// <summary>
        /// Fetches all Ticket Types based on the given RoleId.
        /// Each Ticket Type groups multiple responsible employees (if any) under the same role.
        /// </summary>
        /// <param name="dTO">Request DTO containing RoleId.</param>
        /// <returns>List of Ticket Types with grouped employees responsible for each ticket type.</returns>

        /// <summary>
        /// Fetch all TicketTypes for a given requester's RoleId along with responsible employees.
        /// Each TicketType can have multiple responsible employees.
        /// </summary>
        /// <param name="dTO">DTO containing the requester's RoleId</param>
        /// <returns>List of TicketType DTOs with Employee info</returns>
        public async Task<List<GetTicketTypeRoleResponseDTO>> AllByRoleIdAsync(GetTicketTypeByRoleIdRequestDTO dTO)
        {
            try
            {
                if (dTO == null || dTO.ResponsibleRoleId <= 0)
                {
                    _logger.LogWarning("AllByRoleIdAsync called with invalid RoleId: {RoleId}", dTO?.ResponsibleRoleId);
                    return new List<GetTicketTypeRoleResponseDTO>();
                }

                await using var context = await _contextFactory.CreateDbContextAsync();

                // 1️⃣ Flattened query: TicketType → ResponsibleRole → UserRole → Employee
                var flatData =
                    from ticketType in context.TicketTypes.AsNoTracking()
                    join role in context.Roles.AsNoTracking() on ticketType.ResponsibleRoleId equals role.Id
                    join userRole in context.UserRoles.AsNoTracking() on role.Id equals userRole.RoleId
                    join emp in context.Employees.AsNoTracking() on userRole.EmployeeId equals emp.Id
                    where role.IsActive
                          && userRole.IsActive
                          && emp.IsActive
                          && ticketType.IsActive && ticketType.TenantId==dTO.TenantId
                          && (ticketType.IsSoftDeleted == false || ticketType.IsSoftDeleted == null)
                    select new
                    {
                        TicketTypeId = ticketType.Id,
                        ticketType.TicketTypeName,
                        ticketType.Description,
                        ResponsibleRoleId = role.Id,
                        ResponsibleRoleName = role.RoleName,
                        EmployeeId = emp.Id,
                        EmployeeName = emp.FirstName + " " + emp.LastName,
                        EmployeeEmail = emp.OfficialEmail
                    };

                // 2️⃣ Group by TicketType to consolidate multiple employees
                var grouped = await flatData
                    .GroupBy(t => new
                    {
                        t.TicketTypeId,
                        t.TicketTypeName,
                        t.Description,
                        t.ResponsibleRoleId,
                        t.ResponsibleRoleName
                    })
                    .Select(g => new GetTicketTypeRoleResponseDTO
                    {
                        Id = g.Key.TicketTypeId,
                        TicketTypeName = g.Key.TicketTypeName,
                        Description = g.Key.Description,
                        ResponsibleRoleId = g.Key.ResponsibleRoleId,
                        ResponsibleRoleName = g.Key.ResponsibleRoleName,
                        Employees = g.Select(e => new EmployeeShortInfoDTO
                        {
                            Id = e.EmployeeId,
                            Name = e.EmployeeName,
                            Email = e.EmployeeEmail
                        }).ToList()
                    })
                    .ToListAsync();

                // 3️⃣ Filter only those TicketTypes where the requester's RoleId is allowed
                var filtered = grouped
                    .Where(t => t.ResponsibleRoleId == dTO.ResponsibleRoleId)
                    .ToList();

                _logger.LogInformation("Fetched {Count} grouped TicketTypes for RoleId {RoleId}", filtered.Count, dTO.ResponsibleRoleId);

                return filtered;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in AllByRoleIdAsync for RoleId {RoleId}", dTO?.ResponsibleRoleId);
                throw;
            }
        }


        #endregion


    }
}
