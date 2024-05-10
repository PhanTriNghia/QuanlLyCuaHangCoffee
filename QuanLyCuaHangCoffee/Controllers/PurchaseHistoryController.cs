using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using QuanLyCuaHangCoffee.Models.EF;

namespace QuanLyCuaHangCoffee.Controllers
{
    public class PurchaseHistoryController : Controller
    {
        QLCHUOICOFFEEEntities db = new QLCHUOICOFFEEEntities();
        // GET: PurcharseHistory
        public ActionResult Index()
        {
            var purchaseHistory = db.PurchaseHisotries
                .Where(x => x.Username == User.Identity.Name)
                .OrderByDescending(x => x.IDSanPham)
                .ToList();
            return View(purchaseHistory);
        }
    }
}