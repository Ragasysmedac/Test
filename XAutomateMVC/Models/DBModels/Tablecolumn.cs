using System;
using System.Collections.Generic;

namespace XAutomateMVC.Models.DBModels
{
    public partial class Tablecolumn
    {
        public long TablecolumnId { get; set; }
        public string Tablecolumn1 { get; set; }
        public string FieldName { get; set; }
        public string Active { get; set; }
        public long? Dbconfigid { get; set; }

        public virtual DbConfig Dbconfig { get; set; }
    }
}
