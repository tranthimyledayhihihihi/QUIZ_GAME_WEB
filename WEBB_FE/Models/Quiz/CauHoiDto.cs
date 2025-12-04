using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WEBB.Models.Quiz
{
    public class CauHoiDto
    {
        public int CauHoiID { get; set; }
        public string NoiDung { get; set; }
        public string DapAnA { get; set; }
        public string DapAnB { get; set; }
        public string DapAnC { get; set; }
        public string DapAnD { get; set; }
        // Lưu ý: Frontend không cần DapAnDung nếu bạn muốn bảo mật
        // Nhưng nếu muốn chấm điểm tại chỗ thì cứ để.
        public int ChuDeID { get; set; }
        public string TenChuDe { get; set; } // Nếu API có trả về tên chủ đề
    }
}