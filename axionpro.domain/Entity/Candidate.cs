using System;
using System.Collections.Generic;



public partial class Candidate
{
    public long Id { get; set; }

    public string? Firstname { get; set; }

    public string? Lastname { get; set; }

    public string? Email { get; set; }

    public string? Phonenumber { get; set; }

    public string? Pan { get; set; }

    public string? Aadhaar { get; set; }

    public string? Candidatereferencecode { get; set; }

    public bool Isblacklisted { get; set; }

    public decimal? Experienceyears { get; set; }

    public string? Currentlocation { get; set; }

    public decimal? Expectedsalary { get; set; }

    public string? Currentcompany { get; set; }

    public int? Noticeperiod { get; set; }

    public DateOnly? Dateofbirth { get; set; }

    public DateTime Applieddate { get; set; }

    public DateTime? Lastupdateddatetime { get; set; }

    public string? Skillset { get; set; }

    public bool Isactive { get; set; }

    public string? Actionstatus { get; set; }

    public string? Education { get; set; }

    public bool? Isfresher { get; set; }

    public string? Fewwords { get; set; }

    public string? Resumepath { get; set; }

    public string? Resumeurl { get; set; }
}
