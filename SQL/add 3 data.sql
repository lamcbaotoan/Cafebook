-- ===================================================================
-- ===           SCRIPT THEM DU LIEU MAU (3 DONG/BANG)             ===
-- ===================================================================
-- Ghi chu: Script gia dinh rang ID cua cac bang bat dau tu 1
--          va tu dong tang.
-- ===================================================================

-- =============================================
-- ===         NHOM BANG CO BAN (KHONG PHU THUOC)
-- =============================================

-- Bang: VaiTro
INSERT INTO VaiTro (tenVaiTro) VALUES
(N'Quản lý'),
(N'Nhân viên pha chế'),
(N'Nhân viên phục vụ');
GO

-- Bang: CaLamViec
INSERT INTO CaLamViec (tenCa, gioBatDau, gioKetThuc) VALUES
(N'Ca Sáng', '07:00:00', '15:00:00'),
(N'Ca Chiều', '15:00:00', '23:00:00'),
(N'Ca Tối (Part-time)', '18:00:00', '22:00:00');
GO

-- Bang: LoaiSanPham
INSERT INTO LoaiSanPham (tenLoaiSP) VALUES
(N'Cà phê'),
(N'Trà sữa'),
(N'Bánh ngọt');
GO

-- Bang: NguyenLieu
INSERT INTO NguyenLieu (tenNguyenLieu, donViTinh, soLuongTon) VALUES
(N'Hạt cà phê Robusta', N'kg', 10.0),
(N'Sữa tươi không đường', N'lít', 20.0),
(N'Bột Matcha Nhật Bản', N'g', 500.0);
GO

-- Bang: NhaCungCap
INSERT INTO NhaCungCap (tenNhaCungCap, soDienThoai, diaChi) VALUES
(N'Nông sản Đà Lạt Xanh', '0905111222', N'Lâm Đồng'),
(N'Công ty TNHH Baker''s Land', '0987654321', N'TP. Hồ Chí Minh'),
(N'Trang trại bò sữa Vinamilk', '0283456789', N'Bình Dương');
GO

-- Bang: Ban
INSERT INTO Ban (soBan, soGhe, trangThai) VALUES
(N'A1', 2, N'Trống'),
(N'A2', 4, N'Trống'),
(N'B1 (Ngoài trời)', 4, N'Trống');
GO

-- Bang: KhuyenMai
INSERT INTO KhuyenMai (tenKhuyenMai, moTa, loaiGiamGia, giaTriGiam, ngayBatDau, ngayKetThuc) VALUES
(N'Giảm giá Thứ Ba Vui Vẻ', N'Giảm 10% tổng hóa đơn vào các ngày Thứ Ba', 'PhanTram', 10.00, '2025-01-01', '2025-12-31'),
(N'Mua 1 Tặng 1', N'Áp dụng cho dòng Trà Sữa size M', 'SoTien', 45000.00, '2025-09-01', '2025-09-30'),
(N'Giảm 20k cho hóa đơn trên 100k', N'Giảm trực tiếp 20.000 VNĐ', 'SoTien', 20000.00, '2025-09-15', '2025-10-15');
GO

-- Bang: Sach
INSERT INTO Sach (tieuDe, tacGia, theLoai, moTa, tongSoLuong, soLuongCoSan) VALUES
(N'Đắc Nhân Tâm', N'Dale Carnegie', N'Kỹ năng sống', N'Nghệ thuật đối nhân xử thế', 5, 5),
(N'Nhà Giả Kim', N'Paulo Coelho', N'Tiểu thuyết', N'Chuyến phiêu lưu của Santiago', 3, 2),
(N'Dế Mèn Phiêu Lưu Ký', N'Tô Hoài', N'Thiếu nhi', N'Kinh điển văn học Việt Nam', 10, 8);
GO

-- Bang: KhachHang
INSERT INTO KhachHang (hoTen, soDienThoai, ngayTao) VALUES
(N'Nguyễn Văn An', '0912345678', GETDATE()),
(N'Trần Thị Bích', '0988776655', GETDATE()),
(N'Lê Minh Cường', '0905998877', GETDATE());
GO

-- =============================================
-- ===         NHOM BANG PHU THUOC             ===
-- =============================================

-- Bang: NhanVien
-- Ghi chu: matKhau trong thuc te nen duoc ma hoa (hashed)
INSERT INTO NhanVien (idVaiTro, hoTen, soDienThoai, email, diaChi, matKhau, ngayVaoLam, trangThai) VALUES
(1, N'Trần Hoàng Anh', '0905123456', 'hoanganh.manager@email.com', N'123 Lê Duẩn, Đà Nẵng', 'admin123', '2024-01-15', 1),
(2, N'Lê Thị Yến', '0905654321', 'yen.barista@email.com', N'45 Nguyễn Văn Linh, Đà Nẵng', 'staff123', '2024-03-20', 1),
(3, N'Nguyễn Văn Bình', '0905789012', 'binh.waiter@email.com', N'78 Hùng Vương, Đà Nẵng', 'staff456', '2025-02-10', 1);
GO

-- Bang: SanPham
INSERT INTO SanPham (idLoaiSP, tenSanPham, moTa, donGia, hinhAnh, trangThai) VALUES
(1, N'Cà phê Đen đá', N'Cà phê Robusta rang xay nguyên chất', 25000.00, 'images/capheden.jpg', N'Đang bán'),
(2, N'Trà sữa Matcha Đậu đỏ', N'Trà xanh Nhật Bản kết hợp đậu đỏ', 45000.00, 'images/matchadaudo.jpg', N'Đang bán'),
(3, N'Bánh Tiramisu', N'Bánh phô mai cà phê kiểu Ý', 35000.00, 'images/tiramisu.jpg', N'Đang bán');
GO

-- Bang: LichLamViec
INSERT INTO LichLamViec (idNhanVien, idCa, ngayLam) VALUES
(1, 1, '2025-09-15'), -- Quản lý làm ca sáng
(2, 1, '2025-09-15'), -- Pha chế làm ca sáng
(3, 2, '2025-09-15'); -- Phục vụ làm ca chiều
GO

-- Bang: BangChamCong
-- Giả sử hôm nay là ngày 15/09/2025
INSERT INTO BangChamCong (idLichLamViec, gioVao, gioRa, soGioLam) VALUES
(1, '2025-09-15 06:58:00', '2025-09-15 15:05:00', 8.0),
(2, '2025-09-15 07:00:00', '2025-09-15 15:01:00', 8.0),
(3, '2025-09-15 14:55:00', NULL, NULL); -- Nhân viên này vẫn đang trong ca
GO

-- Bang: CongThuc
INSERT INTO CongThuc (idSanPham, idNguyenLieu, luongCanThiet) VALUES
(1, 1, 0.02),  -- 1 ly Cà phê Đen đá cần 0.02 kg (20g) Hạt cà phê
(2, 2, 0.1),   -- 1 ly Trà sữa Matcha cần 0.1 lít (100ml) Sữa tươi
(2, 3, 0.01);  -- 1 ly Trà sữa Matcha cần 0.01 kg (10g) Bột Matcha
GO

-- Bang: PhieuNhapKho
INSERT INTO PhieuNhapKho (idNhanVien, idNhaCungCap, ngayNhap, tongTien) VALUES
(1, 1, '2025-09-10 10:00:00', 1500000.00), -- Nhập hạt cà phê
(1, 3, '2025-09-11 11:30:00', 800000.00),  -- Nhập sữa tươi
(1, 2, '2025-09-12 09:00:00', 450000.00);   -- Nhập bột matcha (từ nhà cung cấp Baker's Land)
GO

-- Bang: ChiTietPhieuNhap
INSERT INTO ChiTietPhieuNhap (idPhieuNhap, idNguyenLieu, soLuong, donGia) VALUES
(1, 1, 10.0, 150000.00),
(2, 2, 40.0, 20000.00),
(3, 3, 1.0, 450000.00);
GO

-- Bang: HoaDonDauVao
INSERT INTO HoaDonDauVao (idPhieuNhap, maHoaDon, ngayPhatHanh, duongDanFile) VALUES
(1, N'HD-NCC1-001', '2025-09-10', 'invoices/ncc1_001.pdf'),
(2, N'HD-NCC3-056', '2025-09-11', 'invoices/ncc3_056.pdf'),
(3, N'HD-NCC2-102', '2025-09-12', 'invoices/ncc2_102.pdf');
GO

-- Bang: PhieuDatBan
INSERT INTO PhieuDatBan (idKhachHang, tenKhachVangLai, sdtKhachVangLai, idBan, thoiGianDat, soLuongKhach, ghiChu, trangThai) VALUES
(1, NULL, NULL, 3, '2025-09-16 19:00:00', 2, N'Bàn gần cửa sổ', N'Đã xác nhận'),
(NULL, N'Chị Hương', '0905112233', 2, '2025-09-17 12:00:00', 4, N'Cần ghế cho trẻ em', N'Đã xác nhận'),
(2, NULL, NULL, 1, '2025-09-18 09:00:00', 1, N'Bàn yên tĩnh để làm việc', N'Chờ xác nhận');
GO

-- Bang: HoaDon
-- Giả sử có 3 giao dịch đã và đang diễn ra
INSERT INTO HoaDon (idKhachHang, idNhanVien, idBan, idKhuyenMai, thoiGianTao, tongTien, soTienGiam, thanhTien, trangThai) VALUES
(1, 2, 1, NULL, '2025-09-15 08:30:00', 60000.00, 0, 60000.00, N'Đã thanh toán'),
(NULL, 3, 2, 3, '2025-09-15 15:30:00', 115000.00, 20000.00, 95000.00, N'Đã thanh toán'),
(2, 3, 2, NULL, '2025-09-15 16:00:00', 45000.00, 0, 45000.00, N'Chưa thanh toán');
GO

-- Bang: ChiTietHoaDon
INSERT INTO ChiTietHoaDon (idHoaDon, idSanPham, soLuong, donGiaLucBan) VALUES
(1, 1, 1, 25000.00), -- Hóa đơn 1 có 1 Cà phê
(1, 3, 1, 35000.00), -- Hóa đơn 1 có 1 Tiramisu
(2, 2, 2, 45000.00), -- Hóa đơn 2 có 2 Trà sữa
(2, 3, 1, 35000.00); -- Hóa đơn 2 có 1 Tiramisu, tổng 115k
GO
-- Lưu ý: Hóa đơn 3 chưa có chi tiết vì khách mới ngồi vào bàn, chưa gọi món. Hoặc ta có thể thêm chi tiết:
INSERT INTO ChiTietHoaDon (idHoaDon, idSanPham, soLuong, donGiaLucBan) VALUES
(3, 2, 1, 45000.00); -- Hóa đơn 3 có 1 Trà sữa
GO

-- Bang: PhieuThueSach
INSERT INTO PhieuThueSach (idSach, idKhachHang, idNhanVien, ngayThue, ngayHenTra, ngayTraThucTe, tienPhat, trangThai) VALUES
(2, 1, 1, '2025-09-14 09:00:00', '2025-09-21 09:00:00', NULL, 0, N'Đang thuê'), -- Sách Nhà Giả Kim đang được thuê
(3, 2, 2, '2025-09-01 10:00:00', '2025-09-08 10:00:00', '2025-09-08 11:00:00', 0, N'Đã trả'), -- Sách Dế Mèn đã được trả
(3, 3, 2, '2025-09-10 14:00:00', '2025-09-17 14:00:00', NULL, 0, N'Đang thuê'); -- Sách Dế Mèn lại được người khác thuê
GO