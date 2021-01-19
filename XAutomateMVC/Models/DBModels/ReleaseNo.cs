using System;
using System.Collections.Generic;

namespace XAutomateMVC.Models.DBModels
{
    public partial class ReleaseNo
    {
        public long ReleasenoId { get; set; }
        public string ReleaseNo1 { get; set; }
        public string ReleaseName { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}
