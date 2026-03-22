using System;
using System.Collections.Generic;
using System.Text;

namespace axionpro.application.Common.Helpers
{
    public static class AccountTypeHelper
    {
        public static readonly HashSet<string> ValidTypes =
            new(StringComparer.OrdinalIgnoreCase)
            { "Salary", "Current", "Savings" };

        public static string Normalize(string input)
        {
            input = input?.Trim();

            if (string.IsNullOrEmpty(input))
                return "Savings";

            if (input.Equals("saving", StringComparison.OrdinalIgnoreCase))
                return "Savings";

            if (!ValidTypes.Contains(input))
                throw new Exception("Invalid AccountType");

            return char.ToUpper(input[0]) + input.Substring(1).ToLower();
        }
    }
}
