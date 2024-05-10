using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using QuanLyCuaHangCoffee.Models.EF;
namespace QuanLyCuaHangCoffee.Controllers
{
    public class CommentController : Controller
    {
        QLCHUOICOFFEEEntities db = new QLCHUOICOFFEEEntities();
        // GET: Comment
        public ActionResult Index(int IDSanPham)
        {
            var comments = db.Comments.Where(x => x.TrangThai == true && x.IDSanPham == IDSanPham).ToList();
            return PartialView(comments);
        }
        //Get
        public ActionResult PostComment()
        {
            return PartialView();
        }

        [HttpPost]
        public ActionResult PostComment(Comment cmt, int IDSanPham)
        {
            if (ModelState.IsValid)
            {
                cmt.Username = User.Identity.Name;
                cmt.Ngaytao = DateTime.Now;
                cmt.IDSanPham = IDSanPham;
                cmt.TrangThai = false;
                db.Comments.Add(cmt);
                db.SaveChanges();
            }
            return PartialView();
        }
    }
}