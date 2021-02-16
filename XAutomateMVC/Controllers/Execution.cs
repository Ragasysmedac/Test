using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;
using TimeZoneConverter;
using XAutomateMVC.Models;
using XAutomateMVC.Models.DBModels;

namespace XAutomateMVC.Controllers
{
    public class Execution : Controller
    {
        private readonly ILogger<Execution> _logger;

        db_mateContext db = new db_mateContext();
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Execute()
        {
            var TestApproach = (from product in db.TestSuite
                                where product.Status =="1"
                                select new SelectListItem()
                                {
                                    Text = product.TestSuitename,
                                    Value = product.TestSuitename.ToString(),
                                }).ToList();

            //TestApproach.Insert(0, new SelectListItem()
            //{
            //    Text = "----Select----",
            //    Value = string.Empty
            //});

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
            var releaseno = (from product in db.ReleaseNo
                             where product.Status == "1"
                             select new SelectListItem()
                             {
                                 Text = product.ReleaseNo1,
                                 Value = product.ReleaseNo1.ToString(),
                             }).ToList();

            releaseno.Insert(0, new SelectListItem()
            {
                Text = "----Select----",
                Value = string.Empty
            });
            ProductViewModel productViewModel = new ProductViewModel();
            productViewModel.Listofproducts = TestApproach;
            productViewModel.ReleaseList = releaseno;
            productViewModel.TestSuiteList = SuiteList;
            return View(productViewModel);
        }
        public JsonResult testapproach(string SuiteId)
        {
            var header = this.Request.Headers.ToString();
            var header1 = this.Request.Headers.ToList();
             var Auth= (string)this.Request.Headers["Authorization"];
            if (Auth != "" && Auth != null && Auth != "max-age=0")
            {
                TimeZoneInfo timeZoneInfo = TZConvert.GetTimeZoneInfo("Europe/Stockholm");
                //   TimeZoneInfo timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
                //      TimeZoneInfo timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Etc/UTC");
                DateTime dtCNow = TimeZoneInfo.ConvertTime(Convert.ToDateTime(DateTime.Now), timeZoneInfo);
                string dtCNowdate = Convert.ToDateTime(dtCNow).ToString("yyyy-MM-dd");
                TimeSpan tsnow = Convert.ToDateTime(dtCNow).TimeOfDay;
                DateTime currentdatetime = Convert.ToDateTime(dtCNowdate + " " + (tsnow.Hours) + ":" + tsnow.Minutes);
                var resultToken = db.TokenValue.First(b => b.TockenId == Auth && b.Validto >= currentdatetime);
                if (resultToken != null)
                {

                    var a = from b in db.TestApproach
                            where b.SuiteIds.Contains(SuiteId) && b.Status=="1"
                            select new
                            {
                                b.TestApproachid,

                                b.TestApproachName,
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

        public JsonResult Trigger(string SuiteId, string tagno, string approachname,int dbconnection,string releaseno)
        {
            var testcaseresult = "";
            string[] testapproachcheck;
            List<int> ruleadd = new List<int>();
            try
            {
                int MY = 0;
                int SQ = 0;
                List<Execute> list1 = new List<Execute>();
                var header = this.Request.Headers.ToString();
                var header1 = this.Request.Headers.ToList();
                 var Auth= (string)this.Request.Headers["Authorization"];
                if (Auth != "" && Auth != null && Auth != "max-age=0")
                {
                    TimeZoneInfo timeZoneInfo = TZConvert.GetTimeZoneInfo("Europe/Stockholm");
                    //    TimeZoneInfo timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
                    //   TimeZoneInfo timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Etc/UTC");
                    DateTime dtCNow = TimeZoneInfo.ConvertTime(Convert.ToDateTime(DateTime.Now), timeZoneInfo);
                    string dtCNowdate = Convert.ToDateTime(dtCNow).ToString("yyyy-MM-dd");
                    TimeSpan tsnow = Convert.ToDateTime(dtCNow).TimeOfDay;
                    DateTime currentdatetime = Convert.ToDateTime(dtCNowdate + " " + (tsnow.Hours) + ":" + tsnow.Minutes);
                    var resultToken = db.TokenValue.First(b => b.TockenId == Auth && b.Validto >= currentdatetime);
                    if (resultToken != null)
                    {
                        var result = "";
                        var suiteId = SuiteId.Split(',');
                        //var dbconfig1 = dbconnection.Split(',');
                        var approach = approachname.Split(',');
                        Process proc = new Process();
                        proc.StartInfo.FileName = @"/bin/bash";

                        proc.StartInfo.CreateNoWindow = true;
                        proc.StartInfo.RedirectStandardInput = true;
                        proc.StartInfo.RedirectStandardOutput = true;
                        proc.StartInfo.UseShellExecute = false;
                        proc.StartInfo.CreateNoWindow = true;
                        proc.Start();

                        proc.StandardInput.WriteLine("cd  /srv/www/XAutomate/");
                        proc.StandardInput.WriteLine("rm -Rf  " + releaseno + "");
                        proc.StandardInput.WriteLine("mkdir  " + releaseno + " ");
                        proc.StandardInput.Flush();
                        proc.StandardInput.Close();
                        proc.WaitForExit();
                        string Error = proc.StandardOutput.ReadToEnd();
                        Console.WriteLine(proc.StandardOutput.ReadToEnd());
                        System.Diagnostics.Debug.WriteLine(proc.StandardOutput.ReadToEnd());
                        for (int i = 0; i < suiteId.Length; i++)
                        {
                            //int suiteid = Convert.ToInt32(suiteId[i]);
                            //   int dbconfigid = Convert.ToInt32(dbconfig1[0]);





                            //var testapproach = (from c in db.TestApproach where c.SuiteIds.Contains(suiteId[i]) && c.Status == "1" select new { c.TestApproachName, c.TestApproachid,c.Dbconfigid }).ToList();
                            var approach1 = approachname.Split(',');

                            //      var suiteapproach = db.TestApproach.FirstOrDefault(x => x.SuiteIds.Contains(suiteId[i]));
                            // foreach (var item2 in testapproach)
                            // {

                            for (int k = 0; k < approach1.Length; k++)
                            {

                                //var testapproach1 = Convert.ToString(item2.TestApproachid);
                                //if (testapproach1 == approach1[k])
                                // {

                                //       proc.StartInfo.FileName = @"/bin/bash";
                                //       proc.StartInfo.CreateNoWindow = true;
                                //       proc.StartInfo.RedirectStandardInput = true;
                                //       proc.StartInfo.RedirectStandardOutput = true;
                                //       proc.StartInfo.UseShellExecute = false;
                                //       proc.StartInfo.CreateNoWindow = true;
                                //       proc.Start();
                                //       proc.StandardInput.WriteLine("cd /srv/www/XAutomate/" + releaseno + "");
                                //       // proc.StandardInput.WriteLine("touch " + Name + ".robot ");
                                //       proc.StandardInput.WriteLine("cat >> " + item2.TestApproachName + ".robot ");

                                //       //    proc.StandardInput.WriteLine("i");
                                //       proc.StandardInput.WriteLine("*** Settings ***");
                                //       proc.StandardInput.WriteLine("Documentation     test");
                                //       proc.StandardInput.WriteLine("Force Tags    " + item2.TestApproachName + "");
                                //       proc.StandardInput.WriteLine("Suite Setup   Connect To Database   pymysql   ${DBName}  ${DBUser}  ${DBPass}  ${DBHost}  ${DBPort}");
                                //       proc.StandardInput.WriteLine("Suite Teardown   Disconnect From Database");
                                //       proc.StandardInput.WriteLine("Resource          " + result + "Main.robot");
                                ////       proc.StandardInput.WriteLine("*** Test Cases ***");
                                //       proc.StandardInput.Flush();
                                //       proc.StandardInput.Close();
                                //       Console.WriteLine(proc.StandardOutput.ReadToEnd());
                                //       System.Diagnostics.Debug.WriteLine(proc.StandardOutput.ReadToEnd());

                                //    proc.WaitForExit();
                                var rules = (from b in db.Rules
                                             where b.TestApproachName.Contains(approach1[k]) && b.Status == "1"
                                             select new
                                             {
                                                 b.RuleName,
                                                 b.RulesId,
                                                 b.RuleParameter,
                                                 b.RuleCondtion,
                                                 b.Description,
                                                 b.DbConfigId,
                                             }).ToList();

                                if (rules.Count() == 0)
                                {
                                    return Json(approach1[k] + "Rules Not Available");
                                }
                                foreach (var ruleitem in rules)
                                {
                                    if (ruleadd.Count() == 0)
                                    {
                                        ruleadd.Add((int)ruleitem.RulesId);
                                    }
                                    else if (!ruleadd.Contains((int)ruleitem.RulesId))
                                    {
                                        ruleadd.Add((int)ruleitem.RulesId);
                                    }

                                }
                                foreach (var item in rules)
                                {
                                    var connections = db.DbConfig.FirstOrDefault(x => x.Dbconfigid == dbconnection);

                                    if (connections.DatabaseType == "MYSQL")
                                    {
                                        if (MY == 0)
                                        {
                                            //result = db.TestSuite.FirstOrDefault(x => x.TestSuiteId == suiteid).TestSuitename;
                                            proc.StartInfo.FileName = @"/bin/bash";
                                            proc.StartInfo.CreateNoWindow = true;
                                            proc.StartInfo.RedirectStandardInput = true;
                                            proc.StartInfo.RedirectStandardOutput = true;
                                            proc.StartInfo.UseShellExecute = false;
                                            proc.StartInfo.CreateNoWindow = true;
                                            proc.Start();


                                            proc.StandardInput.WriteLine("cd  /srv/www/XAutomate/" + releaseno + "");

                                            proc.StandardInput.WriteLine("cat >  " + result + "Main.robot ");
                                            proc.StandardInput.WriteLine("*** Settings ***");
                                            proc.StandardInput.WriteLine("Library      ListenerLibrary");
                                            proc.StandardInput.WriteLine("Library  OperatingSystem");
                                            proc.StandardInput.WriteLine("Library   DatabaseLibrary");

                                            proc.StandardInput.Flush();
                                            proc.StandardInput.Close();
                                            proc.WaitForExit();
                                            Console.WriteLine(proc.StandardOutput.ReadToEnd());
                                            System.Diagnostics.Debug.WriteLine(proc.StandardOutput.ReadToEnd());
                                            MY++;
                                        }

                                    }
                                    else
                                    {
                                        if (connections.DatabaseType == "SQL")
                                        {
                                            if (SQ == 0)
                                            {
                                                //result = db.TestSuite.FirstOrDefault(x => x.TestSuiteId == suiteId[i]).TestSuitename;
                                                proc.StartInfo.FileName = @"/bin/bash";
                                                proc.StartInfo.CreateNoWindow = true;
                                                proc.StartInfo.RedirectStandardInput = true;
                                                proc.StartInfo.RedirectStandardOutput = true;
                                                proc.StartInfo.UseShellExecute = false;
                                                proc.StartInfo.CreateNoWindow = true;
                                                proc.Start();


                                                proc.StandardInput.WriteLine("cd  /srv/www/XAutomate/" + releaseno + "");

                                                proc.StandardInput.WriteLine("cat >  " + result + "Main.robot ");
                                                proc.StandardInput.WriteLine("*** Settings ***");
                                                proc.StandardInput.WriteLine("Library      ListenerLibrary");
                                                proc.StandardInput.WriteLine("Library  OperatingSystem");
                                                proc.StandardInput.WriteLine("Library   DatabaseLibrary");

                                                proc.StandardInput.Flush();
                                                proc.StandardInput.Close();
                                                proc.WaitForExit();
                                                Console.WriteLine(proc.StandardOutput.ReadToEnd());
                                                System.Diagnostics.Debug.WriteLine(proc.StandardOutput.ReadToEnd());
                                                SQ++;
                                            }

                                        }
                                    }

                                }
                            }
                        }

                        //   }
                        foreach (var rulesfilte in ruleadd)
                        {


                            var test = rulesfilte;
                            //                    }
                            var testcase = (from b in db.TestCases
                                            where b.RulesId == rulesfilte && b.Status == "1"
                                            select new
                                            {
                                                b.Testdata,
                                                b.ExceptedResult,
                                                testcaseparam = b.TestCaseParameters.Select(x => new
                                                {
                                                    x.ParameterName,
                                                }),
                                                RuleCondtion = b.Rules.RuleCondtion,
                                                Description = b.Rules.Description,
                                            }).ToList();
                            //if (testcase.Count() == 0)
                            //{
                            //    return Json(item.RuleName + "Testcase Not Available");
                            //}
                            foreach (var item1 in testcase)
                            {
                                var connections = db.DbConfig.FirstOrDefault(x => x.Dbconfigid == dbconnection);
                                var list = item1.Testdata;
                                //var result2 = Regex.Split(list, ",(?![^()]*\\))");
                                // var Parameter = Regex.Split(list, ",,,(?![^()]*\\))");
                                var Parameter = list.Split(",,,");
                                var demo = item1.RuleCondtion;
                                var Testcase = "";
                                int h = 0;
                                for (int L = 0; L < Parameter.Length; L++)
                                {
                                    demo = demo.Replace("[" + L + "]", "" + Parameter[L] + "");
                                    if (Testcase == "")
                                    {
                                        Testcase = Parameter[L];
                                    }
                                    else if (L == 1)
                                    {
                                        Testcase += "." + Parameter[L];
                                    }
                                    else if (L == 2)
                                    {
                                        Testcase += ":" + Parameter[L];
                                    }
                                    h++;
                                }
                                proc.StartInfo.FileName = @"/bin/bash";
                                proc.StartInfo.CreateNoWindow = true;
                                proc.StartInfo.RedirectStandardInput = true;
                                proc.StartInfo.RedirectStandardOutput = true;
                                proc.StartInfo.UseShellExecute = false;
                                proc.StartInfo.CreateNoWindow = true;
                                proc.Start();
                                proc.StandardInput.WriteLine("cd /srv/www/XAutomate/" + releaseno + "");
                                // proc.StandardInput.WriteLine("touch " + Name + ".robot ");
                                proc.StandardInput.WriteLine("cat >> " + result + "Main.robot ");
                                proc.StandardInput.WriteLine("*** Variables  ***");
                                proc.StandardInput.WriteLine("${DBName}   " + connections.DbName + "");
                                proc.StandardInput.WriteLine("${DBUser}   " + connections.DbUser + "");
                                proc.StandardInput.WriteLine("${DBPass}  " + connections.DbPassword + "");
                                proc.StandardInput.WriteLine("${DBHost}  " + connections.DbHostName + "");

                                if (connections.DatabaseType == "MYSQL")
                                {
                                    proc.StandardInput.WriteLine("${DBPort}    " + connections.DbPort + "");
                                    proc.StandardInput.WriteLine("*** Test Cases ***");
                                    proc.StandardInput.WriteLine("" + item1.Description + ":" + Testcase);
                                    proc.StandardInput.WriteLine("      [Tags]   " + item1.Description);
                                    //proc.StandardInput.WriteLine("Suite Setup   Connect To Database   pymysql   ${DBName}  ${DBUser}  ${DBPass}  ${DBHost}  ${DBPort}");
                                    proc.StandardInput.WriteLine("      Connect To Database   pymysql   ${DBName}  ${DBUser}  ${DBPass}  ${DBHost}  ${DBPort}");
                                    proc.StandardInput.WriteLine("      " + item1.ExceptedResult + "     " + demo + "");
                                }
                                else
                                {
                                    proc.StandardInput.WriteLine("${DBPort}    1433");
                                    proc.StandardInput.WriteLine("*** Test Cases ***");
                                    proc.StandardInput.WriteLine("" + item1.Description + ":" + Testcase);
                                    proc.StandardInput.WriteLine("      [Tags]   " + item1.Description);
                                    //proc.StandardInput.WriteLine("Suite Setup   Connect To Database   pymysql   ${DBName}  ${DBUser}  ${DBPass}  ${DBHost}  ${DBPort}");
                                    proc.StandardInput.WriteLine("          Connect To Database Using Custom Params  pyodbc  'DRIVER={ODBC Driver 17 for SQL Server};SERVER=${DBHost};DATABASE=${DBName};UID=${DBUser};PWD=${DBPass}'");
                                    proc.StandardInput.WriteLine("      " + item1.ExceptedResult + "     " + demo + "");
                                }
                                //    proc.StandardInput.WriteLine("i");

                                proc.StandardInput.Flush();
                                proc.StandardInput.Close();
                                proc.WaitForExit();
                            }


                        }
                        // }

                 //   }
                    proc.StartInfo.FileName = @"/bin/bash";
                    //     proc.StartInfo.FileName = "cmd.exe";
                    proc.StartInfo.CreateNoWindow = true;
                    proc.StartInfo.RedirectStandardInput = true;
                    proc.StartInfo.RedirectStandardOutput = true;
                    proc.StartInfo.UseShellExecute = false;
                    proc.StartInfo.CreateNoWindow = true;
                    proc.Start();

                    proc.StandardInput.WriteLine("sudo su");
                    proc.StandardInput.WriteLine("cd  /srv/www/XAutomate");
                    //  proc.StandardInput.WriteLine("cd /d D:\\Abinesh-learn");
                    proc.StandardInput.WriteLine("robot  --removekeywords All --listener robotframework_reportportal.listener --variable RP_ENDPOINT:http://52.178.152.165:8080 --variable RP_UUID:348cf3f0-14b7-4c61-9d0a-de6937406d89 --variable RP_PROJECT:'default_personal' --variable RP_LAUNCH:'" + releaseno + "' --variable rp.attributes:one --variable rp.keystore.password:false  --variable rp.enable:false   -N  " + tagno + "  --report " + tagno + "" + releaseno + ".html   -d   /srv/www/XAutomate/wwwroot/" + releaseno + "  " + releaseno + "");
                    ///    proc.StandardInput.WriteLine("pybot --listener reportportal_listener --variable RP_ENDPOINT:http://52.178.152.165:8080 --variable RP_UUID:6df6d59c-e0f6-44b0-a8c7-5087f0f36eac --variable RP_LAUNCH:'superadmin_TEST_EXAMPLE' --variable RP_PROJECT:superadmin_personal --report " + SuiteId + "" + Version + ".html  -d  /srv/www/XAutomate/wwwroot/" + SuiteId + "  " + SuiteId + "");
                    //  proc.StandardInput.WriteLine("robot robotframework");
                    proc.StandardInput.WriteLine("python -m dbbot.run -b mysql://root:password@52.178.152.165:3309/graphana /srv/www/XAutomate/wwwroot/" + releaseno + "/output.xml");
                    proc.StandardInput.Flush();
                    proc.StandardInput.Close();
                    proc.WaitForExit();
                    string Error1 = proc.StandardOutput.ReadToEnd();
                    Console.WriteLine(proc.StandardOutput.ReadToEnd());
                    System.Diagnostics.Debug.WriteLine(proc.StandardOutput.ReadToEnd());
                    var Executetime = DateTime.Now.ToString("HH:mm");
                    MySqlConnection sql = new MySqlConnection("Server=52.178.152.165;Port=3309;Database=graphana;User ID=root;Password=password;SslMode=none");
                    sql.Open();

                    MySqlCommand com = new MySqlCommand("select  * from graphana.suite_status  join graphana.suites  on  suite_status.suite_id = suites.id where suites.name='" + tagno + "' order by suite_status.id desc Limit 1 ", sql);
                    using (var reader = com.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Executetime = reader["elapsed"].ToString();
                            list1.Add(new Execute()
                            {

                                Passed = reader["passed"].ToString(),
                                failed = reader["failed"].ToString(),
                                total_test = Convert.ToInt32(reader["passed"]) + Convert.ToInt32(reader["failed"]),
                            });
                        }
                    }
                    sql.Close();
                    var Executetime1 = Convert.ToInt32(Executetime) / 1000;
                    Exceute ins = new Exceute();
                    ins.SuiteName = tagno;
                    ins.Execute = DateTime.Now;
                    ins.Time = Executetime1 + " ms";
                    ins.Status = "1";
                    ins.ResultUrl = "/" + releaseno + "/" + tagno + "" + releaseno + ".html";
                    db.Exceute.Add(ins);
                    db.SaveChanges();
                    var Errorcount = Error1.Split(",");

                    var totaltest = Errorcount[2].Split(" ");

                    var Failtest = Errorcount[4].Split(" ");
                    return Json(list1);
                    //return "";
                    testcaseresult = totaltest[1] + " " + Errorcount[3] + " " + Failtest[0];

                
                    }
                    else
                    {
                          return Json("Auth Fail");
                        //return "";
                    }
                }
                else
                {
                    return Json("Auth Fail");
                    //return "";
                }
            }
            catch (Exception ex)
            {
                //return ex.ToString();
                return Json(ex);
            }
           
        }

        public class Suiteclass
        {
            public int Suite { get; set; }
        }

    }
}
