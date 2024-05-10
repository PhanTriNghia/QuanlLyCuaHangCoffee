using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PagedList;
using QuanLyCuaHangCoffee.Models.EF;

namespace QuanLyCuaHangCoffee.Areas.Admin.Controllers
{
    public class NhanViensController : Controller
    {
        private QLCHUOICOFFEEEntities db = new QLCHUOICOFFEEEntities();

        // GET: Admin/NhanViens
        public ActionResult Index(int? page)
        {
            var pageSize = 10;
            if (page == null)
            {
                page = 1;
            }
            var pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            var nhanViens = db.NhanViens.OrderByDescending(s => s.IDCuaHang).ToPagedList(pageIndex, pageSize);
            return View(nhanViens);
        }

        // GET: Admin/NhanViens/Create
        public ActionResult Create()
        {
            ViewBag.IDCuaHang = new SelectList(db.CuaHangs, "IDCuaHang", "DiaChiCH");
            return View();
        }

        // POST: Admin/NhanViens/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IDNhanVien,IDCuaHang,TenNV,NgaySinh,DiaChiNV,NgayLam,TrangThai,Luong")] NhanVien nhanVien,CuaHang cuaHang, bool trangThaiMoi)
        {
            if (ModelState.IsValid)
            {   
                DateTime selectedNgaySinh = (DateTime)nhanVien.NgaySinh;
                nhanVien.NgaySinh = ((DateTime)nhanVien.NgaySinh).Date;
                DateTime selectedNgayLam = (DateTime)nhanVien.NgaySinh;
                nhanVien.NgayLam = ((DateTime)nhanVien.NgayLam).Date;
                nhanVien.TrangThai = trangThaiMoi;
                db.NhanViens.Add(nhanVien);
                db.SaveChanges();

                //đếm số nhân viên
                var soLuongNV = (from nv in db.NhanViens
                                 where nv.IDCuaHang == cuaHang.IDCuaHang
                                 select nv).Count();
                cuaHang.SoLuongNV = soLuongNV;
                
                var diaChiCuaHang = db.CuaHangs.Where(c => c.IDCuaHang == nhanVien.IDCuaHang).Select(c => c.DiaChiCH).FirstOrDefault();
                cuaHang.DiaChiCH = diaChiCuaHang;
                var sdtCuaHang = db.CuaHangs.Where(c => c.IDCuaHang == nhanVien.IDCuaHang).Select(c => c.SDTCuaHang).FirstOrDefault();
                cuaHang.SDTCuaHang = sdtCuaHang;
                var trangThaiCH = db.CuaHangs.Where(c => c.IDCuaHang == nhanVien.IDCuaHang).Select(c => c.TrangThai).FirstOrDefault();
                cuaHang.TrangThai = trangThaiCH;
                db.Entry(cuaHang).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.IDCuaHang = new SelectList(db.CuaHangs, "IDCuaHang", "DiaChiCH", nhanVien.CuaHang.DiaChiCH);
            return View(nhanVien);
        }

        // GET: Admin/NhanViens/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NhanVien nhanVien = db.NhanViens.Find(id);
            if (nhanVien == null)
            {
                return HttpNotFound();
            }
            ViewBag.IDCuaHang = new SelectList(db.CuaHangs, "IDCuaHang", "DiaChiCH", nhanVien.IDCuaHang);
            return View(nhanVien);
        }

        // POST: Admin/NhanViens/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IDNhanVien,IDCuaHang,TenNV,NgaySinh,DiaChiNV,NgayLam,TrangThai,Luong")] NhanVien nhanVien, CuaHang cuaHang)
        {
            if (ModelState.IsValid)
            {
                cuaHang = db.CuaHangs.FirstOrDefault(c => c.IDCuaHang == nhanVien.IDCuaHang);
                var allCuaHang = db.CuaHangs.ToList();              
                var diaChiCuaHang = db.CuaHangs.Where(c => c.IDCuaHang == nhanVien.IDCuaHang).Select(c => c.DiaChiCH).FirstOrDefault();
                cuaHang.DiaChiCH = diaChiCuaHang;
                var sdt = db.CuaHangs.Where(c => c.IDCuaHang == nhanVien.IDCuaHang).Select(c => c.SDTCuaHang).FirstOrDefault();
                cuaHang.SDTCuaHang = sdt;
                db.Entry(nhanVien).State = EntityState.Modified;
                db.SaveChanges();
                //đếm số lượng của tất cả cửa hàng 
                foreach (var cuahang in allCuaHang)
                {
                    var soLuongNV = db.NhanViens.Count(nv => nv.IDCuaHang == cuahang.IDCuaHang);
                    cuahang.SoLuongNV = soLuongNV;
                }
                foreach(var cuahang in allCuaHang)
                {
                    db.Entry(cuahang).State = EntityState.Modified;                 
                }
                db.SaveChanges();
                return RedirectToAction("Index");

            }
            ViewBag.IDCuaHang = new SelectList(db.CuaHangs, "IDCuaHang", "DiaChiCH", nhanVien.IDCuaHang);
            return View(nhanVien);
        }

        [HttpPost]
        public ActionResult Delete(int id, CuaHang cuaHang, NhanVien nhanVien)
        {
            var item = db.NhanViens.Find(id);
            if (item != null)
            {
                db.NhanViens.Remove(item);
                db.SaveChanges();
                cuaHang = db.CuaHangs.FirstOrDefault(c => c.IDCuaHang == nhanVien.IDCuaHang);
                var allCuahangs = db.CuaHangs.ToList();
                foreach (var cuahang in allCuahangs)
                {
                    var soLuongNV = db.NhanViens.Count(nv => nv.IDCuaHang == cuahang.IDCuaHang);
                    cuahang.SoLuongNV = soLuongNV;
                }
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
