using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOS.InsurancePolicy
{
    /// <summary>
    /// Create Insurance Policy request DTO
    /// (Audit fields are handled internally)
    /// </summary>
    public class DeleteInsurancePolicyRequestDTO
    {
        [Required]
        public int Id { get; set; }          // FK → PolicyType
 
    }
}

