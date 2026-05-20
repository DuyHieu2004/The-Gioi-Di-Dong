using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using TheGioiDiDong.Models;

namespace TheGioiDiDong.Controllers
{
    public class AdminKhuyenMaiController : Controller
    {
        private QLSPEntities db = new QLSPEntities();

        // Trang chủ quản lý: Hiển thị danh sách các chương trình khuyến mãi
        public ActionResult Index()
        {
            var khuyenMais = db.KhuyenMais.Include(k => k.DanhMucSP).Include(k => k.SanPham);
            return View(khuyenMais.OrderByDescending(k => k.MaKM).ToList());
        }

        // Tạo mới mã giảm giá (Giao diện nhận diện)
        [HttpGet]
        public ActionResult Create()
        {
            ViewBag.MaDanhMuc = new SelectList(db.DanhMucSPs, "MaDanhMuc", "TenDanhMuc");
            ViewBag.MaSanPham = new SelectList(db.SanPhams, "MaSanPham", "TenSanPham");
            return View();
        }

        // Tạo mới mã giảm giá (Xử lý lưu dữ liệu)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(KhuyenMai khuyenMai)
        {
            if (ModelState.IsValid)
            {
                // Mặc định thiết lập loại khuyến mãi thông thường
                khuyenMai.LoaiKhuyenMai = khuyenMai.LoaiKhuyenMai ?? 0;
                db.KhuyenMais.Add(khuyenMai);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.MaDanhMuc = new SelectList(db.DanhMucSPs, "MaDanhMuc", "TenDanhMuc", khuyenMai.MaDanhMuc);
            ViewBag.MaSanPham = new SelectList(db.SanPhams, "MaSanPham", "TenSanPham", khuyenMai.MaSanPham);
            return View(khuyenMai);
        }

        // Xóa mã giảm giá
        public ActionResult Delete(int id)
        {
            KhuyenMai km = db.KhuyenMais.Find(id);
            if (km != null)
            {
                db.KhuyenMais.Remove(km);
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }
    }
}