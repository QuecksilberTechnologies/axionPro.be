using System;
using System.Collections.Generic;

namespace axionpro.domain.Entity;

public partial class TenderClient
{
    public int Id { get; set; }

    public string ClientName { get; set; } = null!;

    public string? ContactPerson { get; set; }

    public string? Email { get; set; }

    public string? PhoneNumber { get; set; }

    public string? Address { get; set; }

    public bool IsActive { get; set; }

    public string? Remark { get; set; }

    public int? AddedById { get; set; }

    public DateTime AddedDateTime { get; set; }

    public int? UpdatedById { get; set; }

    public DateTime? UpdatedDateTime { get; set; }

    public virtual ICollection<TenderClient> Tenders { get; set; } = new List<TenderClient>();
}
