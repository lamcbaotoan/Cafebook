using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// BUS/ThongKeBUS.cs
using System.Configuration;
using System.Data.SqlClient;

namespace Cafebook.BUS
{
    public class ThongKeBUS
    {
        private string connectionString;

        public ThongKeBUS()
        {
            connectionString = ConfigurationManager.ConnectionStrings["MyConnectionString"].ConnectionString;
        }

        // Lấy tổng doanh thu trong ngày hôm nay
        public decimal GetDoanhThuHomNay()
        {
            decimal doanhThu = 0;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                // Lấy tổng thành tiền của các hóa đơn đã thanh toán trong ngày
                string query = "SELECT SUM(thanhTien) FROM HoaDon WHERE trangThai = N'Đã thanh toán' AND CONVERT(date, thoiGianTao) = CONVERT(date, GETDATE())";
                SqlCommand cmd = new SqlCommand(query, conn);
                object result = cmd.ExecuteScalar();
                if (result != DBNull.Value)
                {
                    doanhThu = Convert.ToDecimal(result);
                }
            }
            return doanhThu;
        }

        // HÀM MỚI: Lấy dữ liệu doanh thu cho biểu đồ 30 ngày qua
        public Dictionary<DateTime, decimal> GetDoanhThu30NgayQua()
        {
            var data = new Dictionary<DateTime, decimal>();
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                // Lấy dữ liệu từ 30 ngày trước cho đến hôm nay
                string query = @"
                    SELECT CONVERT(date, thoiGianTao) AS Ngay, SUM(thanhTien) AS TongDoanhThu
                    FROM HoaDon
                    WHERE trangThai = N'Đã thanh toán' 
                      AND thoiGianTao >= DATEADD(day, -29, GETDATE())
                    GROUP BY CONVERT(date, thoiGianTao)
                    ORDER BY Ngay ASC";
                var cmd = new SqlCommand(query, conn);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        data[(DateTime)reader["Ngay"]] = (decimal)reader["TongDoanhThu"];
                    }
                }
            }
            return data;
        }

        // Lấy tổng số đơn hàng trong ngày hôm nay
        public int GetSoDonHangHomNay()
        {
            int soDonHang = 0;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT COUNT(idHoaDon) FROM HoaDon WHERE CONVERT(date, thoiGianTao) = CONVERT(date, GETDATE())";
                SqlCommand cmd = new SqlCommand(query, conn);
                object result = cmd.ExecuteScalar();
                if (result != DBNull.Value)
                {
                    soDonHang = Convert.ToInt32(result);
                }
            }
            return soDonHang;
        }

        // Lấy tên sản phẩm bán chạy nhất trong ngày
        public string GetSanPhamBanChayNhatHomNay()
        {
            string tenSP = "Chưa có";
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = @"
                    SELECT TOP 1 SP.tenSanPham
                    FROM ChiTietHoaDon CTDH
                    JOIN HoaDon HD ON CTDH.idHoaDon = HD.idHoaDon
                    JOIN SanPham SP ON CTDH.idSanPham = SP.idSanPham
                    WHERE CONVERT(date, HD.thoiGianTao) = CONVERT(date, GETDATE())
                    GROUP BY SP.tenSanPham
                    ORDER BY SUM(CTDH.soLuong) DESC";
                SqlCommand cmd = new SqlCommand(query, conn);
                object result = cmd.ExecuteScalar();
                if (result != DBNull.Value && result != null)
                {
                    tenSP = result.ToString();
                }
            }
            return tenSP;
        }


    }
}