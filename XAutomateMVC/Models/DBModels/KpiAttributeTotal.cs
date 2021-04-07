using System;
using System.Collections.Generic;

namespace XAutomateMVC.Models.DBModels
{
    public partial class KpiAttributeTotal
    {
        public long KpiAttributeTotalid { get; set; }
        public int? TotalAttributes { get; set; }
        public int? UsedAttribute { get; set; }
        public int? UniqueAttributes { get; set; }
        public int? RemianingAttributes { get; set; }

        public DateTime? enterdatetime { get; set; }

        public string Suitename { get; set; }
    }
}
