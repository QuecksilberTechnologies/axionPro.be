

using AutoMapper;
using axionpro.application.DTOS.Employee.Experience;
using axionpro.application.DTOS.Employee.Sensitive;
using axionpro.application.DTOS.Pagination;
using axionpro.application.Interfaces.IEncryptionService;
using axionpro.application.Interfaces.IHashed;
using axionpro.application.Interfaces.IRepositories;
using axionpro.application.Interfaces.ITokenService;
using axionpro.domain.Entity;
using axionpro.persistance.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace axionpro.persistance.Repositories
{

    public class EmployeeIdentityRepository : IEmployeeIdentityRepository
    {
        private readonly WorkforceDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<EmployeeIdentityRepository> _logger;
        private readonly IDbContextFactory<WorkforceDbContext> _contextFactory;
        private readonly IPasswordService _passwordService;
        private readonly IEncryptionService _encryptionService;
        public EmployeeIdentityRepository(WorkforceDbContext context, IMapper mapper, ILogger<EmployeeIdentityRepository> logger, IDbContextFactory<WorkforceDbContext> contextFactory,
            IPasswordService passwordService, IEncryptionService encryptionService)
        {
            this._context = context;
            this._mapper = mapper;
            this._logger = logger;
            _contextFactory = contextFactory;
            _passwordService = passwordService;
            _encryptionService = encryptionService;

        }


        public async Task<List<GetIdentityResponseDTO>> CreateAsync(EmployeePersonalDetail entity)
        {
            try
            {
                // 1️⃣ Validation
                if (entity == null)
                    throw new ArgumentNullException(nameof(entity), "Personal info entity cannot be null.");

                if (entity.EmployeeId <= 0)
                    throw new ArgumentException("Invalid EmployeeId provided.");

                // 2️⃣ Insert Record
                await _context.EmployeePersonalDetails.AddAsync(entity);
                await _context.SaveChangesAsync();

                // 3️⃣ Fetch with SELECT Projection (FAST + CLEAN)
                var responseList = await _context.EmployeePersonalDetails
                    .AsNoTracking()
                    .Where(x => x.EmployeeId == entity.EmployeeId
                             && x.IsSoftDeleted != true
                             && x.IsActive == true)
                    .OrderByDescending(x => x.Id)
                    .Select(x => new GetIdentityResponseDTO
                    {
                        EmployeeId = x.EmployeeId.ToString(),
                        AadhaarNumber = x.AadhaarNumber,
                        PanNumber = x.PanNumber,
                        PassportNumber = x.PassportNumber,
                        DrivingLicenseNumber = x.DrivingLicenseNumber,
                        VoterId = x.VoterId,
                        BloodGroup = x.BloodGroup,
                        MaritalStatus = x.MaritalStatus,
                        Nationality = x.Nationality,
                        EmergencyContactName = x.EmergencyContactName,
                        EmergencyContactNumber = x.EmergencyContactNumber,
                        EmergencyContactRelation = x.EmergencyContactRelation,

                        // 🔹 Boolean flags for document presence
                        hasAadharIdUploaded = !string.IsNullOrEmpty(x.AadhaarDocPath),
                        hasPanIdUploaded = !string.IsNullOrEmpty(x.PanDocPath),
                        hasPassportIdUploaded = !string.IsNullOrEmpty(x.PassportDocPath),

                        // 🔹 Document paths
                        aadharDocPath = x.AadhaarDocPath,
                        panDocPath = x.PanDocPath,
                        passportDocPath = x.PassportDocPath,

                        // 🔹 Hardcoded/default flags
                        IsInfoVerified = x.IsInfoVerified ?? false,
                        IsEditAllowed = x.IsEditAllowed,  // business rule
                     
                        // 🔹 Completion % (Example logic)
                        CompletionPercentage =
                            (new[]
                            {
                        string.IsNullOrEmpty(x.AadhaarNumber) ? 0 : 1,
                        string.IsNullOrEmpty(x.PanNumber) ? 0 : 1,                        
                        string.IsNullOrEmpty(x.DrivingLicenseNumber) ? 0 : 1,
                        string.IsNullOrEmpty(x.VoterId) ? 0 : 1,
                        string.IsNullOrEmpty(x.BloodGroup) ? 0 : 1,
                        string.IsNullOrEmpty(x.Nationality) ? 0 : 1,
                        string.IsNullOrEmpty(x.EmergencyContactName) ? 0 : 1,
                        string.IsNullOrEmpty(x.EmergencyContactRelation) ? 0 : 1,


                          x.HasAadhaarIdUploaded ? 1 : 0,
                          x.HasPanIdUploaded ? 1 : 0
                            }.Sum() / 10.0) * 100
                    })
                    .Take(1)   // since only latest needed
                    .ToListAsync();

                return responseList;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "❌ Error occurred while adding/fetching personal info for EmployeeId: {EmployeeId}",
                    entity.EmployeeId);

                throw new Exception($"Failed to add or fetch personal info: {ex.Message}");
            }
        }



        public async Task<PagedResponseDTO<GetIdentityResponseDTO>> GetInfo(GetIdentityRequestDTO dto, long employeeId, long id)
        {
            try
            {
                // 🧭 Base Query (Active & SoftDelete check)
                var baseQuery = _context.EmployeePersonalDetails
                    .AsNoTracking()
                    .Where(identity => identity.EmployeeId == employeeId
                                       && identity.IsActive == dto.IsActive
                                       && identity.IsSoftDeleted != true);

                // 🗺️ Optional Filters
                if (!string.IsNullOrWhiteSpace(dto.Id) && long.TryParse(dto.Id, out long parsedId) && parsedId > 0)
                    baseQuery = baseQuery.Where(x => x.Id == parsedId);

                if (!string.IsNullOrWhiteSpace(dto.BloodGroup))
                    baseQuery = baseQuery.Where(x => x.BloodGroup.ToLower().Contains(dto.BloodGroup.ToLower()));

                if (!string.IsNullOrWhiteSpace(dto.MaritalStatus))
                    baseQuery = baseQuery.Where(x => x.MaritalStatus.ToLower().Contains(dto.MaritalStatus.ToLower()));

                if (!string.IsNullOrWhiteSpace(dto.Nationality))
                    baseQuery = baseQuery.Where(x => x.Nationality.ToLower().Contains(dto.Nationality.ToLower()));

                if (!string.IsNullOrWhiteSpace(dto.EmergencyContactName))
                    baseQuery = baseQuery.Where(x => x.EmergencyContactName.ToLower().Contains(dto.EmergencyContactName.ToLower()));

                // 🧩 Aadhaar / PAN / Passport Uploaded Filters
                if (dto.HasAadhaarIdUploaded.HasValue)
                    baseQuery = baseQuery.Where(x => x.HasAadhaarIdUploaded == dto.HasAadhaarIdUploaded.Value);

                if (dto.HasPanIdUploaded.HasValue)
                    baseQuery = baseQuery.Where(x => x.HasPanIdUploaded == dto.HasPanIdUploaded.Value);

                if (dto.HasPassportIdUploaded.HasValue)
                    baseQuery = baseQuery.Where(x => x.HasPassportIdUploaded == dto.HasPassportIdUploaded.Value);

                // 🔍 Keyword Search
                if (!string.IsNullOrEmpty(dto.SortBy))
                {
                    var keyword = dto.SortBy.Trim().ToLower();
                    baseQuery = baseQuery.Where(x =>
                        (x.BloodGroup != null && x.BloodGroup.ToLower().Contains(keyword)) ||
                        (x.MaritalStatus != null && x.MaritalStatus.ToLower().Contains(keyword)) ||
                        (x.Nationality != null && x.Nationality.ToLower().Contains(keyword)) ||
                        (x.EmergencyContactName != null && x.EmergencyContactName.ToLower().Contains(keyword)) ||
                        (x.PanNumber != null && x.PanNumber.ToLower().Contains(keyword)) ||
                        (x.AadhaarNumber != null && x.AadhaarNumber.ToLower().Contains(keyword)) ||
                        (x.PassportNumber != null && x.PassportNumber.ToLower().Contains(keyword)));
                }

                // 🔽 Sorting
                bool isDescending = string.Equals(dto.SortOrder, "desc", StringComparison.OrdinalIgnoreCase);

                baseQuery = !string.IsNullOrEmpty(dto.SortBy)
                    ? dto.SortBy.ToLower() switch
                    {
                        "bloodgroup" => isDescending
                            ? baseQuery.OrderByDescending(x => x.BloodGroup)
                            : baseQuery.OrderBy(x => x.BloodGroup),

                        "maritalstatus" => isDescending
                            ? baseQuery.OrderByDescending(x => x.MaritalStatus)
                            : baseQuery.OrderBy(x => x.MaritalStatus),

                        "nationality" => isDescending
                            ? baseQuery.OrderByDescending(x => x.Nationality)
                            : baseQuery.OrderBy(x => x.Nationality),

                        "hasaadhariduploaded" => isDescending
                            ? baseQuery.OrderByDescending(x => x.HasAadhaarIdUploaded)
                            : baseQuery.OrderBy(x => x.HasAadhaarIdUploaded),

                        "haspaniduploaded" => isDescending
                            ? baseQuery.OrderByDescending(x => x.HasPanIdUploaded)
                            : baseQuery.OrderBy(x => x.HasPanIdUploaded),

                        "haspassportiduploaded" => isDescending
                            ? baseQuery.OrderByDescending(x => x.HasPassportIdUploaded)
                            : baseQuery.OrderBy(x => x.HasPassportIdUploaded),

                        _ => isDescending
                            ? baseQuery.OrderByDescending(x => x.Id)
                            : baseQuery.OrderBy(x => x.Id)
                    }
                    : baseQuery.OrderByDescending(x => x.Id);

                // 📄 Total Count
                var totalRecords = await baseQuery.CountAsync();

                // 🧩 Projection to DTO
                var query = from identity in baseQuery
                            select new GetIdentityResponseDTO
                            {
                              
                               
                                EmployeeId = identity.EmployeeId.ToString(),
                                BloodGroup = identity.BloodGroup,
                                MaritalStatus = identity.MaritalStatus,
                                Nationality = identity.Nationality,
                                EmergencyContactName = identity.EmergencyContactName,
                                EmergencyContactNumber = identity.EmergencyContactNumber,
                                EmergencyContactRelation= identity.EmergencyContactRelation,
                                PanNumber = identity.PanNumber,
                                AadhaarNumber = identity.AadhaarNumber,
                                PassportNumber = identity.PassportNumber,
                                DrivingLicenseNumber = identity.DrivingLicenseNumber,
                                hasPassportIdUploaded = identity.HasPassportIdUploaded,
                                hasAadharIdUploaded = identity.HasAadhaarIdUploaded,
                                hasPanIdUploaded = identity.HasPanIdUploaded,
                                aadharDocPath = identity.AadhaarDocPath,
                                panDocPath = identity.PanDocPath,
                                passportDocPath = identity.PassportDocPath,

                                CompletionPercentage =
                            (
                        new[]
                        {
                            string.IsNullOrEmpty(identity.AadhaarNumber) ? 0 : 1,
                            string.IsNullOrEmpty(identity.PanNumber) ? 0 : 1,
                            string.IsNullOrEmpty(identity.DrivingLicenseNumber) ? 0 : 1,
                            string.IsNullOrEmpty(identity.VoterId) ? 0 : 1,
                            string.IsNullOrEmpty(identity.BloodGroup) ? 0 : 1,
                            string.IsNullOrEmpty(identity.Nationality) ? 0 : 1,
                            string.IsNullOrEmpty(identity.EmergencyContactName) ? 0 : 1,
                            string.IsNullOrEmpty(identity.EmergencyContactRelation) ? 0 : 1,
                            identity.HasAadhaarIdUploaded ? 1 : 0,
                            identity.HasPanIdUploaded ? 1 : 0
                        }.Sum() / 10.0
                    ) * 100

                            };

                // ✅ Calculate overall average percentage (nullable if no record)
              

                // 📜 Pagination
                var pagedRecords = await query
                    .Skip((dto.PageNumber - 1) * dto.PageSize)
                    .Take(dto.PageSize)
                    .ToListAsync();

                // 🔹 Overall average CompletionPercentage
                double? averagePercentage = pagedRecords.Any()
                    ? pagedRecords.Average(x => x.CompletionPercentage ?? 0)
                    : (double?)null;

                // 🔹 HasUploadedAllDocs based on pagedRecords
                bool? hasUploadedAllDocs = pagedRecords.Any()
                    ? pagedRecords.All(x => x.hasAadharIdUploaded && x.hasPanIdUploaded && x.hasPassportIdUploaded)
                    : false;

               
 
                // 📦 Final Response
                return new PagedResponseDTO<GetIdentityResponseDTO>
                {
                    Items = pagedRecords ?? new List<GetIdentityResponseDTO>(),
                    TotalCount = totalRecords,
                    PageNumber = dto.PageNumber,
                    PageSize = dto.PageSize,
                    CompletionPercentage = averagePercentage,
                    HasUploadedAll= hasUploadedAllDocs,
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error occurred while fetching identity info for EmployeeId: {EmployeeId}", employeeId);
                throw new Exception($"Failed to fetch identity info: {ex.Message}");
            }
        }


        public Task<EmployeeContact> GetSingleRecordAsync(long Id, bool IsActive)
        {
            throw new NotImplementedException();
        }

        Task<GetIdentityResponseDTO> IEmployeeIdentityRepository.GetSingleRecordAsync(long Id, bool IsActive)
        {
            throw new NotImplementedException();
        }
    }
}




 
 






