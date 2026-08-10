using AutoMapper;
using axionpro.application.DTOs.SubscriptionModule;
using axionpro.application.DTOS.SubscriptionModule;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Interfaces.IRepositories;
using axionpro.application.Wrappers;
using axionpro.domain.Entity;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace axionpro.application.Features.SubscriptionCmd.Handlers
{
    public class CreateSubscriptionPlanCommand
     : IRequest<ApiResponse<SubscriptionActivePlanDTO>>
    {
        public CreateSubscriptionRequestDTO SubscriptionPlanRequestDTO { get; }

        public CreateSubscriptionPlanCommand(CreateSubscriptionRequestDTO dto)
        {
            SubscriptionPlanRequestDTO = dto;
        }
    }

    public class CreateSubscriptionPlanCommandHandler
    : IRequestHandler<CreateSubscriptionPlanCommand,
        ApiResponse<SubscriptionActivePlanDTO>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICommonRequestService _commonRequestService;
        private readonly ILogger<CreateSubscriptionPlanCommandHandler> _logger;
        private readonly IMapper _mapper;
        public CreateSubscriptionPlanCommandHandler(
            IUnitOfWork unitOfWork,
             IMapper mapper,
            ICommonRequestService commonRequestService,
            ILogger<CreateSubscriptionPlanCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _commonRequestService = commonRequestService;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ApiResponse<SubscriptionActivePlanDTO>> Handle(
            CreateSubscriptionPlanCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                var entity = _mapper.Map<SubscriptionPlan>(request.SubscriptionPlanRequestDTO);

                var result = await _unitOfWork.SubscriptionRepository.AddSubscriptionPlanAsync(entity);
                var dto = _mapper.Map<SubscriptionActivePlanDTO>(entity);
                return ApiResponse<SubscriptionActivePlanDTO>.Success(dto,"Subscription plan created successfully.");
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }
    }
}
