using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOs.Operation
{
    public class UpdateTenantEnabledOperationFromModuleOperationResponseDTO
    {
        /// <summary>
        /// -1 = Error, 0 = Nothing inserted, >0 = Rows inserted
        /// </summary>
        public int Result { get; set; }

        public string Message
        {
            get
            {
                return Result switch
                {
                    -1 => "An error occurred during the operation.",
                    0 => "No new operations were inserted.",
                    > 0 => $"{Result} operations were successfully inserted.",
                    _ => "Unknown result."
                };
            }
        }
    }

}
