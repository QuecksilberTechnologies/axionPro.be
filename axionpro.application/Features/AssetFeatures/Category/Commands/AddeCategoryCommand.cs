using axionpro.application.DTOS.AssetDTO.category;
using axionpro.application.Wrappers;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace axionpro.application.Features.AssetFeatures.Category.Commands
{
    public class AddeCategoryCommand : IRequest<ApiResponse<List<GetCategoryResponseDTO>>>
    {
        public AddCategoryReqestDTO DTO { get; set; }

        public AddeCategoryCommand(AddCategoryReqestDTO dto)
        {
            DTO = dto;
        }
    }
}
