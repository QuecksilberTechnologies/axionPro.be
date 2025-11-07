using AutoMapper;
using axionpro.application.Features.TransportCmd.Commands;
using axionpro.application.Interfaces.IRepositories;
using axionpro.application.Interfaces;
using axionpro.application.Wrappers;
using axionpro.domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MediatR;
using axionpro.application.DTOs.Transport;


namespace axionpro.application.Features.TransportCmd.Handlers
{
    public class CreateTravelModeTypeCommandHandler :IRequestHandler<CreateTravelModeTypeCommand, ApiResponse<List<GetAllTravelModeDTO>>>
    {
        private readonly ITravelRepository travelRepository;
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

    public CreateTravelModeTypeCommandHandler(ITravelRepository travelRepository, IMapper mapper, IUnitOfWork unitOfWork)
    {
        this.travelRepository = travelRepository;
        _mapper = mapper;
        _unitOfWork = unitOfWork;
    }
    public async Task<ApiResponse<List<GetAllTravelModeDTO>>> Handle(CreateTravelModeTypeCommand request, CancellationToken cancellationToken)
    {
        try
        {
                 
            TravelMode travelMode = _mapper.Map<TravelMode>(request.createTravelModeDTO);
            List<TravelMode> travelModes = await travelRepository.CreateTravelTypeAsync(travelMode);

            if (travelModes == null || !travelModes.Any())
            {
                return new ApiResponse<List<GetAllTravelModeDTO>>
                {
                    IsSucceeded = false,
                    Message = "No Travel were created.",
                    Data = new List<GetAllTravelModeDTO>()
                };
            }

            List<GetAllTravelModeDTO> getAllTravelModeDTOs = _mapper.Map<List<GetAllTravelModeDTO>>(travelModes);

            return new ApiResponse<List<GetAllTravelModeDTO>>
            {
                IsSucceeded = true,
                Message = "Travel created successfully",
                Data = getAllTravelModeDTOs
            };
        }
        catch (Exception ex)
        {
            //  _logger.LogError(ex, "Error occurred while creating role.");
            return new ApiResponse<List<GetAllTravelModeDTO>>

            {
                IsSucceeded = false,
                Message = $"An error occurred: {ex.Message}",
                Data = null
            };
        }
    }


}


}
