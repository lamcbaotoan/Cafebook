ALTER TABLE KhuyenMai
ADD giaTriDonHangToiThieu DECIMAL(18, 2) NULL;

ALTER TABLE KhuyenMai
ADD idSanPhamApDung INT NULL;

ALTER TABLE KhuyenMai
ADD FOREIGN KEY (idSanPhamApDung) REFERENCES SanPham(idSanPham);
GO