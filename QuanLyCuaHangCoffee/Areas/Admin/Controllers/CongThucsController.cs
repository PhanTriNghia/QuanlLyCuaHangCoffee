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
    public class CongThucsController : Controller
    {
        private QLCHUOICOFFEEEntities db = new QLCHUOICOFFEEEntities();

        // GET: Admin/CongThucs
        public ActionResult Index(int? page)
        {
            var pageSize = 10;
            if (page == null)
            {
                page = 1;
            }
            var pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            var congThucs = db.CongThucs.OrderByDescending(s => s.IDCongThuc).ToPagedList(pageIndex, pageSize);
            return View(congThucs);
        }     

        // GET: Admin/CongThucs/Create
        public ActionResult Create()
        {
            ViewBag.IDSanPham = new SelectList(db.SanPhams, "IDSanPham", "TenSP");
            return View();
        }

        // POST: Admin/CongThucs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IDCongThuc,IDSanPham,Noidung")] CongThuc congThuc)
        {
            var allSanPham = db.SanPhams.ToList();
            var selectSanPham = db.SanPhams.FirstOrDefault(sp => sp.IDSanPham == congThuc.IDSanPham);
            if (selectSanPham != null)
            {
                allSanPham.Remove(selectSanPham);
            }         

            if (ModelState.IsValid)
            {
                db.CongThucs.Add(congThuc);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.IDSanPham = new SelectList(allSanPham, "IDSanPham", "TenSP", congThuc.IDSanPham);
            return View(congThuc);
        }

        // GET: Admin/CongThucs/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CongThuc congThuc = db.CongThucs.Find(id);
            if (congThuc == null)
            {
                return HttpNotFound();
            }
            ViewBag.IDSanPham = new SelectList(db.SanPhams, "IDSanPham", "TenSP", congThuc.IDSanPham);
            return View(congThuc);
        }

        // POST: Admin/CongThucs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IDCongThuc,IDSanPham,Noidung")] CongThuc congThuc)
        {
            if (ModelState.IsValid)
            {
                db.Entry(congThuc).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.IDSanPham = new SelectList(db.SanPhams, "IDSanPham", "TenSP", congThuc.IDSanPham);
            return View(congThuc);
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            var item = db.CongThucs.Find(id);
            if (item != null)
            {
                db.CongThucs.Remove(item);
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
