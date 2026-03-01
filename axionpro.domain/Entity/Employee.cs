using System;
using System.Collections.Generic;



public partial class Employee
{
    public long Id { get; set; }

    public long? Tenantid { get; set; }

    public int? Employeedocumentid { get; set; }

    public string? Employementcode { get; set; }

    public string? Lastname { get; set; }

    public string? Middlename { get; set; }

    public int? Genderid { get; set; }

    public string? Firstname { get; set; }

    public DateTime? Dateofbirth { get; set; }

    public DateTime? Dateofonboarding { get; set; }

    public DateTime? Dateofexit { get; set; }

    public int? Designationid { get; set; }

    public int? Employeetypeid { get; set; }

    public int? Departmentid { get; set; }

    public string? Officialemail { get; set; }

    public bool Haspermanent { get; set; }

    public bool Isactive { get; set; }

    public int? Functionalid { get; set; }

    public int? Referalid { get; set; }

    public string? Remark { get; set; }

    public bool? Iseditallowed { get; set; }

    public bool? Isinfoverified { get; set; }

    public long Addedbyid { get; set; }

    public DateTime Addeddatetime { get; set; }

    public long? Infoverifiedbyid { get; set; }

    public long? Updatedbyid { get; set; }

    public DateTime? Updateddatetime { get; set; }

    public long? Softdeletedbyid { get; set; }

    public DateTime? Deleteddatetime { get; set; }

    public bool? Issoftdeleted { get; set; }

    public DateTime? Infoverifieddatetime { get; set; }

    public string? Description { get; set; }

    public int Countryid { get; set; }

    public string? Emergencycontactnumber { get; set; }

    public int? Relation { get; set; }

    public string? Bloodgroup { get; set; }

    public string? Mobilenumber { get; set; }

    public bool? Ismarried { get; set; }

    public string? Emergencycontactperson { get; set; }
}
