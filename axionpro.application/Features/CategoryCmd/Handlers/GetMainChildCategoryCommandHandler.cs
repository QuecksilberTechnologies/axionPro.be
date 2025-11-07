using AutoMapper;
using axionpro.application.DTOs.Category;
using axionpro.application.Features.CategoryCmd.Command;
using axionpro.application.Interfaces;
using axionpro.application.Wrappers;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.Features.CategoryCmd.Handlers
{
    public class GetMainCategoryChildRequestCommandHandler : IRequestHandler<GetMainChildCategoryCommand, ApiResponse<List<CategoryResponseDTO>>>
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GetMainCategoryCommandHandler> _logger;

        public GetMainCategoryChildRequestCommandHandler(IMapper mapper, IUnitOfWork unitOfWork, ILogger<GetMainCategoryCommandHandler> logger)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<ApiResponse<List<CategoryResponseDTO>>> Handle(GetMainChildCategoryCommand request, CancellationToken cancellationToken)
        {
            try
            {

                // Validate the request
                if (request == null || request.CategoryRequestDTO == null || request.CategoryRequestDTO.CategoryId == 0)
                {
                    return new ApiResponse<List<CategoryResponseDTO>>
                    {
                        IsSucceeded = false,
                        Message = "Invalid request. Category ID is required.",
                        Data = null
                    };
                }

                var categoryRequestDto = request.CategoryRequestDTO;

                // Validate user authorization
                //if (!await _unitOfWork.UserLoginReopsitory.IsValidUserAsync(categoryRequestDto.Id))
                //{
                //    return new ApiResponse<List<CategoryResponseDTO>>
                //    {
                //        IsSuccecced = false,
                //        Message = "User is not authenticated or authorized to perform this action.",
                //        Data = null
                //    };
                //}

        //        fetch all main categories(where parentcategoryid is null)
                var categories = await _unitOfWork.CategoryRepository.GetAllChildCategoryByIdAsync(categoryRequestDto.CategoryId, categoryRequestDto.CategoryId);

            //    map the domain model to the response dto
                var categoryresponsedtos = _mapper.Map<List<CategoryResponseDTO>>(categories);

              
                return new ApiResponse<List<CategoryResponseDTO>>
                {
                    IsSucceeded = true,
                    Message = "Categories fetched successfully.",
                    Data = categoryresponsedtos
                };
            }
            catch (Exception ex)
            {
                // Log the error
                _logger.LogError(ex, "An error occurred while processing the category request.");

                // Return a failure response
                return new ApiResponse<List<CategoryResponseDTO>>
                {
                    IsSucceeded = false,
                    Message = "An error occurred while processing the category request.",
                    Data = null
                };
            }
        }

       
    }

}
