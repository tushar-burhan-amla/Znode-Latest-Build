using Znode.Engine.Api.Models;
using Znode.Engine.Admin.Models;

namespace Znode.Engine.Admin.Maps
{
    public class ServerValidateViewModelMap
    {
        //Convert RoleViewModel model to RoleModel. 
        public static ValidateServerModel ToModel(BindDataModel roleViewModel)
        {
            if (!Equals(roleViewModel, null))
            {
                return new ValidateServerModel()
                {
                    ControlsData = roleViewModel.ControlsData
                };
            }
            else
                return null;
        }
        

        //Converts Role Model to Role View Model.
        public static ValidateServerModel ToViewModel(ValidateServerModel roleModel)
        {
            if (Equals(roleModel, null))
                return null;

            return new ValidateServerModel()
            {
                ControlsData = roleModel.ControlsData
            };
        }
    }
}