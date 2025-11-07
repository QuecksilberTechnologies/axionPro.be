using AutoMapper;
using axionpro.application.DTOS.Common;
using axionpro.application.DTOS.Gender;
using axionpro.application.DTOS.Location;
using axionpro.application.Interfaces.IRepositories;
using axionpro.application.Wrappers;
using axionpro.domain.Entity;
using axionpro.persistance.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.persistance.Repositories
{
    public class LocationRepository : ILocationRepository
    {
        private readonly WorkforceDbContext _context;
        private readonly IDbContextFactory<WorkforceDbContext> _contextFactory;
        private readonly ILogger<LocationRepository> _logger;
        private readonly IMapper _mapper;

        public LocationRepository(WorkforceDbContext context, ILogger<LocationRepository> logger, IMapper mapper, IDbContextFactory<WorkforceDbContext> contextFactory)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(_mapper));
            _contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
        }
        public async Task<List<Country>> GetAllAsync() => await _context.Countries.ToListAsync();
        public async Task<Country> GetByIdAsync(int id) => await _context.Countries.FindAsync(id);
        public async Task AddAsync(Country country)
        {
            await _context.Countries.AddAsync(country);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateAsync(Country country)
        {
            _context.Countries.Update(country);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteAsync(int id)
        {
            var country = await _context.Countries.FindAsync(id);
            if (country != null)
            {
                _context.Countries.Remove(country);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<ApiResponse<List<GetCountryOptionResponseDTO?>>> GetCountryOptionAsync(GetCountryOptionRequestDTO dto)
        {
            try
            {
                await using var context = _contextFactory.CreateDbContext();

                if (dto.TodaysDate.HasValue)
                {
                    var date = dto.TodaysDate.Value.Date;
                    _logger.LogInformation("Filtering country for date: {Date}", date);
                }
                var countries = await context.Countries.AsNoTracking().ToListAsync();
                var getCountries = _mapper.Map<List<GetCountryOptionResponseDTO>>(countries);
                if (getCountries == null || getCountries.Count == 0)
                {
                    _logger.LogWarning("No active country found");
                    return new ApiResponse<List<GetCountryOptionResponseDTO?>>
                    {
                        IsSucceeded = false,
                        Message = "No country found.",
                        Data = new List<GetCountryOptionResponseDTO?>()
                    };
                }

                return new ApiResponse<List<GetCountryOptionResponseDTO?>>
                {
                    IsSucceeded = true,
                    Message = "country options fetched successfully.",
                    Data = getCountries
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching country options");

                return new ApiResponse<List<GetCountryOptionResponseDTO?>>
                {
                    IsSucceeded = false,
                    Message = "An error occurred while fetching country options.",
                    Data = new List<GetCountryOptionResponseDTO?>()
                };
            }
        }
        public async Task<ApiResponse<List<GetStateOptionResponseDTO?>>> GetStateOptionAsync(GetStateOptionRequestDTO dto)
        {
            try
            {
                await using var context = _contextFactory.CreateDbContext();

                // ✅ Validate CountryId
                if (dto.CountryId <= 0)
                {
                    _logger.LogWarning("Invalid CountryId provided: {CountryId}", dto.CountryId);
                    return new ApiResponse<List<GetStateOptionResponseDTO?>>
                    {
                        IsSucceeded = false,
                        Message = "Invalid CountryId. Please provide a valid CountryId.",
                        Data = new List<GetStateOptionResponseDTO?>()
                    };
                }

                // ✅ Query only active states belonging to given country
                var states = await context.States
                    .AsNoTracking()
                    .Where(s => s.CountryId == dto.CountryId && s.IsActive==true )
                    .ToListAsync();

                // ✅ Map to DTO using AutoMapper
                var mappedStates = _mapper.Map<List<GetStateOptionResponseDTO>>(states);

                // ✅ Handle empty data
                if (mappedStates == null || mappedStates.Count == 0)
                {
                    _logger.LogWarning("No active states found for CountryId: {CountryId}", dto.CountryId);
                    return new ApiResponse<List<GetStateOptionResponseDTO?>>
                    {
                        IsSucceeded = false,
                        Message = "No active states found for the selected state.",
                        Data = new List<GetStateOptionResponseDTO?>()
                    };
                }

                // ✅ Success response
                _logger.LogInformation("Successfully retrieved {Count} active states for CountryId: {CountryId}", mappedStates.Count, dto.CountryId);

                return new ApiResponse<List<GetStateOptionResponseDTO?>>
                {
                    IsSucceeded = true,
                    Message = "State options fetched successfully.",
                    Data = mappedStates
                };
            }
            catch (Exception ex)
            {
                // ✅ Error handling
                _logger.LogError(ex, "Error while fetching state options for CountryId: {CountryId}", dto.CountryId);

                return new ApiResponse<List<GetStateOptionResponseDTO?>>
                {
                    IsSucceeded = false,
                    Message = "An error occurred while fetching state options.",
                    Data = new List<GetStateOptionResponseDTO?>()
                };
            }
        }


        public async Task<ApiResponse<List<GetDistrictOptionResponseDTO?>>> GetDistrictOptionAsync(GetDistrictOptionRequestDTO dto)
        {
            try
            {
                await using var context = _contextFactory.CreateDbContext();

                // ✅ Step 1: Validate input
                if (dto.StateId <= 0)
                {
                    _logger.LogWarning("Invalid StateId provided: {StateId}", dto.StateId);
                    return new ApiResponse<List<GetDistrictOptionResponseDTO?>>
                    {
                        IsSucceeded = false,
                        Message = "Invalid StateId. Please provide a valid StateId.",
                        Data = new List<GetDistrictOptionResponseDTO?>()
                    };
                }

                // ✅ Step 2: Check if the state itself is active
                var isStateActive = await context.States
                    .AsNoTracking()
                    .AnyAsync(s => s.Id == dto.StateId && s.IsActive ==true);

                if (!isStateActive)
                {
                    _logger.LogWarning("StateId {StateId} is inactive or not found.", dto.StateId);
                    return new ApiResponse<List<GetDistrictOptionResponseDTO?>>
                    {
                        IsSucceeded = false,
                        Message = "The selected state is inactive or does not exist.",
                        Data = new List<GetDistrictOptionResponseDTO?>()
                    };
                }

                // ✅ Step 3: Fetch all active districts of that active state
                var districts = await context.Districts
                    .AsNoTracking()
                    .Where(d => d.StateId == dto.StateId && d.IsActive ==true)
                    .ToListAsync();

                // ✅ Step 4: Map to DTO
                var mappedDistricts = _mapper.Map<List<GetDistrictOptionResponseDTO>>(districts);

                // ✅ Step 5: Handle no data
                if (mappedDistricts == null || mappedDistricts.Count == 0)
                {
                    _logger.LogWarning("No active districts found for StateId: {StateId}", dto.StateId);
                    return new ApiResponse<List<GetDistrictOptionResponseDTO?>>
                    {
                        IsSucceeded = false,
                        Message = "No active districts found for the selected state.",
                        Data = new List<GetDistrictOptionResponseDTO?>()
                    };
                }

                // ✅ Step 6: Success response
                _logger.LogInformation("Successfully retrieved {Count} districts for StateId: {StateId}", mappedDistricts.Count, dto.StateId);
                return new ApiResponse<List<GetDistrictOptionResponseDTO?>>
                {
                    IsSucceeded = true,
                    Message = "District options fetched successfully.",
                    Data = mappedDistricts
                };
            }
            catch (Exception ex)
            {
                // ✅ Step 7: Error handling
                _logger.LogError(ex, "Error while fetching district options for StateId: {StateId}", dto.StateId);
                return new ApiResponse<List<GetDistrictOptionResponseDTO?>>
                {
                    IsSucceeded = false,
                    Message = "An error occurred while fetching district options.",
                    Data = new List<GetDistrictOptionResponseDTO?>()
                };
            }
        }



    }

}
