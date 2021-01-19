using System;
using System.Collections.Generic;

namespace XAutomateMVC.Models.DBModels
{
    public partial class TokenValue
    {
        public long TokenId { get; set; }
        public string TockenId { get; set; }
        public DateTime? Validto { get; set; }
    }
}
