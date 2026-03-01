using System;
using System.Collections.Generic;



public partial class Operation
{
    public int Id { get; set; }

    public string? Operationname { get; set; }

    public string? Remark { get; set; }

    public int? Operationtype { get; set; }

    public bool Isactive { get; set; }

    public long? Addedbyid { get; set; }

    public DateTime Addeddatetime { get; set; }

    public long? Updatedbyid { get; set; }

    public DateTime? Updatedatetime { get; set; }

    public string? Iconimage { get; set; }
}
