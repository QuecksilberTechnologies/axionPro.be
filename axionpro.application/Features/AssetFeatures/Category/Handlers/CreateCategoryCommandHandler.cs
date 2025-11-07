using AutoMapper;
using axionpro.application.DTOS.AssetDTO.category;
using axionpro.application.Features.AssetFeatures.Category.Commands;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.IRepositories;
using axionpro.application.Wrappers;
using axionpro.domain.Entity;
using MediatR;
using Microsoft.Extensions.Logging;

namespace axionpro.application.Features.AssetFeatures.Category.Handlers
{
    public class CreateCategoryCommandHandler: IRequestHandler<AddeCategoryCommand, ApiResponse<List<GetCategoryResponseDTO>>>
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CreateCategoryCommandHandler> _logger;
 
        public CreateCategoryCommandHandler(
            IMapper mapper,
            IUnitOfWork unitOfWork,
            ILogger<CreateCategoryCommandHandler> logger)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _logger = logger;
          

        }

        public async Task<ApiResponse<List<GetCategoryResponseDTO>>> Handle( AddeCategoryCommand request,  CancellationToken cancellationToken)
        {
            try
            {
                if (request?.DTO == null)
                {
                    return new ApiResponse<List<GetCategoryResponseDTO>>
                    {
                        IsSucceeded = false,
                        Message = "Invalid request or missing asset category data.",
                        Data = null
                    };
                }

                        
              

                // Add asset category and fetch updated list
               var dto = await _unitOfWork.AssetCategoryRepository.AddAsync(request.DTO);

               

                return new ApiResponse<List<GetCategoryResponseDTO>>
                {
                    IsSucceeded = true,
                    Message = "Asset category created successfully.",
                    Data = dto
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while creating asset category.");
                return new ApiResponse<List<GetCategoryResponseDTO>>
                {
                    IsSucceeded = false,
                    Message = "An error occurred while processing the request.",
                    Data = null
                };
            }
        }
    }

}
