using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using TheGioiDiDong.Models;

namespace TheGioiDiDong.Controllers
{
    public class AdminProductController : Controller
    {
        private QLSPEntities db = new QLSPEntities();

        // Hiển thị danh sách toàn bộ sản phẩm CÓ HỖ TRỢ TÌM KIẾM
        public ActionResult Index(string searchString)
        {
            var sanPhams = db.SanPhams.Include(s => s.DanhMucSP).Include(s => s.TrangThaiSanPham).AsQueryable();

            // Nếu người dùng có gõ chữ vào ô tìm kiếm thì mới lọc
            if (!string.IsNullOrEmpty(searchString))
            {
                sanPhams = sanPhams.Where(s => s.TenSanPham.Contains(searchString));
            }

            // Giữ lại từ khóa vừa tìm để hiển thị lại trên ô input cho khỏi bị mất chữ
            ViewBag.CurrentSearch = searchString;

            return View(sanPhams.OrderByDescending(s => s.MaSanPham).ToList());
        }

        // --- THÊM SẢN PHẨM ---
        [HttpGet]
        public ActionResult Create()
        {
            // Đổ dữ liệu ra Dropdown list cho Danh mục và Trạng thái
            ViewBag.MaDanhMuc = new SelectList(db.DanhMucSPs, "MaDanhMuc", "TenDanhMuc");
            ViewBag.MaTrangThai = new SelectList(db.TrangThaiSanPhams, "MaTrangThai", "TenTrangThai");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(SanPham sanPham)
        {
            if (ModelState.IsValid)
            {
                db.SanPhams.Add(sanPham);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.MaDanhMuc = new SelectList(db.DanhMucSPs, "MaDanhMuc", "TenDanhMuc", sanPham.MaDanhMuc);
            ViewBag.MaTrangThai = new SelectList(db.TrangThaiSanPhams, "MaTrangThai", "TenTrangThai", sanPham.MaTrangThai);
            return View(sanPham);
        }

        // --- SỬA SẢN PHẨM ---
        [HttpGet]
        public ActionResult Edit(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            SanPham sanPham = db.SanPhams.Find(id);
            if (sanPham == null) return HttpNotFound();

            ViewBag.MaDanhMuc = new SelectList(db.DanhMucSPs, "MaDanhMuc", "TenDanhMuc", sanPham.MaDanhMuc);
            ViewBag.MaTrangThai = new SelectList(db.TrangThaiSanPhams, "MaTrangThai", "TenTrangThai", sanPham.MaTrangThai);
            return View(sanPham);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(SanPham sanPham)
        {
            if (ModelState.IsValid)
            {
                db.Entry(sanPham).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.MaDanhMuc = new SelectList(db.DanhMucSPs, "MaDanhMuc", "TenDanhMuc", sanPham.MaDanhMuc);
            ViewBag.MaTrangThai = new SelectList(db.TrangThaiSanPhams, "MaTrangThai", "TenTrangThai", sanPham.MaTrangThai);
            return View(sanPham);
        }

        // --- XÓA MỀM: BẬT/TẮT HIỂN THỊ SẢN PHẨM ---
        public ActionResult ToggleShow(int id)
        {
            SanPham sanPham = db.SanPhams.Find(id);
            if (sanPham != null)
            {
                // Đảo ngược trạng thái hiển thị
                sanPham.IsShow = !sanPham.IsShow;
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }

    }
}