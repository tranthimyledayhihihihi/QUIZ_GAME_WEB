using System;

namespace WEBB.Models.Admin
{
    public class AdminSessionDto
    {
        public int SessionID { get; set; }
        public int UserID { get; set; }

        public string Username { get; set; }
        public DateTime ThoiGianBatDau { get; set; }
        public DateTime? ThoiGianKetThuc { get; set; }

        public bool IsActive { get; set; }

        public string IPAddress { get; set; }
        public string DeviceType { get; set; }

        public AdminSessionDto()
        {
            Username = string.Empty;
            IPAddress = string.Empty;
            DeviceType = string.Empty;
        }
    }
}
