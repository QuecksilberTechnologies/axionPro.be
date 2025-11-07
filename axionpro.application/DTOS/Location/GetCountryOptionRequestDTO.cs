using axionpro.application.DTOS.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOS.Location
{
    public class GetCountryOptionRequestDTO:GetOptionRequestDTO
    {
         
    }
    public class GetStateOptionRequestDTO : GetCountryOptionRequestDTO
    {       

        public required int   CountryId { get; set; }

    }

    public class GetDistrictOptionRequestDTO : GetOptionRequestDTO
    {

        public required int StateId { get; set; }

    }
}
