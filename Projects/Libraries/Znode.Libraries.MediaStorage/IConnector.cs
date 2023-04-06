using System.Collections.Generic;
using System.IO;
using System.Web;
namespace Znode.Libraries.MediaStorage
{
    public interface IConnector
    {
        /// <summary>
        /// Upload Media on Specified server. This Methos help to upload any type of file on Server (AWS S3 , Azure, Local)
        /// </summary>
        /// <param name="stream">Media Stream</param>
        /// <param name="fileName">File Name </param>
        /// <param name="folderName">Name of folder to upload media </param>
        /// <returns>Url of uploaded media</returns>
        string Upload(MemoryStream stream, string fileName, string folderName);

        /// <summary>
        /// Upload List of Media on Specified server. This Methos help to upload any type of file on Server (AWS S3 , Azure, Local)
        /// </summary>
        /// <param name="files">Object List of HttpPostedFileBase</param>
        /// <returns>Returns Uploaded list of file path</returns>
        List<string> Upload(List<HttpPostedFileBase> files);

        /// <summary>
        /// Delete File from server. This method Delete file from server permanently. 
        /// </summary>
        /// <param name="fileName">Name of file</param>
        /// <param name="folderName">Folder name</param>
        /// <returns>Returns name deleted file</returns>
        List<string> Delete(string fileName, string folderName);

        /// <summary>
        ///Returns the base url where media is uploaded 
        /// </summary>
        /// <returns>base url where media is uploaded</returns>
        string GetServerUrl();

        /// <summary>
        /// Copy media to local storage.
        /// </summary>
        /// <returns></returns>
        Dictionary<string, long> Copy(string fileName, string folderName);
        
    }
}
