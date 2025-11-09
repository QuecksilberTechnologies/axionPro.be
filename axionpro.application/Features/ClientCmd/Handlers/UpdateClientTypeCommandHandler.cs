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
using axionpro.application.DTOs.Client;

namespace axionpro.application.Features.ClientCmd.Handlers
{
    public class UpdateClientTypeCommandHandler : IRequestHandler<UpdateClientTypeCommand, ApiResponse<List<GetClientTypeDTO>>>
    {
        private readonly IClientRepository _ClientRepository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateClientTypeCommandHandler(IClientRepository ClientRepository, IMapper mapper, IUnitOfWork unitOfWork)
        {
            _ClientRepository = ClientRepository;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }
        public async Task<ApiResponse<List<GetClientTypeDTO>>> Handle(UpdateClientTypeCommand request, CancellationToken cancellationToken)
        {
            try
            {
                ClientType ClienttypeEntity = _mapper.Map<ClientType>(request.updateClientTypeCommand);
                List<ClientType> ClientTypes = await _ClientRepository.UpdateClientTypeAsync(ClienttypeEntity);

                if (ClientTypes == null || !ClientTypes.Any())
                {
                    return new ApiResponse<List<GetClientTypeDTO>>
                    {
                        IsSucceeded = false,
                        Message = "No Client were update.",
                        Data = new List<GetClientTypeDTO>()
                    };
                }

                List<GetClientTypeDTO> ClientTypeDTOs = _mapper.Map<List<GetClientTypeDTO>>(ClientTypes);

                return new ApiResponse<List<GetClientTypeDTO>>
                {
                    IsSucceeded = true,
                    Message = "Client update successfully",
                    Data = ClientTypeDTOs
                };
            }
            catch (Exception ex)
            {
                //  _logger.LogError(ex, "Error occurred while creating role.");
                return new ApiResponse<List<GetClientTypeDTO>>

                {
                    IsSucceeded = false,
                    Message = $"An error occurred: {ex.Message}",
                    Data = null
                };
            }
        }


    }


}