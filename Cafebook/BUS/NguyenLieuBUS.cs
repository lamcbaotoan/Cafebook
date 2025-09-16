using Cafebook.DTO;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;

namespace Cafebook.BUS
{
    public class NguyenLieuBUS
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["MyConnectionString"].ConnectionString;

        /// <summary>
        /// Lấy toàn bộ danh sách nguyên liệu từ CSDL.
        /// </summary>
        public List<NguyenLieu> GetDanhSachNguyenLieu()
        {
            var ds = new List<NguyenLieu>();
            string query = "SELECT idNguyenLieu, tenNguyenLieu, donViTinh, soLuongTon, nguongCanhBao FROM NguyenLieu ORDER BY tenNguyenLieu";

            using (var conn = new SqlConnection(connectionString))
            using (var cmd = new SqlCommand(query, conn))
            {
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ds.Add(new NguyenLieu
                        {
                            IdNguyenLieu = (int)reader["idNguyenLieu"],
                            TenNguyenLieu = (string)reader["tenNguyenLieu"],
                            DonViTinh = (string)reader["donViTinh"],
                            SoLuongTon = (decimal)reader["soLuongTon"],
                            NguongCanhBao = (decimal)reader["nguongCanhBao"]
                        });
                    }
                }
            }
            return ds;
        }
    }
}