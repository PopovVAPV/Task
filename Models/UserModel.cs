using System;

namespace Task.Models
{
    public class UserModel
    {
        public int Id { get; set; }
        public string Login { get; set; }
        public string PasswordHash { get; set; }
        public string Role { get; set; }
        public bool IsBlocked { get; set; }
        public int FailedLoginAttempts { get; set; }
        public DateTime? LastLoginDate { get; set; }
        public DateTime CreationDate { get; set; }
        public bool NeedPasswordChange { get; set; }
    }
} 