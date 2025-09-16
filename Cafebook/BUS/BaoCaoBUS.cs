using Cafebook.DTO;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;

namespace Cafebook.BUS
{
    public class BaoCaoBUS
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["MyConnectionString"].ConnectionString;

        public bool XuatBaoCaoDoanhThu(string filePath, System.DateTime tuNgay, System.DateTime denNgay)
        {
            var data = new List<DoanhThu>();
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                var cmd = new SqlCommand(@"
                    SELECT hd.idHoaDon, nv.hoTen, b.soBan, hd.thoiGianTao, hd.tongTien, hd.soTienGiam, hd.thanhTien
                    FROM HoaDon hd
                    JOIN NhanVien nv ON hd.idNhanVien = nv.idNhanVien
                    JOIN Ban b ON hd.idBan = b.idBan
                    WHERE hd.trangThai = N'Đã thanh toán' AND CONVERT(date, hd.thoiGianTao) BETWEEN @tuNgay AND @denNgay
                    ORDER BY hd.thoiGianTao", conn);
                cmd.Parameters.AddWithValue("@tuNgay", tuNgay.Date);
                cmd.Parameters.AddWithValue("@denNgay", denNgay.Date);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        data.Add(new DoanhThu
                        {
                            MaHD = (int)reader["idHoaDon"],
                            NhanVien = reader["hoTen"].ToString(),
                            Ban = reader["soBan"].ToString(),
                            ThoiGian = (System.DateTime)reader["thoiGianTao"],
                            TongTien = (decimal)reader["tongTien"],
                            GiamGia = (decimal)reader["soTienGiam"],
                            ThanhTien = (decimal)reader["thanhTien"]
                        });
                    }
                }
            }

            using (var package = new ExcelPackage())
            {
                var ws = package.Workbook.Worksheets.Add($"DoanhThu_{tuNgay:ddMMyy}_{denNgay:ddMMyy}");
                ws.Cells["A1"].Value = $"BÁO CÁO DOANH THU TỪ {tuNgay:dd/MM/yyyy} ĐẾN {denNgay:dd/MM/yyyy}";
                ws.Cells["A1:G1"].Merge = true;
                ws.Cells["A1:G1"].Style.Font.Bold = true;
                ws.Cells["A1:G1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells["A3"].LoadFromCollection(data, true);
                ws.Cells["A3:G3"].Style.Font.Bold = true;
                ws.Cells["E:G"].Style.Numberformat.Format = "#,##0";
                ws.Cells.AutoFitColumns();
                package.SaveAs(new FileInfo(filePath));
            }
            return true;
        }

        public bool XuatBaoCaoTonKhoSach(string filePath)
        {
            List<Sach> tonKho = new SachBUS().GetDanhSachSach();
            using (var package = new ExcelPackage())
            {
                var ws = package.Workbook.Worksheets.Add("TonKhoSach");
                ws.Cells["A1"].LoadFromCollection(tonKho, true);
                using (var range = ws.Cells[1, 1, 1, ws.Dimension.End.Column])
                {
                    range.Style.Font.Bold = true;
                }
                ws.Cells["F:H"].Style.Numberformat.Format = "#,##0";
                ws.Cells.AutoFitColumns();
                package.SaveAs(new FileInfo(filePath));
            }
            return true;
        }

        // THÊM HÀM MỚI NÀY
        public bool XuatBaoCaoTonKhoNguyenLieu(string filePath)
        {
            List<NguyenLieu> tonKho = new NguyenLieuBUS().GetDanhSachNguyenLieu();
            using (var package = new ExcelPackage())
            {
                var ws = package.Workbook.Worksheets.Add("TonKhoNguyenLieu");

                // Thêm tiêu đề cho báo cáo
                ws.Cells["A1"].Value = "BÁO CÁO TỒN KHO NGUYÊN LIỆU";
                ws.Cells["A1:E1"].Merge = true;
                ws.Cells["A1:E1"].Style.Font.Bold = true;
                ws.Cells["A1:E1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                // Load dữ liệu từ dòng 3
                ws.Cells["A3"].LoadFromCollection(tonKho, true);

                // Style cho header
                using (var range = ws.Cells["A3:E3"])
                {
                    range.Style.Font.Bold = true;
                }
                // Format cột số
                ws.Cells["D:E"].Style.Numberformat.Format = "#,##0.00";
                ws.Cells.AutoFitColumns();

                package.SaveAs(new FileInfo(filePath));
            }
            return true;
        }

        // === PHƯƠNG THỨC ĐÃ ĐƯỢC CẬP NHẬT ===
        public bool XuatBaoCaoHieuSuatNV(string filePath, System.DateTime tuNgay, System.DateTime denNgay)
        {
            // SỬA ĐỔI: Sử dụng List<HieuSuatNhanVien>
            var data = new List<HieuSuatNhanVien>();
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                var cmd = new SqlCommand(@"
                    SELECT nv.hoTen, COUNT(hd.idHoaDon) AS SoDonHang, SUM(hd.thanhTien) AS TongDoanhThu
                    FROM HoaDon hd
                    JOIN NhanVien nv ON hd.idNhanVien = nv.idNhanVien
                    WHERE hd.trangThai = N'Đã thanh toán' AND CONVERT(date, hd.thoiGianTao) BETWEEN @tuNgay AND @denNgay
                    GROUP BY nv.hoTen
                    ORDER BY TongDoanhThu DESC", conn);
                cmd.Parameters.AddWithValue("@tuNgay", tuNgay.Date);
                cmd.Parameters.AddWithValue("@denNgay", denNgay.Date);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        // SỬA ĐỔI: Tạo đối tượng HieuSuatNhanVien
                        data.Add(new HieuSuatNhanVien
                        {
                            TenNhanVien = reader["hoTen"].ToString(),
                            SoDonHang = (int)reader["SoDonHang"],
                            TongDoanhThu = (decimal)reader["TongDoanhThu"]
                        });
                    }
                }
            }

            using (var package = new ExcelPackage())
            {
                var ws = package.Workbook.Worksheets.Add("HieuSuatNhanVien");
                ws.Cells["A1"].Value = $"BÁO CÁO HIỆU SUẤT NHÂN VIÊN TỪ {tuNgay:dd/MM/yyyy} ĐẾN {denNgay:dd/MM/yyyy}";
                ws.Cells["A1:C1"].Merge = true;
                ws.Cells["A1:C1"].Style.Font.Bold = true;
                ws.Cells["A1:C1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                ws.Cells["A3"].LoadFromCollection(data, true);
                ws.Cells["A3:C3"].Style.Font.Bold = true;
                ws.Cells["C:C"].Style.Numberformat.Format = "#,##0";
                ws.Cells.AutoFitColumns();

                package.SaveAs(new FileInfo(filePath));
            }
            return true;
        }
    }
}