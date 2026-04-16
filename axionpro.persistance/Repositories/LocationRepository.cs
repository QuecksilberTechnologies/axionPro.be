using AutoMapper;
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
using System.Threading.Tasks;

namespace axionpro.persistance.Repositories
{
    public class LocationRepository : ILocationRepository
    {
        private readonly WorkforceDbContext _context;
        private readonly ILogger<LocationRepository> _logger;
        private readonly IMapper _mapper;

        public LocationRepository(
            WorkforceDbContext context,
            ILogger<LocationRepository> logger,
            IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
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
                if (dto.TodaysDate.HasValue)
                {
                    var date = dto.TodaysDate.Value.Date;
                    _logger.LogInformation("Filtering country for date: {Date}", date);
                }

                var countries = await _context.Countries
                    .AsNoTracking()
                    .Where(c => c.IsActive == true)
                    .Select(c => new GetCountryOptionResponseDTO
                    {
                        Id = c.Id,
                        CountryName = c.CountryName ?? string.Empty,
                        CountryCode = c.CountryCode ?? string.Empty,
                        STDCode = c.Stdcode ,
                        IsActive = c.IsActive
                    })
                    .ToListAsync();

                if (countries == null || countries.Count == 0)
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
                    Message = "Country options fetched successfully.",
                    Data = countries.Cast<GetCountryOptionResponseDTO?>().ToList()
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

                var states = await _context.States
                    .AsNoTracking()
                    .Where(s => s.CountryId == dto.CountryId && s.IsActive == true)
                    .Select(s => new GetStateOptionResponseDTO
                    {
                        Id = s.Id,
                        CountryId = s.CountryId,
                        CountryCode = s.Country != null ? (s.Country.CountryCode ?? string.Empty) : string.Empty,
                        STDCode = s.Country != null ? s.Country.Stdcode : string.Empty,
                        StateName = s.StateName ?? string.Empty,
                        IsActive = s.IsActive
                    })
                    .ToListAsync();

                if (states == null || states.Count == 0)
                {
                    _logger.LogWarning("No active states found for CountryId: {CountryId}", dto.CountryId);
                    return new ApiResponse<List<GetStateOptionResponseDTO?>>
                    {
                        IsSucceeded = false,
                        Message = "No active states found for the selected country.",
                        Data = new List<GetStateOptionResponseDTO?>()
                    };
                }

                _logger.LogInformation(
                    "Successfully retrieved {Count} active states for CountryId: {CountryId}",
                    states.Count,
                    dto.CountryId);

                return new ApiResponse<List<GetStateOptionResponseDTO?>>
                {
                    IsSucceeded = true,
                    Message = "State options fetched successfully.",
                    Data = states.Cast<GetStateOptionResponseDTO?>().ToList()
                };
            }
            catch (Exception ex)
            {
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

                var isStateActive = await _context.States
                    .AsNoTracking()
                    .AnyAsync(s => s.Id == dto.StateId && s.IsActive == true);

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

                var districts = await _context.Districts
                    .AsNoTracking()
                    .Where(d => d.StateId == dto.StateId && d.IsActive == true)
                    .Select(d => new GetDistrictOptionResponseDTO
                    {
                        Id = d.Id,
                        StateId = d.StateId,
                        CountryCode = d.State != null && d.State.Country != null
                            ? (d.State.Country.CountryCode ?? string.Empty)
                            : string.Empty,
                        DistrictName = d.DistrictName ?? string.Empty,
                        IsActive = d.IsActive
                    })
                    .ToListAsync();

                if (districts == null || districts.Count == 0)
                {
                    _logger.LogWarning("No active districts found for StateId: {StateId}", dto.StateId);
                    return new ApiResponse<List<GetDistrictOptionResponseDTO?>>
                    {
                        IsSucceeded = false,
                        Message = "No active districts found for the selected state.",
                        Data = new List<GetDistrictOptionResponseDTO?>()
                    };
                }

                _logger.LogInformation(
                    "Successfully retrieved {Count} districts for StateId: {StateId}",
                    districts.Count,
                    dto.StateId);

                return new ApiResponse<List<GetDistrictOptionResponseDTO?>>
                {
                    IsSucceeded = true,
                    Message = "District options fetched successfully.",
                    Data = districts.Cast<GetDistrictOptionResponseDTO?>().ToList()
                };
            }
            catch (Exception ex)
            {
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