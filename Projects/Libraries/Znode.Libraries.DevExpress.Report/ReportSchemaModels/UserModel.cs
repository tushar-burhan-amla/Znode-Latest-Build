using DevExpress.DataAccess.ObjectBinding;
using System.Collections.Generic;
using System.ComponentModel;

namespace Znode.Libraries.DevExpress.Report
{
    [DisplayName("Users")]
    [HighlightedClass]
    public class UserModel : List<PersonalInfo>
    {
        [HighlightedMember]
        public UserModel()
        {

        }

        public dynamic GetData()
        {
            List<PersonalInfo> _obj = new List<PersonalInfo>();

            _obj.Add(new PersonalInfo { Name = "sachin", LastName = "Manore", City = "Amravati" });
            _obj.Add(new PersonalInfo { Name = "Akash", LastName = "Mohite", City = "Nagpur" });
            _obj.Add(new PersonalInfo { Name = "Harshal", LastName = "Wanare", City = "Nagpur" });
            _obj.Add(new PersonalInfo { Name = "GB", LastName = "Banarase", City = "Nagpur" });
            _obj.Add(new PersonalInfo { Name = "Ishwar", LastName = "Padade", City = "Nagpur" });

            return _obj;
        }
    }
}