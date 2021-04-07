using System;
using System.Collections.Generic;

namespace XAutomateMVC.Models.DBModels
{
    public partial class KpiRuns
    {
        public long KpiRunsid { get; set; }
        public long? KpiTotalid { get; set; }
        public DateTime? ExecuteDate { get; set; }
        public int? RunTestcase { get; set; }
        public int? Passed { get; set; }
        public int? Failed { get; set; }
        public string PercentageRatio { get; set; }

        public virtual KpiTotal KpiTotal { get; set; }
    }
}
