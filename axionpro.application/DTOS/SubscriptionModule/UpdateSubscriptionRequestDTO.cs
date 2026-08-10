using System;
using System.Collections.Generic;
using System.Text;

namespace axionpro.application.DTOS.SubscriptionModule
{
    public class UpdateSubscriptionRequestDTO
    {
        public long Id { get; set; }

        public string PlanName { get; set; } = string.Empty;

        public int MaxUsers { get; set; }

        public bool IsMostPopular { get; set; }

        public bool IsCustom { get; set; }

        public string CurrencyKey { get; set; } = string.Empty;

        public decimal? PerDayPrice { get; set; }

        public bool IsFree { get; set; }

        public decimal? MonthlyPrice { get; set; }

        public decimal? YearlyPrice { get; set; }

        public bool IsActive { get; set; }

        
    }
}
