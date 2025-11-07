using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
 

namespace axionpro.application.Interfaces.IHashed
{
   
        public interface IPasswordService
        {
            string HashPassword(string password);
            bool  VerifyPassword(string hashedPassword, string enteredPassword);
        }

    
}
