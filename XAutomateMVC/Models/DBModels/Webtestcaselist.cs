using System;
using System.Collections.Generic;

namespace XAutomateMVC.Models.DBModels
{
    public partial class Webtestcaselist
    {
        public long WebtestcaselistId { get; set; }
        public long? WebTestcasesid { get; set; }
        public string Cases { get; set; }
        public string CaseType { get; set; }
        public string Status { get; set; }
        public string Description { get; set; }

        public virtual WebTestcases WebTestcases { get; set; }
    }
}
