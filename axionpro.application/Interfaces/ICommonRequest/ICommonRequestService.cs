using axionpro.application.Common.Models.Security;
using axionpro.application.DTOS.Common;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.Interfaces.IRequestValidation
{
   
        public interface ICommonRequestService
        {
            Task<CommonDecodedResult> ValidateRequestAsync(string encodedUserId);
        }

    

  

}
