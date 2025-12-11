using AutoMapper;
using axionpro.application.DTOs.Department;
using axionpro.application.DTOS.Common;
using axionpro.application.Features.DepartmentCmd.Handlers;
using axionpro.application.Interfaces;
using axionpro.application.Wrappers;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOS.Department
{

    public class GetDepartmentOptionResponse 
    {

        public  int  Id { get; set; }
        public string DepartmentName { get; set; } = string.Empty;
       // public bool IsActive { get; set; }
    
    }

}
