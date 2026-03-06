using System;
using System.Collections.Generic;

namespace axionpro.domain.Entity;

public partial class ClientType
{
    public int Id { get; set; }

    public string TypeName { get; set; } = null!;

    public bool IsActive { get; set; }

    public string? Remark { get; set; }

    public string? Description { get; set; }

    public virtual ICollection<Client> Clients { get; set; } = new List<Client>();

    public virtual ICollection<Tender> Tenders { get; set; } = new List<Tender>();
}
