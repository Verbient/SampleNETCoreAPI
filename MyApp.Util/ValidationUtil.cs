using MyApp.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MyApp.Util
{
    public static class ValidationUtil
    {
        public static bool ValidatePassword(string password, out string ErrorMessage)
        {
            var input = password;
            ErrorMessage = string.Empty;
            bool errorOccured = false;
            if (string.IsNullOrWhiteSpace(input))
            {
                throw new CustomException("Password should not be empty");
            }

            var hasNumber = new Regex(@"[0-9]+");
            var hasUpperChar = new Regex(@"[A-Z]+");
            var hasMiniMaxChars = new Regex(@".{8,15}");
            var hasLowerChar = new Regex(@"[a-z]+");
            var hasSymbols = new Regex(@"[!@#$%^&*()_+=\[{\]};:<>|./?,-]");

            if (!hasLowerChar.IsMatch(input))
            {
                errorOccured = true;
            }
            else if (!hasUpperChar.IsMatch(input))
            {
                errorOccured = true;
            }
            else if (!hasMiniMaxChars.IsMatch(input))
            {
                errorOccured = true;
            }
            else if (!hasNumber.IsMatch(input))
            {
                errorOccured = true;
            }
            else if (!hasSymbols.IsMatch(input))
            {
                errorOccured = true;
            }
            if (errorOccured)
            {
                ErrorMessage = "Password should contain at least 1 upper-case, 1 lower-case, 1 special character and should be 8-15 characters long.";
                return false;
            }
            return true;
           
        }

        public static bool ValidateEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }
}
