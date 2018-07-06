using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Controls;

namespace Fieldscribe_Windows_App
{
    public class PasswordValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            Regex _regex = new Regex(@"[^\s]{8,50}");
            Match match = _regex.Match(value.ToString());

            return match.Success ?
                new ValidationResult(false, "Invalid passsword")
                : ValidationResult.ValidResult;
        }
    }
}
