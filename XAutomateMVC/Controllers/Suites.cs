using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using TimeZoneConverter;
using XAutomateMVC.Models;
using XAutomateMVC.Models.DBModels;

namespace XAutomateMVC.Controllers
{
    public class Suites : Controller
    {

        db_mateContext db = new db_mateContext();
        public IActionResult Suite()
        {


            var SuiteList = (from product in db.DbConfig
                             where product.Status=="1"
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
                             where Suite.Status =="1"
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
            return View(productViewModel);

        }

        public string UpdateTestApproach(int testId, string SName, string testapproachname, string Active)
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
                var resultToken = db.TokenValue.First(b => b.TockenId == Auth && b.Validto >= currentdatetime);
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
                    return "Success";
                }
                else
                {
                    return "Auth Fail";
                }
            }
            else
            {
                return "Auth Fail";
            }
        }

        public JsonResult SuiteChange(string suitename)
        {
            List<Rules> ca = new List<Rules>();
            var a = from b in db.Rules
                    where b.TestApproachName == suitename
                    select new
                    {
                        b.RuleName,
                        b.RuleParameter,

                    };
            return Json(ca);
        }


        public string SaveTestSuite(string SName, string Active)
        {
            var header = this.Request.Headers.ToString();
            var header1 = this.Request.Headers.ToList();
             var Auth= (string)this.Request.Headers["Authorization"];
            if (Auth != "" && Auth != null && Auth != "max-age=0")
            {
                TimeZoneInfo timeZoneInfo = TZConvert.GetTimeZoneInfo("Europe/Stockholm");
                //  TimeZoneInfo timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Etc/UTC");
                DateTime dtCNow = TimeZoneInfo.ConvertTime(Convert.ToDateTime(DateTime.Now), timeZoneInfo);
                string dtCNowdate = Convert.ToDateTime(dtCNow).ToString("yyyy-MM-dd");
                TimeSpan tsnow = Convert.ToDateTime(dtCNow).TimeOfDay;
                DateTime currentdatetime = Convert.ToDateTime(dtCNowdate + " " + (tsnow.Hours) + ":" + tsnow.Minutes);
                var resultToken = db.TokenValue.First(b => b.TockenId == Auth && b.Validto >= currentdatetime);
                if (resultToken != null)
                {
                    var check = db.TestSuite.FirstOrDefault(x => x.TestSuitename == SName && x.Status == Active);
                    if (check == null)
                    {
                        TestSuite Ins = new TestSuite();
                        Ins.TestSuitename = SName;
                        Ins.Status = Active;
                        Ins.CreatedDate = DateTime.Now;
                        db.TestSuite.Add(Ins);
                        db.SaveChanges();
                        return "Success";
                    }
                    else
                    {
                        return "Fail";
                    }
                }
                else
                {
                    return "Auth Fail";
                }
            }
            else
            {
                return "Auth Fail";
            }
        }

        public string UpdateTestSuite(int testId, string SName, string Active)
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
                var resultToken = db.TokenValue.First(b => b.TockenId == Auth && b.Validto >= currentdatetime);
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
                                db.SaveChanges();
                            }
                            else
                            {
                                return "Validate";
                            }
                        }
                        else
                        {
                            result.TestSuitename = SName;
                            result.Status = Active;
                            db.SaveChanges();
                        }


                    }
                    return "Success";
                }
                else
                {
                    return "Auth Fail";
                }
            }
            else
            {
                return "Auth Fail";
            }
        }

        public JsonResult TestSuitegrid(string status)
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
                    var a = from b in db.TestSuite
                            where b.Status == status
                            select new
                            {
                                b.TestSuitename,
                                b.CreatedDate,
                                b.TestSuiteId,
                                STATUS = b.Status == "1" ? "Active" : "Inactive",
                            };
                    return Json(a);
                }
                else
                {
                    return Json("Auth Fail");
                }
            }
            else
            {
                return Json("Auth Fail");
            }

        }
        public JsonResult Editvalue(int SuiteId)
        {
            var header = this.Request.Headers.ToString();
            var header1 = this.Request.Headers.ToList();
             var Auth= (string)this.Request.Headers["Authorization"];
            if (Auth != "" && Auth != null && Auth != "max-age=0")
            {
                TimeZoneInfo timeZoneInfo = TZConvert.GetTimeZoneInfo("Europe/Stockholm");
                //  TimeZoneInfo timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Etc/UTC");
                DateTime dtCNow = TimeZoneInfo.ConvertTime(Convert.ToDateTime(DateTime.Now), timeZoneInfo);
                string dtCNowdate = Convert.ToDateTime(dtCNow).ToString("yyyy-MM-dd");
                TimeSpan tsnow = Convert.ToDateTime(dtCNow).TimeOfDay;
                DateTime currentdatetime = Convert.ToDateTime(dtCNowdate + " " + (tsnow.Hours) + ":" + tsnow.Minutes);
                var resultToken = db.TokenValue.First(b => b.TockenId == Auth && b.Validto >= currentdatetime);
                if (resultToken != null)
                {
                    var a = from b in db.TestSuite
                            where b.TestSuiteId == SuiteId
                            select new
                            {
                                b.TestSuitename,
                                b.Status,
                                b.TestSuiteId
                            };
                    return Json(a);
                }
                else
                {
                    return Json("Auth Fail");
                }
            }
            else
            {
                return Json("Auth Fail");
            }
        }

        public JsonResult SuiteBidGrid(string Active)
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
                var resultToken = db.TokenValue.First(b => b.TockenId == Auth && b.Validto >= currentdatetime);
                if (resultToken != null)
                {
                    List<Rules> ca = new List<Rules>();
                    var a = from b in db.TestApproach
                         //  join c in db.TestSuite on b.TestSuiteId equals c.TestSuiteId
                            where b.Status == Active
                            select new
                            {
                                //c.TestSuitename,
                                TestSuitename =( b.SuiteIds == null ? "" : b.SuiteIds),
                                b.CreatedDate,
                                status = b.Status == "1" ? "Active" : "Inactive",
                                Connectionname = (db.DbConfig.FirstOrDefault(x => x.Dbconfigid == Convert.ToInt32(b.Connectionname)).DbName != null ? db.DbConfig.FirstOrDefault(x => x.Dbconfigid == Convert.ToInt32(b.Connectionname)).DbName : ""),
                                b.TestApproachName,
                                b.TestApproachid,


                            };
                    return Json(a);
                }
                else
                {
                    return Json("Auth Fail");
                }
            }
            else
            {
                return Json("Auth Fail");
            }
        }
        public JsonResult SearchTestApproach(string search,string status)
        {
            var header = this.Request.Headers.ToString();
            var header1 = this.Request.Headers.ToList();
             var Auth= (string)this.Request.Headers["Authorization"];
            if (Auth != "" && Auth != null && Auth != "max-age=0")
            {
                TimeZoneInfo timeZoneInfo = TZConvert.GetTimeZoneInfo("Europe/Stockholm");
                //  TimeZoneInfo timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Etc/UTC");
                DateTime dtCNow = TimeZoneInfo.ConvertTime(Convert.ToDateTime(DateTime.Now), timeZoneInfo);
                string dtCNowdate = Convert.ToDateTime(dtCNow).ToString("yyyy-MM-dd");
                TimeSpan tsnow = Convert.ToDateTime(dtCNow).TimeOfDay;
                DateTime currentdatetime = Convert.ToDateTime(dtCNowdate + " " + (tsnow.Hours) + ":" + tsnow.Minutes);
                var resultToken = db.TokenValue.First(b => b.TockenId == Auth && b.Validto >= currentdatetime);
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
                                    TestSuitename= (b.SuiteIds == null ? "" : b.SuiteIds),
                                    b.CreatedDate,
                                    status = b.Status == "1" ? "Active" : "Inactive",
                                    Connectionname = (db.DbConfig.FirstOrDefault(x => x.Dbconfigid == Convert.ToInt32(b.Connectionname)).DbName != null ? db.DbConfig.FirstOrDefault(x => x.Dbconfigid == Convert.ToInt32(b.Connectionname)).DbName : ""),
                                    b.TestApproachName,
                                    b.TestApproachid,


                                };
                        return Json(a);
                    }
                    else
                    {
                        var a =( from b in db.TestApproach
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


                                }).Where(x=> x.TestSuitename.Contains(search) || x.TestApproachName.Contains(search));
                        return Json(a);
                    }
                }
                else
                {
                    return Json("Auth Fail");
                }
               
            }
            else
            {
                return Json("Auth Fail");
            }

        }
        public JsonResult SearchSuite( string search,string status)
        {
            var header = this.Request.Headers.ToString();
            var header1 = this.Request.Headers.ToList();
             var Auth= (string)this.Request.Headers["Authorization"];
            if (Auth != "" && Auth != null && Auth != "max-age=0")
            {
                TimeZoneInfo timeZoneInfo = TZConvert.GetTimeZoneInfo("Europe/Stockholm");
                //   TimeZoneInfo timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Etc/UTC");
                DateTime dtCNow = TimeZoneInfo.ConvertTime(Convert.ToDateTime(DateTime.Now), timeZoneInfo);
                string dtCNowdate = Convert.ToDateTime(dtCNow).ToString("yyyy-MM-dd");
                TimeSpan tsnow = Convert.ToDateTime(dtCNow).TimeOfDay;
                DateTime currentdatetime = Convert.ToDateTime(dtCNowdate + " " + (tsnow.Hours) + ":" + tsnow.Minutes);
                var resultToken = db.TokenValue.First(b => b.TockenId == Auth && b.Validto >= currentdatetime);
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
                        return Json(a);
                    }
                }
                else
                {
                    return Json("Auth Fail");
                }
            }
            else
            {
                return Json("Auth Fail");
            }

        }
        public JsonResult EditApproachvalue(int ApproachId)
        {
            var header = this.Request.Headers.ToString();
            var header1 = this.Request.Headers.ToList();
             var Auth= (string)this.Request.Headers["Authorization"];
            if (Auth != "" && Auth != null && Auth != "max-age=0")
            {
                TimeZoneInfo timeZoneInfo = TZConvert.GetTimeZoneInfo("Europe/Stockholm");
                // TimeZoneInfo timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Etc/UTC");
                DateTime dtCNow = TimeZoneInfo.ConvertTime(Convert.ToDateTime(DateTime.Now), timeZoneInfo);
                string dtCNowdate = Convert.ToDateTime(dtCNow).ToString("yyyy-MM-dd");
                TimeSpan tsnow = Convert.ToDateTime(dtCNow).TimeOfDay;
                DateTime currentdatetime = Convert.ToDateTime(dtCNowdate + " " + (tsnow.Hours) + ":" + tsnow.Minutes);
                var resultToken = db.TokenValue.First(b => b.TockenId == Auth && b.Validto >= currentdatetime);
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
                                TestSuiteId =  ( b.SuiteIds == null ? "" : b.SuiteIds),

                            };
                    return Json(a);
                }
                else
                {
                    return Json("Auth Fail");
                }
            }
            else
            {
                return Json("Auth Fail");
            }
        }


        public ActionResult Release()
        {
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
                    var resultToken = db.TokenValue.First(b => b.TockenId == Auth && b.Validto >= currentdatetime);
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
                        return "Success";
                    }
                    else
                    {
                        return "Auth Fail";
                    }
                }
                else
                {
                    return "Auth Fail";
                }
            }
            catch(Exception ex)
            {
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
                    var resultToken = db.TokenValue.First(b => b.TockenId == Auth && b.Validto >= currentdatetime);
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
                        return Json(a);
                    }
                    else
                    {
                        return Json("Auth Fail");
                    }
                }
                else
                {
                    return Json("Auth Fail");
                }
            }
            catch(Exception ex)
            {
                return null;
            }
        }

        [HttpGet]
        public string SaveSuite(string SName,int testsuite,string Active)
        {
            var header = this.Request.Headers.ToString();
            var header1 = this.Request.Headers.ToList();
             var Auth= (string)this.Request.Headers["Authorization"];
            if (Auth != "" && Auth != null && Auth != "max-age=0")
            {
                TimeZoneInfo timeZoneInfo = TZConvert.GetTimeZoneInfo("Europe/Stockholm");
                //  TimeZoneInfo timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Etc/UTC");
                DateTime dtCNow = TimeZoneInfo.ConvertTime(Convert.ToDateTime(DateTime.Now), timeZoneInfo);
                string dtCNowdate = Convert.ToDateTime(dtCNow).ToString("yyyy-MM-dd");
                TimeSpan tsnow = Convert.ToDateTime(dtCNow).TimeOfDay;
                DateTime currentdatetime = Convert.ToDateTime(dtCNowdate + " " + (tsnow.Hours) + ":" + tsnow.Minutes);
                var resultToken = db.TokenValue.First(b => b.TockenId == Auth && b.Validto >= currentdatetime);
                if (resultToken != null)
                {
                    var result = db.TestApproach.FirstOrDefault(x => x.TestApproachName == SName && x.TestSuiteId == testsuite && x.Status == Active);
                    if (result == null)
                    {
                        TestApproach Ins = new TestApproach()
                        {
                           // TestApproachName = SName,
                            TestSuiteId = testsuite,
                            //  Connectionname = DbNa,
                            //   Dbconfigid = Convert.ToInt32(DbNa),
                            SuiteIds = SName,
                            Status = Active,
                            CreatedDate = DateTime.Now,
                        };
                        db.TestApproach.Add(Ins);
                        db.SaveChanges();
                    }
                    else
                    {
                        return "Fail";
                    }
                }
                else
                {
                    return "Auth Fail";
                }
            }
            else
            {
                return "Auth Fail";
            }



            //Process proc = new Process();
            //proc.StartInfo.FileName = @"/bin/bash";
            //proc.StartInfo.CreateNoWindow = true;
            //proc.StartInfo.RedirectStandardInput = true;
            //proc.StartInfo.RedirectStandardOutput = true;
            //proc.StartInfo.UseShellExecute = false;
            //proc.StartInfo.CreateNoWindow = true;
            //proc.Start();
            //proc.StandardInput.WriteLine("cd  /srv/www/XAutomate/");
            //proc.StandardInput.WriteLine("mkdir  " + SName + " ");
            //proc.StandardInput.WriteLine("cd  /srv/www/XAutomate/" + SName + "");
            ////  proc.StandardInput.WriteLine("type nul > "+ SName + "Main.robot ");
            ////proc.StandardInput.WriteLine("touch  " + SName + "Main.robot ");
            ////proc.StandardInput.WriteLine("vi  " + SName + "Main.robot ");
            ////proc.StandardInput.WriteLine("i");
            ////proc.StandardInput.WriteLine("*** Settings ***");
            ////proc.StandardInput.WriteLine("Library");
            ////proc.StandardInput.WriteLine("DatabaseLibrary");
            ////proc.StandardInput.WriteLine("Library");
            ////proc.StandardInput.WriteLine("OperatingSystem");
            ////proc.StandardInput.WriteLine("*** Variables ***");
            ////proc.StandardInput.WriteLine("${DBName}   zapatademo");
            ////proc.StandardInput.WriteLine("${DBUser}   Zapatademo");
            ////proc.StandardInput.WriteLine("${DBPass}   Sysmedac2020#");
            ////proc.StandardInput.WriteLine("${DBHost}   zapatademo.csp3xo41ydwh.us-west-1.rds.amazonaws.com");
            ////proc.StandardInput.WriteLine("${DBPort}   3306");

            //proc.StandardInput.Flush();
            //proc.StandardInput.Close();
            //proc.WaitForExit();
            //string Error = proc.StandardOutput.ReadToEnd();
            //Console.WriteLine(proc.StandardOutput.ReadToEnd());
            //System.Diagnostics.Debug.WriteLine(proc.StandardOutput.ReadToEnd());


            //var result1 = db.DbConfig.FirstOrDefault(x => x.DbName == DbNa);

            //{
            //    Process proc1 = new Process();
            //    proc1.StartInfo.FileName = @"/bin/bash";
            //    proc1.StartInfo.CreateNoWindow = true;
            //    proc1.StartInfo.RedirectStandardInput = true;
            //    proc1.StartInfo.RedirectStandardOutput = true;
            //    proc1.StartInfo.UseShellExecute = false;
            //    proc1.StartInfo.CreateNoWindow = true;
            //    proc1.Start();
            //    proc1.StandardInput.WriteLine("cd /srv/www/XAutomate/" + SName + "");
            //    // proc1.StandardInput.WriteLine("touch " + Name + ".robot ");
            //    proc1.StandardInput.WriteLine("sudo cat > " + SName + "Main.robot ");

            //    //   proc1.StandardInput.WriteLine("i");
            //    proc1.StandardInput.WriteLine("*** Settings ***");
            //    proc1.StandardInput.WriteLine("Library      ListenerLibrary");
            //    proc1.StandardInput.WriteLine("Library  OperatingSystem");
            //    proc1.StandardInput.WriteLine("Library   DatabaseLibrary");
            //    proc1.StandardInput.WriteLine("*** Variables  ***");
            //    proc1.StandardInput.WriteLine("${DBName}   " + result1.DbName  + "");
            //    proc1.StandardInput.WriteLine("${DBUser}   " + result1.DbUser + "");
            //    proc1.StandardInput.WriteLine("${DBPass}   " + result1.DbPassword + "");
            //    proc1.StandardInput.WriteLine("${DBHost}   " + result1.DbHostName + "");
            //    proc1.StandardInput.WriteLine("${DBPort}   " + result1.DbPort + "");

            //    proc1.StandardInput.Flush();
            //    proc1.StandardInput.Close();
            //    proc1.WaitForExit();
            //    string Error1 = proc1.StandardOutput.ReadToEnd();
            //    Console.WriteLine(proc1.StandardOutput.ReadToEnd());
            //    System.Diagnostics.Debug.WriteLine(proc1.StandardOutput.ReadToEnd());


            //}

            return "Success";
        }
    }
}
