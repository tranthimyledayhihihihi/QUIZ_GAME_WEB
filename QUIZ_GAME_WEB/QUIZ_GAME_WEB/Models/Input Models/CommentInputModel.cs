// Đặt file này tại [Tên Project]/Models/InputModels/CommentInputModel.cs

using System.ComponentModel.DataAnnotations;

namespace QUIZ_GAME_WEB.Models.InputModels
{
    public class CommentInputModel
    {
        [Required(ErrorMessage = "Loại đối tượng là bắt buộc.")]
        [MaxLength(50)]
        public required string EntityType { get; set; }

        [Required(ErrorMessage = "ID đối tượng là bắt buộc.")]
        public required int RelatedEntityID { get; set; }

        [Required(ErrorMessage = "Nội dung bình luận không được để trống.")]
        [MaxLength(500)]
        public required string NoiDung { get; set; }
    }
}