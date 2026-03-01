using System;
using System.Collections.Generic;



public partial class Employeeeducation
{
    public int Id { get; set; }

    public long Employeeid { get; set; }

    public string? Degree { get; set; }

    public string? Institutename { get; set; }

    public string? Remark { get; set; }

    public DateOnly? Startdate { get; set; }

    public DateOnly? Enddate { get; set; }

    public string? Scorevalue { get; set; }

    public string? Gradedivision { get; set; }

    public bool? Educationgap { get; set; }

    public string? Reasonofeducationgap { get; set; }

    public long Addedbyid { get; set; }

    public DateTime Addeddatetime { get; set; }

    public long? Updatedbyid { get; set; }

    public DateTime? Updateddatetime { get; set; }

    public long? Softdeletedbyid { get; set; }

    public DateTime? Deleteddatetime { get; set; }

    public bool? Issoftdeleted { get; set; }

    public long? Infoverifiedbyid { get; set; }

    public bool? Isinfoverified { get; set; }

    public DateTime? Infoverifieddatetime { get; set; }

    public bool? Iseditallowed { get; set; }

    public bool? Isactive { get; set; }

    public string? Filepath { get; set; }

    public int? Filetype { get; set; }

    public string? Filename { get; set; }

    public bool? Haseducationdocuploded { get; set; }

    public int? Scoretype { get; set; }

    public double? Gapyears { get; set; }
}
