using System;
using System.Collections.Generic;

namespace XAutomateMVC.Models.DBModels
{
    public partial class Role
    {
        public Role()
        {
            Login = new HashSet<Login>();
        }

        public long RoleId { get; set; }
        public string RoleName { get; set; }
        public string ScreenName { get; set; }
        public DateTime? Createdate { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }

        public virtual ICollection<Login> Login { get; set; }
    }
}
