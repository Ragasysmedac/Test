using System;
using System.Collections.Generic;

namespace XAutomateMVC.Models.DBModels
{
    public partial class TestSuite
    {
        public TestSuite()
        {
            TestApproach = new HashSet<TestApproach>();
        }

        public long TestSuiteId { get; set; }
        public string TestSuitename { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public DateTime? CreatedDate { get; set; }

        public virtual ICollection<TestApproach> TestApproach { get; set; }
    }
}
