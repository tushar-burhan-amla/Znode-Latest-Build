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
using Znode.Libraries.Framework.Business;
using Znode.Libraries.Resources;
using static Znode.Libraries.ECommerce.Utilities.ZnodeDependencyResolver;

namespace Znode.Engine.Services
{
    public class HighlightService : BaseService, IHighlightService
    {
        #region Private variables              
        private readonly IZnodeRepository<ZnodeHighlight> _highlightRepository;
        private readonly IZnodeRepository<ZnodeHighlightLocale> _highlightLocaleRepository;
        private readonly IZnodeRepository<ZnodeHighlightType> _highlightTypeRepository;
        private readonly IZnodeRepository<ZnodePimAttribute> _pimAttributeRepository;
        private readonly IZnodeRepository<ZnodePimAttributeDefaultValue> _pimAttributeDefaultValueRepository;
        private readonly IEmailTemplateSharedService _emailTemplateSharedService;
        #endregion

        public HighlightService()
        {
            _highlightRepository = new ZnodeRepository<ZnodeHighlight>();
            _highlightLocaleRepository = new ZnodeRepository<ZnodeHighlightLocale>();
            _highlightTypeRepository = new ZnodeRepository<ZnodeHighlightType>();
            _pimAttributeRepository = new ZnodeRepository<ZnodePimAttribute>();
            _pimAttributeDefaultValueRepository = new ZnodeRepository<ZnodePimAttributeDefaultValue>();
            _emailTemplateSharedService = GetService<IEmailTemplateSharedService>();
        }

        #region Public Methods

        //Get paged Highlight list.
        public virtual HighlightListModel GetHighlightList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Info);

            //Set Locale If Not Present.
            SetHighlightLocaleFilterIfNotPresent(ref filters);

            PageListModel pageListModel = new PageListModel(filters, sorts, page);
            ZnodeLogging.LogMessage("pageListModel to set SP parameters :", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Verbose, pageListModel?.ToDebugString());

            IZnodeViewRepository<HighlightModel> objStoredProc = new ZnodeViewRepository<HighlightModel>();
            int localeId = CategoryService.GetLocaleId(filters);

            int isFromAdmin = 0;
            if (filters.Exists(x => string.Equals(x.FilterName, FilterKeys.IsFromAdmin.ToString(), StringComparison.InvariantCultureIgnoreCase)))
                int.TryParse(filters?.Find(x => string.Equals(x.FilterName, FilterKeys.IsFromAdmin, StringComparison.CurrentCultureIgnoreCase))?.FilterValue, out isFromAdmin);

            filters.RemoveAll(x => x.FilterName == "isassociated");
            filters.RemoveAll(x => x.FilterName.Equals(FilterKeys.LocaleId, StringComparison.InvariantCultureIgnoreCase));
            filters.RemoveAll(x => x.FilterName.Equals("HighlightLocaleId", StringComparison.InvariantCultureIgnoreCase));
            filters.RemoveAll(x => x.FilterName.Equals(FilterKeys.IsFromAdmin.ToString(), StringComparison.InvariantCultureIgnoreCase));

            objStoredProc.SetParameter("@WhereClause", pageListModel.SPWhereClause, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@Rows", pageListModel.PagingLength, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@PageNo", pageListModel.PagingStart, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@Order_By", pageListModel.OrderBy, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@RowCount", pageListModel.TotalRowCount, ParameterDirection.Output, DbType.Int32);
            objStoredProc.SetParameter("@LocaleId", localeId, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@IsFromAdmin", isFromAdmin, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@IsAssociated", false, ParameterDirection.Input, DbType.Boolean);

            IList<HighlightModel> highlightlist = objStoredProc.ExecuteStoredProcedureList("Znode_GetHighlightDetail  @WhereClause,@Rows,@PageNo,@Order_By,@RowCount OUT,@LocaleId,@IsFromAdmin,@IsAssociated", 4, out pageListModel.TotalRowCount);
            ZnodeLogging.LogMessage("highlightlist count :", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Verbose, highlightlist?.Count);

            HighlightListModel highlightlistModel = new HighlightListModel { HighlightList = highlightlist?.ToList() };

            highlightlistModel.BindPageListModel(pageListModel);

            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Info);

            return highlightlistModel;
        }

        /// <summary>
        /// Set locale filter if not present
        /// </summary>
        /// <param name="filters">filters</param>
        private void SetHighlightLocaleFilterIfNotPresent(ref FilterCollection filters)
        {
            filters = HelperUtility.IsNull(filters) ? new FilterCollection() : filters;

            if (!filters.Any(x => x.FilterName.ToLower() == ZnodeHighlightLocaleEnum.HighlightLocaleId.ToString().ToLower()))
                filters.Add(ZnodeHighlightLocaleEnum.HighlightLocaleId.ToString(), FilterOperators.Equals, DefaultGlobalConfigSettingHelper.Locale);
        }

        //Create Highlight.
        public virtual HighlightModel CreateHighlight(HighlightModel highlightModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Info);

            if (HelperUtility.IsNull(highlightModel))
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.HighlightModelNotNull);

            //If highlight is already present.
            if (IsHighlightCodeExist(highlightModel.HighlightCode))
                throw new ZnodeException(ErrorCodes.AlreadyExist, Api_Resources.HighlightAlreadyExists);

            //If LocaleId is less than 1 get default locale.
            if (highlightModel.LocaleId < 1)
                highlightModel.LocaleId = GetDefaultLocaleId();

            //Insert into Highlight.
            HighlightModel model = _highlightRepository.Insert(highlightModel.ToEntity<ZnodeHighlight>()).ToModel<HighlightModel>();

            //Insert into Highlight Locale.
            if (model?.HighlightId > 0)
            {
                highlightModel.HighlightId = model.HighlightId;

                ZnodeLogging.LogMessage(string.Format(Admin_Resources.SuccessHighlightWithIdCreated, model.HighlightId), ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Info);

                if(_highlightLocaleRepository.Insert(highlightModel?.ToEntity<ZnodeHighlightLocale>())?.HighlightLocaleId > 0)
                    ZnodeLogging.LogMessage(Admin_Resources.SuccessHighlightLocaleInserted, ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Info);
                else
                    ZnodeLogging.LogMessage(Admin_Resources.ErrorInsertHighlightlocale, ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Info);

                return highlightModel;
            }
            ZnodeLogging.LogMessage(Admin_Resources.ErrorCreateHighlight, ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Info);
            return highlightModel;
        }

        //Get Highlight by highlightId.
        public virtual HighlightModel GetHighlight(int highlightId, int productId, FilterCollection filters)
        {
            return GetHighlightByCodeOrId(highlightId, filters, productId);
        }

        protected virtual HighlightModel GetHighlightByCodeOrId(int highlightId, FilterCollection filters, int productId = 0, string highlightCode = "")
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Info);

            if (highlightId > 0 || !string.IsNullOrEmpty(highlightCode))
            {
                //Generate where clause.
                FilterCollection filter = new FilterCollection();
                if(highlightId > 0)
                {
                    ZnodeLogging.LogMessage("highlightId, productId:", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Verbose, new { highlightId = highlightId, productId = productId });
                    filter.Add(new FilterTuple(ZnodeHighlightEnum.HighlightId.ToString(), FilterOperators.Is, Convert.ToString(highlightId)));
                }
                else
                {
                    ZnodeLogging.LogMessage("highlightCode:", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Verbose, new { highlightCode = highlightCode });
                    filter.Add(new FilterTuple(ZnodeHighlightEnum.HighlightCode.ToString(), FilterOperators.Is, Convert.ToString(highlightCode)));
                }


                //Get filtervalue of SKU
                string sku = GetFilterValue(filters, FilterKeys.SKU);

                string whereClauseModel = DynamicClauseHelper.GenerateDynamicWhereClauseForSP(filter.ToFilterDataCollection());
                ZnodeLogging.LogMessage("WhereClause generated to set SP parameters :", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Verbose, whereClauseModel);

                int isFromAdmin = 0;
                if (filters.Exists(x => string.Equals(x.FilterName, FilterKeys.IsFromAdmin.ToString(), StringComparison.InvariantCultureIgnoreCase)))
                    int.TryParse(filters?.Find(x => string.Equals(x.FilterName, FilterKeys.IsFromAdmin, StringComparison.CurrentCultureIgnoreCase))?.FilterValue, out isFromAdmin);

                filters.RemoveAll(x => x.FilterName.Equals(FilterKeys.IsFromAdmin.ToString(), StringComparison.InvariantCultureIgnoreCase));

                int localeId = GetLocaleId(filters);
                //SP call
                IZnodeViewRepository<HighlightModel> objStoredProc = new ZnodeViewRepository<HighlightModel>();
                objStoredProc.SetParameter("@WhereClause", whereClauseModel, ParameterDirection.Input, DbType.String);
                objStoredProc.SetParameter("@Rows", 10, ParameterDirection.Input, DbType.Int32);
                objStoredProc.SetParameter("@PageNo", 1, ParameterDirection.Input, DbType.Int32);
                objStoredProc.SetParameter("@Order_By", "", ParameterDirection.Input, DbType.String);
                objStoredProc.SetParameter("@RowCount", 1, ParameterDirection.Output, DbType.Int32);
                objStoredProc.SetParameter(ZnodeLocaleEnum.LocaleId.ToString(), localeId, ParameterDirection.Input, DbType.Int32);
                objStoredProc.SetParameter("@IsFromAdmin", isFromAdmin, ParameterDirection.Input, DbType.Int32);
                HighlightModel highlight = objStoredProc.ExecuteStoredProcedureList("Znode_GetHighlightDetail @WhereClause,@Rows,@PageNo,@Order_By,@RowCount,@LocaleId,@IsFromAdmin")?.FirstOrDefault();

                if (HelperUtility.IsNotNull(highlight) && (productId > 0 || !string.IsNullOrEmpty(highlightCode)))
                {
                    if (!string.IsNullOrEmpty(highlight.Description))
                    {
                        ZnodeLogging.LogMessage("GetHighlightDescription with parameters :", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Verbose, new object[] { productId, localeId, highlight });
                        highlight.Description = GetHighlightDescription(productId, localeId, highlight, sku);
                    }
                    highlight.SEOUrl = GetService<ISEOService>().GetProductSeoData(GetLocaleId(filters), productId, PortalId, ZnodeConstant.Product, sku)?.SEOUrl;
                }

                return highlight;
            }
            return null;
        }

        //Get Highlight by highlightCode.
        public virtual HighlightModel GetHighlightByCode(string highlightCode, FilterCollection filters)
        {

            return GetHighlightByCodeOrId(0, filters, 0, highlightCode);
        }

        //Update Highlight.
        public virtual bool UpdateHighlight(HighlightModel highlightModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Info);

            if (HelperUtility.IsNull(highlightModel))
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.HighlightModelNotNull);

            if (highlightModel.HighlightId < 1)
                throw new ZnodeException(ErrorCodes.IdLessThanOne, Admin_Resources.IdCanNotBeLessThanOne);

            ZnodeLogging.LogMessage("HighlightModel with highlightId :", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Verbose, highlightModel?.HighlightId);

            //Update Highlight
            bool ishighlightUpdated = _highlightRepository.Update(highlightModel.ToEntity<ZnodeHighlight>());

            //Save the data into highlight locale.
            if (ishighlightUpdated)
                SaveInHighlightLocale(highlightModel);

            ZnodeLogging.LogMessage(ishighlightUpdated ? Admin_Resources.SuccessHighlightUpdated : Admin_Resources.ErrorHighlightUpdate, ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Info);
            return ishighlightUpdated;
        }

        //Delete Highlight.
        public virtual bool DeleteHighlight(ParameterModel highlightIds)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("highlightIds to be deleted : ", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Verbose, highlightIds?.Ids);

            if (HelperUtility.IsNull(highlightIds) || string.IsNullOrEmpty(highlightIds.Ids))
                throw new ZnodeException(ErrorCodes.IdLessThanOne, Admin_Resources.HighlightIdCanNotBeLessThanOne);

            int status;
            IZnodeViewRepository<HighlightModel> objStoredProc = new ZnodeViewRepository<HighlightModel>();
            objStoredProc.SetParameter("HighlightId", highlightIds.Ids, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("Status", null, ParameterDirection.Output, DbType.Int32);
            objStoredProc.ExecuteStoredProcedureList("Znode_DeleteHighlights @HighlightId,  @Status OUT", 1, out status);

            if (Equals(status, 1))
            {
                ZnodeLogging.LogMessage(Admin_Resources.SuccessHighlightDeleted, ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Info);
                return true;
            }
            ZnodeLogging.LogMessage(Admin_Resources.ErrorHighlightDelete, ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Info);
            return false;
        }

        //Get available highlight codes.
        public virtual HighlightListModel GetAvailableHighlightCodes(string attributeCode)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters attributeCode : ", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Verbose, attributeCode);

            HighlightListModel highlightListModel = new HighlightListModel();

            highlightListModel.HighlightCodes = GetHighlightCodes(attributeCode)?.HighlightCodes;
            List<string> highlightCodes = GetHighlightList(null, null, null, null)?.HighlightList?.Select(x => x.HighlightCode)?.ToList();
            ZnodeLogging.LogMessage("highlightCodes count returned from GetHighlightList method : ", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Verbose, highlightCodes?.Count);

            if (highlightCodes?.Count() > 0)
                highlightListModel.HighlightCodes = highlightListModel?.HighlightCodes?.Where(code => !highlightCodes.Contains(code.AttributeDefaultValueCode))?.ToList();

            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Info);

            return highlightListModel;
        }

        //Get highlight codes.
        public virtual HighlightListModel GetHighlightCodes(string attributeCode)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters attributeCode : ", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Verbose, attributeCode);

            //Filter for attribute code.
            FilterCollection filter = new FilterCollection();
            filter.Add(new FilterTuple(ZnodePimAttributeEnum.AttributeCode.ToString(), FilterOperators.Is, attributeCode));
            EntityWhereClauseModel whereClauseModel = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filter.ToFilterDataCollection());

            //Filter for PIMattrbuteId.
            filter = new FilterCollection();
            filter.Add(new FilterTuple(ZnodePimAttributeDefaultValueEnum.PimAttributeId.ToString(), FilterOperators.Equals, _pimAttributeRepository.GetEntity(whereClauseModel.WhereClause, whereClauseModel.FilterValues)?.PimAttributeId.ToString()));
            whereClauseModel = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filter.ToFilterDataCollection());

            _pimAttributeDefaultValueRepository.EnableDisableLazyLoading = true;
            PIMAttributeDefaultValueListModel pimAttributeDefaultValueListModel = PIMAttributesMap.ToDefaultValueListModel(_pimAttributeDefaultValueRepository.GetEntityList(whereClauseModel.WhereClause).ToList());
            if (pimAttributeDefaultValueListModel.DefaultValues?.Count > 0)
                return new HighlightListModel() { HighlightCodes = pimAttributeDefaultValueListModel.DefaultValues };
            else
                return null;
        }
        #region Highlight Type

        //Get Highlight type list.
        public virtual HighlightTypeListModel GetHighlightTypeList(FilterCollection filters, NameValueCollection sorts)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Info);

            PageListModel pageListModel = new PageListModel(filters, sorts, null);

            //maps the entity list to model.
            IList<ZnodeHighlightType> highlightTypeList = _highlightTypeRepository.GetEntityList(pageListModel.EntityWhereClause.WhereClause, pageListModel.OrderBy, null, pageListModel.EntityWhereClause.FilterValues);
            ZnodeLogging.LogMessage("highlightTypeList count : ", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Verbose, highlightTypeList?.Count);

            HighlightTypeListModel listModel = new HighlightTypeListModel();
            listModel.HighlightTypes = highlightTypeList?.Count > 0 ? highlightTypeList.ToModel<HighlightTypeModel>().ToList() : new List<HighlightTypeModel>();

            ZnodeLogging.LogMessage("GetHighlightTypeList method execution done.", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Info);
            return GetAttributesTokens(listModel);
        }



        #endregion

        #region Highlight Product

        //Associate highlight products.
        public virtual bool AssociateAndUnAssociateProduct(HighlightProductModel highlightProductModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Info);

            IZnodeViewRepository<View_ReturnBoolean> objStoredProc = new ZnodeViewRepository<View_ReturnBoolean>();
            objStoredProc.SetParameter("ProductId", highlightProductModel.ProductIds, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("PimAttributeCode", highlightProductModel.AttributeCode, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("LocaleId", Convert.ToInt32(DefaultGlobalConfigSettingHelper.Locale), ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("AttributeValue", highlightProductModel.AttributeValue, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("UserId", GetLoginUserId(), ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("Status", null, ParameterDirection.Output, DbType.Int32);
            objStoredProc.SetParameter("IsUnAssociated", highlightProductModel.IsUnAssociated, ParameterDirection.Input, DbType.Int32);

            int status = 0;
            IList<View_ReturnBoolean> deleteResult = objStoredProc.ExecuteStoredProcedureList("Znode_UpdateAttributeValue @ProductId,@PimAttributeCode,@LocaleId,@AttributeValue,@UserId,@Status OUT,@IsUnAssociated", 5, out status);
            ZnodeLogging.LogMessage("deleteResult count: ", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Verbose, deleteResult?.Count);

            if (deleteResult.FirstOrDefault().Status.Value)
            {
                ZnodeLogging.LogMessage(Admin_Resources.SuccessHighlightUpdated, ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Info);
                return true;
            }
            else
            {
                ZnodeLogging.LogMessage(Admin_Resources.ErrorHighlightUpdate, ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Info);
                return false;
            }
        }

        #endregion

        #endregion

        #region Private Methods
        //Sets the properties of Highlight model.
        private void SetHighlightModel(ZnodeHighlight highlightEntity, HighlightModel highlightModel, int localeId)
        {
            ZnodeLogging.LogMessage("Execution started: ", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("method call with parameters : ", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Verbose, new object[] { highlightEntity, highlightModel, GetDefaultLocaleId() });

            //Get the highlight locale entity.
            ZnodeHighlightLocale highlightLocale = highlightEntity.ZnodeHighlightLocales.Where(x => x.HighlightId == highlightEntity.HighlightId && x.LocaleId == localeId)?.FirstOrDefault();

            //Set the properies.
            if (HelperUtility.IsNotNull(highlightLocale))
            {
                highlightModel.LocaleId = highlightLocale.LocaleId;
                highlightModel.ShortDescription = highlightLocale.ShortDescription;
                highlightModel.Description = highlightLocale.Description;
                highlightModel.HighlightName = highlightLocale.Name;
                highlightModel.ImageAltTag = highlightLocale.ImageAltTag;
                highlightModel.HighlightLocaleId = highlightLocale.HighlightLocaleId;
            }
            else
            {
                //Sets the properties of highlightmodel for default locale Id.  
                SetHighlightModel(highlightEntity, highlightModel, GetDefaultLocaleId());

            }
            ZnodeLogging.LogMessage("Execution done: ", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Info);

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

        //Save the data into highlight locale.
        private void SaveInHighlightLocale(HighlightModel highlightModel)
        {
            //Get the Highlight locale.
            int highlightLocaleId = _highlightLocaleRepository.Table.Where(x => x.HighlightId == highlightModel.HighlightId && x.LocaleId == highlightModel.LocaleId).Select(x => x.HighlightLocaleId).FirstOrDefault();
            ZnodeLogging.LogMessage("highlightLocaleId : ", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Verbose, new object[] { highlightLocaleId });

            //Locale present for that highlight then update the entry else create the entry.
            if (highlightLocaleId > 0)
            {
                highlightModel.HighlightLocaleId = highlightLocaleId;
                ZnodeLogging.LogMessage(_highlightLocaleRepository.Update(highlightModel?.ToEntity<ZnodeHighlightLocale>())
                    ? Admin_Resources.SuccessHighlightLocaleUpdated : Admin_Resources.ErrorHighlightLocaleUpdate, ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Info);
            }
            else
                ZnodeLogging.LogMessage(_highlightLocaleRepository.Insert(highlightModel?.ToEntity<ZnodeHighlightLocale>())?.HighlightLocaleId > 0
                    ? Admin_Resources.SuccessHighlightLocaleInserted : Admin_Resources.ErrorInsertHighlightlocale, ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Info);
        }

        //Get list of Attributes as tokens excluding multi select type.
        protected virtual HighlightTypeListModel GetAttributesTokens(HighlightTypeListModel listModel)
        {
            listModel.TemplateTokens = "";
            List<string> attributesTypeName = GetAttributesType();
            List<string> attributes = _pimAttributeRepository.Table.Where(x => !x.IsCategory &&
                                                                             attributesTypeName.Contains(x.ZnodeAttributeType.AttributeTypeName)).
                                                                          Select(x => x.AttributeCode).ToList();
            for(int index = 0; index < attributes.Count; index++)
            {
                if(index != ZnodeConstant.TemplateLimitIndex)
                    listModel.TemplateTokens += string.Concat("#", attributes[index], "#,\r\n");
                else
                    listModel.TemplateTokens += string.Concat("#", attributes[index], "#,\r\n~");
            }

            return listModel;
        }

        private List<string> GetAttributesType() => new List<string>
            {
                ZnodeConstant.DateType,
                ZnodeConstant.TypeTextArea,
                ZnodeConstant.TypeText,
                ZnodeConstant.NumberType,
                ZnodeConstant.TypeLabel
            };

        //Get Highlight Description by replacing attributes tokens.
        protected virtual string GetHighlightDescription(int productId, int localeId, HighlightModel highlight, string sku = "")
        {
            PublishedProductEntityModel productDetails;
            Dictionary<string, string> setDictionary = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(sku))
                productDetails = GetService<IPublishedProductDataService>().GetPublishProductBySKU(sku, GetPortalCatalogId(PortalId).GetValueOrDefault(), localeId, GetCatalogVersionId(0, localeId, PortalId))?.ToModel<PublishedProductEntityModel>();
            else
                productDetails = GetService<IPublishedProductDataService>().GetPublishProduct(productId, PortalId, localeId, GetCatalogVersionId(0, localeId, PortalId))?.ToModel<PublishedProductEntityModel>();
            if (HelperUtility.IsNotNull(productDetails))
            {
                //To get distinct records for attributes
                List<PublishedAttributeEntityModel> attributes = productDetails.Attributes.DistinctBy(x => x.AttributeCode).ToList();
                foreach (PublishedAttributeEntityModel attribute in attributes)
                {
                    // Applied the below check, if the attribute code does not exist in the below dictionary,it will add the attribute code and values.
                    if (!setDictionary.ContainsKey($"#{attribute.AttributeCode}#"))
                        setDictionary.Add("#" + attribute.AttributeCode + "#", attribute.AttributeValues);
                }
            }

            return GetService<IEmailTemplateSharedService>().ReplaceTemplateTokens(highlight.Description, setDictionary);         

        }

        //Gets the specified key's value of the filter.
        private static string GetFilterValue(FilterCollection filters, string key)
        {
            string value = filters.Find(x => string.Equals(x.FilterName, key, StringComparison.CurrentCultureIgnoreCase))?.FilterValue;
            filters.RemoveAll(x => string.Equals(x.FilterName, key, StringComparison.CurrentCultureIgnoreCase));
            return value;
        }
        #endregion

        #region Protected Methods
        //Check the uniqueness of highlight.
        protected virtual bool IsHighlightCodeExist(string highlightCode)
        {
           return _highlightRepository.Table.Any(x => x.HighlightCode.Equals(highlightCode, StringComparison.InvariantCultureIgnoreCase));                
        }
        #endregion
    }
}
