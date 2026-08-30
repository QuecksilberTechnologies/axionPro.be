using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks; using axionpro.domain.Entity; using MediatR;

using axionpro.application.DTOs.BaseDTO;

namespace axionpro.application.DTOS.Pagination
{
   
        public class BaseRequest
        {
        public int PageNumber { get; set; }
        public int PageSize { get; set; } 
        public string? SortBy { get; set; }
        public string? SortOrder { get; set; } = "desc";
        public string? UserEmployeeId { get; set; } 
 
      

      }

        /// <summary>
        /// Provides paging fields together with the module-operation context
        /// required by tenant runtime authorization.
        /// </summary>
        public class PermissionPagedRequestDTO : PermissionRequestDTO
        {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string? SortBy { get; set; }
        public string? SortOrder { get; set; } = "desc";
        public string? UserEmployeeId { get; set; }
        }



}
