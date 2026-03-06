using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks; using axionpro.domain.Entity; using MediatR;

namespace axionpro.application.DTOS.InsurancePoliciesMapping
{
    /// <summary>
    /// Create Insurance Policy request DTO
    /// (Audit fields are handled internally)
    /// </summary>
    public class DeletePolicyTypeInsuranceMappingRequestDTO
    {
        [Required]
        public int Id { get; set; }          // FK → PolicyType
 
    }
}

