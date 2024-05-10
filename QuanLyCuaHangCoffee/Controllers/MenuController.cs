using QuanLyCuaHangCoffee.Models.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace QuanLyCuaHangCoffee.Controllers
{
    public class MenuController : Controller
    {
        QLCHUOICOFFEEEntities db = new QLCHUOICOFFEEEntities();
        // GET: Menu
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult MenuProductCategory()
        {
            var items = db.LoaiSPs.ToList();
            return PartialView("_MenuProductCategory", items);
        }
    }
}
