/*
===================================================================
===        SCRIPT TỔNG HỢP CUỐI CÙNG CHO CSDL CAFEBOOK           ===
===           (VERSION ĐÃ SỬA LỖI - CHỈ CẤU TRÚC BẢNG)          ===
===================================================================
*/

-- =============================================
-- ===         NHOM BANG CO BAN (TAO TRUOC)    ===
-- =============================================

CREATE TABLE [dbo].[VaiTro] (
    [idVaiTro]  INT           IDENTITY (1, 1) NOT NULL,
    [tenVaiTro] NVARCHAR (50) NOT NULL,
    PRIMARY KEY CLUSTERED ([idVaiTro] ASC)
);
GO

CREATE TABLE [dbo].[CaLamViec] (
    [idCa]        INT           IDENTITY (1, 1) NOT NULL,
    [tenCa]       NVARCHAR (50) NOT NULL,
    [gioBatDau]   TIME (7)      NOT NULL,
    [gioKetThuc]  TIME (7)      NOT NULL,
    PRIMARY KEY CLUSTERED ([idCa] ASC)
);
GO

CREATE TABLE [dbo].[LoaiSanPham] (
    [idLoaiSP]  INT            IDENTITY (1, 1) NOT NULL,
    [tenLoaiSP] NVARCHAR (100) NOT NULL,
    PRIMARY KEY CLUSTERED ([idLoaiSP] ASC)
);
GO

CREATE TABLE [dbo].[NguyenLieu] (
    [idNguyenLieu]   INT             IDENTITY (1, 1) NOT NULL,
    [tenNguyenLieu]  NVARCHAR (100)  NOT NULL,
    [donViTinh]      NVARCHAR (20)   NOT NULL,
    [soLuongTon]     DECIMAL (18, 3) DEFAULT ((0)) NOT NULL,
    [nguongCanhBao]  DECIMAL (18, 2) DEFAULT ((5)) NOT NULL,
    PRIMARY KEY CLUSTERED ([idNguyenLieu] ASC)
);
GO

CREATE TABLE [dbo].[NhaCungCap] (
    [idNhaCungCap]   INT            IDENTITY (1, 1) NOT NULL,
    [tenNhaCungCap]  NVARCHAR (150) NOT NULL,
    [soDienThoai]    VARCHAR (15)   NULL,
    [diaChi]         NVARCHAR (255) NULL,
    PRIMARY KEY CLUSTERED ([idNhaCungCap] ASC)
);
GO

CREATE TABLE [dbo].[Ban] (
    [idBan]     INT            IDENTITY (1, 1) NOT NULL,
    [soBan]     NVARCHAR (50)  NOT NULL, -- SỬA LỖI: Tăng kích thước
    [soGhe]     INT            NOT NULL,
    [trangThai] NVARCHAR (50)  NOT NULL,
    [ghiChu]    NVARCHAR (255) NULL,
    PRIMARY KEY CLUSTERED ([idBan] ASC)
);
GO

CREATE TABLE [dbo].[Sach] (
    [idSach]        INT             IDENTITY (1, 1) NOT NULL,
    [tieuDe]        NVARCHAR (255)  NOT NULL,
    [tacGia]        NVARCHAR (150)  NULL,
    [theLoai]       NVARCHAR (100)  NULL,
    [moTa]          NVARCHAR (MAX)  NULL,
    [tongSoLuong]   INT             NOT NULL,
    [soLuongCoSan]  INT             NOT NULL,
    PRIMARY KEY CLUSTERED ([idSach] ASC)
);
GO

CREATE TABLE [dbo].[KhachHang] (
    [idKhachHang]   INT            IDENTITY (1, 1) NOT NULL,
    [hoTen]         NVARCHAR (100) NOT NULL,
    [soDienThoai]   VARCHAR (15)   NULL,
    [ngayTao]       DATETIME2 (7)  DEFAULT (getdate()) NOT NULL,
    PRIMARY KEY CLUSTERED ([idKhachHang] ASC),
    UNIQUE NONCLUSTERED ([soDienThoai] ASC)
);
GO

CREATE TABLE [dbo].[LoaiThuongPhat] (
    [idLoai]   INT             IDENTITY (1, 1) NOT NULL,
    [tenLoai]  NVARCHAR (100)  NOT NULL,
    [soTien]   DECIMAL (18, 2) NOT NULL,
    [loai]     NVARCHAR (20)   NOT NULL,
    PRIMARY KEY CLUSTERED ([idLoai] ASC)
);
GO

-- =============================================
-- ===      NHOM BANG PHU THUOC (TAO SAU)      ===
-- =============================================

CREATE TABLE [dbo].[NhanVien] (
    [idNhanVien]      INT             IDENTITY (1, 1) NOT NULL,
    [idVaiTro]        INT             NOT NULL,
    [hoTen]           NVARCHAR (100)  NOT NULL,
    [soDienThoai]     VARCHAR (15)    NULL,
    [email]           VARCHAR (100)   NULL,
    [diaChi]          NVARCHAR (255)  NULL,
    [matKhau]         NVARCHAR (255)  NOT NULL,
    [ngayVaoLam]      DATE            NOT NULL,
    [trangThai]       BIT             NOT NULL,
    [mucLuongTheoGio] DECIMAL (18, 2) DEFAULT ((20000)) NOT NULL,
    PRIMARY KEY CLUSTERED ([idNhanVien] ASC),
    FOREIGN KEY ([idVaiTro]) REFERENCES [dbo].[VaiTro] ([idVaiTro]),
    UNIQUE NONCLUSTERED ([email] ASC),
    UNIQUE NONCLUSTERED ([soDienThoai] ASC)
);
GO

CREATE TABLE [dbo].[SanPham] (
    [idSanPham]   INT             IDENTITY (1, 1) NOT NULL,
    [idLoaiSP]    INT             NOT NULL,
    [tenSanPham]  NVARCHAR (150)  NOT NULL,
    [moTa]        NVARCHAR (MAX)  NULL,
    [donGia]      DECIMAL (18, 2) NOT NULL,
    [hinhAnh]     NVARCHAR (255)  NULL,
    [trangThai]   NVARCHAR (50)   NOT NULL,
    PRIMARY KEY CLUSTERED ([idSanPham] ASC),
    FOREIGN KEY ([idLoaiSP]) REFERENCES [dbo].[LoaiSanPham] ([idLoaiSP])
);
GO

CREATE TABLE [dbo].[KhuyenMai] (
    [idKhuyenMai]             INT             IDENTITY (1, 1) NOT NULL,
    [tenKhuyenMai]            NVARCHAR (150)  NOT NULL,
    [moTa]                    NVARCHAR (MAX)  NULL,
    [loaiGiamGia]             VARCHAR (20)    NOT NULL,
    [giaTriGiam]              DECIMAL (18, 2) NOT NULL,
    [ngayBatDau]              DATETIME2 (7)   NOT NULL,
    [ngayKetThuc]             DATETIME2 (7)   NOT NULL,
    [giaTriDonHangToiThieu]   DECIMAL (18, 2) NULL,
    [idSanPhamApDung]         INT             NULL,
    PRIMARY KEY CLUSTERED ([idKhuyenMai] ASC),
    FOREIGN KEY ([idSanPhamApDung]) REFERENCES [dbo].[SanPham] ([idSanPham])
);
GO

CREATE TABLE [dbo].[LichLamViec] (
    [idLichLamViec] INT  IDENTITY (1, 1) NOT NULL,
    [idNhanVien]    INT  NOT NULL,
    [idCa]          INT  NOT NULL,
    [ngayLam]       DATE NOT NULL,
    PRIMARY KEY CLUSTERED ([idLichLamViec] ASC),
    FOREIGN KEY ([idNhanVien]) REFERENCES [dbo].[NhanVien] ([idNhanVien]),
    FOREIGN KEY ([idCa]) REFERENCES [dbo].[CaLamViec] ([idCa])
);
GO

CREATE TABLE [dbo].[BangChamCong] (
    [idChamCong]    INT           IDENTITY (1, 1) NOT NULL,
    [idLichLamViec] INT           NOT NULL,
    [gioVao]        DATETIME2 (7) NULL,
    [gioRa]         DATETIME2 (7) NULL,
    [soGioLam]      DECIMAL (4, 2) NULL,
    PRIMARY KEY CLUSTERED ([idChamCong] ASC),
    FOREIGN KEY ([idLichLamViec]) REFERENCES [dbo].[LichLamViec] ([idLichLamViec])
);
GO

CREATE TABLE [dbo].[CongThuc] (
    [idSanPham]         INT             NOT NULL,
    [idNguyenLieu]      INT             NOT NULL,
    [luongCanThiet]     DECIMAL (18, 3) NOT NULL,
    [donViTinhSuDung]   NVARCHAR (20)   NULL,
    PRIMARY KEY CLUSTERED ([idSanPham] ASC, [idNguyenLieu] ASC),
    FOREIGN KEY ([idSanPham]) REFERENCES [dbo].[SanPham] ([idSanPham]),
    FOREIGN KEY ([idNguyenLieu]) REFERENCES [dbo].[NguyenLieu] ([idNguyenLieu])
);
GO

CREATE TABLE [dbo].[PhieuNhapKho] (
    [idPhieuNhap]  INT             IDENTITY (1, 1) NOT NULL,
    [idNhanVien]   INT             NOT NULL,
    [idNhaCungCap] INT             NOT NULL,
    [ngayNhap]     DATETIME2 (7)   NOT NULL,
    [tongTien]     DECIMAL (18, 2) NOT NULL,
    PRIMARY KEY CLUSTERED ([idPhieuNhap] ASC),
    FOREIGN KEY ([idNhanVien]) REFERENCES [dbo].[NhanVien] ([idNhanVien]),
    FOREIGN KEY ([idNhaCungCap]) REFERENCES [dbo].[NhaCungCap] ([idNhaCungCap])
);
GO

CREATE TABLE [dbo].[ChiTietPhieuNhap] (
    [idPhieuNhap]  INT             NOT NULL,
    [idNguyenLieu] INT             NOT NULL,
    [soLuong]      DECIMAL (18, 3) NOT NULL,
    [donGia]       DECIMAL (18, 2) NOT NULL,
    PRIMARY KEY CLUSTERED ([idPhieuNhap] ASC, [idNguyenLieu] ASC),
    FOREIGN KEY ([idPhieuNhap]) REFERENCES [dbo].[PhieuNhapKho] ([idPhieuNhap]),
    FOREIGN KEY ([idNguyenLieu]) REFERENCES [dbo].[NguyenLieu] ([idNguyenLieu])
);
GO

CREATE TABLE [dbo].[HoaDonDauVao] (
    [idHoaDonNhap]  INT            IDENTITY (1, 1) NOT NULL,
    [idPhieuNhap]   INT            NOT NULL,
    [maHoaDon]      NVARCHAR (50)  NULL,
    [ngayPhatHanh]  DATE           NULL,
    [duongDanFile]  NVARCHAR (255) NULL,
    PRIMARY KEY CLUSTERED ([idHoaDonNhap] ASC),
    FOREIGN KEY ([idPhieuNhap]) REFERENCES [dbo].[PhieuNhapKho] ([idPhieuNhap]),
    UNIQUE NONCLUSTERED ([idPhieuNhap] ASC)
);
GO

CREATE TABLE [dbo].[PhieuDatBan] (
    [idPhieuDatBan]   INT             IDENTITY (1, 1) NOT NULL,
    [idKhachHang]     INT             NULL,
    [tenKhachVangLai] NVARCHAR (100)  NULL,
    [sdtKhachVangLai] VARCHAR (15)    NULL,
    [idBan]           INT             NULL,
    [thoiGianDat]     DATETIME2 (7)   NOT NULL,
    [soLuongKhach]    INT             NOT NULL,
    [ghiChu]          NVARCHAR (MAX)  NULL,
    [trangThai]       NVARCHAR (50)   NOT NULL,
    PRIMARY KEY CLUSTERED ([idPhieuDatBan] ASC),
    FOREIGN KEY ([idKhachHang]) REFERENCES [dbo].[KhachHang] ([idKhachHang]),
    FOREIGN KEY ([idBan]) REFERENCES [dbo].[Ban] ([idBan])
);
GO

CREATE TABLE [dbo].[HoaDon] (
    [idHoaDon]    INT             IDENTITY (1, 1) NOT NULL,
    [idKhachHang] INT             NULL,
    [idNhanVien]  INT             NOT NULL,
    [idBan]       INT             NOT NULL,
    [idKhuyenMai] INT             NULL,
    [thoiGianTao] DATETIME2 (7)   NOT NULL,
    [tongTien]    DECIMAL (18, 2) NOT NULL,
    [soTienGiam]  DECIMAL (18, 2) DEFAULT ((0)) NOT NULL,
    [thanhTien]   DECIMAL (18, 2) NOT NULL,
    [trangThai]   NVARCHAR (50)   NOT NULL,
    PRIMARY KEY CLUSTERED ([idHoaDon] ASC),
    FOREIGN KEY ([idKhachHang]) REFERENCES [dbo].[KhachHang] ([idKhachHang]),
    FOREIGN KEY ([idNhanVien]) REFERENCES [dbo].[NhanVien] ([idNhanVien]),
    FOREIGN KEY ([idBan]) REFERENCES [dbo].[Ban] ([idBan]),
    FOREIGN KEY ([idKhuyenMai]) REFERENCES [dbo].[KhuyenMai] ([idKhuyenMai])
);
GO

CREATE TABLE [dbo].[ChiTietHoaDon] (
    [idHoaDon]     INT             NOT NULL,
    [idSanPham]    INT             NOT NULL,
    [soLuong]      INT             NOT NULL,
    [donGiaLucBan] DECIMAL (18, 2) NOT NULL,
    [GhiChu]       NVARCHAR (255)  NULL,
    PRIMARY KEY CLUSTERED ([idHoaDon] ASC, [idSanPham] ASC),
    FOREIGN KEY ([idHoaDon]) REFERENCES [dbo].[HoaDon] ([idHoaDon]),
    FOREIGN KEY ([idSanPham]) REFERENCES [dbo].[SanPham] ([idSanPham])
);
GO

CREATE TABLE [dbo].[PhieuThueSach] (
    [idPhieuThue]    INT             IDENTITY (1, 1) NOT NULL,
    [idSach]         INT             NOT NULL,
    [idKhachHang]    INT             NOT NULL,
    [idNhanVien]     INT             NOT NULL,
    [ngayThue]       DATETIME2 (7)   NOT NULL,
    [ngayHenTra]     DATETIME2 (7)   NOT NULL,
    [ngayTraThucTe]  DATETIME2 (7)   NULL,
    [tienPhat]       DECIMAL (18, 2) DEFAULT ((0)) NOT NULL,
    [trangThai]      NVARCHAR (50)   NOT NULL,
    PRIMARY KEY CLUSTERED ([idPhieuThue] ASC),
    FOREIGN KEY ([idSach]) REFERENCES [dbo].[Sach] ([idSach]),
    FOREIGN KEY ([idKhachHang]) REFERENCES [dbo].[KhachHang] ([idKhachHang]),
    FOREIGN KEY ([idNhanVien]) REFERENCES [dbo].[NhanVien] ([idNhanVien])
);
GO

CREATE TABLE [dbo].[ThongBao] (
    [idThongBao]  INT            IDENTITY (1, 1) NOT NULL,
    [idNhanVien]  INT            NOT NULL,
    [noiDung]     NVARCHAR (500) NOT NULL,
    [thoiGianTao] DATETIME2 (7)  DEFAULT (getdate()) NOT NULL,
    [daDoc]       BIT            DEFAULT ((0)) NOT NULL,
    PRIMARY KEY CLUSTERED ([idThongBao] ASC),
    FOREIGN KEY ([idNhanVien]) REFERENCES [dbo].[NhanVien] ([idNhanVien])
);
GO

CREATE TABLE [dbo].[ChiTietThuongPhat] (
    [idChiTiet]  INT            IDENTITY (1, 1) NOT NULL,
    [idNhanVien] INT            NOT NULL,
    [idLoai]     INT            NOT NULL,
    [ngayApDung] DATE           NOT NULL,
    [ghiChu]     NVARCHAR (255) NULL,
    PRIMARY KEY CLUSTERED ([idChiTiet] ASC),
    FOREIGN KEY ([idNhanVien]) REFERENCES [dbo].[NhanVien] ([idNhanVien]),
    FOREIGN KEY ([idLoai]) REFERENCES [dbo].[LoaiThuongPhat] ([idLoai])
);
GO

CREATE TABLE [dbo].[PhieuLuong] (
    [idPhieuLuong]  INT             IDENTITY (1, 1) NOT NULL,
    [idNhanVien]    INT             NOT NULL,
    [tuNgay]        DATE            NOT NULL,
    [denNgay]       DATE            NOT NULL,
    [tongGioLam]    DECIMAL (18, 2) NOT NULL,
    [luongCoBan]    DECIMAL (18, 2) NOT NULL,
    [tongThuong]    DECIMAL (18, 2) NOT NULL,
    [tongPhat]      DECIMAL (18, 2) NOT NULL,
    [thucLanh]      DECIMAL (18, 2) NOT NULL,
    [ngayTinhLuong] DATETIME2 (7)   DEFAULT (getdate()) NOT NULL,
    PRIMARY KEY CLUSTERED ([idPhieuLuong] ASC),
    FOREIGN KEY ([idNhanVien]) REFERENCES [dbo].[NhanVien] ([idNhanVien])
);
GO

PRINT '*** TẠO CẤU TRÚC BẢNG MỚI THÀNH CÔNG! ***'
GO