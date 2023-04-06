using System.Collections.Generic;
using System.Linq;
using Znode.Engine.Admin.Agents;
using Znode.Engine.Admin.Models;
using Znode.Engine.Admin.ViewModels;
using Znode.Engine.Api.Client;

namespace Znode.Engine.Admin.Helpers
{
    public class PublishPopupHelper: IPublishPopupHelper
    {
        public virtual List<PopupInputFieldDataItem> GetAvailablePublishStates()
        {
            IGeneralSettingAgent _generalSettingAgent = new GeneralSettingAgent(new GeneralSettingClient());

            IEnumerable<PublishStateMappingViewModel> mappingList = _generalSettingAgent.GetAvailablePublishStateMappings();

            string defaultOptionText = string.Join(" & ", mappingList.OrderBy(s => s.IsDefault).Select(s => s.PublishState));

            return mappingList?.Select(x => new PopupInputFieldDataItem
            {
                DisplayName = !x.IsDefault ? string.Format("{0} {1}", x.PublishState, "Only") : defaultOptionText,
                HelpText = x.Description,
                Value = x.PublishStateCode,
                IsChecked = x.IsDefault,
                Disabled = false
            })?.ToList();
        }

        public virtual List<PopupInputFieldDataItem> GetAvailablePublishStatesforPreview()
        {
            IGeneralSettingAgent _generalSettingAgent = new GeneralSettingAgent(new GeneralSettingClient());

            IEnumerable<PublishStateMappingViewModel> mappingList = _generalSettingAgent.GetAvailablePublishStatesforPreview();

            string defaultOptionText = string.Join(" & ", mappingList.OrderBy(s => s.IsDefault).Select(s => s.PublishState));

            return mappingList?.Select(x => new PopupInputFieldDataItem
            {
                DisplayName = !x.IsDefault ? string.Format("{0} {1}", x.PublishState, "Only") : defaultOptionText,
                HelpText = x.Description,
                Value = x.PublishStateCode,
                IsChecked = x.IsDefault,
                IsEnabled = x.IsEnabled,
                Disabled = false,
                ApplicationType = x.ApplicationType
            })?.ToList();
        }
    }
}