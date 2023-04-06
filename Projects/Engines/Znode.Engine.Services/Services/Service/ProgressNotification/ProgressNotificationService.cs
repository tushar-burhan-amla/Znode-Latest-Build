using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Znode.Engine.Api.Models;
using Znode.Libraries.Data;
using Znode.Libraries.Data.DataModel;
using Znode.Libraries.Data.Helpers;
using Znode.Libraries.ECommerce.Utilities;


namespace Znode.Engine.Services
{
    public partial class ProgressNotificationService : BaseService, IProgressNotificationService
    {
        #region Private variables
        private readonly IZnodeRepository<ZnodePublishProgressNotifierEntity> _publishProgressNotifier;

        #endregion

        #region Constructor
        public ProgressNotificationService()
        {
            _publishProgressNotifier = new ZnodeRepository<ZnodePublishProgressNotifierEntity>(HelperMethods.Context);

        }
        #endregion

        #region Public Methods

        //Get all progress notification.
        public virtual async Task<List<ProgressNotificationModel>> GetProgressNotifications()
        {
            List<ProgressNotificationModel> result = new List<ProgressNotificationModel>();

            Task t = new Task(() =>
            {
                result = _publishProgressNotifier.Table?.ToModel<ProgressNotificationModel>()?.ToList();

                if (result?.Count(x => !x.IsCompleted && !x.IsFailed) == 0)
                {
                    List<ZnodePublishProgressNotifierEntity> completedRecords = _publishProgressNotifier.Table.Where(x => x.IsCompleted || x.IsFailed)?.ToList();
                    _publishProgressNotifier.Delete(completedRecords);
                }
            });

            t.Start();

            await t;

            return result;
        }

        #endregion      
    }
}
