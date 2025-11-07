using AutoMapper;
using axionpro.application.DTOS.Common;
using axionpro.application.DTOS.Department;
using axionpro.application.DTOS.Gender;
using axionpro.application.Interfaces.IRepositories;
using axionpro.application.Wrappers;
using axionpro.domain.Entity;
using axionpro.persistance.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace axionpro.persistance.Repositories
{
        public class GenderRepository : IGenderRepository
        {
            private readonly WorkforceDbContext _context;
            private readonly IDbContextFactory<WorkforceDbContext> _contextFactory;
            private readonly ILogger<GenderRepository> _logger;
            private readonly IMapper _mapper;

            public GenderRepository(WorkforceDbContext context, ILogger<GenderRepository> logger, IMapper mapper, IDbContextFactory<WorkforceDbContext> contextFactory)
            {
                _context = context ?? throw new ArgumentNullException(nameof(context));
                _logger = logger ?? throw new ArgumentNullException(nameof(logger));
             _mapper = mapper ?? throw new ArgumentNullException(nameof(_mapper));
            _contextFactory =  contextFactory?? throw new ArgumentNullException(nameof(contextFactory));
            }

            public Task AddAsync(Gender gender)
            {
                throw new NotImplementedException();
            }

            public Task DeleteAsync(long id)
            {
                throw new NotImplementedException();
            }

            public async Task<IEnumerable<Gender>> GetAllAsync()
            {
                try
                {
                    _logger.LogInformation("Fetching all genders from database...");

                    var genders = await _context.Genders.ToListAsync();

                    if (genders == null || genders.Count == 0)
                    {
                        _logger.LogWarning("No genders found in database.");
                    }
                    else
                    {
                        _logger.LogInformation("Successfully fetched {Count} genders.", genders.Count);
                    }

                    return genders;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while fetching genders.");
                    throw; // service layer tak propagate karega
                }
            }


            public Task<Gender> GetByIdAsync(long id)
            {
                throw new NotImplementedException();
            }

        public async Task<ApiResponse<List<GetGenderOptionResponseDTO?>>> GetOptionAsync(GetOptionRequestDTO dto)
        {
            try
            {
                await using var context = _contextFactory.CreateDbContext();

                if (dto.TodaysDate.HasValue)
                {
                    var date = dto.TodaysDate.Value.Date;
                    _logger.LogInformation("Filtering gender for date: {Date}", date);
                }
                var genders = await context.Genders.AsNoTracking().ToListAsync();
                var getGender = _mapper.Map<List<GetGenderOptionResponseDTO>>(genders);
                if (getGender == null || getGender.Count == 0)
                {
                    _logger.LogWarning("No active departments found");
                    return new ApiResponse<List<GetGenderOptionResponseDTO?>>
                    {
                        IsSucceeded = false,
                        Message = "No Gender found.",
                        Data = new List<GetGenderOptionResponseDTO?>()
                    };
                }

                return new ApiResponse<List<GetGenderOptionResponseDTO?>>
                {
                    IsSucceeded = true,
                    Message = "Gender options fetched successfully.",
                    Data = getGender
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching gender options");

                return new ApiResponse<List<GetGenderOptionResponseDTO?>>
                {
                    IsSucceeded = false,
                    Message = "An error occurred while fetching gender options.",
                    Data = new List<GetGenderOptionResponseDTO?>()
                };
            }
        }

        public Task UpdateAsync(Gender gender)
            {
                throw new NotImplementedException();
            }
        }
    }
