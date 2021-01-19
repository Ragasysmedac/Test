using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace XAutomateMVC.Controllers
{
    public class UserMangementcontroller : Controller
    {
        public IActionResult Emplyoee()
        {
            return View();
        }
    }
}
