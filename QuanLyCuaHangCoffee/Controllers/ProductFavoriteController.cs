using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using QuanLyCuaHangCoffee.Models.EF;

namespace QuanLyCuaHangCoffee.Controllers
{
    public class ProductFavoriteController : Controller
    {
        QLCHUOICOFFEEEntities db = new QLCHUOICOFFEEEntities();
        // GET: ProductFavarite
        public ActionResult Index()
        {
            var productFavorite = db.ProductFavorites.Where(x => x.Username == User.Identity.Name).ToList();
            return View(productFavorite);
        }
        [HttpPost]
        [AllowAnonymous]
        public ActionResult Favorite(int idProduct)
        {
            if (Request.IsAuthenticated == false)
            {
                return RedirectToAction("Login","Account");
            }

            var userName = User.Identity.Name;
            var existingFavorite = db.ProductFavorites.FirstOrDefault(x => x.IDSanPham == idProduct && x.Username == userName);

            if(existingFavorite != null)
            {
                db.ProductFavorites.Remove(existingFavorite);
            }
            else
            {
                var newFavorite = new ProductFavorite
                {
                    Username = userName,
                    IDSanPham = idProduct,
                };
                db.ProductFavorites.Add(newFavorite);
            }
            db.SaveChanges();

            ViewBag.IsFavorite = db.ProductFavorites.Any(x => x.IDSanPham == idProduct && x.Username == User.Identity.Name);

            return RedirectToAction("Index","ProductFavorite");
        }
    }
}