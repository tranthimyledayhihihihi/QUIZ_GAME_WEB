using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WEBB_FE.Models.Admin
{
    public class PhanThuongDto
    {
        [JsonProperty("thuongID")]
        public int ThuongID { get; set; }

        [JsonProperty("userID")]
        public int UserID { get; set; }   // 👈 THÊM

        [JsonProperty("username")]
        public string Username { get; set; }

        public string PhanThuong { get; set; }
        public int DiemThuong { get; set; }
        public DateTime NgayNhan { get; set; }
        public bool TrangThaiNhan { get; set; }
    }


    public class PhanThuongCreateDto
    {
        [Required]
        public int UserID { get; set; }

        [Required]
        public string PhanThuong { get; set; }

        public int DiemThuong { get; set; }

        public bool TrangThaiNhan { get; set; }
    }

    public class PhanThuongIndexViewModel
    {
        public int Total { get; set; }
        public List<PhanThuongDto> Items { get; set; } = new List<PhanThuongDto>();
    }
}
