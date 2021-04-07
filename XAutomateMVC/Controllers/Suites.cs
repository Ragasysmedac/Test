using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using log4net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using TimeZoneConverter;
using XAutomateMVC.Models;
using XAutomateMVC.Models.DBModels;

namespace XAutomateMVC.Controllers
{
    public class Suites : Controller
    {
        static readonly ILog _log4net = LogManager.GetLogger(typeof(Suites));
        db_mateContext db = new db_mateContext();

        private IConfiguration configuration;

        public Suites(IConfiguration iConfig)
        {
            configuration = iConfig;
        }
        public IActionResult Suite()
        {
            try
            {
                ViewBag.Report =configuration.GetValue<string>("ReportPortalIp");

                var SuiteList = (from product in db.DbConfig
                                 where product.Status == "1"
                                 select new SelectListItem()
                                 {
                                     Text = product.DbName,
                                     Value = product.Dbconfigid.ToString(),
                                 }).ToList();

                SuiteList.Insert(0, new SelectListItem()
                {
                    Text = "----Select----",
                    Value = string.Empty
                });

                var TestSuite = (from Suite in db.TestSuite
                                 where Suite.Status == "1"
                                 select new SelectListItem()
                                 {
                                     Text = Suite.TestSuitename,
                                     Value = Suite.TestSuitename.ToString(),
                                 }).ToList();

                //TestSuite.Insert(0, new SelectListItem()
                //{
                //    Text = "Select Test Suite",
                //    Value = string.Empty
                //});
                ProductViewModel productViewModel = new ProductViewModel();
                productViewModel.Listofproducts = SuiteList;
                productViewModel.TestSuiteList = TestSuite;
                _log4net.Info("Function Name :  Suite Suite Loaded Successfully ");
                return View(productViewModel);
            }
            catch(Exception ex)
            {
                _log4net.Error("Function Name :  Suite Suite  -- " + ex.ToString());
                return null;
            }
           

        }

        public string UpdateTestApproach(int testId, string SName, string testapproachname, string Active)
        {
            try
            {
                var header = this.Request.Headers.ToString();
                var header1 = this.Request.Headers.ToList();
                var Auth = (string)this.Request.Headers["Authorization"];
                if (Auth != "" && Auth != null && Auth != "max-age=0")
                {
                    TimeZoneInfo timeZoneInfo = TZConvert.GetTimeZoneInfo("Europe/Stockholm");
                    //TimeZoneInfo timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
                    // TimeZoneInfo timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Etc/UTC");
                    DateTime dtCNow = TimeZoneInfo.ConvertTime(Convert.ToDateTime(DateTime.Now), timeZoneInfo);
                    string dtCNowdate = Convert.ToDateTime(dtCNow).ToString("yyyy-MM-dd");
                    TimeSpan tsnow = Convert.ToDateTime(dtCNow).TimeOfDay;
                    DateTime currentdatetime = Convert.ToDateTime(dtCNowdate + " " + (tsnow.Hours) + ":" + tsnow.Minutes);
                    var resultToken = db.TokenValue.FirstOrDefault(b => b.TockenId == Auth && b.Validto >= currentdatetime);
                    if (resultToken != null)
                    {
                        var result = db.TestApproach.FirstOrDefault(c => c.TestApproachid == testId);
                        if (result != null)
                        {
                            if (Active == "0")
                            {
                                var approach = db.Rules.FirstOrDefault(x => x.TestApproachid == result.TestApproachid && x.Status == "1");
                                if (approach == null)
                                {
                                    result.SuiteIds = SName;
                                    result.TestApproachName = testapproachname;
                                    // result.Connectionname = Convert.ToString(DbName);
                                    //result.Dbconfigid = Convert.ToInt32(DbName);
                                    result.Status = Active;
                                    db.SaveChanges();
                                }
                                else
                                {
                                    _log4net.Error("Function Name :  Test Approach UpdateTestApproach send valid file");
                                    return "Validate";
                                }

                            }
                            else
                            {
                                result.SuiteIds = SName;
                                result.TestApproachName = testapproachname;
                                //  result.Connectionname = Convert.ToString(DbName);
                                // result.Dbconfigid = Convert.ToInt32(DbName);
                                result.Status = Active;
                                db.SaveChanges();
                            }



                        }
                        _log4net.Error("Function Name :  Test Approach UpdateTestApproach Success fully loaded");
                        return "Success";
                    }
                    else
                    {
                        _log4net.Error("Function Name :  Test Approach UpdateTestApproach Auth Failed ");
                        return "Auth Fail";
                    }
                }
                else
                {
                    _log4net.Error("Function Name :  Test Approach UpdateTestApproach Auth Failed ");
                    return "Auth Fail";
                }
            }
            catch(Exception ex)
            {
                _log4net.Error("Function Name :  Test Approach UpdateTestApproach  -- " + ex.ToString());
                return null;
            }
          
        }

        public JsonResult SuiteChange(string suitename)
        {
            try
            {
                List<Rules> ca = new List<Rules>();
                var a = from b in db.Rules
                        where b.TestApproachName == suitename
                        select new
                        {
                            b.RuleName,
                            b.RuleParameter,

                        };
                _log4net.Error("Function Name :  Suite SuiteChange loaded Successfully ");
                return Json(ca);
            }
            catch(Exception ex)
            {
                _log4net.Error("Function Name :  Suite SuiteChange  -- " + ex.ToString());
                return null;
            }
         
        }


        public string SaveTestSuite(string SName, string Active,int Execpttest)
        {
            try
            {
                var header = this.Request.Headers.ToString();
                var header1 = this.Request.Headers.ToList();
                var Auth = (string)this.Request.Headers["Authorization"];
                if (Auth != "" && Auth != null && Auth != "max-age=0")
                {
                    TimeZoneInfo timeZoneInfo = TZConvert.GetTimeZoneInfo("Europe/Stockholm");
                    //  TimeZoneInfo timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Etc/UTC");
                    DateTime dtCNow = TimeZoneInfo.ConvertTime(Convert.ToDateTime(DateTime.Now), timeZoneInfo);
                    string dtCNowdate = Convert.ToDateTime(dtCNow).ToString("yyyy-MM-dd");
                    TimeSpan tsnow = Convert.ToDateTime(dtCNow).TimeOfDay;
                    DateTime currentdatetime = Convert.ToDateTime(dtCNowdate + " " + (tsnow.Hours) + ":" + tsnow.Minutes);
                    var resultToken = db.TokenValue.FirstOrDefault(b => b.TockenId == Auth && b.Validto >= currentdatetime);
                    if (resultToken != null)
                    {
                        var check = db.TestSuite.FirstOrDefault(x => x.TestSuitename == SName && x.Status == Active);
                        if (check == null)
                        {
                            TestSuite Ins = new TestSuite();
                            Ins.TestSuitename = SName;
                            Ins.Status = Active;
                            Ins.Excepttestcase = Execpttest;
                            Ins.CreatedDate = DateTime.Now;
                            db.TestSuite.Add(Ins);
                            db.SaveChanges();

                            KpiTotal ins1 = new KpiTotal();
                            ins1.TestSuiteId = Ins.TestSuiteId;
                            ins1.LastupdatedDate = DateTime.Now;
                            ins1.Suitename = SName;
                            db.KpiTotal.Add(ins1);
                            db.SaveChanges();
                            _log4net.Error("Function Name :  Suite SaveTestSuite Inserted Successfully");
                            return "Success";
                        }
                        else
                        {
                            _log4net.Error("Function Name :  Suite SaveTestSuite  Failed");
                            return "Fail";
                        }
                    }
                    else
                    {
                        _log4net.Error("Function Name :  Suite SaveTestSuite Auth Failed");
                        return "Auth Fail";
                    }
                }
                else
                {
                    _log4net.Error("Function Name :  Suite SaveTestSuite Auth Failed");
                    return "Auth Fail";
                }
            }
            catch(Exception ex)
            {
                _log4net.Error("Function Name :  Suite SaveTestSuite  -- " + ex.ToString());
                return null;
            }
           
        }

        public string UpdateTestSuite(int testId, string SName, string Active,int Execpttest)
        {
            try
            {
                var header = this.Request.Headers.ToString();
                var header1 = this.Request.Headers.ToList();
                var Auth = (string)this.Request.Headers["Authorization"];
                if (Auth != "" && Auth != null && Auth != "max-age=0")
                {
                    TimeZoneInfo timeZoneInfo = TZConvert.GetTimeZoneInfo("Europe/Stockholm");
                    //   TimeZoneInfo timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Etc/UTC");
                    DateTime dtCNow = TimeZoneInfo.ConvertTime(Convert.ToDateTime(DateTime.Now), timeZoneInfo);
                    string dtCNowdate = Convert.ToDateTime(dtCNow).ToString("yyyy-MM-dd");
                    TimeSpan tsnow = Convert.ToDateTime(dtCNow).TimeOfDay;
                    DateTime currentdatetime = Convert.ToDateTime(dtCNowdate + " " + (tsnow.Hours) + ":" + tsnow.Minutes);
                    var resultToken = db.TokenValue.FirstOrDefault(b => b.TockenId == Auth && b.Validto >= currentdatetime);
                    if (resultToken != null)
                    {
                        var result = db.TestSuite.FirstOrDefault(x => x.TestSuiteId == testId);

                        if (result != null)
                        {
                            if (Active == "0")
                            {
                                var approach = db.TestApproach.FirstOrDefault(x => x.TestSuiteId == result.TestSuiteId && x.Status == "1");
                                if (approach == null)
                                {
                                    result.TestSuitename = SName;
                                    result.Status = Active;
                                    result.Excepttestcase = Execpttest;
                                    db.SaveChanges();
                                }
                                else
                                {
                                    _log4net.Debug("Function Name :  Suite UpdateTestSuite Validate ");
                                    return "Validate";
                                }
                            }
                            else
                            {
                                result.TestSuitename = SName;
                                result.Status = Active;
                                result.Excepttestcase = Execpttest;
                                db.SaveChanges();
                            }


                        }
                        _log4net.Info("Function Name :  Suite UpdateTestSuite Update Successfully ");
                        return "Success";
                    }
                    else
                    {
                        _log4net.Error("Function Name :  Suite UpdateTestSuite Auth Failed ");
                        return "Auth Fail";
                    }
                }
                else
                {
                    _log4net.Error("Function Name :  Suite UpdateTestSuite Auth Failed ");
                    return "Auth Fail";
                }
            }
            catch(Exception ex)
            {
                _log4net.Error("Function Name :  Suite UpdateTestSuite  -- " + ex.ToString());
                return null;
            }
           
        }

        public JsonResult TestSuitegrid(string status)
        {
            try
            {
                var header = this.Request.Headers.ToString();
                var header1 = this.Request.Headers.ToList();
                var Auth = (string)this.Request.Headers["Authorization"];
                if (Auth != "" && Auth != null && Auth != "max-age=0")
                {
                    TimeZoneInfo timeZoneInfo = TZConvert.GetTimeZoneInfo("Europe/Stockholm");
                    // TimeZoneInfo timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
                    //  TimeZoneInfo timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Etc/UTC");
                    DateTime dtCNow = TimeZoneInfo.ConvertTime(Convert.ToDateTime(DateTime.Now), timeZoneInfo);
                    string dtCNowdate = Convert.ToDateTime(dtCNow).ToString("yyyy-MM-dd");
                    TimeSpan tsnow = Convert.ToDateTime(dtCNow).TimeOfDay;
                    DateTime currentdatetime = Convert.ToDateTime(dtCNowdate + " " + (tsnow.Hours) + ":" + tsnow.Minutes);
                    var resultToken = db.TokenValue.FirstOrDefault(b => b.TockenId == Auth && b.Validto >= currentdatetime);
                    if (resultToken != null)
                    {
                        var a = from b in db.TestSuite
                              //  where b.Status == status
                              orderby b.Status descending
                                select new
                                {
                                    b.TestSuitename,
                                    b.CreatedDate,
                                    b.TestSuiteId,
                                    b.Excepttestcase,
                                    STATUS = b.Status == "1" ? "Active" : "Inactive",
                                };
                        _log4net.Error("Function Name :  Suite TestSuitegrid Grid bind successfully");
                        return Json(a);
                    }
                    else
                    {
                        _log4net.Error("Function Name :  Suite TestSuitegrid Auth Failed ");
                        return Json("Auth Fail");
                    }
                }
                else
                {
                    _log4net.Error("Function Name :  Suite TestSuitegrid Auth Failed ");
                    return Json("Auth Fail");
                }
            }
            catch(Exception ex)
            {
                _log4net.Error("Function Name :  Suite TestSuitegrid  -- " + ex.ToString());
                return null;
            }
         

        }
        public JsonResult Editvalue(int SuiteId)
        {
            try
            {
                var header = this.Request.Headers.ToString();
                var header1 = this.Request.Headers.ToList();
                var Auth = (string)this.Request.Headers["Authorization"];
                if (Auth != "" && Auth != null && Auth != "max-age=0")
                {
                    TimeZoneInfo timeZoneInfo = TZConvert.GetTimeZoneInfo("Europe/Stockholm");
                    //  TimeZoneInfo timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Etc/UTC");
                    DateTime dtCNow = TimeZoneInfo.ConvertTime(Convert.ToDateTime(DateTime.Now), timeZoneInfo);
                    string dtCNowdate = Convert.ToDateTime(dtCNow).ToString("yyyy-MM-dd");
                    TimeSpan tsnow = Convert.ToDateTime(dtCNow).TimeOfDay;
                    DateTime currentdatetime = Convert.ToDateTime(dtCNowdate + " " + (tsnow.Hours) + ":" + tsnow.Minutes);
                    var resultToken = db.TokenValue.FirstOrDefault(b => b.TockenId == Auth && b.Validto >= currentdatetime);
                    if (resultToken != null)
                    {
                        var a = from b in db.TestSuite
                                where b.TestSuiteId == SuiteId
                                select new
                                {
                                    b.TestSuitename,
                                    b.Status,
                                    b.TestSuiteId,
                                    b.Excepttestcase
                                };
                        _log4net.Error("Function Name :  Suite Editvalue loaded Successfully");
                        return Json(a);
                    }
                    else
                    {
                        _log4net.Error("Function Name :  Suite Editvalue Auth Failed");
                        return Json("Auth Fail");
                    }
                }
                else
                {
                    _log4net.Error("Function Name :  Suite Editvalue Auth Failed");
                    return Json("Auth Fail");
                }
            }
            catch(Exception ex)
            {
                _log4net.Error("Function Name :  Suite Editvalue  -- " + ex.ToString());
                return null;
            }
        
        }

        public JsonResult SuiteBidGrid(string Active)
        {
            try
            {
                var header = this.Request.Headers.ToString();
                var header1 = this.Request.Headers.ToList();
                var Auth = (string)this.Request.Headers["Authorization"];
                if (Auth != "" && Auth != null && Auth != "max-age=0")
                {
                    TimeZoneInfo timeZoneInfo = TZConvert.GetTimeZoneInfo("Europe/Stockholm");
                    // TimeZoneInfo timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
                    // TimeZoneInfo timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Etc/UTC");
                    DateTime dtCNow = TimeZoneInfo.ConvertTime(Convert.ToDateTime(DateTime.Now), timeZoneInfo);
                    string dtCNowdate = Convert.ToDateTime(dtCNow).ToString("yyyy-MM-dd");
                    TimeSpan tsnow = Convert.ToDateTime(dtCNow).TimeOfDay;
                    DateTime currentdatetime = Convert.ToDateTime(dtCNowdate + " " + (tsnow.Hours) + ":" + tsnow.Minutes);
                    var resultToken = db.TokenValue.FirstOrDefault(b => b.TockenId == Auth && b.Validto >= currentdatetime);
                    if (resultToken != null)
                    {
                        List<Rules> ca = new List<Rules>();
                        var a = from b in db.TestApproach
                                    //  join c in db.TestSuite on b.TestSuiteId equals c.TestSuiteId
                               // where b.Status == Active
                               orderby b.Status descending
                                select new
                                {
                                    //c.TestSuitename,
                                    TestSuitename = (b.SuiteIds == null ? "" : b.SuiteIds),
                                    b.CreatedDate,
                                    status = b.Status == "1" ? "Active" : "Inactive",
                                   // Connectionname = (db.DbConfig.FirstOrDefault(x => x.Dbconfigid == Convert.ToInt32(b.Connectionname)).DbName != null ? db.DbConfig.FirstOrDefault(x => x.Dbconfigid == Convert.ToInt32(b.Connectionname)).DbName : ""),
                                    b.TestApproachName,
                                    b.TestApproachid,


                                };
                        _log4net.Error("Function Name :  Suite SuiteBidGrid Auth Failed");
                        return Json(a);
                    }
                    else
                    {
                        _log4net.Error("Function Name :  Suite SuiteBidGrid Auth Failed");
                        return Json("Auth Fail");
                    }
                }
                else
                {
                    _log4net.Error("Function Name :  Suite SuiteBidGrid Auth Failed");
                    return Json("Auth Fail");
                }
            }
            catch(Exception ex)
            {
                _log4net.Error("Function Name :  Suite SuiteBidGrid  -- " + ex.ToString());
                return null;
            }
          
        }
        public JsonResult SearchTestApproach(string search,string status)
        {
            try
            {
                var header = this.Request.Headers.ToString();
                var header1 = this.Request.Headers.ToList();
                var Auth = (string)this.Request.Headers["Authorization"];
                if (Auth != "" && Auth != null && Auth != "max-age=0")
                {
                    TimeZoneInfo timeZoneInfo = TZConvert.GetTimeZoneInfo("Europe/Stockholm");
                    //  TimeZoneInfo timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Etc/UTC");
                    DateTime dtCNow = TimeZoneInfo.ConvertTime(Convert.ToDateTime(DateTime.Now), timeZoneInfo);
                    string dtCNowdate = Convert.ToDateTime(dtCNow).ToString("yyyy-MM-dd");
                    TimeSpan tsnow = Convert.ToDateTime(dtCNow).TimeOfDay;
                    DateTime currentdatetime = Convert.ToDateTime(dtCNowdate + " " + (tsnow.Hours) + ":" + tsnow.Minutes);
                    var resultToken = db.TokenValue.FirstOrDefault(b => b.TockenId == Auth && b.Validto >= currentdatetime);
                    if (resultToken != null)
                    {
                        List<Rules> ca = new List<Rules>();
                        if (search == "" || search == null || search == "undefiened")
                        {
                            var a = from b in db.TestApproach
                                    join c in db.TestSuite on b.TestSuiteId equals c.TestSuiteId
                                    where b.Status == status
                                    select new
                                    {
                                        //    c.TestSuitename,
                                        TestSuitename = (b.SuiteIds == null ? "" : b.SuiteIds),
                                        b.CreatedDate,
                                        status = b.Status == "1" ? "Active" : "Inactive",
                                        Connectionname = (db.DbConfig.FirstOrDefault(x => x.Dbconfigid == Convert.ToInt32(b.Connectionname)).DbName != null ? db.DbConfig.FirstOrDefault(x => x.Dbconfigid == Convert.ToInt32(b.Connectionname)).DbName : ""),
                                        b.TestApproachName,
                                        b.TestApproachid,


                                    };
                            _log4net.Info("Function Name : Test Approach SearchTestApproach Successfully loaded");
                            return Json(a);
                        }
                        else
                        {
                            var a = (from b in db.TestApproach
                                     join c in db.TestSuite on b.TestSuiteId equals c.TestSuiteId
                                     where b.Status == status
                                     select new
                                     {
                                         TestSuitename = (b.SuiteIds == null ? "" : b.SuiteIds),
                                         // c.TestSuitename,
                                         b.CreatedDate,
                                         status = b.Status == "1" ? "Active" : "Inactive",
                                         Connectionname = (db.DbConfig.FirstOrDefault(x => x.Dbconfigid == Convert.ToInt32(b.Connectionname)).DbName != null ? db.DbConfig.FirstOrDefault(x => x.Dbconfigid == Convert.ToInt32(b.Connectionname)).DbName : ""),
                                         b.TestApproachName,
                                         b.TestApproachid,


                                     }).Where(x => x.TestSuitename.Contains(search) || x.TestApproachName.Contains(search));
                            _log4net.Info("Function Name : Test Approach SearchTestApproach Successfully loaded");
                            return Json(a);
                        }
                    }
                    else
                    {
                        _log4net.Error("Function Name : Test Approach SearchTestApproach Auth Failed ");
                        return Json("Auth Fail");
                    }

                }
                else
                {
                    _log4net.Error("Function Name : Test Approach SearchTestApproach Auth Failed ");
                    return Json("Auth Fail");
                }
            }
            catch(Exception ex)
            {
                _log4net.Error("Function Name : Test Approach SearchTestApproach  -- " + ex.ToString());
                return null;
            }
           

        }
        public JsonResult SearchSuite( string search,string status)
        {
            try
            {
                var header = this.Request.Headers.ToString();
                var header1 = this.Request.Headers.ToList();
                var Auth = (string)this.Request.Headers["Authorization"];
                if (Auth != "" && Auth != null && Auth != "max-age=0")
                {
                    TimeZoneInfo timeZoneInfo = TZConvert.GetTimeZoneInfo("Europe/Stockholm");
                    //   TimeZoneInfo timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Etc/UTC");
                    DateTime dtCNow = TimeZoneInfo.ConvertTime(Convert.ToDateTime(DateTime.Now), timeZoneInfo);
                    string dtCNowdate = Convert.ToDateTime(dtCNow).ToString("yyyy-MM-dd");
                    TimeSpan tsnow = Convert.ToDateTime(dtCNow).TimeOfDay;
                    DateTime currentdatetime = Convert.ToDateTime(dtCNowdate + " " + (tsnow.Hours) + ":" + tsnow.Minutes);
                    var resultToken = db.TokenValue.FirstOrDefault(b => b.TockenId == Auth && b.Validto >= currentdatetime);
                    if (resultToken != null)
                    {
                        if (search == "" || search == null || search == "undefiened")
                        {
                            var a = from b in db.TestSuite
                                    where b.Status == status
                                    select new
                                    {
                                        b.TestSuitename,
                                        b.CreatedDate,
                                        b.TestSuiteId,
                                        STATUS = b.Status == "1" ? "Active" : "Inactive",
                                    };
                            _log4net.Error("Function Name : Suite SearchSuite Search Suite Loaded");
                            return Json(a);
                        }
                        else
                        {
                            var a = from b in db.TestSuite
                                    where b.Status == "1" && b.TestSuitename.Contains(search) && b.Status == status
                                    select new
                                    {
                                        b.TestSuitename,
                                        b.CreatedDate,
                                        b.TestSuiteId,
                                        STATUS = b.Status == "1" ? "Active" : "Inactive",
                                    };
                            _log4net.Error("Function Name : Suite SearchSuite Search Suite Loaded");
                            return Json(a);
                        }
                    }
                    else
                    {
                        _log4net.Error("Function Name : Suite SearchSuite Auth Failed ");
                        return Json("Auth Fail");
                    }
                }
                else
                {
                    _log4net.Error("Function Name : Suite SearchSuite Auth Failed ");
                    return Json("Auth Fail");
                }
            }
            catch(Exception ex)
            {
                _log4net.Error("Function Name : Suite SearchSuite  -- " + ex.ToString());
                return null;
            }
          

        }
        public JsonResult EditApproachvalue(int ApproachId)
        {
            try
            {
                var header = this.Request.Headers.ToString();
                var header1 = this.Request.Headers.ToList();
                var Auth = (string)this.Request.Headers["Authorization"];
                if (Auth != "" && Auth != null && Auth != "max-age=0")
                {
                    TimeZoneInfo timeZoneInfo = TZConvert.GetTimeZoneInfo("Europe/Stockholm");
                    // TimeZoneInfo timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Etc/UTC");
                    DateTime dtCNow = TimeZoneInfo.ConvertTime(Convert.ToDateTime(DateTime.Now), timeZoneInfo);
                    string dtCNowdate = Convert.ToDateTime(dtCNow).ToString("yyyy-MM-dd");
                    TimeSpan tsnow = Convert.ToDateTime(dtCNow).TimeOfDay;
                    DateTime currentdatetime = Convert.ToDateTime(dtCNowdate + " " + (tsnow.Hours) + ":" + tsnow.Minutes);
                    var resultToken = db.TokenValue.FirstOrDefault(b => b.TockenId == Auth && b.Validto >= currentdatetime);
                    if (resultToken != null)
                    {
                        var a = from b in db.TestApproach
                                where b.TestApproachid == ApproachId
                                select new
                                {
                                    b.TestApproachid,
                                    b.TestApproachName,
                                    //b.TestSuiteId,
                                    b.Status,
                                    b.Connectionname,
                                    TestSuiteId = (b.SuiteIds == null ? "" : b.SuiteIds),

                                };
                        _log4net.Error("Function Name : Approach EditApproachvalue loaded Successfully ");
                        return Json(a);
                    }
                    else
                    {
                        _log4net.Error("Function Name : Approach EditApproachvalue Auth Failed ");
                        return Json("Auth Fail");
                    }
                }
                else
                {
                    _log4net.Error("Function Name : Approach EditApproachvalue Auth Failed ");
                    return Json("Auth Fail");
                }
            }
            catch(Exception ex)
            {
                _log4net.Error("Function Name : Approach EditApproachvalue  -- " + ex.ToString());
                return null;
            }
           
        }


        public ActionResult Release()
        {
            ViewBag.Report =configuration.GetValue<string>("ReportPortalIp");
            return View();
        }
        public string UpdatesRelease(string RNo,string RName,string Status ,int ReleaseId)
        {
            try
            {
                var header = this.Request.Headers.ToString();
                var header1 = this.Request.Headers.ToList();
                 var Auth= (string)this.Request.Headers["Authorization"];
                if (Auth != "" && Auth != null && Auth != "max-age=0")
                {
                    TimeZoneInfo timeZoneInfo = TZConvert.GetTimeZoneInfo("Europe/Stockholm");
                    // TimeZoneInfo timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
                    // TimeZoneInfo timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Etc/UTC");
                    DateTime dtCNow = TimeZoneInfo.ConvertTime(Convert.ToDateTime(DateTime.Now), timeZoneInfo);
                    string dtCNowdate = Convert.ToDateTime(dtCNow).ToString("yyyy-MM-dd");
                    TimeSpan tsnow = Convert.ToDateTime(dtCNow).TimeOfDay;
                    DateTime currentdatetime = Convert.ToDateTime(dtCNowdate + " " + (tsnow.Hours) + ":" + tsnow.Minutes);
                    var resultToken = db.TokenValue.FirstOrDefault(b => b.TockenId == Auth && b.Validto >= currentdatetime);
                    if (resultToken != null)
                    {
                        var result = db.ReleaseNo.FirstOrDefault(x => x.ReleasenoId == ReleaseId);
                        if (result != null)
                        {
                            result.ReleaseNo1 = RNo;
                            result.ReleaseName = RName;
                            result.Status = Status;
                            db.SaveChanges();
                        }
                        _log4net.Info("Function Name : Release UpdatesRelease Auth Failed");
                        return "Success";
                    }
                    else
                    {
                        _log4net.Error("Function Name : Release UpdatesRelease Auth Failed");
                        return "Auth Fail";
                    }
                }
                else
                {
                    _log4net.Error("Function Name : Release UpdatesRelease Auth Failed");
                    return "Auth Fail";
                }
            }
            catch(Exception ex)
            {
                _log4net.Error("Function Name : Release UpdatesRelease  -- " + ex.ToString());
                return "Fail";
            }

      
        }

        public JsonResult EditReleaseno(int Release)
        {
            try
            {
                var header = this.Request.Headers.ToString();
                var header1 = this.Request.Headers.ToList();
                 var Auth= (string)this.Request.Headers["Authorization"];
                if (Auth != "" && Auth != null && Auth != "max-age=0")
                {
                    TimeZoneInfo timeZoneInfo = TZConvert.GetTimeZoneInfo("Europe/Stockholm");
                    //TimeZoneInfo timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
                    //   TimeZoneInfo timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Etc/UTC");
                    DateTime dtCNow = TimeZoneInfo.ConvertTime(Convert.ToDateTime(DateTime.Now), timeZoneInfo);
                    string dtCNowdate = Convert.ToDateTime(dtCNow).ToString("yyyy-MM-dd");
                    TimeSpan tsnow = Convert.ToDateTime(dtCNow).TimeOfDay;
                    DateTime currentdatetime = Convert.ToDateTime(dtCNowdate + " " + (tsnow.Hours) + ":" + tsnow.Minutes);
                    var resultToken = db.TokenValue.FirstOrDefault(b => b.TockenId == Auth && b.Validto >= currentdatetime);
                    if (resultToken != null)
                    {
                        var a = from b in db.ReleaseNo
                                where b.ReleasenoId == Release
                                select new
                                {
                                    b.ReleaseNo1,
                                    b.ReleasenoId,
                                    b.ReleaseName,
                                    b.Status,
                                };
                        _log4net.Error("Function Name : Release EditReleaseno Successfully loaded");
                        return Json(a);
                    }
                    else
                    {
                        _log4net.Error("Function Name : Release EditReleaseno Auth Failed");
                        return Json("Auth Fail");
                    }
                }
                else
                {
                    _log4net.Error("Function Name : Release EditReleaseno Auth Failed" );
                    return Json("Auth Fail");
                }
            }
            catch(Exception ex)
            {
                _log4net.Error("Function Name : Release EditReleaseno  -- " + ex.ToString());
                return null;
            }
        }

        [HttpGet]
        public string SaveSuite(string SName,string testsuite,string Active)
        {
            try
            {
                var header = this.Request.Headers.ToString();
                var header1 = this.Request.Headers.ToList();
                var Auth = (string)this.Request.Headers["Authorization"];
                if (Auth != "" && Auth != null && Auth != "max-age=0")
                {
                    TimeZoneInfo timeZoneInfo = TZConvert.GetTimeZoneInfo("Europe/Stockholm");
                    //  TimeZoneInfo timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Etc/UTC");
                    DateTime dtCNow = TimeZoneInfo.ConvertTime(Convert.ToDateTime(DateTime.Now), timeZoneInfo);
                    string dtCNowdate = Convert.ToDateTime(dtCNow).ToString("yyyy-MM-dd");
                    TimeSpan tsnow = Convert.ToDateTime(dtCNow).TimeOfDay;
                    DateTime currentdatetime = Convert.ToDateTime(dtCNowdate + " " + (tsnow.Hours) + ":" + tsnow.Minutes);
                    var resultToken = db.TokenValue.FirstOrDefault(b => b.TockenId == Auth && b.Validto >= currentdatetime);
                    if (resultToken != null)
                    {
                        var result = db.TestApproach.FirstOrDefault(x => x.TestApproachName == SName &&  x.Status == Active);
                        if (result == null)
                        {
                            TestApproach Ins = new TestApproach()
                            {
                                TestApproachName = SName,
                                //TestSuiteId = testsuite,
                                //  Connectionname = DbNa,
                                //   Dbconfigid = Convert.ToInt32(DbNa),
                                SuiteIds = testsuite,
                                Status = Active,
                                CreatedDate = DateTime.Now,
                            };
                            db.TestApproach.Add(Ins);
                            db.SaveChanges();

                          

                        }
                        else
                        {
                            _log4net.Error("Function Name : ReleSase EditReleaseno Auth Failed ");
                            return "Fail";
                        }
                    }
                    else
                    {
                        _log4net.Error("Function Name : ReleSase EditReleaseno Auth Failed ");
                        return "Auth Fail";
                    }
                }
                else
                {
                    _log4net.Error("Function Name : ReleSase EditReleaseno Auth Failed ");
                    return "Auth Fail";
                }
                _log4net.Error("Function Name : ReleSase EditReleaseno Succssfully ");

                return "Success";
            }
            catch(Exception ex)
            {
                _log4net.Error("Function Name : Release EditReleaseno  -- " + ex.ToString());
                return null;
            }
           
        }


        public JsonResult deletesuite(int testsuiteid)
        {
            try
            {
                var header = this.Request.Headers.ToString();
                var header1 = this.Request.Headers.ToList();
                var Auth = (string)this.Request.Headers["Authorization"];
                if (Auth != "" && Auth != null && Auth != "max-age=0")
                {
                    TimeZoneInfo timeZoneInfo = TZConvert.GetTimeZoneInfo("Europe/Stockholm");
                    // TimeZoneInfo timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
                    //  TimeZoneInfo timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Etc/UTC");
                    DateTime dtCNow = TimeZoneInfo.ConvertTime(Convert.ToDateTime(DateTime.Now), timeZoneInfo);
                    string dtCNowdate = Convert.ToDateTime(dtCNow).ToString("yyyy-MM-dd");
                    TimeSpan tsnow = Convert.ToDateTime(dtCNow).TimeOfDay;
                    DateTime currentdatetime = Convert.ToDateTime(dtCNowdate + " " + (tsnow.Hours) + ":" + tsnow.Minutes);
                    var resultToken = db.TokenValue.First(b => b.TockenId == Auth && b.Validto >= currentdatetime);
                    if (resultToken != null)
                    {
                        db.TestSuite.RemoveRange(db.TestSuite.Where(x => x.TestSuiteId == testsuiteid));
                    //    db.TestCases.RemoveRange(db.TestCases.Where(x => x.RulesId == ReportId));
                        //  db.Rules.RemoveRange(db.Rules.FirstOrDefault(x => x.RulesId == RulesId));
                        db.SaveChanges();
                        return Json("success");
                    }
                    else
                    {
                        _log4net.Error("Function Name : connection BindGrid Auth Fail");
                        return Json("Auth Fail");
                    }
                }
                else
                {
                    _log4net.Error("Function Name : connection BindGrid Auth Fail");
                    return Json("Auth Fail");
                }

            }
            catch (Exception ex)
            {
                _log4net.Error("Function Name : connection BindGrid  -- " + ex.ToString());
                return null;
            }

        }


        public JsonResult deleteapproach(int testApproachid)
        {
            try
            {
                var header = this.Request.Headers.ToString();
                var header1 = this.Request.Headers.ToList();
                var Auth = (string)this.Request.Headers["Authorization"];
                if (Auth != "" && Auth != null && Auth != "max-age=0")
                {
                    TimeZoneInfo timeZoneInfo = TZConvert.GetTimeZoneInfo("Europe/Stockholm");
                    // TimeZoneInfo timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
                    //  TimeZoneInfo timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Etc/UTC");
                    DateTime dtCNow = TimeZoneInfo.ConvertTime(Convert.ToDateTime(DateTime.Now), timeZoneInfo);
                    string dtCNowdate = Convert.ToDateTime(dtCNow).ToString("yyyy-MM-dd");
                    TimeSpan tsnow = Convert.ToDateTime(dtCNow).TimeOfDay;
                    DateTime currentdatetime = Convert.ToDateTime(dtCNowdate + " " + (tsnow.Hours) + ":" + tsnow.Minutes);
                    var resultToken = db.TokenValue.First(b => b.TockenId == Auth && b.Validto >= currentdatetime);
                    if (resultToken != null)
                    {
                        db.TestApproach.RemoveRange(db.TestApproach.Where(x => x.TestApproachid == testApproachid));
                        //    db.TestCases.RemoveRange(db.TestCases.Where(x => x.RulesId == ReportId));
                        //  db.Rules.RemoveRange(db.Rules.FirstOrDefault(x => x.RulesId == RulesId));
                        db.SaveChanges();
                        return Json("success");
                    }
                    else
                    {
                        _log4net.Error("Function Name : connection BindGrid Auth Fail");
                        return Json("Auth Fail");
                    }
                }
                else
                {
                    _log4net.Error("Function Name : connection BindGrid Auth Fail");
                    return Json("Auth Fail");
                }

            }
            catch (Exception ex)
            {
                _log4net.Error("Function Name : connection BindGrid  -- " + ex.ToString());
                return null;
            }

        }
    }
}
