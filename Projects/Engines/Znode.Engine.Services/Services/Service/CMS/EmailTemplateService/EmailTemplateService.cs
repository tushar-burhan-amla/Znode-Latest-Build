using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using Znode.Engine.Api.Models;
using Znode.Engine.Exceptions;
using Znode.Engine.Services.Constants;
using Znode.Engine.Services.Maps;
using Znode.Libraries.Data;
using Znode.Libraries.Data.DataModel;
using Znode.Libraries.Data.Helpers;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;
using Znode.Libraries.Resources;

namespace Znode.Engine.Services
{
    public class EmailTemplateService : BaseService, IEmailTemplateService
    {
        #region Private variables              
        private readonly IZnodeRepository<ZnodeEmailTemplate> _emailTemplateRepository;
        private readonly IZnodeRepository<ZnodeEmailTemplateLocale> _emailTemplateLocaleRepository;
        private readonly IZnodeRepository<ZnodeEmailTemplateMapper> _emailTemplateMapperRepository;
        private readonly IZnodeRepository<ZnodeEmailTemplateArea> _emailTemplateAreaRepository;
        public const string ResetPassword = "ResetPassword";
        #endregion

        public EmailTemplateService()
        {
            _emailTemplateRepository = new ZnodeRepository<ZnodeEmailTemplate>();
            _emailTemplateLocaleRepository = new ZnodeRepository<ZnodeEmailTemplateLocale>();
            _emailTemplateMapperRepository = new ZnodeRepository<ZnodeEmailTemplateMapper>();
            _emailTemplateAreaRepository = new ZnodeRepository<ZnodeEmailTemplateArea>();
        }

        #region Public Methods
        //Get the Email Template List
        public virtual EmailTemplateListModel GetEmailTemplateList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            //Bind the Filter, sorts & Paging details.
            PageListModel pageListModel = new PageListModel(filters, sorts, page);
            ZnodeLogging.LogMessage("pageListModel generated to get email template list: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, pageListModel?.ToDebugString());

            expands.Add(ZnodeEmailTemplateEnum.ZnodeEmailTemplateLocales.ToString(), ZnodeEmailTemplateEnum.ZnodeEmailTemplateLocales.ToString());
            //Get Expands.
            List<string> navigationProperties = GetExpands(expands);
            //maps the entity list to model
            List<ZnodeEmailTemplate> emailTemplateList = _emailTemplateRepository.GetPagedList(pageListModel.EntityWhereClause.WhereClause, pageListModel.OrderBy, navigationProperties, pageListModel.EntityWhereClause.FilterValues, pageListModel.PagingStart, pageListModel.PagingLength, out pageListModel.TotalRowCount).ToList();
            ZnodeLogging.LogMessage("emailTemplateList count: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, emailTemplateList?.Count);

            EmailTemplateListModel listModel = new EmailTemplateListModel();
            listModel.EmailTemplatesList = (from test in emailTemplateList
                                            select new EmailTemplateModel
                                            {
                                                EmailTemplateId = test.EmailTemplateId,
                                                TemplateName = test.TemplateName,
                                                Descriptions = test.ZnodeEmailTemplateLocales?.FirstOrDefault()?.Descriptions,
                                                Subject = test.ZnodeEmailTemplateLocales?.FirstOrDefault()?.Subject,
                                                CreatedDate = test.CreatedDate,
                                                ModifiedDate = test.ModifiedDate,
                                                CreatedBy = test.CreatedBy,
                                                ModifiedBy = test.ModifiedBy
                                            }).ToList();

            //Set for pagination
            listModel.BindPageListModel(pageListModel);
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            return listModel;
        }

        //Create Email Template.
        public virtual EmailTemplateModel CreateTemplatePage(EmailTemplateModel model)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);

            if (Equals(model, null))
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.ErrorEmailTemplateModelNull);

            ZnodeLogging.LogMessage("Input parameter EmailTemplateModel: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, model);

            //Check if template name already exists.
            if (_emailTemplateRepository.Table.Count(x => x.TemplateName.Trim() == model.TemplateName.Trim()) > 0)
                throw new ZnodeException(ErrorCodes.AlreadyExist, Admin_Resources.ErrorEmailTemplateAlreadyExists);

            //If LocaleId is zero then get default locale.
            if (model.LocaleId == 0)
                model.LocaleId = GetDefaultLocaleId();

            model.Html = string.Format(Admin_Resources.FormatEmailTemplateBody, model.Html);
            //Insert into Email Template.
            EmailTemplateModel emailTemplateModel = _emailTemplateRepository.Insert(model.ToEntity<ZnodeEmailTemplate>()).ToModel<EmailTemplateModel>();

            //Insert into Email Template Locale.
            if (emailTemplateModel?.EmailTemplateId > 0)
            {
                model.EmailTemplateId = emailTemplateModel.EmailTemplateId;

                ZnodeLogging.LogMessage(string.Format(Admin_Resources.SuccessEmailTemplateCreatedWithId, emailTemplateModel.EmailTemplateId), ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);

                ZnodeLogging.LogMessage(_emailTemplateLocaleRepository.Insert(model?.ToEntity<ZnodeEmailTemplateLocale>())?.EmailTemplateLocaleId > 0
                   ? Admin_Resources.SuccessEmailTemplateLocaleInserted : Admin_Resources.ErrorInsertEmailTemplateLocale, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
                return model;
            }
            ZnodeLogging.LogMessage(Admin_Resources.ErrorEmailTemplateCreate, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            return model;
        }

        //Get Email Template by emailTemplateId.
        public virtual EmailTemplateModel GetEmailTemplate(int emailTemplateId, FilterCollection filters)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameter emailTemplateId: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, emailTemplateId);

            if (emailTemplateId < 1)
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.EmailTemplateIdGreaterThanOne);
            ZnodeEmailTemplate emailTemplateEntity = _emailTemplateRepository.GetEntity(GetWhereClauseForEmailTemplateId(emailTemplateId).WhereClause, new List<string> { ExpandKeys.ZnodeEmailTemplateLocale.ToLower() });

            if (HelperUtility.IsNull(emailTemplateEntity))
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.ErrorModelNull);

            //Maps to Email template locale entity to emailTemplateModel.
            EmailTemplateModel emailTemplateModel = emailTemplateEntity.ToModel<EmailTemplateModel>();
            emailTemplateModel.EmailTemplateTokens = GetEmailTemplateTokens()?.EmailTemplateTokens;
            if (!string.IsNullOrEmpty(emailTemplateModel?.EmailTemplateTokens) && emailTemplateModel.EmailTemplateTokens.Contains("~"))
            {
                emailTemplateModel.EmailTemplateTokensPartOne = emailTemplateModel.EmailTemplateTokens.Split('~')[0];
                emailTemplateModel.EmailTemplateTokensPartTwo = emailTemplateModel.EmailTemplateTokens.Split('~')[1];
            }
            //Sets the properties of emailTemplatemodel.           
            SetEmailTemplateModel(emailTemplateEntity, emailTemplateModel, GetLocaleId(filters));
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            return emailTemplateModel;
        }

        //Update Email Template.
        public virtual bool UpdateTemplatePage(EmailTemplateModel emailTemplateModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);

            if (HelperUtility.IsNull(emailTemplateModel))
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.ErrorEmailTemplateModel);

            if (emailTemplateModel.EmailTemplateId < 1)
                throw new ZnodeException(ErrorCodes.IdLessThanOne, Admin_Resources.IdCanNotBeLessThanOne);
            ZnodeLogging.LogMessage("Input parameter EmailTemplateId of emailTemplateModel: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new { EmailTemplateId = emailTemplateModel?.EmailTemplateId });

            //Check if template name already exists.
            if (_emailTemplateRepository.Table.Count(x => x.TemplateName.Trim() == emailTemplateModel.TemplateName.Trim() && x.EmailTemplateId != emailTemplateModel.EmailTemplateId) > 0)
                throw new ZnodeException(ErrorCodes.AlreadyExist, Admin_Resources.ErrorEmailTemplateAlreadyExists);

            emailTemplateModel.Html = string.Format(Admin_Resources.FormatEmailTemplateBody, emailTemplateModel.Html);

            //Update Email Template
            bool isEmailTemplateUpdated = _emailTemplateRepository.Update(emailTemplateModel.ToEntity<ZnodeEmailTemplate>());

            if (isEmailTemplateUpdated)
                //Save the data into email template locale.
                SaveInEmailTemplateLocale(emailTemplateModel);

            ZnodeLogging.LogMessage(isEmailTemplateUpdated ? Admin_Resources.SuccessEmailTemplateUpdated : Admin_Resources.ErrorEmailTemplateUpdate, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            return isEmailTemplateUpdated;
        }

        //Delete Email Template.
        public virtual bool DeleteTemplatePage(ParameterModel emailTemplateIds)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);

            if (HelperUtility.IsNull(emailTemplateIds) || string.IsNullOrEmpty(emailTemplateIds.Ids))
                throw new ZnodeException(ErrorCodes.IdLessThanOne, Admin_Resources.EmailTemplateIdLessThanOne);
            ZnodeLogging.LogMessage("Input parameter emailTemplateIds to delete email templates: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new { emailTemplateIds = emailTemplateIds?.Ids });

            //Generates filter clause for multiple EmailTemplateIds.
            FilterCollection filter = new FilterCollection();
            filter.Add(new FilterTuple(ZnodeEmailTemplateEnum.EmailTemplateId.ToString(), ProcedureFilterOperators.In, emailTemplateIds.Ids));

            //Returns true if mapped email template deleted successfully else return false.
            bool IsDeleted = _emailTemplateMapperRepository.Delete(DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filter.ToFilterDataCollection()).WhereClause);
            ZnodeLogging.LogMessage(IsDeleted ? Admin_Resources.SuccessEmailTemplateMapperDeleted : Admin_Resources.ErrorEmailTemplateMapperDelete, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);

            //Returns true if email template locale deleted successfully else return false.
            IsDeleted = _emailTemplateLocaleRepository.Delete(DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filter.ToFilterDataCollection()).WhereClause);
            ZnodeLogging.LogMessage(IsDeleted ? Admin_Resources.SuccessEmailTemplateLocaleDeleted : Admin_Resources.SuccessEmailTemplateLocaleDelete, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);

            //Returns true if email template deleted successfully else return false.
            IsDeleted = _emailTemplateRepository.Delete(DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filter.ToFilterDataCollection()).WhereClause);
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage(IsDeleted ? Admin_Resources.SuccessEmailTemplateDeleted : Admin_Resources.ErrorEmailTemplateDelete, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            return IsDeleted;
        }

        //Get Email Template tokens.
        public virtual EmailTemplateModel GetEmailTemplateTokens()
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            string filePath = $"{ ZnodeConfigManager.EnvironmentConfig.ConfigPath}{ZnodeConstant.EmailTemplateTokens}{ZnodeConstant.Dot}{ZnodeConstant.TextExtension}";
            EmailTemplateModel emailTemplateModel = new EmailTemplateModel();
            if (ZnodeStorageManager.Exists(filePath))
            {
                emailTemplateModel.EmailTemplateTokens = ZnodeStorageManager.ReadTextStorage(filePath);
                return emailTemplateModel;
            }
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            return emailTemplateModel;
        }

        //Get the Email Template Area List 
        public virtual EmailTemplateAreaListModel GetEmailTemplateAreaList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            //Bind the Filter, sorts & Paging details.
            PageListModel pageListModel = new PageListModel(filters, sorts, page);
            ZnodeLogging.LogMessage("pageListModel generated to get emailTemplateList: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, pageListModel?.ToDebugString());

            //maps the entity list to model
            IList<ZnodeEmailTemplateArea> emailTemplateList = _emailTemplateAreaRepository.GetPagedList(pageListModel.EntityWhereClause.WhereClause, pageListModel.OrderBy, null, pageListModel.EntityWhereClause.FilterValues, pageListModel.PagingStart, pageListModel.PagingLength, out pageListModel.TotalRowCount);
            ZnodeLogging.LogMessage("emailTemplateList count: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, emailTemplateList?.Count);

            EmailTemplateAreaListModel listModel = new EmailTemplateAreaListModel();
            listModel.EmailTemplatesAreaList = emailTemplateList?.Count > 0 ? emailTemplateList.ToModel<EmailTemplateAreaModel>().ToList() : new List<EmailTemplateAreaModel>();

            //Set for pagination
            listModel.BindPageListModel(pageListModel);
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            return listModel;
        }

        //Get the Email Template Area Mapper list
        public virtual EmailTemplateAreaMapperListModel GetEmailTemplateAreaMapperList(int portalId)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameter portalId: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, portalId);

            EmailTemplateAreaMapperListModel model = new EmailTemplateAreaMapperListModel();

            //Get the Mapped Email Template Details.
            List<EmailTemplateAreaMapperModel> areaMapperDetails = (from mapper in _emailTemplateMapperRepository.Table
                                                                    join template in _emailTemplateRepository.Table on mapper.EmailTemplateId equals template.EmailTemplateId
                                                                    join templateArea in _emailTemplateAreaRepository.Table on mapper.EmailTemplateAreasId equals templateArea.EmailTemplateAreasId
                                                                    where mapper.PortalId == portalId
                                                                    select new EmailTemplateAreaMapperModel()
                                                                    {
                                                                        EmailTemplateMapperId = mapper.EmailTemplateMapperId,
                                                                        EmailTemplateId = mapper.EmailTemplateId,
                                                                        EmailTemplateAreasId = mapper.EmailTemplateAreasId,
                                                                        EmailTemplateName = template.TemplateName,
                                                                        EmailTemplateAreaName = templateArea.Name,
                                                                        PortalId = mapper.PortalId,
                                                                        IsActive = mapper.IsActive,
                                                                        IsEnableBcc=mapper.IsEnableBcc,
                                                                        IsSMSNotificationActive = mapper.IsSmsNotificationActive,
                                                                    }).ToList();
            ZnodeLogging.LogMessage("areaMapperDetails list count: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, areaMapperDetails?.Count);
            model.EmailTemplatesAreaMapperList = areaMapperDetails;
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            return model;
        }

        //Delete Email Template Area Configuration.
        public virtual bool DeleteEmailTemplateAreaConfiguration(ParameterModel parameterModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);

            if (parameterModel?.Ids?.Count() <= 0)
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.IdCanNotBeLessThanOne);

            ZnodeLogging.LogMessage("Input parameter to delete email template area configuration: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, parameterModel?.Ids);

            //Set the Filter Collection
            FilterCollection filters = new FilterCollection();
            filters.Add(new FilterTuple(ZnodeEmailTemplateMapperEnum.EmailTemplateMapperId.ToString(), ProcedureFilterOperators.In, parameterModel?.Ids.ToString()));

            //Delete the Template Area Configuration.
            return _emailTemplateMapperRepository.Delete(DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollection()).WhereClause);
        }

        //Create or Update the email template area configuration.
        public virtual bool SaveEmailTemplateAreaConfiguration(EmailTemplateAreaMapperModel model)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            if (HelperUtility.IsNull(model))
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.ModelCanNotBeNull);
            ZnodeLogging.LogMessage("Input parameter EmailTemplateAreasId of EmailTemplateAreaMapperModel: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new { EmailTemplateAreasId = model?.EmailTemplateAreasId});

            ZnodeEmailTemplateArea emailTemplateArea = _emailTemplateAreaRepository.GetById(model.EmailTemplateAreasId);
            if (emailTemplateArea.Name == ResetPassword)
                model.IsEnableBcc = false;

            bool status = false;
            //Check for Existing Email Template Mapped Id.
            if (model?.EmailTemplateMapperId > 0)
                //Update the email template area configuration.
                status = _emailTemplateMapperRepository.Update(model.ToEntity<ZnodeEmailTemplateMapper>());
            else
            {
                //Create the email template area configuration.
                ZnodeEmailTemplateMapper linkWidgetConfiguration = _emailTemplateMapperRepository.Insert(model.ToEntity<ZnodeEmailTemplateMapper>());

                ZnodeLogging.LogMessage((linkWidgetConfiguration?.EmailTemplateMapperId > 0) ? Admin_Resources.SuccessEmailTemplateAreaMappingInserted : Admin_Resources.ErrorEmailTemplateAreaMappingInsert, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
                status = HelperUtility.IsNotNull(linkWidgetConfiguration);
            }
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            return status;
        }
        #endregion

        #region Private Methods      
        //Get expands and add them to navigation properties
        private List<string> GetExpands(NameValueCollection expands)
        {
            List<string> navigationProperties = new List<string>();
            if (HelperUtility.IsNotNull(expands) && expands.HasKeys())
            {
                foreach (string key in expands.Keys)
                {
                    string value = expands.Get(key);
                    if (Equals(value.ToLower(), ExpandKeys.ZnodeEmailTemplateLocale.ToLower())) { SetExpands(ZnodeEmailTemplateEnum.ZnodeEmailTemplateLocales.ToString(), navigationProperties); }
                }
            }
            return navigationProperties;
        }

        //Get where clause for EmailTemplate id.
        private EntityWhereClauseModel GetWhereClauseForEmailTemplateId(int emailTemplateId)
            => DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(new FilterCollection { new FilterTuple(ZnodeEmailTemplateEnum.EmailTemplateId.ToString(), ProcedureFilterOperators.Equals, emailTemplateId.ToString()) }.ToFilterDataCollection());

        //Sets the properties of Email Template model.
        private void SetEmailTemplateModel(ZnodeEmailTemplate emailTemplateEntity, EmailTemplateModel emailTemplateModel, int localeId)
        {
            ZnodeLogging.LogMessage("Input parameters EmailTemplateId of emailTemplateModel and localeId: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new { EmailTemplateId = emailTemplateModel?.EmailTemplateId, localeId = localeId });

            //get the slider nammer locale entity.
            ZnodeEmailTemplateLocale emailTemplateLocale = emailTemplateEntity.ZnodeEmailTemplateLocales.Where(x => x.EmailTemplateId == emailTemplateEntity.EmailTemplateId && x.LocaleId == localeId)?.FirstOrDefault();

            //Set the properties.
            if (HelperUtility.IsNotNull(emailTemplateLocale))
            {
                //get the slider nammer locale entity.
                emailTemplateLocale = emailTemplateEntity.ZnodeEmailTemplateLocales.FirstOrDefault(x => x.EmailTemplateId == emailTemplateEntity.EmailTemplateId && x.LocaleId == localeId);
            }

            //Set the properties.
            if (HelperUtility.IsNotNull(emailTemplateLocale))
            {
                emailTemplateModel.LocaleId = emailTemplateLocale.LocaleId;
                emailTemplateModel.Descriptions = emailTemplateLocale.Descriptions;
                emailTemplateModel.Subject = emailTemplateLocale.Subject;
                emailTemplateModel.Html = emailTemplateLocale.Content;
                emailTemplateModel.SmsContent = emailTemplateLocale.SmsContent;
            }
        }

        //Get the locale id from filters.
        private static int GetLocaleId(FilterCollection filters)
        {
            int localeId = 0;
            if (filters?.Count > 0)
            {
                Int32.TryParse(filters.FirstOrDefault(x => x.FilterName.Equals(ZnodeLocaleEnum.LocaleId.ToString(), StringComparison.InvariantCultureIgnoreCase))?.FilterValue, out localeId);
                filters.RemoveAll(x => x.FilterName.Equals(ZnodeLocaleEnum.LocaleId.ToString(), StringComparison.InvariantCultureIgnoreCase));
            }
            return localeId;
        }

        //Save the data into email template locale.
        private void SaveInEmailTemplateLocale(EmailTemplateModel emailTemplateModel)
        {
            ZnodeLogging.LogMessage("Input parameter EmailTemplateId of emailTemplateModel: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, emailTemplateModel?.EmailTemplateId);
            //Get the Email Template locale.
            int emailTemplateLocaleId = _emailTemplateLocaleRepository.Table.Where(x => x.EmailTemplateId == emailTemplateModel.EmailTemplateId && x.LocaleId == emailTemplateModel.LocaleId).Select(x => x.EmailTemplateLocaleId).FirstOrDefault();
            ZnodeLogging.LogMessage("emailTemplateLocaleId generated: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, emailTemplateLocaleId);

            //Locale present for that email template then update the entry else create the entry.
            if (emailTemplateLocaleId > 0)
            {
                emailTemplateModel.EmailTemplateLocaleId = emailTemplateLocaleId;
                ZnodeLogging.LogMessage(_emailTemplateLocaleRepository.Update(emailTemplateModel?.ToEntity<ZnodeEmailTemplateLocale>())
                    ? Admin_Resources.SuccessEmailTemplateLocaleUpdated : Admin_Resources.ErrorEmailTemplateLocaleUpdate, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            }
            else
                ZnodeLogging.LogMessage(_emailTemplateLocaleRepository.Insert(emailTemplateModel?.ToEntity<ZnodeEmailTemplateLocale>())?.EmailTemplateLocaleId > 0
                    ? Admin_Resources.SuccessEmailTemplateLocaleInserted : Admin_Resources.ErrorInsertEmailTemplateLocale, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
        }

        #endregion
    }
}
