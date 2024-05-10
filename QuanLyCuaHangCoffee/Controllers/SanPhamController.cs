using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using QuanLyCuaHangCoffee.Models.EF;

namespace QuanLyCuaHangCoffee.Controllers
{
    public class SanPhamController : Controller
    {
        QLCHUOICOFFEEEntities db = new QLCHUOICOFFEEEntities();

        // GET: SanPham
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult ProductCategory(string alias, int? id, int? idCuaHang)
        {
            var items = db.SanPhams.ToList();
            if(id > 0)
            {
                items = items.Where(x => x.IDLoaiSP == id && x.TrangThai == true).ToList();
            }

            // Lấy danh sách cửa hàng từ cơ sở dữ liệu
            ViewBag.StoreList = db.CuaHangs.ToList();

            if (idCuaHang.HasValue)
            {
                // Lọc sản phẩm dựa trên cửa hàng được chọn
                items = items.Where(x => x.IDCuaHang == idCuaHang && x.IDLoaiSP == id && x.TrangThai == true).ToList();
                ViewBag.SelectedStoreId = idCuaHang;
            }

            return View(items);
        }
        public ActionResult Detail(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //SanPham sanPham = db.SanPhams.Find(id);          
            SanPham sanPham = db.SanPhams.FirstOrDefault(x => x.IDSanPham == id);
            List<SanPham> relatedProducts = db.SanPhams.Where(x => x.IDLoaiSP == sanPham.IDLoaiSP
                                                                && x.IDSanPham != sanPham.IDSanPham
                                                                && x.TrangThai == true)
                                                               .OrderBy(x=> Guid.NewGuid()) // sắp xếp ngẫu nhiên
                                                                .Take(6)
                                                                .ToList();
            ViewBag.RelatedSanPham = relatedProducts;
            if (sanPham == null)
            {
                return HttpNotFound();
            }
            return View(sanPham);
        }
      
        public ActionResult ItemsByCateId()
        {
            var items = db.SanPhams.Where(x => x.TrangThai == true).Take(4).ToList();
            return PartialView("_ItemsByCateId", items);
        }

       /* public ActionResult SelectStore(int IDCuaHang)
        {
            var items = db.SanPhams.Where(sp => sp.IDCuaHang == IDCuaHang).ToList();
            ViewBag.IDCuaHang = new SelectList(db.CuaHangs, "IDCuaHang", "DiaChiCH");
            return PartialView("_SelectStore", items);
            //return View(products);
        }*/
    }
}