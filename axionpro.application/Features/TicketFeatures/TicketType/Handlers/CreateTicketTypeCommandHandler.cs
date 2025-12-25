using AutoMapper;
using axionpro.application.Constants;
using axionpro.application.DTOs.Leave;
using axionpro.application.DTOS.TicketDTO.TicketType;
using axionpro.application.Features.LeaveCmd.Commands;
 
using axionpro.application.Features.TicketFeatures.TicketType.Commands;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.IRepositories;
using axionpro.application.Wrappers;
using axionpro.domain.Entity;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.Features.TicketFeatures.TicketType.Handlers
{
    public class CreateTicketTypeCommandHandler : IRequestHandler<CreateTicketTypeCommand, ApiResponse<List<GetTicketTypeResponseDTO>>>
    {
        private readonly ITicketTypeRepository _repository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IStoreProcedureRepository _commonRepository;

        public CreateTicketTypeCommandHandler(ITicketTypeRepository repository,  IMapper mapper, IUnitOfWork unitOfWork, IStoreProcedureRepository commonRepository) // Inject CommonRepository
        {
            _repository = repository;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _commonRepository = commonRepository;
        }

        public async Task<ApiResponse<List<GetTicketTypeResponseDTO>>> Handle(CreateTicketTypeCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // 1️⃣ Validation
                if (request.DTO == null)
                {
                    return new ApiResponse<List<GetTicketTypeResponseDTO>>
                    {
                        IsSucceeded = false,
                        Message = "Invalid request. TicketType data is required.",
                        Data = new List<GetTicketTypeResponseDTO>()
                    };
                }

                

                if (request.DTO.TenantId <= 0)
                {
                    return new ApiResponse<List<GetTicketTypeResponseDTO>>
                    {
                        IsSucceeded = false,
                        Message = "ticket-type Id must be valid.",
                        Data = new List<GetTicketTypeResponseDTO>()
                    };
                }
 
               
                // 3️⃣ Repository Call
                List<GetTicketTypeResponseDTO> response = await _repository.AddAsync(request.DTO);
                if (response == null || !response.Any())
                {
                    return new ApiResponse<List<GetTicketTypeResponseDTO>>
                    {
                        IsSucceeded = false,
                        Message = "ticket-type creation failed.",
                        Data = new List<GetTicketTypeResponseDTO>()
                    };
                }
 
                return new ApiResponse<List<GetTicketTypeResponseDTO>>
                {
                    IsSucceeded = true,
                    Message = "ticket-type created successfully.",
                    Data = response.ToList()
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<GetTicketTypeResponseDTO>>
                {
                    IsSucceeded = false,
                    Message = $"An error occurred while creating ticket-type: {ex.Message}",
                    Data = new List<GetTicketTypeResponseDTO>()
                };
            }
        }


    }


}
