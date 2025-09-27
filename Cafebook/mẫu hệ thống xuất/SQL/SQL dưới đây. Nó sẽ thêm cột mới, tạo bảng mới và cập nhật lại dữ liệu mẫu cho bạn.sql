-- Bước 1: Thêm cột vị trí vào bảng Sach
ALTER TABLE Sach
ADD viTri NVARCHAR(100) NULL;
GO

-- Bước 2: Tạo bảng để lưu trữ các cài đặt của quán
CREATE TABLE CaiDat (
    tenCaiDat VARCHAR(50) PRIMARY KEY,
    giaTri NVARCHAR(255) NOT NULL
);
GO

-- Bước 3: Thiết lập giá trị mặc định cho chính sách thuê sách
INSERT INTO CaiDat (tenCaiDat, giaTri) VALUES
('PhiThueSach', '10000'), -- Phí thuê mỗi lượt
('PhiPhatTreHan', '5000'); -- Phí phạt mỗi ngày trễ
GO

-- Bước 4: Xóa dữ liệu sách cũ và thêm 10 cuốn sách mẫu mới
DELETE FROM PhieuThueSach;
DELETE FROM Sach;
GO


DBCC CHECKIDENT ('[dbo].[Sach]', RESEED, 0);
GO

SET IDENTITY_INSERT [dbo].[Sach] ON
INSERT INTO [dbo].[Sach] ([idSach], [tieuDe], [tacGia], [theLoai], [viTri], [tongSoLuong], [soLuongCoSan]) VALUES
(1, N'Đắc Nhân Tâm', N'Dale Carnegie', N'Kỹ năng sống', N'Kệ A1', 5, 4),
(2, N'Nhà Giả Kim', N'Paulo Coelho', N'Tiểu thuyết', N'Kệ A2', 3, 2),
(3, N'Dế Mèn Phiêu Lưu Ký', N'Tô Hoài', N'Thiếu nhi', N'Kệ B1', 10, 8),
(4, N'Tôi thấy hoa vàng trên cỏ xanh', N'Nguyễn Nhật Ánh', N'Văn học Việt Nam', N'Kệ B1', 7, 7),
(5, N'Tội ác và hình phạt', N'Fyodor Dostoevsky', N'Văn học kinh điển', N'Kệ C3', 2, 1),
(6, N'Harry Potter và Hòn đá Phù thủy', N'J.K. Rowling', N'Fantasy', N'Kệ B2', 8, 8),
(7, N'Lược sử loài người', N'Yuval Noah Harari', N'Lịch sử', N'Kệ A3', 4, 4),
(8, N'Cafe cùng Tony', N'Tony Buổi Sáng', N'Truyền cảm hứng', N'Quầy Bar', 6, 5),
(9, N'Bố già', N'Mario Puzo', N'Tiểu thuyết', N'Kệ C3', 3, 3),
(10, N'Rừng Na Uy', N'Haruki Murakami', N'Văn học Nhật Bản', N'Kệ A2', 4, 3);
SET IDENTITY_INSERT [dbo].[Sach] OFF
GO