using System;
using System.Collections.Generic;
using System.IO;
using Znode.Engine.Admin.ViewModels;
using System.Linq;
using Znode.Engine.Api.Client;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;
using System.Web;
using System.Diagnostics;
using Znode.Libraries.Framework.Business;
using Znode.Libraries.Resources;

namespace Znode.Engine.Admin.Helpers
{
    public class WidgetHelper
    {
        //Get All Available Widget List based on the provided Path.
        //In case checkAllDirectory is true, then pass the FolderPath in path parameter.
        //In case checkAllDirectory is false, then pass the full file path in path parameter.        
        public static CMSWidgetsListViewModel GetAvailableWidgets(string currentThemePath, string parentThemePath, bool checkAllDirectory)
        {
            CMSWidgetsListViewModel widgetList = new CMSWidgetsListViewModel();
            try
            {
                List<CMSWidgetsViewModel> availableWidgets = new List<CMSWidgetsViewModel>();

                List<string> searchedViews = new List<string>();

                string[] wildcardCshtmls = GetWildcardCshtmls();

                //Traversing through current theme folder.
                FileInfo[] cshtmls = checkAllDirectory ? new DirectoryInfo(currentThemePath).GetFiles("*.cshtml", SearchOption.AllDirectories)
                    : new FileInfo[] { new FileInfo(currentThemePath) };

                cshtmls = !string.IsNullOrEmpty(parentThemePath) ? cshtmls.Concat(checkAllDirectory ? new DirectoryInfo(parentThemePath).GetFiles("*.cshtml", SearchOption.AllDirectories) : new FileInfo[] { new FileInfo(parentThemePath) }).ToArray() : cshtmls;

                foreach (FileInfo cshtml in cshtmls)
                {
                    if (!searchedViews.Contains(cshtml.Name) && (wildcardCshtmls?.Length == 0 || !wildcardCshtmls.Contains(cshtml.Name?.ToLowerInvariant())))
                    {
                        string messageText = string.Empty;
                        using (StreamReader re = new StreamReader(cshtml.FullName))
                        {
                            try
                            {
                                //Read the File.
                                messageText = re.ReadToEnd();

                                //Get the widget list from the file.
                                List<CMSWidgetsViewModel> widgets = ExtractFromString(messageText, "<z-widget>", "</z-widget>", cshtml);
                                if (!Equals(widgets, null) && widgets.Count > 0)
                                {
                                    availableWidgets.AddRange(widgets);
                                }
                            }
                            catch (Exception ex)
                            {
                                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
                                Console.WriteLine("Compile fail for: {0}", cshtml.Name);
                            }
                        }
                        searchedViews.Add(cshtml.Name);
                    }
                }
                //Set the Widget default configuration from the database.
                SetWidgetDefaultConfiguration(availableWidgets);
                widgetList.CMSWidgetsList = availableWidgets;
            }
            catch { }
            return widgetList;
        }

        //Set the Actions Url for the Configurable type of widgets.
        public static void SetWidgetActions(CMSWidgetsListViewModel widgets)
        {
            if (HelperUtility.IsNotNull(widgets.CMSWidgetsList) && widgets.CMSWidgetsList.Count > 0)
                widgets.CMSWidgetsList.ForEach(item => { item.WidgetActionUrl = item.IsConfigurable ? SetWidgetActionUrl(widgets.CMSMappingId, item.CMSWidgetsId, item.MappingKey, item.Code, widgets.TypeOFMapping, widgets.DisplayName, item.DisplayName, widgets.FileName) : "#"; });
        }

        public static string[] GetWildcardCshtmls()
        {
            return new string[] { "_widgetajax.cshtml" };
        }

        //Set the Action url based on the Widget.
        private static string SetWidgetActionUrl(int mappingId, int widgetId, string mappingKey, string code, string mappingType, string displayName, string widgetName, string fileName)
        {
            string url = string.Empty;
            ZnodeCMSEnum codeEnum = (ZnodeCMSEnum)Enum.Parse(typeof(ZnodeCMSEnum), code);
            switch (codeEnum)
            {
                case ZnodeCMSEnum.LinkPanel:
                    url = $"/WebSite/GetLinkWidgetConfigurationList?cmsMappingId={mappingId}&cmsWidgetsId={widgetId}&widgetsKey={HttpUtility.UrlEncode(mappingKey)}&typeOFMapping={HttpUtility.UrlEncode(mappingType)}&displayName={ HttpUtility.UrlEncode(displayName)}&widgetName={ HttpUtility.UrlEncode(widgetName)}&fileName={ HttpUtility.UrlEncode(fileName)}";
                    break;
                case ZnodeCMSEnum.TemplateLinkPanel:
                    url = $"/WebSite/GetLinkWidgetConfigurationList?cmsMappingId={mappingId}&cmsWidgetsId={widgetId}&widgetsKey={HttpUtility.UrlEncode(mappingKey)}&typeOFMapping={HttpUtility.UrlEncode(mappingType)}&displayName={ HttpUtility.UrlEncode(displayName)}&widgetName={ HttpUtility.UrlEncode(widgetName)}&fileName={ HttpUtility.UrlEncode(fileName)}";
                    break;
                case ZnodeCMSEnum.BannerSlider:
                    url = $"/WebSite/ManageCMSWidgetSliderBanner?cmsMappingId={mappingId}&cmsWidgetsId={widgetId}&widgetKey={HttpUtility.UrlEncode(mappingKey)}&typeOFMapping={HttpUtility.UrlEncode(mappingType)}&displayName={ HttpUtility.UrlEncode(displayName)}&widgetName={ HttpUtility.UrlEncode(widgetName)}&fileName={ HttpUtility.UrlEncode(fileName)}";
                    break;
                case ZnodeCMSEnum.ProductList:
                    url = $"/WebSite/GetAssociatedProductList?cmsWidgetsId={widgetId}&cmsMappingId={mappingId}&widgetKey={HttpUtility.UrlEncode(mappingKey)}&typeOFMapping={HttpUtility.UrlEncode(mappingType)}&displayName={ HttpUtility.UrlEncode(displayName)}&widgetName={ HttpUtility.UrlEncode(widgetName)}&fileName={ HttpUtility.UrlEncode(fileName)}";
                    break;
                case ZnodeCMSEnum.TemplateProductList:
                    url = $"/WebSite/GetAssociatedProductList?cmsWidgetsId={widgetId}&cmsMappingId={mappingId}&widgetKey={HttpUtility.UrlEncode(mappingKey)}&typeOFMapping={HttpUtility.UrlEncode(mappingType)}&displayName={ HttpUtility.UrlEncode(displayName)}&widgetName={ HttpUtility.UrlEncode(widgetName)}&fileName={ HttpUtility.UrlEncode(fileName)}";
                    break;
                case ZnodeCMSEnum.CategoryList:
                    url = $"/WebSite/GetAssociatedCategoryList?cmsMappingId={mappingId}&cmsWidgetsId={widgetId}&widgetKey={HttpUtility.UrlEncode(mappingKey)}&typeOFMapping={HttpUtility.UrlEncode(mappingType)}&displayName={ HttpUtility.UrlEncode(displayName)}&widgetName={ HttpUtility.UrlEncode(widgetName)}&fileName={ HttpUtility.UrlEncode(fileName)}";
                    break;
                case ZnodeCMSEnum.BrandList:
                    url = $"/WebSite/GetAssociatedBrandList?cmsMappingId={mappingId}&cmsWidgetsId={widgetId}&widgetKey={HttpUtility.UrlEncode(mappingKey)}&typeOFMapping={HttpUtility.UrlEncode(mappingType)}&displayName={ HttpUtility.UrlEncode(displayName)}&widgetName={ HttpUtility.UrlEncode(widgetName)}&fileName={ HttpUtility.UrlEncode(fileName)}";
                    break;
                case ZnodeCMSEnum.TextEditor:
                    url = $"/WebSite/ManageTextWidgetConfiguration?mappingId={mappingId}&widgetId={widgetId}&widgetKey={HttpUtility.UrlEncode(mappingKey)}&mappingType={HttpUtility.UrlEncode(mappingType)}&displayName={ HttpUtility.UrlEncode(displayName)}&widgetName={ HttpUtility.UrlEncode(widgetName)}&fileName={ HttpUtility.UrlEncode(fileName)}";
                    break;
                case ZnodeCMSEnum.TemplateTextEditor:
                    url = $"/WebSite/ManageTextWidgetConfiguration?mappingId={mappingId}&widgetId={widgetId}&widgetKey={HttpUtility.UrlEncode(mappingKey)}&mappingType={HttpUtility.UrlEncode(mappingType)}&displayName={ HttpUtility.UrlEncode(displayName)}&widgetName={ HttpUtility.UrlEncode(widgetName)}&fileName={ HttpUtility.UrlEncode(fileName)}";
                    break;
                case ZnodeCMSEnum.OfferBanner:
                    url = $"/WebSite/ManageCMSWidgetSliderBanner?cmsMappingId={mappingId}&cmsWidgetsId={widgetId}&widgetKey={HttpUtility.UrlEncode(mappingKey)}&typeOFMapping={HttpUtility.UrlEncode(mappingType)}&displayName={ HttpUtility.UrlEncode(displayName)}&widgetName={ HttpUtility.UrlEncode(widgetName)}&fileName={ HttpUtility.UrlEncode(fileName)}&widgetCode={ HttpUtility.UrlEncode(code)}";
                    break;
                case ZnodeCMSEnum.TagManager:
                    url = $"/WebSite/ManageTextWidgetConfiguration?mappingId={mappingId}&widgetId={widgetId}&widgetKey={HttpUtility.UrlEncode(mappingKey)}&mappingType={HttpUtility.UrlEncode(mappingType)}&displayName={ HttpUtility.UrlEncode(displayName)}&widgetName={ HttpUtility.UrlEncode(widgetName)}&fileName={ HttpUtility.UrlEncode(fileName)}";
                    break;
                case ZnodeCMSEnum.FormWidget:
                    url = $"/WebSite/ManageFormWidgetConfiguration?mappingId={mappingId}&widgetId={widgetId}&widgetKey={HttpUtility.UrlEncode(mappingKey)}&mappingType={HttpUtility.UrlEncode(mappingType)}&displayName={ HttpUtility.UrlEncode(displayName)}&widgetName={ HttpUtility.UrlEncode(widgetName)}&fileName={ HttpUtility.UrlEncode(fileName)}";
                    break;
                case ZnodeCMSEnum.SearchWidget:
                    url = $"/WebSite/ManageSearchWidgetConfiguration?mappingId={mappingId}&widgetId={widgetId}&widgetKey={HttpUtility.UrlEncode(mappingKey)}&mappingType={HttpUtility.UrlEncode(mappingType)}&displayName={ HttpUtility.UrlEncode(displayName)}&widgetName={ HttpUtility.UrlEncode(widgetName)}&fileName={ HttpUtility.UrlEncode(fileName)}";
                    break;
                case ZnodeCMSEnum.ImageWidget:
                    url = $"/WebSite/ManageMediaWidgetConfiguration?mappingId={mappingId}&widgetId={widgetId}&widgetKey={HttpUtility.UrlEncode(mappingKey)}&mappingType={HttpUtility.UrlEncode(mappingType)}&displayName={ HttpUtility.UrlEncode(displayName)}&widgetName={ HttpUtility.UrlEncode(widgetName)}&fileName={ HttpUtility.UrlEncode(fileName)}";
                    break;
                case ZnodeCMSEnum.VideoWidget:
                    url = $"/WebSite/ManageMediaWidgetConfiguration?mappingId={mappingId}&widgetId={widgetId}&widgetKey={HttpUtility.UrlEncode(mappingKey)}&mappingType={HttpUtility.UrlEncode(mappingType)}&displayName={ HttpUtility.UrlEncode(displayName)}&widgetName={ HttpUtility.UrlEncode(widgetName)}&fileName={ HttpUtility.UrlEncode(fileName)}";
                    break;
                case ZnodeCMSEnum.ContentContainer:
                    url = $"/WebSite/ManageCMSContentContainerWidget?cmsMappingId={mappingId}&cmsWidgetsId={widgetId}&widgetKey={HttpUtility.UrlEncode(mappingKey)}&typeOFMapping={HttpUtility.UrlEncode(mappingType)}&displayName={ HttpUtility.UrlEncode(displayName)}&widgetName={ HttpUtility.UrlEncode(widgetName)}&fileName={ HttpUtility.UrlEncode(fileName)}";
                    break;
                default:
                    break;
            }

            return url;
        }

        //Get the Available Widget List from the input file stream.
        private static List<CMSWidgetsViewModel> ExtractFromString(string text, string startString, string endString, FileInfo file)
        {
            List<CMSWidgetsViewModel> matched = new List<CMSWidgetsViewModel>();
            int indexStart = 0, indexEnd = 0;
            bool exit = false;
            while (!exit)
            {
                indexStart = text.IndexOf(startString);
                indexEnd = text.IndexOf(endString);
                if (indexStart != -1 && indexEnd != -1)
                {
                    string match = text.Substring(indexStart + startString.Length,
                          indexEnd - indexStart - startString.Length);
                    match = match.TrimStart().TrimEnd().Replace("@Html.WidgetPartial(", "").Replace("@Html.WidgetPartialAjax(", "").Replace("@Html.WidgetPartialAuto(", "").TrimEnd(')');

                    string[] param = match.Split(',');
                    if (!Equals(param, null) && !Equals(param[0], null) && !Equals(param[1], null) && !Equals(param[2], null) && !Equals(param[3], null))
                    {
                        CMSWidgetsViewModel model = new CMSWidgetsViewModel();
                        model.Code = param[0].TrimStart().TrimEnd().Replace("\"", "");
                        model.DisplayName = param[1].TrimStart().TrimEnd().Replace("\"", "");
                        model.MappingKey = param[2].TrimStart().TrimEnd().Replace("\"", "");
                        model.TemplateName = file.Name;
                        model.TemplatePath = file.FullName;
                        matched.Add(model);
                    }
                    text = text.Substring(indexEnd + endString.Length);

                }
                else
                    exit = true;
            }
            return matched;
        }

        //Set the Configurable Widget Default Configuration.
        private static void SetWidgetDefaultConfiguration(List<CMSWidgetsViewModel> widgets)
        {
            if (HelperUtility.IsNotNull(widgets) && widgets.Count > 0)
            {
                //Get all the Widget Codes.
                ParameterModel lstCodes = new ParameterModel() { Ids = string.Join(",", widgets.Select(x => x.Code).Distinct().ToList()) };
                ICMSWidgetsClient _widgetConfigurationClient = new CMSWidgetsClient();

                //Call the API to get the Widget default configuration details.
                List<CMSWidgetsModel> widgetModel = _widgetConfigurationClient.GetWidgetByCodes(lstCodes)?.CMSWidgetsList;

                //Assign the configuration details for the widgets.
                widgets.ForEach(item =>
                {
                    CMSWidgetsModel model = widgetModel?.Where(x => x.Code == item.Code)?.FirstOrDefault();
                    if ((HelperUtility.IsNotNull(model)))
                    {
                        item.IsConfigurable = model.IsConfigurable;
                        item.CMSWidgetsId = model.CMSWidgetsId;
                        item.FileName = model.FileName;
                    }
                });
            }
        }

       // Method to get sorted list of Configurable widgets
        public static List<CMSWidgetsViewModel> GetConfigurableWidget(CMSWidgetsListViewModel listModel)
        {
          
            List<string> lstTemplateName = listModel?.CMSWidgetsList?.GroupBy(x => x.TemplateName)
                 .Select(y => y.First().TemplateName)
                 .ToList();      
            List<CMSWidgetsViewModel> lstAreas = new List<CMSWidgetsViewModel>();
            if (lstTemplateName != null)
            {
                lstTemplateName.ForEach(item =>
                {
                    bool? IsConfigurable = listModel?.CMSWidgetsList?.Where(y => y.TemplateName == item).Any(y => y.IsConfigurable == true);
                    lstAreas.Add(listModel?.CMSWidgetsList?
                                .Where(x => x.TemplateName == item && x.IsConfigurable == IsConfigurable)
                                .FirstOrDefault());
                    string input = item;
                    input = input.Replace(".cshtml", "");
                    string template = input.Replace("_", "");
                    template = string.IsNullOrEmpty(Admin_Resources.ResourceManager.GetString(string.Concat(AdminConstants.ZWidgetFileKey, template))) ? template : Admin_Resources.ResourceManager.GetString(string.Concat(AdminConstants.ZWidgetFileKey, template));
                    if (lstAreas != null)
                    {
                        lstAreas.FirstOrDefault(x => x.TemplateName == item).DisplayName = template;
                    }
                });             
            }
            return lstAreas; 
        }

        //Method for Getting Widget Name from the directory Path
        public static string WidgetPathModifier(string path)
        {
            int index = path.LastIndexOf("\\");
            path = path.Remove(index, path.Length - index);	
            index = path.LastIndexOf("\\");	
            path = path.Remove(0, index+1);	
            if (path.Contains(" "))	
            path =  path.Replace(" ", string.Empty);
            return path;
        }

        //To get List of Names of directories 
        public static List<String> GetParentDirectoryList(List<CMSWidgetsViewModel> lstAreas )
        {           
            List<string> parentList = new List<string>();
            lstAreas.ForEach(x => 
            {
                parentList?.Add(WidgetPathModifier(x.TemplatePath));
            });
            parentList = parentList?.Distinct().ToList();     
            parentList = WidgetSorter(parentList);
            parentList.Remove(ZnodeConstant.SharedFolder);
            return parentList;
        }
        // to sort the tab according to the display name.
        public static List<string> WidgetSorter(List<string> widgetNames)
        {
            if (widgetNames != null)
            {
                SortedDictionary<string, string> displayName = new SortedDictionary<string, string>();
                foreach (string name in widgetNames)
                {
                    displayName.Add(string.IsNullOrEmpty(Admin_Resources.ResourceManager.GetString(name)) ? name : Admin_Resources.ResourceManager.GetString(name), name);
                }
                if (displayName != null)
                {
                    widgetNames = displayName.Select(x => x.Value).ToList();
                }              
            }
            return widgetNames;
        }       
    }
}
