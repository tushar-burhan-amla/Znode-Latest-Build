using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using Znode.Engine.Admin.Extensions;
using Znode.Engine.Admin.Helpers;
using Znode.Engine.Admin.Maps;
using Znode.Engine.Admin.Models;
using Znode.Engine.Admin.ViewModels;
using Znode.Engine.Api.Client;
using Znode.Engine.Api.Client.Expands;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;
using Znode.Engine.Exceptions;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;
using Znode.Libraries.Resources;
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;

namespace Znode.Engine.Admin.Agents
{
    //Import Agent to perform the import activities
    public class ImportAgent : BaseAgent, IImportAgent
    {
        #region Private Variables
        private readonly IStoreAgent _storeAgent;
        private readonly ILocaleAgent _localeAgent;
        private readonly IImportClient _importClient;
        private readonly IPriceClient _priceClient;
        private readonly ICountryAgent _countryAgent;
        private readonly IPortalClient _portalClient;
        private readonly ICatalogClient _catalogClient;
        #endregion

        #region Constructor
        public ImportAgent(IImportClient importClient, IPriceClient priceClient, ICountryClient countryClient,IPortalClient portalClient, ICatalogClient catalogClient)
        {
            _storeAgent = new StoreAgent(GetClient<PortalClient>(), GetClient<EcommerceCatalogClient>(), GetClient<ThemeClient>(), GetClient<DomainClient>(), GetClient<PriceClient>(), GetClient<OrderStateClient>(),
                GetClient<ProductReviewStateClient>(), GetClient<PortalProfileClient>(), GetClient<WarehouseClient>(), GetClient<CSSClient>(), GetClient<ManageMessageClient>(), GetClient<ContentPageClient>(), GetClient<TaxClassClient>(),
                GetClient<PaymentClient>(), GetClient<ShippingClient>(), GetClient<PortalCountryClient>(),GetClient<TagManagerClient>(), GetClient<GeneralSettingClient>());
            _localeAgent = new LocaleAgent(GetClient<LocaleClient>());
            _importClient = GetClient<IImportClient>(importClient);
            _priceClient = GetClient<IPriceClient>(priceClient);
            _countryAgent = new CountryAgent(GetClient<CountryClient>());
            _portalClient = GetClient<IPortalClient>(portalClient);
            _catalogClient = GetClient<ICatalogClient>(catalogClient);
        }
        #endregion

        #region public Methods
        //Get the required components to be shows on UI.
        public ImportViewModel BindViewModel()
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Import.ToString(), TraceLevel.Info);
            return new ImportViewModel()
            {
                ImportTypeList = GetImportTypeList(),
                TemplateTypeList = new List<SelectListItem>(),
                Locale = _localeAgent.GetLocalesList(),
                FamilyList = new List<SelectListItem>(),
                PricingList = new List<SelectListItem>(),
                CountryList = new List<SelectListItem>(),
                PortalList=new List<SelectListItem>(),
                CatalogList = new List<SelectListItem>(),
                PromotionTypeList = new List<SelectListItem>()
            };
        }

        //This method will upload the file and process the data.
        public virtual bool ImportData(ImportViewModel importviewModel)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Import.ToString(), TraceLevel.Info);
            string fileName = importviewModel?.FileName;
            ZnodeLogging.LogMessage("fileName property of importviewModel: ", ZnodeLogging.Components.Import.ToString(), TraceLevel.Verbose, new { fileName = fileName });
            try
            {
                importviewModel.FileName = fileName;
                bool isSuccess = _importClient.ImportData(ImportViewModelMap.ToModel(importviewModel));
                RemoveTemporaryFiles(fileName);
                ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Import.ToString(), TraceLevel.Info);
                return isSuccess;
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Import.ToString(), TraceLevel.Warning);
                //Remove the file if any error comes in.
                RemoveTemporaryFiles(fileName);
                return false;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Import.ToString(), TraceLevel.Error);
                return false;
            }
        }

        //This method will fetch the list of import types supported.
        public virtual List<SelectListItem> GetImportTypeList(bool isDynamicReport = false)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Import.ToString(), TraceLevel.Info);

            FilterCollection filters = new FilterCollection();
            if (isDynamicReport)
                filters.Add(new FilterTuple(ZnodeImportHeadEnum.IsUsedInDynamicReport.ToString(), FilterOperators.Equals, AdminConstants.True));
            else
                filters.Add(new FilterTuple(ZnodeImportHeadEnum.IsUsedInImport.ToString(), FilterOperators.Equals, AdminConstants.True));

            //Filter added for to get import head whose IsCsvUploader value is false of null.
            filters.Add(new FilterTuple(ZnodeImportHeadEnum.IsCsvUploader.ToString(), FilterOperators.NotEquals, AdminConstants.True));
            ZnodeLogging.LogMessage("Input parameters filters of method GetImportTypeList.", ZnodeLogging.Components.Import.ToString(), TraceLevel.Verbose, new { filters = filters });

            ImportTypeListModel importList = _importClient.GetImportTypeList(null, filters, null, null, null)?.ImportTypeList;
            List<SelectListItem> importTypeList = new List<SelectListItem>();
            importTypeList.AddRange(importList.ImportTypeList.Select(import => new SelectListItem() { Text = import.Name, Value = import.ImportHeadId.ToString() }));
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Import.ToString(), TraceLevel.Info);
            return importTypeList;
        }

        //This method will fetch the list of import Template types supported.
        public virtual List<SelectListItem> GetImportTemplateList(int importHeadId, int familyId, int promotionTypeId = 0)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Import.ToString(), TraceLevel.Info);

            ImportTemplateListModel importTemplateList = _importClient.GetAllTemplates(importHeadId, familyId, promotionTypeId).TemplateList;
            List<SelectListItem> templateList = new List<SelectListItem>();
            importTemplateList.TemplateList.ToList().ForEach(item => { templateList.Add(new SelectListItem() { Text = item.TemplateName, Value = Convert.ToString(item.ImportTemplateId) }); });

            if (templateList.Count > 0)
                templateList.Insert(0, new SelectListItem { Text = Admin_Resources.LabelPleaseSelect, Value = "0" });
            ZnodeLogging.LogMessage("templateList count.", ZnodeLogging.Components.Import.ToString(), TraceLevel.Verbose, templateList?.Count);
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Import.ToString(), TraceLevel.Info);
            return templateList.Count > 0 ? templateList : new List<SelectListItem>();
        }

        //This method will get the mapped template list
        public virtual List<List<SelectListItem>> GetImportTemplateMappingList(int templateId, int importHeadId, int familyId, int promotionTypeId = 0)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Import.ToString(), TraceLevel.Info);

            List<List<SelectListItem>> mappings = new List<List<SelectListItem>>();
            ImportTemplateMappingListModel importMappingTemplateList = _importClient.GetTemplateData(templateId, importHeadId, familyId, promotionTypeId).TemplateMappingList;
            List<SelectListItem> targetColumnList = new List<SelectListItem>();
            List<SelectListItem> sourceColumnList = new List<SelectListItem>();

            importMappingTemplateList.Mappings?.ToList().ForEach(x => { targetColumnList.Add(new SelectListItem() { Text = x.TargetColumnName, Value = Convert.ToString(x.ImportTemplateMappingId) }); });
            importMappingTemplateList.Mappings?.ToList().ForEach(x => { sourceColumnList.Add(new SelectListItem() { Text = x.SourceColumnName, Value = Convert.ToString(x.ImportTemplateMappingId) }); });

            mappings.Add(targetColumnList);
            mappings.Add(sourceColumnList);
            ZnodeLogging.LogMessage("mappings list count.", ZnodeLogging.Components.Import.ToString(), TraceLevel.Verbose, mappings?.Count);
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Import.ToString(), TraceLevel.Info);
            return mappings;
        }

        //This method is to get the CSV file headers
        public virtual string GetCsvHeaders(HttpPostedFileBase file, out string fileName)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Import.ToString(), TraceLevel.Info);
            string headers = string.Empty;
            fileName = UploadImportFile(file);
            ZnodeLogging.LogMessage("fileName returned from method UploadImportFile.", ZnodeLogging.Components.Import.ToString(), TraceLevel.Verbose, new { fileName = fileName });
            if (HelperUtility.IsNotNull(file))
            {
                using (var reader = new StreamReader(fileName))
                {
                    IEnumerable<string> firstLine = File.ReadLines(fileName).Take(1);
                    //get the headers from CSV file
                    if (HelperUtility.IsNotNull(firstLine))
                        headers = firstLine.First().ToString();
                }
            }
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Import.ToString(), TraceLevel.Info);
            return headers;
        }

        // Download the template
        public virtual ImportViewModel DownloadTemplate(int importHeadId, string importName, int downloadImportFamilyId, int downloadImportPromotionTypeId, HttpResponseBase response)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Import.ToString(), TraceLevel.Info);
            DownloadHelper helper = new DownloadHelper();
            helper.ExportDownload(_importClient.DownloadTemplate(importHeadId, downloadImportFamilyId, downloadImportPromotionTypeId)?.data, "2", response, ",", $"{importName}{AdminConstants.CSV}");
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Import.ToString(), TraceLevel.Info);
            return BindViewModel();
        }

        //Get import log list.
        public virtual ImportProcessLogsListViewModel GetImportLogs(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int pageIndex, int pageSize)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Import.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters expands, filters and sorts", ZnodeLogging.Components.Import.ToString(), TraceLevel.Verbose, new { expands = expands, filters = filters, sorts = sorts });
            ImportLogsListModel importLogsList = _importClient.GetImportLogs(expands, filters, sorts, pageIndex, pageSize);

            ImportProcessLogsListViewModel listViewModel = new ImportProcessLogsListViewModel { ProcessLogs = importLogsList?.ImportLogs?.ToViewModel<ImportProcessLogsViewModel>().ToList() };
            SetListPagingData(listViewModel, importLogsList);

            //Set tool option menus for import process grid.
            SetImportListToolMenu(listViewModel);
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Import.ToString(), TraceLevel.Info);
            return importLogsList?.ImportLogs?.Count > 0 ? listViewModel : new ImportProcessLogsListViewModel() { ProcessLogs = new List<ImportProcessLogsViewModel>() };
        }


        //Get the import log status
        public virtual ImportProcessLogsListViewModel GetImportLogStatus(int importProcessLogId, ExpandCollection expands, FilterCollection filters, SortCollection sorts, int pageIndex, int pageSize)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Import.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters importProcessLogId, expands, filters and sorts", ZnodeLogging.Components.Import.ToString(), TraceLevel.Verbose, new { importProcessLogId = importProcessLogId, expands = expands, filters = filters, sorts = sorts });

            if (IsNull(sorts))
            {
                sorts = new SortCollection();
                sorts.Add(SortKeys.ModifiedDate, DynamicGridConstants.DESCKey);
            }

            ImportLogsListModel importLogsList = _importClient.GetImportLogStatus(importProcessLogId, expands, filters, sorts, pageIndex, pageSize);

            ImportProcessLogsListViewModel listViewModel = new ImportProcessLogsListViewModel { ProcessLogs = importLogsList?.ImportLogs?.ToViewModel<ImportProcessLogsViewModel>().ToList() };
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Import.ToString(), TraceLevel.Info);
            return importLogsList?.ImportLogs?.Count > 0 ? listViewModel : new ImportProcessLogsListViewModel() { ProcessLogs = new List<ImportProcessLogsViewModel>() };
        }

        //Get the import log details
        public virtual ImportLogsListViewModel GetImportLogDetails(int importProcessLogId, ExpandCollection expands, FilterCollection filters, SortCollection sorts, int pageIndex, int pageSize)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Import.ToString(), TraceLevel.Info);
            if (IsNull(sorts))
            {
                sorts = new SortCollection();
                sorts.Add(SortKeys.ModifiedDate, DynamicGridConstants.DESCKey);
            }
            ZnodeLogging.LogMessage("Input parameters importProcessLogId, expands, filters and sorts", ZnodeLogging.Components.Import.ToString(), TraceLevel.Verbose, new { importProcessLogId = importProcessLogId, expands = expands, filters = filters, sorts = sorts });
            ImportLogDetailsListModel importLogsList = _importClient.GetImportLogDetails(importProcessLogId, expands, filters, sorts, pageIndex, pageSize);

            ImportLogsListViewModel listViewModel = new ImportLogsListViewModel { LogsList = importLogsList?.ImportLogDetails?.ToViewModel<ImportLogsViewModel>().ToList(), ImportLogs = importLogsList?.ImportLogs?.ToViewModel<ImportProcessLogsViewModel>() };
            SetListPagingData(listViewModel, importLogsList);
            SetImportListToolMenu(listViewModel);
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Import.ToString(), TraceLevel.Info);
            return importLogsList?.ImportLogDetails?.Count > 0 ? listViewModel : new ImportLogsListViewModel() { LogsList = new List<ImportLogsViewModel>(), ImportLogs = listViewModel?.ImportLogs };
        }

        //Gets the import log detail list
        public virtual ImportLogsDownloadListViewModel DownloadImportLogDetails(int importProcessLogId, ExpandCollection expands, FilterCollection filters, SortCollection sorts, int pageIndex, int pageSize)
        {
            filters = SessionHelper.GetDataFromSession<FilterCollection>(DynamicGridConstants.FilterCollectionsSessionKey);
            sorts = SessionHelper.GetDataFromSession<SortCollection>(DynamicGridConstants.SortCollectionSessionKey);
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Import.ToString(), TraceLevel.Info);
            if (IsNull(sorts))
            {
                sorts = new SortCollection();
                sorts.Add(SortKeys.ModifiedDate, DynamicGridConstants.DESCKey);
            }
            ZnodeLogging.LogMessage("Input parameters importProcessLogId, expands, filters and sorts", ZnodeLogging.Components.Import.ToString(), TraceLevel.Verbose, new { importProcessLogId = importProcessLogId, expands = expands, filters = filters, sorts = sorts });
            ImportLogDetailsListModel importLogsList = _importClient.GetImportLogDetails(importProcessLogId, expands, filters, sorts, pageIndex, pageSize);

            List<ImportLogDetailsDownloadViewModel> ListImportLogsDownloadViewModel = importLogsList?.ImportLogDetails?.Count > 0 ? importLogsList.ImportLogDetails.ToViewModel<ImportLogDetailsDownloadViewModel>().ToList() : new List<ImportLogDetailsDownloadViewModel>();
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Import.ToString(), TraceLevel.Info);
            ImportLogsDownloadListViewModel importLogsDownloadListViewModel = new ImportLogsDownloadListViewModel { LogsList = ListImportLogsDownloadViewModel };
            return importLogsDownloadListViewModel;
        }

        // Delete the Import logs
        public virtual bool DeleteLog(string importProcessLogIds) => _importClient.DeleteLogs(new ParameterModel { Ids = importProcessLogIds });

        //Get all families for product import
        public virtual List<SelectListItem> GetAllFamilies(bool isCategory)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Import.ToString(), TraceLevel.Info);
            ImportProductFamilyListModel productFamilies = _importClient.GetAllFamilies(isCategory);
            List<SelectListItem> familyList = new List<SelectListItem>();
            productFamilies.FamilyList.ToList().ForEach(item => { familyList.Add(new SelectListItem() { Text = item.FamilyCode, Value = Convert.ToString(item.PimAttributeFamilyId) }); });
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Import.ToString(), TraceLevel.Info);
            return familyList.Count > 0 ? familyList : new List<SelectListItem>();
        }

        //Get all families for product import
        public virtual List<SelectListItem> GetPricingList()
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Import.ToString(), TraceLevel.Info);
            //Get the sort collection for price list id desc.
            SortCollection sorts = new SortCollection();
            sorts = HelperMethods.SortDesc(ZnodePriceListEnum.PriceListId.ToString(), sorts);

            ExpandCollection expands = new ExpandCollection();
            expands.Add(ZnodePriceListEnum.ZnodePriceListProfiles.ToString());
            ZnodeLogging.LogMessage("Input parameters expands and sorts of method GetPriceList:", ZnodeLogging.Components.Import.ToString(), TraceLevel.Verbose, new {  expands = expands, sorts = sorts });
            PriceListModel priceList = _priceClient.GetPriceList(expands, null, sorts, null, null);

            List<SelectListItem> pricingSelectList = new List<SelectListItem>();

            if (priceList?.PriceList?.Count > 0)
                priceList.PriceList.ToList().ForEach(item => { pricingSelectList.Add(new SelectListItem() { Text = item.ListName, Value = Convert.ToString(item.PriceListId) }); });
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Import.ToString(), TraceLevel.Info);
            return pricingSelectList.Count > 0 ? pricingSelectList : new List<SelectListItem>();
        }

        //get all Portal(Store) names for import.
        public virtual List<SelectListItem> GetPortalList()
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Import.ToString(), TraceLevel.Info);
            //Get the sort collection for portal id desc.
            SortCollection sorts = new SortCollection();
            sorts = HelperMethods.SortDesc(ZnodePortalEnum.StoreName.ToString(), sorts);

            ExpandCollection expands = new ExpandCollection();
            expands.Add(ZnodePortalEnum.StoreName.ToString());

            ZnodeLogging.LogMessage("Input parameters expands and sorts of method GetPortalList:", ZnodeLogging.Components.Import.ToString(), TraceLevel.Verbose, new { expands = expands, sorts = sorts });

            PortalListModel portalList = _portalClient.GetPortalList(expands, null, sorts, null, null);

            List<SelectListItem> portalSelectList = new List<SelectListItem>();
           
            if (portalList?.PortalList?.Count > 0)
                portalList.PortalList.ToList().ForEach(item => { portalSelectList.Add(new SelectListItem() { Text = item.StoreName, Value = Convert.ToString(item.PortalId) }); });
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Import.ToString(), TraceLevel.Info);
            return portalSelectList.Count > 0 ? portalSelectList : new List<SelectListItem>();
        }

        //Get all countries for zipcode import.
        public virtual List<SelectListItem> GetCountryList()
        => _countryAgent.GetActiveCountryList();

        // Update TemplateMappings
        public virtual bool UpdateMappings(ImportViewModel model)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Import.ToString(), TraceLevel.Info);
            FilterCollection filters = new FilterCollection();
            filters.Add(new FilterTuple(ZnodeImportTemplateMappingEnum.ImportTemplateId.ToString(), FilterOperators.Equals, model.TemplateId.ToString()));

            bool response = _importClient.UpdateMappings(ImportViewModelMap.ToModel(model), filters);
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Import.ToString(), TraceLevel.Info);
            return response;
        }

        public virtual bool CheckImportProcess() => _importClient.CheckImportProcess();

        //Get all catalog list for import
        public virtual List<SelectListItem> GetCatalogList()
        {
            ZnodeLogging.LogMessage("Agent GetCatalogList method execution started.", ZnodeLogging.Components.Import.ToString(), TraceLevel.Info);

            CatalogListModel catalogList = _catalogClient.GetCatalogList(null, null, null);

            List<SelectListItem> catalogSelectList = new List<SelectListItem>();

            if (catalogList?.Catalogs?.Count > 0)
                catalogList.Catalogs.ToList().ForEach(item => { catalogSelectList.Add(new SelectListItem() { Text = item.CatalogName, Value = Convert.ToString(item.PimCatalogId) }); });
            ZnodeLogging.LogMessage("Agent GetCatalogList method execution done.", ZnodeLogging.Components.Import.ToString(), TraceLevel.Info);
            return catalogSelectList.Count > 0 ? catalogSelectList : new List<SelectListItem>();
        }

        //Get all promotion type list for import
        public virtual List<SelectListItem> GetPromotionTypeList()
        {
            SortCollection sorts = new SortCollection();
            sorts.Add(SortKeys.Name, DynamicGridConstants.ASCKey);

            FilterCollection filters = new FilterCollection();
            filters.Add(new FilterTuple(ZnodePromotionTypeEnum.IsActive.ToString(), FilterOperators.Equals, "true"));
            ZnodeLogging.LogMessage("Agent GetPromotionTypeList method execution started.", ZnodeLogging.Components.Import.ToString(), TraceLevel.Info);
            
            PromotionTypeListModel promotionTypeList =  GetClient<PromotionTypeClient>().GetPromotionTypeList(filters, sorts, null, null);
            List<SelectListItem> selectedPromotionTypeList = new List<SelectListItem>();

            if (promotionTypeList?.PromotionTypes?.Count > 0)
                promotionTypeList.PromotionTypes.ToList().ForEach(item => { selectedPromotionTypeList.Add(new SelectListItem() { Text = item.Name, Value = item.PromotionTypeId.ToString() }); });

            return selectedPromotionTypeList;
        }

        public virtual bool CheckExportProcess() => _importClient.CheckExportProcess();
       
        //Set tool option menus for import log details grid.
        private static void SetImportListToolMenu(ImportLogsListViewModel importLogs)
        {
            if (IsNotNull(importLogs))
            {
                importLogs.GridModel = new GridModel();
                importLogs.GridModel.FilterColumn = new FilterColumnListModel();
                importLogs.GridModel.FilterColumn.ToolMenuList = new List<ToolMenuModel>();
                var temp = Admin_Resources.CSV;
                importLogs.GridModel.FilterColumn.ToolMenuList.Add(new ToolMenuModel
                {
                    DisplayText = Admin_Resources.CSV,
                    ControllerName = "Export",
                    ActionName = "Export",
                    Value = "2",
                    Type = "ImportErrorLogDetails",
                    JSFunctionName = "Import.prototype.Export(event)"
                });
                importLogs.GridModel.FilterColumn.ToolMenuList.Add(new ToolMenuModel
                {
                    DisplayText = Admin_Resources.Excel,
                    ControllerName = "Export",
                    ActionName = "Export",
                    Value = "1",
                    Type = "ImportErrorLogDetails",
                    JSFunctionName = "Import.prototype.Export(event)"
                });
                importLogs.GridModel.FilterColumn.ToolMenuList.Add(new ToolMenuModel 
                {
                    DisplayText = Admin_Resources.PDF,
                    ControllerName = "Export",
                    ActionName = "Export",
                    Value = "3",
                    Type = "ImportErrorLogDetails",
                   //JSFunctionName = "Import.prototype.DownloadImportLogDetails(event)",
                   JSFunctionName = "Import.prototype.Export(event)"
                });

            }
        }

       //Get custom import template list. It will not return the system defined import template.
        public virtual ImportManageTemplateMappingListViewModel GetCustomImportTemplateList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int pageIndex, int pageSize)
        {
            ZnodeLogging.LogMessage("Agent GetImportTemplateByTemplateType method execution started.", ZnodeLogging.Components.Import.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters expands, filters and sorts", ZnodeLogging.Components.Import.ToString(), TraceLevel.Verbose, new { expands = expands, filters = filters, sorts = sorts });
            ImportManageTemplateListModel importManageTemplateList = _importClient.GetCustomImportTemplateList(expands, filters, sorts, pageIndex, pageSize);

            ImportManageTemplateMappingListViewModel listViewModel = new ImportManageTemplateMappingListViewModel { ImportTemplate= importManageTemplateList?.ImportManageTemplates?.ToViewModel<ImportManageTemplateMappingViewModel>().ToList() };
            SetListPagingData(listViewModel, importManageTemplateList);

            //Set tool option menus for import process grid.
            SetImportListToolMenu(listViewModel);

            ZnodeLogging.LogMessage("Agent GetImportTemplateByTemplateType method execution done.", ZnodeLogging.Components.Import.ToString(), TraceLevel.Info);
            return importManageTemplateList?.ImportManageTemplates?.Count > 0 ? listViewModel : new ImportManageTemplateMappingListViewModel() { ImportTemplate = new List<ImportManageTemplateMappingViewModel>() };
        }

        //Deletes the custom import templates. It will not delete the system defined import templates.
        public virtual bool DeleteImportTemplate(string importTemplateIds) => _importClient.DeleteImportTemplate(new ParameterModel { Ids = importTemplateIds });

        // get import details for csv,excel and pdf download.
        public virtual ExportResponseMessageModel GetImportErrorLogList(string fileType, int importProcessLogId, ExpandCollection expands, FilterCollection filters, SortCollection sorts, int pageIndex, int pageSize)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            if (HelperUtility.IsNull(filters))
                filters = new FilterCollection();

            ZnodeLogging.LogMessage("Input parameters filters and sorts of method GetExportFormSubmissionList: ", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, new { filters = filters, sorts = sorts });

            ExportModel exportFormSubmissionMessage = _importClient.GetImportLogDetailsList(fileType, importProcessLogId, expands, filters, sorts, pageIndex, pageSize);
            ExportResponseMessageModel importLogResponseMessage = new ExportResponseMessageModel()
            {
                Message = exportFormSubmissionMessage.Message,
                HasError = exportFormSubmissionMessage.HasError
            };

            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            return importLogResponseMessage;
        }
        #endregion

        #region Private Methods
        //This method will delete the temporary files from the server.
        private void RemoveTemporaryFiles(string fileName)
        {
            try
            {
                if (File.Exists(fileName))
                    File.Delete(fileName);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Import.ToString(), TraceLevel.Error);
            }
        }

        //This methodd will upload the file on the server.
        private string UploadImportFile(HttpPostedFileBase file)
        {
            string temporaryFilePath = string.Empty;
            try
            {
                if (!string.IsNullOrEmpty(file.FileName) && Equals(Path.GetExtension(file.FileName), $".{AdminConstants.CSVFileType}"))
                {
                    temporaryFilePath = GetFileNamewithPath(file.FileName);
                    file.SaveAs(temporaryFilePath);
                }
            }
            catch(Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Import.ToString(), TraceLevel.Error);
                if (File.Exists(temporaryFilePath))
                    File.Delete(temporaryFilePath);
            }
            return temporaryFilePath;
        }

        //This method will concat the file name and append the date format to distinguish the uploaded files.
        private string GetFileNamewithPath(string fileName)
        {
            //Remove space and special characters from file name.
            fileName = Regex.Replace(fileName, AdminConstants.FileValidation, string.Empty);

            string pathwithFileName = $"{Path.GetFileNameWithoutExtension(fileName)}{"_"}{DateTime.Now.ToString("MMddyyyyHHmmsstt")}{"_"}{SessionProxyHelper.GetUserDetails()?.UserId}{"."}{AdminConstants.CSVFileType}";
            return AppendFolderToFileName(pathwithFileName);
        }

        //This method will append the Import folder name against the file
        private string AppendFolderToFileName(string fileName)
        {            
            if (!string.IsNullOrEmpty(ZnodeAdminSettings.ImportFileUploadLocation))
            {
                string uploadFolderLocation = ZnodeAdminSettings.ImportFileUploadLocation;
                CreateFolderIfNotExists(uploadFolderLocation);
                return Path.Combine(ZnodeAdminSettings.ImportFileUploadLocation, fileName);
            }
            CreateFolderIfNotExists();
            return Path.Combine(HttpContext.Current.Server.MapPath(AdminConstants.ImportFolderPath), fileName);
        }

        //This method will create the directory to save the import files
        private void CreateFolderIfNotExists()
        {
            if (!Directory.Exists(HttpContext.Current.Server.MapPath(AdminConstants.ImportFolderPath)))
                Directory.CreateDirectory(HttpContext.Current.Server.MapPath(AdminConstants.ImportFolderPath));
        }

        //This method will create the directory to save the import files
        private void CreateFolderIfNotExists(string uploadFolderLocation)
        {
            if (!Directory.Exists(uploadFolderLocation))
                Directory.CreateDirectory(uploadFolderLocation);
        }

        //Set tool option menus for import process grid.
        private static void SetImportListToolMenu(ImportProcessLogsListViewModel listViewModel)
        {
            if (IsNotNull(listViewModel))
            {
                listViewModel.GridModel = new GridModel();
                listViewModel.GridModel.FilterColumn = new FilterColumnListModel();
                listViewModel.GridModel.FilterColumn.ToolMenuList = new List<ToolMenuModel>();
                listViewModel.GridModel.FilterColumn.ToolMenuList.Add(new ToolMenuModel { DisplayText = Admin_Resources.ButtonDelete, JSFunctionName = "EditableText.prototype.DialogDelete('ImportLogsDeletePopup')", ControllerName = "Import", ActionName = "DeleteLogs" });
            }
        }

        //Set tool option menus for manage custom template process grid.
        private void SetImportListToolMenu(ImportManageTemplateMappingListViewModel listViewModel)
        {
            if (IsNotNull(listViewModel))
            {
                listViewModel.GridModel = new GridModel();
                listViewModel.GridModel.FilterColumn = new FilterColumnListModel();
                listViewModel.GridModel.FilterColumn.ToolMenuList = new List<ToolMenuModel>();
                listViewModel.GridModel.FilterColumn.ToolMenuList.Add(new ToolMenuModel { DisplayText = Admin_Resources.ButtonDelete, JSFunctionName = "EditableText.prototype.DialogDelete('ImportTemplateDeletePopup')", ControllerName = "Import", ActionName = "DeleteImportTemplate" });
            }
        }
        #endregion
    }
}