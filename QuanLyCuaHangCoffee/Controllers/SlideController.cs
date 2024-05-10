using QuanLyCuaHangCoffee.Models.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QuanLyCuaHangCoffee.Controllers
{
    public class SlideController : Controller
    {
        QLCHUOICOFFEEEntities db = new QLCHUOICOFFEEEntities();
        // GET: Slide
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ListAllSlide()
        {
            var items = db.Sliders.ToList();
            return PartialView("_ListAllSlide", items);
        }
    }
}
