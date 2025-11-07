using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOs.Operation
{
    public class OperationDTO
    {
        public int Id { get; set; }
        public string OperationName { get; set; }
        public string Remark { get; set; }
        public bool IsActive { get; set; }
        public int AddedById { get; set; }
        public DateTime AddedDateTime { get; set; }
        public int? UpdatedById { get; set; }
        public DateTime? UpdateDateTime { get; set; }
    }

}
