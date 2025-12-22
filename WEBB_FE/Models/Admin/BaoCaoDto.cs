using System;
using System.Collections.Generic;

namespace WEBB.Models.Admin
{
    public class BaoCaoDto
    {
        public List<GameByDateDto> GamesByDate { get; set; }
        public List<UserRegisterByDateDto> UserRegistrationByDate { get; set; }
        public List<TopWrongAnswerDto> TopWrongAnswers { get; set; }

        // Constructor cho C# 7.3
        public BaoCaoDto()
        {
            GamesByDate = new List<GameByDateDto>();
            UserRegistrationByDate = new List<UserRegisterByDateDto>();
            TopWrongAnswers = new List<TopWrongAnswerDto>();
        }
    }

    public class GameByDateDto
    {
        public DateTime Ngay { get; set; }
        public int SoTran { get; set; }
        public int TongDiem { get; set; }
        public int SoCauDung { get; set; }
    }

    public class UserRegisterByDateDto
    {
        public DateTime Ngay { get; set; }
        public int SoNguoiDungMoi { get; set; }
    }

    public class TopWrongAnswerDto
    {
        public int CauHoiID { get; set; }
        public int SoLanSai { get; set; }
        public string NoiDungCauHoi { get; set; }
    }
}
