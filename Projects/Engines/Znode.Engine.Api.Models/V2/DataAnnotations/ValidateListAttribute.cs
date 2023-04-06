using System;
using System.Collections;
using System.ComponentModel.DataAnnotations;

namespace Znode.Engine.Api.Models.V2.DataAnnotations
{
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class ValidateListAttribute : ValidationAttribute
    {
        private const string defaultError = "'{0}' cannot be empty.";
        public ValidateListAttribute() : base(defaultError) //
        {
        }

        public override bool IsValid(object value)
        {
            IList list = value as IList;
            return (list != null && list.Count > 0);
        }

        public override string FormatErrorMessage(string name)
        {
            return string.Format(ErrorMessageString, name);
        }
    }
}