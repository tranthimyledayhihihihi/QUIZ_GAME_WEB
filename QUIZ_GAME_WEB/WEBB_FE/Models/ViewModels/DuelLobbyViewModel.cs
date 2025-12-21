using WEBB.Models.Quiz;

namespace WEBB.ViewModels
{
    public class DuelLobbyViewModel
    {
        public DuelMatchDto CurrentMatch { get; set; }
        public bool IsWaiting { get; set; }
        public string Message { get; set; }
    }
}
