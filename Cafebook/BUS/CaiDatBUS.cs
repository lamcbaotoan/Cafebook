using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;

namespace Cafebook.BUS
{
    public class CaiDatBUS
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["MyConnectionString"].ConnectionString;

        /// <summary>
        /// Lấy thông tin địa chỉ và SĐT của cửa hàng từ CSDL.
        /// </summary>
        /// <returns>Một Dictionary chứa địa chỉ và SĐT.</returns>
        public Dictionary<string, string> GetThongTinCuaHang()
        {
            var thongTin = new Dictionary<string, string>();
            string query = "SELECT tenCaiDat, giaTri FROM CaiDat WHERE tenCaiDat IN ('StoreAddress', 'StorePhoneNumber')";

            using (var conn = new SqlConnection(connectionString))
            using (var cmd = new SqlCommand(query, conn))
            {
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        thongTin[reader.GetString(0)] = reader.GetString(1);
                    }
                }
            }
            // Đảm bảo các key luôn tồn tại để tránh lỗi
            if (!thongTin.ContainsKey("StoreAddress")) thongTin["StoreAddress"] = "Chưa có địa chỉ";
            if (!thongTin.ContainsKey("StorePhoneNumber")) thongTin["StorePhoneNumber"] = "Chưa có SĐT";

            return thongTin;
        }

        /// <summary>
        /// Lưu thông tin địa chỉ và SĐT của cửa hàng vào CSDL.
        /// </summary>
        public bool LuuThongTinCuaHang(string diaChi, string soDienThoai)
        {
            // Sử dụng một câu lệnh để vừa UPDATE vừa INSERT nếu chưa có
            string query = @"
                -- Cập nhật địa chỉ
                IF EXISTS (SELECT 1 FROM CaiDat WHERE tenCaiDat = 'StoreAddress')
                    UPDATE CaiDat SET giaTri = @diaChi WHERE tenCaiDat = 'StoreAddress'
                ELSE
                    INSERT INTO CaiDat (tenCaiDat, giaTri) VALUES ('StoreAddress', @diaChi);
                
                -- Cập nhật SĐT
                IF EXISTS (SELECT 1 FROM CaiDat WHERE tenCaiDat = 'StorePhoneNumber')
                    UPDATE CaiDat SET giaTri = @sdt WHERE tenCaiDat = 'StorePhoneNumber'
                ELSE
                    INSERT INTO CaiDat (tenCaiDat, giaTri) VALUES ('StorePhoneNumber', @sdt);
            ";

            using (var conn = new SqlConnection(connectionString))
            using (var cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@diaChi", diaChi);
                cmd.Parameters.AddWithValue("@sdt", soDienThoai);
                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }
    }
}