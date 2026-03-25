using System;
using System.Collections.Generic;

namespace axionpro.domain.Entity;

public partial class EmployeeExperienceDocument
{
   
        // ---------------------------------------------------------
        // 🔹 Primary Key
        // ---------------------------------------------------------
        public long Id { get; set; }

        // ---------------------------------------------------------
        // 🔹 Foreign Key
        // ---------------------------------------------------------
        public long EmployeeExperienceDetailId { get; set; }

        // ---------------------------------------------------------
        // 🔹 Document Info
        // ---------------------------------------------------------
        public int DocumentType { get; set; }   // 🔥 ENUM use karenge
        public string? FileName { get; set; }
        public string? FilePath { get; set; }

        public bool IsUploaded { get; set; } = true;

        public string? Remark { get; set; }

        // ---------------------------------------------------------
        // 🔹 Audit Fields
        // ---------------------------------------------------------
        public bool IsActive { get; set; } = true;
        public bool IsSoftDeleted { get; set; } = false;

        public long AddedById { get; set; }
        public DateTime AddedDateTime { get; set; }

        public long? UpdatedById { get; set; }
        public DateTime? UpdatedDateTime { get; set; }

        // ---------------------------------------------------------
        // 🔹 Navigation
        // ---------------------------------------------------------
        public virtual EmployeeExperienceDetail EmployeeExperienceDetail { get; set; } = null!;
    
}
