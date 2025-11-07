using axionpro.application.DTOS.Module.ParentModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOs.SubscriptionModule
{
    public class SubscriptionPlanResponseDTO
    {
        public int? Id { get; set; }

        public string? PlanName { get; set; } = null!;

        public int? MaxUsers { get; set; }
        
        public bool IsMostPopular { get; set; }
        public bool IsCustom { get; set; }
        public string? CurrencyKey { get; set; }

        public decimal? PerDayPrice { get; set; }

        public bool? IsFree { get; set; }

        public decimal? MonthlyPrice { get; set; }

        public decimal? YearlyPrice { get; set; }

        public bool? IsActive { get; set; }
        // ✅ Mapped Modules List
        public List<ModuleResponseDTO> Modules { get; set; } = new()
            ;



    }
     



}
