using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ChoETL.NACHA
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class ChoImmediateOriginValidationAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            string v = value.ToNString();
            return !v.IsNullOrEmpty() && ((v.Length == 9 && !v.Where(c => !Char.IsDigit(c)).Any()) || (v.Length == 10 && v[0] == ' ' && !v.Skip(1).Where(c => !Char.IsDigit(c)).Any()) || (v.Length == 10 && !v.Where(c => !Char.IsDigit(c)).Any()));
        }
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class ChoImmediateDestinationValidationAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            string v = value.ToNString();
            return !v.IsNullOrEmpty() && ((v.Length == 9 && !v.Where(c => !Char.IsDigit(c)).Any()) || (v.Length == 10 && v[0] == ' ' && !v.Skip(1).Where(c => !Char.IsDigit(c)).Any()));
        }
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class ChoOriginatorStatusCodeAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            string v = value.ToNString();
            return !v.IsNullOrEmpty() && v.Length > 0 && v[0] != '0' && Regex.IsMatch(v, "^[0-9]+$", RegexOptions.Compiled);
        }
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class ChoIsDigitAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            string v = value.ToNString();
            return !v.IsNullOrWhiteSpace() && v.Length > 0 && Char.IsDigit(v[0]);
        }
    }
}
