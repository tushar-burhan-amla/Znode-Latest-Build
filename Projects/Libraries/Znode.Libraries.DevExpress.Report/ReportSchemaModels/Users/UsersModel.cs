using DevExpress.DataAccess.ObjectBinding;
using System.Collections.Generic;
using System.ComponentModel;

namespace Znode.Libraries.DevExpress.Report
{
    [DisplayName("Users")]
    [HighlightedClass]
    public class UsersModel : List<UsersInfo>
    {
        [HighlightedMember]
        public UsersModel()
        {

        }
    }
}
