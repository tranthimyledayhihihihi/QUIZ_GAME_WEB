// Models/ViewModels/ArenaViewModels.cs
using System.Collections.Generic;
using WEBB.Models.Quiz;

namespace WEBB.Models.ViewModels
{
    public class ArenaViewModel
    {
        public List<ArenaRoomDto> Rooms { get; set; } = new List<ArenaRoomDto>();
    }

    public class DuelLobbyViewModel
    {
        public DuelMatchDto CurrentMatch { get; set; } = new DuelMatchDto
        {
            Status = "Idle"
        };

        public bool IsWaiting { get; set; }
        public string Message { get; set; }
    }
}
