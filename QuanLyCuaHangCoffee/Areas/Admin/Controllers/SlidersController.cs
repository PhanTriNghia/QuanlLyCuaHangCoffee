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
    [Authorize(Roles = "Admin,NhanVien")]
    public class SlidersController : Controller
    {
        private QLCHUOICOFFEEEntities db = new QLCHUOICOFFEEEntities();

        // GET: Admin/Sliders
        public ActionResult Index(int? page)
        {
            var pageSize = 10;
            if (page == null)
            {
                page = 1;
            }
            var pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            var sliders = db.Sliders.OrderByDescending(s => s.IDSlider).ToPagedList(pageIndex, pageSize);
            return View(sliders);
        }

        // GET: Admin/Sliders/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/Sliders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IDSlider,HinhAnhSlider,TrangThai,NoiDungSlider")] Slider slider)
        {
            if (ModelState.IsValid)
            {
                db.Sliders.Add(slider);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(slider);
        }

        // GET: Admin/Sliders/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Slider slider = db.Sliders.Find(id);
            if (slider == null)
            {
                return HttpNotFound();
            }
            return View(slider);
        }

        // POST: Admin/Sliders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IDSlider,HinhAnhSlider,TrangThai,NoiDungSlider")] Slider slider)
        {
            if (ModelState.IsValid)
            {
                db.Entry(slider).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(slider);
        }
        public ActionResult IsActive(int id)
        {
            var item = db.Sliders.Find(id);
            if (item != null)
            {
                item.TrangThai = !item.TrangThai;
                db.Entry(item).State = EntityState.Modified;
                db.SaveChanges();
                return Json(new { success = true, isActive = item.TrangThai });
            }

            return Json(new { success = false });
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            var item = db.Sliders.Find(id);
            if (item != null)
            {
                db.Sliders.Remove(item);
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
