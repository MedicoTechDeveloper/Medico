using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Medico.Application.DataAnnotation
{
    public class NonRequiredEmailAddressAttribute : ValidationAttribute
    {
        private readonly Regex _emailRegex =
            new Regex(
                "^(([^<>()[\\]\\\\.,;:\\s@\\\"]+(\\.[^<>()[\\]\\\\.,;:\\s@\\\"]+)*)|(\\\".+\\\"))@((\\[[0-9]{1,3}\\.[0-9]{1,3}\\.[0-9]{1,3}\\.[0-9]{1,3}\\])|(([a-zA-Z\\-0-9]+\\.)+[a-zA-Z]{2,}))$");
        
        public override bool IsValid(object value)
        {
            var stringValue = value as string;
            return string.IsNullOrEmpty(stringValue) || _emailRegex.IsMatch(stringValue);
        }
    }
}