using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using XAutomateMVC.Models;
using XAutomateMVC.Models.DBModels;

namespace XAutomateMVC.Controllers
{
    public class UserManagementController : Controller
    {
        db_mateContext db = new db_mateContext();
        public IActionResult Employee()
        {
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
            return View(productViewModel);
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
                    TimeZoneInfo timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Etc/UTC");
                    DateTime dtCNow = TimeZoneInfo.ConvertTime(Convert.ToDateTime(DateTime.Now), timeZoneInfo);
                    string dtCNowdate = Convert.ToDateTime(dtCNow).ToString("yyyy-MM-dd");
                    TimeSpan tsnow = Convert.ToDateTime(dtCNow).TimeOfDay;
                    DateTime currentdatetime = Convert.ToDateTime(dtCNowdate + " " + (tsnow.Hours) + ":" + tsnow.Minutes);
                    var resultToken = db.TokenValue.First(b => b.TockenId == Auth && b.Validto >= currentdatetime);
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
            catch (Exception ex)
            {
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
                    TimeZoneInfo timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Etc/UTC");
                    DateTime dtCNow = TimeZoneInfo.ConvertTime(Convert.ToDateTime(DateTime.Now), timeZoneInfo);
                    string dtCNowdate = Convert.ToDateTime(dtCNow).ToString("yyyy-MM-dd");
                    TimeSpan tsnow = Convert.ToDateTime(dtCNow).TimeOfDay;
                    DateTime currentdatetime = Convert.ToDateTime(dtCNowdate + " " + (tsnow.Hours) + ":" + tsnow.Minutes);
                    var resultToken = db.TokenValue.First(b => b.TockenId == Auth && b.Validto >= currentdatetime);
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

        public JsonResult EditEmployee(int LoginId)
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

        public JsonResult BindEmployee(int status)
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
                        var a = from b in db.Login
                                where b.Status == status
                                select new
                                {
                                    b.EmployeeName,
                                    b.EmailId,
                                    b.EmployeeNo,
                                    b.Password,
                                    Description = (b.Description == null ? "" : b.Description),
                                    Status = b.Status == 1 ? "Active" : "InActive",
                                    b.LoginId,

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

        public string SaveRole(string R_Name,string Role,string Status,string Description)
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
                        return "success";
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
            catch (Exception ex)
            {
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
                    TimeZoneInfo timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Etc/UTC");
                    DateTime dtCNow = TimeZoneInfo.ConvertTime(Convert.ToDateTime(DateTime.Now), timeZoneInfo);
                    string dtCNowdate = Convert.ToDateTime(dtCNow).ToString("yyyy-MM-dd");
                    TimeSpan tsnow = Convert.ToDateTime(dtCNow).TimeOfDay;
                    DateTime currentdatetime = Convert.ToDateTime(dtCNowdate + " " + (tsnow.Hours) + ":" + tsnow.Minutes);
                    var resultToken = db.TokenValue.First(b => b.TockenId == Auth && b.Validto >= currentdatetime);
                    if (resultToken != null)
                    {
                        var a = from b in db.Role
                                where b.Status == status
                                select new
                                {
                                    b.RoleId,
                                    b.RoleName,
                                    Description = (b.Description == null ? "" : b.Description),
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

        public JsonResult EditRole(int RoleId)
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


        public string UpdateRole(string R_Name,string Role,string Status,string Description,int UserRole)
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
                        var result = db.Role.FirstOrDefault(x => x.RoleId == UserRole);
                        if (result != null)
                        {
                            result.RoleName = R_Name;
                            result.ScreenName = Role;
                            result.Status = Status;
                            result.Description = Description;
                            db.SaveChanges();
                        }

                        return "success";
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
            catch (Exception ex)
            {
                return null;
            }
        }

        public IActionResult UserRole()
        {
            return View();
        }

    }
}
