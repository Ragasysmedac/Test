using System;
using System.Collections.Generic;

namespace XAutomateMVC.Models.DBModels
{
    public partial class WebTestcases
    {
        public WebTestcases()
        {
            Webtestcaselist = new HashSet<Webtestcaselist>();
        }

        public long WebTestcasesid { get; set; }
        public long? WebFilesId { get; set; }
        public string Webtestcase { get; set; }
        public string Testcase { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }

        public virtual WebFiles WebFiles { get; set; }
        public virtual ICollection<Webtestcaselist> Webtestcaselist { get; set; }
    }
}
