using System;

namespace WEBB.Models.ViewModels
{
    public class DailyStreakViewModel
    {
        // Số ngày liên tiếp
        public int SoNgayLienTiep { get; set; }

        // Lần cập nhật chuỗi gần nhất (từ BE trả về)
        public DateTime? NgayCapNhatCuoi { get; set; }

        // Thông báo hiển thị cho user
        public string Message { get; set; }
    }
}
