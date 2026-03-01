using System;
using System.Collections.Generic;



public partial class Category
{
    public int Id { get; set; }

    public int? Parentid { get; set; }

    public string? Name { get; set; }

    public string? Description { get; set; }

    public string? Remark { get; set; }

    public string? Code { get; set; }

    public int Depth { get; set; }

    public string? Tags { get; set; }

    public bool Isactive { get; set; }
}
