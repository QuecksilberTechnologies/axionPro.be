using System;
using System.Collections.Generic;



public partial class Client
{
    public int Id { get; set; }

    public int Clienttypeid { get; set; }

    public string? Clientname { get; set; }

    public string? Contactperson { get; set; }

    public string? Email { get; set; }

    public string? Phonenumber { get; set; }

    public string? Address { get; set; }

    public bool Isactive { get; set; }

    public string? Remark { get; set; }
}
