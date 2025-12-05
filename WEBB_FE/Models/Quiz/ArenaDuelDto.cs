namespace WEBB.Models.Quiz
{
    public class ArenaRoomDto
    {
        public int RoomId { get; set; }
        public string Name { get; set; }
        public int CurrentPlayers { get; set; }
        public int MaxPlayers { get; set; }
        public string Status { get; set; }
    }

    public class DuelMatchDto
    {
        public string MatchId { get; set; }
        public string OpponentName { get; set; }
        public string Status { get; set; }
    }
}
