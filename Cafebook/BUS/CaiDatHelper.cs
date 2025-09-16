using System.Configuration;
using System.Data.SqlClient;

namespace Cafebook.BUS
{
    public static class CaiDatHelper
    {
        private static string connectionString = ConfigurationManager.ConnectionStrings["MyConnectionString"].ConnectionString;

        public static decimal GetGiaTriDecimal(string tenCaiDat)
        {
            decimal giaTri = 0;
            string query = "SELECT giaTri FROM CaiDat WHERE tenCaiDat = @ten";
            using (var conn = new SqlConnection(connectionString))
            using (var cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@ten", tenCaiDat);
                conn.Open();
                var result = cmd.ExecuteScalar();
                if (result != null)
                {
                    decimal.TryParse(result.ToString(), out giaTri);
                }
            }
            return giaTri;
        }
    }
}