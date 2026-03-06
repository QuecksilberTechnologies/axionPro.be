using System;
using System.Collections.Generic;

namespace axionpro.domain.Entity;

public partial class PageTypeEnum
{
    public int Id { get; set; }

    public string? PageTypeName { get; set; }

    public virtual ICollection<ModuleOperationMapping> ModuleOperationMappings { get; set; } = new List<ModuleOperationMapping>();
}
