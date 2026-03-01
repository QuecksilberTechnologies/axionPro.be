using System;
using System.Collections.Generic;



public partial class Candidatecategoryskill
{
    public int Id { get; set; }

    public long Candidateid { get; set; }

    public int Categoryid { get; set; }

    public string? Description { get; set; }

    public DateTime Addeddatetime { get; set; }

    public bool Isactive { get; set; }
}
