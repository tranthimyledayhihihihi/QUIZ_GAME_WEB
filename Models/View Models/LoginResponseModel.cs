// Models/View Models/LoginResponseModel.cs
using System.ComponentModel.DataAnnotations;

namespace QUIZ_GAME_WEB.Models.ViewModels
{
    public class LoginResponseModel
    {
        [Required]
        public string Token { get; set; } = string.Empty;

        [Required]
        public string HoTen { get; set; } = string.Empty;

        [Required]
        public string VaiTro { get; set; } = string.Empty;
    }
}