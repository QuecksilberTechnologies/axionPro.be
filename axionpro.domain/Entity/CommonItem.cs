using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.domain.Entity
{
    [Keyless]
    public class CommonItem
    {
        public int SubModuleId { get; set; }
        public string SubModuleName { get; set; }
        public string SubModuleURL { get; set; }

        [Column("SubModule")] // Stored procedure ke JSON column ka sahi naam
        public string SubModuleJson { get; set; }

        public List<ChildModule>? ChildModules =>
            string.IsNullOrEmpty(SubModuleJson)
                ? new List<ChildModule>()
                : JsonConvert.DeserializeObject<List<ChildModule>>(SubModuleJson);
    }

    public class ChildModule
    {
        public int ChildModuleId { get; set; }
        public string ChildModuleName { get; set; }
        public string ChildModuleURL { get; set; }
        public int Priority { get; set; }
        public List<Operation> Operations { get; set; } = new List<Operation>();


    }


    [Keyless]

 
    public class RoleModulePermission
    {
        //public int ModuleId { get; set; }
        //public string Module { get; set; }
        //public byte[] ModuleIcon { get; set; }  // VARBINARY(MAX) ke liye byte[] 
        //public byte[] ChildModuleIcon { get; set; }  // VARBINARY(MAX) ke liye byte[] 
        //public byte[] SubModuleIcon { get; set; }  // VARBINARY(MAX) ke liye byte[] 
        //public string ModuleURL { get; set; }
        //public int SubModuleId { get; set; }
        //public string SubModule { get; set; }
        //public string SubModuleURL { get; set; }
        //public int ChildModuleId { get; set; }
        //public string ChildModule { get; set; }
        //public string ChildModuleURL { get; set; }
        //public int OperationId { get; set; }
        //public string Operation { get; set; }
        //public string CompletePath { get; set; }
        public int ModuleId { get; set; }
        public string Module { get; set; }
        public string ModuleURL { get; set; }
        public string? ModuleIcon { get; set; }
        public int SubModuleId { get; set; }
        public string? SubModuleIcon { get; set; }
        public string SubModule { get; set; }
        public string SubModuleURL { get; set; }
        public string ChildModuleURL { get; set; }
        public int ChildModuleId { get; set; }
        public string ChildModule { get; set; }  
        public string? ChildModuleIcon { get; set; }
        public int OperationId { get; set; }
        public string Operation { get; set; }
        // public List<Operation> Operations { get; set; } = new();
    }
















}
