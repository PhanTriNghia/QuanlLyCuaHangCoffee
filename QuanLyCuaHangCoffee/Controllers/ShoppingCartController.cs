using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using QuanLyCuaHangCoffee.Models;
using QuanLyCuaHangCoffee.Models.EF;
using QuanLyCuaHangCoffee.Commom;
using System.Configuration;
using Microsoft.AspNet.Identity.Owin;

namespace QuanLyCuaHangCoffee.Controllers
{
    public class ShoppingCartController : Controller
    {
        QLCHUOICOFFEEEntities db = new QLCHUOICOFFEEEntities();

        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public ShoppingCartController()
        {
        }

        public ShoppingCartController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }
        public Cart GetCart()
        {
            Cart cart = Session["Cart"] as Cart;
            if (cart == null || Session["Cart"] == null)
            {
                cart = new Cart();
                Session["Cart"] = cart;
            }
            return cart;
        }
        // GET: ShoppingCart
        public ActionResult Index()
        {
            if (Session["Cart"] == null)
            {
                return View();
            }
            Cart cart = Session["Cart"] as Cart;
            return View(cart);
        }
        public ActionResult CheckOut()
        {
            Cart cart = (Cart)Session["Cart"];
            if (cart != null && cart.Items.Any())
            {
                ViewBag.CheckCart = cart;
            }
            return View();
        }
        public ActionResult AddToCart(int? id)
        {
            var sanpham = db.SanPhams.SingleOrDefault(s => s.IDSanPham == id);
            if (sanpham != null)
            {
                GetCart().Add(sanpham);
            }
            return RedirectToAction("Index", "ShoppingCart");
        }
        public ActionResult RemoveCart(int? id)
        {
            Cart cart = Session["Cart"] as Cart;
            cart.Remove(id);
            return RedirectToAction("Index", "ShoppingCart");
        }
        public ActionResult Update(FormCollection form /*int id, int soluong*/ )
        {
            Cart cart = (Cart)Session["Cart"];
            int idsanpham = int.Parse(form["ID_Sanpham"]);
            int soluong = int.Parse(form["Soluong"]);
            cart.Update(idsanpham, soluong);
            return RedirectToAction("Index", "ShoppingCart");

        }

        public ActionResult Shopping_Success()
        {
            return View(); 
        }

        public ActionResult Partial_CheckOut()
        {
            var user = UserManager.FindByNameAsync(User.Identity.Name).Result;
            if(user != null)
            {
                ViewBag.User = user;
            }
            return PartialView();
        }
        public ActionResult Partial_Items_ThanhToan()
        {
            Cart cart = (Cart)Session["Cart"];
            if (cart != null && cart.Items.Any())
            {
                return PartialView(cart.Items);
            }
            return PartialView();
        }
        

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CheckOut(OderViewModel req, string payment_method)
        {
            if (payment_method.Equals("COD"))
            {
                Cart cart = (Cart)Session["Cart"];
                if (cart != null)
                {
                    DonHang donHang = new DonHang();
                    donHang.TenKH = req.CustomerName;
                    donHang.Username = User.Identity.Name;
                    donHang.SDTKH = req.Phone;
                    donHang.DiaChiKH = req.Address;
                    donHang.Email = req.Email;
                    donHang.TongTienBill = cart.Items.Sum(x => (x._sanpham.GiaBan * x._soluong));
                    donHang.PTTT = "COD";
                    donHang.NgayDat = DateTime.Now;
                    db.DonHangs.Add(donHang);
                    foreach (var item in cart.Items)
                    {
                        ChiTietDonHang _ctdh = new ChiTietDonHang();
                        _ctdh.IDDonHang = donHang.IDDonHang;
                        _ctdh.IDSanPham = item._sanpham.IDSanPham;
                        _ctdh.TongTienSP = item._sanpham.GiaBan;
                        _ctdh.SoLuongSP = item._soluong;
                        db.ChiTietDonHangs.Add(_ctdh);

                        // lưu lại lịch sử mua hàng
                        AddPurchaseHistory(_ctdh.IDSanPham, _ctdh.SoLuongSP, _ctdh.TongTienSP);
                    }

                    db.SaveChanges();



                    var strSanpham = "";
                    var tongtien = 0;
                    foreach (var sp in cart.Items)
                    {
                        tongtien += sp._sanpham.GiaBan * sp._soluong;
                        strSanpham += "<tr>";
                        strSanpham += "<td>" + sp._sanpham.TenSP + "</td>";
                        strSanpham += "<td>" + sp._soluong + "</td>";
                        strSanpham += "<td>" + tongtien + "</td>";
                        strSanpham += "</tr>";

                    }
                    string contentCustomer = System.IO.File.ReadAllText(Server.MapPath("~/Content/templates/send2.html"));
                    contentCustomer = contentCustomer.Replace("{{Sanpham}}", strSanpham);
                    contentCustomer = contentCustomer.Replace("{{MaDH}}", donHang.IDDonHang.ToString());
                    contentCustomer = contentCustomer.Replace("{{TenKH}}", donHang.TenKH);
                    contentCustomer = contentCustomer.Replace("{{ThanhTien}}", tongtien.ToString());
                    contentCustomer = contentCustomer.Replace("{{Email}}", donHang.Email);
                    contentCustomer = contentCustomer.Replace("{{PhuongThucTT}}", donHang.PTTT);
                    //contentCustomer = contentCustomer.Replace("{{NgayDat}}", order.NgayTao.ToString("MM-dd-yyyy HH:mm:ss"));
                    contentCustomer = contentCustomer.Replace("{{SDT}}", donHang.SDTKH);
                    contentCustomer = contentCustomer.Replace("{{Diachi}}", donHang.DiaChiKH);
                    QuanLyCuaHangCoffee.Commom.Common.SenEmail("CoffeeShop", "Đơn hàng", contentCustomer.ToString(), donHang.Email);

                    string contentAdmin = System.IO.File.ReadAllText(Server.MapPath("~/Content/templates/send1.html"));
                    contentAdmin = contentAdmin.Replace("{{Sanpham}}", strSanpham);
                    contentAdmin = contentAdmin.Replace("{{Sanpham}}", strSanpham);
                    contentAdmin = contentAdmin.Replace("{{MaDH}}", donHang.IDDonHang.ToString());
                    contentAdmin = contentAdmin.Replace("{{TenKH}}", donHang.TenKH);
                    contentAdmin = contentAdmin.Replace("{{ThanhTien}}", tongtien.ToString());
                    contentAdmin = contentAdmin.Replace("{{Email}}", donHang.Email);
                    contentAdmin = contentAdmin.Replace("{{PhuongThucTT}}", donHang.PTTT);
                    //contentAdmin = contentAdmin.Replace("{{NgayDat}}", order.NgayTao.ToString("MM-dd-yyyy HH:mm:ss"));
                    contentAdmin = contentAdmin.Replace("{{SDT}}", donHang.SDTKH);
                    contentAdmin = contentAdmin.Replace("{{Diachi}}", donHang.DiaChiKH);
                    QuanLyCuaHangCoffee.Commom.Common.SenEmail("CoffeeShop", "Đơn hàng mới", contentAdmin.ToString(), ConfigurationSettings.AppSettings["Email"]);


                    cart.ClearCart();
                }
                return RedirectToAction("Shopping_Success", "ShoppingCart");
            }
            else if (payment_method.Equals("VnPay"))
            {
                Random rand = new Random((int)DateTime.Now.Ticks);
                int numIterations = 0;
                numIterations = rand.Next(1, 100000);
                DateTime time = DateTime.Now;

                Cart cart = (Cart)Session["Cart"];
                if (cart != null)
                {
                    DonHang donHang = new DonHang();
                    donHang.TenKH = req.CustomerName;
                    donHang.Username = User.Identity.Name;
                    donHang.SDTKH = req.Phone;
                    donHang.DiaChiKH = req.Address;
                    donHang.Email = req.Email;
                    donHang.TongTienBill = cart.Items.Sum(x => (x._sanpham.GiaBan * x._soluong));
                    donHang.PTTT = "VnPay";
                    donHang.NgayDat = DateTime.Now;
                    db.DonHangs.Add(donHang);
                    foreach (var item in cart.Items)
                    {
                        ChiTietDonHang _ctdh = new ChiTietDonHang();
                        _ctdh.IDDonHang = donHang.IDDonHang;
                        _ctdh.IDSanPham = item._sanpham.IDSanPham;
                        _ctdh.TongTienSP = item._sanpham.GiaBan;
                        _ctdh.SoLuongSP = item._soluong;
                        db.ChiTietDonHangs.Add(_ctdh);
                    }
                    db.SaveChanges();

                    var urlPayment = "";
                    //var cartItemProduct = (List<CartItem>)Session["Cart"];

                    //Get Config Info
                    string vnp_Returnurl = ConfigurationManager.AppSettings["vnp_Returnurl"]; //URL nhan ket qua tra ve 
                    string vnp_Url = ConfigurationManager.AppSettings["vnp_Url"]; //URL thanh toan cua VNPAY 
                    string vnp_TmnCode = ConfigurationManager.AppSettings["vnp_TmnCode"]; //Ma định danh merchant kết nối (Terminal Id)
                    string vnp_HashSecret = ConfigurationManager.AppSettings["vnp_HashSecret"]; //Secret Key

                    //Build URL for VNPAY
                    Models.VnPayLibrary vnpay = new VnPayLibrary();
                    vnpay.AddRequestData("vnp_Version", VnPayLibrary.VERSION);
                    vnpay.AddRequestData("vnp_Command", "pay");
                    vnpay.AddRequestData("vnp_TmnCode", vnp_TmnCode);
                    vnpay.AddRequestData("vnp_Amount", (donHang.TongTienBill * 100).ToString());

                    vnpay.AddRequestData("vnp_CreateDate", time.ToString("yyyyMMddHHmmss"));
                    vnpay.AddRequestData("vnp_CurrCode", "VND");
                    vnpay.AddRequestData("vnp_IpAddr", Utils.GetIpAddress());
                    vnpay.AddRequestData("vnp_Locale", "vn");
                    vnpay.AddRequestData("vnp_OrderInfo", "Thanh toán đơn hàng:" + numIterations);
                    vnpay.AddRequestData("vnp_OrderType", "other"); //default value: other

                    vnpay.AddRequestData("vnp_ReturnUrl", vnp_Returnurl);
                    vnpay.AddRequestData("vnp_TxnRef", numIterations.ToString());

                    var strSanpham = "";
                    var tongtien = 0;
                    foreach (var sp in cart.Items)
                    {
                        tongtien += sp._sanpham.GiaBan * sp._soluong;
                        strSanpham += "<tr>";
                        strSanpham += "<td>" + sp._sanpham.TenSP + "</td>";
                        strSanpham += "<td>" + sp._soluong + "</td>";
                        strSanpham += "<td>" + tongtien + "</td>";
                        strSanpham += "</tr>";

                    }

                    string contentCustomer = System.IO.File.ReadAllText(Server.MapPath("~/Content/templates/send2.html"));
                    contentCustomer = contentCustomer.Replace("{{Sanpham}}", strSanpham);
                    contentCustomer = contentCustomer.Replace("{{MaDH}}", donHang.IDDonHang.ToString());
                    contentCustomer = contentCustomer.Replace("{{TenKH}}", donHang.TenKH);
                    contentCustomer = contentCustomer.Replace("{{ThanhTien}}", tongtien.ToString());
                    contentCustomer = contentCustomer.Replace("{{Email}}", donHang.Email);
                    contentCustomer = contentCustomer.Replace("{{PhuongThucTT}}", donHang.PTTT);
                    //contentCustomer = contentCustomer.Replace("{{NgayDat}}", order.NgayTao.ToString("MM-dd-yyyy HH:mm:ss"));
                    contentCustomer = contentCustomer.Replace("{{SDT}}", donHang.SDTKH);
                    contentCustomer = contentCustomer.Replace("{{Diachi}}", donHang.DiaChiKH);
                    QuanLyCuaHangCoffee.Commom.Common.SenEmail("CoffeeShop", "Đơn hàng", contentCustomer.ToString(), donHang.Email);

                    string contentAdmin = System.IO.File.ReadAllText(Server.MapPath("~/Content/templates/send1.html"));
                    contentAdmin = contentAdmin.Replace("{{Sanpham}}", strSanpham);
                    contentAdmin = contentAdmin.Replace("{{Sanpham}}", strSanpham);
                    contentAdmin = contentAdmin.Replace("{{MaDH}}", donHang.IDDonHang.ToString());
                    contentAdmin = contentAdmin.Replace("{{TenKH}}", donHang.TenKH);
                    contentAdmin = contentAdmin.Replace("{{ThanhTien}}", tongtien.ToString());
                    contentAdmin = contentAdmin.Replace("{{Email}}", donHang.Email);
                    contentAdmin = contentAdmin.Replace("{{PhuongThucTT}}", donHang.PTTT);
                    //contentAdmin = contentAdmin.Replace("{{NgayDat}}", order.NgayTao.ToString("MM-dd-yyyy HH:mm:ss"));
                    contentAdmin = contentAdmin.Replace("{{SDT}}", donHang.SDTKH);
                    contentAdmin = contentAdmin.Replace("{{Diachi}}", donHang.DiaChiKH);
                    QuanLyCuaHangCoffee.Commom.Common.SenEmail("CoffeeShop", "Đơn hàng mới", contentAdmin.ToString(), ConfigurationSettings.AppSettings["Email"]);

                    cart.ClearCart();
                    urlPayment = vnpay.CreateRequestUrl(vnp_Url, vnp_HashSecret);
                    return Redirect(urlPayment);
                }
            }
            return View();
        }

        public ActionResult VnPayReturn()
        {
            if (Request.QueryString.Count > 0)
            {
                string vnp_HashSecret = ConfigurationManager.AppSettings["vnp_HashSecret"]; //Chuoi bi mat
                var vnpayData = Request.QueryString;
                VnPayLibrary vnpay = new VnPayLibrary();

                foreach (string s in vnpayData)
                {
                    //get all querystring data
                    if (!string.IsNullOrEmpty(s) && s.StartsWith("vnp_"))
                    {
                        vnpay.AddResponseData(s, vnpayData[s]);
                    }
                }
                string orderCode = Convert.ToString(vnpay.GetResponseData("vnp_TxnRef"));
                long vnpayTranId = Convert.ToInt64(vnpay.GetResponseData("vnp_TransactionNo"));
                string vnp_ResponseCode = vnpay.GetResponseData("vnp_ResponseCode");
                string vnp_TransactionStatus = vnpay.GetResponseData("vnp_TransactionStatus");
                String vnp_SecureHash = Request.QueryString["vnp_SecureHash"];
                String TerminalID = Request.QueryString["vnp_TmnCode"];
                long vnp_Amount = Convert.ToInt64(vnpay.GetResponseData("vnp_Amount")) / 100;
                String bankCode = Request.QueryString["vnp_BankCode"];

                bool checkSignature = vnpay.ValidateSignature(vnp_SecureHash, vnp_HashSecret);
                if (checkSignature)
                {
                    if (vnp_ResponseCode == "00" && vnp_TransactionStatus == "00")
                    {
                        //Thanh toan thanh cong
                        ViewBag.InnerText = "Giao dịch được thực hiện thành công. Cảm ơn quý khách đã sử dụng dịch vụ";
                        //log.InfoFormat("Thanh toan thanh cong, OrderId={0}, VNPAY TranId={1}", orderId, vnpayTranId);
                    }
                    else
                    {
                        //Thanh toan khong thanh cong. Ma loi: vnp_ResponseCode
                        ViewBag.InnerText = "Có lỗi xảy ra trong quá trình xử lý.Mã lỗi: " + vnp_ResponseCode;
                        //log.InfoFormat("Thanh toan loi, OrderId={0}, VNPAY TranId={1},ResponseCode={2}", orderId, vnpayTranId, vnp_ResponseCode);
                    }
                    /* displayTmnCode.InnerText = "Mã Website (Terminal ID):" + TerminalID;
                     displayTxnRef.InnerText = "Mã giao dịch thanh toán:" + orderId.ToString();
                     displayVnpayTranNo.InnerText = "Mã giao dịch tại VNPAY:" + vnpayTranId.ToString();

                     displayBankCode.InnerText = "Ngân hàng thanh toán:" + bankCode;*/
                    ViewBag.ThanhToan = "Số tiền thanh toán (VND):" + vnp_Amount.ToString();
                }
            }
            return View();
        }

        public void AddPurchaseHistory(int idProduct, int quantity, int totalPrice)
        {
            var purchaseHistory = new PurchaseHisotry
            {
                Username = User.Identity.Name,
                IDSanPham = idProduct,
                NgayMua = DateTime.Now,
                SoLuong = quantity,
                TongTien = totalPrice,
            };
            
            db.PurchaseHisotries.Add(purchaseHistory);
            db.SaveChanges();
        }

        public ActionResult OrderUser()
        {
            var orderUser = db.ChiTietDonHangs
                .Where(x => x.DonHang.Username == User.Identity.Name)
                .ToList();
            return View(orderUser);
        }

        [HttpPost]
        public ActionResult DeleteOrderUser(int id)
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

        

      /*  public bool ChekCoupon(string maVoucher, int DonHangID)
        {
           
        }*/
    }
}