using System;
using System.Collections.Generic;

namespace XAutomateMVC.Models.DBModels
{
    public partial class Exceute
    {
        public long ExecuteId { get; set; }
        public string SuiteName { get; set; }
        public string ResultUrl { get; set; }
        public string Status { get; set; }
        public DateTime? Execute { get; set; }
        public string Time { get; set; }
        public string Executiontime { get; set; }
    }
}
