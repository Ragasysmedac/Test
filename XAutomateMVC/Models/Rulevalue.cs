using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace XAutomateMVC.Models
{
    public class Rulevalue
    {
        public long TestApproachid { get; set; }
        public string TestApproachName { get; set; }
        public long? TestSuiteId { get; set; }
        public long? Dbconfigid { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string Connectionname { get; set; }
        public string SuiteIds { get; set; }
    }
}
