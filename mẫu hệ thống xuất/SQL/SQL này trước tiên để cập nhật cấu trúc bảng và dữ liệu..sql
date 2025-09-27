-- Thêm cột giaBia (dùng làm tiền cọc) vào bảng Sach
ALTER TABLE [dbo].[Sach]
ADD [giaBia] DECIMAL(18, 2) NOT NULL DEFAULT 0;
GO

-- Cập nhật giá bìa (tiền cọc) cho các sách hiện có (bạn có thể thay đổi giá trị này)
UPDATE [dbo].[Sach] SET [giaBia] = 150000 WHERE [idSach] = 1;
UPDATE [dbo].[Sach] SET [giaBia] = 120000 WHERE [idSach] = 2;
UPDATE [dbo].[Sach] SET [giaBia] = 80000 WHERE [idSach] = 3;
UPDATE [dbo].[Sach] SET [giaBia] = 100000 WHERE [idSach] = 4;
UPDATE [dbo].[Sach] SET [giaBia] = 200000 WHERE [idSach] = 5;
UPDATE [dbo].[Sach] SET [giaBia] = 250000 WHERE [idSach] = 6;
UPDATE [dbo].[Sach] SET [giaBia] = 300000 WHERE [idSach] = 7;
UPDATE [dbo].[Sach] SET [giaBia] = 95000 WHERE [idSach] = 8;
UPDATE [dbo].[Sach] SET [giaBia] = 180000 WHERE [idSach] = 9;
UPDATE [dbo].[Sach] SET [giaBia] = 160000 WHERE [idSach] = 10;
GO


-- Thêm các cột phí và cọc vào bảng PhieuThueSach
ALTER TABLE [dbo].[PhieuThueSach]
ADD [phiThue] DECIMAL(18, 2) NOT NULL DEFAULT 0,
    [tienCoc] DECIMAL(18, 2) NOT NULL DEFAULT 0;
GO

-- Chèn dữ liệu cài đặt (nếu bạn chưa có)
-- Xóa dữ liệu cũ để tránh trùng lặp nếu bạn chạy lại
DELETE FROM [dbo].[CaiDat] WHERE [tenCaiDat] IN (N'PhiPhatTreHan', N'PhiThueSach');
INSERT INTO [dbo].[CaiDat] ([tenCaiDat], [giaTri]) VALUES (N'PhiPhatTreHan', N'5000');
INSERT INTO [dbo].[CaiDat] ([tenCaiDat], [giaTri]) VALUES (N'PhiThueSach', N'10000');
GO