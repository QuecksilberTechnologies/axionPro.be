using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.Common.Enums
{
    
        public enum OperationType
        {
            Add = 1,
            Update = 2,
            Delete = 3,
            View = 4,
            Approve = 5,
            Reject = 6,
            Assign = 7,
            Transfer = 8,
            Activate = 9,
            Deactivate = 10,
            Export = 11,
            Import = 12,
            Download = 13,
            Upload = 14,
            Print = 15,
            Share = 16,
            Reassign = 17,
            Verify = 18,
            Submit = 19,
            Review = 20,
            Close = 21,
            Reopen = 22,
            Apply = 23,
            Request = 24,
            Deposit = 25,
            Restore = 26,
            Resend = 27
        }

    public enum TabInfoType
    {
        Employee = 1,
        Bank = 2,
        Contact = 3,
        Experience = 4,
        Identity = 5,
        Education = 6,
        Dependent = 7,
        Insurance = 8,
       
    }

    public enum RelationDependant
    {
        Father = 1,
        Mother = 2,
        Spouse = 3,
        Son = 4,
        Daughter = 5,
        Brother = 6,
        Sister = 7,
      

        Other = 99
    }


}
