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
    [Authorize(Roles = "Admin")]
    public class SanPhamsController : Controller
    {
        private QLCHUOICOFFEEEntities db = new QLCHUOICOFFEEEntities();

        // GET: Admin/SanPhams
        public ActionResult Index(int? page)
        {
            /*var pageSize = 10;
            if(page == null)
            {
                page = 1;
            }
            var pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            var sanPhams = db.SanPhams.OrderByDescending(s => s.IDCuaHang).ToPagedList(pageIndex, pageSize);
            return View(sanPhams);*/
            var sanPhams = db.SanPhams.Where(s => s.TrangThai == true).ToList();
            return View(sanPhams);
        }

        // GET: Admin/SanPhams/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SanPham sanPham = db.SanPhams.Find(id);
            if (sanPham == null)
            {
                return HttpNotFound();
            }
            return View(sanPham);
        }

        // GET: Admin/SanPhams/Create
        public ActionResult Create()
        {
            ViewBag.IDLoaiSP = new SelectList(db.LoaiSPs, "IDLoaiSP", "TenLSP");
            ViewBag.IDCuaHang = new SelectList(db.CuaHangs, "IDCuaHang", "DiaChiCH");
            return View();
        }

        // POST: Admin/SanPhams/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IDSanPham,IDLoaiSP,IDCuaHang,TenSP,HinhAnhSP,GiaBan,MoTa,TrangThai")] SanPham sanPham)
        {
            if (ModelState.IsValid)
            {
                db.SanPhams.Add(sanPham);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.IDLoaiSP = new SelectList(db.LoaiSPs, "IDLoaiSP", "TenLSP", sanPham.IDLoaiSP);
            ViewBag.IDCuaHang = new SelectList(db.CuaHangs, "IDCuaHang", "DiaChiCH", sanPham.IDCuaHang);
            return View(sanPham);
        }

        // GET: Admin/SanPhams/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SanPham sanPham = db.SanPhams.Find(id);
            if (sanPham == null)
            {
                return HttpNotFound();
            }
            ViewBag.IDLoaiSP = new SelectList(db.LoaiSPs, "IDLoaiSP", "TenLSP", sanPham.IDLoaiSP);
            ViewBag.IDCuaHang = new SelectList(db.CuaHangs, "IDCuaHang", "DiaChiCH", sanPham.IDCuaHang);
            return View(sanPham);
        }
  
        public ActionResult IsActive(int id)
        {
            var item = db.SanPhams.Find(id);
            if (item != null)
            {
                item.TrangThai = !item.TrangThai;
                db.Entry(item).State = EntityState.Modified;
                db.SaveChanges();
                return Json(new { success = true, isActive = item.TrangThai });
            }

            return Json(new { success = false });
        }

        // POST: Admin/SanPhams/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IDSanPham,IDLoaiSP,IDCuaHang,TenSP,HinhAnhSP,GiaBan,MoTa,TrangThai")] SanPham sanPham)
        {
            if (ModelState.IsValid)
            {
                db.Entry(sanPham).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.IDLoaiSP = new SelectList(db.LoaiSPs, "IDLoaiSP", "TenLSP", sanPham.IDLoaiSP);
            ViewBag.IDCuaHang = new SelectList(db.CuaHangs, "IDCuaHang", "DiaChiCH", sanPham.IDCuaHang);
            return View(sanPham);
        }


        [HttpPost]
        public ActionResult Delete(int id)
        {
            var item = db.SanPhams.Find(id);
            if (item != null)
            {
                db.SanPhams.Remove(item);
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
