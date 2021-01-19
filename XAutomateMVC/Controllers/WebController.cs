using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MySql.Data.MySqlClient;
using XAutomateMVC.Models;
using XAutomateMVC.Models.DBModels;

namespace XAutomateMVC.Controllers
{
    public class WebController : Controller
    {
        db_mateContext db = new db_mateContext();
        public IActionResult Web()
        {
            var TestApproach = (from product in db.TestSuite
                                where product.Status == "1"
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
            var ResourceFile = (from product in db.WebFiles
                             where product.Status == "1" && product.FileType == "Resource"
                                select new SelectListItem()
                             {
                                 Text = product.FileName,
                                 Value = product.WebFilesId.ToString(),
                             }).ToList();

            ResourceFile.Insert(0, new SelectListItem()
            {
                Text = "----Select----",
                Value = string.Empty
            });
            var TestcaseFile = (from product in db.WebFiles
                                where product.Status == "1" && product.FileType== "Test Case"
                                select new SelectListItem()
                                {
                                    Text = product.FileName,
                                    Value = product.WebFilesId.ToString(),
                                }).ToList();

            TestcaseFile.Insert(0, new SelectListItem()
            {
                Text = "----Select----",
                Value = string.Empty
            });
            ProductViewModel productViewModel = new ProductViewModel();
            productViewModel.Listofproducts = TestApproach;
            productViewModel.TestCaseFileList = TestcaseFile;
            productViewModel.ResourceFile = ResourceFile;
            productViewModel.ReleaseList = releaseno;
            return View(productViewModel);
        }
        public JsonResult testcasename(int TestcaseId)
        {
            var header = this.Request.Headers.ToString();
            var header1 = this.Request.Headers.ToList();
            var Auth = (string)this.Request.Headers["Authorization"];
            if (Auth != "" && Auth != null && Auth != "max-age=0")
            {
               //   TimeZoneInfo timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
                TimeZoneInfo timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Etc/UTC");
                DateTime dtCNow = TimeZoneInfo.ConvertTime(Convert.ToDateTime(DateTime.Now), timeZoneInfo);
                string dtCNowdate = Convert.ToDateTime(dtCNow).ToString("yyyy-MM-dd");
                TimeSpan tsnow = Convert.ToDateTime(dtCNow).TimeOfDay;
                DateTime currentdatetime = Convert.ToDateTime(dtCNowdate + " " + (tsnow.Hours) + ":" + tsnow.Minutes);
                var resultToken = db.TokenValue.First(b => b.TockenId == Auth && b.Validto >= currentdatetime);
                if (resultToken != null)
                {

                    var a = from b in db.WebTestcases
                            where b.WebFilesId == TestcaseId
                            select new
                            {
                                b.WebTestcasesid,

                                b.Webtestcase,
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

        public async Task<IActionResult> UploadToFileSystem(List<IFormFile> files, string ProductId)
        {
            try
            {
                foreach (var file in files)
                {
                    var basePath = Path.Combine(Directory.GetCurrentDirectory() + "\\WebTestCases\\");
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
                    decimal WebFileId = 0;
                    if (filePath.ToLower().Contains("resource"))
                    {
                        var filenamecheck = db.WebFiles.FirstOrDefault(x => x.FileName == file.FileName);
                        if(filenamecheck != null)
                        {
                            filenamecheck.Status = "0";
                            db.SaveChanges();
                        }
                        WebFiles Ins = new WebFiles();
                        Ins.FileType = "Resource";
                        Ins.WebFilesPath = filePath;
                        Ins.FileName = file.FileName;
                        Ins.Status = "1";
                        db.WebFiles.Add(Ins);
                        db.SaveChanges();
                    }
                    else
                    {
                        var filenamecheck = db.WebFiles.FirstOrDefault(x => x.FileName == file.FileName);
                        if (filenamecheck != null)
                        {
                            WebFileId = filenamecheck.WebFilesId;
                            filenamecheck.Status = "0";
                            db.SaveChanges();
                        }
                        WebFiles Ins = new WebFiles();
                        Ins.FileType = "Test Case";
                        Ins.WebFilesPath = filePath;
                        Ins.FileName = file.FileName;
                        Ins.Status = "1";
                        db.WebFiles.Add(Ins);
                        db.SaveChanges();
                        var Testcase = "";
                        var fileCount = new StreamReader(filePath).ReadToEnd(); // big string
                        string[] lines = fileCount.Split(new char[] { '\r' });           // big array
                        for(int i=0; i < lines.Length; i++)
                        {
                            var test = lines[i];
                            test = test.Replace(" ","");

                            if (lines[i] == "*** Settings ***")
                            {
                               

                                WebTestcases Ins1 = new WebTestcases();
                                Ins1.WebFilesId = Ins.WebFilesId;
                                Ins1.Webtestcase = "Settings";
                                Ins1.Testcase = "S";
                                Ins1.Status = "1";
                                db.WebTestcases.Add(Ins1);
                                db.SaveChanges();
                                i = i + 1;
                                for (int j=i; j< lines.Length; j++)
                                {
                                    Testcase = lines[j].Replace("\n", "");
                                    if (Testcase == "*** Test Cases ***")
                                    {

                                        i = j - 1;
                                        break;
                                    }
                         //           j = j + 1;
                                    if(Testcase != "")
                                    {
                                        Webtestcaselist Ins2 = new Webtestcaselist();
                                        Ins2.Status = "1";
                                        Ins2.WebTestcasesid = Ins1.WebTestcasesid;
                                        Ins2.Cases = Testcase;
                                        Ins2.CaseType = "S";
                                        db.Webtestcaselist.Add(Ins2);
                                        db.SaveChanges();
                                    }
                            

                                   
                                }
                            }
                            Testcase = lines[i].Replace("\n", "");
                            if (Testcase == "*** Test Cases ***")
                            {
                                i = i + 1;
                                var testcases = db.WebTestcases.FirstOrDefault(x => x.Webtestcase == lines[i].Replace("\n", "") && x.WebFilesId == WebFileId);
                                if(testcases != null)
                                {
                                    testcases.Status = "0";
                                    db.SaveChanges();
                                }
                                WebTestcases Ins1 = new WebTestcases();
                                Ins1.WebFilesId = Ins.WebFilesId;
                                Ins1.Webtestcase = lines[i].Replace("\n", "");
                                Ins1.Testcase = "T";
                                Ins1.Status = "1";
                                db.WebTestcases.Add(Ins1);
                                db.SaveChanges();
                                i = i + 1;
                                for (int j = i; j < lines.Length; j++)
                                {
                                    Testcase = lines[j].Replace("\n", "");
                                    if (lines[j] == "*** Test Cases ***")
                                    {

                                        Testcase = "";
                                        i = j - 1;
                                        break;
                                    }
                                    j = j + 1;
                                    if (Testcase != "")
                                    {
                                        Webtestcaselist Ins2 = new Webtestcaselist();
                                        Ins2.Status = "1";
                                        Ins2.WebTestcasesid = Ins1.WebTestcasesid;
                                        Ins2.Cases = Testcase;
                                        Ins2.CaseType = "T";
                                        db.Webtestcaselist.Add(Ins2);
                                        db.SaveChanges();
                                    }
                                    
                                }
                            }
                        }

                    }
                  
                    //count = lines.Length + 1;
                }
                TempData["Message"] = "File successfully uploaded to File System.";
                return RedirectToAction("Web");
            }

            catch (Exception ex)
            {
                return Json("Please Enter Valid Files");
            }
        }


        public JsonResult WebExecute(string ResourceName, string tagno, string testcasefile, string testcasename, string releaseno)
        {
            var testcaseresult = "";
            try
            {
                List<Execute> list1 = new List<Execute>();
                var header = this.Request.Headers.ToString();
                var header1 = this.Request.Headers.ToList();
                var Auth = (string)this.Request.Headers["Authorization"];
                if (Auth != "" && Auth != null && Auth != "max-age=0")
                {
                    //          TimeZoneInfo timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
                    TimeZoneInfo timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Etc/UTC");
                    DateTime dtCNow = TimeZoneInfo.ConvertTime(Convert.ToDateTime(DateTime.Now), timeZoneInfo);
                    string dtCNowdate = Convert.ToDateTime(dtCNow).ToString("yyyy-MM-dd");
                    TimeSpan tsnow = Convert.ToDateTime(dtCNow).TimeOfDay;
                    DateTime currentdatetime = Convert.ToDateTime(dtCNowdate + " " + (tsnow.Hours) + ":" + tsnow.Minutes);
                    var resultToken = db.TokenValue.First(b => b.TockenId == Auth && b.Validto >= currentdatetime);
                    if (resultToken != null)
                    {
                        var result = "";
                        var Resourcefile = ResourceName.Split(',');
                        var testcasefilelist = testcasefile.Split(',');
                        var testcaselist = testcasename.Split(',');
                        Process proc = new Process();
                        proc.StartInfo.FileName = @"/bin/bash";

                        proc.StartInfo.CreateNoWindow = true;
                        proc.StartInfo.RedirectStandardInput = true;
                        proc.StartInfo.RedirectStandardOutput = true;
                        proc.StartInfo.UseShellExecute = false;
                        proc.StartInfo.CreateNoWindow = true;
                        proc.Start();

                        proc.StandardInput.WriteLine("cd  /srv/www/Xautomatedemo/");
                        proc.StandardInput.WriteLine("rm -Rf  " + releaseno + "");
                        proc.StandardInput.WriteLine("mkdir  " + releaseno + " ");
                        proc.StandardInput.Flush();
                        proc.StandardInput.Close();
                        proc.WaitForExit();
                        string Error = proc.StandardOutput.ReadToEnd();
                        Console.WriteLine(proc.StandardOutput.ReadToEnd());
                        System.Diagnostics.Debug.WriteLine(proc.StandardOutput.ReadToEnd());
                        for (int i = 0; i < Resourcefile.Length; i++)
                        {
                            int resourcefile = Convert.ToInt32(Resourcefile[i]);
                         //   int dbconfigid = Convert.ToInt32(dbconfig1[0]);
                          //  var connections = db.DbConfig.FirstOrDefault(x => x.Dbconfigid == dbconfigid);

                           

                                result = db.WebFiles.FirstOrDefault(x => x.WebFilesId == resourcefile).WebFilesPath;
                                proc.StartInfo.FileName = @"/bin/bash";
                                proc.StartInfo.CreateNoWindow = true;
                                proc.StartInfo.RedirectStandardInput = true;
                                proc.StartInfo.RedirectStandardOutput = true;
                                proc.StartInfo.UseShellExecute = false;
                                proc.StartInfo.CreateNoWindow = true;
                                proc.Start();


                                proc.StandardInput.WriteLine("cd  /srv/www/Xautomatedemo/" + releaseno + "");

                            proc.StandardInput.WriteLine("cp " + result + "  /srv/www/Xautomatedemo/" + releaseno + "");
                            proc.StandardInput.Flush();
                                proc.StandardInput.Close();
                                proc.WaitForExit();
                                Console.WriteLine(proc.StandardOutput.ReadToEnd());
                                System.Diagnostics.Debug.WriteLine(proc.StandardOutput.ReadToEnd());
                            


                            var testapproach = (from c in db.WebTestcases where c.WebFilesId == resourcefile && c.Status == "1" select new { c.WebTestcasesid }).ToList();
                            var approach1 = testcasefile.Split(',');
                           // var suiteapproach = db.TestApproach.FirstOrDefault(x => x.TestSuiteId == suiteid);
                            foreach (var item2 in testapproach)
                            {
                                for (int k = 0; k < approach1.Length; k++)
                                {
                                    var testapproach1 = Convert.ToString(item2.WebTestcasesid);
                                    if (testapproach1 == approach1[k])
                                    {


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
                                        var rules = (from b in db.Webtestcaselist
                                                     where b.WebTestcasesid == item2.WebTestcasesid && b.Status == "1"
                                                     select new
                                                     {
                                                         b.CaseType,
                                                         b.Cases,
                                                     }).ToList();
                                        i = 0;
                                        foreach (var item in rules)
                                        {
                                           
                                               
                                                proc.StartInfo.FileName = @"/bin/bash";
                                                proc.StartInfo.CreateNoWindow = true;
                                                proc.StartInfo.RedirectStandardInput = true;
                                                proc.StartInfo.RedirectStandardOutput = true;
                                                proc.StartInfo.UseShellExecute = false;
                                                proc.StartInfo.CreateNoWindow = true;
                                                proc.Start();
                                                proc.StandardInput.WriteLine("cd /srv/www/Xautomatedemo/" + releaseno + "");
                                                // proc.StandardInput.WriteLine("touch " + Name + ".robot ");
                                                proc.StandardInput.WriteLine("cat >> " + result + "Main.robot ");
                                                if(item.CaseType == "S")
                                            {
                                                if(i == 0)
                                                {
                                                    proc.StandardInput.WriteLine("*** Settings ***");
                                                    proc.StandardInput.WriteLine(""+item.Cases+"");
                                                    i++;
                                                }
                                                else
                                                {
                                                    proc.StandardInput.WriteLine("" + item.Cases + "");
                                                }
                                            }
                                            else
                                            {
                                                proc.StandardInput.WriteLine("" + item.Cases + "");
                                            }
                                                //    proc.StandardInput.WriteLine("i");
                                               // proc.StandardInput.WriteLine("*** Test Cases ***");
                                             //   proc.StandardInput.WriteLine("" + item.Description + ":" + Testcase);
                                               // proc.StandardInput.WriteLine("      [Tags]   " + item.Description);
                                                //proc.StandardInput.WriteLine("Suite Setup   Connect To Database   pymysql   ${DBName}  ${DBUser}  ${DBPass}  ${DBHost}  ${DBPort}");
                                             //   proc.StandardInput.WriteLine("      Connect To Database   pymysql   ${DBName}  ${DBUser}  ${DBPass}  ${DBHost}  ${DBPort}");
                                               // proc.StandardInput.WriteLine("      " + item1.ExceptedResult + "     " + demo + "");
                                                proc.StandardInput.Flush();
                                                proc.StandardInput.Close();
                                                proc.WaitForExit();
                                            


                                        }
                                    }
                                }
                            }

                        }
                        proc.StartInfo.FileName = @"/bin/bash";
                        //     proc.StartInfo.FileName = "cmd.exe";
                        proc.StartInfo.CreateNoWindow = true;
                        proc.StartInfo.RedirectStandardInput = true;
                        proc.StartInfo.RedirectStandardOutput = true;
                        proc.StartInfo.UseShellExecute = false;
                        proc.StartInfo.CreateNoWindow = true;
                        proc.Start();

                        proc.StandardInput.WriteLine("sudo su");
                        proc.StandardInput.WriteLine("cd  /srv/www/Xautomatedemo/" + releaseno);
                        //  proc.StandardInput.WriteLine("cd /d D:\\Abinesh-learn");
                        proc.StandardInput.WriteLine("robot --listener robotframework_reportportal.listener --variable RP_ENDPOINT:http://13.74.176.44:8080 --variable RP_UUID:268f5445-764f-49c6-8e22-e0024d762a03 --variable RP_PROJECT:'default_personal' --variable RP_LAUNCH:'" + releaseno + "' -N  " + tagno + "  --report " + tagno + "" + releaseno + ".html  --removekeywords All  -d   /srv/www/Xautomatedemo/wwwroot/" + releaseno + "  " + result + "Main.robot");
                        ///    proc.StandardInput.WriteLine("pybot --listener reportportal_listener --variable RP_ENDPOINT:http://13.74.176.44:8080 --variable RP_UUID:6df6d59c-e0f6-44b0-a8c7-5087f0f36eac --variable RP_LAUNCH:'superadmin_TEST_EXAMPLE' --variable RP_PROJECT:superadmin_personal --report " + SuiteId + "" + Version + ".html  -d  /srv/www/XAutomate/wwwroot/" + SuiteId + "  " + SuiteId + "");
                      //  proc.StandardInput.WriteLine("robot robotframework");
                        proc.StandardInput.WriteLine("python -m dbbot.run -b mysql://root:password@13.74.176.44:3308/graphana /srv/www/Xautomatedemo/wwwroot/" + releaseno + "/output.xml");
                        proc.StandardInput.Flush();
                        proc.StandardInput.Close();
                        proc.WaitForExit();
                        string Error1 = proc.StandardOutput.ReadToEnd();
                        Console.WriteLine(proc.StandardOutput.ReadToEnd());
                        System.Diagnostics.Debug.WriteLine(proc.StandardOutput.ReadToEnd());
                        var Executetime = DateTime.Now.ToString("HH:mm");
                        MySqlConnection sql = new MySqlConnection("Server=13.74.176.44;Port=3308;Database=graphana;User ID=root;Password=password;SslMode=none");
                        sql.Open();

                        MySqlCommand com = new MySqlCommand("select * from graphana.suite_status order by id desc  Limit 1 ", sql);
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
                        Exceute ins = new Exceute();
                        ins.SuiteName = tagno;
                        ins.Execute = DateTime.Now;
                        ins.Time = Executetime + " sec";
                        ins.ResultUrl = "/" + releaseno + "/" + tagno + "" + releaseno + ".html";
                        db.Exceute.Add(ins);
                        db.SaveChanges();
                        var Errorcount = Error1.Split(",");

                        var totaltest = Errorcount[2].Split(" ");

                        var Failtest = Errorcount[4].Split(" ");
                        return Json(list1);
                        testcaseresult = totaltest[1] + " " + Errorcount[3] + " " + Failtest[0];


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
                return Json("");
            }

        }


    }
}
