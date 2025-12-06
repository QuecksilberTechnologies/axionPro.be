using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOS.Location
{
    public class GetCountryOptionResponseDTO 
    {
        public int Id { get; set; }

        public string CountryName { get; set; } = null!;
        public string CountryCode { get; set; } = null!;

        public bool? IsActive { get; set; }
    }
    public class GetStateOptionResponseDTO
    {
        public int Id { get; set; }
        public int CountryId { get; set; }

        public string StateName { get; set; } = null!;

        public bool? IsActive { get; set; }
    }

    public class GetDistrictOptionResponseDTO
    {
        public int Id { get; set; }
        public int StateId { get; set; }

        public string DistrictName { get; set; } = null!;

        public bool? IsActive { get; set; }
    }

}
