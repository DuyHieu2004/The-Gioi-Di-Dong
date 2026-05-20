using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TheGioiDiDong.Models;

namespace TheGioiDiDong.Controllers
{
    public class AdminHomeController : Controller
    {
        private QLSPEntities db = new QLSPEntities();

        public ActionResult Index()
        {
            // 1. Thống kê tổng số đơn hàng
            ViewBag.TongSoDonHang = db.DonHangs.Count();

            // 2. Thống kê doanh thu (Chỉ tính các đơn có Trạng thái = 4: Hoàn tất)
            ViewBag.TongDoanhThu = db.DonHangs
                                     .Where(d => d.MaTrangThai == 4)
                                     .Sum(d => (decimal?)d.TongTien) ?? 0;

            // 3. Thống kê tổng số lượng sản phẩm đang có
            ViewBag.TongSoSanPham = db.SanPhams.Count();

            // 4. Lấy danh sách đơn hàng để hiển thị ở bảng quản lý
            var donHangs = db.DonHangs.OrderByDescending(d => d.NgayDat).ToList();

            // 5. GOM DỮ LIỆU ĐỂ VẼ BIỂU ĐỒ TRÒN (TRẠNG THÁI)
            var thongKeBieuDo = db.DonHangs
                                  .GroupBy(d => d.TrangThaiDonHang.TenTrangThai)
                                  .Select(g => new { TenTrangThai = g.Key, SoLuong = g.Count() })
                                  .ToList();

            ViewBag.ChartLabels = string.Join(",", thongKeBieuDo.Select(x => $"'{x.TenTrangThai}'"));
            ViewBag.ChartData = string.Join(",", thongKeBieuDo.Select(x => x.SoLuong));

            // 6. GOM DỮ LIỆU ĐỂ VẼ BIỂU ĐỒ CỘT (DOANH THU THEO NGÀY) - TÍNH NĂNG MỚI
            // Lấy hết các đơn Hoàn Tất ra trước
            var donHoanTat = db.DonHangs.Where(d => d.MaTrangThai == 4 && d.NgayDat != null).ToList();

            // Nhóm theo ngày (bỏ giờ phút) và tính tổng tiền
            var doanhThuTheoNgay = donHoanTat
                                     .GroupBy(d => d.NgayDat.Value.Date)
                                     .OrderBy(g => g.Key) // Sắp xếp theo ngày tăng dần
                                     .Take(10) // Lấy 10 ngày gần nhất
                                     .Select(g => new { Ngay = g.Key.ToString("dd/MM"), DoanhThu = g.Sum(x => x.TongTien) })
                                     .ToList();

            ViewBag.RevenueLabels = string.Join(",", doanhThuTheoNgay.Select(x => $"'{x.Ngay}'"));
            ViewBag.RevenueData = string.Join(",", doanhThuTheoNgay.Select(x => x.DoanhThu));

            return View(donHangs);
        }

        // HÀM XỬ LÝ DUYỆT ĐƠN NHANH ĐỂ TEST DOANH THU
        public ActionResult CompleteOrder(int id)
        {
            DonHang dh = db.DonHangs.Find(id);
            if (dh != null)
            {
                dh.MaTrangThai = 4; // Chuyển sang trạng thái 4: Hoàn tất
                db.SaveChanges();
                TempData["Message"] = $"Đã phê duyệt hoàn tất đơn hàng #{id}. Doanh thu đã được cập nhật!";
            }
            return RedirectToAction("Index");
        }
    }

}