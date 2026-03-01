using System;
using System.Collections.Generic;

 

public partial class License
{
    public int Id { get; set; }

    public DateTime? Licensestartdate { get; set; }

    public DateTime? Licenseenddate { get; set; }

    public bool? Isactive { get; set; }
}
