using AutoMapper;
 
using axionpro.application.DTOS.Host;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Interfaces.IHashed;
using axionpro.application.Wrappers;
using axionpro.domain.Entity;
using MediatR;
using Microsoft.Extensions.Logging;

namespace axionpro.application.Features.HostCmd.Handler
{
    public class CreateHostUserCommand  : IRequest<ApiResponse<CreateHostUserResponseDTO>>
    {
        public CreateHostUserRequestDTO DTO { get; }

        public CreateHostUserCommand(CreateHostUserRequestDTO dto)
        {
            DTO = dto;
        }
    }
    public class CreateHostUserCommandHandler
        : IRequestHandler<
            CreateHostUserCommand,
            ApiResponse<CreateHostUserResponseDTO>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICommonRequestService _commonRequestService;
        private readonly ILogger<CreateHostUserCommandHandler> _logger;
        private readonly IMapper _mapper;
        private readonly IPasswordService _passwordService;

        public CreateHostUserCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ICommonRequestService commonRequestService,
            ILogger<CreateHostUserCommandHandler> logger,
            IPasswordService passwordService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _commonRequestService = commonRequestService;
            _logger = logger;
            _passwordService = passwordService;
        }

        public async Task<ApiResponse<CreateHostUserResponseDTO>> Handle(
            CreateHostUserCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                // Create Entity
                var entity = _mapper.Map<HostUser>(request.DTO);

                // Password Hash
                entity.PasswordHash =
                    _passwordService.HashPassword(request.DTO.Password);

                // Default Values
                entity.IsActive = true;
                entity.IsSoftDeleted = false;
                entity.AddedDateTime = DateTime.UtcNow;

                // Add Host User
                var result = await _unitOfWork.HostUserRepository.AddAsync(entity);

                await _unitOfWork.SaveChangesAsync();

                //// Get Role + Permissions
                //var permissions = await _unitOfWork.HostUserRepository
                //    .GetHostUserPermissionsAsync(
                //        entity.HostRoleId);

                // Prepare Response
                var response = _mapper.Map<CreateHostUserResponseDTO>(entity);

                //response.RoleName = permissions.RoleName;

                //response.Permissions = permissions.Permissions;

                await _unitOfWork.CommitTransactionAsync();

                return ApiResponse<CreateHostUserResponseDTO>
                    .Success(
                        response,
                        "Host user created successfully.");
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();

                _logger.LogError(
                    ex,
                    "Error while creating Host User.");

                throw;
            }
        }
    }
}