namespace QUIZ_GAME_WEB.Models.ResultModels
{
    public class SocketResponse
    {
        public string Type { get; set; }     // LOGIN_SUCCESS, QUESTIONS, SCORE_UPDATE...
        public string Status { get; set; }   // success | error
        public string Message { get; set; }
        public object Data { get; set; }
    }
}