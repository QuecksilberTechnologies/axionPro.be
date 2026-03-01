using System;
using System.Collections.Generic;



public partial class Attendancedevicetype
{
    public int Id { get; set; }

    public string? Devicetype { get; set; }

    public string? Remark { get; set; }

    public bool? Isactive { get; set; }

    public bool? Isdeviceregister { get; set; }

    public long Addedbyid { get; set; }

    public DateTime? Addeddatetime { get; set; }

    public long? Updatedbyid { get; set; }

    public DateTime? Updateddatetime { get; set; }
}
