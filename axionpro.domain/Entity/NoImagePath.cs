using System;
using System.Collections.Generic;

namespace axionpro.domain.Entity;

public partial class NoImagePath
{
    public int Id { get; set; }

    public string? ImageName { get; set; }

    public string DefaultImagePath { get; set; } = null!;

    public int ImageType { get; set; }

    public bool IsActive { get; set; }
}
