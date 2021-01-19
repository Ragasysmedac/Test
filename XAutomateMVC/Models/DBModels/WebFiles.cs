using System;
using System.Collections.Generic;

namespace XAutomateMVC.Models.DBModels
{
    public partial class WebFiles
    {
        public WebFiles()
        {
            WebTestcases = new HashSet<WebTestcases>();
        }

        public long WebFilesId { get; set; }
        public string WebFilesPath { get; set; }
        public string FileType { get; set; }
        public string FileName { get; set; }
        public string Status { get; set; }
        public string Description { get; set; }

        public virtual ICollection<WebTestcases> WebTestcases { get; set; }
    }
}
