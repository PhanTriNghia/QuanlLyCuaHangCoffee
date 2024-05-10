using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using QuanLyCuaHangCoffee.Models.EF;

namespace QuanLyCuaHangCoffee.Areas.Admin.Controllers
{
    public class CommentController : Controller
    {
        QLCHUOICOFFEEEntities db = new QLCHUOICOFFEEEntities();
        // GET: Admin/Comment
        public ActionResult Index(int? page)
        {
            var pageSize = 10;
            if (page == null)
            {
                page = 1;
            }
            var pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            var comments = db.Comments.OrderByDescending(s => s.IDComment).ToPagedList(pageIndex, pageSize);
            return View(comments);
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            var item = db.Comments.Find(id);
            if (item != null)
            {
                db.Comments.Remove(item);
                db.SaveChanges();
                return Json(new { success = true });
            }

            return Json(new { success = false });
        }

        public ActionResult IsActive(int id)
        {
            var item = db.Comments.Find(id);
            if (item != null)
            {
                item.TrangThai = !item.TrangThai;
                db.Entry(item).State = EntityState.Modified;
                db.SaveChanges();
                return Json(new { success = true, isActive = item.TrangThai });
            }

            return Json(new { success = false });
        }
    }
}