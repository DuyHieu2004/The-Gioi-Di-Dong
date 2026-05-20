-- ==========================================
-- 1. TẠO DATABASE
-- ==========================================
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'QLSP')
BEGIN
    CREATE DATABASE QLSP;
END
GO

USE QLSP;
GO

-- ==========================================
-- 2. TẠO BẢNG ĐỘC LẬP (Không chứa Foreign Key)
-- ==========================================

CREATE TABLE NguoiDung (
    MaNguoiDung INT IDENTITY PRIMARY KEY,
    TenDangNhap NVARCHAR(50) UNIQUE NOT NULL,
    MatKhau NVARCHAR(64) NOT NULL,
    HoTen NVARCHAR(100),
    Email NVARCHAR(100),
    DienThoai NVARCHAR(20),
    DiaChi NVARCHAR(255),
    QuyenHan NVARCHAR(20) DEFAULT 'User'
);

CREATE TABLE DanhMucSP (
    MaDanhMuc INT IDENTITY PRIMARY KEY,
    TenDanhMuc NVARCHAR(100),
    MoTa NVARCHAR(255)
);

CREATE TABLE TrangThaiSanPham (
    MaTrangThai INT IDENTITY PRIMARY KEY,
    TenTrangThai NVARCHAR(50)
);

CREATE TABLE TrangThaiDonHang (
    MaTrangThai INT IDENTITY PRIMARY KEY,
    TenTrangThai NVARCHAR(50)
);

CREATE TABLE TinTuc (
    MaTin INT IDENTITY PRIMARY KEY,
    TieuDe NVARCHAR(100),
    NoiDung NVARCHAR(MAX),
    TacGia NVARCHAR(50),
    NgayDang DATETIME DEFAULT GETDATE()
);

CREATE TABLE __EFMigrationsHistory (
    MigrationId NVARCHAR(150) NOT NULL PRIMARY KEY,
    ProductVersion NVARCHAR(32) NOT NULL
);

-- ==========================================
-- 3. TẠO BẢNG PHỤ THUỘC (Chứa Foreign Key)
-- ==========================================

CREATE TABLE SanPham (
    MaSanPham INT IDENTITY PRIMARY KEY,
    MaDanhMuc INT FOREIGN KEY REFERENCES DanhMucSP(MaDanhMuc),
    TenSanPham NVARCHAR(100),
    Gia DECIMAL(18,0),
    SoLuong INT,
    MoTa NVARCHAR(255),
    HinhAnh NVARCHAR(100),
    MaTrangThai INT FOREIGN KEY REFERENCES TrangThaiSanPham(MaTrangThai)
);

CREATE TABLE KhuyenMai (
    MaKM INT IDENTITY PRIMARY KEY,
    MaGiamGia NVARCHAR(20),
    PhanTramGiam INT,
    NgayBatDau DATE,
    NgayKetThuc DATE,
    LoaiKhuyenMai INT DEFAULT 0,
    MaSanPham INT FOREIGN KEY REFERENCES SanPham(MaSanPham),
    MaDanhMuc INT FOREIGN KEY REFERENCES DanhMucSP(MaDanhMuc)
);

CREATE TABLE DonHang (
    MaDonHang INT IDENTITY PRIMARY KEY,
    MaNguoiDung INT FOREIGN KEY REFERENCES NguoiDung(MaNguoiDung),
    NgayDat DATETIME DEFAULT GETDATE(),
    TongTien DECIMAL(18,0),
    MaTrangThai INT FOREIGN KEY REFERENCES TrangThaiDonHang(MaTrangThai)
);

CREATE TABLE ChiTietDH (
    MaChiTiet INT IDENTITY PRIMARY KEY,
    MaDonHang INT FOREIGN KEY REFERENCES DonHang(MaDonHang),
    MaSanPham INT FOREIGN KEY REFERENCES SanPham(MaSanPham),
    SoLuong INT,
    DonGia DECIMAL(18,0)
);

CREATE TABLE GioHang (
    MaGioHang INT IDENTITY PRIMARY KEY,
    MaNguoiDung INT FOREIGN KEY REFERENCES NguoiDung(MaNguoiDung)
);

CREATE TABLE ChiTietGH (
    MaChiTietGH INT IDENTITY PRIMARY KEY,
    MaGioHang INT FOREIGN KEY REFERENCES GioHang(MaGioHang),
    MaSanPham INT FOREIGN KEY REFERENCES SanPham(MaSanPham),
    SoLuong INT
);

CREATE TABLE DanhGia (
    MaDanhGia INT IDENTITY PRIMARY KEY,
    MaSanPham INT FOREIGN KEY REFERENCES SanPham(MaSanPham),
    MaNguoiDung INT FOREIGN KEY REFERENCES NguoiDung(MaNguoiDung),
    SoSao INT CHECK(SoSao BETWEEN 1 AND 5)
);

CREATE TABLE BinhLuan (
    MaBinhLuan INT IDENTITY PRIMARY KEY,
    MaSanPham INT FOREIGN KEY REFERENCES SanPham(MaSanPham),
    MaNguoiDung INT FOREIGN KEY REFERENCES NguoiDung(MaNguoiDung),
    NoiDung NVARCHAR(255),
    NgayBinhLuan DATETIME DEFAULT GETDATE()
);

-- ==========================================
-- 4. TẠO INDEXES (Tối ưu truy vấn)
-- ==========================================
CREATE INDEX IX_BinhLuan_MaNguoiDung ON BinhLuan(MaNguoiDung);
CREATE INDEX IX_BinhLuan_MaSanPham ON BinhLuan(MaSanPham);
CREATE INDEX IX_ChiTietDH_MaDonHang ON ChiTietDH(MaDonHang);
CREATE INDEX IX_ChiTietDH_MaSanPham ON ChiTietDH(MaSanPham);
CREATE INDEX IX_ChiTietGH_MaGioHang ON ChiTietGH(MaGioHang);
CREATE INDEX IX_ChiTietGH_MaSanPham ON ChiTietGH(MaSanPham);
CREATE INDEX IX_DanhGia_MaNguoiDung ON DanhGia(MaNguoiDung);
CREATE INDEX IX_DanhGia_MaSanPham ON DanhGia(MaSanPham);
CREATE INDEX IX_DonHang_MaNguoiDung ON DonHang(MaNguoiDung);
CREATE INDEX IX_DonHang_MaTrangThai ON DonHang(MaTrangThai);
CREATE INDEX IX_GioHang_MaNguoiDung ON GioHang(MaNguoiDung);
CREATE INDEX IX_SanPham_MaDanhMuc ON SanPham(MaDanhMuc);
CREATE INDEX IX_SanPham_MaTrangThai ON SanPham(MaTrangThai);
GO

-- ==========================================
-- 5. THÊM DỮ LIỆU MẪU
-- ==========================================
INSERT INTO NguoiDung (TenDangNhap, MatKhau, HoTen, Email, DienThoai, DiaChi, QuyenHan) VALUES
('admin', CONVERT(VARCHAR(64), HASHBYTES('SHA2_256', 'admin123'), 2), N'Quản trị viên', 'admin@tgdd.com', '0123456789', N'Hà Nội', 'Admin'),
('user1', CONVERT(VARCHAR(64), HASHBYTES('SHA2_256', 'user123'), 2), N'Nguyễn Văn A', 'user1@gmail.com', '0987654321', N'Hồ Chí Minh', 'User'),
('user2', CONVERT(VARCHAR(64), HASHBYTES('SHA2_256', 'user123'), 2), N'Trần Thị B', 'user2@gmail.com', '0912345678', N'Đà Nẵng', 'User'),
('user3', CONVERT(VARCHAR(64), HASHBYTES('SHA2_256', 'user123'), 2), N'Lê Văn C', 'user3@gmail.com', '0934567890', N'Hải Phòng', 'User'),
('user4', CONVERT(VARCHAR(64), HASHBYTES('SHA2_256', 'user123'), 2), N'Phạm Thị D', 'user4@gmail.com', '0945678901', N'Cần Thơ', 'User');

INSERT INTO DanhMucSP (TenDanhMuc, MoTa) VALUES
(N'Điện thoại', N'Smartphone các hãng'),
(N'Máy tính bảng', N'Tablet iOS/Android'),
(N'Laptop', N'Laptop văn phòng, gaming'),
(N'Phụ kiện', N'Tai nghe, sạc, ốp lưng'),
(N'Đồng hồ thông minh', N'Apple Watch, Galaxy Watch'),
(N'Tivi', N'Tivi thông minh, OLED, QLED'),
(N'Màn hình', N'Màn hình máy tính'),
(N'PC - Máy bộ', N'Máy tính để bàn'),
(N'Thiết bị mạng', N'Router, Wifi Mesh'),
(N'Gaming Gear', N'Chuột, bàn phím gaming');

INSERT INTO TrangThaiSanPham (TenTrangThai) VALUES
(N'Còn hàng'), (N'Hết hàng'), (N'Ngừng kinh doanh'), (N'Sắp ra mắt'), (N'Đang nhập hàng');

INSERT INTO SanPham (MaDanhMuc, TenSanPham, Gia, SoLuong, MoTa, HinhAnh, MaTrangThai) VALUES
(1,N'iPhone 14',23990000,40,N'iPhone 14 chính hãng',N'1.jpg',1),
(1,N'iPhone 13',19990000,50,N'iPhone 13 pin trâu',N'2.jpg',1),
(1,N'Xiaomi 14',18990000,60,N'Flagship Xiaomi',N'3.jpg',1),
(1,N'Redmi Note 13 Pro',9990000,80,N'Camera 200MP',N'4.jpg',1),
(2,N'iPad Gen 10',11990000,45,N'iPad học tập',N'5.jpg',1),
(2,N'iPad Mini 6',13990000,30,N'iPad nhỏ gọn',N'6.jpg',1),
(2,N'Galaxy Tab S8',15990000,20,N'Tablet Samsung',N'7.jpg',1),
(3,N'MacBook Pro M2',45990000,15,N'Laptop Apple cao cấp',N'8.jpg',1),
(3,N'MacBook Air M1',22990000,25,N'Laptop sinh viên',N'9.jpg',1),
(3,N'HP Pavilion 15',17990000,40,N'Laptop văn phòng',N'10.jpg',1),
(3,N'Lenovo ThinkPad X1',39990000,10,N'Laptop doanh nhân',N'11.jpg',1),
(4,N'Cáp sạc Anker Type C',390000,300,N'Cáp sạc nhanh',N'12.jpg',1),
(4,N'Pin sạc dự phòng 20000mAh',790000,150,N'Pin dung lượng lớn',N'13.jpg',1),
(4,N'Tai nghe Sony WH-1000XM5',8990000,35,N'Tai nghe chống ồn',N'14.jpg',1),
(5,N'Apple Watch Series 9',10990000,50,N'Apple Watch mới',N'15.jpg',1),
(5,N'Garmin Forerunner 965',14990000,20,N'Đồng hồ thể thao',N'16.jpg',1),
(6,N'Samsung QLED 55 inch',18990000,20,N'Tivi QLED',N'17.jpg',1),
(6,N'LG OLED C3 65 inch',42990000,10,N'Tivi OLED',N'18.jpg',1),
(7,N'Màn hình Dell 27 inch',6990000,40,N'Màn hình văn phòng',N'19.jpg',1),
(7,N'Màn hình LG UltraGear',9990000,25,N'Màn hình gaming',N'20.jpg',1),
(8,N'PC Gaming RTX 4060',32990000,10,N'Máy chơi game',N'21.jpg',1),
(8,N'PC Văn phòng i5',15990000,30,N'Máy văn phòng',N'22.jpg',1),
(9,N'Router TP-Link AX73',3490000,50,N'Wifi 6',N'23.jpg',1),
(9,N'Wifi Mesh Deco X20',5990000,35,N'Wifi phủ sóng',N'24.jpg',1),
(10,N'Bàn phím cơ Akko',1590000,80,N'Bàn phím gaming',N'25.jpg',1),
(10,N'Chuột Logitech G Pro',2590000,60,N'Chuột esport',N'26.jpg',1),
(10,N'Tai nghe SteelSeries Arctis 7',3990000,40,N'Headset gaming',N'27.jpg',1),
(1, N'iPhone 15 Pro Max', 34990000, 50, N'Flagship Apple 2023', N'28.jpg', 1),
(1, N'Samsung Galaxy S24 Ultra', 29990000, 30, N'Flagship Samsung 2024', N'29.jpg', 1),
(2, N'iPad Pro M2 11 inch', 25990000, 20, N'Máy tính bảng hiệu năng cao', N'30.jpg', 1),
(3, N'Dell XPS 15', 45990000, 10, N'Laptop cao cấp', N'31.jpg', 2),
(4, N'AirPods Pro 2', 5990000, 100, N'Tai nghe chống ồn', N'32.jpg', 1),
(5, N'Apple Watch Ultra 2', 18990000, 25, N'Đồng hồ thể thao cao cấp', N'33.jpg', 4),
(1, N'OPPO Find X7', 19990000, 40, N'Camera mạnh, sạc nhanh', N'34.jpg', 1),
(3, N'Asus ROG Zephyrus', 49990000, 5, N'Laptop gaming hiệu năng cao', N'35.jpg', 1),
(1, N'iPhone 14 Pro', 27990000, 35, N'Flagship Apple 2022', N'36.jpg', 1),
(1, N'Xiaomi 14 Pro', 22990000, 45, N'Hiệu năng cao, giá tốt', N'37.jpg', 1),
(2, N'Samsung Galaxy Tab S9', 18990000, 25, N'Tablet Android cao cấp', N'38.jpg', 1),
(3, N'HP Spectre x360', 38990000, 12, N'Laptop 2-in-1 cao cấp', N'39.jpg', 1),
(3, N'MacBook Air M3', 32990000, 20, N'Laptop mỏng nhẹ Apple', N'40.jpg', 1),
(4, N'Sạc nhanh Anker 65W', 1290000, 200, N'Sạc nhanh đa cổng', N'41.jpg', 1),
(4, N'Chuột Logitech MX Master 3S', 2490000, 80, N'Chuột không dây cao cấp', N'42.jpg', 1),
(5, N'Galaxy Watch 6 Classic', 9990000, 40, N'Đồng hồ thông minh Samsung', N'43.jpg', 1);

INSERT INTO TrangThaiDonHang (TenTrangThai) VALUES
(N'Chờ xác nhận'), (N'Đã xác nhận'), (N'Đang giao'), (N'Hoàn tất'), (N'Hủy');

INSERT INTO TinTuc (TieuDe, NoiDung, TacGia) VALUES
(N'Apple ra mắt iPhone 15', N'iPhone 15 Pro Max với chip A17 Pro.', N'Admin'),
(N'Samsung Galaxy S24 Ultra trình làng', N'Mẫu flagship mới với camera 200MP.', N'Admin'),
(N'Laptop ROG mới của Asus', N'Thiết kế mỏng nhẹ, hiệu năng cao.', N'Admin'),
(N'MacBook Air M3 ra mắt', N'Apple giới thiệu MacBook Air dùng chip M3 mạnh mẽ.', N'Admin'),
(N'Xiaomi 14 Pro mở bán tại Việt Nam', N'Flagship Xiaomi với giá cạnh tranh.', N'Admin'),
(N'Galaxy Watch 6 cải tiến pin', N'Samsung nâng cấp pin và màn hình.', N'Admin');

INSERT INTO KhuyenMai (MaGiamGia, PhanTramGiam, NgayBatDau, NgayKetThuc) VALUES
('SALE10', 10, '2025-09-01', '2025-09-30'),
('TGDD20', 20, '2025-09-15', '2025-10-15');

INSERT INTO __EFMigrationsHistory (MigrationId, ProductVersion) VALUES 
(N'20251220161346_UpdateKhuyenMaiForDiscount', N'9.0.9');
GO