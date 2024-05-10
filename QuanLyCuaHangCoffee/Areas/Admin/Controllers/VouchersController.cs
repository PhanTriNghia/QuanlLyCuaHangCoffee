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
    [Authorize(Roles = "Admin, NhanVien")]
    public class VouchersController : Controller
    {
        private QLCHUOICOFFEEEntities db = new QLCHUOICOFFEEEntities();

        // GET: Admin/Vouchers
        public ActionResult Index(int? page)
        {
            var pageSize = 10;
            if (page == null)
            {
                page = 1;
            }
            var pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            var vouchers = db.Vouchers.OrderByDescending(s => s.IDCoupon).ToPagedList(pageIndex, pageSize);
            return View(vouchers);        
        }

        // GET: Admin/Vouchers/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/Vouchers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IDCoupon,TenVoucher,MoTa,TienGiam,NgayBatDau,NgayKetThuc,GioiHan,TrangThai")] Voucher voucher)
        {
            if (ModelState.IsValid)
            {
                DateTime selectedStartDate = voucher.NgayBatDau;
                DateTime selectedEndDate = voucher.NgayKetThuc;
                if(selectedStartDate <= selectedEndDate)
                {
                    voucher.NgayBatDau = (voucher.NgayBatDau).Date;
                    voucher.NgayKetThuc = (voucher.NgayKetThuc).Date;
                    if(voucher.NgayKetThuc > DateTime.Now)
                    {
                        voucher.TrangThai = true;
                    }
                    else
                    {
                        voucher.TrangThai = false;
                    }
                    db.Vouchers.Add(voucher);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("NgayBatDau", "Ngày bắt đầu phải trước hoặc bằng ngày kết thúc");
                }
            }
            return View(voucher);
        }

        // GET: Admin/Vouchers/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Voucher voucher = db.Vouchers.Find(id);
            if (voucher == null)
            {
                return HttpNotFound();
            }
            return View(voucher);
        }

        // POST: Admin/Vouchers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IDCoupon,TenVoucher,MoTa,TienGiam,NgayBatDau,NgayKetThuc,GioiHan,TrangThai")] Voucher voucher)
        {
            if (ModelState.IsValid)
            {
                db.Entry(voucher).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(voucher);
        }
        [HttpPost]
        public ActionResult Delete(int id)
        {
            var item = db.Vouchers.Find(id);
            if (item != null)
            {
                db.Vouchers.Remove(item);
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
