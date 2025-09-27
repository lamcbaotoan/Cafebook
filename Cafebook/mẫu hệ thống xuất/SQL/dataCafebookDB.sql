/*
===================================================================
===        SCRIPT XÓA SẠCH VÀ THÊM LẠI DỮ LIỆU MẪU              ===
===             (PHIÊN BẢN NHIỀU DỮ LIỆU HƠN)                    ===
===================================================================
*/

PRINT '*** BẮT ĐẦU QUÁ TRÌNH XÓA DỮ LIỆU... ***';
GO

--- PHẦN 1: XÓA SẠCH TOÀN BỘ DỮ LIỆU CŨ ---

EXEC sp_MSforeachtable 'ALTER TABLE ? NOCHECK CONSTRAINT ALL'
GO
EXEC sp_MSforeachtable 'DELETE FROM ?'
GO

-- Reset lại tất cả các cột ID tự tăng về 1
DBCC CHECKIDENT ('[dbo].[VaiTro]', RESEED, 0);
DBCC CHECKIDENT ('[dbo].[CaLamViec]', RESEED, 0);
DBCC CHECKIDENT ('[dbo].[LoaiSanPham]', RESEED, 0);
DBCC CHECKIDENT ('[dbo].[NguyenLieu]', RESEED, 0);
DBCC CHECKIDENT ('[dbo].[NhaCungCap]', RESEED, 0);
DBCC CHECKIDENT ('[dbo].[Ban]', RESEED, 0);
DBCC CHECKIDENT ('[dbo].[Sach]', RESEED, 0);
DBCC CHECKIDENT ('[dbo].[KhachHang]', RESEED, 0);
DBCC CHECKIDENT ('[dbo].[LoaiThuongPhat]', RESEED, 0);
DBCC CHECKIDENT ('[dbo].[NhanVien]', RESEED, 0);
DBCC CHECKIDENT ('[dbo].[SanPham]', RESEED, 0);
DBCC CHECKIDENT ('[dbo].[KhuyenMai]', RESEED, 0);
DBCC CHECKIDENT ('[dbo].[LichLamViec]', RESEED, 0);
DBCC CHECKIDENT ('[dbo].[BangChamCong]', RESEED, 0);
DBCC CHECKIDENT ('[dbo].[PhieuNhapKho]', RESEED, 0);
DBCC CHECKIDENT ('[dbo].[HoaDonDauVao]', RESEED, 0);
DBCC CHECKIDENT ('[dbo].[PhieuDatBan]', RESEED, 0);
DBCC CHECKIDENT ('[dbo].[HoaDon]', RESEED, 0);
DBCC CHECKIDENT ('[dbo].[PhieuThueSach]', RESEED, 0);
DBCC CHECKIDENT ('[dbo].[ThongBao]', RESEED, 0);
DBCC CHECKIDENT ('[dbo].[ChiTietThuongPhat]', RESEED, 0);
DBCC CHECKIDENT ('[dbo].[PhieuLuong]', RESEED, 0);
GO

EXEC sp_MSforeachtable 'ALTER TABLE ? WITH CHECK CHECK CONSTRAINT ALL'
GO

PRINT '*** ĐÃ XÓA SẠCH DỮ LIỆU CŨ THÀNH CÔNG! ***';
GO
PRINT '*** BẮT ĐẦU THÊM DỮ LIỆU MẪU MỚI... ***';
GO

--- PHẦN 2: THÊM DỮ LIỆU MẪU MỚI ---

-- Bảng cơ bản
SET IDENTITY_INSERT [dbo].[VaiTro] ON;
INSERT INTO [dbo].[VaiTro] ([idVaiTro], [tenVaiTro]) VALUES (1, N'Quản lý'), (2, N'Nhân viên');
SET IDENTITY_INSERT [dbo].[VaiTro] OFF;
GO
SET IDENTITY_INSERT [dbo].[CaLamViec] ON;
INSERT INTO [dbo].[CaLamViec] ([idCa], [tenCa], [gioBatDau], [gioKetThuc]) VALUES (1, N'Ca Sáng', '07:00:00', '15:00:00'), (2, N'Ca Chiều', '15:00:00', '23:00:00'), (3, N'Ca Tối', '18:00:00', '23:00:00');
SET IDENTITY_INSERT [dbo].[CaLamViec] OFF;
GO
SET IDENTITY_INSERT [dbo].[LoaiSanPham] ON;
INSERT INTO [dbo].[LoaiSanPham] ([idLoaiSP], [tenLoaiSP]) VALUES (1, N'Cà phê'), (2, N'Trà'), (3, N'Nước ép'), (4, N'Bánh ngọt'), (5, N'Đồ ăn vặt');
SET IDENTITY_INSERT [dbo].[LoaiSanPham] OFF;
GO
SET IDENTITY_INSERT [dbo].[NguyenLieu] ON;
INSERT INTO [dbo].[NguyenLieu] ([idNguyenLieu], [tenNguyenLieu], [donViTinh], [soLuongTon], [nguongCanhBao]) VALUES 
(1, N'Hạt cà phê Robusta', N'kg', 8.50, 2.00), 
(2, N'Sữa tươi không đường', N'lít', 15.00, 5.00), 
(3, N'Đường cát trắng', N'kg', 45.00, 10.00),
(4, N'Lá trà Oolong', N'kg', 1.5, 0.5),
(5, N'Chanh tươi', N'kg', 3.0, 1.0),
(6, N'Bột mì', N'kg', 25.0, 5.0),
(7, N'Trứng gà', N'quả', 50, 12);
SET IDENTITY_INSERT [dbo].[NguyenLieu] OFF;
GO
SET IDENTITY_INSERT [dbo].[NhaCungCap] ON;
INSERT INTO [dbo].[NhaCungCap] ([idNhaCungCap], [tenNhaCungCap], [soDienThoai], [diaChi]) VALUES (1, N'Nông sản Đà Lạt Xanh', '0905111222', N'Lâm Đồng'),(2, N'Thực phẩm An Toàn', '0987654321', N'TP. Hồ Chí Minh'), (3, N'Vua Cà Phê', '0123456789', N'Đắk Lắk');
SET IDENTITY_INSERT [dbo].[NhaCungCap] OFF;
GO
SET IDENTITY_INSERT [dbo].[Ban] ON;
INSERT INTO [dbo].[Ban] ([idBan], [soBan], [soGhe], [trangThai], [ghiChu]) VALUES (1, N'A1', 2, N'Trống', N'Gần cửa sổ'), (2, N'A2', 4, N'Trống', NULL), (3, N'A3', 4, N'Trống', N'Trong góc'),(4, N'B1', 4, N'Bảo trì', N'Ngoài trời, bàn bị lung lay'), (5, N'Tầng 2 - T1', 8, N'Trống', N'Khu vực nhóm');
SET IDENTITY_INSERT [dbo].[Ban] OFF;
GO
SET IDENTITY_INSERT [dbo].[KhachHang] ON;
INSERT INTO [dbo].[KhachHang] ([idKhachHang], [hoTen], [soDienThoai]) VALUES (1, N'Nguyễn Văn An', '0912345678'), (2, N'Trần Thị Bích', '0988776655'), (3, N'Lê Minh Cường', '0935123123');
SET IDENTITY_INSERT [dbo].[KhachHang] OFF;
GO
SET IDENTITY_INSERT [dbo].[LoaiThuongPhat] ON;
INSERT INTO [dbo].[LoaiThuongPhat] ([idLoai], [tenLoai], [soTien], [loai]) VALUES (1, N'Thưởng chuyên cần tháng', 200000.00, N'Thuong'), (2, N'Phạt đi trễ (mỗi lần)', 50000.00, N'Phat'), (3, N'Phạt làm vỡ đồ', 100000.00, N'Phat');
SET IDENTITY_INSERT [dbo].[LoaiThuongPhat] OFF;
GO
SET IDENTITY_INSERT [dbo].[NhanVien] ON;
INSERT INTO [dbo].[NhanVien] ([idNhanVien], [idVaiTro], [hoTen], [soDienThoai], [email], [matKhau], [ngayVaoLam], [trangThai], [mucLuongTheoGio]) VALUES 
(1, 1, N'Trần Hoàng Anh', '0905123456', 'admin@email.com', 'admin', '2024-01-15', 1, 30000.00), 
(2, 2, N'Lê Thị Yến', '0905654321', 'nhanvien@email.com', '123', '2024-03-20', 1, 22000.00),
(3, 2, N'Phạm Văn Toàn', '0905987654', 'toanpham@email.com', '123', '2025-08-01', 1, 20000.00);
SET IDENTITY_INSERT [dbo].[NhanVien] OFF;
GO
SET IDENTITY_INSERT [dbo].[SanPham] ON;
INSERT INTO [dbo].[SanPham] ([idSanPham], [idLoaiSP], [tenSanPham], [donGia], [trangThai]) VALUES 
(1, 1, N'Cà phê Đen đá', 25000.00, N'Đang bán'), 
(2, 1, N'Cà phê Sữa', 30000.00, N'Đang bán'),
(3, 2, N'Trà Oolong', 40000.00, N'Đang bán'),
(4, 3, N'Nước chanh', 35000.00, N'Đang bán'),
(5, 4, N'Bánh Tiramisu', 35000.00, N'Đang bán');
SET IDENTITY_INSERT [dbo].[SanPham] OFF;
GO
SET IDENTITY_INSERT [dbo].[KhuyenMai] ON;
INSERT INTO [dbo].[KhuyenMai] ([idKhuyenMai], [tenKhuyenMai], [loaiGiamGia], [giaTriGiam], [ngayBatDau], [ngayKetThuc], [giaTriDonHangToiThieu], [idSanPhamApDung]) VALUES 
(1, N'Giảm 20k cho hóa đơn trên 100k', N'SoTien', 20000.00, '2025-01-01', '2025-12-31', 100000.00, NULL),
(2, N'Đi kèm Tiramisu giảm 50%', N'PhanTram', 50.00, '2025-01-01', '2025-12-31', NULL, 5);
SET IDENTITY_INSERT [dbo].[KhuyenMai] OFF;
GO
INSERT INTO [dbo].[CongThuc] ([idSanPham], [idNguyenLieu], [luongCanThiet], [donViTinhSuDung]) VALUES
(1, 1, 20, N'g'), (1, 3, 10, N'g'),
(2, 1, 20, N'g'), (2, 2, 40, N'ml'), (2, 3, 20, N'g'),
(3, 4, 15, N'g'),
(4, 5, 100, N'g'), (4, 3, 30, N'g');
GO
SET IDENTITY_INSERT [dbo].[LichLamViec] ON;
INSERT INTO [dbo].[LichLamViec] ([idLichLamViec], [idNhanVien], [idCa], [ngayLam]) VALUES
(1, 1, 1, GETDATE()),
(2, 2, 2, GETDATE()),
(3, 3, 2, GETDATE());
SET IDENTITY_INSERT [dbo].[LichLamViec] OFF;
GO
SET IDENTITY_INSERT [dbo].[BangChamCong] ON;
INSERT INTO [dbo].[BangChamCong] ([idChamCong], [idLichLamViec], [gioVao], [gioRa], [soGioLam]) VALUES
(1, 2, DATEADD(hour, -3, GETDATE()), NULL, NULL); -- Yến đã vào ca
SET IDENTITY_INSERT [dbo].[BangChamCong] OFF;
GO
SET IDENTITY_INSERT [dbo].[ThongBao] ON;
INSERT INTO [dbo].[ThongBao] ([idThongBao], [idNhanVien], [noiDung], [daDoc]) VALUES
(1, 2, N'[Bàn B1]: Bàn ngoài trời bị lung lay, cần sửa chữa.', 0),
(2, 3, N'[Kho]: Sữa tươi sắp hết, cần nhập thêm.', 0);
SET IDENTITY_INSERT [dbo].[ThongBao] OFF;
GO

-- Hóa đơn 1 (chưa thanh toán)
SET IDENTITY_INSERT [dbo].[HoaDon] ON;
INSERT INTO [dbo].[HoaDon] ([idHoaDon], [idNhanVien], [idBan], [thoiGianTao], [tongTien], [thanhTien], [trangThai]) VALUES (1, 2, 2, DATEADD(minute, -30, GETDATE()), 65000.00, 65000.00, N'Chưa thanh toán');
SET IDENTITY_INSERT [dbo].[HoaDon] OFF;
INSERT INTO [dbo].[ChiTietHoaDon] ([idHoaDon], [idSanPham], [soLuong], [donGiaLucBan]) VALUES (1, 1, 1, 25000.00), (1, 4, 1, 35000.00);
GO

-- Hóa đơn 2 (đã thanh toán)
SET IDENTITY_INSERT [dbo].[HoaDon] ON;
INSERT INTO [dbo].[HoaDon] ([idHoaDon], [idNhanVien], [idBan], [thoiGianTao], [tongTien], [thanhTien], [trangThai]) VALUES (2, 3, 1, DATEADD(hour, -2, GETDATE()), 55000.00, 55000.00, N'Đã thanh toán');
SET IDENTITY_INSERT [dbo].[HoaDon] OFF;
INSERT INTO [dbo].[ChiTietHoaDon] ([idHoaDon], [idSanPham], [soLuong], [donGiaLucBan]) VALUES (2, 2, 1, 30000.00), (2, 1, 1, 25000.00);
GO

PRINT '*** THÊM DỮ LIỆU MẪU THÀNH CÔNG! ***'
GO