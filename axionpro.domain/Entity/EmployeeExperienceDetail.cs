using System;
using System.Collections.Generic;



public partial class Employeeexperiencedetail
{
    public long Id { get; set; }

    public long Employeeid { get; set; }

    public string? Companyname { get; set; }

    public bool Isinfolatestyear { get; set; }

    public int Experience { get; set; }

    public bool Isanygap { get; set; }

    public string? Reasonofgap { get; set; }

    public DateTime? Gapyearfrom { get; set; }

    public DateTime? Gapyearto { get; set; }

    public bool Hasuploadedgapcertificate { get; set; }

    public string? Gapcertificatedocname { get; set; }

    public string? Gapcertificatedocpath { get; set; }

    public bool Hasuploadedbankstatement { get; set; }

    public bool Iswfh { get; set; }

    public int? Workingcountryid { get; set; }

    public int? Workingstateid { get; set; }

    public int? Workingdistrictid { get; set; }

    public bool Hastaxationdoc { get; set; }

    public bool Hasuploadedtaxationdoc { get; set; }

    public string? Taxationdocfilename { get; set; }

    public string? Taxationdocfilepath { get; set; }

    public bool Hasuploadedexperienceletter { get; set; }

    public string? Experienceletterdocname { get; set; }

    public string? Experienceletterdocpath { get; set; }

    public bool Hasuploadedjoiningletter { get; set; }

    public string? Joiningletterdocname { get; set; }

    public string? Joiningletterdocpath { get; set; }

    public string? Employeeidofcompany { get; set; }

    public string? Colleaguename { get; set; }

    public string? Colleaguedesignation { get; set; }

    public string? Colleaguecontactnumber { get; set; }

    public string? Reportingmanagername { get; set; }

    public string? Reportingmanagernumber { get; set; }

    public string? Verificationemail { get; set; }

    public string? Reasonforleaving { get; set; }

    public string? Remark { get; set; }

    public string? Designation { get; set; }

    public DateTime? Startdate { get; set; }

    public DateTime? Enddate { get; set; }

    public bool? Isexperienceverified { get; set; }

    public bool? Isexperienceverifiedbymail { get; set; }

    public bool? Isexperienceverifiedbycall { get; set; }

    public long? Infoverifiedbyid { get; set; }

    public bool? Iseditallowed { get; set; }

    public DateTime? Infoverifieddatetime { get; set; }

    public bool? Isinfoverified { get; set; }

    public bool Isactive { get; set; }

    public bool? Issoftdeleted { get; set; }

    public long? Softdeletedbyid { get; set; }

    public long Addedbyid { get; set; }

    public long? Updatedbyid { get; set; }

    public DateTime Addeddatetime { get; set; }

    public DateTime? Updateddatetime { get; set; }

    public bool? Hasbankstatementuploaded { get; set; }

    public string? Bankstatementdocname { get; set; }

    public string? Bankstatementdocpath { get; set; }

    public long? Employeeexperienceid { get; set; }

    public bool Isforeignexperience { get; set; }

    public string? Visatype { get; set; }

    public string? Workpermitnumber { get; set; }

    public string? Visadocname { get; set; }

    public string? Visadocpath { get; set; }

    public string? Workpermitdocname { get; set; }

    public string? Workpermitdocpath { get; set; }

    public string? Immigrationstampdocname { get; set; }

    public string? Immigrationstampdocpath { get; set; }

    public string? Foreigncontractdocname { get; set; }

    public string? Foreigncontractdocpath { get; set; }

    public bool? Hasvisauploaded { get; set; }

    public bool? Hasworkpermituploaded { get; set; }

    public bool? Hasimmigrationstampuploaded { get; set; }

    public bool? Hasforeigncontractuploaded { get; set; }
}
