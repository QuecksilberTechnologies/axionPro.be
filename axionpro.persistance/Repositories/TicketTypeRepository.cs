using AutoMapper;
using axionpro.application.Constants;
using axionpro.application.DTOS.Pagination;
using axionpro.application.DTOS.TicketDTO.TicketType;
using axionpro.application.Exceptions;
using axionpro.application.Interfaces.IRepositories;
using axionpro.domain.Entity;
using axionpro.persistance.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace axionpro.persistance.Repositories
{

    public class TicketTypeRepository : ITicketTypeRepository
    {
        #region Fields
        private readonly WorkforceDbContext _context;
        private readonly ILogger<TicketTypeRepository> _logger;
        private readonly IMapper _mapper;
      
        #endregion



        #region Constructor
        public TicketTypeRepository(WorkforceDbContext context, ILogger<TicketTypeRepository> logger, IMapper mapper
            )
        {
            this._context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
  
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
        public async Task<PagedResponseDTO<GetTicketTypeResponseDTO>> AllAsync(GetTicketTypeRequestDTO dto)
        {
            try
            {
                // ===============================
                // 1️⃣ DEFAULT PAGINATION
                // ===============================
                int pageNumber = dto.PageNumber <= 0 ? 1 : dto.PageNumber;
                int pageSize = dto.PageSize <= 0 ? 10 : dto.PageSize;

                // ===============================
                // 2️⃣ BASE QUERY
                // ===============================
                var baseQuery =
                    from t in _context.TicketTypes.AsNoTracking()

                    join th in _context.TicketHeaders.AsNoTracking()
                        on t.TicketHeaderId equals th.Id into headerGroup
                    from th in headerGroup.DefaultIfEmpty()

                    join r in _context.Roles.AsNoTracking()
                        on t.ResponsibleRoleId equals r.Id into roleGroup
                    from r in roleGroup.DefaultIfEmpty()

                    join ar in _context.Roles.AsNoTracking()
                        on t.ApprovalRoleId equals ar.Id into approvalGroup
                    from ar in approvalGroup.DefaultIfEmpty()

                    where t.TenantId == dto.Prop.TenantId
                          && t.IsActive == true
                          && t.IsSoftDeleted != true

                    select new GetTicketTypeResponseDTO
                    {
                        Id = t.Id,
                        TicketTypeName = t.TicketTypeName,

                        TicketHeaderId = t.TicketHeaderId,
                        TicketHeaderName = th != null ? th.HeaderName : null,

                        TenantId = t.TenantId,

                        ResponsibleRoleId = r != null ? r.Id : null,
                        ResponsibleRoleName = r != null ? r.RoleName : null,

                        ApprovalId = t.ApprovalRoleId,
                        ApprovalRoleName = ar != null ? ar.RoleName : null,

                        IsApprovalRequired = t.IsApprovalRequired,
                        AutoApproveIfSameRole = t.AutoApproveIfSameRole,
                        IsAttachmentRequired = t.IsAttachmentRequired,
                        SLAHours = t.SLAHours,
                        IsActiveForAllUsers = t.IsActiveForAllUsers,

                        Description = t.Description,
                        IsActive = t.IsActive
                    };

                // ===============================
                // 3️⃣ TOTAL COUNT (BEFORE PAGINATION)
                // ===============================
                var totalCount = await baseQuery.CountAsync();

                // ===============================
                // 4️⃣ SORTING
                // ===============================
                baseQuery = dto.SortBy?.ToLower() switch
                {
                    "name" => dto.SortOrder == "asc"
                        ? baseQuery.OrderBy(x => x.TicketTypeName)
                        : baseQuery.OrderByDescending(x => x.TicketTypeName),

                    _ => baseQuery.OrderByDescending(x => x.Id)
                };

                // ===============================
                // 5️⃣ PAGINATION
                // ===============================
                var data = await baseQuery
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                // ===============================
                // 6️⃣ RESPONSE
                // ===============================
                return new PagedResponseDTO<GetTicketTypeResponseDTO>
                {
                    Data = data,
                    TotalCount = totalCount,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching TicketTypes for TenantId {TenantId}", dto.Prop?.TenantId);
                throw;
            }
        }
        public async Task<List<GetTicketTypeResponseDTO>> AllByHeaderIdAsync(GetTicketTypeByHeaderIdRequestDTO dTO)
        {
            try
            {
                var ticketTypes = await _context.TicketTypes
                    .Where(t => t.TicketHeaderId == dTO.TicketHeaderId && t.TenantId == dTO.Prop.TenantId && t.IsActive && (t.IsSoftDeleted !=true))
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
                    .FirstOrDefaultAsync(t =>  t.Id == id && t.IsActive && (t.IsSoftDeleted != true));

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
        public async Task<GetTicketTypeResponseDTO> AddAsync(TicketType entity)
        {
            try
            {
                await _context.TicketTypes.AddAsync(entity);
                await _context.SaveChangesAsync();

                var data = await _context.TicketTypes
                    .Include(t => t.TicketHeader)
                    .Include(t => t.ResponsibleRole)
                    .Include(t => t.ApprovalRole)
                    .FirstOrDefaultAsync(t => t.Id == entity.Id);

                return _mapper.Map<GetTicketTypeResponseDTO>(data);
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
        public async Task<bool> UpdateAsync(UpdateTicketTypeRequestDTO dto, long userId)
        {
            try
            {
                var entity = await _context.TicketTypes
                    .FirstOrDefaultAsync(x =>
                        x.Id == dto.Id &&
                        x.IsSoftDeleted != true &&
                        x.TenantId == dto.Prop.TenantId);

                if (entity == null)
                    throw new ValidationErrorException("TicketType not found.");

                // ===============================
                // UPDATE FIELDS
                // ===============================
                entity.TicketTypeName = dto.TicketTypeName;
                entity.TicketHeaderId = dto.TicketHeaderId;
                entity.Description = dto.Description;

                entity.ResponsibleRoleId = dto.ResponsibleRoleId;

                entity.IsApprovalRequired = dto.IsApprovalRequired;
                entity.ApprovalRoleId = dto.ApprovalRoleId;
                entity.AutoApproveIfSameRole = dto.AutoApproveIfSameRole;
                entity.IsAttachmentRequired = dto.IsAttachmentRequired;
                entity.SLAHours = dto.SLAHours;
                entity.IsActiveForAllUsers = dto.IsActiveForAllUsers;

                entity.UpdatedById = userId;
                entity.UpdatedDateTime = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating TicketType Id {Id}", dto.Id);
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
                    .FirstOrDefaultAsync(x =>
                        x.Id == id && x.IsSoftDeleted != true && x.IsActive);

                if (entity == null)
                    throw new ValidationErrorException("TicketType not found.");

                // 🔥 SOFT DELETE
                entity.IsSoftDeleted = true;
                entity.IsActive = false;
                entity.SoftDeletedById = employeeId;
                entity.SoftDeletedTime = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting TicketType Id {Id}", id);
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

               
                // 1️⃣ Flattened query: TicketType → ResponsibleRole → UserRole → Employee
                var flatData =
                    from ticketType in _context.TicketTypes.AsNoTracking()
                    join role in _context.Roles.AsNoTracking() on ticketType.ResponsibleRoleId equals role.Id
                    join userRole in _context.UserRoles.AsNoTracking() on role.Id equals userRole.RoleId
                    join emp in _context.Employees.AsNoTracking() on userRole.EmployeeId equals emp.Id
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
                        EmployeeEmail = emp.OfficialEmail,
                       
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
                        
                        Employees = g.Select(e => new EmployeeMinInfoDTO
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
