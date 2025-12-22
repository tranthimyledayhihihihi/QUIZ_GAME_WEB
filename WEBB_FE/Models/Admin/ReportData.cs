// WEBB/Models/Admin/ReportData.cs
using System;
using System.Collections.Generic;

namespace WEBB.Models.Admin
{
    public class ReportData
    {
        public List<GameStat> GamesByDate { get; set; } = new List<GameStat>();
        public List<UserRegistrationStat> UserRegistrationByDate { get; set; } = new List<UserRegistrationStat>();
        public List<WrongAnswerStat> TopWrongAnswers { get; set; } = new List<WrongAnswerStat>();
    }

    public class GameStat
    {
        public DateTime Ngay { get; set; }
        public int SoTran { get; set; }
        public int TongDiem { get; set; }
        public int SoCauDung { get; set; }
    }

    public class UserRegistrationStat
    {
        public DateTime Ngay { get; set; }
        public int SoNguoiDungMoi { get; set; }
    }

    public class WrongAnswerStat
    {
        public int CauHoiID { get; set; }
        public int SoLanSai { get; set; }
        public string NoiDungCauHoi { get; set; }
    }
}