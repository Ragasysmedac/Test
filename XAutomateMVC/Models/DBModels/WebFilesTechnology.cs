using System;
using System.Collections.Generic;

namespace XAutomateMVC.Models.DBModels
{
    public partial class WebFilesTechnology
    {
        public WebFilesTechnology()
        {
            WebFiles = new HashSet<WebFiles>();
        }

        public long WebFilesTechnologyid { get; set; }
        public string WebfileTechnologyFoler { get; set; }
        public string Technology { get; set; }
        public long? Status { get; set; }

        public virtual ICollection<WebFiles> WebFiles { get; set; }
    }
}
