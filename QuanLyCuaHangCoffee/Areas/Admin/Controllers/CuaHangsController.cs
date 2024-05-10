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
    public class CuaHangsController : Controller
    {
        private QLCHUOICOFFEEEntities db = new QLCHUOICOFFEEEntities();

        // GET: Admin/CuaHangs
        public ActionResult Index(int? page)
        {
            var pageSize = 10;
            if (page == null)
            {
                page = 1;
            }
            var pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            var cuaHangs = db.CuaHangs.OrderByDescending(s => s.IDCuaHang).ToPagedList(pageIndex, pageSize);
            return View(cuaHangs);
        }

        // GET: Admin/CuaHangs/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/CuaHangs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IDCuaHang,DiaChiCH,SoLuongNV,TrangThai")] CuaHang cuaHang, bool trangThaiMoi)
        {
            if (ModelState.IsValid)
            {
                cuaHang.TrangThai = trangThaiMoi;
                db.CuaHangs.Add(cuaHang);
                db.SaveChanges();
               
                return RedirectToAction("Index");
            }

            return View(cuaHang);
        }
       
        // GET: Admin/CuaHangs/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CuaHang cuaHang = db.CuaHangs.Find(id);
            if (cuaHang == null)
            {
                return HttpNotFound();
            }
            return View(cuaHang);
        }

        // POST: Admin/CuaHangs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IDCuaHang,DiaChiCH,SoLuongNV,SDTCuaHang")] CuaHang cuaHang, int id, bool trangThaiMoi, string diaChi, string SDT)
        {
            cuaHang = db.CuaHangs.Find(id);
            if (cuaHang != null && ModelState.IsValid)
            {
                cuaHang.TrangThai = trangThaiMoi;
                cuaHang.DiaChiCH = diaChi;
                cuaHang.SDTCuaHang = SDT;
                db.Entry(cuaHang).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(cuaHang);
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            var item = db.CuaHangs.Find(id);
            if (item != null)
            {
                db.CuaHangs.Remove(item);
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
