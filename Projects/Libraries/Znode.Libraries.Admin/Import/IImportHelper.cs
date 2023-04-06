using Znode.Engine.Api.Models;

namespace Znode.Libraries.Admin.Import
{
    public interface IImportHelper
    {

        #region Public Methods
        //This method will process the data uploaded from the file.
        int ProcessData(ImportModel model, int userId);

        //Update the data of ZnodeImportHead table
        int UpdateTemplateData(ImportModel model);

        //check the import status
        bool CheckImportStatus();

        //This method will process the data uploaded from the file.
        int ProcessProductUpdateData(ImportModel model, int userId, out string importedGuid);
        #endregion
    }
}
