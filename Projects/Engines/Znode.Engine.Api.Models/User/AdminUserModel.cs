namespace Znode.Engine.Api.Models
{
    public class AdminUserModel
    {
        public int LoggedUserAccountId { get; set; }
        public string UserName { get; set; }
        public string RoleName { get; set; }
        public string ColumnList { get; set; }
    }
}
