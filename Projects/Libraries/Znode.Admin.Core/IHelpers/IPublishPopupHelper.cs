using System.Collections.Generic;
using System.Linq;
using Znode.Engine.Admin.Agents;
using Znode.Engine.Admin.Models;
using Znode.Engine.Admin.ViewModels;
using Znode.Engine.Api.Client;

namespace Znode.Engine.Admin.Helpers
{
    public interface IPublishPopupHelper
    {
        List<PopupInputFieldDataItem> GetAvailablePublishStates();

        List<PopupInputFieldDataItem> GetAvailablePublishStatesforPreview();
    }
}