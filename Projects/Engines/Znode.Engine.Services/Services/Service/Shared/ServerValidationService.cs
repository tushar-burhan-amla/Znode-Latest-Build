using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using Znode.Engine.Api.Models;
using Znode.Engine.Services.Maps;
using Znode.Libraries.Data;
using Znode.Libraries.Data.DataModel;
using Znode.Libraries.Data.Helpers;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;
using Znode.Libraries.Resources;

namespace Znode.Engine.Services
{

    public class ServerValidationService : BaseService, IServerValidationService
    {
        private readonly IZnodeRepository<ZnodeMediaAttributeValidation> _attributeValidation;
        private readonly IZnodeRepository<ZnodeMediaAttribute> _attributeRepository;
        private readonly IZnodeViewRepository<View_AttributeValidationList> _AttributeValidationStoredProc;

        #region Constructor
        public ServerValidationService()
        {
            _attributeRepository = new ZnodeRepository<ZnodeMediaAttribute>();
            _attributeValidation = new ZnodeRepository<ZnodeMediaAttributeValidation>();
            _AttributeValidationStoredProc = new ZnodeViewRepository<View_AttributeValidationList>();
        }
        #endregion

        //compair
        public virtual ValidateServerModel CompairValidation(ValidateServerModel model)
        {
            ZnodeLogging.LogMessage("Execution started.", string.Empty, TraceLevel.Info);
            var codekeyArray = model.ControlsData.Keys;
            string codekey = string.Join(",", codekeyArray);

            FilterCollection filters = new FilterCollection();
            filters.Add(new FilterTuple(ZnodeMediaAttributeEnum.AttributeCode.ToString(), ProcedureFilterOperators.Or, codekey));
            //gets the where clause with filter Values.              
            EntityWhereClauseModel whereClauseModel = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollection());
            ZnodeLogging.LogMessage("WhereClause to get attributes: ", string.Empty, TraceLevel.Verbose, whereClauseModel.WhereClause);
            var attributes = _attributeRepository.GetEntityList(whereClauseModel.WhereClause, whereClauseModel.FilterValues);

            //gets the where clause with filter Values.              
            EntityWhereClauseModel whereClause = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollection());
            ZnodeLogging.LogMessage("WhereClause to get attributeValidation: ", string.Empty, TraceLevel.Verbose, whereClause.WhereClause);
            var attributeValidation = _attributeValidation.GetEntityList(whereClause.WhereClause, whereClause.FilterValues);

            Dictionary<string, string> ErrorDictionary = new Dictionary<string, string>();

            foreach (var item in model.ControlsData.Keys)
            {
                var keyValuePair = model.ControlsData.Single(x => x.Key == item.ToString());
                var value = keyValuePair.Value;

                var data = attributes.SingleOrDefault(x => x.AttributeCode == item);
                if (!Equals(data, null))
                {
                    if (data.IsRequired.GetValueOrDefault() && string.IsNullOrEmpty(value.ToString()))  //Validation for IsRequired
                    {
                        ErrorDictionary.Add(item, Admin_Resources.ErrorFieldRequired);
                    }
                    if (data.IsRequired.GetValueOrDefault())    //Validation for RegExp
                    {
                        string expr = "^\\d{0,3}(\\.\\d{0})?$";
                        if (!Regex.IsMatch(value.ToString(), expr))
                            ErrorDictionary.Add(item, Admin_Resources.AllowedNumberRange);
                    }
                }
                var extCollection = attributeValidation.Select(p => p.Name).ToArray();
                string ext = System.IO.Path.GetExtension("http://localhost:6766/MediaFolder/7033133-windows-black-background.jpg");
                if (extCollection.Contains(ext.Substring(1).ToUpper()))
                {
                    ErrorDictionary.Add("Code9", Admin_Resources.FileWithWrongFormatUploaded);
                    break;
                }
            }

            model.ErrorDictionary = ErrorDictionary;
            foreach (KeyValuePair<string, string> entry in ErrorDictionary)
            {
                ZnodeLogging.LogMessage(string.Format(Admin_Resources.ErrorProcessRequest, entry.Key, entry.Value), string.Empty, TraceLevel.Info);
            }
            ZnodeLogging.LogMessage("Execution done.", string.Empty, TraceLevel.Info);
            return model;
        }

        public virtual bool CheckServerValidation(string attributeCodes, object value)
        {
            ZnodeLogging.LogMessage("Execution started.", string.Empty, TraceLevel.Info);
            bool isValid = true;
            var AttributeCodeArray = attributeCodes.Split(',');
            
            _AttributeValidationStoredProc.SetParameter("@AttributeCode", attributeCodes, ParameterDirection.Input, DbType.String);
            ZnodeLogging.LogMessage("attributeCodes to get ValidationAttribute: ", string.Empty, TraceLevel.Verbose, attributeCodes);
            var ValidationAttribute = _AttributeValidationStoredProc.ExecuteStoredProcedureList("Znode_GetAttributeValidationDetails @AttributeCode");

            foreach (var Attributecode in AttributeCodeArray)
            {
                var data = ValidationAttribute.Where(x => Equals(x.AttributeCode, Attributecode)).ToList();
                if (!Equals(data, null) && data.Any())
                {
                    var AttributeType = data[0].AttributeTypeName.ToString();

                    IEnumerable<AttributeValidationModel> AttributeValidationList = data.Select(
                      model => new AttributeValidationModel()
                      {
                          AttributeId = model.MediaAttributeId,
                          AttributeTypeId = model.AttributeTypeId,
                          AttributeTypeName = model.AttributeTypeName,
                          AttributeCode = model.AttributeCode,
                          IsRequired = model.IsRequired,
                          IsLocalizable = model.IsLocalizable,
                          IsFilterable = model.IsFilterable,
                          AttributeName = model.AttributeName,
                          ControlName = model.ControlName,
                          ValidationName = model.ValidationName,
                          SubValidationName = model.SubValidationName,
                          ValidationValue = model.ValidationValue,
                          RegExp = model.RegExp,
                          IsRegExp = model.IsRegExp,

                      });

                    Dictionary<string, string> ErrorDictionary = new Dictionary<string, string>();
                    if (data[0].IsRequired.GetValueOrDefault() && string.IsNullOrEmpty(value.ToString()))
                    {
                        ErrorDictionary.Add(data[0].AttributeCode, Admin_Resources.ErrorFieldRequired);
                        isValid = false;
                    }

                    switch (AttributeType)
                    {
                        case "Text":
                            isValid = TextboxValidations(AttributeValidationList.ToList(), value);
                            break;
                        case "Number":
                        case "Price":
                            isValid = NumberValidations(AttributeValidationList.ToList(), value);
                            break;
                        case "Identifire":
                            isValid = IdentifiersValidations(AttributeValidationList.ToList(), value);
                            break;
                        case "Date":
                            isValid = DateValidations(AttributeValidationList.ToList(), value);
                            break;
                        case "File":
                            isValid = FileValidations(AttributeValidationList.ToList(), value);
                            break;
                        case "Image":
                            break;
                        default:
                            break;
                    }
                }
            }
            ZnodeLogging.LogMessage("Execution done.", string.Empty, TraceLevel.Info);
            return isValid;
        }

        //Textbox validation
        public virtual bool TextboxValidations(List<AttributeValidationModel> model, object value)
        {
            ZnodeLogging.LogMessage("Execution started.", string.Empty, TraceLevel.Info);
            bool ValidTextBox = true;
            Dictionary<string, string> ErrorDictionary = new Dictionary<string, string>();

            if (model[0].IsRegExp.GetValueOrDefault())
            {
                string expr = model[0].RegExp.ToString();
                if (!Regex.IsMatch(value.ToString(), expr))
                {
                    ErrorDictionary.Add(model[0].AttributeCode, Admin_Resources.AllowedNumberRange);
                    ValidTextBox = false;
                }
            }

            foreach (KeyValuePair<string, string> entry in ErrorDictionary)
            {
                ZnodeLogging.LogMessage(string.Format(Admin_Resources.ErrorProcessRequest, entry.Key, entry.Value), string.Empty, TraceLevel.Info);
            }

            ZnodeLogging.LogMessage("Execution done.", string.Empty, TraceLevel.Info);
            return ValidTextBox;
        }

        //Number validation
        public virtual bool NumberValidations(List<AttributeValidationModel> model, object value)
        {
            ZnodeLogging.LogMessage("Execution started.", string.Empty, TraceLevel.Info);
            ZnodeLogging.LogMessage("Attribute validation list count: ", string.Empty, TraceLevel.Verbose, model?.Count);
            bool ValidNumber = true;
            Dictionary<string, string> NumberErrorDictionary = new Dictionary<string, string>();

            if (string.IsNullOrEmpty(Convert.ToString(value)))
            {
                return ValidNumber;
            }

            if (!Regex.IsMatch(value.ToString(), @"^\d+(?:[\.\,]\d+)?$"))
            {
                NumberErrorDictionary.Add(model[0].AttributeCode, Admin_Resources.NumbersFromZeroToNineAllowed);
                ValidNumber = false;
            }

            var allowDecimal = model.Where(x => Equals(x.ValidationName, "AllowDecimals")).Select(x => x.SubValidationName).FirstOrDefault();
            var allowNegative = model.Where(x => Equals(x.ValidationName, "AllowNegative")).Select(x => x.SubValidationName).FirstOrDefault();

            if (Equals(allowDecimal, "NO") && value.ToString().Contains("."))
            {
                NumberErrorDictionary.Add(model[0].AttributeCode, Admin_Resources.ErrorValueInDecimalFormat);
                ValidNumber = false;
            }

            if (Equals(allowNegative, "NO") && Convert.ToInt32(value) < 0)
            {
                NumberErrorDictionary.Add(model[0].AttributeCode, Admin_Resources.ErrorValueNegative);
                ValidNumber = false;
            }

            var maxNumber = model.Where(x => Equals(x.ValidationName, "MaxNumber")).Select(x => x.ValidationValue).FirstOrDefault();
            var minNumber = model.Where(x => Equals(x.ValidationName, "MinNumber")).Select(x => x.ValidationValue).FirstOrDefault();


            if (!string.IsNullOrEmpty(maxNumber) && (Convert.ToInt32(value) > Convert.ToInt32(maxNumber)))
            {
                NumberErrorDictionary.Add(model[0].AttributeCode, Admin_Resources.ErrorValueGreaterThanMaxNumber);
                ValidNumber = false;
            }

            if (!string.IsNullOrEmpty(minNumber) && (Convert.ToInt32(value) < Convert.ToInt32(minNumber)))
            {
                NumberErrorDictionary.Add(model[0].AttributeCode, Admin_Resources.ErrorValueLessThanMinNumber);
                ValidNumber = false;
            }


            if (!string.IsNullOrEmpty(maxNumber) && !string.IsNullOrEmpty(minNumber))
            {
                if (HelperUtility.Between(Convert.ToInt32(value), Convert.ToInt32(minNumber), Convert.ToInt32(maxNumber), true))
                    ValidNumber = true;
            }


            ZnodeLogging.LogMessage("Execution done.", string.Empty, TraceLevel.Info);
            return ValidNumber;
        }

        //Number validation
        public virtual bool IdentifiersValidations(List<AttributeValidationModel> model, object value)
        {
            ZnodeLogging.LogMessage("Execution started.", string.Empty, TraceLevel.Info);
            ZnodeLogging.LogMessage("Attribute validation list count: ", string.Empty, TraceLevel.Verbose, model?.Count);
            bool validIdentifiers = true;
            Dictionary<string, string> IdentifierErrorDictionary = new Dictionary<string, string>();
            var maxChar = model.Where(x => Equals(x.ValidationName, "MaxCharacters")).Select(x => x.ValidationValue).FirstOrDefault();
            var validationRule = model.Where(x => Equals(x.ValidationName, "ValidationRule")).Select(x => x.ValidationValue).FirstOrDefault();

            if (!Equals(maxChar, null) && Convert.ToInt32(maxChar) < value.ToString().Length)
            {
                IdentifierErrorDictionary.Add(model[0].AttributeCode, Admin_Resources.ErrorCharecterGreaterThanSpecifiedLength);
                validIdentifiers = false;
            }

            if (!Equals(validationRule, null) && !Regex.IsMatch(value.ToString(), validationRule.ToString()))
            {
                IdentifierErrorDictionary.Add(model[0].AttributeCode, Admin_Resources.ErrorPatternMatch);
                validIdentifiers = false;
            }

            ZnodeLogging.LogMessage("Execution done.", string.Empty, TraceLevel.Info);
            return validIdentifiers;
        }

        //Date validation
        public virtual bool DateValidations(List<AttributeValidationModel> model, object value)
        {
            ZnodeLogging.LogMessage("Execution started.", string.Empty, TraceLevel.Info);
            ZnodeLogging.LogMessage("Attribute validation list count: ", string.Empty, TraceLevel.Verbose, model?.Count);
            bool validDate = true;

            if (string.IsNullOrEmpty(Convert.ToString(value)))
            {
                return validDate;
            }

            Dictionary<string, string> DateErrorDictionary = new Dictionary<string, string>();
            var maxDate = model.Where(x => Equals(x.ValidationName, "MaxDate")).Select(x => x.ValidationValue).FirstOrDefault();
            var MinDate = model.Where(x => Equals(x.ValidationName, "MinDate")).Select(x => x.ValidationValue).FirstOrDefault();

            if (!string.IsNullOrEmpty(maxDate) && Convert.ToDateTime(value) > Convert.ToDateTime(maxDate))
            {
                DateErrorDictionary.Add(model[0].AttributeCode, Admin_Resources.ErrorDateGreaterThanMaxDate);
                validDate = false;
            }


            if (!string.IsNullOrEmpty(MinDate) && Convert.ToDateTime(value) < Convert.ToDateTime(MinDate))
            {
                DateErrorDictionary.Add(model[0].AttributeCode, Admin_Resources.ErrorDateLessThanMinDate);
                validDate = false;
            }


            ZnodeLogging.LogMessage("Execution done.", string.Empty, TraceLevel.Info);
            return validDate;
        }

        //File validation
        public virtual bool FileValidations(List<AttributeValidationModel> model, object value) => true;
    }
}
