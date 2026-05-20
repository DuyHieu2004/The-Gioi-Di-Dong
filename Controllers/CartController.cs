using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using System.Transactions;
using TheGioiDiDong.Models;

namespace TheGioiDiDong.Controllers
{
    public class CartController : Controller
    {
        private QLSPEntities db = new QLSPEntities();

        // Định nghĩa đối tượng chứa thông tin một dòng trong giỏ hàng
        public class CartItem
        {
            public SanPham SanPham { get; set; }
            public int SoLuong { get; set; }
            public decimal ThanhTien => (SanPham.Gia ?? 0) * SoLuong;
        }

        // Lấy hoặc khởi tạo giỏ hàng từ Session
        private List<CartItem> GetCartSession()
        {
            var cart = Session["Cart"] as List<CartItem>;
            if (cart == null)
            {
                cart = new List<CartItem>();
                Session["Cart"] = cart;
            }
            return cart;
        }

        // 1. Hiển thị giỏ hàng và tổng tiền 
        public ActionResult Index()
        {
            var cart = GetCartSession();
            decimal tongTienGoc = cart.Sum(item => item.ThanhTien);
            int phanTramGiam = Session["DiscountPercentage"] != null ? (int)Session["DiscountPercentage"] : 0;

            decimal soTienGiam = tongTienGoc * phanTramGiam / 100;
            decimal tongTienSauGiam = tongTienGoc - soTienGiam;

            ViewBag.TongTienGoc = tongTienGoc;
            ViewBag.PhanTramGiam = phanTramGiam;
            ViewBag.SoTienGiam = soTienGiam;
            ViewBag.TongTienSauGiam = tongTienSauGiam;

            return View(cart);
        }

        // 2. Thêm sản phẩm vào giỏ hàng 
        public ActionResult AddToCart(int id, int quantity = 1)
        {
            var product = db.SanPhams.Find(id);
            if (product == null) return HttpNotFound();

            var cart = GetCartSession();
            var cartItem = cart.FirstOrDefault(p => p.SanPham.MaSanPham == id);

            if (cartItem == null)
            {
                cart.Add(new CartItem { SanPham = product, SoLuong = quantity });
            }
            else
            {
                cartItem.SoLuong += quantity; // Cập nhật số lượng nếu đã tồn tại 
            }
            return RedirectToAction("Index");
        }

        // 3. Cập nhật số lượng / Cập nhật giỏ hàng 
        [HttpPost]
        public ActionResult UpdateQuantity(int id, int quantity)
        {
            var cart = GetCartSession();
            var cartItem = cart.FirstOrDefault(p => p.SanPham.MaSanPham == id);

            if (cartItem != null)
            {
                if (quantity <= 0)
                {
                    cart.Remove(cartItem);
                }
                else
                {
                    cartItem.SoLuong = quantity;
                }
            }
            return RedirectToAction("Index");
        }

        // Xóa sản phẩm khỏi giỏ
        public ActionResult RemoveItem(int id)
        {
            var cart = GetCartSession();
            var cartItem = cart.FirstOrDefault(p => p.SanPham.MaSanPham == id);
            if (cartItem != null) cart.Remove(cartItem);
            return RedirectToAction("Index");
        }

        // 4. Xác nhận thông tin đặt hàng (Checkout) 
        [HttpPost]
        public ActionResult Checkout()
        {
            // Kiểm tra xem khách hàng đã đăng nhập tài khoản chưa 
            if (Session["MaNguoiDung"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var cart = GetCartSession();
            if (!cart.Any())
            {
                TempData["Message"] = "Giỏ hàng trống, không thể đặt hàng!";
                return RedirectToAction("Index");
            }

            // Fallback to TransactionScope to support EF versions that may not expose Database.BeginTransaction()
            using (var scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                try
                {
                    // Thêm mới một bản ghi đơn hàng vào bảng DonHang
                    DonHang order = new DonHang
                    {
                        MaNguoiDung = (int)Session["MaNguoiDung"],
                        NgayDat = DateTime.Now,
                        TongTien = cart.Sum(item => item.ThanhTien),
                        MaTrangThai = 1 // Trạng thái mặc định: 1 - Chờ xác nhận
                    };

                    db.DonHangs.Add(order);
                    db.SaveChanges(); // Lưu để phát sinh IDENTITY MaDonHang tự động

                    // Duyệt qua từng item trong giỏ để lưu vào bảng chi tiết và trừ kho
                    foreach (var item in cart)
                    {
                        ChiTietDH orderDetail = new ChiTietDH
                        {
                            MaDonHang = order.MaDonHang,
                            MaSanPham = item.SanPham.MaSanPham,
                            SoLuong = item.SoLuong,
                            DonGia = item.SanPham.Gia ?? 0
                        };
                        db.ChiTietDHs.Add(orderDetail);

                        // Logic bổ sung: Khấu trừ trực tiếp số lượng sản phẩm trong kho tồn
                        var productInDb = db.SanPhams.Find(item.SanPham.MaSanPham);
                        if (productInDb != null)
                        {
                            if (productInDb.SoLuong >= item.SoLuong)
                            {
                                productInDb.SoLuong -= item.SoLuong;
                            }
                            else
                            {
                                throw new Exception($"Sản phẩm {productInDb.TenSanPham} không đủ số lượng trong kho.");
                            }
                        }
                    }

                    db.SaveChanges();

                    // Complete the ambient transaction so all DB operations commit
                    scope.Complete();

                    // Làm sạch giỏ hàng sau khi đặt thành công
                    Session["Cart"] = null;

                    return View("OrderSuccess");
                }
                catch (Exception ex)
                {
                    // TransactionScope will auto-rollback if Complete() was not called
                    TempData["Error"] = "Quá trình đặt hàng thất bại: " + ex.Message;
                    return RedirectToAction("Index");
                }
            }
        }

        // Thêm Action xử lý áp dụng mã khuyến mãi
        [HttpPost]
        public ActionResult ApplyPromo(string promoCode)
        {
            // Kiểm tra mã trong bảng KhuyenMai và thời hạn sử dụng
            var km = db.KhuyenMais.FirstOrDefault(k => k.MaGiamGia == promoCode);
            DateTime today = DateTime.Today;

            if (km != null && (km.NgayBatDau == null || km.NgayBatDau <= today) && (km.NgayKetThuc == null || km.NgayKetThuc >= today))
            {
                // Lưu tỉ lệ phần trăm giảm và tên mã vào Session
                Session["DiscountPercentage"] = km.PhanTramGiam ?? 0;
                Session["PromoCode"] = km.MaGiamGia;
                TempData["Message"] = $"Áp dụng mã {km.MaGiamGia} thành công! Bạn được giảm {km.PhanTramGiam}%.";
            }
            else
            {
                TempData["Error"] = "Mã khuyến mãi không hợp lệ, viết sai ký tự hoặc đã hết hạn sử dụng.";
            }
            return RedirectToAction("Index");
        }
    }
}