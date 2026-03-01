using System;
using System.Collections.Generic;



public partial class Employeepersonaldetail
{
    public long Id { get; set; }

    public long Employeeid { get; set; }

    public string? Aadhaarnumber { get; set; }

    public string? Pannumber { get; set; }

    public string? Passportnumber { get; set; }

    public string? Drivinglicensenumber { get; set; }

    public string? Voterid { get; set; }

    public string? Bloodgroup { get; set; }

    public bool Maritalstatus { get; set; }

    public string? Nationality { get; set; }

    public string? Emergencycontactname { get; set; }

    public string? Emergencycontactnumber { get; set; }

    public bool Isactive { get; set; }

    public bool? Iseditallowed { get; set; }

    public bool? Issoftdeleted { get; set; }

    public long? Addedbyid { get; set; }

    public DateTime? Addeddatetime { get; set; }

    public long? Updatedbyid { get; set; }

    public DateTime? Updateddatetime { get; set; }

    public long? Softdeletedbyid { get; set; }

    public DateTime? Deleteddatetime { get; set; }

    public long? Infoverifiedbyid { get; set; }

    public DateTime? Infoverifieddatetime { get; set; }

    public bool? Isinfoverified { get; set; }

    public bool? Haspaniduploaded { get; set; }

    public bool? Hasaadhaariduploaded { get; set; }

    public bool? Haspassportiduploaded { get; set; }

    public string? Pandocname { get; set; }

    public string? Passportdocname { get; set; }

    public string? Aadhaardocname { get; set; }

    public string? Pandocpath { get; set; }

    public string? Passportdocpath { get; set; }

    public string? Aadhaardocpath { get; set; }

    public string? Emergencycontactrelation { get; set; }

    public string? Uannumber { get; set; }

    public bool? Hasepfaccount { get; set; }
}
