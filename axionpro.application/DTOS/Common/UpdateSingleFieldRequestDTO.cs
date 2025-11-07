using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOs.Common
{
    public class UpdateSingleFieldRequestDTO
    {
         
            public string TableName { get; set; } = string.Empty;
            public long EmployeeId { get; set; }
            public string FieldName { get; set; } = string.Empty;
            public object? NewValue { get; set; }
            public long UpdatedById { get; set; }
         
    }
}
