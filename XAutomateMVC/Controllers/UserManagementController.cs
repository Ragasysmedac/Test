using System;
using System.Collections.Generic;
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
    public class UserManagementController : Controller
    {
        static readonly ILog _log4net = LogManager.GetLogger(typeof(UserManagementController));
        db_mateContext db = new db_mateContext();

        private IConfiguration configuration;

        public UserManagementController(IConfiguration iConfig)
        {
            configuration = iConfig;
        }
        public IActionResult Employee()
        {
            try
            {
                ViewBag.Report =configuration.GetValue<string>("ReportPortalIp");
                var Role = (from product in db.Role
                            where product.Status == "1"
                            select new SelectListItem()
                            {
                                Text = product.RoleName,
                                Value = product.RoleId.ToString(),
                            }).ToList();

                Role.Insert(0, new SelectListItem()
                {
                    Text = "----Select----",
                    Value = string.Empty
                });
                ProductViewModel productViewModel = new ProductViewModel();
                productViewModel.Listofproducts = Role;
                _log4net.Info("Function Name : Employee Employee Page loaded Successfully");
                return View(productViewModel);
            }
            catch(Exception ex)
            {
                _log4net.Error("Function Name : Employee Employee  -- " + ex.ToString());
                return null;
            }
          
        }
        public string UpdateEmployee(string Emp_No, string E_Name, string Emp_pass, int Emp_role, int Status, string Description,int LoginId,string EmailId)
        {
            try
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
                    var resultToken = db.TokenValue.FirstOrDefault(b => b.TockenId == Auth && b.Validto >= currentdatetime);
                    if (resultToken != null)
                    {
                        var result = db.Login.FirstOrDefault(x => x.LoginId == LoginId);
                        if (result != null)
                        {
                            result.EmployeeName = E_Name;
                            result.EmployeeNo = Emp_No;
                            result.Password = Emp_pass;
                            result.RoleId = Emp_role;
                            result.UserName = E_Name;
                            result.Status = Status;
                            result.Description = Description;
                            result.EmailId = EmailId;
                            db.SaveChanges();
                        }
                        _log4net.Info("Function Name : Employee UpdateEmployee Update Successfully");

                        return "Success";
                    }
                    else
                    {
                        _log4net.Error("Function Name : Employee UpdateEmployee Auth Failed");
                        return "Auth Fail";
                    }
                }
                else
                {
                    _log4net.Error("Function Name : Employee UpdateEmployee Auth Failed" );
                    return "Auth Fail";
                }
            }
            catch (Exception ex)
            {
                _log4net.Error("Function Name : Employee UpdateEmployee  -- " + ex.ToString());
                return "Fail";
            }

        }
        public string SaveEmployee(string Emp_No,string E_Name,string Emp_pass,int Emp_role,int Status,string Description,string EmailId)
        {
            try
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
                    var resultToken = db.TokenValue.FirstOrDefault(b => b.TockenId == Auth && b.Validto >= currentdatetime);
                    if (resultToken != null)
                    {
                        var result = db.Login.FirstOrDefault(x => x.EmployeeName == E_Name);
                        if (result != null)
                        {
                            return "Fail";
                        }
                        Login Ins = new Login();
                        Ins.EmployeeName = E_Name;
                        Ins.EmployeeNo = Emp_No;
                        Ins.Password = Emp_pass;
                        Ins.RoleId = Emp_role;
                        Ins.Status = Status;
                        Ins.UserName = E_Name;
                        Ins.EmailId = EmailId;
                        Ins.Description = Description;
                        db.Login.Add(Ins);
                        db.SaveChanges();
                        _log4net.Info("Function Name : Employee SaveEmployee Saved Successfully");
                        return "Success";
                    }
                    else
                    {
                        _log4net.Error("Function Name : Employee SaveEmployee Auth Failed ");
                        return "Auth Fail";

                    }

                }
                else
                {
                    _log4net.Error("Function Name : Employee SaveEmployee Auth Failed ");
                    return "Auth Fail";
                }
            }
            catch(Exception ex)
            {
                _log4net.Error("Function Name : Employee SaveEmployee  -- " + ex.ToString());
                return "Fail";
            }
          
        }

        public JsonResult EditEmployee(int LoginId)
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
                    var resultToken = db.TokenValue.FirstOrDefault(b => b.TockenId == Auth && b.Validto >= currentdatetime);
                    if (resultToken != null)
                    {
                        var a = from b in db.Login
                                where b.LoginId == LoginId

                                select new
                                {
                                    b.EmailId,
                                    b.LoginId,
                                    b.EmployeeName,
                                    b.EmployeeNo,
                                    b.RoleId,
                                    b.Password,
                                    b.Status,
                                    b.Description,
                                };
                        _log4net.Info("Function Name : Employee EditEmployee loaded Successfully ");
                        return Json(a);
                    }
                    else
                    {
                        _log4net.Error("Function Name : Employee EditEmployee Auth Failed ");
                        return Json("Auth Fail");
                    }
                }
                else
                {
                    _log4net.Error("Function Name : Employee EditEmployee Auth Failed ");
                    return Json("Auth Fail");
                }

            }
            catch(Exception ex)
            {
                _log4net.Error("Function Name : Employee EditEmployee  -- " + ex.ToString());
                return null;
            }
        }

        public JsonResult BindEmployee(int status)
        {
            try
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
                    var resultToken = db.TokenValue.FirstOrDefault(b => b.TockenId == Auth && b.Validto >= currentdatetime);
                    if (resultToken != null)
                    {
                        var a = from b in db.Login
                              //  where b.Status == status
                              orderby b.Status descending
                                select new
                                {
                                    b.EmployeeName,
                                    b.EmailId,
                                    b.EmployeeNo,
                                    b.Password,
                                    Description = (b.Description == null ? "" : b.Description),
                                    Status = b.Status == 1 ? "Active" : "Inactive",
                                    b.LoginId,

                                };
                        _log4net.Info("Function Name : Employee BindEmployee loaded Successfully");
                        return Json(a);
                    }
                    else
                    {
                        _log4net.Error("Function Name : Employee BindEmployee Auth Failed");
                        return Json("Auth Fail");
                    }
                }
                else
                {
                    _log4net.Error("Function Name : Employee BindEmployee Auth Failed");
                    return Json("Auth Fail");
                }
            }
            catch(Exception ex)
            {
                _log4net.Error("Function Name : Employee BindEmployee  -- " + ex.ToString());
                return null;
            }
        }
        public JsonResult SearchEmployee(int status,string search)
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

                            var a = from b in db.Login
                                    where b.Status == status
                                    select new
                                    {
                                        b.EmployeeName,
                                        b.EmailId,
                                        b.EmployeeNo,
                                        b.Password,
                                        Description = (b.Description == null ? "" : b.Description),
                                        Status = b.Status == 1 ? "Active" : "Inactive",
                                        b.LoginId,

                                    };
                            _log4net.Info("Function Name : Employee SearchEmployee loaded Successfully");
                            return Json(a);
                        }

                        else
                        {
                            var a = (from b in db.Login
                                     where b.Status == status
                                     select new
                                     {
                                         b.EmployeeName,
                                         b.EmailId,
                                         b.EmployeeNo,
                                         b.Password,
                                         Description = (b.Description == null ? "" : b.Description),
                                         Status = b.Status == 1 ? "Active" : "Inactive",
                                         b.LoginId,

                                     }).Where(x => x.EmployeeName.Contains(search) || x.EmailId.Contains(search) || x.EmployeeNo.Contains(search) || x.Description.Contains(search));
                            _log4net.Info("Function Name : Employee SearchEmployee loaded Successfully");
                            return Json(a);
                        }
                     
                    }
                    else
                    {
                        _log4net.Error("Function Name : Employee SearchEmployee Auth Failed");
                        return Json("Auth Fail");
                    }
                }
                else
                {
                    _log4net.Error("Function Name : Employee SearchEmployee Auth Failed");
                    return Json("Auth Fail");
                }
            }
            catch (Exception ex)
            {
                _log4net.Error("Function Name : Employee SearchEmployee  -- " + ex.ToString());
                return null;
            }
        }

        public JsonResult SearchUserRole(string status, string search)
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

                            var a = from b in db.Role
                                    where b.Status == status
                                    select new
                                    {
                                        b.RoleId,
                                        b.RoleName,
                                        Description = (b.Description == null ? "" : b.Description),
                                        Status = b.Status == "1" ? "Active" : "Inactive",
                                    };
                            _log4net.Info("Function Name : User Role SearchUserRole loaded Successfully ");
                            return Json(a);
                        }

                        else
                        {
                            var a = (from b in db.Role
                                     where b.Status == status
                                     select new
                                     {
                                         b.RoleId,
                                         b.RoleName,
                                         Description = (b.Description == null ? "" : b.Description),
                                         Status = b.Status == "1" ? "Active" : "Inactive",
                                     }).Where(x => x.RoleName.Contains(search) || x.Description.Contains(search));
                            _log4net.Info("Function Name : User Role SearchUserRole loaded Successfully ");
                            return Json(a);
                        }

                    }
                    else
                    {
                        _log4net.Error("Function Name : User Role SearchUserRole  Auth Failed ");
                        return Json("Auth Fail");
                    }
                }
                else
                {
                    _log4net.Error("Function Name : User Role SearchUserRole  Auth Failed ");
                    return Json("Auth Fail");
                }
            }
            catch (Exception ex)
            {
                _log4net.Error("Function Name : User Role SearchUserRole  -- " + ex.ToString());
                return null;
            }
        }
        public string SaveRole(string R_Name,string Role,string Status,string Description)
        {
            try
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
                    var resultToken = db.TokenValue.FirstOrDefault(b => b.TockenId == Auth && b.Validto >= currentdatetime);
                    if (resultToken != null)
                    {
                        var role = db.Role.FirstOrDefault(x => x.RoleName == R_Name);
                        if (role != null)
                        {
                            return "Fail";
                        }
                        Role Ins = new Role();
                        Ins.Createdate = DateTime.Now;
                        Ins.RoleName = R_Name;
                        Ins.ScreenName = Role;
                        Ins.Status = Status;
                        Ins.Description = Description;
                        db.Role.Add(Ins);
                        db.SaveChanges();
                        _log4net.Error("Function Name : User Role SaveRole loaded Successfully");
                        return "success";
                    }
                    else
                    {
                        _log4net.Error("Function Name : User Role SaveRole Auth Failed");
                        return "Auth Fail";
                    }
                }
                else
                {
                    _log4net.Error("Function Name : User Role SaveRole Auth Failed");
                    return "Auth Fail";
                }
            }
            catch (Exception ex)
            {
                _log4net.Error("Function Name : User Role SaveRole  -- " + ex.ToString());
                return null;
            }
        }

        public JsonResult UserBinds(string status)
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
                    var resultToken = db.TokenValue.FirstOrDefault(b => b.TockenId == Auth && b.Validto >= currentdatetime);
                    if (resultToken != null)
                    {
                        var a = from b in db.Role
                               // where b.Status == status
                               orderby b.Status descending
                                select new
                                {
                                    b.RoleId,
                                    b.RoleName,
                                    Description = (b.Description == null ? "" : b.Description),
                                    Status = b.Status == "1" ? "Active" : "Inactive",
                                };
                        _log4net.Info("Function Name : User Role UserBinds loaded Successfully");
                        return Json(a);
                    }
                    else
                    {
                        _log4net.Error("Function Name : User Role UserBinds Auth Failed");
                        return Json("Auth Fail");
                    }
                }
                else
                {
                    _log4net.Error("Function Name : User Role UserBinds Auth Failed");
                    return Json("Auth Fail");
                }
            }
            catch(Exception ex)
            {
                _log4net.Error("Function Name : User Role UserBinds  -- " + ex.ToString());
                return null;
            }
        }

        public JsonResult EditRole(int RoleId)
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
                        var a = from b in db.Role
                                where b.RoleId == RoleId
                                select new
                                {
                                    b.RoleId,
                                    b.ScreenName,
                                    b.Status,
                                    b.Description,
                                    b.RoleName,
                                };
                        _log4net.Info("Function Name : User Role EditRole loaded Successfully");
                        return Json(a);
                    }
                    else
                    {
                        _log4net.Error("Function Name : User Role EditRole  Auth Failed");
                        return Json("Auth Fail");
                    }
                }
                else
                {
                    _log4net.Error("Function Name : User Role EditRole  Auth Failed");
                    return Json("Auth Fail");
                }
            }
            catch(Exception ex)
            {
                _log4net.Error("Function Name : User Role EditRole  -- " + ex.ToString());
                return null;
            }
           
        }


        public string UpdateRole(string R_Name,string Role,string Status,string Description,int UserRole)
        {

            try
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
                    var resultToken = db.TokenValue.FirstOrDefault(b => b.TockenId == Auth && b.Validto >= currentdatetime);
                    if (resultToken != null)
                    {
                        var result = db.Role.FirstOrDefault(x => x.RoleId == UserRole);
                        if (result != null)
                        {
                            if (Status == "1")
                            {
                                result.RoleName = R_Name;
                                result.ScreenName = Role;
                                result.Status = Status;
                                result.Description = Description;
                                db.SaveChanges();
                            }
                            else
                            {
                                var emp = db.Login.FirstOrDefault(x => x.RoleId == result.RoleId && x.Status == 1);
                                if(emp == null)
                                {
                                    result.RoleName = R_Name;
                                    result.ScreenName = Role;
                                    result.Status = Status;
                                    result.Description = Description;
                                    db.SaveChanges();
                                }
                                else
                                {
                                    _log4net.Info("Function Name : User Role EditRole loaded Successfully");
                                    return "employee";
                                }

                            }
                           
                        }
                        _log4net.Info("Function Name : User Role EditRole loaded Successfully");
                        return "success";
                    }
                    else
                    {
                        _log4net.Error("Function Name : User Role EditRole  Auth Failed");
                        return "Auth Fail";
                    }
                }
                else
                {
                    _log4net.Error("Function Name : User Role EditRole  Auth Failed");
                    return "Auth Fail";
                }
            }
            catch (Exception ex)
            {
                _log4net.Error("Function Name : User Role EditRole  -- " + ex.ToString());
                return null;
            }
        }

        public IActionResult UserRole()
        {
            try
            {
                ViewBag.Report =configuration.GetValue<string>("ReportPortalIp");
                _log4net.Info("Function Name : User Role UserRole loaded Successfully");
                return View();
            }
            catch(Exception ex)
            {
                _log4net.Error("Function Name : User Role EditRole  -- " + ex.ToString());
                return null;
            }
        }

        public JsonResult loginid(int loginid)
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
                        db.Login.RemoveRange(db.Login.Where(x => x.LoginId == loginid));
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


        public JsonResult Roleid(int roleid)
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
                        db.Role.RemoveRange(db.Role.Where(x => x.RoleId == roleid));
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
