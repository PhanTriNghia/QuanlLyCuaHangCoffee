using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using QuanLyCuaHangCoffee.Models.EF;

namespace QuanLyCuaHangCoffee.Models
{
    public class CartItem
    {
        public SanPham _sanpham { get; set; }
        public int _soluong { get; set; }
        public int _total { get; set; }
    }
    public class Cart
    {
        List<CartItem> items = new List<CartItem>();
        public IEnumerable<CartItem> Items
        {
            get { return items; }
        }
        public void Add(SanPham _product, int soluong = 1)
        {
            var item = items.FirstOrDefault(s => s._sanpham.IDSanPham == _product.IDSanPham);
            if (item == null)
            {
                items.Add(new CartItem
                {
                    _sanpham = _product,
                    _soluong = soluong,
                });
            }
            else
                item._soluong += soluong;
        }
        public void Remove(int? id)
        {
            items.RemoveAll(s => s._sanpham.IDSanPham == id);
        }
        public void Update(int id, int _soluong)
        {
            var item = items.SingleOrDefault(s => s._sanpham.IDSanPham == id);
            if (item != null)
            {
                item._soluong = _soluong;
            }
        }
        public int TotalMoney()
        {
            var total = items.Sum(s => s._sanpham.GiaBan * s._soluong);
            return total;
        }
        // tổng số lượng sản phẩm trong giỏ hàng
        public int TotalQuantitity()
        {
            return items.Sum(s => s._soluong);
        }
        public void ClearCart()
        {
            items.Clear();
        }
    }
}