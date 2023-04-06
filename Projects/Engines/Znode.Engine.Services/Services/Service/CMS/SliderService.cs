using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Diagnostics;
using System.Linq;
using Znode.Engine.Api.Models;
using Znode.Engine.Exceptions;
using Znode.Engine.Services.Maps;
using Znode.Libraries.Data;
using Znode.Libraries.Data.DataModel;
using Znode.Libraries.Data.Helpers;
using Znode.Libraries.ECommerce.Utilities;
using static Znode.Libraries.ECommerce.Utilities.ZnodeDependencyResolver;
using Znode.Libraries.Framework.Business;
using Znode.Libraries.Resources;
using Znode.Engine.Services.Helper;
using Znode.Libraries.Caching.Events;

namespace Znode.Engine.Services
{
    public class SliderService : BaseService, ISliderService
    {
        #region Private Variable
        private readonly IZnodeRepository<ZnodeCMSSlider> _cmsSliderRepository;
        private readonly IZnodeRepository<ZnodeCMSSliderBanner> _cmsSliderBannerRepository;
        private readonly IZnodeRepository<ZnodeCMSSliderBannerLocale> _cmsSliderBannerLocaleRepository;
        private readonly IZnodeRepository<ZnodeCMSWidgetSliderBanner> _cmsWidgetSliderBannerRepository;
        private readonly IZnodeRepository<ZnodePublishPortalLog> _publishPortalLogRepository;
        #endregion

        #region Constructor
        public SliderService()
        {
            _cmsSliderRepository = new ZnodeRepository<ZnodeCMSSlider>();
            _cmsSliderBannerRepository = new ZnodeRepository<ZnodeCMSSliderBanner>();
            _cmsSliderBannerLocaleRepository = new ZnodeRepository<ZnodeCMSSliderBannerLocale>();
            _cmsWidgetSliderBannerRepository = new ZnodeRepository<ZnodeCMSWidgetSliderBanner>();
            _publishPortalLogRepository = new ZnodeRepository<ZnodePublishPortalLog>();
        }
        #endregion

        #region Public Methods
        #region Slider
        //Get Slider List
        public virtual SliderListModel GetSliders(FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            //Set filter, sort and paging parameters.
            PageListModel pageListModel = new PageListModel(filters, sorts, page);

            SliderListModel listModel = new SliderListModel();

            ZnodeLogging.LogMessage("pageListModel to get slider list : ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, pageListModel?.ToDebugString());
            List<ZnodeCMSSlider> sliders = _cmsSliderRepository.GetPagedList(pageListModel.EntityWhereClause.WhereClause, pageListModel.OrderBy, new List<string> { "ZnodePublishState" }, pageListModel.EntityWhereClause.FilterValues, pageListModel.PagingStart, pageListModel.PagingLength, out pageListModel.TotalRowCount).ToList();

            listModel.Sliders = sliders.Select(x => new SliderModel
            {
                CMSSliderId = x.CMSSliderId,
                IsPublished = x.IsPublished,
                Name = x.Name,
                PublishStatus = x.ZnodePublishState?.DisplayName,
                PublishStateId = x.PublishStateId
            }).ToList();
            listModel.BindPageListModel(pageListModel);
            ZnodeLogging.LogMessage("slider list count: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, listModel?.Sliders?.Count);
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            return listModel;
        }

        //Create slider.
        public virtual SliderModel CreateSlider(SliderModel sliderModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            if (HelperUtility.IsNull(sliderModel))
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.ModelCanNotBeNull);

            //Check if highlight name already exists.
            if (_cmsSliderRepository.Table.Count(x => Equals(x.Name.Trim(), sliderModel.Name.Trim())) > 0)
                throw new ZnodeException(ErrorCodes.AlreadyExist, Admin_Resources.ErrorSliderExists);

            ZnodeCMSSlider sliderToInsert = sliderModel.ToEntity<ZnodeCMSSlider>();

            if (HelperUtility.IsNotNull(sliderToInsert))
                sliderToInsert.PublishStateId = (byte)ZnodePublishStatesEnum.NOT_PUBLISHED;

            //Create new slider and return it.
            ZnodeCMSSlider slider = _cmsSliderRepository.Insert(sliderToInsert);

            ZnodeLogging.LogMessage((slider?.CMSSliderId < 0) ? Admin_Resources.ErrorSliderInsert : string.Format(Admin_Resources.SuccessSliderInserted, slider?.CMSSliderId), ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);

            if (HelperUtility.IsNotNull(slider))
                return slider.ToModel<SliderModel>();

            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            return sliderModel;
        }

        //Get slider by cmsSliderId.
        public virtual SliderModel GetSlider(int cmsSliderId)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            SliderModel model = new SliderModel();
            if (cmsSliderId > 0)
            {
                ZnodeLogging.LogMessage("cmsSliderId to get slider: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, cmsSliderId);
                model = (from cmsSlider in _cmsSliderRepository.Table
                         join cmsWidgetSlider in _cmsWidgetSliderBannerRepository.Table on cmsSlider.CMSSliderId equals cmsWidgetSlider.CMSSliderId into widgetSliderDetail
                         from cmsWidgetSliderDetail in widgetSliderDetail.DefaultIfEmpty()
                         where cmsSlider.CMSSliderId == cmsSliderId
                         select new SliderModel
                         {
                             CMSSliderId = cmsSliderId,
                             Name = cmsSlider.Name,
                             IsWidgetAssociated = cmsWidgetSliderDetail.WidgetsKey != null
                         }).FirstOrDefault();
            }
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            return model;
        }
        //Update Slider.
        public virtual bool UpdateSlider(SliderModel sliderModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            if (HelperUtility.IsNull(sliderModel))
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.ModelCanNotBeNull);

            //Check if highlight name already exists.
            if (_cmsSliderRepository.Table.Count(x => Equals(x.Name.Trim(), sliderModel.Name.Trim()) && x.CMSSliderId != sliderModel.CMSSliderId) > 0)
                throw new ZnodeException(ErrorCodes.AlreadyExist, Admin_Resources.ErrorSliderExists);

            if (sliderModel.CMSSliderId < 1)
                throw new ZnodeException(ErrorCodes.IdLessThanOne, Admin_Resources.ErrorCMSSliderIdLessThanOne);

            if (Equals(sliderModel.PublishStateId, null) || sliderModel.PublishStateId == 0)
            {
                sliderModel.PublishStateId = (byte)ZnodePublishStatesEnum.DRAFT;
            }
            //Update slider
            bool isSliderUpdated = _cmsSliderRepository.Update(sliderModel.ToEntity<ZnodeCMSSlider>());
            ZnodeLogging.LogMessage(isSliderUpdated ? string.Format(Admin_Resources.SuccessSliderUpdated, sliderModel?.CMSSliderId) : Admin_Resources.ErrorSliderUpdate, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            return isSliderUpdated;
        }

        //Delete slider.
        public virtual bool DeleteSlider(ParameterModel cmsPortalSliderIds)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            if (HelperUtility.IsNull(cmsPortalSliderIds) || string.IsNullOrEmpty(cmsPortalSliderIds.Ids))
                throw new ZnodeException(ErrorCodes.IdLessThanOne, Admin_Resources.ErrorCMSPortalSliderIdLessThanOne);

            IZnodeViewRepository<View_ReturnBoolean> objStoredProc = new ZnodeViewRepository<View_ReturnBoolean>();
            objStoredProc.SetParameter(ZnodeCMSSliderEnum.CMSSliderId.ToString(), cmsPortalSliderIds.Ids, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("Status", null, ParameterDirection.Output, DbType.Int32);
            int status = 0;
            ZnodeLogging.LogMessage("Slider with Ids to be deleted: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, cmsPortalSliderIds?.Ids);
            IList<View_ReturnBoolean> deleteResult = objStoredProc.ExecuteStoredProcedureList("Znode_DeleteCMSSlider @CMSSliderId,  @Status OUT", 1, out status);
            if (deleteResult.FirstOrDefault().Status.Value)
            {
                ZnodeLogging.LogMessage(Admin_Resources.SuccessSliderDeleted, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
                return true;
            }
            else
            {
                ZnodeLogging.LogMessage(Admin_Resources.ErrorSliderDeleted, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
                throw new ZnodeException(ErrorCodes.AssociationDeleteError, Admin_Resources.ErrorDeleteSlider);
            }
        }

        public virtual PublishedModel PublishSlider(string cmsPortalSliderId, int portalId, int localeId, string targetPublishState = null, bool takeFromDraftFirst = false)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Publish slider banner with the parameters :", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new object[] { cmsPortalSliderId, portalId, portalId, targetPublishState });

            if (string.IsNullOrEmpty(cmsPortalSliderId) || string.IsNullOrWhiteSpace(cmsPortalSliderId))
                throw new ZnodeException(ErrorCodes.InvalidEntityPassedDuringPublish, Api_Resources.InvalidEntityMessageDuringPublish);

            bool isDataPublished = GetService<IPublishPortalDataService>().PublishSlider(cmsPortalSliderId, portalId, localeId, targetPublishState, takeFromDraftFirst);

            if (isDataPublished)
            {
                var sliderIds = new int[] { Convert.ToInt32(cmsPortalSliderId) };

                ClearCacheHelper.EnqueueEviction(new BannerSliderPublishEvent()
                {
                    Comment = "From publishing banner sliders.",
                    SliderIds = sliderIds
                });
            }

            ZnodeLogging.LogMessage("If the Cloudflare is enabled then purge the store URL manually otherwise changes will not reflect on store.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            return new PublishedModel { IsPublished = isDataPublished, ErrorMessage = isDataPublished ? String.Empty : Api_Resources.GenericExceptionMessageDuringPublish };
        }
        #endregion

        #region Banner
        //Gets the list of banner for selected slider.
        public virtual BannerListModel GetBannerList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            PageListModel pageListModel = new PageListModel(filters, sorts, page);

            IZnodeViewRepository<BannerModel> objStoredProc = new ZnodeViewRepository<BannerModel>();

            //SP parameters
            objStoredProc.SetParameter("@WhereClause", pageListModel.SPWhereClause, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@Rows", pageListModel.PagingLength, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@PageNo", pageListModel.PagingStart, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@Order_By", pageListModel.OrderBy, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@RowCount", pageListModel.TotalRowCount, ParameterDirection.Output, DbType.Int32);

            ZnodeLogging.LogMessage("pageListModel to get bannerlist: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, pageListModel?.ToDebugString());
            IList<BannerModel> bannerlist = objStoredProc.ExecuteStoredProcedureList("Znode_GetCMSSliderBannerPath  @WhereClause,@Rows,@PageNo,@Order_By,@RowCount OUT", 4, out pageListModel.TotalRowCount);
            ZnodeLogging.LogMessage("bannerlist count: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, bannerlist?.Count);

            BannerListModel listModel = new BannerListModel { Banners = bannerlist?.ToList() };
            listModel.BindPageListModel(pageListModel);

            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            return listModel;
        }

        //Create banner.
        public virtual BannerModel CreateBanner(BannerModel bannerModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            if (HelperUtility.IsNull(bannerModel))
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.ModelCanNotBeNull);

            //Create new banner and return it.
            ZnodeCMSSliderBanner banner = _cmsSliderBannerRepository.Insert(bannerModel.ToEntity<ZnodeCMSSliderBanner>());
            if (banner?.CMSSliderBannerId > 0)
            {
                ZnodeLogging.LogMessage(Admin_Resources.SuccessBannerInserted, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
                bannerModel.CMSSliderBannerId = banner.CMSSliderBannerId;
                ZnodeCMSSliderBannerLocale bannerLocale = _cmsSliderBannerLocaleRepository.Insert(GetSliderBannerLocaleEntity(bannerModel));
                if (bannerLocale?.CMSSliderBannerLocaleId > 0)
                {
                    ZnodeLogging.LogMessage("CMSSliderId to update publish slider status: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, bannerModel?.CMSSliderId);
                    UpdatePublishSliderStatus(bannerModel.CMSSliderId, ZnodePublishStatesEnum.DRAFT, false);
                }
                ZnodeLogging.LogMessage(bannerLocale?.CMSSliderBannerLocaleId > 0
                    ? string.Format(Admin_Resources.SuccessBannerLocaleInserted, bannerLocale?.CMSSliderBannerLocaleId) : Admin_Resources.ErrorBannerLocaleInsert, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);

                return bannerModel;
            }

            ZnodeLogging.LogMessage(Admin_Resources.ErrorBannerInsert, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            return null;
        }

        //Get banner by cmsSliderBannerId.
        public virtual BannerModel GetBanner(int cmsSliderBannerId, FilterCollection filters)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            if (cmsSliderBannerId < 1)
                throw new ZnodeException(ErrorCodes.IdLessThanOne, Admin_Resources.ErrorSliderBannerIdLessThanOne);

            ZnodeLogging.LogMessage("cmsSliderBannerId to get sliderBannerEntity: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, cmsSliderBannerId);
            ZnodeCMSSliderBanner sliderBannerEntity = _cmsSliderBannerRepository.GetEntity(GetWhereClauseForSliderBannerId(cmsSliderBannerId).WhereClause, new List<string> { "ZnodeCMSSliderBannerLocales" });

            if (HelperUtility.IsNull(sliderBannerEntity))
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.ModelNotNull);

            //Maps to model.
            BannerModel model = sliderBannerEntity.ToModel<BannerModel>();

            //Sets the properties of banner model.
            SetBannerModel(sliderBannerEntity, model, GetLocaleId(filters));

            //Binds the slider name.
            model.Name = _cmsSliderRepository.Table.Where(x => x.CMSSliderId == model.CMSSliderId)?.FirstOrDefault().Name;

            //GetMediaPath method is used to get media path from media Id.
            model.MediaPath = model.MediaId > 0 ? GetMediaPath(model) : string.Empty;
            ZnodeLogging.LogMessage("BannerModel with CMSSliderBannerId and CMSSliderId: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new { CMSSliderBannerId = model.CMSSliderBannerId, CMSSliderId = model.CMSSliderId });
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            return model;
        }

        //Update banner.
        public virtual bool UpdateBanner(BannerModel bannerModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            if (HelperUtility.IsNull(bannerModel))
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.ModelNotNull);

            if (bannerModel.CMSSliderBannerId < 1)
                throw new ZnodeException(ErrorCodes.IdLessThanOne, Admin_Resources.IdCanNotBeLessThanOne);

            //Update banner
            ZnodeLogging.LogMessage("BannerModel with CMSSliderBannerId to be updated: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, bannerModel?.CMSSliderBannerId);
            bool isBannerUpdated = _cmsSliderBannerRepository.Update(bannerModel.ToEntity<ZnodeCMSSliderBanner>());

            //Save the data into slider banner locale.
            SaveInSliderBannerLocale(bannerModel);
            if (isBannerUpdated)
            {
                UpdatePublishSliderStatus(bannerModel.CMSSliderId, ZnodePublishStatesEnum.DRAFT, false);
            }

            ZnodeLogging.LogMessage(isBannerUpdated ? Admin_Resources.SuccessBannerUpdated : Admin_Resources.ErrorBannerUpdate, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            return isBannerUpdated;
        }

        //Delete banner.
        public virtual bool DeleteBanner(ParameterModel cmsSliderBannerIds)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            if (HelperUtility.IsNull(cmsSliderBannerIds) || string.IsNullOrEmpty(cmsSliderBannerIds.Ids))
                throw new ZnodeException(ErrorCodes.IdLessThanOne, Admin_Resources.ErrorCMSSliderBannerIdLessThanOne);

            //Generates filter clause for multiple cmsSliderBannerIds.
            FilterCollection filter = new FilterCollection();
            filter.Add(new FilterTuple(ZnodeCMSSliderBannerEnum.CMSSliderBannerId.ToString(), ProcedureFilterOperators.In, cmsSliderBannerIds.Ids));
            int cmsSliderBannerId = 0;
            Int32.TryParse(cmsSliderBannerIds.Ids.Split(',').FirstOrDefault(), out cmsSliderBannerId);
            ZnodeLogging.LogMessage("cmsSliderBannerId to get cmsSliderBanner: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, cmsSliderBannerId);
            ZnodeCMSSliderBanner cmsSliderBanner = _cmsSliderBannerRepository.GetById(cmsSliderBannerId);
            //Returns true if banner locale deleted successfully else return false.
            ZnodeLogging.LogMessage("CMS slider banner locale with Ids to be deleted: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, cmsSliderBannerIds?.Ids);
            bool IsDeleted = _cmsSliderBannerLocaleRepository.Delete(DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filter.ToFilterDataCollection()).WhereClause);
            ZnodeLogging.LogMessage(IsDeleted ? Admin_Resources.SuccessBannerLocaleDeleted : Admin_Resources.ErrorBannerLocaleDelete, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);

            //Returns true if banner deleted successfully else return false.
            IsDeleted = _cmsSliderBannerRepository.Delete(DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filter.ToFilterDataCollection()).WhereClause);
            if (IsDeleted)
            {
                UpdatePublishSliderStatus((cmsSliderBanner?.CMSSliderId).GetValueOrDefault(), ZnodePublishStatesEnum.DRAFT, false);
            }
            ZnodeLogging.LogMessage(IsDeleted ? Admin_Resources.SuccessBannerDeleted : Admin_Resources.ErrorBannerDelete, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            return IsDeleted;
        }
        #endregion

        #endregion

        #region Private Method







        //Get where clause for Slider banner id.
        private EntityWhereClauseModel GetWhereClauseForSliderBannerId(int sliderBannerId)
            => DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(new FilterCollection { new FilterTuple(ZnodeCMSSliderBannerEnum.CMSSliderBannerId.ToString(), ProcedureFilterOperators.Equals, sliderBannerId.ToString()) }.ToFilterDataCollection());

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

        //Save the data into slider banner locale.
        private void SaveInSliderBannerLocale(BannerModel bannerModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            //Get the banner locale.
            ZnodeCMSSliderBannerLocale bannerLocale = _cmsSliderBannerLocaleRepository.Table.Where(x => x.CMSSliderBannerId == bannerModel.CMSSliderBannerId && x.LocaleId == bannerModel.LocaleId)?.FirstOrDefault();

            //Locale present for that banner the update the entry else create the entry.
            if (HelperUtility.IsNotNull(bannerLocale))
            {
                bannerLocale.Description = bannerModel.Description;
                bannerLocale.ImageAlternateText = bannerModel.ImageAlternateText;
                bannerLocale.Title = bannerModel.Title;
                bannerLocale.MediaId = bannerModel.MediaId;
                bannerLocale.ButtonLabelName = bannerModel.ButtonLabelName;
                bannerLocale.ButtonLink = bannerModel.ButtonLink;

                ZnodeLogging.LogMessage(_cmsSliderBannerLocaleRepository.Update(bannerLocale)
                    ? Admin_Resources.SuccessSliderLocaleBannerUpdated : Admin_Resources.ErrorSliderLocaleBannerUpdate, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            }
            else
                ZnodeLogging.LogMessage(_cmsSliderBannerLocaleRepository.Insert(GetSliderBannerLocaleEntity(bannerModel))?.CMSSliderBannerLocaleId > 0
                    ? Admin_Resources.SuccessSliderLocaleBannerInserted : Admin_Resources.ErrorSliderLocaleBannerInsert, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
        }

        //Get the slider banner locale entity.
        private static ZnodeCMSSliderBannerLocale GetSliderBannerLocaleEntity(BannerModel bannerModel)
            => new ZnodeCMSSliderBannerLocale
            {
                LocaleId = bannerModel.LocaleId,
                CMSSliderBannerId = bannerModel.CMSSliderBannerId,
                Description = bannerModel.Description,
                ImageAlternateText = bannerModel.ImageAlternateText,
                ButtonLabelName = bannerModel.ButtonLabelName,
                ButtonLink = bannerModel.ButtonLink,
                Title = bannerModel.Title,
                MediaId = bannerModel.MediaId
            };

        //Sets the properties of banner model.
        private void SetBannerModel(ZnodeCMSSliderBanner sliderBannerEntity, BannerModel model, int localeId)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            //Get the slider nammer locale entity.
            ZnodeCMSSliderBannerLocale sliderBannerLocale = sliderBannerEntity.ZnodeCMSSliderBannerLocales.Where(x => x.CMSSliderBannerId == sliderBannerEntity.CMSSliderBannerId && x.LocaleId == localeId)?.FirstOrDefault();

            //Gets slider banner default locale details for the locale other than default if it doesn't have its localized details.
            if (!Equals(localeId, GetDefaultLocaleId()) && HelperUtility.IsNull(sliderBannerLocale))
                sliderBannerLocale = sliderBannerEntity.ZnodeCMSSliderBannerLocales.Where(x => x.CMSSliderBannerId == sliderBannerEntity.CMSSliderBannerId && x.LocaleId == GetDefaultLocaleId())?.FirstOrDefault();

            ZnodeLogging.LogMessage("ZnodeCMSSliderBannerLocale and ZnodeCMSSliderBanner with Ids respectively: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new object[] { sliderBannerLocale?.CMSSliderBannerLocaleId, sliderBannerEntity?.CMSSliderBannerId });
            //Set the properties.
            if (HelperUtility.IsNotNull(sliderBannerLocale))
            {
                model.Description = sliderBannerLocale.Description;
                model.ImageAlternateText = sliderBannerLocale.ImageAlternateText;
                model.LocaleId = sliderBannerLocale.LocaleId;
                model.Title = sliderBannerLocale.Title;
                model.MediaId = sliderBannerLocale.MediaId;
                model.ButtonLabelName = sliderBannerLocale.ButtonLabelName;
                model.ButtonLink = sliderBannerLocale.ButtonLink;

                if (model.MediaId > 0)
                {
                    IZnodeRepository<ZnodeMedia> _znodeMediaRepository = new ZnodeRepository<ZnodeMedia>();
                    model.FileName = _znodeMediaRepository.Table.FirstOrDefault(x => x.MediaId == model.MediaId)?.FileName;
                }
            }
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
        }

        //This method is used to get media path from media Id.
        private string GetMediaPath(BannerModel model)
        {
            if(HelperUtility.IsNotNull(model))
            {
                IMediaManagerServices mediaService = GetService<IMediaManagerServices>();
                MediaManagerModel mediaData = mediaService.GetMediaByID(Convert.ToInt32(model?.MediaId), null);
                model.MediaPath = HelperUtility.IsNotNull(mediaData) ? mediaData.MediaServerThumbnailPath : string.Empty;
            }
            return model.MediaPath;
        }



        [Obsolete("To be discontinued in one of the upcoming versions.")]
        private void UpdatePublishSliderStatus(int cmsSliderId, bool isPublished)
        => _cmsSliderRepository.Update(new ZnodeCMSSlider
        {
            CMSSliderId = cmsSliderId,
            Name = _cmsSliderRepository.Table.Where(x => x.CMSSliderId == cmsSliderId)?.FirstOrDefault().Name,
            IsPublished = isPublished
        });

        private void UpdatePublishSliderStatus(int cmsSliderId, ZnodePublishStatesEnum targetPublishState, bool isPublished)
        => _cmsSliderRepository.Update(new ZnodeCMSSlider
        {
            CMSSliderId = cmsSliderId,
            Name = _cmsSliderRepository.Table.FirstOrDefault(x => x.CMSSliderId == cmsSliderId)?.Name,
            IsPublished = isPublished,
            PublishStateId = isPublished ? (byte)targetPublishState : (byte)ZnodePublishStatesEnum.DRAFT
        });



        #endregion
    }
}
