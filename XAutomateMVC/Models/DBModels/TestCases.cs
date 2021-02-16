using System;
using System.Collections.Generic;

namespace XAutomateMVC.Models.DBModels
{
    public partial class TestCases
    {
        public TestCases()
        {
            TestCaseParameters = new HashSet<TestCaseParameters>();
        }

        public long TestCasesId { get; set; }
        public string TestCaseName { get; set; }
        public string TestcaseTitle { get; set; }
        public string Testdata { get; set; }
        public string ExceptedResult { get; set; }
        public string Status { get; set; }
        public string Description { get; set; }
        public DateTime? CreatedDate { get; set; }
        public long? TestApproachid { get; set; }
        public long? RulesId { get; set; }
        public string Expextedparameter { get; set; }

        public virtual Rules Rules { get; set; }
        public virtual TestApproach TestApproach { get; set; }
        public virtual ICollection<TestCaseParameters> TestCaseParameters { get; set; }
    }
}
