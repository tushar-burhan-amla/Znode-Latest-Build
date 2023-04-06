using System;
using System.IO;
using System.Web;

namespace Znode.Libraries.ECommerce.Utilities
{
    public static class RelationalThemeHelper
    {
        /// <summary>
        /// Check if a CSS file is found in the supplied theme folder.
        /// </summary>
        /// <param name="themeRootPath">Path to the folder which contains all the theme folders.</param>
        /// <param name="themeName">Name of the theme folder to look into.</param>
        /// <param name="fileName">Name of the file including file extension.</param>
        /// <param name="intermediateFolderPath">Intermediate directory path which leads from theem root folder to the location where the file is supposed to be found.</param>
        /// <returns>Returns true if the file can be found.</returns>
        public static bool ThemeFileExists(string themeRootPath, string themeName, string relativeFilePath)
            => File.Exists(Path.Combine(HttpContext.Current.Server.MapPath(themeRootPath + themeName + relativeFilePath)));

        /// <summary>
        /// Check if the theme directory exists.
        /// </summary>
        /// <param name="themeRootPath">Path to the folder which contains all the theme folders.</param>
        /// <param name="themeName">Name of the theme folder.</param>
        /// <returns>Returns true if the directory can be found.</returns>
        public static bool ThemeDirectoryExists(string themeRootPath, string themeName)
            => (string.IsNullOrEmpty(themeRootPath) || string.IsNullOrEmpty(themeName)) ? false : Directory.Exists(Path.Combine(HttpContext.Current.Server.MapPath(themeRootPath), themeName));
    }
}
