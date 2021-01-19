using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel;
//using XAutomateMVC.Models.DBModels;

namespace XAutomateMVC.Models
{
    public class ErrorViewModel
    {
        public string RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
    public class ProductViewModel
    {
        [DisplayName("Product")]
        public string ProductId { get; set; }
        public List<SelectListItem> Listofproducts { get; set; }
        public string RulesId { get; set; }
        public List<SelectListItem> RuleList { get; set; }
        public List<SelectListItem> ExpectedResult { get; set; }

        public string ExpectedResultVal { get; set; }
        public string ProductSearchId { get; set; }

        public List<string> Multisuite { get; set; }
        public List<SelectListItem> ProductSearchList { get; set; }

        public List<string> TestCaseFile { get; set; }
        public List<SelectListItem> TestCaseFileList { get; set; }

        public List<SelectListItem> ResourceFile { get; set; }
        public string ResourceFileName { get; set; }
        public string  TestsuidId { get;  set; }
        public List<SelectListItem> TestSuiteList { get; set; }
        public string ReleaseId { get; set; }
        public List<SelectListItem> ReleaseList { get; set; }
    }
  

    public class RulesModel
    {
        [DisplayName("RulesId")]
        public string RulesId { get; set; }
        public List<SelectListItem> RuleName { get; set; }
    }
}
