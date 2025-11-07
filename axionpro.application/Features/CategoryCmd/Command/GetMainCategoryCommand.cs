 
using axionpro.application.DTOs.Category;
using axionpro.application.Wrappers;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.Features.CategoryCmd.Command
{
    public class GetMainCategoryCommand : IRequest<ApiResponse<List<CategoryResponseDTO>>>
    {
        public CategoryRequestDTO CategoryRequestDTO { get; set; }


        public GetMainCategoryCommand(CategoryRequestDTO categoryRequestDTO)
        {
            CategoryRequestDTO = categoryRequestDTO;
        }

    }

}