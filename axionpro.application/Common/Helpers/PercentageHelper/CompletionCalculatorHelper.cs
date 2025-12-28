using axionpro.application.DTOS.Employee.Bank;
using axionpro.application.DTOS.Employee.CompletionPercentage;
using axionpro.domain.Entity;

namespace axionpro.application.Common.Helpers.PercentageHelper
{
    public static class CompletionCalculatorHelper
    {
        // =========================================================
        // EMPLOYEE BASIC COMPLETION
        // =========================================================
        public static double EmployeePropCalculate(Employee emp, bool hasPrimaryImage)
        {
            if (emp == null)
                return 0;

            int[] checks =
            {
                IsFilled(emp.FirstName),
                IsFilled(emp.LastName),
                emp.DateOfOnBoarding != null ? 1 : 0,
                emp.DesignationId > 0 ? 1 : 0,
                emp.DepartmentId > 0 ? 1 : 0,
                IsFilled(emp.OfficialEmail),
                emp.IsActive ? 1 : 0,
                emp.IsEditAllowed == true ? 1 : 0,
                emp.IsInfoVerified == true ? 1 : 0,
                hasPrimaryImage ? 1 : 0,
                IsFilled(emp.BloodGroup),
                IsFilled(emp.MobileNumber),
                IsFilled(emp.EmergencyContactNumber),
                IsFilled(emp.EmergencyContactPerson)
            };

            return CalculatePercentage(checks);
        }

        // =========================================================
        // BANK DETAILS COMPLETION
        // =========================================================
        public static double BankPropCalculate(GetBankResponseDTO record)
        {
            if (record == null)
                return 0;

            int[] checks =
            {
        IsFilled(record.BankName),
        IsFilled(record.AccountNumber),
        IsFilled(record.IFSCCode),
        IsFilled(record.BranchName),
        IsFilled(record.AccountType),

        // 🔥 BUSINESS RULE (Primary vs Non-Primary)
        record.IsPrimaryAccount == true? (record.HasChequeDocUploaded ? 1 : 0): 1
        };

            int completed = checks.Sum();
            int total = checks.Length;   // ✅ dynamic & future-safe

            return Math.Round((completed / (double)total) * 100, 2);
        }

        // =========================================================
        // COMMON HELPERS
        // =========================================================
        private static double CalculatePercentage(int[] checks)
        {
            int completed = checks.Sum();
            int total = checks.Length;

            return total == 0
                ? 0
                : Math.Round((completed / (double)total) * 100, 2);
        }

        private static int IsFilled(string? value)
            => string.IsNullOrWhiteSpace(value) ? 0 : 1;
    }
}



//using axionpro.application.DTOS.Employee.Bank;
//using axionpro.domain.Entity;

//namespace axionpro.application.Common.Helpers.PercentageHelper
//{
//    public static class CompletionCalculatorHelper
//    {
//        public static double EmployeePropCalculate(Employee emp, bool hasPrimaryImage)
//        {
//            if (emp == null)
//                return 0;

//            int[] checks =
//            {
//                IsFilled(emp.FirstName),
//                IsFilled(emp.LastName),
//                emp.DateOfOnBoarding != null ? 1 : 0,
//                emp.DesignationId > 0 ? 1 : 0,
//                emp.DepartmentId > 0 ? 1 : 0,
//                IsFilled(emp.OfficialEmail),
//                emp.IsActive ? 1 : 0,
//                emp.IsEditAllowed == true ? 1 : 0,
//                emp.IsInfoVerified == true ? 1 : 0,
//                hasPrimaryImage ? 1 : 0,
//                IsFilled(emp.BloodGroup),
//                IsFilled(emp.MobileNumber),
//                IsFilled(emp.EmergencyContactNumber),
//                IsFilled(emp.EmergencyContactPerson)
//            };

//            int completed = checks.Sum();
//            int total = checks.Length;   // ✅ dynamic total

//            return Math.Round((completed / (double)total) * 100, 2);
//        }

//        private static int IsFilled(string? value)
//            => string.IsNullOrWhiteSpace(value) ? 0 : 1;
//    }

//        public static double BankPropCalculate(Employee emp, bool hasPrimaryImage)
//        {
//            if (emp == null)
//                return 0;

//            int[] checks =
//            {
//               if (record == null) return 0;

//            int totalFields = 6;
//            int filled = 0;

//            if (!string.IsNullOrWhiteSpace(record.BankName)) filled++;
//            if (!string.IsNullOrWhiteSpace(record.AccountNumber)) filled++;
//            if (!string.IsNullOrWhiteSpace(record.IFSCCode)) filled++;
//            if (!string.IsNullOrWhiteSpace(record.BranchName)) filled++;
//            if (!string.IsNullOrWhiteSpace(record.AccountType)) filled++;

//            // 🔥 CRITICAL RULE
//            // Document counts ONLY IF primary
//            if (record.IsPrimaryAccount)
//            {
//                if (record.HasChequeDocUploaded)
//                    filled++;

//            }
//            else
//            {
//                // non-primary: document irrelevant
//                filled++;
//            }

//            return Math.Round((filled * 100.0) / totalFields, 0);
//        };

//            int completed = checks.Sum();
//            int total = checks.Length;   // ✅ dynamic total

//            return Math.Round((completed / (double)total) * 100, 2);
//        }

//        private static int IsFilled(string? value)
//            => string.IsNullOrWhiteSpace(value) ? 0 : 1;
//    }
//}
