using System;
using System.Collections.Generic;

namespace XAutomateMVC.Models.DBModels
{
    public partial class Rules
    {
        public Rules()
        {
            RuleParamters = new HashSet<RuleParamters>();
            TestCases = new HashSet<TestCases>();
        }

        public long RulesId { get; set; }
        public string RuleName { get; set; }
        public string RuleCondtion { get; set; }
        public string RuleParameter { get; set; }
        public string Status { get; set; }
        public string TestApproachName { get; set; }
        public string Description { get; set; }
        public DateTime? CreateDate { get; set; }
        public long? TestApproachid { get; set; }
        public long? DbConfigId { get; set; }

        public virtual DbConfig DbConfig { get; set; }
        public virtual TestApproach TestApproach { get; set; }
        public virtual ICollection<RuleParamters> RuleParamters { get; set; }
        public virtual ICollection<TestCases> TestCases { get; set; }
    }
}
