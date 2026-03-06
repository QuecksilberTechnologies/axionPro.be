using System;
using System.Collections.Generic;

namespace axionpro.domain.Entity;

public partial class District
{
    public int Id { get; set; }

    public int StateId { get; set; }

    public string? DistrictCode { get; set; }

    public string? PinCode { get; set; }

    public string DistrictName { get; set; } = null!;

    public bool IsActive { get; set; }

    public string? Remark { get; set; }

    public long? AddedById { get; set; }

    public DateTime? AddedDateTime { get; set; }

    public long? UpdatedById { get; set; }

    public DateTime? UpdatedDateTime { get; set; }

    public virtual State State { get; set; } = null!;
}
