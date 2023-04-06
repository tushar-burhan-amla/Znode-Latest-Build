using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using Znode.Engine.Api.Models;
using Znode.Engine.Exceptions;
using Znode.Engine.Services.Maps;
using Znode.Libraries.Data;
using Znode.Libraries.Data.DataModel;
using Znode.Libraries.Data.Helpers;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;
using Znode.Libraries.Resources;
using static Znode.Libraries.ECommerce.Utilities.ZnodeDependencyResolver;
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;
using System;

namespace Znode.Engine.Services
{
    public class TemplateService : BaseService, ITemplateService
    {
        #region Private Variable
        private readonly IZnodeRepository<ZnodeCMSTemplate> _cmsTemplateRepository;
        private readonly IZnodeRepository<ZnodeMedia> _mediaRepository;
        #endregion

        #region Constructor
        public TemplateService()
        {
            _cmsTemplateRepository = new ZnodeRepository<ZnodeCMSTemplate>();
            _mediaRepository = new ZnodeRepository<ZnodeMedia>();
        }
        #endregion

        #region Public Methods

        //Get Template List
        public virtual TemplateListModel GetTemplates(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            //Bind the Filter, sorts & Paging details.
            PageListModel pageListModel = new PageListModel(filters, sorts, page);
            ZnodeLogging.LogMessage("pageListModel generated to get Templates list: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, pageListModel?.ToDebugString());

            TemplateListModel listModel = new TemplateListModel();
            listModel.Templates = GetTemplateList(pageListModel);
            ZnodeLogging.LogMessage("Templates list count: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, listModel.Templates?.Count());

            listModel.BindPageListModel(pageListModel);
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            return listModel;
        }

        //Create Template.
        public virtual TemplateModel CreateTemplate(TemplateModel templateModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            if (HelperUtility.IsNull(templateModel))
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.ErrorTemplateModelNull);

            if (NameAlreadyExists(templateModel.Name))
                throw new ZnodeException(ErrorCodes.AlreadyExist, Admin_Resources.ErrorNameExists);

            ZnodeLogging.LogMessage("Input parameter templateModel: ", ZnodeLogging.Components.Warehouse.ToString(), TraceLevel.Verbose, templateModel);

            //Create new Template and return it.
            ZnodeCMSTemplate template = _cmsTemplateRepository.Insert(templateModel.ToEntity<ZnodeCMSTemplate>());
            ZnodeLogging.LogMessage("Template inserted with CMSTemplateId: ", ZnodeLogging.Components.Warehouse.ToString(), TraceLevel.Verbose, template?.CMSTemplateId);

            ZnodeLogging.LogMessage((template?.CMSTemplateId < 0) ? Admin_Resources.ErrorInsertTemplate : Admin_Resources.SuccessTemplateInserted, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);

            if (HelperUtility.IsNotNull(template))
                return template.ToModel<TemplateModel>();

            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            return templateModel;
        }

        //Get template by cmsTemplateId.
        public virtual TemplateModel GetTemplate(int cmsTemplateId, NameValueCollection expands)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            if (cmsTemplateId < 1)
                throw new ZnodeException(ErrorCodes.IdLessThanOne, Admin_Resources.ErrorCMSTemplateIdLessThanOne);
            ZnodeLogging.LogMessage("Input parameter cmsTemplateId value: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, cmsTemplateId);

            FilterCollection filter = new FilterCollection();
            filter.Add(new FilterTuple(ZnodeCMSTemplateEnum.CMSTemplateId.ToString(), FilterOperators.Equals, cmsTemplateId.ToString()));
            EntityWhereClauseModel whereClause = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filter.ToFilterDataCollection());
            ZnodeLogging.LogMessage("whereClause generated to get template: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, whereClause.WhereClause);

            TemplateModel templateModel = GetTemplate(whereClause);
            return templateModel;
        }

        //Update Template.
        public virtual bool UpdateTemplate(TemplateModel templateModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            if (HelperUtility.IsNull(templateModel))
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.ErrorTemplateModelNull);
            ZnodeLogging.LogMessage("Input parameter templateModel value: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, templateModel);

            if (_cmsTemplateRepository.Table.Any(x => x.Name.Trim() == templateModel.Name.Trim() && x.CMSTemplateId != templateModel.CMSTemplateId))
                throw new ZnodeException(ErrorCodes.AlreadyExist, Admin_Resources.ErrorNameExists);
            ZnodeLogging.LogMessage("CMSTemplateId value: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, templateModel.CMSTemplateId);

            ZnodeLogging.LogMessage(_cmsTemplateRepository.Update(templateModel.ToEntity<ZnodeCMSTemplate>()) ? Admin_Resources.SuccessTemplateUpdated : Admin_Resources.ErrorTemplateUpdate, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            return true;
        }


        //Delete Template.
        public virtual bool DeleteTemplate(ParameterModel cmsTemplateIds)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);

            if (HelperUtility.IsNull(cmsTemplateIds) || string.IsNullOrEmpty(cmsTemplateIds.Ids))
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.ErrorCMSTemplateIdsEmpty);
            ZnodeLogging.LogMessage("Input parameter cmsTemplateIds to delete template: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, cmsTemplateIds?.Ids);

            FilterCollection filter = new FilterCollection();
            filter.Add(new FilterTuple(ZnodeCMSTemplateEnum.CMSTemplateId.ToString(), FilterOperators.In, cmsTemplateIds.Ids));
            EntityWhereClauseModel whereClause = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filter.ToFilterDataCollection());
            ZnodeLogging.LogMessage("whereClause to get associatedContentPages list: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, whereClause.WhereClause);

            bool isAssociatedDelete = false;
            bool isDeleted = false;

            IZnodeRepository<ZnodeCMSContentPage> _cmsContentPage = new ZnodeRepository<ZnodeCMSContentPage>();
            List<ZnodeCMSContentPage> associatedContentPages = _cmsContentPage.GetEntityList(whereClause.WhereClause, whereClause.FilterValues)?.ToList();
            ZnodeLogging.LogMessage("associatedContentPages list count: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, associatedContentPages?.Count());

            //If templates are associated with content pages.
            if (associatedContentPages?.Count > 0)
            {
                isAssociatedDelete = true;
                List<string> allTemplates = cmsTemplateIds.Ids.Split(',').ToList();
                //Remove all the templates which are associated with Content pages.
                allTemplates.RemoveAll(x => associatedContentPages.Select(associated => associated.CMSTemplateId.ToString()).ToList().Contains(x));
                isDeleted = allTemplates.Count == 0;
                if (!isDeleted)
                {
                    //Create where clause for templates to delete(which are not associated).
                    filter = new FilterCollection();
                    filter.Add(new FilterTuple(ZnodeCMSTemplateEnum.CMSTemplateId.ToString(), FilterOperators.In, string.Join(",", allTemplates)));
                    whereClause = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filter.ToFilterDataCollection());
                }
            }
            if (!isDeleted)
            {
                isDeleted = _cmsTemplateRepository.Delete(whereClause.WhereClause, whereClause.FilterValues);
                ZnodeLogging.LogMessage(isDeleted ? Admin_Resources.SuccessTemplateDeleted : Admin_Resources.ErrorTemplateDelete, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            }

            if (isAssociatedDelete)
                throw new ZnodeException(ErrorCodes.AssociationDeleteError, Admin_Resources.ErrorTemplateDeleteSomeAssociation);

            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            return isDeleted;
        }
        #endregion

        #region Private Methods
        //Returns true if name already exists.
        private bool NameAlreadyExists(string name)
            => _cmsTemplateRepository.Table.Any(x => x.Name.Trim() == name.Trim());

        //This method is used to get media path.        
        private TemplateModel GetMediaPath(TemplateModel templateModel, string serverPath, string thumbnailFolderName)
        {
            string mediaServerPath = string.IsNullOrEmpty(templateModel.MediaPath) ? string.Empty : $"{serverPath}{templateModel.MediaPath}?v={templateModel.Version}";
            string mediaServerThumbnailPath = string.IsNullOrEmpty(templateModel.MediaPath) ? string.Empty : $"{serverPath}{thumbnailFolderName}/{templateModel.MediaPath}?v={templateModel.Version}";
            templateModel.MediaPath = mediaServerPath;
            templateModel.MediaThumbNailPath = mediaServerThumbnailPath;
            return templateModel;
        }

        //Get template list with images
        private List<TemplateModel> GetTemplateList(PageListModel pageListModel)
        {
            var templateList = (from _template in _cmsTemplateRepository.GetPagedList(pageListModel.EntityWhereClause.WhereClause, pageListModel.OrderBy, null, pageListModel.EntityWhereClause.FilterValues, pageListModel.PagingStart, pageListModel.PagingLength, out pageListModel.TotalRowCount)
                                join _media in _mediaRepository.Table on _template.MediaId equals _media.MediaId
                                into templateMedia
                                from tm in templateMedia.DefaultIfEmpty()
                                select new TemplateModel
                                {
                                    CMSTemplateId = _template.CMSTemplateId,
                                    Name = _template.Name,
                                    FileName = _template.FileName,
                                    MediaId = _template.MediaId,
                                    MediaPath = tm == null ? string.Empty : (tm.Path ?? string.Empty),
                                    Version = tm == null ? 0 : (tm.Version)
                                }).ToList();

            MediaConfigurationModel configurationModel = GetService<IMediaConfigurationService>().GetDefaultMediaConfiguration();
            string serverPath = GetMediaServerUrl(configurationModel);
            //Get image path for templates.
            templateList?.ForEach(
                x =>
                {
                    x.MediaPath = GetMediaPath(x, serverPath, configurationModel.ThumbnailFolderName).MediaThumbNailPath;

                });
            return templateList;

        }

        //Get template  with image
        private TemplateModel GetTemplate(EntityWhereClauseModel whereClause)
        {
            TemplateModel templateModel = _cmsTemplateRepository.GetEntity(whereClause.WhereClause, whereClause.FilterValues)?.ToModel<TemplateModel>();

            //Get image path for template.
            if (IsNotNull(templateModel) && IsNotNull(templateModel.MediaId))
            {
                var media = _mediaRepository.Table.FirstOrDefault(x => x.MediaId == templateModel.MediaId);
                if (IsNotNull(media))
                {
                    templateModel.MediaPath = media.Path;
                    templateModel.Version = media.Version;
                    //Get path for image from thumbnail
                    MediaConfigurationModel configurationModel = GetService<IMediaConfigurationService>().GetDefaultMediaConfiguration();
                    string serverPath = GetMediaServerUrl(configurationModel);

                    templateModel.MediaPath = GetMediaPath(templateModel, serverPath, configurationModel.ThumbnailFolderName).MediaThumbNailPath;
                }

                ZnodeLogging.LogMessage("MediaId and MediaPath properties of TemplateModel: ", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Verbose, new { MediaId = templateModel?.MediaId, MediaPath = templateModel?.MediaPath });
            }
            return templateModel;

        }
        #endregion
    }
}
