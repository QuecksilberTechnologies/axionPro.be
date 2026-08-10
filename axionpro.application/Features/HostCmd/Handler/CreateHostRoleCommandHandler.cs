using AutoMapper;
using axionpro.application.DTOS.Host;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Interfaces.IHashed;
using axionpro.application.Wrappers;
using axionpro.domain.Entity;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace axionpro.application.Features.HostCmd.Handler
{
    public class CreateHostRoleCommand
     : IRequest<ApiResponse<CreateHostRoleResponseDTO>>
    {
        public CreateHostRoleRequestDTO DTO { get; }

        public CreateHostRoleCommand(CreateHostRoleRequestDTO dto)
        {
            DTO = dto;
        }
    }
    public class CreateHostRoleCommandHandler
       : IRequestHandler<
           CreateHostRoleCommand,
           ApiResponse<CreateHostRoleResponseDTO>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICommonRequestService _commonRequestService;
        private readonly ILogger<CreateHostRoleCommandHandler> _logger;
        private readonly IMapper _mapper;

        public CreateHostRoleCommandHandler(

            IUnitOfWork unitOfWork,
            IMapper mapper,
            ICommonRequestService commonRequestService,
            ILogger<CreateHostRoleCommandHandler> logger,
            IPasswordService passwordService
             )
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _commonRequestService = commonRequestService;
            _logger = logger;
        }

        public async Task<ApiResponse<CreateHostRoleResponseDTO>> Handle( CreateHostRoleCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                // Map Request DTO -> Entity
                var entity = _mapper.Map<HostRole>(request.DTO);

                // Default values
                entity.IsActive = true;
                entity.IsSoftDeleted = false;
                entity.AddedDateTime = DateTime.UtcNow;

                // Create Host Role
                var result = await _unitOfWork.HostRoleRepository
                    .AddAsync(entity);

                // Prepare Response
                var response =
                    _mapper.Map<CreateHostRoleResponseDTO>(entity);

                //// Get permissions assigned to this role
                //var permissions =
                //    await _unitOfWork.HostRoleRepository
                //        .GetRolePermissionsAsync(entity.Id);

                //response.Permissions = permissions;
 

                return ApiResponse<CreateHostRoleResponseDTO>.Success(
                    response,
                    "Host role created successfully.");
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();

                _logger.LogError(
                    ex,
                    "Error while creating Host Role.");

                throw;
            }
        }
    }
}
