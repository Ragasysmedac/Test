using System;
using System.Collections.Generic;

namespace XAutomateMVC.Models.DBModels
{
    public partial class DbConfig
    {
        public DbConfig()
        {
            Tablecolumn = new HashSet<Tablecolumn>();
            TestApproach = new HashSet<TestApproach>();
        }

        public long Dbconfigid { get; set; }
        public string DbName { get; set; }
        public string DbHostName { get; set; }
        public string DbPort { get; set; }
        public string DbUser { get; set; }
        public string DbPassword { get; set; }
        public string Status { get; set; }
        public string Description { get; set; }
        public string SuiteName { get; set; }
        public DateTime? CreateDate { get; set; }

        public virtual ICollection<Tablecolumn> Tablecolumn { get; set; }
        public virtual ICollection<TestApproach> TestApproach { get; set; }
    }
}
