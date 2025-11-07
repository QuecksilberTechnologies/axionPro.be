using AutoMapper;
using axionpro.application.Features.ClientCmd.Commands;
using axionpro.application.Interfaces.IRepositories;
using axionpro.application.Interfaces;
using axionpro.application.Wrappers;
using axionpro.domain.Entity;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using axionpro.application.Features.ClientCmd.Queries;

using axionpro.application.Features.LeaveCmd.Handlers;
using Microsoft.Extensions.Logging;
using axionpro.application.DTOs.Client;

namespace axionpro.application.Features.ClientCmd.Handlers
{
    internal class GetAllClientTypeQueryHandler : IRequestHandler<GetAllClientTypeQuery, ApiResponse<List<GetAllClientTypeDTO>>>
    {
       // private readonly IClientRepository _clienttypeRepository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GetAllLeaveRuleQueryHandler> _logger;



        public GetAllClientTypeQueryHandler(IMapper mapper, IUnitOfWork unitOfWork, ILogger<GetAllLeaveRuleQueryHandler> logger)      
        {
           
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }


        public async Task<ApiResponse<List<GetAllClientTypeDTO>>> Handle(GetAllClientTypeQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // ✅ Correcting the method call
                List<ClientType> clientTypes = await _unitOfWork.ClientsRepository.GetAllClientTypeAsync();

                //if (clientTypes == null || !clientTypes.Any())
                //{
                //    _logger.LogWarning("No clientTypes found.");
                //    return new ApiResponse<List<GetAllClientTypeDTO>>(false, "No clientTypes found", new List<GetAllClientTypeQuery>());
                //}

                //// ✅ Map Role entities to DTOs
                var getAllClientTypeDTOs = _mapper.Map<List<GetAllClientTypeDTO>>(clientTypes);

                _logger.LogInformation("Successfully retrieved {Count} roles.", getAllClientTypeDTOs.Count);
                return new ApiResponse<List<GetAllClientTypeDTO>>
                {
                    IsSucceeded = true,
                    Message = "Categories fetched successfully.",
                    Data = getAllClientTypeDTOs
                };
                
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching roles.");
                return new ApiResponse<List<GetAllClientTypeDTO>>
                {
                    IsSucceeded = false,
                    Message = "Categories fetched successfully.",
                    Data = null
                };
            }

        }
    }
}