using System;
using System.Collections.Generic;

namespace XAutomateMVC.Models.DBModels
{
    public partial class TestCaseParameters
    {
        public long TestCaseParametersId { get; set; }
        public string ParameterName { get; set; }
        public string Status { get; set; }
        public long? TestCasesId { get; set; }

        public virtual TestCases TestCases { get; set; }
    }
}
