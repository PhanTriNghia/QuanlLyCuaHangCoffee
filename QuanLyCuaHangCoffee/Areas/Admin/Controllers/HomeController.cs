using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QuanLyCuaHangCoffee.Areas.Admin.Controllers
{
   [Authorize(Roles = "Admin, Nhân Viên")]
    public class HomeController : Controller
    {
        // GET: Admin/Home
        public ActionResult Index()
        {
            
            return View();
        }

        public ActionResult trangchu()
        {
            return View();
        }

    }
}