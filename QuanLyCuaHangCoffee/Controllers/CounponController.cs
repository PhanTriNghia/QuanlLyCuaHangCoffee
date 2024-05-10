using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using QuanLyCuaHangCoffee.Models.EF;

namespace QuanLyCuaHangCoffee.Controllers
{

    public class CounponController : Controller
    {
        QLCHUOICOFFEEEntities db = new QLCHUOICOFFEEEntities();
        // GET: Counpon
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ApplyVoucher(string discountCode, int DonHangID)
        {
            var voucher = db.Vouchers.FirstOrDefault(x => x.TenVoucher == discountCode && x.TrangThai == true
                                                   && x.NgayBatDau <= DateTime.Now && x.NgayKetThuc >= DateTime.Now
                                                   && x.GioiHan > 0);
            if (voucher != null)
            {
                var order = db.CouponUsers.FirstOrDefault(x => x.IDDonHang == DonHangID);

                if (order != null && order.NgayDung == null)
                {
                    // order. = maVoucher;
                    order.NgayDung = DateTime.Now;
                    order.DonHang.TongTienBill -= voucher.TienGiam;

                    db.Entry(order).Reference(o => o.DonHang).Load();

                    db.SaveChanges();

                    Json(new { success = true, message = "Áp dụng mã giảm giá thành công!" });
                }
                else
                {
                    return Json(new { success = false, message = "Mã giảm giá không hợp lệ!" });
                }

            }
            return View();
        }
        /*[HttpPost]
        public ActionResult ApplyDiscount(string discountCode)
        {
            var coupon = db.Vouchers.FirstOrDefault(c => c.TenVoucher == discountCode && c.TrangThai);

            if (coupon != null)
            {
                var discountAmount = coupon.TienGiam;
                // Áp dụng giảm giá theo logic của bạn
                // Ví dụ: tongtien -= discountAmount;

                // Cập nhật giảm giá trong session hoặc database (tùy thuộc vào yêu cầu của bạn)

                return Json(new { success = true, message = "Áp dụng mã giảm giá thành công!" });
            }
            else
            {
                return Json(new { success = false, message = "Mã giảm giá không hợp lệ!" });
            }
        }*/
    }
}