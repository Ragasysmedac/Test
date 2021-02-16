using System;
using System.Collections.Generic;

namespace XAutomateMVC.Models.DBModels
{
    public partial class TestApproach
    {
        public TestApproach()
        {
            Rules = new HashSet<Rules>();
            TestCases = new HashSet<TestCases>();
        }

        public long TestApproachid { get; set; }
        public string TestApproachName { get; set; }
        public long? TestSuiteId { get; set; }
        public long? Dbconfigid { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string Connectionname { get; set; }
        public string SuiteIds { get; set; }

        public virtual DbConfig Dbconfig { get; set; }
        public virtual TestSuite TestSuite { get; set; }
        public virtual ICollection<Rules> Rules { get; set; }
        public virtual ICollection<TestCases> TestCases { get; set; }
    }
}
