using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOs.SubscriptionModule
{
    public class SubscriptionPlanRequestDTO
    {
        public long? TenantId { get; set; }
        public string? PlanName { get; set; }              // Example: Basic, Pro, Premium
        public int? MaxUsers { get; set; }                // Optional: Maximum number of users allowed
        public decimal? PerDayPrice { get; set; }         // Optional: Price per day
        public decimal? MonthlyPrice { get; set; }        // Optional: Price per month
        public decimal? YearlyPrice { get; set; }         // Optional: Price per year
        public bool IsActive { get; set; } = true;        // Default: Active

    }
}