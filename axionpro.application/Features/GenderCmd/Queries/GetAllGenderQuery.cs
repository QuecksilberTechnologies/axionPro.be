using axionpro.application.DTOs.Gender;
using axionpro.application.DTOs.Leave;
using axionpro.application.Wrappers;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.Features.GenderCmd.Queries
{
    public class GetAllGenderQuery : IRequest<ApiResponse<List<GetGenderResponseDTO>>>
    {
        public GetGenderRequestDTO DTO { get; set; }

        public GetAllGenderQuery(GetGenderRequestDTO dTO)
        {
            DTO = dTO;
        }
    }
}
