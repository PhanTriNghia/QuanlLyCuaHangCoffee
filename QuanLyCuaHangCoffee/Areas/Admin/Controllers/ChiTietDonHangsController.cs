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
    public class ChiTietDonHangsController : Controller
    {
        private QLCHUOICOFFEEEntities db = new QLCHUOICOFFEEEntities();

        // GET: Admin/ChiTietDonHangs
        public ActionResult Index(int? page)
        {
            var pageSize = 10;
            if (page == null)
            {
                page = 1;
            }
            var pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            var ctDonHang = db.ChiTietDonHangs.OrderByDescending(s => s.IDChiTietDH).ToPagedList(pageIndex, pageSize);
            return View(ctDonHang);
        }

        // GET: Admin/ChiTietDonHangs/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ChiTietDonHang chiTietDonHang = db.ChiTietDonHangs.Find(id);
            if (chiTietDonHang == null)
            {
                return HttpNotFound();
            }
            return View(chiTietDonHang);
        }

        // GET: Admin/ChiTietDonHangs/Create
        public ActionResult Create()
        {
            ViewBag.IDDonHang = new SelectList(db.DonHangs, "IDDonHang", "TinhTrang");
            ViewBag.IDSanPham = new SelectList(db.SanPhams, "IDSanPham", "TenSP");
            return View();
        }

        // POST: Admin/ChiTietDonHangs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IDChiTietDH,IDDonHang,IDSanPham,SoLuongSP,TongTienSP")] ChiTietDonHang chiTietDonHang)
        {
            if (ModelState.IsValid)
            {
                db.ChiTietDonHangs.Add(chiTietDonHang);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.IDDonHang = new SelectList(db.DonHangs, "IDDonHang", "TinhTrang", chiTietDonHang.IDDonHang);
            ViewBag.IDSanPham = new SelectList(db.SanPhams, "IDSanPham", "TenSP", chiTietDonHang.IDSanPham);
            return View(chiTietDonHang);
        }

        // GET: Admin/ChiTietDonHangs/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ChiTietDonHang chiTietDonHang = db.ChiTietDonHangs.Find(id);
            if (chiTietDonHang == null)
            {
                return HttpNotFound();
            }
            ViewBag.IDDonHang = new SelectList(db.DonHangs, "IDDonHang", "TinhTrang", chiTietDonHang.IDDonHang);
            ViewBag.IDSanPham = new SelectList(db.SanPhams, "IDSanPham", "TenSP", chiTietDonHang.IDSanPham);
            return View(chiTietDonHang);
        }

        // POST: Admin/ChiTietDonHangs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IDChiTietDH,IDDonHang,IDSanPham,SoLuongSP,TongTienSP")] ChiTietDonHang chiTietDonHang)
        {
            if (ModelState.IsValid)
            {
                db.Entry(chiTietDonHang).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.IDDonHang = new SelectList(db.DonHangs, "IDDonHang", "TinhTrang", chiTietDonHang.IDDonHang);
            ViewBag.IDSanPham = new SelectList(db.SanPhams, "IDSanPham", "TenSP", chiTietDonHang.IDSanPham);
            return View(chiTietDonHang);
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            var item = db.ChiTietDonHangs.Find(id);
            if (item != null)
            {
                db.ChiTietDonHangs.Remove(item);
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
