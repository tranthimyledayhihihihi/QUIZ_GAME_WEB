namespace QUIZ_GAME_WEB.Models.InputModels
{
    public class SocketMessage
    {
        // LOGIN | JOIN_MATCH | GET_QUESTIONS | SUBMIT_ANSWER | END_MATCH | PING
        public string Type { get; set; }

        public int UserId { get; set; }          // LOGIN
        public string MatchCode { get; set; }    // JOIN_MATCH
        public MatchAnswerModel Answer { get; set; } // SUBMIT_ANSWER
    }
}