using axionpro.domain.Entity;

namespace axionpro.application.Common.Helpers.PercentageHelper
{
    public static class CompletionCalculatorHelper
    {
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

            int completed = checks.Sum();
            int total = checks.Length;   // ✅ dynamic total

            return Math.Round((completed / (double)total) * 100, 2);
        }

        private static int IsFilled(string? value)
            => string.IsNullOrWhiteSpace(value) ? 0 : 1;
    }
}
