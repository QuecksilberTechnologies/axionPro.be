using AutoMapper;
using axionpro.application.DTOs.SubscriptionModule;
using axionpro.application.DTOS.SubscriptionModule;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Wrappers;
using axionpro.domain.Entity;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace axionpro.application.Features.SubscriptionCmd.Handlers
{
    public class UpdateSubscriptionPlanCommand
    : IRequest<ApiResponse<SubscriptionActivePlanDTO>>
    {
        public UpdateSubscriptionRequestDTO SubscriptionPlanRequestDTO { get; }

        public UpdateSubscriptionPlanCommand(UpdateSubscriptionRequestDTO dto)
        {
            SubscriptionPlanRequestDTO = dto;
        }
    }
    public class UpdateSubscriptionPlanCommandHandler
     : IRequestHandler<UpdateSubscriptionPlanCommand,
         ApiResponse<SubscriptionActivePlanDTO>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ICommonRequestService _commonRequestService;
        private readonly ILogger<UpdateSubscriptionPlanCommandHandler> _logger;

        public UpdateSubscriptionPlanCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ICommonRequestService commonRequestService,
            ILogger<UpdateSubscriptionPlanCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _commonRequestService = commonRequestService;
            _logger = logger;
        }

        public async Task<ApiResponse<SubscriptionActivePlanDTO>> Handle(
            UpdateSubscriptionPlanCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                var entity = _mapper.Map<SubscriptionPlan>(request.SubscriptionPlanRequestDTO);

                entity = await _unitOfWork.SubscriptionRepository
                    .UpdateSubscriptionPlanAsync(entity);

                var dto = _mapper.Map<SubscriptionActivePlanDTO>(entity);

                return ApiResponse<SubscriptionActivePlanDTO>.Success(
                    dto,
                    "Subscription plan updated successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error while updating subscription plan. Id : {Id}",
                    request.SubscriptionPlanRequestDTO.Id);

                throw;
            }
        }
    }
    }
 
