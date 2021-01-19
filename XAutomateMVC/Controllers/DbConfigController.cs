using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ClosedXML.Excel;
using System.IO;
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

namespace XAutomateMVC.Controllers
{
    public class DbConfigController : Controller
    {
        db_mateContext db = new db_mateContext();
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult DbConfig()
        {
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

            RulesModel viewmodel = new RulesModel();
            ProductViewModel productViewModel = new ProductViewModel();
            productViewModel.Listofproducts = SuiteList;
            productViewModel.RuleList = RuleList;
            return View(productViewModel);
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
        public async Task<IActionResult> UploadToFileSystem(List<IFormFile> files, string ProductId)
        {
            try
            {
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
                                    dataRow[j] = GetCellValue(row.GetCell(j));
                            }
                        }
                        table.Rows.Add(dataRow);
                    }
                    foreach (DataRow row in table.Rows)
                    {

                        var RullName = row["Rule Name"].ToString();
                    
                        var paramete = row["Parameter"].ToString();

                        var Query = row["Query"].ToString();
                        var suiteName = row["Test Approach"].ToString();
                        var Description = row["Description"].ToString();

                       

                        var suiteID = db.TestApproach.FirstOrDefault(x => x.TestApproachName == suiteName);
                        if(suiteID != null)
                        {
                            var ruleNameexit = db.Rules.FirstOrDefault(x => x.RuleName == RullName && x.TestApproachid == suiteID.TestApproachid);
                            if(ruleNameexit != null)
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
                                Ins.SuiteName = suiteName;
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
                TempData["Message"] = "File successfully uploaded to File System.";
                return RedirectToAction("RulesSet");
            }

            catch (Exception ex)
            {
                return Json("Please Enter Valid Files");
            }
        }

        public FileResult TemplateDownload()
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
                             RuleParamter = b.RuleParamters.Select(x => new
                             {
                                 x.ParameterName,
                             }),
                         }).ToList();
            DataRow row;
            foreach (var items in Rules)
            {
                DataTable dt = new DataTable(items.RuleName);
                dt.Columns.AddRange(new DataColumn[4] { new DataColumn("Test Approach"),
                                            new DataColumn("Rule Name"),
                                            new DataColumn("Parameter"),
                                            new DataColumn("Query"),
               
                });
                var length= items.RuleParamter.Count() + 1;
                for (var i=0; i< items.RuleParamter.Count();i++)
                {
                    dt.Columns.AddRange(new DataColumn[1]{ new DataColumn("Parameter Name "+i+""),
                                          
                            });
                }
                dt.Columns.AddRange(new DataColumn[1]{ new DataColumn("Description"),

                            });

               
                    var RuleAdd= items.TestApproachName +"," +items.RuleName +","+ items.RuleParameter + "," + items.RuleCondtion;
                RuleAdd += "," + items.Description;
                var Rulecount = RuleAdd.Split(",");
                
                    row = dt.NewRow();
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
                            FilePath= Directory.GetCurrentDirectory() + "\\Rules\\" + items.RuleName + ".xlsx",
                        });
                        wb.SaveAs(stream);
                        wb.SaveAs(Directory.GetCurrentDirectory() + "\\Rules\\"+items.RuleName+".xlsx");
                        var result = File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Grid.xlsx");

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
                        return File(memoryStream.ToArray(), "application/zip", zipName);
                    }
                }
            return File("","","");
            //var archive = Server.MapPath("~/archive.zip");
            //var temp = Server.MapPath("~/App_Data");

            //if (System.IO.File.Exists(archive))
            //{
            //    System.IO.File.Delete(archive);
            //}

            //ZipFile.CreateFromDirectory(temp, archive);
            //Response.ContentType = "application/zip";
            //Response.AddHeader("Content-Disposition", "attachment; filename=archive.zip");
            //Response.TransmitFile(archive);
            //return "";
        }

        [HttpGet]
        public String DbconfigSave(string Dbname, string Dbhost, string DbPortname, string Dbuser, string DbPassword,string Active,string Describtion)
        {
            var header = this.Request.Headers.ToString();
            var header1 = this.Request.Headers.ToList();
             var Auth= (string)this.Request.Headers["Authorization"];
            if (Auth != "" && Auth != null && Auth != "max-age=0")
            {
                TimeZoneInfo timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Etc/UTC");
                DateTime dtCNow = TimeZoneInfo.ConvertTime(Convert.ToDateTime(DateTime.Now), timeZoneInfo);
                string dtCNowdate = Convert.ToDateTime(dtCNow).ToString("yyyy-MM-dd");
                TimeSpan tsnow = Convert.ToDateTime(dtCNow).TimeOfDay;
                DateTime currentdatetime = Convert.ToDateTime(dtCNowdate + " " + (tsnow.Hours) + ":" + tsnow.Minutes);
                var resultToken = db.TokenValue.First(b => b.TockenId == Auth && b.Validto >= currentdatetime);
                if (resultToken != null)
                {
                    var result = db.DbConfig.FirstOrDefault(x => x.DbName == Dbname && x.DbHostName == Dbhost && x.DbPort == DbPortname && x.Status == "1");
                    if (result == null)
                    {

                        DbConfig Is = new DbConfig();
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
                            return "Valid Db";
                        }



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

        [HttpGet]
        public String DbConfigUpdate(string Dbname, string Dbhost, string DbPortname, string Dbuser, string DbPassword, string Active, string Describtion,int DbConfigId)
        {
            var header = this.Request.Headers.ToString();
            var header1 = this.Request.Headers.ToList();
             var Auth= (string)this.Request.Headers["Authorization"];
            if (Auth != "" && Auth != null && Auth != "max-age=0")
            {
                TimeZoneInfo timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Etc/UTC");
                DateTime dtCNow = TimeZoneInfo.ConvertTime(Convert.ToDateTime(DateTime.Now), timeZoneInfo);
                string dtCNowdate = Convert.ToDateTime(dtCNow).ToString("yyyy-MM-dd");
                TimeSpan tsnow = Convert.ToDateTime(dtCNow).TimeOfDay;
                DateTime currentdatetime = Convert.ToDateTime(dtCNowdate + " " + (tsnow.Hours) + ":" + tsnow.Minutes);
                var resultToken = db.TokenValue.First(b => b.TockenId == Auth && b.Validto >= currentdatetime);
                if (resultToken != null)
                {
                    var result = db.DbConfig.FirstOrDefault(x => x.Dbconfigid == DbConfigId);
                    if (result != null)
                    {
                        result.DbName = Dbname;
                        result.DbHostName = Dbhost;
                        result.DbPort = DbPortname;
                        result.DbUser = Dbuser;
                        result.DbPassword = DbPassword;
                        result.Status = Active;
                        result.Description = Describtion;
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


        public IActionResult RulesSet()
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
            productViewModel.RuleList = RuleList;
            return View(productViewModel);
        }

        public string RulesUpdate(int SuiteName, string RuleName, string Parameter, string rulecond, string Status, string Description,int RulesId,string Ruleparameter)
        {
            var header = this.Request.Headers.ToString();
            var header1 = this.Request.Headers.ToList();
             var Auth= (string)this.Request.Headers["Authorization"];
            if (Auth != "" && Auth != null && Auth != "max-age=0")
            {
                TimeZoneInfo timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Etc/UTC");
                DateTime dtCNow = TimeZoneInfo.ConvertTime(Convert.ToDateTime(DateTime.Now), timeZoneInfo);
                string dtCNowdate = Convert.ToDateTime(dtCNow).ToString("yyyy-MM-dd");
                TimeSpan tsnow = Convert.ToDateTime(dtCNow).TimeOfDay;
                DateTime currentdatetime = Convert.ToDateTime(dtCNowdate + " " + (tsnow.Hours) + ":" + tsnow.Minutes);
                var resultToken = db.TokenValue.First(b => b.TockenId == Auth && b.Validto >= currentdatetime);
                if (resultToken != null)
                {
                    var rules = db.Rules.FirstOrDefault(x => x.RulesId == RulesId);
                    if (rules != null)
                    {
                        rules.TestApproachid = SuiteName;
                        rules.RuleParameter = Parameter;
                        rules.RuleCondtion = rulecond;
                        rules.Status = Status;
                        rules.Description = Description;
                        rules.RuleName = RuleName;
                        db.SaveChanges();

                        db.RuleParamters.RemoveRange(db.RuleParamters.Where(x => x.RulesId == RulesId));

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
        public class Parameter1
        {
            public string parameter { get; set; }
        };

        public String RulesSaveGrid(int SuiteName, string RuleName, string Parameter, string rulecond,string Status,string Description,string Ruleparameter)
        {
            var header = this.Request.Headers.ToString();
            var header1 = this.Request.Headers.ToList();
             var Auth= (string)this.Request.Headers["Authorization"];
            if (Auth != "" && Auth != null && Auth != "max-age=0")
            {
                TimeZoneInfo timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Etc/UTC");
                DateTime dtCNow = TimeZoneInfo.ConvertTime(Convert.ToDateTime(DateTime.Now), timeZoneInfo);
                string dtCNowdate = Convert.ToDateTime(dtCNow).ToString("yyyy-MM-dd");
                TimeSpan tsnow = Convert.ToDateTime(dtCNow).TimeOfDay;
                DateTime currentdatetime = Convert.ToDateTime(dtCNowdate + " " + (tsnow.Hours) + ":" + tsnow.Minutes);
                var resultToken = db.TokenValue.First(b => b.TockenId == Auth && b.Validto >= currentdatetime);
                if (resultToken != null)
                {
                    var rules = db.Rules.FirstOrDefault(x => x.TestApproachid == SuiteName && x.RuleName == RuleName && x.Status == Status);
                    if (rules == null)
                    {
                        Rules Is = new Rules();
                        Is.TestApproachid = SuiteName;
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




        public JsonResult BindGrid(string status)
        {
            try
            {
                var header = this.Request.Headers.ToString();
                var header1 = this.Request.Headers.ToList();
                 var Auth= (string)this.Request.Headers["Authorization"];
                if (Auth != "" && Auth != null && Auth != "max-age=0")
                {
                    TimeZoneInfo timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Etc/UTC");
                    DateTime dtCNow = TimeZoneInfo.ConvertTime(Convert.ToDateTime(DateTime.Now), timeZoneInfo);
                    string dtCNowdate = Convert.ToDateTime(dtCNow).ToString("yyyy-MM-dd");
                    TimeSpan tsnow = Convert.ToDateTime(dtCNow).TimeOfDay;
                    DateTime currentdatetime = Convert.ToDateTime(dtCNowdate + " " + (tsnow.Hours) + ":" + tsnow.Minutes);
                    var resultToken = db.TokenValue.First(b => b.TockenId == Auth && b.Validto >= currentdatetime);
                    if (resultToken != null)
                    {
                        var a = from b in db.DbConfig
                                where b.Status == status
                                select
                                new
                                {
                                    b.DbHostName,
                                    b.DbName,
                                    b.DbPort,
                                    b.DbUser,
                                    b.Dbconfigid,
                                    b.SuiteName,
                                    Description = b.Description == null ? "" : b.Description,
                                    CreateDate = b.CreateDate == null ? "" : Convert.ToString(b.CreateDate),
                                    Status = b.Status == "1" ? "Active" : "InActive",
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
    
        public JsonResult Editdbconfig(int DbconfigId)
        {
            try
            {
                var header = this.Request.Headers.ToString();
                var header1 = this.Request.Headers.ToList();
                 var Auth= (string)this.Request.Headers["Authorization"];
                if (Auth != "" && Auth != null && Auth != "max-age=0")
                {
                    TimeZoneInfo timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Etc/UTC");
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
                                         b.DbName,
                                         b.DbPassword,
                                         b.DbPort,
                                         b.DbUser,
                                         Description = (b.Description != null && b.Description != "undefiened" ? b.Description : ""),
                                         b.Status,
                                     };
                        return Json(dbconf);
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

        public JsonResult EditRuleset(int Rulesetvalue)
        {
            var header = this.Request.Headers.ToString();
            var header1 = this.Request.Headers.ToList();
             var Auth= (string)this.Request.Headers["Authorization"];
            if (Auth != "" && Auth != null && Auth != "max-age=0")
            {
                TimeZoneInfo timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Etc/UTC");
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
                                b.TestApproachid,
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

        public JsonResult RulesBindGrid(string status)
        {
            var header = this.Request.Headers.ToString();
            var header1 = this.Request.Headers.ToList();
             var Auth= (string)this.Request.Headers["Authorization"];
            if (Auth != "" && Auth != null && Auth != "max-age=0")
            {
                TimeZoneInfo timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Etc/UTC");
                DateTime dtCNow = TimeZoneInfo.ConvertTime(Convert.ToDateTime(DateTime.Now), timeZoneInfo);
                string dtCNowdate = Convert.ToDateTime(dtCNow).ToString("yyyy-MM-dd");
                TimeSpan tsnow = Convert.ToDateTime(dtCNow).TimeOfDay;
                DateTime currentdatetime = Convert.ToDateTime(dtCNowdate + " " + (tsnow.Hours) + ":" + tsnow.Minutes);
                var resultToken = db.TokenValue.First(b => b.TockenId == Auth && b.Validto >= currentdatetime);
                if (resultToken != null)
                {
                    var a = from b in db.Rules
                            where b.Status == status
                            select new
                            {
                                b.RuleCondtion,
                                b.RulesId,
                                b.RuleName,
                                b.RuleParameter,
                                Description = (b.Description == null ? "" : b.Description),
                                SuiteName = db.TestApproach.FirstOrDefault(x => x.TestApproachid == b.TestApproachid).TestApproachName,
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
        public JsonResult SearchDbConfig(string search)
        {
            var header = this.Request.Headers.ToString();
            var header1 = this.Request.Headers.ToList();
             var Auth= (string)this.Request.Headers["Authorization"];
            if (Auth != "" && Auth != null && Auth != "max-age=0")
            {
                TimeZoneInfo timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Etc/UTC");
                DateTime dtCNow = TimeZoneInfo.ConvertTime(Convert.ToDateTime(DateTime.Now), timeZoneInfo);
                string dtCNowdate = Convert.ToDateTime(dtCNow).ToString("yyyy-MM-dd");
                TimeSpan tsnow = Convert.ToDateTime(dtCNow).TimeOfDay;
                DateTime currentdatetime = Convert.ToDateTime(dtCNowdate + " " + (tsnow.Hours) + ":" + tsnow.Minutes);
                var resultToken = db.TokenValue.First(b => b.TockenId == Auth && b.Validto >= currentdatetime);
                if (resultToken != null)
                {
                    if (search == "" || search == null || search == "undefiend")
                    {
                        var a = from b in db.DbConfig
                                where b.Status == "1"
                                select
                                new
                                {
                                    b.DbHostName,
                                    b.DbName,
                                    b.DbPort,
                                    b.DbUser,
                                    b.Dbconfigid,
                                    b.SuiteName,
                                    Description = b.Description == null ? "" : b.Description,
                                    CreateDate = b.CreateDate == null ? "" : Convert.ToString(b.CreateDate),
                                    Status = b.Status == "1" ? "Active" : "InActive",
                                };
                        return Json(a);
                    }
                    else
                    {
                        var a = from b in db.DbConfig
                                where b.Status == "1" && b.DbHostName.Contains(search) || b.DbName.Contains(search) || b.DbPort.Contains(search) || b.SuiteName.Contains(search)
                                select
                                new
                                {
                                    b.DbHostName,
                                    b.DbName,
                                    b.DbPort,
                                    b.DbUser,
                                    b.Dbconfigid,
                                    b.SuiteName,
                                    Description = b.Description == null ? "" : b.Description,
                                    CreateDate = b.CreateDate == null ? "" : Convert.ToString(b.CreateDate),
                                    Status = b.Status == "1" ? "Active" : "InActive",
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
        public JsonResult SearchRules(string search)
        {
            var header = this.Request.Headers.ToString();
            var header1 = this.Request.Headers.ToList();
             var Auth= (string)this.Request.Headers["Authorization"];
            if (Auth != "" && Auth != null && Auth != "max-age=0")
            {
                TimeZoneInfo timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Etc/UTC");
                DateTime dtCNow = TimeZoneInfo.ConvertTime(Convert.ToDateTime(DateTime.Now), timeZoneInfo);
                string dtCNowdate = Convert.ToDateTime(dtCNow).ToString("yyyy-MM-dd");
                TimeSpan tsnow = Convert.ToDateTime(dtCNow).TimeOfDay;
                DateTime currentdatetime = Convert.ToDateTime(dtCNowdate + " " + (tsnow.Hours) + ":" + tsnow.Minutes);
                var resultToken = db.TokenValue.First(b => b.TockenId == Auth && b.Validto >= currentdatetime);
                if (resultToken != null)
                {
                    if (search == "" || search == null || search == "undefiened")
                    {
                        var a = from b in db.Rules
                                where b.Status == "1"
                                select new
                                {
                                    b.RuleCondtion,
                                    b.RulesId,
                                    b.RuleName,
                                    b.RuleParameter,
                                    Description = (b.Description == null ? "" : b.Description),
                                    SuiteName = db.TestApproach.FirstOrDefault(x => x.TestApproachid == b.TestApproachid).TestApproachName,
                                };
                        return Json(a);
                    }
                    else
                    {
                        var a = (from b in db.Rules
                                 where b.Status == "1"
                                 select new
                                 {
                                     b.RuleCondtion,
                                     b.RulesId,
                                     b.RuleName,
                                     b.RuleParameter,
                                     Description = (b.Description == null ? "" : b.Description),
                                     SuiteName = db.TestApproach.FirstOrDefault(x => x.TestApproachid == b.TestApproachid).TestApproachName,
                                 }).Where(x => x.RuleCondtion.Contains(search) || x.RuleName.Contains(search) || x.RuleParameter.Contains(search) || x.Description.Contains(search) || x.SuiteName.Contains(search));
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


    }

}
