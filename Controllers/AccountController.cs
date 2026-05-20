using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using TheGioiDiDong.Models;

namespace TheGioiDiDong.Controllers
{
    public class AccountController : Controller
    {
        private QLSPEntities db = new QLSPEntities();

        // --- CHỨC NĂNG: ĐĂNG KÝ --- 
        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(NguoiDung user)
        {
            if (ModelState.IsValid)
            {
                // Kiểm tra xem tên đăng nhập đã được dùng chưa
                var check = db.NguoiDungs.FirstOrDefault(s => s.TenDangNhap == user.TenDangNhap);
                if (check == null)
                {
                    // Mã hóa mật khẩu theo chuẩn SHA256 tương thích với Database
                    user.MatKhau = ComputeSHA256(user.MatKhau);
                    user.QuyenHan = "User"; // Quyền mặc định cho khách hàng đăng ký online

                    db.NguoiDungs.Add(user);
                    db.SaveChanges();
                    return RedirectToAction("Login");
                }
                else
                {
                    ViewBag.error = "Tên đăng nhập này đã tồn tại!";
                    return View(user);
                }
            }
            return View(user);
        }

        // --- CHỨC NĂNG: ĐĂNG NHẬP --- 
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string tenDangNhap, string matKhau)
        {
            if (string.IsNullOrEmpty(tenDangNhap) || string.IsNullOrEmpty(matKhau))
            {
                ViewBag.error = "Vui lòng nhập đầy đủ thông tin!";
                return View();
            }

            string hashedPassword = ComputeSHA256(matKhau);
            var user = db.NguoiDungs.FirstOrDefault(s => s.TenDangNhap.Equals(tenDangNhap) && s.MatKhau.Equals(hashedPassword));

            if (user != null)
            {
                // CHỐT CHẶN KIỂM TRA: Nếu tài khoản đã bị khóa
                if (user.IsLocked == true)
                {
                    ViewBag.error = "Tài khoản của bạn đã bị khóa bởi quản trị viên!";
                    return View();
                }

                // Nếu không bị khóa thì lưu Session và chuyển trang bình thường
                Session["MaNguoiDung"] = user.MaNguoiDung;
                Session["TenDangNhap"] = user.TenDangNhap;
                Session["HoTen"] = user.HoTen;
                Session["QuyenHan"] = user.QuyenHan;

                if (user.QuyenHan.Equals("Admin", StringComparison.OrdinalIgnoreCase))
                {
                    return RedirectToAction("Index", "AdminHome");
                }
                return RedirectToAction("Index", "Product");
            }
            else
            {
                ViewBag.error = "Tài khoản hoặc mật khẩu không chính xác!";
                return View();
            }
        }

        // --- CHỨC NĂNG: ĐĂNG XUẤT ---
        public ActionResult Logout()
        {
            Session.Clear(); // Giải phóng toàn bộ dữ liệu Session
            return RedirectToAction("Index", "Product");
        }

        // Hàm mã hóa chuỗi sang mã băm SHA256 dạng Hex viết hoa/thường khớp cấu trúc SQL
        private string ComputeSHA256(string rawData)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
}