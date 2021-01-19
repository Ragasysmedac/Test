using System;
using System.Collections.Generic;

namespace XAutomateMVC.Models.DBModels
{
    public partial class RuleParamters
    {
        public long RuleParameterId { get; set; }
        public string ParameterName { get; set; }
        public long? RulesId { get; set; }
        public string Status { get; set; }
        public DateTime? CreatedDate { get; set; }

        public virtual Rules Rules { get; set; }
    }
}
