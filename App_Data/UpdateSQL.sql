USE QLSP;
GO

-- Cập nhật mật khẩu thành 'admin123' cho tài khoản admin
UPDATE NguoiDung
SET MatKhau = LOWER(CONVERT(VARCHAR(64), HASHBYTES('SHA2_256', 'admin123'), 2))
WHERE TenDangNhap = 'admin';
GO

-- Thêm cột IsLocked vào bảng NguoiDung, mặc định là 0 (Không bị khóa)
ALTER TABLE NguoiDung 
ADD IsLocked BIT NOT NULL DEFAULT 0;
GO

ALTER TABLE SanPham 
ADD IsShow BIT NOT NULL DEFAULT 1;
GO