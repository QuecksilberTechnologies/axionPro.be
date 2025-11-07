using AutoMapper;
using axionpro.application.DTOs.Category;
 
using axionpro.application.DTOs.UserLogin;
using axionpro.application.Features.CategoryCmd.Command;
using axionpro.application.Features.UserLoginAndDashboardCmd.Commands;
using axionpro.application.Features.UserLoginAndDashboardCmd.Handlers;
using axionpro.application.Interfaces;
using axionpro.application.Wrappers;
using axionpro.domain.Entity;
using MediatR;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.Features.CategoryCmd.Handlers
{


    public class GetMainCategoryCommandHandler : IRequestHandler<GetMainCategoryCommand, ApiResponse<List<CategoryResponseDTO>>>
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GetMainCategoryCommandHandler> _logger;

        public GetMainCategoryCommandHandler(IMapper mapper, IUnitOfWork unitOfWork, ILogger<GetMainCategoryCommandHandler> logger)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<ApiResponse<List<CategoryResponseDTO>>> Handle(GetMainCategoryCommand request, CancellationToken cancellationToken)
        {
            try
            {
              int Id =  Convert.ToInt32(request.CategoryRequestDTO.CategoryId);
             
                // Validate the request
                if (request == null || request.CategoryRequestDTO == null || Id == 0)
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

                // Fetch all main categories (where ParentCategoryId is NULL)
                  var categories = await _unitOfWork.CategoryRepository.GetAllMainCategoriesAsync();

                // Map the domain model to the response DTO
                  var categoryResponseDTOs = _mapper.Map<List<CategoryResponseDTO>>(categories);

                // Return a success response
                return new ApiResponse<List<CategoryResponseDTO>>
                {
                    IsSucceeded = true,
                    Message = "Categories fetched successfully.",
                    Data = categoryResponseDTOs
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
