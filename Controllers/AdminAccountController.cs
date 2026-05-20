using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using TheGioiDiDong.Models;

namespace TheGioiDiDong.Controllers
{
    public class AdminAccountController : Controller
    {
        private QLSPEntities db = new QLSPEntities();

        // 1. Hiển thị danh sách toàn bộ người dùng
        public ActionResult Index()
        {
            var danhSachUser = db.NguoiDungs.OrderByDescending(u => u.MaNguoiDung).ToList();
            return View(danhSachUser);
        }

        // 2. Xem chi tiết thông tin một tài khoản
        public ActionResult Details(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            NguoiDung nguoiDung = db.NguoiDungs.Find(id);
            if (nguoiDung == null) return HttpNotFound();
            return View(nguoiDung);
        }

        // 3. Thêm mới tài khoản cấp cao hoặc tài khoản khách (GET)
        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        // Thêm mới tài khoản (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(NguoiDung nguoiDung)
        {
            if (ModelState.IsValid)
            {
                var check = db.NguoiDungs.FirstOrDefault(s => s.TenDangNhap == nguoiDung.TenDangNhap);
                if (check == null)
                {
                    nguoiDung.MatKhau = ComputeSHA256(nguoiDung.MatKhau);
                    nguoiDung.IsLocked = false; // Mặc định mở

                    db.NguoiDungs.Add(nguoiDung);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.error = "Tên đăng nhập này đã tồn tại trên hệ thống!";
                }
            }
            return View(nguoiDung);
        }

        // 4. Tính năng cốt lõi: Khóa / Mở khóa tài khoản nhanh bằng 1 click
        public ActionResult ToggleLock(int id)
        {
            NguoiDung user = db.NguoiDungs.Find(id);
            if (user != null)
            {
                // Tránh tình trạng Admin tự khóa chính mình gây lỗi hệ thống
                if (Session["TenDangNhap"] != null && user.TenDangNhap == Session["TenDangNhap"].ToString())
                {
                    TempData["Error"] = "Bạn không thể tự khóa tài khoản Admin đang đăng nhập của chính mình!";
                    return RedirectToAction("Index");
                }

                // Đảo ngược trạng thái: true thành false, false thành true
                user.IsLocked = !user.IsLocked;
                db.SaveChanges();
                TempData["Message"] = $"Cập nhật trạng thái tài khoản [{user.TenDangNhap}] thành công!";
            }
            return RedirectToAction("Index");
        }

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
