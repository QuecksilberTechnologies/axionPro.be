using axionpro.application.DTOs.Category;
using axionpro.application.Wrappers;
using axionpro.domain.Entity; using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks; using axionpro.domain.Entity; using MediatR;

namespace axionpro.application.Features.CategoryCmd.Command
{
    public class GetTenderMainCategoryCommand :IRequest<ApiResponse<List<TenderCategoryResponseDTO>>>
    {
        public TenderCategoryRequestDTO TenderCategoryRequestDTO { get; set; }


        public GetTenderMainCategoryCommand(TenderCategoryRequestDTO tenderCategoryRequestDTO)
        {
            TenderCategoryRequestDTO = tenderCategoryRequestDTO;
        }
    }
}
