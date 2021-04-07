using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using log4net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using TimeZoneConverter;
using XAutomateMVC.Models;
using XAutomateMVC.Models.DBModels;

namespace XAutomateMVC.Controllers
{
    public class WebController : Controller
    {
        static readonly ILog _log4net = LogManager.GetLogger(typeof(WebController));
        db_mateContext db = new db_mateContext();

        private IConfiguration configuration;

        public WebController(IConfiguration iConfig)
        {
            configuration = iConfig;
        }

        public IActionResult Webnew()
        {
            try
            {
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

                var SuiteList = (from product in db.DbConfig
                                 where product.Status == "1"
                                 select new SelectListItem()
                                 {
                                     Text = product.DbName,
                                     Value = product.DbName.ToString(),
                                 }).ToList();

                SuiteList.Insert(0, new SelectListItem()
                {
                    Text = "----Select----",
                    Value = string.Empty
                });
                ProductViewModel productViewModel = new ProductViewModel();
                productViewModel.ReleaseList = releaseno;
                productViewModel.TestSuiteList = SuiteList;
                _log4net.Error("Function Name : Web Web loaded Successfully");
                return View(productViewModel);
            }
            catch(Exception ex)
            {

                _log4net.Error("Function Name :  Login Webnew  -- " + ex.ToString());
                return null;
            }
          
        }

        public IActionResult Web()
        {
            try
            {
                ViewBag.Report =configuration.GetValue<string>("ReportPortalIp");
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
                                    where product.Status == "1" && (product.FileType == "Test Case" || product.FileType =="py")
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
                _log4net.Error("Function Name : Web Web loaded Successfully");
                return View(productViewModel);
            }
            catch(Exception ex)
            {
                _log4net.Error("Function Name : Web Web  -- " + ex.ToString());
                return null;
            }
         
        }


        public JsonResult technology(string technology)
        {
            try
            {
                var header = this.Request.Headers.ToString();
                var header1 = this.Request.Headers.ToList();
                var Auth = (string)this.Request.Headers["Authorization"];
                if (Auth != "" && Auth != null && Auth != "max-age=0")
                {
                    TimeZoneInfo timeZoneInfo = TZConvert.GetTimeZoneInfo("Europe/Stockholm");
                    //   TimeZoneInfo timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
                    // TimeZoneInfo timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Etc/UTC");
                    DateTime dtCNow = TimeZoneInfo.ConvertTime(Convert.ToDateTime(DateTime.Now), timeZoneInfo);
                    string dtCNowdate = Convert.ToDateTime(dtCNow).ToString("yyyy-MM-dd");
                    TimeSpan tsnow = Convert.ToDateTime(dtCNow).TimeOfDay;
                    DateTime currentdatetime = Convert.ToDateTime(dtCNowdate + " " + (tsnow.Hours) + ":" + tsnow.Minutes);
                    var resultToken = db.TokenValue.FirstOrDefault(b => b.TockenId == Auth && b.Validto >= currentdatetime);
                    if (resultToken != null)
                    {
                        if (technology == "ETL/DB")
                        {
                            var a = (from product in db.TestSuite
                                                where product.Status == "1"
                                                select new 
                                                {
                                                    WebfileTechnologyFoler = product.TestSuitename,
                                                    WebFilesTechnologyid = product.TestSuitename.ToString(),
                                                });
                            _log4net.Error("Function Name : Web testcasename loaded Successfully");
                            return Json(a);
                        }
                        else
                        {
                            var a = (from product in db.WebFilesTechnology
                                     where product.Status == 1 && product.Technology == technology
                                     select new
                                     {
                                         product.WebfileTechnologyFoler,
                                         product.WebFilesTechnologyid
                                     });
                            _log4net.Error("Function Name : Web testcasename loaded Successfully");
                            return Json(a);
                        }

                            //if(technology == "API")
                            //   {
                           
                     //   }
                        //else if (technology == "Web")
                        //{
                         
                     //   }
                      //  return null;
                    }
                    else
                    {
                        _log4net.Error("Function Name : Web testcasename Auth Failed ");
                        return Json("Auth Fail");
                    }
                }
                else
                {
                    _log4net.Error("Function Name : Web testcasename Auth Failed ");
                    return Json("Auth Fail");
                }
            }
            catch (Exception ex)
            {
                _log4net.Error("Function Name : Web testcasename  -- " + ex.ToString());
                return null;

            }

        }

        public JsonResult testapproach(string SuiteId)
        {
            try
            {
                var header = this.Request.Headers.ToString();
                var header1 = this.Request.Headers.ToList();
                var Auth = (string)this.Request.Headers["Authorization"];
                if (Auth != "" && Auth != null && Auth != "max-age=0")
                {
                    TimeZoneInfo timeZoneInfo = TZConvert.GetTimeZoneInfo("Europe/Stockholm");
                    //   TimeZoneInfo timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
                    //      TimeZoneInfo timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Etc/UTC");
                    DateTime dtCNow = TimeZoneInfo.ConvertTime(Convert.ToDateTime(DateTime.Now), timeZoneInfo);
                    string dtCNowdate = Convert.ToDateTime(dtCNow).ToString("yyyy-MM-dd");
                    TimeSpan tsnow = Convert.ToDateTime(dtCNow).TimeOfDay;
                    DateTime currentdatetime = Convert.ToDateTime(dtCNowdate + " " + (tsnow.Hours) + ":" + tsnow.Minutes);
                    var resultToken = db.TokenValue.FirstOrDefault(b => b.TockenId == Auth && b.Validto >= currentdatetime);
                    if (resultToken != null)
                    {

                        var a = from b in db.TestApproach
                                where b.SuiteIds.Contains(SuiteId) && b.Status == "1"
                                select new
                                {
                                    b.TestApproachid,

                                    b.TestApproachName,
                                };
                        _log4net.Info("Function Name : Execute testapproach Loaded Successfully");
                        return Json(a);
                    }
                    else
                    {
                        _log4net.Error("Function Name : Execute testapproach Auth Failled");
                        return Json("Auth Fail");
                    }
                }
                else
                {
                    _log4net.Error("Function Name : Execute testapproach Auth Failled");
                    return Json("Auth Fail");
                }
            }
            catch (Exception ex)
            {
                _log4net.Error("Function Name : Execute testapproach  -- " + ex.ToString());
                return null;
            }

        }
        public JsonResult testcasename(string TestcaseId,string technology)
        {
            try
            {
                var header = this.Request.Headers.ToString();
                var header1 = this.Request.Headers.ToList();
                var Auth = (string)this.Request.Headers["Authorization"];
                if (Auth != "" && Auth != null && Auth != "max-age=0")
                {
                    TimeZoneInfo timeZoneInfo = TZConvert.GetTimeZoneInfo("Europe/Stockholm");
                    //   TimeZoneInfo timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
                    // TimeZoneInfo timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Etc/UTC");
                    DateTime dtCNow = TimeZoneInfo.ConvertTime(Convert.ToDateTime(DateTime.Now), timeZoneInfo);
                    string dtCNowdate = Convert.ToDateTime(dtCNow).ToString("yyyy-MM-dd");
                    TimeSpan tsnow = Convert.ToDateTime(dtCNow).TimeOfDay;
                    DateTime currentdatetime = Convert.ToDateTime(dtCNowdate + " " + (tsnow.Hours) + ":" + tsnow.Minutes);
                    var resultToken = db.TokenValue.FirstOrDefault(b => b.TockenId == Auth && b.Validto >= currentdatetime);
                    if (resultToken != null)
                    {

                        if(technology == "ETL/DB")
                        {
                            var a = from b in db.TestApproach
                                    where b.SuiteIds.Contains(TestcaseId) && b.Status == "1"
                                    select new
                                    {
                                        webTestcasesid = b.TestApproachid,

                                        webtestcase=  b.TestApproachName,
                                    };
                            _log4net.Info("Function Name : Execute testapproach Loaded Successfully");
                            return Json(a);
                        }
                        else
                        {
                            var a = (from product in db.WebFiles
                                     join d in db.WebFilesTechnology on product.WebFilesTechnologyid equals d.WebFilesTechnologyid
                                     where product.Status == "1" && (product.FileType == "Test Case") && d.Technology == TestcaseId
                                     select new
                                     {
                                         webtestcase = product.FileName,
                                         webTestcasesid = product.WebFilesId
                                     });

                            _log4net.Error("Function Name : Web testcasename loaded Successfully");
                            return Json(a);
                        }

                       
                    }
                    else
                    {
                        _log4net.Error("Function Name : Web testcasename Auth Failed ");
                        return Json("Auth Fail");
                    }
                }
                else
                {
                    _log4net.Error("Function Name : Web testcasename Auth Failed ");
                    return Json("Auth Fail");
                }
            }
            catch(Exception ex)
            {
                _log4net.Error("Function Name : Web testcasename  -- " + ex.ToString());
                return null;

            }
           
        }
        public JsonResult testapproachcasename(string TestcaseId,string technology)
        {
            try
            {
                var header = this.Request.Headers.ToString();
                var header1 = this.Request.Headers.ToList();
                var Auth = (string)this.Request.Headers["Authorization"];
                if (Auth != "" && Auth != null && Auth != "max-age=0")
                {
                    TimeZoneInfo timeZoneInfo = TZConvert.GetTimeZoneInfo("Europe/Stockholm");
                    //   TimeZoneInfo timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
                    // TimeZoneInfo timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Etc/UTC");
                    DateTime dtCNow = TimeZoneInfo.ConvertTime(Convert.ToDateTime(DateTime.Now), timeZoneInfo);
                    string dtCNowdate = Convert.ToDateTime(dtCNow).ToString("yyyy-MM-dd");
                    TimeSpan tsnow = Convert.ToDateTime(dtCNow).TimeOfDay;
                    DateTime currentdatetime = Convert.ToDateTime(dtCNowdate + " " + (tsnow.Hours) + ":" + tsnow.Minutes);
                    var resultToken = db.TokenValue.FirstOrDefault(b => b.TockenId == Auth && b.Validto >= currentdatetime);
                    if (resultToken != null)
                    {

                        if(technology == "ETL/DB")
                        {
                            var a = from b in db.TestCases
                                    join d in db.Rules on b.RulesId equals d.RulesId
                                    where d.TestApproachName.Contains(TestcaseId)  && d.Status == "1"
                                    select new
                                    {
                                      //  b.WebTestcasesid,

                                       Webtestcase = b.TestCaseName
                                    };
                            _log4net.Error("Function Name : Web testcasename loaded Successfully");
                            return Json(a);
                        }
                        else
                        {
                            var a = from b in db.WebTestcases
                                    join d in db.WebFiles on b.WebFilesId equals d.WebFilesId
                                    where d.FileName == TestcaseId && b.Testcase == "T" && d.Status == "1"
                                    select new
                                    {
                                        b.WebTestcasesid,

                                        b.Webtestcase,
                                    };
                            _log4net.Error("Function Name : Web testcasename loaded Successfully");
                            return Json(a);
                        }

                       
                    }
                    else
                    {
                        _log4net.Error("Function Name : Web testcasename Auth Failed ");
                        return Json("Auth Fail");
                    }
                }
                else
                {
                    _log4net.Error("Function Name : Web testcasename Auth Failed ");
                    return Json("Auth Fail");
                }
            }
            catch (Exception ex)
            {
                _log4net.Error("Function Name : Web testcasename  -- " + ex.ToString());
                return null;

            }

        }
        public JsonResult testapproachcase(string SuiteId)
        {
            try
            {
                var header = this.Request.Headers.ToString();
                var header1 = this.Request.Headers.ToList();
                var Auth = (string)this.Request.Headers["Authorization"];
                if (Auth != "" && Auth != null && Auth != "max-age=0")
                {
                    TimeZoneInfo timeZoneInfo = TZConvert.GetTimeZoneInfo("Europe/Stockholm");
                    //   TimeZoneInfo timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
                    // TimeZoneInfo timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Etc/UTC");
                    DateTime dtCNow = TimeZoneInfo.ConvertTime(Convert.ToDateTime(DateTime.Now), timeZoneInfo);
                    string dtCNowdate = Convert.ToDateTime(dtCNow).ToString("yyyy-MM-dd");
                    TimeSpan tsnow = Convert.ToDateTime(dtCNow).TimeOfDay;
                    DateTime currentdatetime = Convert.ToDateTime(dtCNowdate + " " + (tsnow.Hours) + ":" + tsnow.Minutes);
                    var resultToken = db.TokenValue.FirstOrDefault(b => b.TockenId == Auth && b.Validto >= currentdatetime);
                    if (resultToken != null)
                    {

                        var a = from b in db.TestCases
                                where b.TestApproach.TestApproachName.Contains(SuiteId)
                                select new
                                {
                                    WebTestcasesid=    b.TestCasesId,

                                    Webtestcase=      b.TestCaseName,
                                };
                        _log4net.Error("Function Name : Web testcasename loaded Successfully");
                        return Json(a);
                    }
                    else
                    {
                        _log4net.Error("Function Name : Web testcasename Auth Failed ");
                        return Json("Auth Fail");
                    }
                }
                else
                {
                    _log4net.Error("Function Name : Web testcasename Auth Failed ");
                    return Json("Auth Fail");
                }
            }
            catch (Exception ex)
            {
                _log4net.Error("Function Name : Web testcasename  -- " + ex.ToString());
                return null;

            }

        }
        [HttpPost]
        public async Task<IActionResult> Web(List<IFormFile> files, IFormCollection technology)
        {
            try
            {
                var technoloy = technology["technology"];
                var test123 = files.Count;
                foreach (var file in files)
                {
                    var filename = file.FileName.Split(".");
                    var firstfile = filename[0].Substring(0,6);
                    string fileload = configuration.GetValue<string>("Systempath:uploadpath");
                    var basePath = Path.Combine(Directory.GetCurrentDirectory() + fileload + technology);
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
                    if (filePath.ToLower().Contains(".xl"))
                    {
                       
                        var basePath1 = Path.Combine(Directory.GetCurrentDirectory());
                        bool basePathExists1 = System.IO.Directory.Exists(basePath1);
                        if (!basePathExists1) Directory.CreateDirectory(basePath1);
                        var fileName1 = Path.GetFileNameWithoutExtension(file.FileName);
                        var filePath1 = Path.Combine(basePath1, file.FileName);
                        var extension1 = Path.GetExtension(file.FileName);
                        using (var stream = new FileStream(filePath1, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }
                        var fileModel = new FileonSystem
                        {
                            CreatedOn = DateTime.UtcNow,
                            FileType = file.ContentType,
                            Extension = extension1,
                            Name = fileName1,
                            FilePath = filePath1
                        };
                    }
                    var fullPathImport = System.IO.Path.Combine(filePath);

                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                    decimal WebFileId = 0;
                    var dircetor = Directory.GetCurrentDirectory();
                    if (!filePath.ToLower().Contains("testcase"))
                    {
                        if (filePath.ToLower().Contains("resource"))
                        {
                            var filenamecheck = db.WebFiles.FirstOrDefault(x => x.FileName == file.FileName && x.Status == "1");
                            if (filenamecheck != null)
                            {
                                filenamecheck.Status = "0";
                                db.SaveChanges();
                            }
                            WebFiles Ins = new WebFiles();
                            Ins.FileType = "Resource";
                            if (fileName.Contains("("))
                            {
                                Ins.WebFilesPath = dircetor +  "/" + file.FileName + "'";
                            }
                            else
                            {
                                Ins.WebFilesPath = dircetor + "/" + file.FileName;
                            }

                            Ins.FileName = file.FileName;
                            Ins.Status = "1";
                            db.WebFiles.Add(Ins);
                            db.SaveChanges();
                        }
                        else if (filePath.ToLower().Contains(".py"))
                        {
                            WebFiles Ins = new WebFiles();
                            Ins.FileType = "py";
                            if (fileName.Contains("("))
                            {
                                Ins.WebFilesPath = dircetor + "/" + file.FileName + "'";
                            }
                            else
                            {
                                Ins.WebFilesPath = dircetor + "/" + file.FileName;
                            }

                            Ins.FileName = file.FileName;
                            Ins.Status = "1";
                            db.WebFiles.Add(Ins);
                            db.SaveChanges();
                        }
                        else
                        {
                            WebFiles Ins = new WebFiles();
                            Ins.FileType = "Other";
                            if (fileName.Contains("("))
                            {
                                Ins.WebFilesPath = dircetor + "/"+ file.FileName + "'";
                            }
                            else
                            {
                                Ins.WebFilesPath = dircetor + "/"+ file.FileName;
                            }

                            Ins.FileName = file.FileName;
                            Ins.Status = "1";
                            Ins.Technology = technoloy;
                            db.WebFiles.Add(Ins);
                            db.SaveChanges();
                        }
                    }
                    else
                    {
                        var filenamecheck = db.WebFiles.FirstOrDefault(x => x.FileName == file.FileName && x.Status =="1");
                        if (filenamecheck != null)
                        {
                            WebFileId = filenamecheck.WebFilesId;
                            filenamecheck.Status = "0";
                            db.SaveChanges();
                        }
                        db_mateContext db1 = new db_mateContext();
                        WebFiles Ins = new WebFiles();
                        Ins.FileType = "Test Case";
                        Ins.WebFilesPath = filePath;
                        Ins.FileName = file.FileName;
                        Ins.Status = "1";
                        db1.WebFiles.Add(Ins);
                        db1.SaveChanges();
                        var Testcase = "";
                        string[] lines;
                        var fileCount = new StreamReader(filePath).ReadToEnd(); // big string
                      lines = fileCount.Split(new char[] { '\n' });           // big array
                        if(lines.Length == 1)
                        {
                            lines = fileCount.Split(new char[] { '\r' });
                        }
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
                                    if (Testcase != "")
                                    {
                                        Webtestcaselist Ins2 = new Webtestcaselist();
                                        Ins2.Status = "1";
                                        Ins2.WebTestcasesid = Ins1.WebTestcasesid;
                                        Ins2.Cases = Testcase;
                                        Ins2.CaseType = "S";
                                        db.Webtestcaselist.Add(Ins2);
                                        db.SaveChanges();
                                    }
                                    if (Testcase == "*** Test Cases ***")
                                    {

                                        i = j - 1;
                                        break;
                                    }
                         //           j = j + 1;
                                  
                            

                                   
                                }
                            }
                            Testcase = lines[i].Replace("\n", "");
                            if (Testcase == "*** Test Cases ***")
                            {
                               
                                i = i + 1;
                                var test1 = lines[i].Replace("\n", "");
                                var testcases = db.WebTestcases.FirstOrDefault(x => x.Webtestcase == lines[i].Replace("\n", "") && x.WebFilesId == WebFileId);
                                if(testcases != null)
                                {
                                    testcases.Status = "0";
                                    db.SaveChanges();
                                }

                                WebTestcases Ins1 = new WebTestcases();
                                Ins1.WebFilesId = Ins.WebFilesId;
                                Ins1.Webtestcase = test1;
                                Ins1.Testcase = "T";
                                Ins1.Status = "1";
                                db.WebTestcases.Add(Ins1);
                                db.SaveChanges();
                                Webtestcaselist Ins3 = new Webtestcaselist();
                                Ins3.Status = "1";
                                Ins3.WebTestcasesid = Ins1.WebTestcasesid;
                                Ins3.Cases = test1;
                                Ins3.CaseType = "T";
                                db.Webtestcaselist.Add(Ins3);
                                db.SaveChanges();
                                i = i + 1;
                                for (int j = i; j < lines.Length; j++)
                                {
                                    Testcase = lines[j].Replace("\n", "");
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
                                    if (Testcase == "*** Test Cases ***")
                                    {

                                        Testcase = "";
                                        i = j - 1;
                                        break;
                                    }
                                    
                                   
                                    i = j;
                                }
                                
                            }
                        }

                    }
                  
                    //count = lines.Length + 1;
                }
                _log4net.Info("Function Name : Web UploadToFileSystem loaded Successfully");
                TempData["Message"] = "File successfully uploaded to File System.";
                return View();
            }

            catch (Exception ex)
            {
                _log4net.Error("Function Name : Web UploadToFileSystem  -- " + ex.ToString());
                return View("Please Enter Valid Files");
            }
        }


        public JsonResult WebExecute( string tagno, string testcasefile, string testcasename, string releaseno)
        {
            //var testcaseresult = "";
          //  int zz = 0;
            var testfile = "";
           // int res1 = 0;
           // var test = "testdemo";
          
            try
            {
             
                List<Execute> list1 = new List<Execute>();
                var header = this.Request.Headers.ToString();
                var header1 = this.Request.Headers.ToList();
                var Auth = (string)this.Request.Headers["Authorization"];
                if (Auth != "" && Auth != null && Auth != "max-age=0")
                {
                    //      TimeZoneInfo timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
                    // TimeZoneInfo timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Etc/UTC");
                    TimeZoneInfo timeZoneInfo = TZConvert.GetTimeZoneInfo("Europe/Stockholm");
                    DateTime dtCNow = TimeZoneInfo.ConvertTime(Convert.ToDateTime(DateTime.Now), timeZoneInfo);
                    string dtCNowdate = Convert.ToDateTime(dtCNow).ToString("yyyy-MM-dd");
                    TimeSpan tsnow = Convert.ToDateTime(dtCNow).TimeOfDay;
                    DateTime currentdatetime = Convert.ToDateTime(dtCNowdate + " " + (tsnow.Hours) + ":" + tsnow.Minutes);
                    var resultToken = db.TokenValue.FirstOrDefault(b => b.TockenId == Auth && b.Validto >= currentdatetime);
                    if (resultToken != null)
                    {
                        string fileload = configuration.GetValue<string>("Systempath:pagepath");
                        string reportPath = configuration.GetValue<string>("Systempath:ReportPath");
                        string fileload1 = configuration.GetValue<string>("Systempath:uploadpath");
                        var result = "";
                     //   var Resourcefile = ResourceName.Split(',');
                     if(testcasename != null && testcasename != "")
                        {
                            var testcasefilelist = testcasefile.Split(',');
                            var testcaselist = testcasename.Split(',');
                            var testcaselaname = "";
                            for(var z =0; z < testcaselist.Length; z++)
                            {
                                var valuete = testcaselist[z];
                                var conver = Convert.ToInt32(valuete);
                                var testfilename = db.WebTestcases.FirstOrDefault(x => x.WebTestcasesid == conver).Webtestcase;
                                if(testcaselaname == "")
                                {
                                    testcaselaname = "-t  " + "'"+ testfilename + "'";
                                }
                                else
                                {
                                    testcaselaname += "  -t  " + "'" + testfilename + "'";
                                }
                            }
                            ///Temprovery Comment
                            Process proc = new Process();
                            proc.StartInfo.FileName = @"/bin/bash";

                            proc.StartInfo.CreateNoWindow = true;
                            proc.StartInfo.RedirectStandardInput = true;
                            proc.StartInfo.RedirectStandardOutput = true;
                            proc.StartInfo.UseShellExecute = false;
                            proc.StartInfo.CreateNoWindow = true;
                            proc.Start();

                            proc.StandardInput.WriteLine("" + fileload + "");
                            proc.StandardInput.WriteLine("rm -Rf  " + releaseno + "");
                            proc.StandardInput.WriteLine("mkdir  " + releaseno + " ");
                            proc.StandardInput.Flush();
                            proc.StandardInput.Close();
                            proc.WaitForExit();
                            string Error = proc.StandardOutput.ReadToEnd();
                            Console.WriteLine(proc.StandardOutput.ReadToEnd());
                            System.Diagnostics.Debug.WriteLine(proc.StandardOutput.ReadToEnd());

                            var testfile1 = Convert.ToInt32(testcasefile);
                            var testcaseId = db.WebFiles.FirstOrDefault(x => x.WebFilesId == testfile1).FileName;
                            proc.StartInfo.FileName = @"/bin/bash";
                            //     proc.StartInfo.FileName = "cmd.exe";
                            proc.StartInfo.CreateNoWindow = true;
                            proc.StartInfo.RedirectStandardInput = true;
                            proc.StartInfo.RedirectStandardOutput = true;
                            proc.StartInfo.UseShellExecute = false;
                            proc.StartInfo.CreateNoWindow = true;
                            proc.Start();
                            var ReportPortal =configuration.GetValue<string>("ReportPortalIp");
                            var ReportalId = HttpContext.Session.GetString("ReportportalId");
                            var graphanausr = HttpContext.Session.GetString("graphanauser");
                            var graphanapass = HttpContext.Session.GetString("graphanapass");
                            var graphanaIp = HttpContext.Session.GetString("graphanadbip");
                            var graphanaport = HttpContext.Session.GetString("graphanaport");
                            var graphanadbname = HttpContext.Session.GetString("graphananame");
                            proc.StandardInput.WriteLine("" + fileload + "" + fileload1 + "");
                            //  proc.StandardInput.WriteLine("cd /d D:\\Abinesh-learn");
                            proc.StandardInput.WriteLine("robot    " + testcaselaname + " --removekeywords All  --variable RP_ENDPOINT:" + ReportPortal + " --variable RP_UUID:" + ReportalId + " --variable RP_PROJECT:'default_personal' --variable RP_LAUNCH:'" + releaseno + "' --variable rp.attributes:one --variable rp.keystore.password:false  --variable rp.enable:false   -N  " + tagno + "   --report " + tagno + "" + testfile + ".html  --log " + tagno + "" + releaseno + "log.html   -d   " + reportPath + "" + releaseno + " " + testcaseId + "");
                         
                            //  proc.StandardInput.WriteLine("robot robotframework");

                            proc.StandardInput.WriteLine("python - m dbbot.run -b mysql://"+ graphanausr + ":"+ graphanapass + "@"+ graphanaIp + ":"+ graphanaport + "/"+ graphanadbname + " /srv/www/XAutomate/wwwroot/" + releaseno + "/output.xml");
                            proc.StandardInput.Flush();
                            proc.StandardInput.Close();
                            proc.WaitForExit();
                            string Error1 = proc.StandardOutput.ReadToEnd();
                            Console.WriteLine(proc.StandardOutput.ReadToEnd());
                            System.Diagnostics.Debug.WriteLine(proc.StandardOutput.ReadToEnd());
                            var Executetime = DateTime.Now.ToString("HH:mm");
                           
                            MySqlConnection sql = new MySqlConnection("Server=" + graphanaIp + ";Port=" + graphanaport + ";Database=" + graphanadbname + ";User ID=" + graphanausr + ";Password=" + graphanapass + ";SslMode=none");
                            sql.Open();
                            var suitename = testcaseId.Split(".");
                            var suitelate = suitename[0];
                            MySqlCommand com = new MySqlCommand("select  * from graphana.suite_status  join graphana.suites  on  suite_status.suite_id = suites.id where suites.name='" + suitelate + "' order by suite_status.id desc Limit 1 ", sql);
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
                            ins.Status = "1";
                            ins.Time = Executetime1 + " s";
                            ins.ResultUrl = "/" + releaseno + "/" + tagno + "" + testfile + ".html";
                            db.Exceute.Add(ins);
                            db.SaveChanges();
                            var Errorcount = Error1.Split(",");

                            var totaltest = Errorcount[2].Split(" ");

                            var Failtest = Errorcount[4].Split(" ");
                            _log4net.Info("Function Name : Web Execute Excuted successfully");
                            return Json(list1);
                            //testcaseresult = totaltest[1] + " " + Errorcount[3] + " " + Failtest[0];
                            //     return "";
                        }
                        else
                        {
                            var testfile1 = Convert.ToInt32(testcasefile);
                            var testcaseId = db.WebFiles.FirstOrDefault(x => x.WebFilesId == testfile1).FileName;
                            Process proc = new Process();
                            proc.StartInfo.FileName = @"/bin/bash";
                            //     proc.StartInfo.FileName = "cmd.exe";
                            proc.StartInfo.CreateNoWindow = true;
                            proc.StartInfo.RedirectStandardInput = true;
                            proc.StartInfo.RedirectStandardOutput = true;
                            proc.StartInfo.UseShellExecute = false;
                            proc.StartInfo.CreateNoWindow = true;
                            proc.Start();
                            proc.StandardInput.WriteLine("" + fileload + "");
                            //  proc.StandardInput.WriteLine("cd /d D:\\Abinesh-learn");
                            proc.StandardInput.WriteLine("python  --listener robotframework_reportportal.listener --variable RP_ENDPOINT:" +configuration.GetValue<string>("ReportPortalIp") + " --variable RP_UUID:" + HttpContext.Session.GetString("ReportportalId") + "  --variable RP_PROJECT:'default_personal' --variable RP_LAUNCH:'" + testfile + "'  -N  " + tagno + "  --report " + tagno + "" + testfile + ".html  --log " + tagno + "" + releaseno + "log.html   -d   " + reportPath + "" + testfile + "   " + testcaseId + "");
                            
                            //  proc.StandardInput.WriteLine("robot robotframework");
                            proc.StandardInput.WriteLine("python -m dbbot.run -b mysql://" + HttpContext.Session.GetString("graphanauser") + ":" + HttpContext.Session.GetString("graphanapass") + "@" + HttpContext.Session.GetString("graphanadbip") + ":" + HttpContext.Session.GetString("graphanaport") + "/" + HttpContext.Session.GetString("graphananame") + " " + reportPath + "" + testfile + "/output.xml");
                            proc.StandardInput.Flush();
                            proc.StandardInput.Close();
                            proc.WaitForExit();
                            string Error1 = proc.StandardOutput.ReadToEnd();
                            Console.WriteLine(proc.StandardOutput.ReadToEnd());
                            System.Diagnostics.Debug.WriteLine(proc.StandardOutput.ReadToEnd());
                            var Executetime = DateTime.Now.ToString("HH:mm");
                            MySqlConnection sql = new MySqlConnection("Server=" + HttpContext.Session.GetString("graphanadbip") + ";Port=" + HttpContext.Session.GetString("graphanaport") + ";Database=" + HttpContext.Session.GetString("graphananame") + ";User ID=" + HttpContext.Session.GetString("graphanauser") + ";Password=" + HttpContext.Session.GetString("graphanapass") + ";SslMode=none");
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
                            ins.Status = "1";
                            ins.Time = Executetime1 + " s";
                            ins.ResultUrl = "/" + testfile + "/" + tagno + "" + result.Substring(0, 6) + ".html";
                            db.Exceute.Add(ins);
                            db.SaveChanges();
                            var Errorcount = Error1.Split(",");

                            var totaltest = Errorcount[2].Split(" ");

                            var Failtest = Errorcount[4].Split(" ");
                            _log4net.Info("Function Name : Web Execute Excuted successfully");
                            return Json(list1);
                            //testcaseresult = totaltest[1] + " " + Errorcount[3] + " " + Failtest[0];
                            //     return "";
                        }

                    }
                    else
                    {
                        _log4net.Error("Function Name : Web Execute Auth Failed");
                        return Json("Auth Fail");
                      //  return "";
                    }
                }
                else
                {
                    _log4net.Error("Function Name : Web Execute Auth Failed");
                    return Json("Auth Fail");
                    //return "";
                }
            }
            catch (Exception ex)
            {
                _log4net.Error("Function Name : Web Execute  -- " + ex.ToString());
                return Json(ex.ToString());
            //    return ex.ToString() +"  " + zz + "   " + ResourceName + "   " + res1;
            }

        }
        public JsonResult Executescreen(string suitename, string technology, string tagno, string testapproach, string testcasename, string releaseno, string connection,string Adoplan)
        {
            try
            {
                var header = this.Request.Headers.ToString();
                var header1 = this.Request.Headers.ToList();
                var Auth = (string)this.Request.Headers["Authorization"];
                if (Auth != "" && Auth != null && Auth != "max-age=0")
                {
                    TimeZoneInfo timeZoneInfo = TZConvert.GetTimeZoneInfo("Europe/Stockholm");
                    DateTime dtCNow = TimeZoneInfo.ConvertTime(Convert.ToDateTime(DateTime.Now), timeZoneInfo);
                    string dtCNowdate = Convert.ToDateTime(dtCNow).ToString("yyyy-MM-dd");
                    TimeSpan tsnow = Convert.ToDateTime(dtCNow).TimeOfDay;
                    DateTime currentdatetime = Convert.ToDateTime(dtCNowdate + " " + (tsnow.Hours) + ":" + tsnow.Minutes);
                    var resultToken = db.TokenValue.FirstOrDefault(b => b.TockenId == Auth && b.Validto >= currentdatetime);
                    if (resultToken != null)
                    {
                        WebAPIExecutetech(suitename, technology, tagno, testapproach, testcasename, releaseno, connection, Adoplan);
                        var exctionid = (from b in db.Exceute orderby b.ExecuteId descending select new { b.ExecuteId }).FirstOrDefault();
                        var execue = exctionid.ExecuteId;
                        return Json(execue);
                    }
                    else
                    {
                        return Json("Authutication Failed");
                    }
                }
                else
                {
                    return Json("Authutication Failed");
                }
            }
            catch(Exception ex)
            {
                _log4net.Error("Function Name :  Login Executescreen  -- " + ex.ToString());
                return Json("Failed");
            }
          
        
        }

        public JsonResult Executescreenoutput(int executeid)
        {
            var header = this.Request.Headers.ToString();
            var header1 = this.Request.Headers.ToList();
            var Auth = (string)this.Request.Headers["Authorization"];
            if (Auth != "" && Auth != null && Auth != "max-age=0")
            {
                TimeZoneInfo timeZoneInfo = TZConvert.GetTimeZoneInfo("Europe/Stockholm");
                DateTime dtCNow = TimeZoneInfo.ConvertTime(Convert.ToDateTime(DateTime.Now), timeZoneInfo);
                string dtCNowdate = Convert.ToDateTime(dtCNow).ToString("yyyy-MM-dd");
                TimeSpan tsnow = Convert.ToDateTime(dtCNow).TimeOfDay;
                DateTime currentdatetime = Convert.ToDateTime(dtCNowdate + " " + (tsnow.Hours) + ":" + tsnow.Minutes);
                var resultToken = db.TokenValue.FirstOrDefault(b => b.TockenId == Auth && b.Validto >= currentdatetime);
                if (resultToken != null)
                {
                    var a = db.Exceute.FirstOrDefault(x => x.ExecuteId == executeid).SuiteName;
            if(a != null)
            {
                List<Execute> list1 = new List<Execute>();

                var ReportPortal =configuration.GetValue<string>("ReportPortalIp");
                var ReportalId = HttpContext.Session.GetString("ReportportalId");
                var graphanausr = HttpContext.Session.GetString("graphanauser");
                var graphanapass = HttpContext.Session.GetString("graphanapass");
                var graphanaIp = HttpContext.Session.GetString("graphanadbip");
                var graphanaport = HttpContext.Session.GetString("graphanaport");
                var graphanadbname = HttpContext.Session.GetString("graphananame");
                MySqlConnection sql = new MySqlConnection("Server=" + graphanaIp + ";Port=" + graphanaport + ";Database=" + graphanadbname + ";User ID=" + graphanausr + ";Password=" + graphanapass + ";SslMode=none");
                sql.Open();

                MySqlCommand com = new MySqlCommand("select  * from graphana.suite_status  join graphana.suites  on  suite_status.suite_id = suites.id where suites.name='" + a + "' order by suite_status.id desc Limit 1 ", sql);
                using (var reader = com.ExecuteReader())
                {
                    while (reader.Read())
                    {

                        list1.Add(new Execute()
                        {

                            Passed = reader["passed"].ToString(),
                            failed = reader["failed"].ToString(),
                            total_test = Convert.ToInt32(reader["passed"]) + Convert.ToInt32(reader["failed"]),
                        });
                    }
                }
                return Json(list1);
            }
            else
            {
                return Json("Process didn't initialted");

            }
                }
                else
                {
                    return Json("Authutication Failed");
                }
            }
            else
            {
                return Json("Authutication Failed");
            }


        }
        public JsonResult WebExecutetech(string suitename,string technology,string tagno, string testapproach, string testcasename, string releaseno,string connection,string Adoplan)
        {
            int tablecolumn = 0;
          //  int zz = 0;
            var testfile = "";
           // int res1 = 0;
            var test = "testdemo";
            var testerror = "testdemo";
            int dublicatecol = 0;
            if (technology == "ETL/DB")
            {
             //   string[] testapproachcheck;
                List<int> ruleadd = new List<int>();
                List<string> coloumnvalue_common = new List<string>();
                try
                {


                    int MY = 0;
                    int SQ = 0;
                    List<Execute> list1 = new List<Execute>();
                    var header = this.Request.Headers.ToString();
                    var header1 = this.Request.Headers.ToList();
                    var Auth = (string)this.Request.Headers["Authorization"];
                    if (Auth != "" && Auth != null && Auth != "max-age=0")
                    {
                        TimeZoneInfo timeZoneInfo = TZConvert.GetTimeZoneInfo("Europe/Stockholm");
                        DateTime dtCNow = TimeZoneInfo.ConvertTime(Convert.ToDateTime(DateTime.Now), timeZoneInfo);
                        string dtCNowdate = Convert.ToDateTime(dtCNow).ToString("yyyy-MM-dd");
                        TimeSpan tsnow = Convert.ToDateTime(dtCNow).TimeOfDay;
                        DateTime currentdatetime = Convert.ToDateTime(dtCNowdate + " " + (tsnow.Hours) + ":" + tsnow.Minutes);
                        var resultToken = db.TokenValue.FirstOrDefault(b => b.TockenId == Auth && b.Validto >= currentdatetime);
                        if (resultToken != null)
                        {
                            string fileload = configuration.GetValue<string>("Systempath:pagepath");
                            string reportPath = configuration.GetValue<string>("Systempath:ReportPath");
                            string External = configuration.GetValue<string>("Systempath:Externalproperties");
                            //string[] lines;
                            //var filePath = Path.Combine(Directory.GetCurrentDirectory() + "" + External + "");

                            //var fileCount = new StreamReader(filePath).ReadToEnd(); // big string
                            //lines = fileCount.Split(new char[] { '\n' });           // big array
                            //if (lines.Length == 1)
                            //{
                            //    lines = fileCount.Split(new char[] { '\r' });
                            //}
                            //for (int i = 0; i < lines.Length; i++)
                            //{
                            //    var linesval = lines[i];
                            //    linesval = linesval.Replace(" ", "");
                            //    if (linesval.Contains("ReportPortalIp"))
                            //    {
                            //        var ReportPortal1 = lines[i + 1];
                            //        ReportPortal1 = ReportPortal1.Replace(" ", "");
                            //        HttpContext.Session.SetString("ReportPortal", ReportPortal1);
                            //        ViewBag.Report =configuration.GetValue<string>("ReportPortalIp");
                            //    }
                            //    if (linesval.Contains("graphanaIp"))
                            //    {
                            //        var graphana = lines[i + 1];
                            //        graphana = graphana.Replace(" ", "");
                            //        HttpContext.Session.SetString("graphanaIp", graphana);
                            //    }
                            //    if (linesval.Contains("graphanadbIp"))
                            //    {
                            //        var graphanadbip = lines[i + 1];
                            //        graphanadbip = graphanadbip.Replace(" ", "");
                            //        HttpContext.Session.SetString("graphanadbip", graphanadbip);
                            //    }
                            //    if (linesval.Contains("graphanadbPort"))
                            //    {
                            //        var graphanaport1 = lines[i + 1];
                            //        graphanaport1 = graphanaport1.Replace(" ", "");
                            //        HttpContext.Session.SetString("graphanaport", graphanaport1);
                            //    }
                            //    if (linesval.Contains("graphanausername"))
                            //    {
                            //        var graphanauser = lines[i + 1];
                            //        graphanauser = graphanauser.Replace(" ", "");
                            //        HttpContext.Session.SetString("graphanauser", graphanauser);
                            //    }
                            //    if (linesval.Contains("graphanapass"))
                            //    {
                            //        var graphanapass1 = lines[i + 1];
                            //        graphanapass1 = graphanapass1.Replace(" ", "");
                            //        HttpContext.Session.SetString("graphanapass", graphanapass1);
                            //    }
                            //    if (linesval.Contains("graphanaDbname"))
                            //    {
                            //        var graphananame = lines[i + 1];
                            //        graphananame = graphananame.Replace(" ", "");
                            //        HttpContext.Session.SetString("graphananame", graphananame);
                            //    }
                            //    if (linesval.Contains("ReportPortalUid"))
                            //    {
                            //        var Reportportal = lines[i + 1];
                            //        Reportportal = Reportportal.Replace(" ", "");
                            //        HttpContext.Session.SetString("ReportportalId", Reportportal);
                            //    }
                            //}
                         //   var result = "";
                            var suiteId = suitename.Split(',');
                            //var dbconfig1 = dbconnection.Split(',');
                            var approach = testapproach.Split(',');
                            Process proc = new Process();
                            proc.StartInfo.FileName = @"/bin/bash";

                            proc.StartInfo.CreateNoWindow = true;
                            proc.StartInfo.RedirectStandardInput = true;
                            proc.StartInfo.RedirectStandardOutput = true;
                            proc.StartInfo.UseShellExecute = false;
                            proc.StartInfo.CreateNoWindow = true;
                            proc.Start();

                            proc.StandardInput.WriteLine("" + fileload + "");
                            proc.StandardInput.WriteLine("rm -Rf  " + suiteId[0] + "");
                            proc.StandardInput.WriteLine("mkdir  " + suiteId[0] + " ");
                            proc.StandardInput.Flush();
                            proc.StandardInput.Close();
                            proc.WaitForExit();
                            proc.StandardOutput.ReadToEnd();
                            Console.WriteLine(proc.StandardOutput.ReadToEnd());
                            System.Diagnostics.Debug.WriteLine(proc.StandardOutput.ReadToEnd());
                            for (int i = 0; i < suiteId.Length; i++)
                            {
                               
                                var approach1 = testapproach.Split(',');


                                for (int k = 0; k < approach1.Length; k++)
                                {
                                    MY = 0;
                                    SQ = 0;
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
                                        //return "";
                                    }
                                  
                                    foreach (var item in rules)
                                    {
                                        var connections = db.DbConfig.FirstOrDefault(x => x.DbName == connection);

                                        if (connections.DatabaseType == "MYSQL")
                                        {
                                            if (MY == 0)
                                            {
                                                //   result = db.TestSuite.FirstOrDefault(x => x.TestSuiteId == suiteid).TestSuitename;
                                                proc.StartInfo.FileName = @"/bin/bash";
                                                proc.StartInfo.CreateNoWindow = true;
                                                proc.StartInfo.RedirectStandardInput = true;
                                                proc.StartInfo.RedirectStandardOutput = true;
                                                proc.StartInfo.UseShellExecute = false;
                                                proc.StartInfo.CreateNoWindow = true;
                                                proc.Start();


                                                proc.StandardInput.WriteLine("" + fileload  + ""+ suiteId[i] + "");

                                                proc.StandardInput.WriteLine("cat >  " + approach1[k] + ".robot ");
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


                                                    proc.StandardInput.WriteLine("" + fileload + "" + suiteId[i] + "");

                                                    proc.StandardInput.WriteLine("cat >  " + approach1[k] + ".robot ");
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

                                    foreach (var ruleitem in rules)
                                    {
                                        //if (ruleadd.Count() == 0)
                                        //{
                                        //    ruleadd.Add((int)ruleitem.RulesId);
                                        //}
                                        //else if (!ruleadd.Contains((int)ruleitem.RulesId))
                                        //{
                                        //    ruleadd.Add((int)ruleitem.RulesId);
                                        //}
                                        var testcase = (from b in db.TestCases
                                                        where b.RulesId == ruleitem.RulesId && b.Status == "1"
                                                        select new
                                                        {
                                                            b.Testdata,
                                                            b.ExceptedResult,
                                                            testcaseparam = b.TestCaseParameters.Select(x => new
                                                            {
                                                                x.ParameterName,
                                                            }),
                                                            RuleCondtion = b.Rules.RuleCondtion,
                                                            b.Expextedparameter,
                                                            Description = b.Rules.Description,
                                                            Desc = b.Description,
                                                            b.TestCaseName
                                                        }).ToList();
                                        //if (testcase.Count() == 0)
                                        //{
                                        //    return Json(item.RuleName + "Testcase Not Available");
                                        //}
                                        foreach (var item1 in testcase)
                                        {
                                            var connections = db.DbConfig.FirstOrDefault(x => x.DbName == connection);
                                            var list = item1.Testdata;
                                          //  tablecolumn = db.Tablecolumn.Where(x => x.Dbconfigid == connections.Dbconfigid &&  x.FieldName !="D" ).Count();

                                          
                                            //var result2 = Regex.Split(list, ",(?![^()]*\\))");
                                            // var Parameter = Regex.Split(list, ",,,(?![^()]*\\))");
                                            var Parameter = list.Split(",,,");
                                            var demo = item1.RuleCondtion;
                                            var Testcase = "";
                                            int h = 0;
                                            dublicatecol = 0;
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
                                                //foreach(var col in coloumnvalues)
                                                //{
                                                   
                                                //    if (col.Tablecolumn1 == Parameter[L])
                                                //    {
                                                //        if (coloumnvalue_common.Contains(col.Tablecolumn1))
                                                //        {
                                                //            dublicatecol++;
                                                //        }
                                                //        coloumnvalue_common.Add(col.Tablecolumn1);
                                                //        break;
                                                //    }
                                                   
                                                //}
                                                h++;

                                            }
                                            proc.StartInfo.FileName = @"/bin/bash";
                                            proc.StartInfo.CreateNoWindow = true;
                                            proc.StartInfo.RedirectStandardInput = true;
                                            proc.StartInfo.RedirectStandardOutput = true;
                                            proc.StartInfo.UseShellExecute = false;
                                            proc.StartInfo.CreateNoWindow = true;
                                            proc.Start();
                                            proc.StandardInput.WriteLine("" + fileload + "" + suiteId[i] + "");
                                            // proc.StandardInput.WriteLine("touch " + Name + ".robot ");
                                            proc.StandardInput.WriteLine("cat >> " + approach1[k] + ".robot ");
                                            proc.StandardInput.WriteLine("*** Variables  ***");
                                            proc.StandardInput.WriteLine("${DBName}   " + connections.DbName + "");
                                            proc.StandardInput.WriteLine("${DBUser}   " + connections.DbUser + "");
                                            proc.StandardInput.WriteLine("${DBPass}  " + connections.DbPassword + "");
                                            proc.StandardInput.WriteLine("${DBHost}  " + connections.DbHostName + "");

                                            if (connections.DatabaseType == "MYSQL")
                                            {
                                                proc.StandardInput.WriteLine("${DBPort}    " + connections.DbPort + "");
                                                proc.StandardInput.WriteLine("*** Test Cases ***");
                                                proc.StandardInput.WriteLine("" + item1.TestCaseName + "");
                                                proc.StandardInput.WriteLine("      [Tags]   " + item1.Desc);
                                                //proc.StandardInput.WriteLine("Suite Setup   Connect To Database   pymysql   ${DBName}  ${DBUser}  ${DBPass}  ${DBHost}  ${DBPort}");
                                                proc.StandardInput.WriteLine("      Connect To Database   pymysql   ${DBName}  ${DBUser}  ${DBPass}  ${DBHost}  ${DBPort}");
                                                //   proc.StandardInput.WriteLine("      " + item1.ExceptedResult + "     " + demo + "");
                                                if (item1.Expextedparameter == null || item1.Expextedparameter == "")
                                                {
                                                    proc.StandardInput.WriteLine("      " + item1.ExceptedResult + "     " + demo + "");
                                                }
                                                else
                                                {
                                                    var paramete = "";
                                                    var testres = item1.Expextedparameter.Split(" ");
                                                    for (int d = 0; d < testres.Length; d++)
                                                    {
                                                        if (d == testres.Length - 1)
                                                        {
                                                            paramete += " " + testres[d];
                                                        }
                                                    }

                                                    proc.StandardInput.WriteLine("      " + paramete + "X     " + demo + "        " + item1.Expextedparameter + "");
                                                }
                                            }
                                            else
                                            {
                                                proc.StandardInput.WriteLine("${DBPort}    1433");
                                                proc.StandardInput.WriteLine("*** Test Cases ***");
                                                proc.StandardInput.WriteLine("" + item1.TestCaseName + "");
                                                proc.StandardInput.WriteLine("      [Tags]   " + item1.Desc);
                                                //proc.StandardInput.WriteLine("Suite Setup   Connect To Database   pymysql   ${DBName}  ${DBUser}  ${DBPass}  ${DBHost}  ${DBPort}");
                                                proc.StandardInput.WriteLine("          Connect To Database Using Custom Params  pyodbc  'DRIVER={ODBC Driver 17 for SQL Server};SERVER=${DBHost};DATABASE=${DBName};UID=${DBUser};PWD=${DBPass}'");
                                                //  proc.StandardInput.WriteLine("      " + item1.ExceptedResult + "     " + demo + "");
                                                if (item1.Expextedparameter == null || item1.Expextedparameter == "")
                                                {
                                                    proc.StandardInput.WriteLine("      " + item1.ExceptedResult + "     " + demo + "");
                                                }
                                                else
                                                {
                                                    var paramete = "";
                                                    var testres = item1.Expextedparameter.Split(" ");
                                                    for (int d = 0; d < testres.Length; d++)
                                                    {
                                                        if (d == testres.Length - 1)
                                                        {
                                                            paramete += " " + testres[d];
                                                        }
                                                    }

                                                    proc.StandardInput.WriteLine("      " + paramete + "X     " + demo + "        " + item1.Expextedparameter + "");
                                                }
                                            }
                                            //    proc.StandardInput.WriteLine("i");

                                            proc.StandardInput.Flush();
                                            proc.StandardInput.Close();
                                            proc.WaitForExit();
                                        }

                                    }
                                }
                            }
                            var testcaselist = testcasename.Split(',');
                            var testcaselaname = "";
                            for (var z = 0; z < testcaselist.Length; z++)
                            {
                                var valuete = testcaselist[z];
                                // var conver = Convert.ToInt32(valuete);
                                // var testfilename = db.WebTestcases.FirstOrDefault(x => x.WebTestcasesid == conver).Webtestcase;
                                if (testcaselaname == "")
                                {
                                    testcaselaname = "-t  " + "'" + valuete + "'";
                                }
                                else
                                {
                                    testcaselaname += "  -t  " + "'" + valuete + "'";
                                }
                            }

                            //   }
                            //for (int i = 0; i < suiteId.Length; i++)
                            //{
                            //    var approach1 = testapproach.Split(',');
                            //    for (int k = 0; k < approach1.Length; k++)
                            //    {
                            //        foreach (var rulesfilte in ruleadd)
                            //        {


                            //            //  var test = rulesfilte;
                            //            //                    }


                            //        }
                            //    }
                            //    // }
                            //}

                            //   }
                            proc.StartInfo.FileName = @"/bin/bash";
                            //     proc.StartInfo.FileName = "cmd.exe";
                            proc.StartInfo.CreateNoWindow = true;
                            proc.StartInfo.RedirectStandardInput = true;
                            proc.StartInfo.RedirectStandardOutput = true;
                            proc.StartInfo.UseShellExecute = false;
                            proc.StartInfo.CreateNoWindow = true;
                            proc.Start();
                            var ReportPortal = configuration.GetValue<string>("ReportPortalIp");
                            var ReportalId = configuration.GetValue<string>("ReportPortalUid");
                            var graphanausr = configuration.GetValue<string>("graphanausername");
                            var graphanapass = configuration.GetValue<string>("graphanapass");
                            var graphanaIp = configuration.GetValue<string>("graphanadbIp");
                            var graphanaport = configuration.GetValue<string>("graphanadbPort");
                            var graphanadbname = configuration.GetValue<string>("graphanaDbname");
                            //   proc.StandardInput.WriteLine("sudo su");
                            var testreport = "http://40.115.111.12:8080";
                            proc.StandardInput.WriteLine("" + fileload + "");
                            //  proc.StandardInput.WriteLine("cd /d D:\\Abinesh-learn");
                            proc.StandardInput.WriteLine("robot  "+ testcaselaname + "   --removekeywords All  --listener robotframework_reportportal.listener  --variable RP_ENDPOINT:"+ ReportPortal + " --variable RP_UUID:"+ ReportalId + " --variable RP_PROJECT:'default_personal' --variable RP_LAUNCH:'" + releaseno + "' --variable rp.attributes:one --variable rp.keystore.password:false  -N  " + tagno + " --variable rp.enable:false  --report " + tagno + "" + releaseno + ".html   -d   " + reportPath + "" + releaseno + "  " + suiteId[0] + "");
                            ///    proc.StandardInput.WriteLine("pybot --listener reportportal_listener --variable RP_ENDPOINT:http://52.157.105.144:8080 --variable RP_UUID:6df6d59c-e0f6-44b0-a8c7-5087f0f36eac --variable RP_LAUNCH:'superadmin_TEST_EXAMPLE' --variable RP_PROJECT:superadmin_personal --report " + SuiteId + "" + Version + ".html  -d  /srv/www/XAutomate/wwwroot/" + SuiteId + "  " + SuiteId + "");
                            //  proc.StandardInput.WriteLine("robot robotframework");
                              proc.StandardInput.WriteLine("python3 -m dbbot.run -b mysql://" + graphanausr + ":" + graphanapass + "@" + graphanaIp + ":" + graphanaport + "/" + graphanadbname + " " + reportPath + "" + releaseno + "/output.xml");
                          //  proc.StandardInput.WriteLine("python3 -m dbbot.run -b mysql://root:password@40.115.111.12:3308/graphana " + reportPath + "" + releaseno + "/output.xml");
                            // proc.StandardInput.WriteLine("python3 -m dbbot.run -b mysql://root:password@40.115.111.12:3308/graphana /srv/www/XAutomate/wwwroot/" + releaseno + "/output.xml");
                            //   proc.StandardInput.WriteLine("python -m dbbot.run -b mysql://" + graphanausr + ":" + graphanapass + "@" + graphanaIp + ":" + graphanaport + "/" + graphanadbname + " /srv/www/XAutomate/wwwroot/" + releaseno + "/output.xml");
                         if(Adoplan == "1")
                            {
                                proc.StandardInput.WriteLine("cd "+ reportPath + "");
                                proc.StandardInput.WriteLine("cp prototype_upload_test_results_to_ado.py "+ reportPath + "" + releaseno + "");
                                proc.StandardInput.WriteLine("cd "+ reportPath + "" + releaseno + "");

                                proc.StandardInput.WriteLine("python3 prototype_upload_test_results_to_ado.py");

                            }
                       
                            proc.StandardInput.Flush();
                            proc.StandardInput.Close();
                            proc.WaitForExit();
                            string Error1 = proc.StandardOutput.ReadToEnd();
                            Console.WriteLine(proc.StandardOutput.ReadToEnd());
                            System.Diagnostics.Debug.WriteLine(proc.StandardOutput.ReadToEnd());
                            if (Adoplan == "1")
                            {

                                proc.StartInfo.FileName = @"/bin/bash";
                                //     proc.StartInfo.FileName = "cmd.exe";
                                proc.StartInfo.CreateNoWindow = true;
                                proc.StartInfo.RedirectStandardInput = true;
                                proc.StartInfo.RedirectStandardOutput = true;
                                proc.StartInfo.UseShellExecute = false;
                                proc.StartInfo.CreateNoWindow = true;
                                proc.Start();

                                proc.StandardInput.WriteLine("cd "+ reportPath + "" + releaseno + "");

                                proc.StandardInput.WriteLine("python3 prototype_upload_test_results_to_ado.py");

                                // proc.StandardInput.WriteLine("python -m dbbot.run -b mysql://" + graphanausr + ":" + graphanapass + "@" + graphanaIp + ":" + graphanaport + "/" + graphanadbname + " /srv/www/XAutomate/wwwroot/" + releaseno + "/output.xml");
                                proc.StandardInput.Flush();
                                proc.StandardInput.Close();
                                proc.WaitForExit();
                                test = proc.StandardOutput.ReadToEnd();

                                //     testerror = proc.StandardError.ReadToEnd();
                                Console.WriteLine(proc.StandardOutput.ReadToEnd());
                                System.Diagnostics.Debug.WriteLine(proc.StandardOutput.ReadToEnd());
                            }
                            var Executetime = DateTime.Now.ToString("HH:mm");
                            MySqlConnection sql = new MySqlConnection("Server=" + graphanaIp + ";Port=" + graphanaport + ";Database=" + graphanadbname + ";User ID=" + graphanausr + ";Password=" + graphanapass + ";SslMode=none");
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
                            ins.Time = Executetime1 + " s";
                            ins.Status = "1";
                            ins.ResultUrl = "/" + releaseno + "/" + tagno + "" + releaseno + ".html";
                            db.Exceute.Add(ins);
                            db.SaveChanges();
                            //int uni = coloumnvalue_common.Count() - dublicatecol;
                            //KpiAttributeTotal kpi = new KpiAttributeTotal();
                            //kpi.enterdatetime = DateTime.Now;
                            //kpi.TotalAttributes = tablecolumn;
                            //kpi.UsedAttribute = coloumnvalue_common.Count();
                            //kpi.UniqueAttributes = coloumnvalue_common.Count() - dublicatecol;
                            //kpi.RemianingAttributes = coloumnvalue_common.Count() - uni;
                            //kpi.Suitename = tagno;
                            //db.KpiAttributeTotal.Add(kpi);
                            //db.SaveChanges();
                            int testcaseccount = 0;
                            var suitename12 = (from b in db.KpiTotal
                                             select new
                                             {
                                                 b.KpiTotalid,
                                                 b.Suitename,
                                                 b.TotalTestcase,
                                             }).ToList();

                            foreach (var item in suitename12)
                            {
                                testcaseccount = 0;
                                var testapproach12 = (from c in db.TestApproach
                                                    where c.SuiteIds.Contains(item.Suitename)
                                                    select new
                                                    {
                                                        c.TestApproachName
                                                    }).ToList();
                                foreach (var testapproach1 in testapproach12)
                                {
                                    var ruleid = (from c in db.Rules
                                                  where c.TestApproachName.Contains(testapproach1.TestApproachName)
                                                  select new
                                                  {
                                                      c.RulesId,
                                                      c.RuleName
                                                  }).ToList();
                                    foreach (var rules in ruleid)
                                    {
                                        testcaseccount += db.TestCases.Where(x => x.RulesId == rules.RulesId).Count();
                                    }
                                }
                                var result = db.KpiTotal.FirstOrDefault(x => x.KpiTotalid == item.KpiTotalid);
                                if (result != null)
                                {
                                    result.LastupdatedDate = DateTime.Now;
                                    result.TotalTestcase = testcaseccount;
                                    db.SaveChanges();
                                }

                            }

                      //      var Errorcount = Error1.Split(",");

                      //      var totaltest = Errorcount[2].Split(" ");

                     //       var Failtest = Errorcount[4].Split(" ");
                            _log4net.Info("Function Name : Execute Trigger Successfully Execute");
                            return Json(list1);
                            //return "";
                            //testcaseresult = totaltest[1] + " " + Errorcount[3] + " " + Failtest[0];


                        }
                        else
                        {
                            _log4net.Error("Function Name : Execute Trigger Auth Failed");
                            return Json("Auth Fail");
                          //  return "";
                        }
                    }
                    else
                    {
                        _log4net.Error("Function Name : Execute Trigger Auth Failed");
                       return Json("Auth Fail");
                       // return "";
                    }
                }
                catch (Exception ex)
                {
                    _log4net.Error("Function Name : Execute Trigger  -- " + ex.ToString());
                  //  return ex.ToString();
                    return Json(ex);
                }
            }
            else
            {
                //    var testcaseresult = "";
                try
                {

                    List<Execute> list1 = new List<Execute>();
                    var header = this.Request.Headers.ToString();
                    var header1 = this.Request.Headers.ToList();
                    var Auth = (string)this.Request.Headers["Authorization"];
                    if (Auth != "" && Auth != null && Auth != "max-age=0")
                    {
                        //      TimeZoneInfo timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
                        // TimeZoneInfo timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Etc/UTC");
                        TimeZoneInfo timeZoneInfo = TZConvert.GetTimeZoneInfo("Europe/Stockholm");
                        DateTime dtCNow = TimeZoneInfo.ConvertTime(Convert.ToDateTime(DateTime.Now), timeZoneInfo);
                        string dtCNowdate = Convert.ToDateTime(dtCNow).ToString("yyyy-MM-dd");
                        TimeSpan tsnow = Convert.ToDateTime(dtCNow).TimeOfDay;
                        DateTime currentdatetime = Convert.ToDateTime(dtCNowdate + " " + (tsnow.Hours) + ":" + tsnow.Minutes);
                        var resultToken = db.TokenValue.FirstOrDefault(b => b.TockenId == Auth && b.Validto >= currentdatetime);
                        if (resultToken != null)
                        {
                            string fileload = configuration.GetValue<string>("Systempath:pagepath");
                            string reportPath = configuration.GetValue<string>("Systempath:ReportPath");
                            string fileload1 = configuration.GetValue<string>("Systempath:uploadpath");
                            var result = "";
                            //   var Resourcefile = ResourceName.Split(',');
                            if (testcasename != null && testcasename != "")
                            {
                                var testapproach1 = testapproach.Split(',');
                                var testcaselist = testcasename.Split(',');
                                var testcaselaname = "";
                                for (var z = 0; z < testcaselist.Length; z++)
                                {
                                    var valuete = testcaselist[z];
                                    // var conver = Convert.ToInt32(valuete);
                                    // var testfilename = db.WebTestcases.FirstOrDefault(x => x.WebTestcasesid == conver).Webtestcase;
                                    if (testcaselaname == "")
                                    {
                                        testcaselaname = "-t  " + "'" + valuete + "'";
                                    }
                                    else
                                    {
                                        testcaselaname += "  -t  " + "'" + valuete + "'";
                                    }
                                }
                                //   var techno = db.WebFilesTechnology.FirstOrDefault(x => x.WebFilesTechnologyid == suitename).WebfileTechnologyFoler;
                                ///Temprovery Comment
                                Process proc = new Process();
                                proc.StartInfo.FileName = @"/bin/bash";
                                //  proc.StartInfo.FileName = "cmd.exe";
                                proc.StartInfo.CreateNoWindow = true;
                                proc.StartInfo.RedirectStandardInput = true;
                                proc.StartInfo.RedirectStandardOutput = true;
                                proc.StartInfo.UseShellExecute = false;
                                proc.StartInfo.CreateNoWindow = true;
                                proc.Start();
                                // proc.StandardInput.WriteLine("cd /d C:\\Users\\Admin\\Downloads");
                                // proc.StandardInput.WriteLine("python prototype_upload_test_results_to_ado.py");
                                //proc.StandardInput.WriteLine("" + fileload + "");
                                //proc.StandardInput.WriteLine("rm -Rf  " + releaseno + "");
                                //proc.StandardInput.WriteLine("mkdir  " + releaseno + " ");
                                proc.StandardInput.Flush();
                                proc.StandardInput.Close();
                                proc.WaitForExit();
                                string Error = proc.StandardOutput.ReadToEnd();
                                Console.WriteLine(proc.StandardOutput.ReadToEnd());
                                System.Diagnostics.Debug.WriteLine(proc.StandardOutput.ReadToEnd());
                              //  var testfile1 = Convert.ToInt32(testapproach);
                                //var testcaseId = db.WebFiles.FirstOrDefault(x => x.WebFilesId == testfile1).FileName;
                                proc.StartInfo.FileName = @"/bin/bash";
                                //     proc.StartInfo.FileName = "cmd.exe";
                                proc.StartInfo.CreateNoWindow = true;
                                proc.StartInfo.RedirectStandardInput = true;
                                proc.StartInfo.RedirectStandardOutput = true;
                                proc.StartInfo.UseShellExecute = false;
                                proc.StartInfo.CreateNoWindow = true;
                                proc.Start();
                                var ReportPortal = configuration.GetValue<string>("ReportPortalIp");
                                var ReportalId = configuration.GetValue<string>("ReportPortalUid");
                                var graphanausr = configuration.GetValue<string>("graphanausername");
                                var graphanapass = configuration.GetValue<string>("graphanapass");
                                var graphanaIp = configuration.GetValue<string>("graphanadbIp");
                                var graphanaport = configuration.GetValue<string>("graphanadbPort");
                                var graphanadbname = configuration.GetValue<string>("graphanaDbname");
                                proc.StandardInput.WriteLine("" + fileload + "");
                                //  proc.StandardInput.WriteLine("cd /d D:\\Abinesh-learn");
                                proc.StandardInput.WriteLine("robot  " + testcaselaname + " --removekeywords All  --listener robotframework_reportportal.listener  --variable RP_ENDPOINT:" + ReportPortal + " --variable RP_UUID:" + ReportalId + " --variable RP_PROJECT:'default_personal' --variable RP_LAUNCH:'" + releaseno + "' --report " + tagno + "" + testfile + ".html  --log " + tagno + "" + releaseno + "log.html   -d   " + reportPath + "" + releaseno + "  " + suitename + "");
                                //proc.StandardInput.WriteLine("robot  " + testcaselaname + "  --report " + tagno + "" + testfile + ".html  --log " + tagno + "" + releaseno + "log.html   -d   " + reportPath + "" + releaseno + "  File");
                                if (Adoplan == "1")
                                {
                                    proc.StandardInput.WriteLine("cd "+ reportPath + "");
                                    proc.StandardInput.WriteLine("cp prototype_upload_test_results_to_ado.py "+ reportPath + "" + releaseno + "");
                                    proc.StandardInput.WriteLine("cd "+ reportPath + "" + releaseno + "");

                                    proc.StandardInput.WriteLine("python3 prototype_upload_test_results_to_ado.py");
                                }
                                //  proc.StandardInput.WriteLine("robot robotframework");
                                proc.StandardInput.WriteLine("python3 -m dbbot.run -b  mysql://" + graphanausr + ":" + graphanapass + "@" + graphanaIp + ":" + graphanaport + "/" + graphanadbname + "  " + reportPath + "" + releaseno + "/output.xml");

                                // proc.StandardInput.WriteLine("python -m dbbot.run -b mysql://" + graphanausr + ":" + graphanapass + "@" + graphanaIp + ":" + graphanaport + "/" + graphanadbname + " /srv/www/XAutomate/wwwroot/" + releaseno + "/output.xml");
                                proc.StandardInput.Flush();
                                proc.StandardInput.Close();
                                proc.WaitForExit();
                                test = proc.StandardOutput.ReadToEnd();
                                Console.WriteLine(proc.StandardOutput.ReadToEnd());
                                System.Diagnostics.Debug.WriteLine(proc.StandardOutput.ReadToEnd());

                                if (Adoplan == "1")
                                {
                                    proc.StartInfo.FileName = @"/bin/bash";
                                    //     proc.StartInfo.FileName = "cmd.exe";
                                    proc.StartInfo.CreateNoWindow = true;
                                    proc.StartInfo.RedirectStandardInput = true;
                                    proc.StartInfo.RedirectStandardOutput = true;
                                    proc.StartInfo.UseShellExecute = false;
                                    proc.StartInfo.CreateNoWindow = true;
                                    proc.Start();

                                    proc.StandardInput.WriteLine("cd "+ reportPath + "" + releaseno + "");

                                    proc.StandardInput.WriteLine("python3 prototype_upload_test_results_to_ado.py");

                                    // proc.StandardInput.WriteLine("python -m dbbot.run -b mysql://" + graphanausr + ":" + graphanapass + "@" + graphanaIp + ":" + graphanaport + "/" + graphanadbname + " /srv/www/XAutomate/wwwroot/" + releaseno + "/output.xml");
                                    proc.StandardInput.Flush();
                                    proc.StandardInput.Close();
                                    proc.WaitForExit();
                                    test = proc.StandardOutput.ReadToEnd();

                                    //     testerror = proc.StandardError.ReadToEnd();
                                    Console.WriteLine(proc.StandardOutput.ReadToEnd());
                                    System.Diagnostics.Debug.WriteLine(proc.StandardOutput.ReadToEnd());
                                }
                                var Executetime = DateTime.Now.ToString("HH:mm");

                                MySqlConnection sql = new MySqlConnection("Server="+ graphanaIp + ";Port="+ graphanaport + ";Database="+ graphanadbname + ";User ID="+ graphanausr + ";Password="+ graphanapass + ";SslMode=none");
                                sql.Open();
                                var suitename1 = testapproach.Split(".");
                                var suitelate = suitename1[0];
                                MySqlCommand com = new MySqlCommand("select  * from graphana.suite_status  join graphana.suites  on  suite_status.suite_id = suites.id where suites.name='" + suitelate + "' order by suite_status.id desc Limit 1 ", sql);
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
                                ins.Status = "1";
                                ins.Time = Executetime1 + " s";
                                ins.ResultUrl = "/" + releaseno + "/" + tagno + "" + testfile + ".html";
                                db.Exceute.Add(ins);
                                db.SaveChanges();
                                _log4net.Info("Function Name : Web Execute Excuted successfully" + test + " error " + testerror);
                                return Json(list1);
                                   //  return "";
                            }
                            else
                            {
                                var testfile1 = Convert.ToInt32(testapproach);
                                var testcaseId = db.WebFiles.FirstOrDefault(x => x.WebFilesId == testfile1).FileName;
                                Process proc = new Process();
                                proc.StartInfo.FileName = @"/bin/bash";
                                //     proc.StartInfo.FileName = "cmd.exe";
                                proc.StartInfo.CreateNoWindow = true;
                                proc.StartInfo.RedirectStandardInput = true;
                                proc.StartInfo.RedirectStandardOutput = true;
                                proc.StartInfo.UseShellExecute = false;
                                proc.StartInfo.CreateNoWindow = true;
                                proc.Start();
                                proc.StandardInput.WriteLine("" + fileload + "");
                                //  proc.StandardInput.WriteLine("cd /d D:\\Abinesh-learn");
                                proc.StandardInput.WriteLine("python3  --listener robotframework_reportportal.listener --variable RP_ENDPOINT:" +configuration.GetValue<string>("ReportPortalIp") + " --variable RP_UUID:" + HttpContext.Session.GetString("ReportportalId") + "  --variable RP_PROJECT:'default_personal' --variable RP_LAUNCH:'" + testfile + "'  -N  " + tagno + "  --report " + tagno + "" + testfile + ".html  --log " + tagno + "" + releaseno + "log.html   -d   " + reportPath + "" + testfile + "   " + testcaseId + "");

                                //  proc.StandardInput.WriteLine("robot robotframework");
                                proc.StandardInput.WriteLine("python3 -m dbbot.run -b mysql://" + HttpContext.Session.GetString("graphanauser") + ":" + HttpContext.Session.GetString("graphanapass") + "@" + HttpContext.Session.GetString("graphanadbip") + ":" + HttpContext.Session.GetString("graphanaport") + "/" + HttpContext.Session.GetString("graphananame") + " " + reportPath + "" + testfile + "/output.xml");
                                proc.StandardInput.Flush();
                                proc.StandardInput.Close();
                                proc.WaitForExit();
                                string Error1 = proc.StandardOutput.ReadToEnd();
                                Console.WriteLine(proc.StandardOutput.ReadToEnd());
                                System.Diagnostics.Debug.WriteLine(proc.StandardOutput.ReadToEnd());
                                var Executetime = DateTime.Now.ToString("HH:mm");
                                MySqlConnection sql = new MySqlConnection("Server=" + HttpContext.Session.GetString("graphanadbip") + ";Port=" + HttpContext.Session.GetString("graphanaport") + ";Database=" + HttpContext.Session.GetString("graphananame") + ";User ID=" + HttpContext.Session.GetString("graphanauser") + ";Password=" + HttpContext.Session.GetString("graphanapass") + ";SslMode=none");
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
                                ins.Status = "1";
                                ins.Time = Executetime1 + " s";
                                ins.ResultUrl = "/" + testfile + "/" + tagno + "" + result.Substring(0, 6) + ".html";
                                db.Exceute.Add(ins);
                                db.SaveChanges();
                                var Errorcount = Error1.Split(",");

                                var totaltest = Errorcount[2].Split(" ");

                                var Failtest = Errorcount[4].Split(" ");
                                _log4net.Info("Function Name : Web Execute Excuted successfully");
                                return Json(list1);
                                //testcaseresult = totaltest[1] + " " + Errorcount[3] + " " + Failtest[0];
                               //      return "";
                            }

                        }
                        else
                        {
                            _log4net.Error("Function Name : Web Execute Auth Failed");
                            return Json("Auth Fail");
                            //  return "";
                        }
                    }
                    else
                    {
                        _log4net.Error("Function Name : Web Execute Auth Failed");
                        return Json("Auth Fail");
                       // return "";
                    }
                }
                catch (Exception ex)
                {
                    _log4net.Error("Function Name : Web Execute  -- " + ex.ToString());
                    return Json(ex.ToString());
                   //     return ex.ToString() +"  " + zz + "     " + res1;
                }
            }

        }



        public JsonResult WebAPIExecutetech(string suitename, string technology, string tagno, string testapproach, string testcasename, string releaseno, string connection, string Adoplan)
        {
           // var testcaseresult = "";
           // int zz = 0;
            var testfile = "";
            //int res1 = 0;
            var test = "testdemo";
            var testerror = "testdemo";
            if (technology == "ETL/DB")
            {
               // string[] testapproachcheck;
                List<int> ruleadd = new List<int>();
                try
                {


                    int MY = 0;
                    int SQ = 0;
                    List<Execute> list1 = new List<Execute>();
                   
                      
                            string fileload = configuration.GetValue<string>("Systempath:pagepath");
                            string reportPath = configuration.GetValue<string>("Systempath:ReportPath");
                            string External = configuration.GetValue<string>("Systempath:Externalproperties");
                            //string[] lines;
                            //var filePath = Path.Combine(Directory.GetCurrentDirectory() + "" + External + "");

                            //var fileCount = new StreamReader(filePath).ReadToEnd(); // big string
                            //lines = fileCount.Split(new char[] { '\n' });           // big array
                            //if (lines.Length == 1)
                            //{
                            //    lines = fileCount.Split(new char[] { '\r' });
                            //}
                            //for (int i = 0; i < lines.Length; i++)
                            //{
                            //    var linesval = lines[i];
                            //    linesval = linesval.Replace(" ", "");
                            //    if (linesval.Contains("ReportPortalIp"))
                            //    {
                            //        var ReportPortal1 = lines[i + 1];
                            //        ReportPortal1 = ReportPortal1.Replace(" ", "");
                            //        HttpContext.Session.SetString("ReportPortal", ReportPortal1);
                            //        ViewBag.Report =configuration.GetValue<string>("ReportPortalIp");
                            //    }
                            //    if (linesval.Contains("graphanaIp"))
                            //    {
                            //        var graphana = lines[i + 1];
                            //        graphana = graphana.Replace(" ", "");
                            //        HttpContext.Session.SetString("graphanaIp", graphana);
                            //    }
                            //    if (linesval.Contains("graphanadbIp"))
                            //    {
                            //        var graphanadbip = lines[i + 1];
                            //        graphanadbip = graphanadbip.Replace(" ", "");
                            //        HttpContext.Session.SetString("graphanadbip", graphanadbip);
                            //    }
                            //    if (linesval.Contains("graphanadbPort"))
                            //    {
                            //        var graphanaport1 = lines[i + 1];
                            //        graphanaport1 = graphanaport1.Replace(" ", "");
                            //        HttpContext.Session.SetString("graphanaport", graphanaport1);
                            //    }
                            //    if (linesval.Contains("graphanausername"))
                            //    {
                            //        var graphanauser = lines[i + 1];
                            //        graphanauser = graphanauser.Replace(" ", "");
                            //        HttpContext.Session.SetString("graphanauser", graphanauser);
                            //    }
                            //    if (linesval.Contains("graphanapass"))
                            //    {
                            //        var graphanapass1 = lines[i + 1];
                            //        graphanapass1 = graphanapass1.Replace(" ", "");
                            //        HttpContext.Session.SetString("graphanapass", graphanapass1);
                            //    }
                            //    if (linesval.Contains("graphanaDbname"))
                            //    {
                            //        var graphananame = lines[i + 1];
                            //        graphananame = graphananame.Replace(" ", "");
                            //        HttpContext.Session.SetString("graphananame", graphananame);
                            //    }
                            //    if (linesval.Contains("ReportPortalUid"))
                            //    {
                            //        var Reportportal = lines[i + 1];
                            //        Reportportal = Reportportal.Replace(" ", "");
                            //        HttpContext.Session.SetString("ReportportalId", Reportportal);
                            //    }
                            //}
                          //  var result = "";
                            var suiteId = suitename.Split(',');
                            //var dbconfig1 = dbconnection.Split(',');
                            var approach = testapproach.Split(',');
                            Process proc = new Process();
                            for (int i = 0; i < suiteId.Length; i++)
                            {
                                proc.StartInfo.FileName = @"/bin/bash";

                                proc.StartInfo.CreateNoWindow = true;
                                proc.StartInfo.RedirectStandardInput = true;
                                proc.StartInfo.RedirectStandardOutput = true;
                                proc.StartInfo.UseShellExecute = false;
                                proc.StartInfo.CreateNoWindow = true;
                                proc.Start();

                                proc.StandardInput.WriteLine("" + fileload + "");
                                proc.StandardInput.WriteLine("rm -Rf  " + suiteId[0] + "");
                                proc.StandardInput.WriteLine("mkdir  " + suiteId[0] + " ");
                                proc.StandardInput.Flush();
                                proc.StandardInput.Close();
                                proc.WaitForExit();
                                proc.StandardOutput.ReadToEnd();
                                Console.WriteLine(proc.StandardOutput.ReadToEnd());
                                System.Diagnostics.Debug.WriteLine(proc.StandardOutput.ReadToEnd());
                                var approach1 = testapproach.Split(',');


                                for (int k = 0; k < approach1.Length; k++)
                                {
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
                                        //return "";
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
                                        var connections = db.DbConfig.FirstOrDefault(x => x.DbName == connection);

                                        if (connections.DatabaseType == "MYSQL")
                                        {
                                            if (MY == 0)
                                            {
                                                //   result = db.TestSuite.FirstOrDefault(x => x.TestSuiteId == suiteid).TestSuitename;
                                                proc.StartInfo.FileName = @"/bin/bash";
                                                proc.StartInfo.CreateNoWindow = true;
                                                proc.StartInfo.RedirectStandardInput = true;
                                                proc.StartInfo.RedirectStandardOutput = true;
                                                proc.StartInfo.UseShellExecute = false;
                                                proc.StartInfo.CreateNoWindow = true;
                                                proc.Start();


                                                proc.StandardInput.WriteLine("" + fileload + "" + suiteId[i] + "");

                                                proc.StandardInput.WriteLine("cat >  " + approach1[k] + ".robot ");
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


                                                    proc.StandardInput.WriteLine("" + fileload + "" + suiteId[i] + "");

                                                    proc.StandardInput.WriteLine("cat >  " + approach1[k] + ".robot ");
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
                            foreach (var ruleitem in rules)
                            {
                                //if (ruleadd.Count() == 0)
                                //{
                                //    ruleadd.Add((int)ruleitem.RulesId);
                                //}
                                //else if (!ruleadd.Contains((int)ruleitem.RulesId))
                                //{
                                //    ruleadd.Add((int)ruleitem.RulesId);
                                //}
                                var testcase = (from b in db.TestCases
                                                where b.RulesId == ruleitem.RulesId && b.Status == "1"
                                                select new
                                                {
                                                    b.Testdata,
                                                    b.ExceptedResult,
                                                    testcaseparam = b.TestCaseParameters.Select(x => new
                                                    {
                                                        x.ParameterName,
                                                    }),
                                                    RuleCondtion = b.Rules.RuleCondtion,
                                                    b.Expextedparameter,
                                                    Description = b.Rules.Description,
                                                    Desc = b.Description,
                                                    b.TestCaseName
                                                }).ToList();
                                //if (testcase.Count() == 0)
                                //{
                                //    return Json(item.RuleName + "Testcase Not Available");
                                //}
                                foreach (var item1 in testcase)
                                {
                                    var connections = db.DbConfig.FirstOrDefault(x => x.DbName == connection);
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
                                    proc.StandardInput.WriteLine("" + fileload + "" + suiteId[i] + "");
                                    // proc.StandardInput.WriteLine("touch " + Name + ".robot ");
                                    proc.StandardInput.WriteLine("cat >> " + approach1[k] + ".robot ");
                                    proc.StandardInput.WriteLine("*** Variables  ***");
                                    proc.StandardInput.WriteLine("${DBName}   " + connections.DbName + "");
                                    proc.StandardInput.WriteLine("${DBUser}   " + connections.DbUser + "");
                                    proc.StandardInput.WriteLine("${DBPass}  " + connections.DbPassword + "");
                                    proc.StandardInput.WriteLine("${DBHost}  " + connections.DbHostName + "");

                                    if (connections.DatabaseType == "MYSQL")
                                    {
                                        proc.StandardInput.WriteLine("${DBPort}    " + connections.DbPort + "");
                                        proc.StandardInput.WriteLine("*** Test Cases ***");
                                        proc.StandardInput.WriteLine("" + item1.TestCaseName + "");
                                        proc.StandardInput.WriteLine("      [Tags]   " + item1.Desc);
                                        //proc.StandardInput.WriteLine("Suite Setup   Connect To Database   pymysql   ${DBName}  ${DBUser}  ${DBPass}  ${DBHost}  ${DBPort}");
                                        proc.StandardInput.WriteLine("      Connect To Database   pymysql   ${DBName}  ${DBUser}  ${DBPass}  ${DBHost}  ${DBPort}");
                                        //   proc.StandardInput.WriteLine("      " + item1.ExceptedResult + "     " + demo + "");
                                        if (item1.Expextedparameter == null || item1.Expextedparameter == "")
                                        {
                                            proc.StandardInput.WriteLine("      " + item1.ExceptedResult + "     " + demo + "");
                                        }
                                        else
                                        {
                                            var paramete = "";
                                            var testres = item1.Expextedparameter.Split(" ");
                                            for (int d = 0; d < testres.Length; d++)
                                            {
                                                if (d == testres.Length - 1)
                                                {
                                                    paramete += " " + testres[d];
                                                }
                                            }

                                            proc.StandardInput.WriteLine("      " + paramete + "X     " + demo + "        " + item1.Expextedparameter + "");
                                        }
                                    }
                                    else
                                    {
                                        proc.StandardInput.WriteLine("${DBPort}    1433");
                                        proc.StandardInput.WriteLine("*** Test Cases ***");
                                        proc.StandardInput.WriteLine("" + item1.TestCaseName + "");
                                        proc.StandardInput.WriteLine("      [Tags]   " + item1.Desc);
                                        //proc.StandardInput.WriteLine("Suite Setup   Connect To Database   pymysql   ${DBName}  ${DBUser}  ${DBPass}  ${DBHost}  ${DBPort}");
                                        proc.StandardInput.WriteLine("          Connect To Database Using Custom Params  pyodbc  'DRIVER={ODBC Driver 17 for SQL Server};SERVER=${DBHost};DATABASE=${DBName};UID=${DBUser};PWD=${DBPass}'");
                                        //  proc.StandardInput.WriteLine("      " + item1.ExceptedResult + "     " + demo + "");
                                        if (item1.Expextedparameter == null || item1.Expextedparameter == "")
                                        {
                                            proc.StandardInput.WriteLine("      " + item1.ExceptedResult + "     " + demo + "");
                                        }
                                        else
                                        {
                                            var paramete = "";
                                            var testres = item1.Expextedparameter.Split(" ");
                                            for (int d = 0; d < testres.Length; d++)
                                            {
                                                if (d == testres.Length - 1)
                                                {
                                                    paramete += " " + testres[d];
                                                }
                                            }

                                            proc.StandardInput.WriteLine("      " + paramete + "X     " + demo + "        " + item1.Expextedparameter + "");
                                        }
                                    }
                                    //    proc.StandardInput.WriteLine("i");

                                    proc.StandardInput.Flush();
                                    proc.StandardInput.Close();
                                    proc.WaitForExit();
                                }

                            }
                        }
                            }
                            var testcaselist = testcasename.Split(',');
                            var testcaselaname = "";
                            for (var z = 0; z < testcaselist.Length; z++)
                            {
                                var valuete = testcaselist[z];
                                // var conver = Convert.ToInt32(valuete);
                                // var testfilename = db.WebTestcases.FirstOrDefault(x => x.WebTestcasesid == conver).Webtestcase;
                                if (testcaselaname == "")
                                {
                                    testcaselaname = "-t  " + "'" + valuete + "'";
                                }
                                else
                                {
                                    testcaselaname += "  -t  " + "'" + valuete + "'";
                                }
                            }
                            //   }
                     

                            //   }
                            proc.StartInfo.FileName = @"/bin/bash";
                            //     proc.StartInfo.FileName = "cmd.exe";
                            proc.StartInfo.CreateNoWindow = true;
                            proc.StartInfo.RedirectStandardInput = true;
                            proc.StartInfo.RedirectStandardOutput = true;
                            proc.StartInfo.UseShellExecute = false;
                            proc.StartInfo.CreateNoWindow = true;
                            proc.Start();
                    var ReportPortal = configuration.GetValue<string>("ReportPortalIp");
                    var ReportalId = configuration.GetValue<string>("ReportPortalUid");
                    var graphanausr = configuration.GetValue<string>("graphanausername");
                    var graphanapass = configuration.GetValue<string>("graphanapass");
                    var graphanaIp = configuration.GetValue<string>("graphanadbIp");
                    var graphanaport = configuration.GetValue<string>("graphanadbPort");
                    var graphanadbname = configuration.GetValue<string>("graphanaDbname");
                    //   proc.StandardInput.WriteLine("sudo su");
                    proc.StandardInput.WriteLine("" + fileload + "");
                            //  proc.StandardInput.WriteLine("cd /d D:\\Abinesh-learn");
                            proc.StandardInput.WriteLine("robot  " + testcaselaname + "  --removekeywords All  --listener robotframework_reportportal.listener  --variable RP_ENDPOINT:" + ReportPortal + " --variable RP_UUID:" + ReportalId + "  --variable RP_PROJECT:'default_personal' --variable RP_LAUNCH:'" + releaseno + "' --variable rp.attributes:one --variable rp.keystore.password:false  --variable rp.enable:false   -N  " + tagno + "  --report " + tagno + "" + releaseno + ".html   -d   " + reportPath + "" + releaseno + "  " + suiteId[0] + "");
                            ///    proc.StandardInput.WriteLine("pybot --listener reportportal_listener --variable RP_ENDPOINT:http://52.157.105.144:8080 --variable RP_UUID:6df6d59c-e0f6-44b0-a8c7-5087f0f36eac --variable RP_LAUNCH:'superadmin_TEST_EXAMPLE' --variable RP_PROJECT:superadmin_personal --report " + SuiteId + "" + Version + ".html  -d  /srv/www/XAutomate/wwwroot/" + SuiteId + "  " + SuiteId + "");
                            //  proc.StandardInput.WriteLine("robot robotframework");
                            //  proc.StandardInput.WriteLine("python -m dbbot.run -b mysql://" + graphanausr + ":" + graphanapass + "@" + graphanaIp + ":" + graphanaport + "/" + graphanadbname + " /srv/www/XAutomate/wwwroot/" + releaseno + "/output.xml");
                            proc.StandardInput.WriteLine("python -m dbbot.run -b mysql://" + graphanausr + ":" + graphanapass + "@" + graphanaIp + ":" + graphanaport + "/" + graphanadbname + " " + reportPath + "" + releaseno + "/output.xml");
                            // proc.StandardInput.WriteLine("python -m dbbot.run -b mysql://root:password@40.115.111.12:3308/graphana /srv/www/XAutomate/wwwroot/" + releaseno + "/output.xml");
                            //   proc.StandardInput.WriteLine("python -m dbbot.run -b mysql://" + graphanausr + ":" + graphanapass + "@" + graphanaIp + ":" + graphanaport + "/" + graphanadbname + " /srv/www/XAutomate/wwwroot/" + releaseno + "/output.xml");
                            if (Adoplan == "1")
                            {
                                proc.StandardInput.WriteLine("cd "+ reportPath + "");
                                proc.StandardInput.WriteLine("cp prototype_upload_test_results_to_ado.py "+ reportPath + "" + releaseno + "");
                                proc.StandardInput.WriteLine("cd "+ reportPath + "" + releaseno + "");

                                proc.StandardInput.WriteLine("python prototype_upload_test_results_to_ado.py");

                            }

                            proc.StandardInput.Flush();
                            proc.StandardInput.Close();
                            proc.WaitForExit();
                            string Error1 = proc.StandardOutput.ReadToEnd();
                            Console.WriteLine(proc.StandardOutput.ReadToEnd());
                            System.Diagnostics.Debug.WriteLine(proc.StandardOutput.ReadToEnd());
                            if (Adoplan == "1")
                            {

                                proc.StartInfo.FileName = @"/bin/bash";
                                //     proc.StartInfo.FileName = "cmd.exe";
                                proc.StartInfo.CreateNoWindow = true;
                                proc.StartInfo.RedirectStandardInput = true;
                                proc.StartInfo.RedirectStandardOutput = true;
                                proc.StartInfo.UseShellExecute = false;
                                proc.StartInfo.CreateNoWindow = true;
                                proc.Start();

                                proc.StandardInput.WriteLine("cd "+ reportPath + "" + releaseno + "");

                                proc.StandardInput.WriteLine("python prototype_upload_test_results_to_ado.py");

                                // proc.StandardInput.WriteLine("python -m dbbot.run -b mysql://" + graphanausr + ":" + graphanapass + "@" + graphanaIp + ":" + graphanaport + "/" + graphanadbname + " /srv/www/XAutomate/wwwroot/" + releaseno + "/output.xml");
                                proc.StandardInput.Flush();
                                proc.StandardInput.Close();
                                proc.WaitForExit();
                                test = proc.StandardOutput.ReadToEnd();

                                //     testerror = proc.StandardError.ReadToEnd();
                                Console.WriteLine(proc.StandardOutput.ReadToEnd());
                                System.Diagnostics.Debug.WriteLine(proc.StandardOutput.ReadToEnd());
                            }
                            var Executetime = DateTime.Now.ToString("HH:mm");
                            MySqlConnection sql = new MySqlConnection("Server=" + graphanaIp + ";Port=" + graphanaport + ";Database=" + graphanadbname + ";User ID=" + graphanausr + ";Password=" + graphanapass + ";SslMode=none");
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
                            ins.Time = Executetime1 + " s";
                            ins.Status = "1";
                            ins.ResultUrl = "/" + releaseno + "/" + tagno + "" + releaseno + ".html";
                            db.Exceute.Add(ins);
                            db.SaveChanges();
                            var Errorcount = Error1.Split(",");

                            var totaltest = Errorcount[2].Split(" ");

                            var Failtest = Errorcount[4].Split(" ");
                            _log4net.Info("Function Name : Execute Trigger Successfully Execute");
                            return Json(list1);
                            //return "";
                            //testcaseresult = totaltest[1] + " " + Errorcount[3] + " " + Failtest[0];


                      
                    }
                  
                
                catch (Exception ex)
                {
                    _log4net.Error("Function Name : Execute Trigger  -- " + ex.ToString());
                    //  return ex.ToString();
                    return Json(ex);
                }
            }
            else
            {
                //    var testcaseresult = "";
                try
                {

                    List<Execute> list1 = new List<Execute>();
                  
                        //      TimeZoneInfo timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
                        // TimeZoneInfo timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Etc/UTC");
                        TimeZoneInfo timeZoneInfo = TZConvert.GetTimeZoneInfo("Europe/Stockholm");
                        DateTime dtCNow = TimeZoneInfo.ConvertTime(Convert.ToDateTime(DateTime.Now), timeZoneInfo);
                        string dtCNowdate = Convert.ToDateTime(dtCNow).ToString("yyyy-MM-dd");
                        TimeSpan tsnow = Convert.ToDateTime(dtCNow).TimeOfDay;
                        DateTime currentdatetime = Convert.ToDateTime(dtCNowdate + " " + (tsnow.Hours) + ":" + tsnow.Minutes);
                      
                        string fileload = configuration.GetValue<string>("Systempath:pagepath");
                            string reportPath = configuration.GetValue<string>("Systempath:ReportPath");
                            string fileload1 = configuration.GetValue<string>("Systempath:uploadpath");
                            var result = "";
                            //   var Resourcefile = ResourceName.Split(',');
                            if (testcasename != null && testcasename != "")
                            {
                                var testapproach1 = testapproach.Split(',');
                                var testcaselist = testcasename.Split(',');
                                var testcaselaname = "";
                                for (var z = 0; z < testcaselist.Length; z++)
                                {
                                    var valuete = testcaselist[z];
                                    // var conver = Convert.ToInt32(valuete);
                                    // var testfilename = db.WebTestcases.FirstOrDefault(x => x.WebTestcasesid == conver).Webtestcase;
                                    if (testcaselaname == "")
                                    {
                                        testcaselaname = "-t  " + "'" + valuete + "'";
                                    }
                                    else
                                    {
                                        testcaselaname += "  -t  " + "'" + valuete + "'";
                                    }
                                }
                                //   var techno = db.WebFilesTechnology.FirstOrDefault(x => x.WebFilesTechnologyid == suitename).WebfileTechnologyFoler;
                                ///Temprovery Comment
                                Process proc = new Process();
                                proc.StartInfo.FileName = @"/bin/bash";
                                //  proc.StartInfo.FileName = "cmd.exe";
                                proc.StartInfo.CreateNoWindow = true;
                                proc.StartInfo.RedirectStandardInput = true;
                                proc.StartInfo.RedirectStandardOutput = true;
                                proc.StartInfo.UseShellExecute = false;
                                proc.StartInfo.CreateNoWindow = true;
                                proc.Start();
                                // proc.StandardInput.WriteLine("cd /d C:\\Users\\Admin\\Downloads");
                                // proc.StandardInput.WriteLine("python prototype_upload_test_results_to_ado.py");
                                //proc.StandardInput.WriteLine("" + fileload + "");
                                //proc.StandardInput.WriteLine("rm -Rf  " + releaseno + "");
                                //proc.StandardInput.WriteLine("mkdir  " + releaseno + " ");
                                proc.StandardInput.Flush();
                                proc.StandardInput.Close();
                                proc.WaitForExit();
                                string Error = proc.StandardOutput.ReadToEnd();
                                Console.WriteLine(proc.StandardOutput.ReadToEnd());
                                System.Diagnostics.Debug.WriteLine(proc.StandardOutput.ReadToEnd());
                                //  var testfile1 = Convert.ToInt32(testapproach);
                                //var testcaseId = db.WebFiles.FirstOrDefault(x => x.WebFilesId == testfile1).FileName;
                                proc.StartInfo.FileName = @"/bin/bash";
                                //     proc.StartInfo.FileName = "cmd.exe";
                                proc.StartInfo.CreateNoWindow = true;
                                proc.StartInfo.RedirectStandardInput = true;
                                proc.StartInfo.RedirectStandardOutput = true;
                                proc.StartInfo.UseShellExecute = false;
                                proc.StartInfo.CreateNoWindow = true;
                                proc.Start();
                        var ReportPortal = configuration.GetValue<string>("ReportPortalIp");
                        var ReportalId = configuration.GetValue<string>("ReportPortalUid");
                        var graphanausr = configuration.GetValue<string>("graphanausername");
                        var graphanapass = configuration.GetValue<string>("graphanapass");
                        var graphanaIp = configuration.GetValue<string>("graphanadbIp");
                        var graphanaport = configuration.GetValue<string>("graphanadbPort");
                        var graphanadbname = configuration.GetValue<string>("graphanaDbname");
                        proc.StandardInput.WriteLine("" + fileload + "");
                                //  proc.StandardInput.WriteLine("cd /d D:\\Abinesh-learn");
                                proc.StandardInput.WriteLine("robot  " + testcaselaname + " --removekeywords All  --listener robotframework_reportportal.listener  --variable RP_ENDPOINT:" + ReportPortal + " --variable RP_UUID:" + ReportalId + "  --variable RP_PROJECT:'default_personal' --variable RP_LAUNCH:'" + releaseno + "' --report " + tagno + "" + testfile + ".html  --log " + tagno + "" + releaseno + "log.html   -d   " + reportPath + "" + releaseno + "  " + suitename + "");
                                //proc.StandardInput.WriteLine("robot  " + testcaselaname + "  --report " + tagno + "" + testfile + ".html  --log " + tagno + "" + releaseno + "log.html   -d   " + reportPath + "" + releaseno + "  File");
                                if (Adoplan == "1")
                                {
                                    proc.StandardInput.WriteLine("cd "+ reportPath + "");
                                    proc.StandardInput.WriteLine("cp prototype_upload_test_results_to_ado.py "+ reportPath + "" + releaseno + "");
                                    proc.StandardInput.WriteLine("cd "+ reportPath + "" + releaseno + "");

                                    proc.StandardInput.WriteLine("python prototype_upload_test_results_to_ado.py");
                                }
                                //  proc.StandardInput.WriteLine("robot robotframework");
                             //   proc.StandardInput.WriteLine("python -m dbbot.run -b mysql://root:password@40.115.111.12:3308/graphana /srv/www/XAutomate/wwwroot/" + releaseno + "/output.xml");

                                proc.StandardInput.WriteLine("python -m dbbot.run -b mysql://" + graphanausr + ":" + graphanapass + "@" + graphanaIp + ":" + graphanaport + "/" + graphanadbname + " "+ reportPath + "" + releaseno + "/output.xml");
                                proc.StandardInput.Flush();
                                proc.StandardInput.Close();
                                proc.WaitForExit();
                                test = proc.StandardOutput.ReadToEnd();
                                Console.WriteLine(proc.StandardOutput.ReadToEnd());
                                System.Diagnostics.Debug.WriteLine(proc.StandardOutput.ReadToEnd());

                                if (Adoplan == "1")
                                {
                                    proc.StartInfo.FileName = @"/bin/bash";
                                    //     proc.StartInfo.FileName = "cmd.exe";
                                    proc.StartInfo.CreateNoWindow = true;
                                    proc.StartInfo.RedirectStandardInput = true;
                                    proc.StartInfo.RedirectStandardOutput = true;
                                    proc.StartInfo.UseShellExecute = false;
                                    proc.StartInfo.CreateNoWindow = true;
                                    proc.Start();

                                    proc.StandardInput.WriteLine("cd "+ reportPath + "" + releaseno + "");

                                    proc.StandardInput.WriteLine("python prototype_upload_test_results_to_ado.py");

                                    // proc.StandardInput.WriteLine("python -m dbbot.run -b mysql://" + graphanausr + ":" + graphanapass + "@" + graphanaIp + ":" + graphanaport + "/" + graphanadbname + " /srv/www/XAutomate/wwwroot/" + releaseno + "/output.xml");
                                    proc.StandardInput.Flush();
                                    proc.StandardInput.Close();
                                    proc.WaitForExit();
                                    test = proc.StandardOutput.ReadToEnd();

                                    //     testerror = proc.StandardError.ReadToEnd();
                                    Console.WriteLine(proc.StandardOutput.ReadToEnd());
                                    System.Diagnostics.Debug.WriteLine(proc.StandardOutput.ReadToEnd());
                                }
                                var Executetime = DateTime.Now.ToString("HH:mm");

                        MySqlConnection sql = new MySqlConnection("Server=" + graphanaIp + ";Port=" + graphanaport + ";Database=" + graphanadbname + ";User ID=" + graphanausr + ";Password=" + graphanapass + ";SslMode=none");
                        sql.Open();
                                var suitename1 = testapproach.Split(".");
                                var suitelate = suitename1[0];
                                MySqlCommand com = new MySqlCommand("select  * from graphana.suite_status  join graphana.suites  on  suite_status.suite_id = suites.id where suites.name='" + suitelate + "' order by suite_status.id desc Limit 1 ", sql);
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
                                ins.SuiteName = suitelate;
                                ins.Execute = DateTime.Now;
                                ins.Status = "1";
                                ins.Time = Executetime1 + " s";
                                ins.ResultUrl = "/" + releaseno + "/" + tagno + "" + testfile + ".html";
                                db.Exceute.Add(ins);
                                db.SaveChanges();
                                _log4net.Info("Function Name : Web Execute Excuted successfully" + test + " error " + testerror);
                                return Json(list1);
                                //  return "";
                            }
                            else
                            {
                                var testfile1 = Convert.ToInt32(testapproach);
                                var testcaseId = db.WebFiles.FirstOrDefault(x => x.WebFilesId == testfile1).FileName;
                                Process proc = new Process();
                                proc.StartInfo.FileName = @"/bin/bash";
                                //     proc.StartInfo.FileName = "cmd.exe";
                                proc.StartInfo.CreateNoWindow = true;
                                proc.StartInfo.RedirectStandardInput = true;
                                proc.StartInfo.RedirectStandardOutput = true;
                                proc.StartInfo.UseShellExecute = false;
                                proc.StartInfo.CreateNoWindow = true;
                                proc.Start();
                                proc.StandardInput.WriteLine("" + fileload + "");
                                //  proc.StandardInput.WriteLine("cd /d D:\\Abinesh-learn");
                                proc.StandardInput.WriteLine("python  --listener robotframework_reportportal.listener --variable RP_ENDPOINT:" +configuration.GetValue<string>("ReportPortalIp") + " --variable RP_UUID:" + HttpContext.Session.GetString("ReportportalId") + "  --variable RP_PROJECT:'default_personal' --variable RP_LAUNCH:'" + testfile + "'  -N  " + tagno + "  --report " + tagno + "" + testfile + ".html  --log " + tagno + "" + releaseno + "log.html   -d   " + reportPath + "" + testfile + "   " + testcaseId + "");

                                //  proc.StandardInput.WriteLine("robot robotframework");
                                proc.StandardInput.WriteLine("python -m dbbot.run -b mysql://" + HttpContext.Session.GetString("graphanauser") + ":" + HttpContext.Session.GetString("graphanapass") + "@" + HttpContext.Session.GetString("graphanadbip") + ":" + HttpContext.Session.GetString("graphanaport") + "/" + HttpContext.Session.GetString("graphananame") + " " + reportPath + "" + testfile + "/output.xml");
                                proc.StandardInput.Flush();
                                proc.StandardInput.Close();
                                proc.WaitForExit();
                                string Error1 = proc.StandardOutput.ReadToEnd();
                                Console.WriteLine(proc.StandardOutput.ReadToEnd());
                                System.Diagnostics.Debug.WriteLine(proc.StandardOutput.ReadToEnd());
                                var Executetime = DateTime.Now.ToString("HH:mm");
                                MySqlConnection sql = new MySqlConnection("Server=" + HttpContext.Session.GetString("graphanadbip") + ";Port=" + HttpContext.Session.GetString("graphanaport") + ";Database=" + HttpContext.Session.GetString("graphananame") + ";User ID=" + HttpContext.Session.GetString("graphanauser") + ";Password=" + HttpContext.Session.GetString("graphanapass") + ";SslMode=none");
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
                                ins.Status = "1";
                                ins.Time = Executetime1 + " s";
                                ins.ResultUrl = "/" + testfile + "/" + tagno + "" + result.Substring(0, 6) + ".html";
                                db.Exceute.Add(ins);
                                db.SaveChanges();
                                var Errorcount = Error1.Split(",");

                                var totaltest = Errorcount[2].Split(" ");

                                var Failtest = Errorcount[4].Split(" ");
                                _log4net.Info("Function Name : Web Execute Excuted successfully");
                                return Json(list1);
                                //testcaseresult = totaltest[1] + " " + Errorcount[3] + " " + Failtest[0];
                                //      return "";
                            }

                        }
                   
              
                catch (Exception ex)
                {
                    _log4net.Error("Function Name : Web Execute  -- " + ex.ToString());
                    return Json(ex.ToString());
                    //     return ex.ToString() +"  " + zz + "     " + res1;
                }
            }

        }


       
    }
}
