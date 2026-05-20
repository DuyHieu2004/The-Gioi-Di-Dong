using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using TheGioiDiDong.Models;

namespace TheGioiDiDong.Controllers
{
    public class ProductController : Controller
    {
        // Khởi tạo DbContext từ Entity Framework (tên khớp với chuỗi kết nối trong Web.config)
        private QLSPEntities db = new QLSPEntities();

       // 1. Xem danh sách sản phẩm, Tìm kiếm sản phẩm, Lọc sản phẩm theo danh mục 
        public ActionResult Index(int? categoryId, string searchString)
        {
            // Lấy danh sách sản phẩm đang ở trạng thái kinh doanh (MaTrangThai = 1: Còn hàng, v.v...)
            IQueryable<TheGioiDiDong.Models.SanPham> products = db.SanPhams
                 .Include(p => p.DanhMucSP)
                 .Include(p => p.TrangThaiSanPham)
                 .Where(p => p.IsShow == true); // Lọc bỏ hàng đã bị xóa mềm

            // Chức năng: Lọc sản phẩm theo danh mục 
            if (categoryId.HasValue)
            {
                products = products.Where(p => p.MaDanhMuc == categoryId.Value);
            }

            // Chức năng: Tìm kiếm sản phẩm theo tên 
            if (!string.IsNullOrEmpty(searchString))
            {
                products = products.Where(p => p.TenSanPham.Contains(searchString));
            }

            // Gửi danh mục sang View để hiển thị thanh Menu hoặc Sidebar lọc
            ViewBag.Categories = db.DanhMucSPs.ToList();
            ViewBag.CurrentSearch = searchString;
            ViewBag.CurrentCategory = categoryId;

            return View(products.ToList());
        }

       // 2. Xem chi tiết sản phẩm 
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // Tìm sản phẩm theo mã định danh khóa chính
            SanPham product = db.SanPhams.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }

            // Lấy thêm danh sách bình luận và đánh giá của sản phẩm này
            ViewBag.BinhLuan = db.BinhLuans.Where(b => b.MaSanPham == id).OrderByDescending(b => b.NgayBinhLuan).ToList();
            ViewBag.DanhGiaTtrungBinh = db.DanhGias.Where(d => d.MaSanPham == id).Select(d => (double?)d.SoSao).Average() ?? 5.0;

            return View(product);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SubmitReview(int maSanPham, string noiDung, int? soSao)
        {
            // Kiểm tra đăng nhập, nếu chưa đăng nhập thì bắt buộc đi tới trang Login
            if (Session["MaNguoiDung"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            int maNguoiDung = (int)Session["MaNguoiDung"];

            // 1. Xử lý lưu Bình luận (nếu người dùng có nhập nội dung)
            if (!string.IsNullOrEmpty(noiDung))
            {
                BinhLuan bl = new BinhLuan
                {
                    MaSanPham = maSanPham,
                    MaNguoiDung = maNguoiDung,
                    NoiDung = noiDung,
                    NgayBinhLuan = DateTime.Now
                };
                db.BinhLuans.Add(bl);
            }

            // 2. Xử lý lưu Đánh giá số sao (nếu người dùng có tích chọn sao)
            if (soSao.HasValue)
            {
                // Nếu đã từng đánh giá sản phẩm này rồi thì cập nhật lại số sao, chưa thì thêm mới
                var danhGiaCu = db.DanhGias.FirstOrDefault(d => d.MaSanPham == maSanPham && d.MaNguoiDung == maNguoiDung);
                if (danhGiaCu != null)
                {
                    danhGiaCu.SoSao = soSao.Value;
                }
                else
                {
                    DanhGia dg = new DanhGia
                    {
                        MaSanPham = maSanPham,
                        MaNguoiDung = maNguoiDung,
                        SoSao = soSao.Value
                    };
                    db.DanhGias.Add(dg);
                }
            }

            db.SaveChanges();
            return RedirectToAction("Details", new { id = maSanPham });
        }
    }
}