using System;
using System.Collections.Generic;

namespace XAutomateMVC.Models.DBModels
{
    public partial class ExpectedResult
    {
        public long ExpectedResult1 { get; set; }
        public string Expectedresultname { get; set; }
        public int? Parameter { get; set; }
        public long? Status { get; set; }
    }
}
