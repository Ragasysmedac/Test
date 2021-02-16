using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Mail;
using System.Threading.Tasks;
using ClosedXML.Excel;
using Ionic.Zip;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using TimeZoneConverter;
using XAutomateMVC.Models;
using XAutomateMVC.Models.DBModels;

namespace XAutomateMVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        db_mateContext db = new db_mateContext();
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Login()
        {
            return View();
        }

        public JsonResult loginSubmit(string uName,string pass)
        {
            try
            {
                var list = (string)this.Request.Headers["Authorization"];
                var header = this.Request.Headers.ToString();
                var header1 = this.Request.Headers.ToList();
                 var Auth= (string)this.Request.Headers["Authorization"];
                if (Auth != "" && Auth != null && Auth != "max-age=0")
                {
                    //   TimeZoneInfo timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
                    // TimeZoneInfo timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Etc/UTC");
                    TimeZoneInfo timeZoneInfo = TZConvert.GetTimeZoneInfo("Europe/Stockholm");
                    DateTime dtCNow = TimeZoneInfo.ConvertTime(Convert.ToDateTime(DateTime.Now), timeZoneInfo);

                    string dtCNowdate = Convert.ToDateTime(dtCNow).ToString("yyyy-MM-dd");
                    TimeSpan tsnow = Convert.ToDateTime(dtCNow).TimeOfDay;
                    DateTime currentdatetime = Convert.ToDateTime(dtCNowdate + " " + (tsnow.Hours + 1) + ":" + tsnow.Minutes);
                    TokenValue Ins = new TokenValue();
                    Ins.TockenId = Auth;
                    Ins.Validto = currentdatetime;
                    db.TokenValue.Add(Ins);
                    db.SaveChanges();
                    var a = from b in db.Login
                            where b.UserName == uName && b.Password == pass
                            select new
                            {
                                b.LoginId,
                                b.RoleNavigation.ScreenName,
                                b.RoleNavigation.RoleName,
                            };
                    return Json(a);

                }
                else
                {
                    return Json("Auth Fail");
                }
            }
         catch(Exception ex)
            {
                return Json(ex);
            }
          
            //var result = db.Login.FirstOrDefault(x => x.UserName == uName && x.Password == pass);
            //if(result != null)
            //{
            //    TempData["Role"] = result.Role;
            //    return Json(result.RoleNavigation.RoleName);
            //}
            //else
            //{
            //    return Json("Fail");
            //}
            //return Json("Success");
        }

        public IActionResult Privacy()
        {
            return View();
        }
        public IActionResult Dbetl()
        {
            var SuiteList = (from product in db.TestApproach

                             where product.Status == "1"
                             select new SelectListItem()
                             {
                                 Text = product.TestApproachName,
                                 Value = product.TestApproachid.ToString(),
                             }).ToList();

            SuiteList.Insert(0, new SelectListItem()
            {
                Text = "----Select----",
                Value = string.Empty
            });

            var SuiteSearch = (from product in db.TestApproach

                               where product.Status == "1"
                               select new SelectListItem()
                             {
                                 Text = product.TestApproachName,
                                 Value = product.TestApproachid.ToString(),
                             }).ToList();

            SuiteSearch.Insert(0, new SelectListItem()
            {
                Text = "Select Suite",
                Value = string.Empty
            });

            var Expectedresult = (from product in db.ExpectedResult

                                  where product.Status == 1
                                  select new SelectListItem()
                               {
                                   Text = product.Expectedresultname,
                                   Value = product.Expectedresultname.ToString(),
                               }).ToList();

            Expectedresult.Insert(0, new SelectListItem()
            {
                Text = "Select Expected Result",
                Value = string.Empty
            });
            var RuleList = (from Rules in db.Rules

                            where Rules.Status == "1"
                            select new SelectListItem()
                            {
                                Text = Rules.RuleName,
                                Value = Rules.RulesId.ToString(),
                            }).ToList();

            RuleList.Insert(0, new SelectListItem()
            {
                Text = "Select Rule",
                Value = string.Empty
            });
            var Dbconfig = (from product in db.DbConfig
                            where product.Status == "1"
                            select new SelectListItem()
                            {
                                Text = product.DbName,
                                Value = product.Dbconfigid.ToString(),
                            }).ToList();

            Dbconfig.Insert(0, new SelectListItem()
            {
                Text = "----Select----",
                Value = string.Empty
            });
            RulesModel viewmodel = new RulesModel();
            ProductViewModel productViewModel = new ProductViewModel();
            productViewModel.Listofproducts = RuleList;
            productViewModel.ProductSearchList = RuleList;
            productViewModel.ExpectedResult = Expectedresult;
            productViewModel.TestSuiteList = Dbconfig;
            productViewModel.RuleList = RuleList;
            return View(productViewModel);
        }


        public FileResult Tescasetemplate()
        {
            List<FileonSystem> list = new List<FileonSystem>();

            var Rules = (from b in db.Rules
                         where b.Status == "1"
                         select new
                         {
                             b.TestApproach.TestApproachName,
                             b.RuleName,
                             b.RuleParameter,
                             b.Description,
                             b.RuleCondtion,
                             TestCase= b.TestCases.Select(xx=> new
                             {
                                 xx.TestCaseName,
                                 xx.TestcaseTitle,
                                 xx.ExceptedResult,
                                 xx.Expextedparameter,
                                 xx.Testdata,
                                 xx.Description,
                             } ),
                             RuleParamter = b.RuleParamters.Select(x => new
                             {
                                 x.ParameterName,
                             }),
                         }).ToList();
            DataRow row;
            foreach (var items in Rules)
            {
                DataTable dt = new DataTable(items.RuleName);
                dt.Columns.AddRange(new DataColumn[3] { new DataColumn("Test Approach"),
                                            new DataColumn("Rule Name"),
                                              new DataColumn("TestCae Name"),

                });
                var rulescolum = "";
                int d = 0;
                int T = 0;
                int C = 0;
                foreach (var rules in items.RuleParamter)
                {
                     rulescolum += ","+rules.ParameterName;
                    if (dt.Columns.Contains(rules.ParameterName))
                    {
                        
                        d++;
                        dt.Columns.Add(rules.ParameterName + "" + d);
                       
                       
                    }
                    else
                    {
                        dt.Columns.Add(rules.ParameterName);

                    }
                   
                   // dt.Columns.AddRange(new DataColumn[1] { new DataColumn(rules.ParameterName) });
                }
                dt.Columns.AddRange(new DataColumn[2] { new DataColumn("Expected Result"),
                                            new DataColumn("Expected Value"),

                });
                dt.Columns.AddRange(new DataColumn[1]{ new DataColumn("Description"),

                            });


                foreach (var item in items.TestCase)
                    {
                    row = dt.NewRow();
                    row["Test Approach"] = items.TestApproachName;
                    row["Rule Name"] = items.RuleName;
                    row["TestCae Name"] = item.TestCaseName;
                    int m = 0;
                    int r = 0;
                    //foreach (var rules in items.RuleParamter)
                    //{
                    for(int k =0;k< (items.RuleParamter.Count()); k++) { 
                        var testcase = item.Testdata.Split(",");
                        row[k +3] = testcase[m];
                        m++;
                      
                            //r++;
                            //row[rules.ParameterName+""+r] = testcase[m];
                            ////   dt.Columns.Add(rules.ParameterName + "" + d);
                        
                      
                        //for (var k = 0; k < testcase.Length; k++)
                        //{
                        //    dt.Columns.AddRange(new DataColumn[1] { new DataColumn(testcase[k]) });
                        //}

                    }
                    var testcaselist = item.Testdata.Split(",");
                    row["Expected Result"] = item.ExceptedResult; 
                    row["Expected Value"] =item.Expextedparameter;
                    row["Description"] = item.Description;
                    dt.Rows.Add(row);
                }
               
             

                using (XLWorkbook wb = new XLWorkbook())
                {
                    wb.Worksheets.Add(dt);
                    using (MemoryStream stream = new MemoryStream())
                    {
                        list.Add(new FileonSystem()
                        {
                            FilePath = Directory.GetCurrentDirectory() + "\\Testcases\\" + items.RuleName + ".xlsx",
                        });
                        wb.SaveAs(stream);
                        wb.SaveAs(Directory.GetCurrentDirectory() + "\\Testcases\\" + items.RuleName + ".xlsx");
                        var result = File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Grid.xlsx");

                    }
                }

            }

            using (ZipFile zip = new ZipFile())
            {
                zip.AlternateEncodingUsage = ZipOption.AsNecessary;

                foreach (var file in list)
                {

                    zip.AddFile(file.FilePath, "Testcases");

                }
                //   zip.AddFile(wb, "Files");
                string zipName = String.Format("Zip_{0}.zip", DateTime.Now.ToString("yyyy-MMM-dd-HHmmss"));
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    zip.Save(memoryStream);
                    return File(memoryStream.ToArray(), "application/zip", zipName);
                }
            }
            return File("", "", "");
        }


        public JsonResult SearchReport(string search,string status)
        {
            if (search == "" || search == null || search == "undefiend")
            {
                var a = from b in db.Exceute
                        where b.Status == status
                        orderby b.ExecuteId descending
                        select new
                        {
                            b.SuiteName,
                            b.ResultUrl,
                            Execute = b.Execute,
                            b.Time,
                            b.Status,
                        };
                return Json(a);
            }
            else
            {
                var a = from b in db.Exceute
                        where b.SuiteName.Contains(search) || b.ResultUrl.Contains(search) && b.Status == status
                        orderby b.ExecuteId descending
                        select new
                        {
                            b.SuiteName,
                            b.ResultUrl,
                            Execute = b.Execute,
                            b.Time,
                            b.Status,
                        };
                return Json(a);
            }

        }
        public JsonResult ExecBindGrid(string Active)
        {
            var a = from b in db.Exceute
                    where b.Status == Active
                    orderby b.ExecuteId descending
                    select new
                    {
                        b.SuiteName,
                        b.ResultUrl,
                        Execute = b.Execute,
                        b.Time,
                        Status = b.Status,
                        b.ExecuteId,
                    };
            return Json(a);
        }

        public IActionResult Report()
        {
            return View();
        }

       
        public JsonResult RulesConfigset(int SuiteId)
        {
            var header = this.Request.Headers.ToString();
            var header1 = this.Request.Headers.ToList();
             var Auth= (string)this.Request.Headers["Authorization"];
            if (Auth != "" && Auth != null && Auth != "max-age=0")
            {
                TimeZoneInfo timeZoneInfo = TZConvert.GetTimeZoneInfo("Europe/Stockholm");
                //TimeZoneInfo timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Etc/UTC");
                DateTime dtCNow = TimeZoneInfo.ConvertTime(Convert.ToDateTime(DateTime.Now), timeZoneInfo);
                string dtCNowdate = Convert.ToDateTime(dtCNow).ToString("yyyy-MM-dd");
                TimeSpan tsnow = Convert.ToDateTime(dtCNow).TimeOfDay;
                DateTime currentdatetime = Convert.ToDateTime(dtCNowdate + " " + (tsnow.Hours) + ":" + tsnow.Minutes);
                var resultToken = db.TokenValue.First(b => b.TockenId == Auth && b.Validto >= currentdatetime);
                if (resultToken != null)
                {
                    var a = from b in db.Rules
                            where b.TestApproachid == SuiteId && b.Status =="1"
                            select new
                            {
                                b.RuleName,

                                b.RulesId,
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

        //public JsonResult Tablecolumn (int SuiteId)
        //{
        //    try
        //    {
        //        var a = from b in db.TestApproach
        //                join c in db.DbConfig on b.Dbconfigid equals c.Dbconfigid
        //                join d in db.Tablecolumn on c.DbName equals d.DatabseName
        //                where b.TestApproachid == SuiteId
        //                select new
        //                {
        //                    d.Tablecolumn1,
        //                    d.DatabseName,
        //                    d.FieldName,
        //                };
        //        return Json(a);
        //    }
        //    catch(Exception ex)
        //    {
        //        return null;
        //    }
          
       // }

        public JsonResult Connectionchange(int connections,int rules)
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
                    var resultToken = db.TokenValue.First(b => b.TockenId == Auth && b.Validto >= currentdatetime);
                    if (resultToken != null)
                    {
                        var dbconn = db.DbConfig.FirstOrDefault(x => x.Dbconfigid == connections);
                        if(dbconn != null)
                        {
                            try
                            {
                                if (dbconn.DatabaseType == "MYSQL")
                                {
                                    MySqlConnection sql = new MySqlConnection("Server=" + dbconn.DbHostName + ";Port=" + dbconn.DbPort + ";Database=" + dbconn.DbName + ";User ID=" + dbconn.DbUser + ";Password=" + dbconn.DbPassword + ";SslMode=none");
                                    sql.Open();
                                    MySqlCommand com = new MySqlCommand("SELECT DISTINCT  COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS  where TABLE_SCHEMA != 'information_schema' && TABLE_SCHEMA !='performance_schema' && TABLE_SCHEMA != 'sys' && TABLE_SCHEMA != 'mysql'; ", sql);
                                    using (var reader = com.ExecuteReader())
                                    {
                                        db.Tablecolumn.RemoveRange(db.Tablecolumn.Where(x => x.Dbconfigid == connections));
                                        db.SaveChanges();
                                        while (reader.Read())
                                        {

                                            Tablecolumn Ins = new Tablecolumn();
                                            Ins.Tablecolumn1 = reader["COLUMN_NAME"].ToString();
                                            Ins.FieldName = "C";
                                            Ins.Active = "1";
                                            Ins.Dbconfigid = connections;
                                            db.Tablecolumn.Add(Ins);
                                            db.SaveChanges();
                                        }
                                    }
                                    MySqlCommand com1 = new MySqlCommand("SELECT DISTINCT  TABLE_NAME FROM INFORMATION_SCHEMA.COLUMNS  where TABLE_SCHEMA != 'information_schema' && TABLE_SCHEMA !='performance_schema' && TABLE_SCHEMA != 'sys' && TABLE_SCHEMA != 'mysql'; ", sql);
                                    using (var reader = com1.ExecuteReader())
                                    {
                                        while (reader.Read())
                                        {

                                            Tablecolumn Ins2 = new Tablecolumn();
                                            Ins2.Tablecolumn1 = reader["TABLE_NAME"].ToString();
                                            Ins2.FieldName = "T";
                                            Ins2.Active = "1";
                                            Ins2.Dbconfigid = connections;
                                            db.Tablecolumn.Add(Ins2);

                                            db.SaveChanges();
                                        }
                                    }

                                    MySqlCommand com2 = new MySqlCommand("SELECT DISTINCT  TABLE_SCHEMA FROM INFORMATION_SCHEMA.COLUMNS where TABLE_SCHEMA != 'information_schema' && TABLE_SCHEMA !='performance_schema' && TABLE_SCHEMA != 'sys' && TABLE_SCHEMA != 'mysql'; ",sql);
                                    using (var reader = com2.ExecuteReader())
                                    {
                                        while (reader.Read())
                                        {

                                            Tablecolumn Ins3 = new Tablecolumn();
                                            Ins3.Tablecolumn1 = reader["TABLE_SCHEMA"].ToString();
                                            Ins3.FieldName = "D";
                                            Ins3.Active = "1";
                                            Ins3.Dbconfigid = connections;
                                            db.Tablecolumn.Add(Ins3);
                                            db.SaveChanges();
                                          
                                        }
                                    }
                                    sql.Close();
                                }
                                else
                                {
                                    SqlConnection sql = new SqlConnection("Server=" + dbconn.DbHostName + ";Database=" + dbconn.DbName + ";Uid=" + dbconn.DbUser + ";Pwd=" + dbconn.DbPassword + ";");
                                    sql.Open();
                                    SqlCommand com = new SqlCommand("SELECT DISTINCT  COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS", sql);
                                    //  MySqlConnection sql = new MySqlConnection("Server=" + Dbhost + ";Port=" + DbPortname + ";Database=" + Dbname + ";User ID=" + Dbuser + ";Password=" + DbPassword + ";SslMode=none");
                                    // sql.Open();
                                    // MySqlCommand com = new MySqlCommand("SELECT DISTINCT  COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS where  TABLE_SCHEMA='" + Dbname + "' ", sql);
                                    using (var reader = com.ExecuteReader())
                                    {
                                        db.Tablecolumn.RemoveRange(db.Tablecolumn.Where(x => x.Dbconfigid == connections));
                                        db.SaveChanges();
                                        while (reader.Read())
                                        {

                                            Tablecolumn Ins = new Tablecolumn();
                                            Ins.Tablecolumn1 = reader["COLUMN_NAME"].ToString();
                                            Ins.FieldName = "C";
                                            Ins.Active = "1";
                                            Ins.Dbconfigid = connections;
                                            db.Tablecolumn.Add(Ins);
                                            db.SaveChanges();
                                        }
                                    }
                                    SqlCommand com1 = new SqlCommand("SELECT DISTINCT  TABLE_NAME FROM INFORMATION_SCHEMA.COLUMNS ", sql);
                                    //  MySqlCommand com1 = new MySqlCommand("SELECT DISTINCT  TABLE_NAME FROM INFORMATION_SCHEMA.COLUMNS where  TABLE_SCHEMA='" + Dbname + "' ", sql);
                                    using (var reader = com1.ExecuteReader())
                                    {
                                        while (reader.Read())
                                        {

                                            Tablecolumn Ins2 = new Tablecolumn();
                                            Ins2.Tablecolumn1 = reader["TABLE_NAME"].ToString();
                                            Ins2.FieldName = "T";
                                            Ins2.Active = "1";
                                            Ins2.Dbconfigid = connections;
                                            db.Tablecolumn.Add(Ins2);

                                            db.SaveChanges();
                                        }
                                    }
                                    SqlCommand com2 = new SqlCommand("SELECT DISTINCT  TABLE_SCHEMA FROM INFORMATION_SCHEMA.COLUMNS  ", sql);
                                    using (var reader = com2.ExecuteReader())
                                    {
                                        while (reader.Read())
                                        {

                                            Tablecolumn Ins3 = new Tablecolumn();
                                            Ins3.Tablecolumn1 = reader["TABLE_SCHEMA"].ToString();
                                            Ins3.FieldName = "D";
                                            Ins3.Active = "1";
                                            Ins3.Dbconfigid = connections;
                                            db.Tablecolumn.Add(Ins3);
                                            db.SaveChanges();

                                        }
                                    }
                                    sql.Close();

                                }

                                


                            }
                            catch (Exception ex)
                            {
                                return Json("");
                            }
                        }

                       


                        var a = from b in db.Rules
                                where b.Status == "1" && b.RulesId == rules
                                orderby b.RulesId descending
                                select new
                                {
                                  
                                    b.RulesId,
                                   

                                    parameter = b.DbConfig.Tablecolumn.Select(x => new
                                    {
                                        x.FieldName,
                                        color = x.FieldName == "C" ? "black" : x.FieldName == "D" ? "RED" : "Blue",
                                        x.Tablecolumn1,
                                        x.Dbconfigid,
                                    }),
                                    LabelName = b.RuleParamters.Select(x => new
                                    {
                                        x.ParameterName,
                                    }),
                                   ruleParameter = b.RuleParameter,
                                    connection = b.DbConfigId,
                                    ruleCondtion = b.RuleCondtion,
                                   
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
            catch (Exception ex)
            {
                return Json("Fail");
            }
        }

        public JsonResult Editdbetl(int dbetlId)
        {
            try
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
                        var a = from b in db.TestCases
                                where b.Status == "1" && b.TestCasesId == dbetlId
                                orderby b.TestCasesId descending
                                select new
                                {
                                    b.TestCasesId,
                                    b.TestCaseName,
                                    b.TestcaseTitle,
                                    b.Testdata,
                                    b.Status,
                                    b.Description,
                                    b.RulesId,
                                    b.TestApproachid,

                                    parameter = b.Rules.DbConfig.Tablecolumn.Select(x => new
                                    {
                                        x.FieldName,
                                        color = x.FieldName == "C" ? "black" : x.FieldName == "D" ? "RED" : "Blue",
                                        x.Tablecolumn1,
                                        x.Dbconfigid,
                                    }),
                                    LabelName = b.Rules.RuleParamters.Select(x => new
                                    {
                                        x.ParameterName,
                                    }),
                                    testcaseparameter = b.TestCaseParameters.Select(x=> new { 
                                    x.ParameterName,
                                    }),
                                    b.ExceptedResult,
                                    ruleParameter = b.Rules.RuleParameter,
                                    connection = b.Rules.DbConfigId,
                                    ruleCondtion = b.Rules.RuleCondtion,
                                    b.Expextedparameter,
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
                return Json("Fail");
            }
        }

        public JsonResult Bindgridrule(int Rules,string status)
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
                    var a = from b in db.TestCases
                            join c in db.Rules on b.RulesId equals c.RulesId
                            join d in db.TestApproach on c.TestApproachid equals d.TestApproachid
                            where b.RulesId == Rules && b.Status == status
                            orderby b.TestCasesId descending
                            select new
                            {
                                TestCaseName = (b.TestCaseName == null ? "" : b.TestCaseName),
                                d.TestApproachName,
                                c.RuleName,
                                c.RulesId,
                                testcaseparameter = b.TestCaseParameters.Select(x => new {
                                    x.ParameterName,
                                }),
                                b.TestcaseTitle,
                                b.Testdata,
                                b.ExceptedResult,
                                Description = (b.Description == null ? "" : b.Description),
                                b.TestCasesId,
                                Status=   b.Status =="1" ? "Active" : "Inactive",
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

        public JsonResult EditGrid(int test, string rules)
        {
            try
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
                        var a = from b in db.TestCases
                                join c in db.Rules on b.RulesId equals c.RulesId
                                join d in db.TestApproach on c.TestApproachid equals d.TestApproachid
                                where b.TestCasesId == test
                                select
                                new
                                {
                                    c.RuleCondtion,
                                    c.RuleParameter,
                                    b.TestCaseName,
                                    d.TestApproachName,
                                    d.TestApproachid,
                                    c.RulesId,
                                    parameter = d.Dbconfig.Tablecolumn.Select(x => new
                                    {
                                        x.FieldName,
                                        color = x.FieldName == "C" ? "black" : x.FieldName == "D" ? "RED" : "Blue",
                                        x.Tablecolumn1,
                                        x.Dbconfigid,
                                    }),
                                    LabelName = c.RuleParamters.Select(x => new
                                    {
                                        x.ParameterName,
                                    }),
                                    c.RuleName,
                                    b.TestcaseTitle,
                                    b.Testdata,
                                    b.ExceptedResult
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

            catch (Exception ex)
            {
                return null;
            }

        }

        public JsonResult SearchBindGrid(string search,string status)
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
                    if (search == "" || search == null || search == "undefiened")
                    {
                        var a = from b in db.TestCases
                                join c in db.Rules on b.RulesId equals c.RulesId
                                join d in db.TestApproach on c.TestApproachid equals d.TestApproachid
                                where b.Status == status
                                select new
                                {
                                    TestCaseName = (b.TestCaseName == null ? "" : b.TestCaseName),
                                    d.TestApproachName,
                                    c.RuleName,
                                    b.TestcaseTitle,
                                    b.Testdata,
                                    b.ExceptedResult,
                                    testparameter = b.TestCaseParameters.Select(x=> new
                                    {
                                        x.ParameterName,
                                    }),
                                    b.TestCasesId,
                                    Status =  b.Status == "1" ? "Active" : "Inactive",
                                    Description = (b.Description == null ? "" : b.Description),
                                };
                        return Json(a);
                    }
                    else
                    {
                        var a = (from b in db.TestCases
                                join c in db.Rules on b.RulesId equals c.RulesId
                                join d in db.TestApproach on c.TestApproachid equals d.TestApproachid
                                where  b.Status == status
                                select new
                                {
                                    TestCaseName =(   b.TestCaseName == null ? "" :b.TestCaseName),
                                    d.TestApproachName,
                                    c.RuleName,
                                    b.TestcaseTitle,
                                    b.Testdata,
                                    b.ExceptedResult,
                                    testparameter = b.TestCaseParameters.Select(x => new
                                    {
                                        x.ParameterName,
                                    }),
                                    Status = b.Status == "1" ? "Active" : "Inactive",
                                    b.TestCasesId,
                                    Description = (b.Description == null ? "" : b.Description),
                                }).Where(x=> x.TestCaseName.Contains(search) || x.TestApproachName.Contains(search) || x.RuleName.Contains(search) || x.ExceptedResult.Contains(search) || x.Testdata.Contains(search) );
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

        public JsonResult ETLBindGrid(string status)
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
                    var a = from b in db.TestCases
                            join c in db.Rules on b.RulesId equals c.RulesId
                            join d in db.TestApproach on c.TestApproachid equals d.TestApproachid
                            where b.Status == status
                            select new
                            {
                                b.TestCaseName,
                                d.TestApproachName,
                                c.RuleName,
                                b.TestcaseTitle,
                               
                               Testdata = b.TestCaseParameters.Select(X=> new { 
                               X.ParameterName,
                               }),
                                b.ExceptedResult,
                                b.TestCasesId,
                                Description = (b.Description == null ? "" : b.Description),
                                Status = (b.Status == "1" ? "Active" : "Inactive"),
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

        public IActionResult Execute()
        {
            var TestApproach = (from product in db.TestSuite
                                where product.Status=="1"
                                select new SelectListItem()
                             {
                                 Text = product.TestSuitename,
                                 Value = product.TestSuiteId.ToString(),
                             }).ToList();

            TestApproach.Insert(0, new SelectListItem()
            {
                Text = "----Select----",
                Value = string.Empty
            });

            ProductViewModel productViewModel = new ProductViewModel();
            productViewModel.Listofproducts = TestApproach;

            return View(productViewModel);
        }

        public JsonResult RulesCondtions(int RulesId)
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
                    var a = from b in db.Rules
                            where b.RulesId == RulesId
                            select
                            new
                            {
                                Parameter = b.DbConfig.Tablecolumn.Select(x => new
                                {
                                    x.FieldName,
                                    color = x.FieldName == "C" ? "black" : x.FieldName == "D" ? "RED" : "blue",
                                    x.Tablecolumn1,
                                    x.Dbconfigid,
                                }),
                                Label = b.RuleParamters.Select(x => new
                                {
                                    x.ParameterName,
                                }),
                                b.RuleCondtion,
                                b.RuleParameter,
                                b.RuleName,
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
      
        [HttpGet]
        public String SaveGrid( string Name, int SuiteName, string Except, string parameter, string testRule,string Description,string status,string ExpectedValue ,int connection)
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
                    TestCases Is = new TestCases();
                    //  Is.SuiteId = SuiteName;
                  //  Is.TestApproachid = SuiteName;
                    Is.RulesId = SuiteName;
                    Is.TestCaseName = Name;
                    // Is.RuleId = Rules;
                    Is.ExceptedResult = Except + ExpectedValue;
                    Is.Testdata = parameter;
                    Is.Description = Description;
                    Is.CreatedDate = DateTime.Now;
                    Is.Expextedparameter = ExpectedValue;
                    Is.Status = status;
                    db.TestCases.Add(Is);
                    db.SaveChanges();

                    var rulechecck = db.Rules.FirstOrDefault(x => x.RulesId == Is.RulesId);
                    if(rulechecck != null)
                    {
                        rulechecck.DbConfigId = Is.RulesId;
                        db.SaveChanges();
                    }


                    var para = parameter.Split(",,,");
                    for(int i=0; i < para.Length; i++)
                    {
                        TestCaseParameters ins = new TestCaseParameters();
                        ins.ParameterName = para[i];
                        ins.TestCasesId = Is.TestCasesId;
                        db.TestCaseParameters.Add(ins);
                        db.SaveChanges();
                    }

                    var a = "Success";
                    return a;
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
            //var Parameter = parameter.Split(",");
            //var demo = testRule;
            //for (int i = 0; i < Parameter.Length; i++)
            //{
            //    demo = demo.Replace("[" + i + "]", "" + Parameter[i] + "");
            //}


            //Process proc = new Process();
            //proc.StartInfo.FileName = @"/bin/bash";
            //proc.StartInfo.CreateNoWindow = true;
            //proc.StartInfo.RedirectStandardInput = true;
            //proc.StartInfo.RedirectStandardOutput = true;
            //proc.StartInfo.UseShellExecute = false;
            //proc.StartInfo.CreateNoWindow = true;
            //proc.Start();
            //proc.StandardInput.WriteLine("cd /srv/www/XAutomate/" + SuiteName + "");
            //// proc.StandardInput.WriteLine("touch " + Name + ".robot ");
            //proc.StandardInput.WriteLine("sudo cat > " + Name + ".robot ");

            ////   proc.StandardInput.WriteLine("i");
            //proc.StandardInput.WriteLine("*** Settings ***");
            //proc.StandardInput.WriteLine("Documentation     test");
            //proc.StandardInput.WriteLine("Suite Setup   Connect To Database   pymysql   ${DBName}  ${DBUser}  ${DBPass}  ${DBHost}  ${DBPort}");
            //proc.StandardInput.WriteLine("Suite Teardown   Disconnect From Database");
            //proc.StandardInput.WriteLine("Resource          " + SuiteName + "Main.robot");
            //proc.StandardInput.WriteLine("*** Test Cases ***");
            //proc.StandardInput.WriteLine("" + Name + "");
            //proc.StandardInput.WriteLine("      " + Except + "     " + demo + "");

            //proc.StandardInput.Flush();
            //proc.StandardInput.Close();
            //proc.WaitForExit();
            //string Error = proc.StandardOutput.ReadToEnd();
            //Console.WriteLine(proc.StandardOutput.ReadToEnd());
            //System.Diagnostics.Debug.WriteLine(proc.StandardOutput.ReadToEnd());


        }

        public string Latestgrafana()
        {
            var header = this.Request.Headers.ToString();
            var header1 = this.Request.Headers.ToList();
            var Auth = (string)this.Request.Headers["Authorization"];
            if (Auth != "" && Auth != null && Auth != "max-age=0")
            {
                TimeZoneInfo timeZoneInfo = TZConvert.GetTimeZoneInfo("Europe/Stockholm");
                //    TimeZoneInfo timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
                //  TimeZoneInfo timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Etc/UTC");
                DateTime dtCNow = TimeZoneInfo.ConvertTime(Convert.ToDateTime(DateTime.Now), timeZoneInfo);
                string dtCNowdate = Convert.ToDateTime(dtCNow).ToString("yyyy-MM-dd");
                TimeSpan tsnow = Convert.ToDateTime(dtCNow).TimeOfDay;
                DateTime currentdatetime = Convert.ToDateTime(dtCNowdate + " " + (tsnow.Hours) + ":" + tsnow.Minutes);
                var resultToken = db.TokenValue.First(b => b.TockenId == Auth && b.Validto >= currentdatetime);
                if (resultToken != null)
                {
                    var report = db.Exceute.OrderByDescending(x => x.ExecuteId).Take(1);

                    return report.FirstOrDefault().SuiteName;
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

        public String UpdateGrid(string Name, int SuiteName, string Except, string parameter, string testRule, int testcaseid,string status,string Describtion,string Exceptvalues,int connection)
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
                    var result = db.TestCases.FirstOrDefault(x => x.TestCasesId == testcaseid);
                    if (result != null)
                    {
                        //  result.SuiteId = SuiteName;
                      //  result.TestApproachid = SuiteName;
                        result.RulesId = SuiteName;
                        result.TestCaseName = Name;
                        //result.RuleId = Rules;
                        result.ExceptedResult = Except + Exceptvalues;
                        result.Status = status;
                        result.Description = Describtion;
                        result.Testdata = parameter;
                        result.Expextedparameter = Exceptvalues;
                        db.SaveChanges();

                        var rulescheck = db.Rules.FirstOrDefault(x => x.RulesId == result.RulesId);
                        if(rulescheck != null)
                        {
                            rulescheck.DbConfigId = connection;
                            db.SaveChanges();
                        }

                        db.TestCaseParameters.RemoveRange(db.TestCaseParameters.Where(X=>X.TestCasesId == result.TestCasesId));

                        var para = parameter.Split(",,,");
                        for (int i = 0; i < para.Length; i++)
                        {
                            TestCaseParameters ins = new TestCaseParameters();
                            ins.ParameterName = para[i];
                            ins.TestCasesId = testcaseid;
                            db.TestCaseParameters.Add(ins);
                            db.SaveChanges();
                        }
                    }
                    var a = "";
                    return a;
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
            //proc.StandardInput.WriteLine("cd /srv/www/XAutomate/" + SuiteName + "");

            //proc.StandardInput.WriteLine("rm -r  " + Rules + ".robot");
            //// proc.StandardInput.WriteLine("touch " + Name + ".robot ");
            //proc.StandardInput.WriteLine("sudo cat > " + Rules + ".robot ");

            ////   proc.StandardInput.WriteLine("i");
            //proc.StandardInput.WriteLine("*** Settings ***");
            //proc.StandardInput.WriteLine("Documentation     test");
            //proc.StandardInput.WriteLine("Suite Setup   Connect To Database   pymysql   ${DBName}  ${DBUser}  ${DBPass}  ${DBHost}  ${DBPort}");
            //proc.StandardInput.WriteLine("Suite Teardown   Disconnect From Database");
            //proc.StandardInput.WriteLine("Resource          " + SuiteName + "Main.robot");
            //proc.StandardInput.WriteLine("*** Test Cases ***");
            ////  proc.StandardInput.WriteLine("" + description + "");
            //proc.StandardInput.Flush();
            //proc.StandardInput.Close();
            //proc.WaitForExit();
            //string Error = proc.StandardOutput.ReadToEnd();
            //Console.WriteLine(proc.StandardOutput.ReadToEnd());
            //System.Diagnostics.Debug.WriteLine(proc.StandardOutput.ReadToEnd());
            ///Test
            ///





            //var list = (from b in db.Rules
            //            join c in db.TestCases on b.SuiteName equals c.SuiteId
            //            where c.SuiteId == SuiteName
            //            select new
            //            {
            //                b.RuleCondtion,
            //                c.Testdata,
            //                c.ExceptedResult,
            //                c.SuiteId
            //            }).ToList();

            ////Test
            ///



        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


     
        private static string GetCellValue(ICell cell)
        {


            if (cell == null)
                return string.Empty;
            switch (cell.CellType)
            {
                case CellType.Blank:
                    return string.Empty;
                case CellType.Boolean:
                    return cell.BooleanCellValue.ToString();
                case CellType.Error:
                    return cell.ErrorCellValue.ToString();
                case CellType.Numeric:
                case CellType.Unknown:
                default:
                    return cell.ToString();//This is a trick to get the correct value of the cell. NumericCellValue will return a numeric value no matter the cell value is a date or a number
                case CellType.String:
                    return cell.StringCellValue;
                case CellType.Formula:
                    try
                    {
                        var e = new HSSFFormulaEvaluator(cell.Sheet.Workbook);
                        e.EvaluateInCell(cell);
                        return cell.ToString();
                    }
                    catch
                    {
                        return cell.NumericCellValue.ToString();
                    }
            }
        }

        [HttpPost]
        public async Task<IActionResult> TestCASES(List<IFormFile> files)
        {
            try
            {
                var ProductId = "";
                var Rules = "";
                foreach (var file in files)
                {
                    var basePath = Path.Combine(Directory.GetCurrentDirectory() + "\\Files\\");
                    bool basePathExists = System.IO.Directory.Exists(basePath);
                    if (!basePathExists) Directory.CreateDirectory(basePath);
                    var fileName = Path.GetFileNameWithoutExtension(file.FileName);
                    var filePath = Path.Combine(basePath, file.FileName);
                    var extension = Path.GetExtension(file.FileName);
                    if (!System.IO.File.Exists(filePath))
                    {
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }
                        var fileModel = new FileonSystem
                        {
                            CreatedOn = DateTime.UtcNow,
                            FileType = file.ContentType,
                            Extension = extension,
                            Name = fileName,
                            FilePath = filePath
                        };
                        //context.FilesOnFileSystem.Add(fileModel);
                        //context.SaveChanges();
                    }
                    else
                    {
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }
                    }
                    var fullPathImport = System.IO.Path.Combine(filePath);

                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                    FileStream excelStream = new FileStream(filePath, FileMode.Open);
                    var table = new DataTable();
                    var book = new XSSFWorkbook(excelStream);
                    excelStream.Close();
                    var sheetname = book.GetSheetAt(0).SheetName;
                    var sheet = book.GetSheetAt(0);
                    var headerRow = sheet.GetRow(0);//
                    var cellCount = headerRow.LastCellNum;//LastCellNum = PhysicalNumberOfCells
                    var rowCount = sheet.LastRowNum;//LastRowNum = PhysicalNumberOfRows - 1

                    int i = 0;
                    int columncount = 0;
                    //header
                    for (int i1 = headerRow.FirstCellNum; i1 < cellCount; i1++)
                    {
                        var column = new DataColumn(headerRow.GetCell(i1).StringCellValue);
                        columncount += 1;
                        table.Columns.Add(column);
                    }

                    //body
                    for (var i2 = sheet.FirstRowNum + 1; i2 <= rowCount; i2++)
                    {
                        var row = sheet.GetRow(i2);
                        var dataRow = table.NewRow();
                        if (row != null)
                        {
                            for (int j = row.FirstCellNum; j < cellCount; j++)
                            {
                                if (row.GetCell(j) != null)
                                    dataRow[j] = GetCellValue(row.GetCell(j));
                            }
                        }
                        table.Rows.Add(dataRow);
                    }
                    var Testdata = "";
                    var ExceptedResult = "";
                    var Descriptio = "";
                    var ExceptedResultparameter = "";
                    //  columncount = columncount - 2;
                    var name = "";
                    int d= 0;

                    foreach (DataRow row in table.Rows)
                    {

                        Rules = row["Rule Name"].ToString();

                        if (ProductId == null || ProductId == "")
                        {
                            ProductId = row["Test Approach"].ToString();
                            name = row["TestCae Name"].ToString();
                            //Process proc = new Process();
                            //proc.StartInfo.FileName = @"/bin/bash";
                            //proc.StartInfo.CreateNoWindow = true;
                            //proc.StartInfo.RedirectStandardInput = true;
                            //proc.StartInfo.RedirectStandardOutput = true;
                            //proc.StartInfo.UseShellExecute = false;
                            //proc.StartInfo.CreateNoWindow = true;
                            //proc.Start();
                            //proc.StandardInput.WriteLine("cd /srv/www/XAutomate/" + ProductId + "");
                            //// proc.StandardInput.WriteLine("touch " + Name + ".robot ");
                            //proc.StandardInput.WriteLine("sudo cat > " + Rules + ".robot ");

                            ////   proc.StandardInput.WriteLine("i");
                            //proc.StandardInput.WriteLine("*** Settings ***");
                            //proc.StandardInput.WriteLine("Documentation     test");
                            //proc.StandardInput.WriteLine("Suite Setup   Connect To Database   pymysql   ${DBName}  ${DBUser}  ${DBPass}  ${DBHost}  ${DBPort}");
                            //proc.StandardInput.WriteLine("Suite Teardown   Disconnect From Database");
                            //proc.StandardInput.WriteLine("Resource          " + ProductId + "Main.robot");
                            //proc.StandardInput.WriteLine("*** Test Cases ***");
                            ////  proc.StandardInput.WriteLine("" + description + "");
                            //proc.StandardInput.Flush();
                            //proc.StandardInput.Close();
                            //proc.WaitForExit();
                            //string Error = proc.StandardOutput.ReadToEnd();
                            //Console.WriteLine(proc.StandardOutput.ReadToEnd());
                            //System.Diagnostics.Debug.WriteLine(proc.StandardOutput.ReadToEnd());

                        }
                        Testdata = "";

                        for (int k = 3; k < columncount; k++)
                        {

                            if (k == columncount - 3)
                            {
                                ExceptedResult = row["Expected Result"].ToString();
                      //          break;

                            }
                            if (k == columncount - 2)
                            {
                                ExceptedResultparameter = row["Expected Value"].ToString();
                        //        break;

                            }
                            if(k == columncount - 1)
                            {
                                Descriptio = row["Description"].ToString();
                            }
                            if (k != columncount - 2)
                            {
                                if(k != columncount - 3)
                                {
                                    if (k != columncount - 1)
                                    {
                                        if (Testdata == null || Testdata == "")
                                        {
                                            Testdata = row[k].ToString();
                                        }
                                        else
                                        {
                                            Testdata += ",,," + row[k].ToString();

                                        }
                                    }
                                }
                              
                            
                            }



                        }

                       
                        var testapproachid = db.TestApproach.FirstOrDefault(x => x.TestApproachName == ProductId).TestApproachid;

                        var RulesValue = db.Rules.FirstOrDefault(x => x.RuleName == Rules && x.TestApproachid == testapproachid);
                        if (RulesValue != null)
                        {
                            if (d == 0)
                            {
                                db.TestCases.RemoveRange(db.TestCases.Where(x => x.RulesId == RulesValue.RulesId));
                                db.TestCaseParameters.RemoveRange(db.TestCaseParameters.Where(x => x.TestCases.RulesId == RulesValue.RulesId));
                                d++;
                            }

                            TestCases Is = new TestCases();
                            //  Is.SuiteId = ProductId;
                            Is.TestApproachid = RulesValue.TestApproachid;
                            Is.RulesId = RulesValue.RulesId;
                            Is.TestCaseName = name;
                            //Is.RuleId = Rules;
                            Is.ExceptedResult = ExceptedResult;
                            Is.Description = Descriptio;
                            Is.Expextedparameter = ExceptedResultparameter;
                            Is.Testdata = Testdata;
                            Is.Status = "1";
                            db.TestCases.Add(Is);
                            db.SaveChanges();

                            var Parameter = Testdata.Split(",,,");
                            var demo = RulesValue.RuleCondtion;
                            for (int L = 0; L < Parameter.Length; L++)
                            {
                                demo = demo.Replace("[" + L + "]", "" + Parameter[L] + "");
                            }


                            //Process proc1 = new Process();
                            //proc1.StartInfo.FileName = @"/bin/bash";
                            //proc1.StartInfo.CreateNoWindow = true;
                            //proc1.StartInfo.RedirectStandardInput = true;
                            //proc1.StartInfo.RedirectStandardOutput = true;
                            //proc1.StartInfo.UseShellExecute = false;
                            //proc1.StartInfo.CreateNoWindow = true;
                            //proc1.Start();
                            //proc1.StandardInput.WriteLine("cd /srv/www/XAutomate/" + ProductId + "");
                            //// proc.StandardInput.WriteLine("touch " + Name + ".robot ");
                            //proc1.StandardInput.WriteLine("sudo cat >> " + Rules + ".robot ");

                            //   proc.StandardInput.WriteLine("i");
                            //proc1.StandardInput.WriteLine("*** Settings ***");
                            //proc1.StandardInput.WriteLine("Documentation     test");
                            //proc1.StandardInput.WriteLine("Suite Setup   Connect To Database   pymysql   ${DBName}  ${DBUser}  ${DBPass}  ${DBHost}  ${DBPort}");
                            //proc1.StandardInput.WriteLine("Suite Teardown   Disconnect From Database");
                            //proc1.StandardInput.WriteLine("Resource          " + ProductId + "Main.robot");
                            //   proc1.StandardInput.WriteLine("*** Test Cases ***");
                            //proc1.StandardInput.WriteLine("" + Rules + "");
                            //proc1.StandardInput.WriteLine("      " + ExceptedResult + "     " + demo + "");

                            //proc1.StandardInput.Flush();
                            //proc1.StandardInput.Close();
                            //proc1.WaitForExit();
                            //string Error1 = proc1.StandardOutput.ReadToEnd();
                            //Console.WriteLine(proc1.StandardOutput.ReadToEnd());
                            //System.Diagnostics.Debug.WriteLine(proc1.StandardOutput.ReadToEnd());

                        }

                    }




                }
                TempData["Message"] = "File successfully uploaded to File System.";
                return RedirectToAction("Dbetl");
            }

            catch (Exception ex)
            {
                return Json("Please Enter Valid Files");
            }
        }

        public ActionResult MenuExample()
        {
            return View();
        }

        public ActionResult TestSuite()
        {
            return View();
        }
        public ActionResult Release()
        {
            return View();
        }

        public JsonResult ReleaseBidGrid(string Statuss)
        {
            try
            {
                var header = this.Request.Headers.ToString();
                var header1 = this.Request.Headers.ToList();
                var Auth = (string)this.Request.Headers["Authorization"];
                if (Auth != "" && Auth != null && Auth != "max-age=0")
                {
                    TimeZoneInfo timeZoneInfo = TZConvert.GetTimeZoneInfo("Europe/Stockholm");
                    //       TimeZoneInfo timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
                    //  TimeZoneInfo timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Etc/UTC");
                    DateTime dtCNow = TimeZoneInfo.ConvertTime(Convert.ToDateTime(DateTime.Now), timeZoneInfo);
                    string dtCNowdate = Convert.ToDateTime(dtCNow).ToString("yyyy-MM-dd");
                    TimeSpan tsnow = Convert.ToDateTime(dtCNow).TimeOfDay;
                    DateTime currentdatetime = Convert.ToDateTime(dtCNowdate + " " + (tsnow.Hours) + ":" + tsnow.Minutes);
                    var resultToken = db.TokenValue.First(b => b.TockenId == Auth && b.Validto >= currentdatetime);
                    if (resultToken != null)
                    {
                        var b = from a in db.ReleaseNo
                                where a.Status == Statuss
                                select new
                                {
                                    a.ReleasenoId,
                                    a.ReleaseNo1,
                                    a.ReleaseName,
                                    a.Status,
                                    a.CreatedDate,
                                };
                        return Json(b);
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
            catch (Exception ex)
            {
                return null;
            }
        }
        public JsonResult SearchRelease(string search,string status)
        {
            try
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
                        if (search == "" || search == null || search == "undefiened")
                        {
                            var b = from a in db.ReleaseNo
                                    where a.Status == status
                                    select new
                                    {
                                        a.ReleasenoId,
                                        a.ReleaseNo1,
                                        a.ReleaseName,
                                        a.Status,
                                        a.CreatedDate,
                                    };
                            return Json(b);
                        }
                        else
                        {
                            var b = from a in db.ReleaseNo
                                    where a.Status == status && a.ReleaseName.Contains(search) || a.ReleaseNo1.Contains(search)
                                    select new
                                    {
                                        a.ReleasenoId,
                                        a.ReleaseNo1,
                                        a.ReleaseName,
                                        a.Status,
                                        a.CreatedDate,
                                    };
                            return Json(b);

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
            catch (Exception ex)
            {
                return null;
            }
        }


        public string SaveRelease(string RNo,string RName,string Status)
        {
            try
            {
                var header = this.Request.Headers.ToString();
                var header1 = this.Request.Headers.ToList();
                 var Auth= (string)this.Request.Headers["Authorization"];
                if (Auth != "" && Auth != null && Auth != "max-age=0")
                {
                    TimeZoneInfo timeZoneInfo = TZConvert.GetTimeZoneInfo("Europe/Stockholm");
                    //       TimeZoneInfo timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Etc/UTC");
                    // TimeZoneInfo timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
                    DateTime dtCNow = TimeZoneInfo.ConvertTime(Convert.ToDateTime(DateTime.Now), timeZoneInfo);
                    string dtCNowdate = Convert.ToDateTime(dtCNow).ToString("yyyy-MM-dd");
                    TimeSpan tsnow = Convert.ToDateTime(dtCNow).TimeOfDay;
                    DateTime currentdatetime = Convert.ToDateTime(dtCNowdate + " " + (tsnow.Hours) + ":" + tsnow.Minutes);
                    var resultToken = db.TokenValue.First(b => b.TockenId == Auth && b.Validto >= currentdatetime);
                    if (resultToken != null)
                    {
                        var restult = db.ReleaseNo.FirstOrDefault(x => x.ReleaseNo1 == RNo && x.ReleaseName == RName && x.Status == Status);
                        if (restult == null)
                        {
                            ReleaseNo Ins = new ReleaseNo();
                            Ins.ReleaseNo1 = RNo;
                            Ins.ReleaseName = RName;
                            Ins.CreatedDate = DateTime.Now;
                            Ins.Status = Status;
                            db.ReleaseNo.Add(Ins);
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
            catch(Exception ex)
            {
                return "Connection";
            }
        }

        public string Forgotpassword(string Emailid)
        {
            try
            {
                var result = db.Login.FirstOrDefault(x => x.EmailId == Emailid && x.Status == 1);
                if (result == null)
                {
                    return "Valid Email";
                }
                else
                {
                    MailMessage mailMsg = new MailMessage();
                    // From
                    MailAddress mailAddress = new MailAddress(Emailid, "Forgot Password");

                    mailMsg.From = mailAddress;

                    // Subject and Body
                    mailMsg.Subject = "Forgot Password";
                    string messegeitem = string.Empty;

                    messegeitem = "<html> ";
                    messegeitem += "<head> ";
                    messegeitem += "<title>Forgot Password</title>";
                    messegeitem += "</head>";
                    messegeitem += "<body style = \"padding:0px;margin:0px\" >";
                    messegeitem += "<div style = \"background-color:#f2f2f2;height:100%\" >";
                    messegeitem += "<table width = \"100%\" align = \"center\" cellpadding = \"0\" cellspacing = \"0\" border = \"0\" > ";
                    messegeitem += "<tbody>";
                    messegeitem += "<tr style = \"background-color:#232f3e\" >";
                    messegeitem += "<td align = \"center\" valign = \"top\" style = \"padding:0 20px\" >";
                    messegeitem += "<table align = \"center\" cellpadding = \"0\" cellspacing = \"0\" border = \"0\" >";
                    messegeitem += "<tbody><tr> ";
                    messegeitem += "<td width = \"320\" align = \"center\" valign = \"top\" >";
                    messegeitem += "<table width = \"100%\" align = \"center\" cellpadding = \"0\" cellspacing = \"0\" border = \"0\" >";
                    messegeitem += "<tbody> ";
                    messegeitem += "<tr> ";
                    messegeitem += "<td height = \"30\" style = \"height:30px;line-height:30px;text-align:center\" > ";
                    //messegeitem += "<img align = \"center\" border = \"0\"  Width=300px\" alt = \"Sysmedac Ecom\"  src =\"" + vCompanyDetails[0].InvoiceLogo + "\" style = \"margin:audio\" ><br><h2 style = \"margin:0px;font-family:Arial,sans-serif;font-size:20px;color:#3a3a3a;\" > </h2>";
                    messegeitem += "</td> </tr>";
                    messegeitem += " <tr> ";
                    messegeitem += " <td> </td> ";
                    messegeitem += " </tr>";
                    messegeitem += " </tbody> ";
                    messegeitem += " </table> ";
                    messegeitem += " </td> ";
                    messegeitem += " </tr> ";
                    messegeitem += " </tbody> ";
                    messegeitem += " </table> ";
                    messegeitem += " </td> ";
                    messegeitem += " </tr> ";
                    messegeitem += " </tbody>";
                    messegeitem += "</table>";
                    messegeitem += "<table width = \"76%\" align = \"center\" cellpadding =\"0\" cellspacing = \"0\" border = \"0\" style = \"background-color:#fff;box-shadow:0px 0px 5px #ccc;border-left:5px\">";
                    messegeitem += "<tbody> ";
                    messegeitem += "<tr> ";
                    messegeitem += "<td align = \"left\" valign = \"top\" style = \"padding:3%\" > ";
                    messegeitem += "<p style = \"font -family:Arial,sans-serif;text-align:left;font-size:17px;color:#3a3a3a;padding:15px\" > Hi " +result.UserName + "</p>";
                    messegeitem += "<table width = \"100%\" style = \"font-family:Arial,sans-serif;text-align:left;font-size:15px;color:#3a3a3a;padding:15px\">";
                    messegeitem += "<tr> ";
                    messegeitem += "<td width = \"100%\">Greetings from XAutomate!</td><td></td>";
                    messegeitem += "</tr>";
                  
                    messegeitem += "<tr> ";
                    messegeitem += "<td width = \"100%\">Your Password: " + result.Password + "</td><td></td>";
                    messegeitem += "</tr>";
                    messegeitem += "<tr> ";
                    messegeitem += "<td width = \"100%\"></td><td></td>";
                    messegeitem += "</tr>";
                    messegeitem += "<tr> ";
                    messegeitem += "</tr>";
                    messegeitem += "<tr> ";
                    messegeitem += "<td width = \"100%\"></td><td></td>";
                    messegeitem += "</tr>";
                    messegeitem += "<tr> ";
                 
                    messegeitem += "</tr>";
                    messegeitem += "</table>";
                    messegeitem += "<p style = \"font -family:Arial,sans-serif;text-align:left;font-size:15px;color:#3a3a3a;padding:15px\" >  Regards, <br/>Team XAutomate.</p> ";
                    messegeitem += "<p style = \"text -align:left;font-family:Arial,sans-serif;color:#2f2f2f\" > ";
                    messegeitem += "</p><p style = \"text -align:justify;font-family:Arial,sans-serif;padding:15px;color:grey\" >";
                    messegeitem += "<small> CONFIDENTIALITY NOTICE:<br> ";
                    messegeitem += "Proprietary / Confidential information belonging to Xautomate and its affiliates";
                    messegeitem += "may be contained in this message.If you are not a recipient indicated or intended in this message(or responsible for delivery of this message to such person), or you think for any reason that this message may have been addressed to you in error, you may not use or copy or deliver this message to anyone else.In such case, you should destroy this message and are asked to notify the sender by reply email.</small>";
                    messegeitem += "</p> ";
                    messegeitem += "<p></p> ";
                    messegeitem += "</td> ";
                    messegeitem += "</tr>";
                    messegeitem += "</tbody>";
                    messegeitem += "</table>";
                    messegeitem += "<table width = \"100%\" align = \"center\" cellpadding = \"0\" cellspacing = \"0\" border = \"0\" >";
                    messegeitem += "<tbody>";
                    messegeitem += "<tr>";
                    messegeitem += "<td><table width = \"40%\" align = \"center\" cellpadding = \"0\" cellspacing = \"0\" border = \"0\" >";
                    messegeitem += "<tbody><tr> ";
                    messegeitem += "<td colspan = \"3\" style = \"padding:10px\" >";
                    messegeitem += "</td>";
                    messegeitem += "</tr>";
                    messegeitem += "</tbody></table> </td>";
                    messegeitem += "</tr>";
                    messegeitem += "<tr>";
                    messegeitem += "<td height = \"12\" style = \"height:12px;line-height:12px\" ></td>";
                    messegeitem += "</tr>";
                    messegeitem += "<tr>";
                    messegeitem += "<td align = \"center\" valign = \"top\" style = \"color:#777777;font-family:Arial,sans-serif;font-size:12px;line-height:23px;font-weight:400\" > Copyright© -2020  Sysmedac Pvt Lmt.All Rights Reserved.";
                    messegeitem += "</td>";
                    messegeitem += "</tr>";
                    messegeitem += "<tr>";
                    messegeitem += "<td height = \"12\" style = \"height:12px;line-height:12px\" ></td> ";
                    messegeitem += "</tr> ";
                    messegeitem += "</tbody>";
                    messegeitem += "</table><div class=\"yj6qo\"></div><div class=\"adL\">";
                    messegeitem += "</div></div>";
                    messegeitem += " </body>";
                    messegeitem += "</html>";

                    mailMsg.Body = messegeitem;
                    mailMsg.To.Add(Emailid);
                    mailMsg.CC.Add("abinesh@sysmedac.com");

                    mailMsg.IsBodyHtml = true;
                    SmtpClient emailClient = new SmtpClient("send.one.com", 465);
                    System.Net.NetworkCredential credentials = new System.Net.NetworkCredential("accountadmin@xautomate.se", "Xautomate@123");
                    emailClient.Credentials = credentials;
                    try
                    {
                        emailClient.Send(mailMsg);
                    }
                    catch (Exception ex)
                    {
                        string register = ex.ToString();
                    }

                }
            

                return "Success";
            }
            catch(Exception ex)
            {
                return "Fail";
            }
        }

        public ActionResult Menutest()
        {
            return View();
        }

    }



}
