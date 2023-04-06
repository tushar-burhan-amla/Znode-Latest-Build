using System;

namespace Znode.Engine.Api.Models
{
    public class LoginUserModel : BaseModel
    {
        public string Comment { get; set; }

        public string Email { get; set; }
        public string NewPassword { get; set; }
        public string Password { get; set; }
        public string ProviderName { get; set; }
        public string Username { get; set; }
        public string PasswordToken { get; set; }
        public string RoleName { get; set; }

        public bool IsApproved { get; set; }
        public bool IsLockedOut { get; set; }
        public bool IsOnline { get; set; }
        public bool RememberMe { get; set; }
        public bool IsConfirmed { get; set; }

        public DateTime CreateDate { get; set; }
        public DateTime LastActivityDate { get; set; }
        public DateTime LastLockoutDate { get; set; }
        public DateTime LastLoginDate { get; set; }
        public DateTime LastPasswordChangedDate { get; set; }

        public string UserId { get; set; }

        public string RoleId { get; set; }

    }
}
