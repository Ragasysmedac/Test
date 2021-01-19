using System;
using System.Collections.Generic;

namespace XAutomateMVC.Models.DBModels
{
    public partial class Login
    {
        public long LoginId { get; set; }
        public long? RoleId { get; set; }
        public string EmployeeNo { get; set; }
        public string EmployeeName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string EmailId { get; set; }
        public string Role { get; set; }
        public string Description { get; set; }
        public long? Status { get; set; }

        public virtual Role RoleNavigation { get; set; }
    }
}
