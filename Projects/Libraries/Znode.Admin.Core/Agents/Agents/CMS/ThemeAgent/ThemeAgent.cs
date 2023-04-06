using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using Znode.Engine.Admin.Extensions;
using Znode.Engine.Admin.Helpers;
using Znode.Engine.Admin.Maps;
using Znode.Engine.Admin.ViewModels;
using Znode.Engine.Api.Client;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;
using Znode.Engine.Exceptions;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;
using Znode.Libraries.Resources;
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;

namespace Znode.Engine.Admin.Agents
{
    public class ThemeAgent : BaseAgent, IThemeAgent
    {
        #region Private Variables
        private readonly IThemeClient _themeClient;
        private readonly ICSSClient _cssClient;
        #endregion

        #region Constructor
        public ThemeAgent(IThemeClient themeClient, ICSSClient cssClient)
        {
            _themeClient = GetClient<IThemeClient>(themeClient);
            _cssClient = GetClient<ICSSClient>(cssClient);
        }
        #endregion

        #region public Methods

        #region Theme
        public virtual ThemeListViewModel GetThemeList(FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
			ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
			//Get the sort collection for theme id descending.
			sorts = HelperMethods.SortDesc(ZnodeCMSThemeEnum.CMSThemeId.ToString(), sorts);
			ZnodeLogging.LogMessage("Input parameters of method GetThemes: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new { filters = filters, sorts = sorts });
			ThemeListModel themeListModel = _themeClient.GetThemes(filters, sorts, pageIndex, pageSize);
            ThemeListViewModel themeListViewModel = new ThemeListViewModel { ThemeList = themeListModel?.Themes?.ToViewModel<ThemeViewModel>().ToList() };
            SetListPagingData(themeListViewModel, themeListModel);

            //Set tool options for grid.
            SetThemeToolMenus(themeListViewModel);
			ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
			return themeListViewModel?.ThemeList?.Count > 0 ? themeListViewModel : new ThemeListViewModel();
        }

        //Gets list of Parent Themes in Select Item List format
        public virtual List<SelectListItem> GetParentThemeList()
        {
			ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
			FilterCollection filter = new FilterCollection() { new FilterTuple(ZnodeCMSThemeEnum.IsParentTheme.ToString(), FilterOperators.Equals, "true") };
			ZnodeLogging.LogMessage("Input parameters of method GetThemeList: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new { filter = filter});
			return ThemeViewModelMap.ToSelectListItemForThemes(GetThemeList(filter, null, null, null)?.ThemeList);
        }

        //Create new theme.
        public virtual ThemeViewModel CreateTheme(ThemeViewModel themeViewModel)
        {
			ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
			string fileName = themeViewModel?.FilePath?.FileName;
			ZnodeLogging.LogMessage("fileName: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new { fileName = fileName });
			try
			{
                if (IsNotNull(themeViewModel))
                {
                    if (IsNotNull(fileName))
                    {
                        //Extract the uploaded zip file.
                        ExtractThemeTemporarily(themeViewModel.FilePath);

                        //Get the list of CSS.
                        themeViewModel.CssList = GetListOfCssModel(fileName);
                    }
                    //Create new theme and get its details.
                    themeViewModel = _themeClient.CreateTheme(themeViewModel?.ToModel<ThemeModel>())?.ToViewModel<ThemeViewModel>();

                    if (themeViewModel?.CMSThemeId > 0)
                        MoveFolderContent(fileName, themeViewModel.Name);
					ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
				}

			}
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Warning);
                //Remove the zip file and the temporarily extracted folder.
                RemoveTemporaryFiles(fileName);

                if (Equals(ex.ErrorCode, ErrorCodes.AlreadyExist))
                    return (ThemeViewModel)GetViewModelWithErrorMessage(themeViewModel, Admin_Resources.ErrorThemeAlreadyExist);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
                //Remove the zip file and the temporarily extracted folder.
                RemoveTemporaryFiles(fileName);
                return (ThemeViewModel)GetViewModelWithErrorMessage(themeViewModel, Admin_Resources.ErrorValidFile);
            }
            finally
            {
                if (IsNotNull(fileName))
                {
                    //delete the uploaded zip file which is temporarily saved.
                    if (File.Exists(Path.Combine(HttpContext.Current.Server.MapPath("~/Data"), fileName)))
                        File.Delete(Path.Combine(HttpContext.Current.Server.MapPath("~/Data"), fileName));
                }
            }
            return themeViewModel;
        }

        //Get theme by CMS theme Id.
        public virtual ThemeViewModel GetTheme(int cmsThemeId)
        {
			ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
			if (cmsThemeId > 0)
            {
                ThemeViewModel themeViewModel = _themeClient.GetTheme(cmsThemeId).ToViewModel<ThemeViewModel>();
                //Bind File Path to model
                themeViewModel.IsFilePathExists = RelationalThemeHelper.ThemeDirectoryExists(AdminConstants.ThemeFolderPath, themeViewModel.Name)
                    && (themeViewModel.IsParentTheme || RelationalThemeHelper.ThemeDirectoryExists(AdminConstants.ThemeFolderPath, themeViewModel.ParentThemeName));
				ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
				return themeViewModel;
            }
			ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
			return new ThemeViewModel { HasError = true };
        }

        //Update theme.
        public virtual ThemeViewModel UpdateTheme(ThemeViewModel themeViewModel)
        {
			ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
			if (IsNotNull(themeViewModel))
            {
                string fileName = themeViewModel.FilePath?.FileName;
                string themeName = themeViewModel.Name;
				ZnodeLogging.LogMessage("fileName and themeName: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new { fileName = fileName, themeName = themeName });
				try
				{
                    if (IsNotNull(themeViewModel.FilePath))
                        return SaveFile(ref themeViewModel, fileName, themeName);
					ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
					return themeViewModel;
                }
                catch (ZnodeException ex)
                {
                    ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Warning);
                    //Remove the zip file and the temporarily extracted folder.
                    RemoveTemporaryFiles(fileName);

                    if (Equals(ex.ErrorCode, ErrorCodes.AlreadyExist))
                        return (ThemeViewModel)GetViewModelWithErrorMessage(themeViewModel, Admin_Resources.ErrorThemeAlreadyExist);
                }
                catch (Exception ex)
                {
                    ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
                    //Remove the zip file and the temporarily extracted folder.
                    RemoveTemporaryFiles(fileName);
                    return (ThemeViewModel)GetViewModelWithErrorMessage(themeViewModel, Admin_Resources.ErrorValidFile);
                }
                finally
                {
                    //delete the uploaded zip file which is temporarily saved.
                    if (IsNotNull(fileName))
                        if (File.Exists(Path.Combine(HttpContext.Current.Server.MapPath("~/Data"), fileName)))
                            File.Delete(Path.Combine(HttpContext.Current.Server.MapPath("~/Data"), fileName));
                }
            }
			ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
			return themeViewModel;
        }

        //Delete theme which is not associated to store.
        public virtual bool DeleteTheme(string themeId, string cmsThemeName, out string errorMessage)
        {
			ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
			ZnodeLogging.LogMessage("Input parameters: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new { themeId = themeId, cmsThemeName = cmsThemeName });
			errorMessage = Admin_Resources.ErrorFailedToDelete;
            bool isDeleted = false;
            try
            {
                //Get the list of theme names.
                List<string> themeNames = !string.IsNullOrEmpty(cmsThemeName) ? cmsThemeName.Split(',')?.ToList() : null;
				ZnodeLogging.LogMessage("themeNames list count: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new { themeNamesListCount = themeNames?.Count });

				if (_themeClient.DeleteTheme(themeId))
                {
                    if (themeNames?.Count > 0)
                    {
                        foreach (string themeName in themeNames)
                        {
                            //Delete the directory of theme name.
                            if (Directory.Exists(Path.Combine(HttpContext.Current.Server.MapPath(AdminConstants.ThemeFolderPath), themeName)))
                                Directory.Delete(new FileInfo(Path.Combine(HttpContext.Current.Server.MapPath(AdminConstants.ThemeFolderPath), themeName)).FullName, true);

                            DirectoryInfo hdDirectoryInWhichToSearch = new DirectoryInfo(HttpContext.Current.Server.MapPath(AdminConstants.RevisionFolderPath));
                            FileSystemInfo[] filesAndDirs = hdDirectoryInWhichToSearch.GetFileSystemInfos("*" + themeName + "_" + "*");

                            foreach (var item in filesAndDirs)
                            {
                                //delete the upload zip file which is temporarily saved.
                                if (File.Exists(Path.Combine(HttpContext.Current.Server.MapPath(AdminConstants.RevisionFolderPath), item.Name)))
                                    File.Delete(Path.Combine(HttpContext.Current.Server.MapPath(AdminConstants.RevisionFolderPath), item.Name));
                            }
                        }
                    }
                    isDeleted = true;
                }
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Warning);
                if (ex.ErrorCode == ErrorCodes.AssociationDeleteError)
                    errorMessage = Admin_Resources.ErrorThemeDelete;
                isDeleted = false;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
                errorMessage = ex.Message;
                isDeleted = false;
            }
			ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
			return isDeleted;
        }

        //Check whether the Theme Name already exists.
        public virtual bool CheckThemeNameExist(string themeName, int cmsThemeId)
        {
			ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
			ZnodeLogging.LogMessage("Input parameters: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new { cmsThemeId = cmsThemeId, themeName = themeName });

			FilterCollection filters = new FilterCollection();
            filters.Add(new FilterTuple(ZnodeCMSThemeEnum.Name.ToString(), FilterOperators.Is, themeName.Trim()));
			ZnodeLogging.LogMessage("Input parameters of method GetThemes: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new { filters = filters });
			//Get the theme List based on the theme name filter.
			ThemeListModel themeList = _themeClient.GetThemes(filters, null, null, null);
            if (IsNotNull(themeList) && IsNotNull(themeList.Themes))
            {
                if (cmsThemeId > 0)
                {
                    //Set the status in case the Theme is open in edit mode.
                    ThemeModel theme = themeList.Themes.Find(x => x.CMSThemeId == cmsThemeId);
                    if (IsNotNull(theme))
                        return !Equals(theme.Name.Trim(), themeName.Trim());
                }
                return themeList.Themes.FindIndex(x => x.Name == themeName) != -1;
            }
            return false;
        }

        //Get theme details by theme themeName.
        public virtual string DownloadTheme(string themeName)
        {
			ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
			if (Directory.Exists(Path.Combine(HttpContext.Current.Server.MapPath(AdminConstants.ThemeFolderPath), themeName)))
                return Path.Combine(HttpContext.Current.Server.MapPath(AdminConstants.ThemeFolderPath), themeName);

            return null;
        }

        //Get the contents of zip file of theme.
        public virtual byte[] GetZipFile(string filePath, string themeName)
        {
			ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
			ZnodeLogging.LogMessage("Input parameters: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new { filePath = filePath, themeName = themeName });

			//Create a zip file to download.
			Ionic.Zip.ZipFile zip = new Ionic.Zip.ZipFile();
            zip.AddDirectory(filePath);
            zip.Save($"{ filePath}/{themeName}.zip");

            //Read all bytes of zip file.
            byte[] fileBytes = File.ReadAllBytes($"{ filePath}/{themeName}.zip");

            //Dispose the zip file object.
            zip.Dispose();

            //Delete saved zip File.
            if (File.Exists(Path.Combine(HttpContext.Current.Server.MapPath(AdminConstants.ThemeFolderPath), $"{ themeName}\\{themeName}.zip")))
                File.Delete(new FileInfo(Path.Combine($"{HttpContext.Current.Server.MapPath(AdminConstants.ThemeFolderPath)}\\{themeName}\\", themeName + ".zip")).ToString());
			ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
			return fileBytes;
        }
        #endregion

        #region Revised theme
        //Delete theme which is not associated to store.
        public virtual bool DeleteRevisedTheme(string cmsThemeName, out string errorMessage)
        {
			ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
			ZnodeLogging.LogMessage("Input parameter: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new { cmsThemeName = cmsThemeName });

			errorMessage = Admin_Resources.ErrorFailedToDelete;
            bool isDeleted = false;
            try
            {
                //Get the list of theme names.
                List<string> themeNames = !string.IsNullOrEmpty(cmsThemeName) ? cmsThemeName.Split(',')?.ToList() : null;

                if (themeNames?.Count > 0)
                {
                    foreach (string themeName in themeNames)
                    {
                        //delete the upload zip file which is temporarily saved.
                        if (File.Exists(Path.Combine(HttpContext.Current.Server.MapPath(AdminConstants.RevisionFolderPath), themeName)))
                            File.Delete(Path.Combine(HttpContext.Current.Server.MapPath(AdminConstants.RevisionFolderPath), themeName));
                    }
                }
                isDeleted = true;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
                errorMessage = ex.Message;
                isDeleted = false;
            }
			ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
			return isDeleted;
        }

        //Get theme revision theme.
        public virtual ThemeListViewModel GetThemeRevisionList(int CMSThemeId, string themeName, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
			ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
			ZnodeLogging.LogMessage("Input parameters: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new { CMSThemeId = CMSThemeId, themeName = themeName });

			string partialName = themeName;

            DirectoryInfo hdDirectoryInWhichToSearch = new DirectoryInfo(HttpContext.Current.Server.MapPath(AdminConstants.RevisionFolderPath));
            FileSystemInfo[] filesAndDirs = hdDirectoryInWhichToSearch?.GetFileSystemInfos("*" + partialName + "_" + "*")?.OrderByDescending(x => x.CreationTime).ToArray();

            ThemeListViewModel themeListViewModel = new ThemeListViewModel();

            foreach (FileSystemInfo foundFile in filesAndDirs)
            {
                ThemeViewModel themeViewModel = new ThemeViewModel();
                themeViewModel.Name = foundFile.Name;
                themeViewModel.CreatedDate = foundFile.CreationTime.ToTimeZoneDateTimeFormat();
                themeViewModel.CMSThemeId = CMSThemeId;
                themeListViewModel.ThemeList.Add(themeViewModel);
            }
            //Set tool options for grid.
            SetRevisedThemeToolMenus(themeListViewModel);
			ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
			return themeListViewModel;
        }

        //Update revised theme.
        public virtual bool UpdateRevisedTheme(int cmsThemeId, string themeName)
        {
			ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
			ZnodeLogging.LogMessage("Input parameters: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new { cmsThemeId = cmsThemeId, themeName = themeName });

			if (!string.IsNullOrEmpty(themeName))
            {
                string temporaryPath = string.Empty;
                string themeFolderPath = string.Empty;
                string oldThemeName = themeName.Substring(0, themeName.IndexOf("_"));
                try
                {
                    ThemeViewModel themeViewModel = new ThemeViewModel();
                    themeViewModel.CMSThemeId = cmsThemeId;
                    themeViewModel.Name = oldThemeName;

                    string[] existingCssResult;

                    //Check revised theme CSS exists in Old theme CSS.
                    CSSListViewModel cssListViewModel = CheckRevisedThemeCssExistsInOldThemeCss(themeName, themeViewModel, out existingCssResult);
					ZnodeLogging.LogMessage("existingCssResult length: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new { existingCssResultLength = existingCssResult?.Length});

					//Check old theme CSS exists in the revised theme CSS.
					if (existingCssResult.Length <= 0)
                    {
                        //Remove same CSS from the old theme.
                        foreach (CSSViewModel item in cssListViewModel.CssList)
                            themeViewModel.CssList.RemoveAll(x => x.CSSName == item.CSSName.Replace(".css", ""));

                        //Create new theme and get its details.
                        themeViewModel = _themeClient.UpdateTheme(themeViewModel?.ToModel<ThemeModel>())?.ToViewModel<ThemeViewModel>();

                        // Move old theme content to revision folder.
                        MoveFolderContentToRevisionFolder(themeName, oldThemeName);

                        if (!string.IsNullOrEmpty(themeName) && Equals(Path.GetExtension(themeName), $".{AdminConstants.ZipFileType}"))
                        {
                            temporaryPath = Path.Combine(HttpContext.Current.Server.MapPath(AdminConstants.RevisionFolderPath), themeName);
                            themeFolderPath = Path.Combine(HttpContext.Current.Server.MapPath(AdminConstants.ThemeFolderPath), oldThemeName);
                            ZipFile.ExtractToDirectory(temporaryPath, themeFolderPath);
                        }

                        if (File.Exists(temporaryPath))
                            File.Delete(temporaryPath);
						ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
						return true;
                    }
                    else
                    {
                        //Remove the zip file and the temporarily extracted folder.
                        RemoveTemporaryFiles(themeName);
						ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
						return false;
                    }

                }
                catch (Exception ex)
                {
                    ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
                    if (File.Exists(temporaryPath))
                        File.Delete(temporaryPath);

                    return false;
                }
                finally
                {
                    //Delete the directory of theme name.
                    if (Directory.Exists(Path.Combine(HttpContext.Current.Server.MapPath(AdminConstants.TemporaryThemeFolderPath), Path.GetFileNameWithoutExtension(themeName))))
                        Directory.Delete(new FileInfo(Path.Combine(HttpContext.Current.Server.MapPath(AdminConstants.TemporaryThemeFolderPath), Path.GetFileNameWithoutExtension(themeName))).FullName, true);

                }
            }
			ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
			return false;

        }

        #endregion

        #region Css
        //Get CSS list.
        public virtual CSSListViewModel GetCssList(int themeId, FilterCollection filters, SortCollection sorts, int pageIndex, int pageSize)
        {
			ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
			//Get the sort collection for theme CSS id descending.
			sorts = HelperMethods.SortDesc(ZnodeCMSThemeCSSEnum.CMSThemeCSSId.ToString(), sorts);

			ZnodeLogging.LogMessage("Input parameters of GetCssListByThemeId : ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new { filters = filters, sorts = sorts });
			CSSListModel cssListModel = _cssClient.GetCssListByThemeId(themeId, filters, sorts, pageIndex, pageSize);
            CSSListViewModel cssListViewModel = new CSSListViewModel { CssList = cssListModel?.CSSs?.ToViewModel<CSSViewModel>()?.ToList(), CMSThemeName = cssListModel?.CMSThemeName };
            cssListViewModel?.CssList?.ForEach(x => x.ThemeName = cssListViewModel?.CMSThemeName);
            SetListPagingData(cssListViewModel, cssListModel);
            SetThemeCssToolMenus(cssListViewModel);
			ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
			return cssListViewModel?.CssList?.Count > 0 ? cssListViewModel : new CSSListViewModel { CssList = new List<CSSViewModel>(), CMSThemeName = cssListModel?.CMSThemeName };
        }

        //Create CSS
        public virtual CSSViewModel CreateCSS(CSSViewModel cssViewModel)
        {
            try
            {
				ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
				if (IsNotNull(cssViewModel))
                {
                    string themeName = cssViewModel.ThemeName;
					ZnodeLogging.LogMessage("themeName property of CSSViewModel: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new { themeName = themeName });
					HttpPostedFileBase[] cssFile = cssViewModel.FilePath != null && cssViewModel.FilePath.Any(x => x != null) ? cssViewModel.FilePath : null;
                    CSSModel cssModel = cssViewModel?.ToModel<CSSModel>();
                    if (cssFile?.Count() > 0)
                    {
                        foreach (HttpPostedFileBase item in cssFile)
                        {
                            cssModel.CSSName = Path.GetFileNameWithoutExtension(item.FileName);
                            cssModel.cssList.Add(cssModel.CSSName);
                        }
                    }
                    else
                        cssModel.cssList.Add(cssModel.CSSName);

                    //Create new theme and get its details.
                    cssViewModel = _cssClient.CreateCSS(cssModel)?.ToViewModel<CSSViewModel>();

                    if (cssViewModel.CMSThemeCSSId > 0 && cssFile != null)
                    {
                        //Saved multiple CSS file.
                        foreach (HttpPostedFileBase cssName in cssFile)
                        {
                            if (Directory.Exists(Path.Combine(HttpContext.Current.Server.MapPath(AdminConstants.ThemeFolderPath), themeName)))
                                cssName.SaveAs(Path.Combine($"{HttpContext.Current.Server.MapPath(AdminConstants.ThemeFolderPath)}/{themeName}/Content/Css", cssName.FileName));
                        }
                    }
                }
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Warning);
                if (Equals(ex.ErrorCode, ErrorCodes.AlreadyExist))
                    return (CSSViewModel)GetViewModelWithErrorMessage(cssViewModel, Admin_Resources.ErrorCSSAlreadyExists);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
                return (CSSViewModel)GetViewModelWithErrorMessage(cssViewModel, ex.Message);
            }
			ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
			return cssViewModel;
        }

        //Delete CSS.
        public virtual bool DeleteCss(string cssId, string cssName, string themeName, out string errorMessage)
        {
			ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
			errorMessage = Admin_Resources.ErrorFailedToDelete;
            bool isDeleted = false;
            try
            {
                //Get the list of CSS names.
                List<string> cssNames = !string.IsNullOrEmpty(cssName) ? cssName?.Split(',')?.ToList() : null;
				ZnodeLogging.LogMessage("cssNames list count: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new { cssNamesListCount = cssNames?.Count });

				if (_cssClient.DeleteCSS(cssId))
                {
                    if (cssNames?.Count > 0)
                    {
                        foreach (string name in cssNames)
                        {
                            //delete the file of CSS name.
                            if (File.Exists(Path.Combine($"{HttpContext.Current.Server.MapPath(AdminConstants.ThemeFolderPath)}\\{themeName}\\Content\\Css", name)))
                                File.Delete(new FileInfo(Path.Combine($"{HttpContext.Current.Server.MapPath(AdminConstants.ThemeFolderPath)}\\{themeName}\\Content\\Css", name)).FullName);
                        }
                    }
                    isDeleted = true;
                }
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Warning);
                if (ex.ErrorCode == ErrorCodes.AssociationDeleteError)
                    errorMessage = Admin_Resources.ErrorCSSDelete;

                isDeleted = false;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
                errorMessage = ex.Message;
                isDeleted = false;
            }
			ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
			return isDeleted;
        }

        //Get theme details by CSS name.
        public virtual string DownloadCss(int CMSThemeId, string cssName, string themeName)
        {
			ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
			if (File.Exists(Path.Combine($"{HttpContext.Current.Server.MapPath(AdminConstants.ThemeFolderPath)}\\{themeName}\\Content\\Css", cssName)))
                return Path.Combine($"{HttpContext.Current.Server.MapPath(AdminConstants.ThemeFolderPath)}\\{themeName}\\Content\\Css");

            return null;
        }
        #endregion

        #region Associate Store

        //Associate stores to theme.
        public virtual bool AssociateStore(int cmsThemeId, string storeIds)
        {
            try
            {
				ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
				return _themeClient.AssociateStore(PriceViewModelMap.ToAssociateStoreListModel(cmsThemeId, storeIds));
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
                return false;
            }
        }

        //Unassociate associated store list from theme.
        public virtual bool RemoveAssociatedStores(string cmsPortalThemeId)
           => _themeClient.RemoveAssociatedStores(new ParameterModel { Ids = cmsPortalThemeId });

        #endregion

        #endregion

        #region Private Methods
        //Set the Tool Menus for Theme List Grid View.
        private void SetThemeToolMenus(ThemeListViewModel model)
        {
            if (IsNotNull(model))
            {
                model.GridModel = GetGridModel();
                model.GridModel.FilterColumn.ToolMenuList.Add(new Models.ToolMenuModel { DisplayText = Admin_Resources.ButtonDelete, JSFunctionName = "EditableText.prototype.DialogDelete('DeletePopup')", ControllerName = "Theme", ActionName = "Delete" });
            }
        }

        //Set the Tool Menus for Theme List Grid View.
        private void SetRevisedThemeToolMenus(ThemeListViewModel model)
        {
            if (IsNotNull(model))
            {
                model.GridModel = GetGridModel();
                model.GridModel.FilterColumn.ToolMenuList.Add(new Models.ToolMenuModel { DisplayText = Admin_Resources.ButtonDelete, JSFunctionName = "EditableText.prototype.DialogDelete('DeleteRevisedThemePopup')", ControllerName = "Theme", ActionName = "DeleteRevisedTheme" });
            }
        }

        //Set tool menu for CSS list.
        private void SetThemeCssToolMenus(CSSListViewModel model)
        {
            if (IsNotNull(model))
            {
                model.GridModel = new Models.GridModel();
                model.GridModel.FilterColumn = new Models.FilterColumnListModel();
                model.GridModel.FilterColumn.ToolMenuList = new List<Models.ToolMenuModel>();
                model.GridModel.FilterColumn.ToolMenuList.Add(new Models.ToolMenuModel { DisplayText = Admin_Resources.ButtonDelete, JSFunctionName = "EditableText.prototype.DialogDelete('DeletePopup')", ControllerName = "Theme", ActionName = "DeleteCss" });
            }
        }

        //Extract the uploaded zip file.
        private void ExtractThemeTemporarily(HttpPostedFileBase file)
        {
            string temporaryPath = string.Empty;
            string temporaryThemeFolderPath = string.Empty;
            try
            {
                if (!string.IsNullOrEmpty(file.FileName) && Equals(Path.GetExtension(file.FileName), $".{AdminConstants.ZipFileType}"))
                {
                    temporaryPath = Path.Combine(HttpContext.Current.Server.MapPath("~/Data"), file.FileName);
                    temporaryThemeFolderPath = Path.Combine(HttpContext.Current.Server.MapPath(AdminConstants.TemporaryThemeFolderPath));
                    file.SaveAs(temporaryPath);
                    ZipFile.ExtractToDirectory(temporaryPath, temporaryThemeFolderPath);
                }

            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
                if (File.Exists(temporaryPath))
                    File.Delete(temporaryPath);

                if (File.Exists(Path.Combine(temporaryThemeFolderPath, file.FileName)))
                    File.Delete(Path.Combine(temporaryThemeFolderPath, file.FileName));
            }
        }

        //Remove the zip file and the temporarily extracted folder.
        private void RemoveTemporaryFiles(string fileName)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(HttpContext.Current.Server.MapPath(AdminConstants.TemporaryThemeFolderPath));

            //Delete files from the directories.
            foreach (FileInfo file in directoryInfo.GetFiles())
                file.Delete();

            //Delete sub directories from the directory.
            foreach (DirectoryInfo directory in directoryInfo.GetDirectories())
                directory.Delete(true);
        }

        //Check old CSS content from theme is exists in newly uploaded theme.
        private void CheckOldCSSExistInTheNewCSS(ThemeViewModel themeViewModel, string fileName, out List<CSSViewModel> oldCSSList, out string[] result)
        {
            //Extract the uploaded zip file.
            ExtractThemeTemporarily(themeViewModel.FilePath);

            //Get the list of CSS.
            themeViewModel.CssList = GetListOfCssModel(fileName);
            CSSListViewModel cssListViewModel = new CSSListViewModel { CssList = _cssClient.GetCssListByThemeId(Convert.ToInt32(themeViewModel.CMSThemeId), null, null, null, null)?.CSSs?.ToViewModel<CSSViewModel>()?.ToList() };

            List<CSSViewModel> newCSSList = themeViewModel.CssList.Except(cssListViewModel.CssList).ToList();
            oldCSSList = cssListViewModel.CssList.Except(themeViewModel.CssList).ToList();
            string oldCss = string.Join(",", oldCSSList.Select(x => x.CSSName.Replace(".css", "")));
            string newCss = string.Join(",", newCSSList.Select(x => x.CSSName));
			ZnodeLogging.LogMessage("oldCss and newCss: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new { oldCss = oldCss, newCss = newCss });

			result = oldCss.Split(new char[] { ',' })
                            .Except(newCss.Split(new char[] { ',' })).ToArray();
        }

        //Get the list of CSS.
        private List<CSSViewModel> GetListOfCssModel(string fileName)
        {
            fileName = fileName.Split('(')?.Length > 0 ? fileName.Split('(')[0] : fileName;
            List<CSSViewModel> CssList = new List<CSSViewModel>();
            CSSViewModel cssViewModel = new CSSViewModel();

            //Get the path of CSS file.
            string filePath = Directory.Exists(Path.Combine(HttpContext.Current.Server.MapPath(AdminConstants.TemporaryThemeFolderPath), Path.GetFileNameWithoutExtension(fileName))) ?
                $"{HttpContext.Current.Server.MapPath(AdminConstants.TemporaryThemeFolderPath)}/{Path.GetFileNameWithoutExtension(fileName)}/Content/Css" : $"{HttpContext.Current.Server.MapPath(AdminConstants.TemporaryThemeFolderPath)}/Content/Css";

            if (Directory.Exists(filePath))
                foreach (string file in Directory.GetFiles(new FileInfo(filePath)?.FullName, "*.css"))
                {
                    string cssName = Path.GetFileNameWithoutExtension(file);
                    if (Regex.IsMatch(cssName = Path.GetFileNameWithoutExtension(file), AdminConstants.CssNameValidation))
                        CssList.Add(new CSSViewModel { CSSName = cssName });
                }
			ZnodeLogging.LogMessage("CssList count: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new { CssListCount = CssList?.Count});
			return CssList;
        }

        //Move content of temporary folder to theme folder.
        private void MoveFolderContent(string fileName, string themeName)
        {
            if (!Directory.Exists(HttpContext.Current.Server.MapPath(AdminConstants.ThemeFolderPath)))
                Directory.CreateDirectory(HttpContext.Current.Server.MapPath(AdminConstants.ThemeFolderPath));

            if (!string.IsNullOrEmpty(fileName))
            {
                fileName = fileName.Split('(')?.Length > 0 ? fileName.Split('(')[0] : fileName;

                //Get the path of CSS file.
                string filePath = Directory.Exists(Path.Combine(HttpContext.Current.Server.MapPath(AdminConstants.TemporaryThemeFolderPath), Path.GetFileNameWithoutExtension(fileName))) ?
                    Path.Combine(HttpContext.Current.Server.MapPath(AdminConstants.TemporaryThemeFolderPath), Path.GetFileNameWithoutExtension(fileName)) : HttpContext.Current.Server.MapPath(AdminConstants.TemporaryThemeFolderPath);

                //Get the source file path and target file path and move the content from source to target.
                DirectoryInfo sourceinfo = new DirectoryInfo(new FileInfo(filePath).FullName);
                DirectoryInfo target = new DirectoryInfo(new FileInfo(Path.Combine(HttpContext.Current.Server.MapPath(AdminConstants.ThemeFolderPath), themeName)).FullName);
                Directory.Move(sourceinfo.FullName, target.FullName);
            }
        }

        //Move content of temporary folder to revision folder.
        private void MoveFolderContentToRevisionFolder(string fileName, string themeName)
        {
            //Get the path of CSS file.
            string filePath = Directory.Exists(Path.Combine(HttpContext.Current.Server.MapPath(AdminConstants.ThemeFolderPath), Path.GetFileNameWithoutExtension(fileName))) ?
                Path.Combine(HttpContext.Current.Server.MapPath(AdminConstants.ThemeFolderPath), Path.GetFileNameWithoutExtension(themeName)) : Path.Combine(HttpContext.Current.Server.MapPath(AdminConstants.ThemeFolderPath), Path.GetFileNameWithoutExtension(themeName));

            //Create a zip file to download.
            Ionic.Zip.ZipFile zip = new Ionic.Zip.ZipFile();
            zip.AddDirectory(filePath);
            zip.Save($"{ HttpContext.Current.Server.MapPath(AdminConstants.ThemeFolderPath)}/{themeName}.zip");

            //Dispose the zip file object.
            zip.Dispose();

            //Get the source file path and target file path and move the content from source to target.
            DirectoryInfo sourceinfo = new DirectoryInfo(new FileInfo($"{ HttpContext.Current.Server.MapPath(AdminConstants.ThemeFolderPath)}/{themeName}.zip").FullName);

            //Check specific theme already exists or not and return the file count.
            int fileCount = CheckFileExistsOrNot(themeName);
			ZnodeLogging.LogMessage("fileCount: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new { fileCount = fileCount });

			DirectoryInfo target = new DirectoryInfo(new FileInfo(Path.Combine(HttpContext.Current.Server.MapPath(AdminConstants.RevisionFolderPath), $"{ HttpContext.Current.Server.MapPath(AdminConstants.RevisionFolderPath)}/{themeName}{'_'}{fileCount + 1}.zip")).FullName);
            Directory.Move(sourceinfo.FullName, target.FullName);

            //Delete the directory of theme name.
            if (Directory.Exists(Path.Combine(HttpContext.Current.Server.MapPath(AdminConstants.ThemeFolderPath), themeName)))
                Directory.Delete(new FileInfo(Path.Combine(HttpContext.Current.Server.MapPath(AdminConstants.ThemeFolderPath), themeName)).FullName, true);
        }

        //Check specific theme already exists or not and return the file count.
        private int CheckFileExistsOrNot(string themeName)
        {
            DirectoryInfo hdDirectoryInWhichToSearch = new DirectoryInfo(HttpContext.Current.Server.MapPath(AdminConstants.RevisionFolderPath));
            FileSystemInfo[] filesAndDirs = hdDirectoryInWhichToSearch.GetFileSystemInfos("*" + themeName + "_" + "*");

            int fileCount = filesAndDirs.Length;
			ZnodeLogging.LogMessage("fileCount: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new { fileCount = fileCount });

			foreach (FileSystemInfo item in filesAndDirs)
            {
                //Check file already exists in revision folder or not.
                if (File.Exists(Path.Combine(HttpContext.Current.Server.MapPath(AdminConstants.RevisionFolderPath), $"{themeName}{'_'}{fileCount + 1}.zip")))
                    fileCount = fileCount + 1;
            }

            return fileCount;
        }

        //Saves the file.
        private ThemeViewModel SaveFile(ref ThemeViewModel themeViewModel, string fileName, string themeName)
		{
			ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
			List<CSSViewModel> oldCSSList;
            string[] result;

            //Check old CSS content from theme is exists in newly uploaded theme.
            CheckOldCSSExistInTheNewCSS(themeViewModel, fileName, out oldCSSList, out result);

            //if theme file is already uploaded, then replace the old CSS files with new CSS 
            if (result.Length <= 0)
            {
                foreach (CSSViewModel item in oldCSSList)
                    themeViewModel.CssList.RemoveAll(x => x.CSSName == item.CSSName.Replace(".css", ""));

                // Move old theme content to revision folder.
                MoveFolderContentToRevisionFolder(fileName, themeName);
            }
            //else theme is newly uploaded now, then create a folder for new CSS
            else
            {
                if (IsNotNull(fileName))
                {
                    //Extract the uploaded zip file.
                    ExtractThemeTemporarily(themeViewModel.FilePath);

                    //Get the list of CSS.
                    themeViewModel.CssList = GetListOfCssModel(fileName);
                }
            }
            //Update the theme and get its details.
            themeViewModel = _themeClient.UpdateTheme(themeViewModel?.ToModel<ThemeModel>())?.ToViewModel<ThemeViewModel>();

            //Move content of temporary folder to theme folder.
            if (themeViewModel?.CMSThemeId > 0)
                MoveFolderContent(fileName, themeViewModel.Name);
			ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
			return themeViewModel;
        }

        //Check revised theme CSS exists in Old theme CSS.
        private CSSListViewModel CheckRevisedThemeCssExistsInOldThemeCss(string themeName, ThemeViewModel themeViewModel, out string[] existingCssResult)
        {
            //Extract the revised theme to temporary theme folder.
            if (!string.IsNullOrEmpty(themeName) && Equals(Path.GetExtension(themeName), $".{AdminConstants.ZipFileType}"))
            {
                string revisionFolderPath = Path.Combine(HttpContext.Current.Server.MapPath(AdminConstants.RevisionFolderPath), themeName);
                string temporaryThemeFolderPath = Path.Combine(HttpContext.Current.Server.MapPath(AdminConstants.TemporaryThemeFolderPath), Path.GetFileNameWithoutExtension(themeName));
                ZipFile.ExtractToDirectory(revisionFolderPath, temporaryThemeFolderPath);
            }

            //Get the list of CSS.
            themeViewModel.CssList = GetListOfCssModel(themeName);

            CSSListViewModel cssListViewModel = new CSSListViewModel { CssList = _cssClient.GetCssListByThemeId(Convert.ToInt32(themeViewModel.CMSThemeId), null, null, null, null)?.CSSs?.ToViewModel<CSSViewModel>()?.ToList() };
            string revisedThemeCss = string.Join(",", themeViewModel.CssList.Select(x => x.CSSName.Replace(".css", "")));
            string actualThemeCss = string.Join(",", cssListViewModel.CssList.Select(x => x.CSSName.Replace(".css", "")));
			ZnodeLogging.LogMessage("revisedThemeCss and actualThemeCss: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new { revisedThemeCss = revisedThemeCss, actualThemeCss = actualThemeCss });

			//Compare both the CSS list (Revised theme CSS and old theme CSS)
			existingCssResult = actualThemeCss.Split(new char[] { ',' })
                            .Except(revisedThemeCss.Split(new char[] { ',' })).ToArray();

            return cssListViewModel;
        }
        #endregion
    }
}