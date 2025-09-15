-- =============================================
-- ===     SCHEMA TINH GON CHO UNG DUNG WPF    ===
-- =============================================

-- =============================================
-- ===         NHOM BANG CO BAN (TAO TRUOC)    ===
-- =============================================

-- Vai tro cua Nhan Vien (Admin, Nhan vien...)
CREATE TABLE VaiTro (
    idVaiTro INT PRIMARY KEY IDENTITY(1,1),
    tenVaiTro NVARCHAR(50) NOT NULL
);
GO

-- Thong tin cac Ca lam viec trong ngay
CREATE TABLE CaLamViec (
    idCa INT PRIMARY KEY IDENTITY(1,1),
    tenCa NVARCHAR(50) NOT NULL,
    gioBatDau TIME NOT NULL,
    gioKetThuc TIME NOT NULL
);
GO

-- Loai san pham (Ca phe, Tra, Banh ngot...)
CREATE TABLE LoaiSanPham (
    idLoaiSP INT PRIMARY KEY IDENTITY(1,1),
    tenLoaiSP NVARCHAR(100) NOT NULL
);
GO

-- Danh sach nguyen lieu trong kho
CREATE TABLE NguyenLieu (
    idNguyenLieu INT PRIMARY KEY IDENTITY(1,1),
    tenNguyenLieu NVARCHAR(100) NOT NULL,
    donViTinh NVARCHAR(20) NOT NULL,
    soLuongTon DECIMAL(18, 2) NOT NULL DEFAULT 0
);
GO

-- Danh sach nha cung cap nguyen lieu
CREATE TABLE NhaCungCap (
    idNhaCungCap INT PRIMARY KEY IDENTITY(1,1),
    tenNhaCungCap NVARCHAR(150) NOT NULL,
    soDienThoai VARCHAR(15),
    diaChi NVARCHAR(255)
);
GO

-- Danh sach ban trong quan
CREATE TABLE Ban (
    idBan INT PRIMARY KEY IDENTITY(1,1),
    soBan NVARCHAR(10) NOT NULL,
    soGhe INT NOT NULL,
    trangThai NVARCHAR(50) NOT NULL -- Trong, Co khach, Da dat
);
GO

-- Thong tin cac chuong trinh khuyen mai
CREATE TABLE KhuyenMai (
    idKhuyenMai INT PRIMARY KEY IDENTITY(1,1),
    tenKhuyenMai NVARCHAR(150) NOT NULL,
    moTa NVARCHAR(MAX),
    loaiGiamGia VARCHAR(20) NOT NULL, -- PhanTram, SoTien
    giaTriGiam DECIMAL(18, 2) NOT NULL,
    ngayBatDau DATETIME2 NOT NULL,
    ngayKetThuc DATETIME2 NOT NULL
);
GO

-- Danh muc sach trong thu vien
CREATE TABLE Sach (
    idSach INT PRIMARY KEY IDENTITY(1,1),
    tieuDe NVARCHAR(255) NOT NULL,
    tacGia NVARCHAR(150),
    theLoai NVARCHAR(100),
    moTa NVARCHAR(MAX),
    tongSoLuong INT NOT NULL,
    soLuongCoSan INT NOT NULL
);
GO

-- Thong tin khach hang (phuc vu dat ban, quan ly khach quen)
-- [DA TINH GON] Bo mat khau, email, diem tich luy
CREATE TABLE KhachHang (
    idKhachHang INT PRIMARY KEY IDENTITY(1,1),
    hoTen NVARCHAR(100) NOT NULL,
    soDienThoai VARCHAR(15) UNIQUE,
    ngayTao DATETIME2 NOT NULL DEFAULT GETDATE()
);
GO


-- =============================================
-- ===      NHOM BANG PHU THUOC (TAO SAU)      ===
-- =============================================

-- Thong tin Nhan Vien
CREATE TABLE NhanVien (
    idNhanVien INT PRIMARY KEY IDENTITY(1,1),
    idVaiTro INT NOT NULL,
    hoTen NVARCHAR(100) NOT NULL,
    soDienThoai VARCHAR(15) UNIQUE,
    email VARCHAR(100) UNIQUE,
    diaChi NVARCHAR(255),
    matKhau NVARCHAR(255) NOT NULL, -- Mat khau dang nhap ung dung WPF
    ngayVaoLam DATE NOT NULL,
    trangThai BIT NOT NULL, -- 1 = Dang lam, 0 = Da nghi
    FOREIGN KEY (idVaiTro) REFERENCES VaiTro(idVaiTro)
);
GO

-- Bang phan cong lich lam viec
CREATE TABLE LichLamViec (
    idLichLamViec INT PRIMARY KEY IDENTITY(1,1),
    idNhanVien INT NOT NULL,
    idCa INT NOT NULL,
    ngayLam DATE NOT NULL,
    FOREIGN KEY (idNhanVien) REFERENCES NhanVien(idNhanVien),
    FOREIGN KEY (idCa) REFERENCES CaLamViec(idCa)
);
GO

-- Bang ghi nhan cham cong thuc te
CREATE TABLE BangChamCong (
    idChamCong INT PRIMARY KEY IDENTITY(1,1),
    idLichLamViec INT NOT NULL,
    gioVao DATETIME2,
    gioRa DATETIME2,
    soGioLam DECIMAL(4, 2),
    FOREIGN KEY (idLichLamViec) REFERENCES LichLamViec(idLichLamViec)
);
GO

-- Thong tin san pham trong menu
CREATE TABLE SanPham (
    idSanPham INT PRIMARY KEY IDENTITY(1,1),
    idLoaiSP INT NOT NULL,
    tenSanPham NVARCHAR(150) NOT NULL,
    moTa NVARCHAR(MAX),
    donGia DECIMAL(18, 2) NOT NULL,
    hinhAnh NVARCHAR(255),
    trangThai NVARCHAR(50) NOT NULL, -- Dang ban, Tam ngung
    FOREIGN KEY (idLoaiSP) REFERENCES LoaiSanPham(idLoaiSP)
);
GO

-- Bang dinh luong cong thuc cho san pham
CREATE TABLE CongThuc (
    idSanPham INT NOT NULL,
    idNguyenLieu INT NOT NULL,
    luongCanThiet DECIMAL(18, 2) NOT NULL,
    PRIMARY KEY (idSanPham, idNguyenLieu),
    FOREIGN KEY (idSanPham) REFERENCES SanPham(idSanPham),
    FOREIGN KEY (idNguyenLieu) REFERENCES NguyenLieu(idNguyenLieu)
);
GO

-- Thong tin phieu nhap kho
CREATE TABLE PhieuNhapKho (
    idPhieuNhap INT PRIMARY KEY IDENTITY(1,1),
    idNhanVien INT NOT NULL,
    idNhaCungCap INT NOT NULL,
    ngayNhap DATETIME2 NOT NULL,
    tongTien DECIMAL(18, 2) NOT NULL,
    FOREIGN KEY (idNhanVien) REFERENCES NhanVien(idNhanVien),
    FOREIGN KEY (idNhaCungCap) REFERENCES NhaCungCap(idNhaCungCap)
);
GO

-- Chi tiet cac nguyen lieu trong mot phieu nhap kho
CREATE TABLE ChiTietPhieuNhap (
    idPhieuNhap INT NOT NULL,
    idNguyenLieu INT NOT NULL,
    soLuong DECIMAL(18, 2) NOT NULL,
    donGia DECIMAL(18, 2) NOT NULL,
    PRIMARY KEY (idPhieuNhap, idNguyenLieu),
    FOREIGN KEY (idPhieuNhap) REFERENCES PhieuNhapKho(idPhieuNhap),
    FOREIGN KEY (idNguyenLieu) REFERENCES NguyenLieu(idNguyenLieu)
);
GO

-- Luu tru thong tin hoa don dau vao
CREATE TABLE HoaDonDauVao (
    idHoaDonNhap INT PRIMARY KEY IDENTITY(1,1),
    idPhieuNhap INT NOT NULL UNIQUE,
    maHoaDon NVARCHAR(50),
    ngayPhatHanh DATE,
    duongDanFile NVARCHAR(255),
    FOREIGN KEY (idPhieuNhap) REFERENCES PhieuNhapKho(idPhieuNhap)
);
GO

-- Thong tin dat ban (do nhan vien tao)
CREATE TABLE PhieuDatBan (
    idPhieuDatBan INT PRIMARY KEY IDENTITY(1,1),
    idKhachHang INT NULL, -- Cho phep NULL de luu khach vang lai
    tenKhachVangLai NVARCHAR(100) NULL,
    sdtKhachVangLai VARCHAR(15) NULL,
    idBan INT NULL, -- Ban co the duoc xep sau
    thoiGianDat DATETIME2 NOT NULL,
    soLuongKhach INT NOT NULL,
    ghiChu NVARCHAR(MAX),
    trangThai NVARCHAR(50) NOT NULL, -- Cho xac nhan, Da xac nhan, Da huy
    FOREIGN KEY (idKhachHang) REFERENCES KhachHang(idKhachHang),
    FOREIGN KEY (idBan) REFERENCES Ban(idBan)
);
GO

-- Thong tin hoa don ban hang
CREATE TABLE HoaDon (
    idHoaDon INT PRIMARY KEY IDENTITY(1,1),
    idKhachHang INT NULL, -- Cho phep NULL de luu khach vang lai
    idNhanVien INT NOT NULL,
    idBan INT NOT NULL,
    idKhuyenMai INT NULL,
    thoiGianTao DATETIME2 NOT NULL,
    tongTien DECIMAL(18, 2) NOT NULL,
    soTienGiam DECIMAL(18, 2) NOT NULL DEFAULT 0,
    thanhTien DECIMAL(18, 2) NOT NULL,
    trangThai NVARCHAR(50) NOT NULL, -- Chua thanh toan, Da thanh toan
    FOREIGN KEY (idKhachHang) REFERENCES KhachHang(idKhachHang),
    FOREIGN KEY (idNhanVien) REFERENCES NhanVien(idNhanVien),
    FOREIGN KEY (idBan) REFERENCES Ban(idBan),
    FOREIGN KEY (idKhuyenMai) REFERENCES KhuyenMai(idKhuyenMai)
);
GO

-- Chi tiet cac san pham trong mot hoa don
CREATE TABLE ChiTietHoaDon (
    idHoaDon INT NOT NULL,
    idSanPham INT NOT NULL,
    soLuong INT NOT NULL,
    donGiaLucBan DECIMAL(18, 2) NOT NULL,
    PRIMARY KEY (idHoaDon, idSanPham),
    FOREIGN KEY (idHoaDon) REFERENCES HoaDon(idHoaDon),
    FOREIGN KEY (idSanPham) REFERENCES SanPham(idSanPham)
);
GO

-- Thong tin cac luot thue sach
CREATE TABLE PhieuThueSach (
    idPhieuThue INT PRIMARY KEY IDENTITY(1,1),
    idSach INT NOT NULL,
    idKhachHang INT NOT NULL,
    idNhanVien INT NOT NULL,
    ngayThue DATETIME2 NOT NULL,
    ngayHenTra DATETIME2 NOT NULL,
    ngayTraThucTe DATETIME2 NULL, -- NULL khi chua tra
    tienPhat DECIMAL(18, 2) NOT NULL DEFAULT 0,
    trangThai NVARCHAR(50) NOT NULL, -- Dang thue, Da tra, Qua han
    FOREIGN KEY (idSach) REFERENCES Sach(idSach),
    FOREIGN KEY (idKhachHang) REFERENCES KhachHang(idKhachHang),
    FOREIGN KEY (idNhanVien) REFERENCES NhanVien(idNhanVien)
);
GO