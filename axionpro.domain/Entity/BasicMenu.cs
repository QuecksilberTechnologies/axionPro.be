using System;
using System.Collections.Generic;



public partial class Basicmenu
{
    public int Id { get; set; }

    public string? Menuname { get; set; }

    public string? Menuurl { get; set; }

    public int? Parentmenuid { get; set; }

    public string? Remark { get; set; }

    public bool Isactive { get; set; }

    public string? Imageicon { get; set; }
}
