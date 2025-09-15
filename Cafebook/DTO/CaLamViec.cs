using System;
namespace Cafebook.DTO
{
    public class CaLamViec
    {
        public int IdCa { get; set; }
        public string TenCa { get; set; }
        public TimeSpan GioBatDau { get; set; }
        public TimeSpan GioKetThuc { get; set; }
    }
}