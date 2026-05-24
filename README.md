#  Đồ Án Lập Trình Web - Cửa Hàng Thế Giới Di Động

![ASP.NET MVC](https://img.shields.io/badge/Framework-ASP.NET_MVC-blue)
![C#](https://img.shields.io/badge/Language-C%23-purple)
![Bootstrap](https://img.shields.io/badge/UI-Bootstrap_5-success)

---


## Mục lục (Table of Contents)
- [Giới thiệu dự án](#giới-thiệu-dự-án)
- [Công nghệ & Thư viện sử dụng](#công-nghệ-thư-viện-sử-dụng)
- [Các tính năng nổi bật (Features)](#các-tính-năng-nổi-bật-features)
- [Cấu trúc thư mục (Project Structure)](#cấu-trúc-thư-mục-project-structure)
- [Hướng dẫn Cài đặt & Khởi chạy chi tiết](#hướng-dẫn-cài-đặt-khởi-chạy-chi-tiết-step-by-step)
- [Tài khoản Demo (Dành cho Test)](#tài-khoản-demo-dành-cho-test)

--- 

##  Giới thiệu dự án
Đây là hệ thống website thương mại điện tử chuyên cung cấp các thiết bị công nghệ (Điện thoại, Laptop, Phụ kiện, Smartwatch...). Dự án được xây dựng theo kiến trúc **ASP.NET MVC**, mô phỏng lại quy trình nghiệp vụ thực tế của một nền tảng bán lẻ đồ công nghệ. 

Dự án không chỉ tập trung vào trải nghiệm mua sắm mượt mà của khách hàng mà còn xây dựng một hệ thống Quản trị (Admin Dashboard) chuyên sâu, bảo mật và toàn vẹn dữ liệu. Đây là sản phẩm được thực hiện nhằm phục vụ cho đồ án học phần Lập trình Web tại trường **Đại học Công Thương TP.HCM (HUIT)**.


## Công nghệ & Thư viện sử dụng
* **Backend:** C# / ASP.NET MVC 5.
* **Cơ sở dữ liệu:** Microsoft SQL Server & Entity Framework (Mô hình Database First).
* **Frontend:** HTML5, CSS3, JavaScript.
* **UI Framework:** Bootstrap 5 (Tích hợp hoàn toàn Offline, đảm bảo chạy mượt mà không cần Internet).
* **Trực quan hóa dữ liệu:** Chart.js (Vẽ biểu đồ thống kê Offline).

---

## Các tính năng nổi bật (Features)

### Phân hệ Khách hàng (User Web)
* **Duyệt & Lọc sản phẩm:** Khách hàng có thể tìm kiếm sản phẩm theo tên, hoặc lọc theo từng danh mục (Điện thoại, Laptop...).
* **Quản lý Giỏ hàng:** Thêm, sửa số lượng, xóa sản phẩm khỏi giỏ hàng. Tổng tiền được tính toán tự động theo thời gian thực.
* **Áp dụng Khuyến mãi:** Hỗ trợ nhập mã Voucher giảm giá và trừ thẳng vào tổng tiền thanh toán.
* **Tương tác & Đánh giá:** Khách hàng đăng nhập có thể chấm điểm (Rating 1-5 sao) và để lại bình luận dưới mỗi sản phẩm.
* **Bảo mật Tài khoản:** Đăng ký và đăng nhập an toàn. Mật khẩu người dùng được mã hóa một chiều bằng thuật toán băm **SHA-256** trước khi lưu vào cơ sở dữ liệu.

###  Phân hệ Quản trị viên (Admin Dashboard)
* **Thống kê Trực quan:** Dashboard hiển thị tổng số đơn, số sản phẩm, tổng doanh thu. Tích hợp biểu đồ tròn (Trạng thái đơn) và biểu đồ cột (Doanh thu theo ngày).
* **Quản lý Sản phẩm (Soft Delete):** Hỗ trợ tính năng Thêm/Sửa thông tin sản phẩm. Đặc biệt, áp dụng kỹ thuật **Xóa mềm (Ẩn/Hiện)** để ngừng kinh doanh sản phẩm mà không làm ảnh hưởng đến dữ liệu khóa ngoại của các đơn hàng cũ.
* **Quản lý Đơn hàng:** Xem danh sách toàn bộ đơn đặt hàng và thao tác "Duyệt đơn" cực kỳ nhanh chóng.
* **Quản lý Khuyến mãi:** Admin có quyền tạo mã giảm giá mới, cấu hình mức % giảm, thiết lập ngày bắt đầu và ngày hết hạn.
* **Quản lý Tài khoản (Khóa User):** Cấp tài khoản mới hoặc kiểm soát người dùng hiện tại. Tích hợp tính năng **Khóa/Mở khóa tài khoản** bằng một cú click chuột đối với các khách hàng vi phạm. (Hệ thống tự động vô hiệu hóa quyền mua hàng đối với tài khoản Admin).

---

##  Hướng dẫn Cài đặt & Khởi chạy (Chạy Offline)

Để chạy dự án này trên máy cá nhân, vui lòng thực hiện theo các bước sau:

**1. Khởi tạo Cơ sở dữ liệu**
* Mở **Microsoft SQL Server Management Studio (SSMS)**.
* Chạy các script SQL nằm trong thư mục `App_Data` (Bao gồm file `databaseDiDong.sql` và các file `Update...` nếu có) để tạo cấu trúc bảng và chèn dữ liệu mẫu.

**2. Cấu hình Chuỗi kết nối**
* Mở dự án bằng **Visual Studio**.
* Mở file `Web.config` tại thư mục gốc của Solution.
* Tìm đến thẻ `<connectionStrings>` và cập nhật lại thuộc tính `Data Source=...` sao cho khớp với tên Server SQL trên máy của bạn. Cập nhật Model `.edmx` nếu cần thiết.

**3. Biên dịch và Chạy dự án**
* Nhấn tổ hợp phím `Ctrl + Shift + B` để tiến hành Clean và Rebuild lại toàn bộ hệ thống.
* Nhấn `F5` (hoặc nút Run IIS Express) để khởi chạy trang web trên trình duyệt.

---

## Tài khoản Demo (Dành cho Test)

Để trải nghiệm toàn bộ tính năng của hệ thống, bạn có thể sử dụng tài khoản sau:

* **Tài khoản Quản trị viên (Admin Dashboard):**
  * Tên đăng nhập: `admin`
  * Mật khẩu: `admin123`

* **Tài khoản Khách hàng (User):**
  * Tên đăng nhập: `user1`
  * Mật khẩu: `user1` (Hoặc có thể tự tạo mới qua form Đăng ký).
	

---
##  Cấu trúc thư mục (Project Structure)

Dự án được tổ chức chặt chẽ theo mô hình **MVC (Model - View - Controller)** chuẩn của ASP.NET. Dưới đây là kiến trúc các thư mục trọng yếu:

```text
TheGioiDiDong/
├── App_Data/                   # Nơi chứa file script SQL Server (databaseDiDong.sql)
├── App_Start/                  
│   └── RouteConfig.cs          # Cấu hình định tuyến (Mặc định trỏ về Product/Index)
├── Content/                    # Chứa tài nguyên tĩnh (CSS, Images)
│   ├── bootstrap/              # Thư viện Bootstrap CSS (Offline)
│   └── Images/                 # Hình ảnh sản phẩm (Laptop, Điện thoại...)
├── Controllers/                # Xử lý logic hệ thống (C#)
│   ├── AccountController.cs    # Đăng nhập, Đăng ký, Đăng xuất (User)
│   ├── AdminBaseController.cs  # Chốt chặn kiểm tra quyền Admin
│   ├── AdminHomeController.cs  # Xử lý Thống kê & Duyệt đơn hàng (Admin)
│   ├── AdminProductController.cs # Quản lý CRUD Sản phẩm (Admin)
│   ├── AdminAccountController.cs # Quản lý & Khóa tài khoản (Admin)
│   ├── CartController.cs       # Xử lý Giỏ hàng, Mã giảm giá, Thanh toán
│   └── ProductController.cs    # Hiển thị mặt tiền mua sắm, Đánh giá, Bình luận
├── Models/                     # Tương tác Cơ sở dữ liệu (Entity Framework)
│   └── QLSPModel.edmx          # Sơ đồ quan hệ các bảng (Database Context)
├── Scripts/                    # Chứa các file JavaScript
│   ├── bootstrap.bundle.min.js # Hiệu ứng giao diện Bootstrap
│   └── chart.min.js            # Thư viện vẽ biểu đồ doanh thu (Offline)
└── Views/                      # Chứa giao diện người dùng (.cshtml)
    ├── Account/                # Giao diện Đăng nhập / Đăng ký
    ├── Admin.../               # Các thư mục giao diện của riêng Admin
    ├── Cart/                   # Giao diện Giỏ hàng & Thông báo đặt hàng thành công
    ├── Product/                # Giao diện Trang chủ & Chi tiết sản phẩm
    └── Shared/                 
        ├── _Layout.cshtml      # Layout chung của Khách hàng
        └── _LayoutAdmin.cshtml # Layout thiết kế riêng cho Admin (Có Sidebar)

```   
##  Hướng dẫn Cài đặt & Khởi chạy chi tiết (Step-by-Step)

Để chạy dự án này dưới môi trường cục bộ (Localhost) một cách ổn định và hoàn toàn Offline, vui lòng thực hiện chuẩn xác theo các bước sau:

### Bước 1: Tải mã nguồn về máy tính
Có 2 cách để lấy mã nguồn về máy:
* **Cách 1 (Dùng Git):** Mở Terminal (Git Bash, Command Prompt hoặc PowerShell) tại thư mục bạn muốn lưu dự án và chạy lệnh:
  ```bash
  git clone https://github.com/DuyHieu2004/The-Gioi-Di-Dong.git
  ```

###  Bước 2: Thiết lập Cơ sở dữ liệu (SQL Server)
1. Khởi động phần mềm **Microsoft SQL Server Management Studio (SSMS)** và kết nối (Connect) vào Server của bạn.
2. Trên thanh menu, chọn **File -> Open -> File...** (hoặc nhấn `Ctrl + O`) và tìm đến file `databaseDiDong.sql` nằm trong thư mục `App_Data` của dự án.
3. Nhấn nút **Execute** (hoặc phím `F5`) để chạy script. Lệnh này sẽ tự động tạo cơ sở dữ liệu tên là `QLSP`, khởi tạo cấu trúc các bảng và chèn đầy đủ dữ liệu sản phẩm mẫu.
4. *(Nếu có)* Tiếp tục mở và chạy thêm các file script cập nhật như `UpdateSQL.sql` bằng cách tương tự để cập nhật các tính năng mới nhất (như cột `IsLocked`, `IsShow`).

###  Bước 3: Cấu hình chuỗi kết nối (Connection String)
1. Dùng Visual Studio mở thư mục dự án lên, tìm và mở file **`Web.config`** nằm ở thư mục gốc của dự án.
2. Tìm đến thẻ `<connectionStrings>`. Đoạn mã cấu hình sẽ có dạng tương tự như sau:
   ```xml
   <connectionStrings>
     <add name="QLSPEntities" connectionString="metadata=res://*/Models.QLSPModel.csdl|res://*/Models.QLSPModel.ssdl|res://*/Models.QLSPModel.msl;provider=System.Data.SqlClient;provider connection string=&quot;data source=TEN_SERVER_CUA_BAN;initial catalog=QLSP;integrated security=True;trustservercertificate=True;MultipleActiveResultSets=True;App=EntityFramework&quot;" providerName="System.Data.EntityClient" />
   </connectionStrings>
   ```
3. Hãy sửa lại giá trị của thuộc tính data source=... thành chính xác tên Server SQL Server trên máy tính của bạn (Ví dụ: data source=LAPTOP-1RKGC1HF\SQLEXPRESS hoặc data source=.). Sau đó nhấn Ctrl + S để lưu lại.

###  Bước 4: Khôi phục thư viện và Biên dịch dự án
Tại giao diện Visual Studio, nhìn sang cột Solution Explorer bên phải, click chuột phải vào dòng đầu tiên Solution 'TheGioiDiDong' và chọn Restore NuGet Packages để hệ thống tự động tải lại các thư viện còn thiếu.

Tiếp tục click chuột phải vào tên dự án TheGioiDiDong -> Chọn Clean để dọn dẹp bộ nhớ đệm cũ.

Click chuột phải một lần nữa vào TheGioiDiDong -> Chọn Rebuild. Hãy quan sát cửa sổ Output/Error List ở góc dưới màn hình, đảm bảo hệ thống báo Rebuild: 1 succeeded, 0 failed (Biên dịch thành công 100% và không có lỗi đỏ).

### Bước 5: Khởi chạy website
Trên thanh công cụ của Visual Studio, hãy chọn trình duyệt bạn muốn chạy (Ví dụ: Google Chrome, Microsoft Edge).

Nhấn phím F5 (hoặc nút IIS Express hình tam giác màu xanh) để khởi chạy dự án ở chế độ Debug.

Hệ thống sẽ tự động bật trình duyệt và dẫn thẳng vào trang chủ Thế Giới Di Động của nhóm (https://localhost:xxxx/).

Bạn có thể rút dây mạng Internet ra để kiểm tra, toàn bộ giao diện Bootstrap, biểu đồ Chart.js và các logic mua sắm, quản trị đều sẽ hoạt động mượt mà offline 100%.
