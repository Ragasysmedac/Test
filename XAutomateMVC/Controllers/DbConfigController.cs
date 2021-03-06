using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MySql.Data.MySqlClient;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using XAutomateMVC.Models;
using XAutomateMVC.Models.DBModels;
using Ionic.Zip;
using System.Data.SqlClient;
using TimeZoneConverter;
using log4net;
using Microsoft.Extensions.Configuration;

namespace XAutomateMVC.Controllers
{
    public class DbConfigController : Controller
    {
        static readonly ILog _log4net = LogManager.GetLogger(typeof(DbConfigController));
        db_mateContext db = new db_mateContext();


        private IConfiguration configuration;

        public DbConfigController(IConfiguration iConfig)
        {
            configuration = iConfig;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult DbConfig()
        {
            try
            {
                ViewBag.Report =configuration.GetValue<string>("ReportPortalIp");
                var SuiteList = (from product in db.TestApproach
                                 where product.Status == "1"
                                 select new SelectListItem()
                                 {
                                     Text = product.TestApproachName,
                                     Value = product.TestApproachName.ToString(),
                                 }).ToList();

                SuiteList.Insert(0, new SelectListItem()
                {
                    Text = "----Select----",
                    Value = string.Empty
                });

                var RuleList = (from Rules in db.Rules
                                where Rules.Status == "1"
                                select new SelectListItem()
                                {
                                    Text = Rules.RuleName,
                                    Value = Rules.RuleName.ToString(),
                                }).ToList();

                RuleList.Insert(0, new SelectListItem()
                {
                    Text = "----Select----",
                    Value = string.Empty
                });
                ViewBag.testing = configuration.GetValue<string>("Testingvalue");
              //  ViewBag.testing = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
                var testing = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
                RulesModel viewmodel = new RulesModel();
                ProductViewModel productViewModel = new ProductViewModel();
                productViewModel.Listofproducts = SuiteList;
                productViewModel.RuleList = RuleList;
                _log4net.Debug("Connection Page Successfully Loaded");
                return View(productViewModel);
            }
            catch(Exception ex)
            {
                _log4net.Error("Function Name : DbConfig  -- " + ex.ToString());
                return null;
            }
           
        }

        //private static string GetCellValue(ICell cell)
        //{


        //    if (cell == null)
        //        return string.Empty;
        //    switch (cell.CellType)
        //    {
        //        case CellType.Blank:
        //            return string.Empty;
        //        case CellType.Boolean:
        //            return cell.BooleanCellValue.ToString();
        //        case CellType.Error:
        //            return cell.ErrorCellValue.ToString();
        //        case CellType.Numeric:
        //        case CellType.Unknown:
        //        default:
        //            return cell.ToString();//This is a trick to get the correct value of the cell. NumericCellValue will return a numeric value no matter the cell value is a date or a number
        //        case CellType.String:
        //            return cell.StringCellValue;
        //        case CellType.Formula:
        //            try
        //            {
        //                var e = new HSSFFormulaEvaluator(cell.Sheet.Workbook);
        //                e.EvaluateInCell(cell);
        //                return cell.ToString();
        //            }
        //            catch
        //            {
        //                return cell.NumericCellValue.ToString();
        //            }
        //    }
        //}


        [HttpPost]
        public async Task<IActionResult> RULEUploadToFile(List<IFormFile> files, string ProductId)
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
                        foreach (var file in files)
                        {
                            string uploadpath = configuration.GetValue<string>("Systempath:uploadpath");
                            var basePath = Path.Combine(Directory.GetCurrentDirectory() + ""+ uploadpath + "");
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

                            //header
                            int columncount = 0;
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
                                            //  dataRow[j] = GetCellValue(row.GetCell(j));
                                            dataRow[j] = FileUpload.GetCellValue(row.GetCell(j));


                                    }
                                }
                                table.Rows.Add(dataRow);
                            }
                            foreach (DataRow row in table.Rows)
                            {
                                int RId = 0;
                                var RuId = row["R Id"].ToString();
                                var RullName = row["Rule Name"].ToString();

                                var paramete = row["Parameter"].ToString();

                                var Query = row["Query"].ToString();
                                var suiteName = row["Test Approach"].ToString();
                                var Description = row["Description"].ToString();

                               

                                var suiteID = db.TestApproach.FirstOrDefault(x => x.TestApproachName == suiteName);
                                if (suiteID != null)
                                {
                                    if (RuId != "" && RuId != null)
                                    {
                                        RId = Convert.ToInt32(RId);
                                        var ruleNameexit = db.Rules.FirstOrDefault(x => x.RulesId == RId);
                                        if (ruleNameexit != null)
                                        {
                                            ruleNameexit.RuleParameter = paramete;
                                            ruleNameexit.RuleCondtion = Query;
                                            ruleNameexit.Description = Description;
                                            db.SaveChanges();
                                            var parameter = Convert.ToInt32(paramete);
                                            db.RuleParamters.RemoveRange(db.RuleParamters.Where(x => x.RulesId == ruleNameexit.RulesId));
                                            for (int k = 0; k < parameter; k++)
                                            {
                                                var paramete1 = row["Parameter Name " + k].ToString();
                                                if (paramete1 != "" && paramete1 != null)
                                                {
                                                    RuleParamters Ins1 = new RuleParamters();
                                                    Ins1.ParameterName = paramete1;
                                                    Ins1.RulesId = ruleNameexit.RulesId;

                                                    db.RuleParamters.Add(Ins1);
                                                    db.SaveChanges();
                                                }

                                            }
                                        }
                                        else
                                        {
                                            Rules Ins = new Rules();
                                            Ins.TestApproachName = suiteName;
                                            Ins.RuleName = RullName;
                                            Ins.RuleParameter = paramete;
                                            Ins.RuleCondtion = Query;
                                            Ins.Description = Description;
                                            Ins.Status = "1";
                                            Ins.TestApproachid = suiteID.TestApproachid;
                                            db.Rules.Add(Ins);
                                            db.SaveChanges();
                                            var parameter = Convert.ToInt32(paramete);
                                            for (int k = 0; k < parameter; k++)
                                            {
                                                var paramete1 = row["Parameter Name " + k].ToString();
                                                if (paramete1 != "" && paramete1 != null)
                                                {
                                                    RuleParamters Ins1 = new RuleParamters();
                                                    Ins1.ParameterName = paramete1;
                                                    Ins1.RulesId = Ins.RulesId;
                                                    Ins.Status = "1";
                                                    Ins.CreateDate = DateTime.Now;
                                                    db.RuleParamters.Add(Ins1);
                                                    db.SaveChanges();
                                                }

                                            }
                                        }
                                    }
                                    else
                                    {
                                        Rules Ins = new Rules();
                                        Ins.TestApproachName = suiteName;
                                        Ins.RuleName = RullName;
                                        Ins.RuleParameter = paramete;
                                        Ins.RuleCondtion = Query;
                                        Ins.Description = Description;
                                        Ins.Status = "1";
                                        Ins.TestApproachid = suiteID.TestApproachid;
                                        db.Rules.Add(Ins);
                                        db.SaveChanges();
                                        var parameter = Convert.ToInt32(paramete);
                                        for (int k = 0; k < parameter; k++)
                                        {
                                            var paramete1 = row["Parameter Name " + k].ToString();
                                            if (paramete1 != "" && paramete1 != null)
                                            {
                                                RuleParamters Ins1 = new RuleParamters();
                                                Ins1.ParameterName = paramete1;
                                                Ins1.RulesId = Ins.RulesId;
                                                Ins.Status = "1";
                                                Ins.CreateDate = DateTime.Now;
                                                db.RuleParamters.Add(Ins1);
                                                db.SaveChanges();
                                            }

                                        }
                                    }


                                }


                            }
                        }
                        _log4net.Info("Rules  File Uploaded Success");
                        TempData["Message"] = "File successfully uploaded to File System.";
                        return RedirectToAction("RulesSet");
                    }
                    else
                    {
                        _log4net.Error("Function Name :Rules UploadToFileSystem Auth Failed" );
                        return Json("Auth Fail");
                    }
                }
                else
                {
                    _log4net.Error("Function Name :Rules UploadToFileSystem Auth Failed");
                    return Json("Auth Fail");
                }
            }
            catch (Exception ex)
            {
                _log4net.Error("Function Name :Rules UploadToFileSystem  -- " + ex.ToString());
                return Json("Please Enter Valid Files");
            }
        }

        public FileResult TemplateDownload()
        {
            try
            {
                List<FileonSystem> list = new List<FileonSystem>();

                var Rules = (from b in db.Rules
                             where b.Status == "1"
                             select new
                             {
                                 b.RulesId,
                                 b.TestApproach.TestApproachName,
                                 b.RuleName,
                                 b.RuleParameter,
                                 b.Description,
                                 b.RuleCondtion,
                                 RuleParamter = b.RuleParamters.Select(x => new
                                 {
                                     x.ParameterName,
                                 }),
                             }).ToList();
                DataRow row;
                foreach (var items in Rules)
                {
                    var rulescreen = "";
                    if (items.RuleName.Length > 30)
                    {
                        rulescreen = items.RuleName.Substring(0, 25);
                    }
                    else
                    {
                        rulescreen = items.RuleName;
                    }
                    DataTable dt = new DataTable(rulescreen);
                    dt.Columns.AddRange(new DataColumn[5] {new DataColumn("R Id"), new DataColumn("Test Approach"),
                                            new DataColumn("Rule Name"),
                                            new DataColumn("Parameter"),
                                            new DataColumn("Query"),

                });
                    var length = items.RuleParamter.Count() + 1;
                    for (var i = 0; i < items.RuleParamter.Count(); i++)
                    {
                        dt.Columns.AddRange(new DataColumn[1]{ new DataColumn("Parameter Name "+i+""),

                            });
                    }
                    dt.Columns.AddRange(new DataColumn[1]{ new DataColumn("Description"),

                            });


                    var RuleAdd = items.TestApproachName + "," + items.RuleName + "," + items.RuleParameter + "," + items.RuleCondtion;
                    RuleAdd += "," + items.Description;
                    var Rulecount = RuleAdd.Split(",");

                    row = dt.NewRow();
                    row["R Id"] = items.RulesId;
                    row["Test Approach"] = items.TestApproachName;
                    row["Rule Name"] = items.RuleName;
                    row["Parameter"] = items.RuleParameter;
                    row["Query"] = items.RuleCondtion;
                    var j = 0;
                    foreach (var itemsrule in items.RuleParamter)
                    {
                        row["Parameter Name " + j + ""] = itemsrule.ParameterName;
                        j++;
                    }

                    row["Description"] = items.Description;
                    dt.Rows.Add(row);
                    using (XLWorkbook wb = new XLWorkbook())
                    {
                        wb.Worksheets.Add(dt);
                        using (MemoryStream stream = new MemoryStream())
                        {
                            list.Add(new FileonSystem()
                            {
                                FilePath = Directory.GetCurrentDirectory() + "\\Rules\\" + items.RuleName + ".xlsx",
                            });
                            wb.SaveAs(stream);
                            wb.SaveAs(Directory.GetCurrentDirectory() + "\\Rules\\" + items.RuleName + ".xlsx");

                        }
                    }

                }

                using (ZipFile zip = new ZipFile())
                {
                    zip.AlternateEncodingUsage = ZipOption.AsNecessary;

                    foreach (var file in list)
                    {

                        zip.AddFile(file.FilePath, "Rules");

                    }
                    //   zip.AddFile(wb, "Files");
                    string zipName = String.Format("Zip_{0}.zip", DateTime.Now.ToString("yyyy-MMM-dd-HHmmss"));
                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        zip.Save(memoryStream);
                        _log4net.Info("Rules  TemplateDownload Success");
                        return File(memoryStream.ToArray(), "application/zip", zipName);
                    }
                }
                //return File("", "", "");
           
            }
            catch(Exception ex)
            {
                _log4net.Error("Function Name : Rules TemplateDownload  -- " + ex.ToString());
                return null;
            }

          
        
        }

        [HttpGet]
        public String DbconfigSave(string Databasetype1,string Dbname, string Dbhost, string DbPortname, string Dbuser, string DbPassword,string Active,string Describtion)
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
                        var result = db.DbConfig.FirstOrDefault(x => x.DbName == Dbname && x.DbHostName == Dbhost && x.DbPort == DbPortname && x.Status == "1");
                        if (result == null)
                        {

                            DbConfig Is = new DbConfig();
                            Is.DatabaseType = Databasetype1;
                            Is.DbName = Dbname;
                            Is.DbHostName = Dbhost;
                            Is.DbPort = DbPortname;
                            Is.DbUser = Dbuser;
                            Is.DbPassword = DbPassword;
                            Is.Description = Describtion;
                            Is.Status = Active;
                            Is.CreateDate = DateTime.Now;
                            db.DbConfig.Add(Is);
                            db.SaveChanges();
                            try
                            {
                                if (Databasetype1 == "MYSQL")
                                {
                                    MySqlConnection sql = new MySqlConnection("Server=" + Dbhost + ";Port=" + DbPortname + ";Database=" + Dbname + ";User ID=" + Dbuser + ";Password=" + DbPassword + ";SslMode=none");
                                    sql.Open();
                                    MySqlCommand com = new MySqlCommand("SELECT DISTINCT  COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS where  TABLE_SCHEMA='" + Dbname + "' ", sql);
                                    using (var reader = com.ExecuteReader())
                                    {
                                        db.Tablecolumn.RemoveRange(db.Tablecolumn.Where(x => x.Dbconfigid == Is.Dbconfigid));
                                        db.SaveChanges();
                                        while (reader.Read())
                                        {

                                            Tablecolumn Ins = new Tablecolumn();
                                            Ins.Tablecolumn1 = reader["COLUMN_NAME"].ToString();
                                            Ins.FieldName = "C";
                                            Ins.Active = "1";
                                            Ins.Dbconfigid = Is.Dbconfigid;
                                            db.Tablecolumn.Add(Ins);
                                            db.SaveChanges();
                                        }
                                    }
                                    MySqlCommand com1 = new MySqlCommand("SELECT DISTINCT  TABLE_NAME FROM INFORMATION_SCHEMA.COLUMNS where  TABLE_SCHEMA='" + Dbname + "' ", sql);
                                    using (var reader = com1.ExecuteReader())
                                    {
                                        while (reader.Read())
                                        {

                                            Tablecolumn Ins2 = new Tablecolumn();
                                            Ins2.Tablecolumn1 = reader["TABLE_NAME"].ToString();
                                            Ins2.FieldName = "T";
                                            Ins2.Active = "1";
                                            Ins2.Dbconfigid = Is.Dbconfigid;
                                            db.Tablecolumn.Add(Ins2);

                                            db.SaveChanges();
                                        }
                                    }
                                    sql.Close();
                                }
                                else
                                {
                                    SqlConnection sql = new SqlConnection("Server=" + Dbhost + ";Database=" + Dbname + ";Uid=" + Dbuser + ";Pwd=" + DbPassword + ";");
                                    sql.Open();
                                    SqlCommand com = new SqlCommand("SELECT DISTINCT  COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS", sql);
                                    //  MySqlConnection sql = new MySqlConnection("Server=" + Dbhost + ";Port=" + DbPortname + ";Database=" + Dbname + ";User ID=" + Dbuser + ";Password=" + DbPassword + ";SslMode=none");
                                    // sql.Open();
                                    // MySqlCommand com = new MySqlCommand("SELECT DISTINCT  COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS where  TABLE_SCHEMA='" + Dbname + "' ", sql);
                                    using (var reader = com.ExecuteReader())
                                    {
                                        db.Tablecolumn.RemoveRange(db.Tablecolumn.Where(x => x.Dbconfigid == Is.Dbconfigid));
                                        db.SaveChanges();
                                        while (reader.Read())
                                        {

                                            Tablecolumn Ins = new Tablecolumn();
                                            Ins.Tablecolumn1 = reader["COLUMN_NAME"].ToString();
                                            Ins.FieldName = "C";
                                            Ins.Active = "1";
                                            Ins.Dbconfigid = Is.Dbconfigid;
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
                                            Ins2.Dbconfigid = Is.Dbconfigid;
                                            db.Tablecolumn.Add(Ins2);

                                            db.SaveChanges();
                                        }
                                    }
                                    sql.Close();

                                }



                                Tablecolumn Ins3 = new Tablecolumn();
                                Ins3.Tablecolumn1 = Dbname;
                                Ins3.FieldName = "D";
                                Ins3.Active = "1";
                                Ins3.Dbconfigid = Is.Dbconfigid;
                                db.Tablecolumn.Add(Ins3);
                                db.SaveChanges();
                            }
                            catch (Exception ex)
                            {
                                _log4net.Error("Function Name : connection DbconfigSave  -- " + ex.ToString());
                                return "Valid Db";
                            }


                            _log4net.Debug("connection Save Successfully");
                            return "Success";

                        }
                        else
                        {
                            _log4net.Debug("connection Failed to auth");
                            return "Fail";
                        }
                    }
                    else
                    {
                        _log4net.Debug("connection Failed to auth");
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
                _log4net.Error("Function Name : connection DbconfigSave  -- " + ex.ToString());
                return null;
            }
         
        }

        [HttpGet]
        public String DbConfigUpdate(string Databasetype1, string Dbname, string Dbhost, string DbPortname, string Dbuser, string DbPassword, string Active, string Describtion,int DbConfigId)
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
                        var result = db.DbConfig.FirstOrDefault(x => x.Dbconfigid == DbConfigId);
                        if (result != null)
                        {
                            result.DatabaseType = Databasetype1;
                            result.DbName = Dbname;
                            result.DbHostName = Dbhost;
                            result.DbPort = DbPortname;
                            result.DbUser = Dbuser;
                            result.DbPassword = DbPassword;
                            result.Status = Active;
                            result.Description = Describtion;
                            db.SaveChanges();

                            _log4net.Debug("Function Name : connection Update Successfully ");
                            return "Success";
                        }
                        else
                        {
                            _log4net.Debug("Function Name : connection Update Successfully");
                            return "Fail";
                        }
                    }
                    else
                    {

                        _log4net.Error("Function Name : connection Update Auth Fail");
                        return "Auth Fail";
                    }
                }
                else
                {
                    _log4net.Error("Function Name : connection Update Auth Fail");
                    return "Auth Fail";
                }
            }
            catch(Exception ex)
            {
                _log4net.Error("Function Name : connection DbConfigUpdate  -- " + ex.ToString());
                return null;
            }
           
        }


        public IActionResult RulesSet()
        {
            try
            {
                ViewBag.Report =configuration.GetValue<string>("ReportPortalIp");
                var SuiteList = (from product in db.TestApproach
                                 where product.Status == "1"
                                 select new SelectListItem()
                                 {
                                     Text = product.TestApproachName,
                                     Value = product.TestApproachName.ToString(),
                                 }).ToList();

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
                var RuleList = (from Rules in db.Rules
                                where Rules.Status == "1"
                                select new SelectListItem()
                                {
                                    Text = Rules.RuleName,
                                    Value = Rules.RulesId.ToString(),
                                }).ToList();

                RuleList.Insert(0, new SelectListItem()
                {
                    Text = "----Select----",
                    Value = string.Empty
                });

                RulesModel viewmodel = new RulesModel();
                ProductViewModel productViewModel = new ProductViewModel();
                productViewModel.Listofproducts = SuiteList;
                productViewModel.TestSuiteList = Dbconfig;
                productViewModel.RuleList = RuleList;
                _log4net.Debug("Function Name : Rule Page Successfully Open");
                return View(productViewModel);
            }
            catch(Exception ex)
            {
                _log4net.Error("Function Name :Rules RulesSet -- " + ex.ToString());
                return null;
            }
          
        }


        public string RulesUpdate(string SuiteName, string RuleName, string Parameter, string rulecond, string Status, string Description, int RulesId, string Ruleparameter,int connection)
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
                        var rules = db.Rules.FirstOrDefault(x => x.RulesId == RulesId);
                        if (rules != null)
                        {
                            if (Status == "0")
                            {
                                var etldb = db.TestCases.FirstOrDefault(X => X.RulesId == rules.RulesId && X.Status == "1");
                                if (etldb == null)
                                {
                                    rules.TestApproachName = SuiteName;
                                    rules.RuleParameter = Parameter;
                                    rules.RuleCondtion = rulecond;
                                    rules.Status = Status;
                                    rules.Description = Description;
                                    rules.RuleName = RuleName;
                                    //rules.DbConfigId = connection;
                                    db.SaveChanges();

                                    db.RuleParamters.RemoveRange(db.RuleParamters.Where(x => x.RulesId == RulesId));

                                    if (Parameter != "0")
                                    {
                                        var rulesarraysplit = Ruleparameter.Split(",");
                                        foreach (var item in rulesarraysplit)
                                        {
                                            RuleParamters Ins = new RuleParamters();
                                            Ins.RulesId = RulesId;
                                            Ins.ParameterName = item;
                                            Ins.Status = "1";
                                            Ins.CreatedDate = DateTime.Now;
                                            db.RuleParamters.Add(Ins);
                                            db.SaveChanges();
                                        }
                                    }
                                }
                                else
                                {
                                    return "Validate";
                                }
                            }
                            else
                            {
                                rules.TestApproachName = SuiteName;
                                rules.RuleParameter = Parameter;
                                rules.RuleCondtion = rulecond;
                                rules.Status = Status;
                                //rules.DbConfigId = connection;
                                rules.Description = Description;
                                rules.RuleName = RuleName;
                                db.SaveChanges();

                                db.RuleParamters.RemoveRange(db.RuleParamters.Where(x => x.RulesId == RulesId));

                                if (Parameter != "0")
                                {
                                    var rulesarraysplit = Ruleparameter.Split(",");
                                    foreach (var item in rulesarraysplit)
                                    {
                                        RuleParamters Ins = new RuleParamters();
                                        Ins.RulesId = RulesId;
                                        Ins.ParameterName = item;
                                        Ins.Status = "1";
                                        Ins.CreatedDate = DateTime.Now;
                                        db.RuleParamters.Add(Ins);
                                        db.SaveChanges();
                                    }
                                }
                            }

                            _log4net.Debug("Function Name : Rules RulesUpdate Update Successfully ");
                            return "Success";
                        }
                        else
                        {
                            _log4net.Error("Function Name : Rules RulesUpdate Update Auth Faill ");
                            return "Fail";
                        }
                    }
                    else
                    {
                        _log4net.Error("Function Name : Rules RulesUpdate Update Auth Fail ");
                        return "Auth Fail";
                    }
                }
                else
                {
                    _log4net.Error("Function Name : Rules RulesUpdate Update Auth Fail ");
                    return "Auth Fail";
                }
            }
            catch(Exception ex)
            {
                _log4net.Error("Function Name : RulesUpdate " + ex.ToString());
                return null;
            }
           
        }
        public class Parameter1
        {
            public string parameter { get; set; }
        };



        public JsonResult deletevalues(int dbconfig)
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
                        db.Tablecolumn.RemoveRange(db.Tablecolumn.Where(x => x.Dbconfigid == dbconfig));
                        db.DbConfig.RemoveRange(db.DbConfig.FirstOrDefault(x => x.Dbconfigid == dbconfig));
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



        public JsonResult deleteRule(int RulesId)
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
                        var result = from b in db.TestCases
                                     where b.RulesId == RulesId
                                     select new
                                     {
                                         b.TestCasesId
                                     };
                        foreach(var item in result)
                        {
                            db.TestCaseParameters.RemoveRange(db.TestCaseParameters.Where(x => x.TestCasesId == item.TestCasesId));
                            db.SaveChanges();
                        }
                        db.TestCases.RemoveRange(db.TestCases.Where(x => x.RulesId == RulesId));
                        db.SaveChanges();
                        db.RuleParamters.RemoveRange(db.RuleParamters.Where(x => x.RulesId == RulesId));
                        db.SaveChanges();
                        db.Rules.RemoveRange(db.Rules.FirstOrDefault(x => x.RulesId == RulesId));
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


        public String RulesSaveGrid(string SuiteName, string RuleName, string Parameter, string rulecond,string Status,string Description,string Ruleparameter,int connection)
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
                        var rules = db.Rules.FirstOrDefault(x => x.TestApproachName == SuiteName && x.RuleName == RuleName && x.Status == Status);
                        if (rules == null)
                        {
                            Rules Is = new Rules();
                            Is.TestApproachName = SuiteName;
                            Is.RuleName = RuleName;
                            Is.RuleParameter = Parameter;
                            Is.RuleCondtion = rulecond;
                            Is.Description = Description;
                            Is.Status = Status;
                            db.Rules.Add(Is);
                            db.SaveChanges();
                            var rulesarraysplit = Ruleparameter.Split(",");
                            foreach (var item in rulesarraysplit)
                            {
                                RuleParamters Ins = new RuleParamters();
                                Ins.RulesId = Is.RulesId;
                                Ins.ParameterName = item;
                                Ins.Status = "1";
                                Ins.CreatedDate = DateTime.Now;
                                db.RuleParamters.Add(Ins);
                                db.SaveChanges();
                            }
                            _log4net.Info("Function Name : Rules RulesSaveGrid Saved Successfully");
                            return "Success";



                        }
                        else
                        {
                            _log4net.Info("Function Name : Rules RulesSaveGrid  Auth Fail");
                            return "Fail";
                        }
                    }
                    else
                    {
                        _log4net.Info("Function Name : Rules RulesSaveGrid  Auth Fail");
                        return "Auth Fail";
                    }
                }
                else
                {
                    _log4net.Info("Function Name : Rules RulesSaveGrid  Auth Fail");
                    return "Auth Fail";
                }
            }
            catch(Exception ex)
            {
                _log4net.Error("Function Name : Rules RulesSaveGrid  -- " + ex.ToString());
                return null;
            }
           
        }

        public JsonResult BindGrid(string status)
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
                        var a = from b in db.DbConfig
                                orderby b.Status descending
                                select
                                new
                                {
                                    b.DbHostName,
                                    b.DbName,
                                    DbPort = (b.DbPort == null ? "" : b.DbPort),
                                    b.DbUser,
                                    b.Dbconfigid,
                                    b.SuiteName,
                                    Description = b.Description == null ? "" : b.Description,
                                    CreateDate = b.CreateDate == null ? "" : Convert.ToString(b.CreateDate),
                                    Status = b.Status == "1" ? "Active" : "Inactive",
                                };
                        return Json(a);
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

        public JsonResult Editdbconfig(int DbconfigId)
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
                        var dbconf = from b in db.DbConfig
                                     where b.Dbconfigid == DbconfigId
                                     select new
                                     {
                                         b.Dbconfigid,
                                         b.DbHostName,
                                         b.DatabaseType,
                                         b.DbName,
                                         b.DbPassword,
                                         DbPort = (b.DbPort == null ? "" : b.DbPort),
                                         b.DbUser,
                                         Description = (b.Description != null && b.Description != "undefiened" ? b.Description : ""),
                                         b.Status,
                                     };
                        return Json(dbconf);
                    }
                    else
                    {
                        _log4net.Error("Function Name : connection Editdbconfig  Auth Failed");
                        return Json("Auth Fail");
                    }
                }
                else
                {
                    _log4net.Error("Function Name : connection Editdbconfig  Auth Failed");
                    return Json("Auth Fail");
                }
            }
            catch(Exception ex)
            {
                _log4net.Error("Function Name : connection Editdbconfig  -- " + ex.ToString());
                return null;
            }
        }

        public JsonResult EditRuleset(int Rulesetvalue)
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
                        var a = from b in db.Rules
                                where b.RulesId == Rulesetvalue
                                select new
                                {
                                    b.RulesId,
                                    testApproachid = b.TestApproachName,
                                    b.DbConfigId,
                                    b.RuleParameter,
                                    b.RuleName,
                                    b.Description,
                                    b.RuleCondtion,
                                    b.Status,
                                    RuleParamtername = b.RuleParamters.Select(x => new
                                    {
                                        x.ParameterName,
                                    }),
                                };
                        _log4net.Info("Function Name : Rules EditRuleset Edit Successfully");
                        return Json(a);
                    }
                    else
                    {
                        _log4net.Error("Function Name : Rules EditRuleset Auth Failed");
                        return Json("Auth Fail");
                    }
                }
                else
                {
                    _log4net.Error("Function Name : Rules EditRuleset Auth Failed" );
                    return Json("Auth Fail");
                }
            }
            catch(Exception ex)
            {
                _log4net.Error("Function Name : Rules EditRuleset  -- " + ex.ToString());
                return null;
            }
          
        }

        public JsonResult RulesBindGrid(string status)
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
                    var resultToken = db.TokenValue.First(b => b.TockenId == Auth && b.Validto >= currentdatetime);
                    if (resultToken != null)
                    {
                        var a = from b in db.Rules
                                orderby b.Status descending
                                select new
                                {
                                    b.RuleCondtion,
                                    b.RulesId,
                                    b.RuleName,
                                    b.RuleParameter,
                                    Description = (b.Description == null ? "" : b.Description),
                                    SuiteName = (b.TestApproachName == null ? "" : b.TestApproachName),
                                    b.DbConfigId,
                                    status = b.Status =="1" ? "Active" : "Inactive",
                                };
                        _log4net.Info("Function Name : Rules RulesBindGrid Successfully ");
                        return Json(a);
                    }
                    else
                    {
                        _log4net.Error("Function Name : Rules RulesBindGrid Auth Failed ");
                        return Json("Auth Fail");
                    }
                }
                else
                {
                    _log4net.Error("Function Name : Rules RulesBindGrid Auth Failed ");
                    return Json("Auth Fail");
                }
            }
            catch(Exception ex)
            {
                _log4net.Error("Function Name : Rules RulesBindGrid  -- " + ex.ToString());
                return null;
            }
           
        }
        public JsonResult SearchDbConfig(string search,string Status)
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
                        if (search == "" || search == null || search == "undefiend")
                        {
                            var a = from b in db.DbConfig
                                    where b.Status == Status
                                    select
                                    new
                                    {
                                        b.DbHostName,
                                        b.DbName,
                                        DbPort = (b.DbPort == null ? "" : b.DbPort),
                                        b.DbUser,
                                        b.Dbconfigid,
                                        b.SuiteName,
                                        Description = b.Description == null ? "" : b.Description,
                                        CreateDate = b.CreateDate == null ? "" : Convert.ToString(b.CreateDate),
                                        Status = b.Status == "1" ? "Active" : "InActive",
                                    };
                            _log4net.Error("Function Name : connection SearchDbConfig Successfully searched ");
                            return Json(a);
                        }
                        else
                        {
                            var a = (from b in db.DbConfig
                                     where b.Status == Status
                                     select
                                     new
                                     {
                                         b.DbHostName,
                                         b.DbName,
                                         DbPort = (b.DbPort == null ? "" : b.DbPort),
                                         b.DbUser,
                                         b.Dbconfigid,
                                         b.SuiteName,
                                         Description = b.Description == null ? "" : b.Description,
                                         CreateDate = b.CreateDate == null ? "" : Convert.ToString(b.CreateDate),
                                         Status = b.Status == "1" ? "Active" : "Inactive",
                                     }).Where(x => x.DbHostName.Contains(search) || x.DbName.Contains(search) || x.DbPort.Contains(search) || x.SuiteName.Contains(search));

                            _log4net.Error("Function Name : connection SearchDbConfig Successfully searched ");
                            return Json(a);
                        }

                    }
                    else
                    {
                        _log4net.Error("Function Name : connection SearchDbConfig Auth Failed ");
                        return Json("Auth Fail");
                    }
                }
                else
                {
                    _log4net.Error("Function Name : connection SearchDbConfig Auth Failed ");
                    return Json("Auth Fail");
                }
            }
            catch(Exception ex)
            {
                _log4net.Error("Function Name : connection SearchDbConfig  -- " + ex.ToString());
                return null;
            }
           

        }
        public JsonResult SearchRules(string search,string Status)
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
                        if (search == "" || search == null || search == "undefiened")
                        {
                            var a = from b in db.Rules
                                    where b.Status == Status
                                    select new
                                    {
                                        b.RuleCondtion,
                                        b.RulesId,
                                        b.RuleName,
                                        b.RuleParameter,
                                        Description = (b.Description == null ? "" : b.Description),
                                        SuiteName = (b.TestApproachName == null ? "" : b.TestApproachName),
                                        Status = b.Status == "1" ? "Active" : "Inactive",
                                    };
                            _log4net.Error("Function Name : Rules SearchRules Successfully Searched ");
                            return Json(a);
                        }
                        else
                        {
                            var a = (from b in db.Rules
                                     where b.Status == Status
                                     select new
                                     {
                                         b.RuleCondtion,
                                         b.RulesId,
                                         b.RuleName,
                                         b.RuleParameter,
                                         Description = (b.Description == null ? "" : b.Description),
                                         SuiteName = (b.TestApproachName == null ? "" : b.TestApproachName),
                                         Status = b.Status == "1" ? "Active" : "Inactive",
                                     }).Where(x => x.RuleCondtion.Contains(search) || x.RuleName.Contains(search) || x.RuleParameter.Contains(search) || x.Description.Contains(search) || x.SuiteName.Contains(search));
                            _log4net.Error("Function Name : Rules SearchRules Successfully Searched ");
                            return Json(a);
                        }
                    }
                    else
                    {
                        _log4net.Error("Function Name : Rules SearchRules Auth Failed ");
                        return Json("Auth Fail");
                    }
                }
                else
                {
                    _log4net.Error("Function Name : Rules SearchRules Auth Failed ");
                    return Json("Auth Fail");
                }
            }
            catch(Exception ex)
            {
                _log4net.Error("Function Name : Rules SearchRules  -- " + ex.ToString());
                return null;
            }
          
            }


    }

}
