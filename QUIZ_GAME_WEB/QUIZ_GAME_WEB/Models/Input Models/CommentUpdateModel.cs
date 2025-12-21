// Đặt file này tại [Tên Project]/Models/InputModels/CommentUpdateModel.cs

using System.ComponentModel.DataAnnotations;

namespace QUIZ_GAME_WEB.Models.InputModels
{
    /// <summary>
    /// Input Model dùng để nhận dữ liệu khi người dùng yêu cầu cập nhật bình luận.
    /// </summary>
    public class CommentUpdateModel
    {
        [Required(ErrorMessage = "Nội dung bình luận không được để trống.")]
        [MaxLength(500, ErrorMessage = "Nội dung không được vượt quá 500 ký tự.")]
        public required string NoiDung { get; set; }
    }
}