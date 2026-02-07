using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOS.CompanyPolicyDocument
{
    public class UpdateCompanyPolicyDocumentRequestDTO
    {         
                      
            public int Id { get; set; }  //Haan
            public string? DocumentTitle { get; set; } = null!;  //Nahi 
            public string? FileName { get; set; } = null!;    //Nahi
            public string? FilePath { get; set; } = null!;  //Nahi       
              
            public bool? IsActive { get; set; } = true;
       
         

    }
}
