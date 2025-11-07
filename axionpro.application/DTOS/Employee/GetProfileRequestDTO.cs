using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOs.Employee
{
    /// <summary>
    /// post-request get self profile info
    /// </summary>

    public class GetProfileRequestDTO
    {
        /// <summary> self user login id Required</summary>

        [Required]
        public string SelfLoginId { get; set; }
    }
}
