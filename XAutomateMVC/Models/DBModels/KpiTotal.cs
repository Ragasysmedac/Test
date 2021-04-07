using System;
using System.Collections.Generic;

namespace XAutomateMVC.Models.DBModels
{
    public partial class KpiTotal
    {
        public KpiTotal()
        {
            KpiRuns = new HashSet<KpiRuns>();
        }

        public long KpiTotalid { get; set; }
        public long? TestSuiteId { get; set; }
        public string Suitename { get; set; }
        public DateTime? LastupdatedDate { get; set; }
        public int? TotalTestcase { get; set; }

        public virtual TestSuite TestSuite { get; set; }
        public virtual ICollection<KpiRuns> KpiRuns { get; set; }
    }
}
