using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Znode.Libraries.ECommerce.Utilities
{
    public enum GenericCompareOperator
    {
        GreaterThan,
        GreaterThanOrEqual,
        LessThan,
        LessThanOrEqual
    }

    public class GenericCompareAttribute : ValidationAttribute, IClientValidatable
    {
        private GenericCompareOperator operatorname = GenericCompareOperator.GreaterThanOrEqual;

        public string CompareToPropertyName { get; set; }
        public GenericCompareOperator OperatorName { get { return operatorname; } set { operatorname = value; } }

        public GenericCompareAttribute() : base() { }

        //Returns validation error message if comparison between value (currentValue) & validationContext (compareToValue) returns false
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (Equals(value, null) || Equals(validationContext, null))
                return null;

            var valueToCompare = (IComparable)value;
            var compareWith = GetCompareWithValue(validationContext);

            if (!Equals(valueToCompare, null) && !Equals(compareWith, null))
                return CompareValues(valueToCompare, compareWith, operatorname);
            return null;
        }

        //Returns IComparable CompareToValue from context
        private IComparable GetCompareWithValue(ValidationContext validationContext)
        {
            var basePropertyInfo = validationContext.ObjectType.GetProperty(CompareToPropertyName);

            return (IComparable)basePropertyInfo.GetValue(validationContext.ObjectInstance, null);
        }
        
        //Compares the two IComparable properties using GenericCompareOperator
        private ValidationResult CompareValues(IComparable valueToCompare, IComparable compareWith, GenericCompareOperator comparisonOperator)
        {
            if ((comparisonOperator == GenericCompareOperator.GreaterThan && valueToCompare.CompareTo(compareWith) <= 0) ||
                             (comparisonOperator == GenericCompareOperator.GreaterThanOrEqual && valueToCompare.CompareTo(compareWith) < 0) ||
                             (comparisonOperator == GenericCompareOperator.LessThan && valueToCompare.CompareTo(compareWith) >= 0) ||
                             (comparisonOperator == GenericCompareOperator.LessThanOrEqual && valueToCompare.CompareTo(compareWith) > 0))
                return new ValidationResult(base.ErrorMessage);
            return null;
        }

        #region IClientValidatable Members

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            string errorMessage = this.FormatErrorMessage(metadata.DisplayName);
            ModelClientValidationRule compareRule = new ModelClientValidationRule();
            compareRule.ErrorMessage = errorMessage;
            compareRule.ValidationType = "genericcompare";
            compareRule.ValidationParameters.Add("comparetopropertyname", CompareToPropertyName);
            compareRule.ValidationParameters.Add("operatorname", OperatorName.ToString());
            yield return compareRule;
        }

        #endregion
    }
}