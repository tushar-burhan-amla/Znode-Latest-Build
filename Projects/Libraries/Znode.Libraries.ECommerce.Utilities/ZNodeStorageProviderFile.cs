using System;
using System.Configuration.Provider;
using System.Diagnostics;
using System.IO;
using System.Web;
using Znode.Libraries.Framework.Business;

namespace Znode.Libraries.ECommerce.Utilities
{
    public class ZnodeStorageProviderFile : ZnodeStorageProviderBase
    {
        #region Public Methods

        // Initialize the Provider base. A collection of the name/value pairs representing the provider-specific attributes specified in the configuration for this provider.
        public override void Initialize(string name, System.Collections.Specialized.NameValueCollection config)
        {
            if ((Equals(config, null)) || (Equals(config.Count, 0)))
            {
                throw new ArgumentNullException("You must supply a valid configuration dictionary.");
            }

            if (string.IsNullOrEmpty(config["description"]))
            {
                config.Remove("description");
                config.Add("description", "Default File storage provider ");
            }

            // Let ProviderBase perform the basic initialization
            base.Initialize(name, config);

            // Check to see if any attributes were set in configuration that does not need for file writing.
            if (config.Count > 0)
            {
                string extraAttribute = config.GetKey(0);
                if (!String.IsNullOrEmpty(extraAttribute))
                {
                    throw new ProviderException($"The following unrecognized attribute was found in {Name}'s configuration:' {extraAttribute} '");
                }
                else
                {
                    throw new ProviderException("An unrecognized attribute was found in the provider's configuration.");
                }
            }
        }

        // Reads binary file (such as Images) from persistant storage and Returns the binary content of the file.
        public override byte[] ReadBinaryStorage(string filePath)
        {
            try
            {
                return File.ReadAllBytes(HttpContext.Current.Server.MapPath(filePath));
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
                throw;
            }
        }

        // Writes binary file (such as Images) to persistant storage.
        public override void WriteBinaryStorage(byte[] fileData, string filePath)
        {
            try
            {
                // Create directory if not exists.
                FileInfo fileInfo = new FileInfo(HttpContext.Current.Server.MapPath(filePath));
                if (!fileInfo.Directory.Exists)
                    fileInfo.Directory.Create();

                // Write the binary content.
                File.WriteAllBytes(HttpContext.Current.Server.MapPath(filePath), fileData);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
                throw;
            }
        }

        /// Reads text file from persistant storage and Returns the text content of the file.
        public override string ReadTextStorage(string filePath)
        {
            try
            {
                return File.ReadAllText(HttpContext.Current.Server.MapPath(filePath));
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
                throw;
            }
        }

        // Writes text file to persistant storage.
        public override void WriteTextStorage(string fileData, string filePath, Mode fileMode)
        {
            try
            {
                // Create directory if not exists.
                FileInfo fileInfo = new FileInfo(HttpContext.Current.Server.MapPath(filePath));
                if (!fileInfo.Directory.Exists)
                {
                    fileInfo.Directory.Create();
                }

                // Check file write mode and write content.
                if (Equals(fileMode, Mode.Append))
                {
                    File.AppendAllText(HttpContext.Current.Server.MapPath(filePath), fileData);
                }
                else
                {
                    File.WriteAllText(HttpContext.Current.Server.MapPath(filePath), fileData);
                }
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
                throw;
            }
        }

        // Deletes the file from persistant storage.

        public override bool DeleteStorage(string filePath)
        {
            try
            {
                if (File.Exists(HttpContext.Current.Server.MapPath(filePath)))
                {
                    File.Delete(HttpContext.Current.Server.MapPath(filePath));
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
                return false;
            }
        }

        // Check file exists in persistant storage and Returns boolean indicating the file exists (TRUE) or not (FALSE).

        public override bool Exists(string filePath)
        {
            try
            {
                return File.Exists(HttpContext.Current.Server.MapPath(filePath));
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
                throw;
            }
        }

        /// Get HttpPath of file from file storage and Returns HttpPath of file.
        public override string HttpPath(string filePath) => filePath;

        #endregion
    }
}
