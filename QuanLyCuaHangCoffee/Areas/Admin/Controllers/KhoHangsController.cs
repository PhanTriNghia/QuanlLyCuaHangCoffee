using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PagedList;
using QuanLyCuaHangCoffee.Models.EF;

namespace QuanLyCuaHangCoffee.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin,Nhân Viên")]
    public class KhoHangsController : Controller
    {  
        private QLCHUOICOFFEEEntities db = new QLCHUOICOFFEEEntities();

        // GET: Admin/KhoHangs
        public ActionResult Index(int? page)
        {
            var pageSize = 10;
            if (page == null)
            {
                page = 1;
            }
            var pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            var khoHangs = db.KhoHangs.OrderByDescending(s => s.IDNguyenLieu).ToPagedList(pageIndex, pageSize);
            return View(khoHangs);
        }

        // GET: Admin/KhoHangs/Create
        public ActionResult Create()
        {
            ViewBag.IDNguyenLieu = new SelectList(db.NguyenLieux, "IDNguyenLieu", "TenNguyenLieu");
            return View();
        }

        // POST: Admin/KhoHangs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IDKhoHang,IDNguyenLieu,NgayGiao,SoLuongGiao,TrangThai")] KhoHang khoHang)
        {
            if (ModelState.IsValid)
            {
                
                DateTime selectedDate = (DateTime)khoHang.NgayGiao;
                khoHang.NgayGiao = ((DateTime)khoHang.NgayGiao).Date;
                if (khoHang.SoLuongGiao > 0)
                {
                    khoHang.TrangThai = true; // còn hàng
                }
                else
                {
                    khoHang.TrangThai = false; // hết hàng
                }           
                db.KhoHangs.Add(khoHang);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.IDNguyenLieu = new SelectList(db.NguyenLieux, "IDNguyenLieu", "TenNguyenLieu", khoHang.IDNguyenLieu);
            return View(khoHang);
        }

        // GET: Admin/KhoHangs/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            KhoHang khoHang = db.KhoHangs.Find(id);
            if (khoHang == null)
            {
                return HttpNotFound();
            }
            ViewBag.IDNguyenLieu = new SelectList(db.NguyenLieux, "IDNguyenLieu", "TenNguyenLieu", khoHang.IDNguyenLieu);
            return View(khoHang);
        }

        // POST: Admin/KhoHangs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IDKhoHang,IDNguyenLieu,NgayGiao,SoLuongGiao,TrangThai")] KhoHang khoHang)
        {
            if (ModelState.IsValid)
            {
                db.Entry(khoHang).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.IDNguyenLieu = new SelectList(db.NguyenLieux, "IDNguyenLieu", "TenNguyenLieu", khoHang.IDNguyenLieu);
            return View(khoHang);
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            var item = db.KhoHangs.Find(id);
            if (item != null)
            {
                db.KhoHangs.Remove(item);
                db.SaveChanges();
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
