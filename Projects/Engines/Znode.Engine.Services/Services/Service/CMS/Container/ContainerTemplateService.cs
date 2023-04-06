using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using Znode.Engine.Api.Models;
using Znode.Libraries.Data;
using Znode.Libraries.Data.DataModel;
using Znode.Libraries.ECommerce.Utilities;
using static Znode.Libraries.ECommerce.Utilities.ZnodeDependencyResolver;
using Znode.Libraries.Framework.Business;
using Znode.Engine.Exceptions;
using Znode.Libraries.Resources;
using System.Data;

namespace Znode.Engine.Services
{
    public class ContainerTemplateService : BaseService, IContainerTemplateService
    {
        #region Private Variable
        protected readonly IZnodeRepository<ZnodeCMSContainerTemplate> _containerTemplateRepository;
        protected readonly IZnodeRepository<ZnodeMedia> _mediaRepository;
        #endregion

        #region Constructor
        public ContainerTemplateService()
        {
            _containerTemplateRepository = new ZnodeRepository<ZnodeCMSContainerTemplate>();
            _mediaRepository = new ZnodeRepository<ZnodeMedia>();
        }
        #endregion

        #region Public Methods

        //Get the List of Container Template
        public virtual ContainerTemplateListModel GetContainerTemplateList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
 
            //Bind the Filter, sorts & Paging details.
            PageListModel pageListModel = new PageListModel(filters, sorts, page);

            ContainerTemplateListModel listModel = new ContainerTemplateListModel();
            listModel.ContainerTemplates = ContainerTemplateList(pageListModel);

            listModel.BindPageListModel(pageListModel);
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            return listModel;
        }

        //Create Container Template
        public virtual ContainerTemplateModel CreateContainerTemplate(ContainerTemplateCreateModel containerTemplate)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);

            if (Equals(containerTemplate, null))
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.ModelNotNull);

            //Validate if the name already exists
            if (IsContainerTemplateExists(containerTemplate.Code))
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.TemplateCodeExists);

            if(string.IsNullOrEmpty(containerTemplate.FileName))
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.InvalidData);

            ZnodeCMSContainerTemplate model = _containerTemplateRepository.Insert(containerTemplate.ToEntity<ZnodeCMSContainerTemplate>());

            return model.ToModel<ContainerTemplateModel>();
        }

        //Get Container Template
        public virtual ContainerTemplateModel GetContainerTemplate(string templateCode)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);

            if (string.IsNullOrEmpty(templateCode))
                throw new ZnodeException(ErrorCodes.IdLessThanOne, Admin_Resources.InvalidTemplateCode);

            ContainerTemplateModel model = _containerTemplateRepository.Table.FirstOrDefault(x => x.Code.ToLower() == templateCode.ToLower()).ToModel<ContainerTemplateModel>();
            GetMediaDetails(model);
            return model;
        }
        
        //Update Container Template
        public virtual ContainerTemplateModel UpdateContainerTemplate(ContainerTemplateUpdateModel containerTemplateModel)
        {

            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);

            int templateId = GetTemplateId(containerTemplateModel.Code);

            ZnodeCMSContainerTemplate model = _containerTemplateRepository.Table.FirstOrDefault(x => x.CMSContainerTemplateId == templateId);

            if (Equals(model, null))
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.ErrorModelNull);

            model.FileName = containerTemplateModel.FileName;
            model.MediaId = containerTemplateModel.MediaId;
            model.Name = containerTemplateModel.Name;

            bool isUpdated = _containerTemplateRepository.Update(model);

            return model.ToModel<ContainerTemplateModel>();
        }

        //Delete Template by Code
        public virtual bool DeleteContainerTemplateByCode(string templateCode)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            int containerTemplateId = GetTemplateId(templateCode);
            return DeleteContainerTemplate(containerTemplateId.ToString());

        }

        //Delete Template By Ids
        public virtual bool DeleteContainerTemplateById(ParameterModel ContainerTemplateIds)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            return DeleteContainerTemplate(ContainerTemplateIds.Ids);
        }


        //Validate if the Container Template Exists
        public virtual bool IsContainerTemplateExists(string templateCode)
        {
            if (string.IsNullOrEmpty(templateCode))
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.InvalidTemplateCode);

            return _containerTemplateRepository.Table.Any(x => x.Code.ToLower() == templateCode.ToLower());
        }


        #endregion

        #region Private Methods

        //Delete Container Template
        protected virtual bool DeleteContainerTemplate(string ContainerTemplateIds)
        {
            int status = 0;
            IZnodeViewRepository<View_ReturnBoolean> objStoredProc = new ZnodeViewRepository<View_ReturnBoolean>();
            objStoredProc.SetParameter("@ContainerTemplateIds", ContainerTemplateIds, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@Status", null, ParameterDirection.Output, DbType.Int32);
            var deleteResult = objStoredProc.ExecuteStoredProcedureList("Znode_DeleteCMSContainerTemplates @ContainerTemplateIds,@Status OUT", 1, out status);

            if (deleteResult.FirstOrDefault().Status.Value)
            {
                ZnodeLogging.LogMessage(Admin_Resources.DeleteMessage, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
                return true;
            }
            else
            {
                throw new ZnodeException(ErrorCodes.AssociationDeleteError, Admin_Resources.ErrorFailedToDelete);
            }

        }

        //Get media Details
        protected virtual void GetMediaDetails(ContainerTemplateModel model)
        {
            if (HelperUtility.IsNotNull(model) && HelperUtility.IsNotNull(model.MediaId))
            {
                ZnodeMedia media = _mediaRepository.Table.FirstOrDefault(x => x.MediaId == model.MediaId);
                if (HelperUtility.IsNotNull(media))
                {
                    model.MediaPath = media.Path;
                    model.Version = media.Version;
                    MediaConfigurationModel configurationModel = GetService<IMediaConfigurationService>().GetDefaultMediaConfiguration();
                    string serverPath = GetMediaServerUrl(configurationModel);

                    model.MediaPath = GetMediaPath(model, serverPath, configurationModel.ThumbnailFolderName).MediaThumbNailPath;
                }

            }

        }

        //Get container template list
        protected virtual List<ContainerTemplateModel> ContainerTemplateList(PageListModel pageListModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            ContainerTemplateListModel templateListModel = new ContainerTemplateListModel();

            IZnodeViewRepository<ContainerTemplateModel> objStoredProc = new ZnodeViewRepository<ContainerTemplateModel>();
            objStoredProc.SetParameter("@WhereClause", pageListModel.SPWhereClause, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@Rows", pageListModel.PagingLength, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@PageNo", pageListModel.PagingStart, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@Order_BY", pageListModel.OrderBy, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@RowsCount", pageListModel.TotalRowCount, ParameterDirection.Output, DbType.Int32);

            IList<ContainerTemplateModel> templateList = objStoredProc.ExecuteStoredProcedureList("Znode_GetCMSContainerTemplatelist @WhereClause,@Rows,@PageNo,@Order_BY,@RowsCount OUT", 4, out pageListModel.TotalRowCount);

            templateListModel.ContainerTemplates = templateList?.Count > 0 ? templateList?.ToList() : null;

            MediaConfigurationModel configurationModel = GetService<IMediaConfigurationService>().GetDefaultMediaConfiguration();
            string serverPath = GetMediaServerUrl(configurationModel);

            templateListModel.ContainerTemplates?.ForEach(
                x =>
                {
                    x.MediaPath = GetMediaPath(x, serverPath, configurationModel.ThumbnailFolderName).MediaThumbNailPath;

                });
            return templateListModel.ContainerTemplates;

        }

        //Get media path
        protected virtual ContainerTemplateModel GetMediaPath(ContainerTemplateModel templateModel, string serverPath, string thumbnailFolderName)
        {
            string mediaServerPath = string.IsNullOrEmpty(templateModel.MediaPath) ? string.Empty : $"{serverPath}{templateModel.MediaPath}?v={templateModel.Version}";
            string mediaServerThumbnailPath = string.IsNullOrEmpty(templateModel.MediaPath) ? string.Empty : $"{serverPath}{thumbnailFolderName}/{templateModel.MediaPath}?v={templateModel.Version}";
            templateModel.MediaPath = mediaServerPath;
            templateModel.MediaThumbNailPath = mediaServerThumbnailPath;
            return templateModel;
        }

        //Get Container Template Id
        private int GetTemplateId(string code)
        {
            int containerTemplateId = (from templateRepository in _containerTemplateRepository.Table.Where(x => x.Code.ToLower() == code.ToLower()) select templateRepository.CMSContainerTemplateId).FirstOrDefault();

            if (containerTemplateId < 1)
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.InvalidTemplateCode);

            return containerTemplateId;

        }
        #endregion
    }
}
