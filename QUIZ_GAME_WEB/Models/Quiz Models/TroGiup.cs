using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QUIZ_GAME_WEB.Models.QuizModels
{
    public class TroGiup
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TroGiupID { get; set; }

        [Required]
        [MaxLength(50)]
        public string TenTroGiup { get; set; } = null!;

        [MaxLength(255)]
        public string? MoTa { get; set; }
    }
}