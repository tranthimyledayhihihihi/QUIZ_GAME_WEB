// Models/Implementations/ContentManagementService.cs (ĐÃ SỬA LỖI)
using QUIZ_GAME_WEB.Models.Interfaces;
using QUIZ_GAME_WEB.Models.InputModels;
using QUIZ_GAME_WEB.Models.QuizModels;
using System.Threading.Tasks;
using System; // Cần thiết cho DateTime.Now

namespace QUIZ_GAME_WEB.Models.Implementations
{
    public class ContentManagementService : IContentManagementService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ContentManagementService(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        // TẠO CÂU HỎI
        public async Task<int> CreateQuestionAsync(QuizCreateEditModel model)
        {
            // Logic 1: Map từ Model sang Entity CauHoi (Các thuộc tính đã được bổ sung vào CauHoi.cs)
            var cauHoi = new CauHoi
            {
                NoiDung = model.NoiDung,
                HinhAnh = model.HinhAnh,
                DapAnA = model.A, // KHẮC PHỤC LỖI NÀY
                DapAnB = model.B,
                DapAnC = model.C,
                DapAnD = model.D,
                DapAnDung = model.DapAnDung,
                ChuDeID = model.ChuDeID,
                DoKhoID = model.DoKhoID,
                NgayTao = DateTime.Now
            };

            // Logic 2: Thêm vào Repository và lưu DB
            _unitOfWork.Quiz.Add(cauHoi); // Dùng Generic Add
            await _unitOfWork.CompleteAsync();
            return cauHoi.CauHoiID;
        }

        // CẬP NHẬT CÂU HỎI
        public async Task<bool> UpdateQuestionAsync(int questionId, QuizCreateEditModel model)
        {
            var cauHoi = await _unitOfWork.Quiz.GetByIdAsync(questionId);
            if (cauHoi == null) return false;

            // Logic: Cập nhật các trường từ Model
            cauHoi.NoiDung = model.NoiDung;
            cauHoi.HinhAnh = model.HinhAnh;
            cauHoi.DapAnA = model.A; // KHẮC PHỤC LỖI NÀY
            cauHoi.DapAnB = model.B;
            cauHoi.DapAnC = model.C;
            cauHoi.DapAnD = model.D;
            cauHoi.DapAnDung = model.DapAnDung;
            cauHoi.ChuDeID = model.ChuDeID;
            cauHoi.DoKhoID = model.DoKhoID;

            _unitOfWork.Quiz.Update(cauHoi); // Dùng Generic Update
            await _unitOfWork.CompleteAsync();
            return true;
        }

        // XÓA CÂU HỎI
        public async Task<bool> DeleteQuestionAsync(int questionId)
        {
            var cauHoi = await _unitOfWork.Quiz.GetByIdAsync(questionId);
            if (cauHoi == null) return false;

            _unitOfWork.Quiz.Delete(cauHoi); // Dùng Generic Delete
            await _unitOfWork.CompleteAsync();
            return true;
        }

        // TẠO CHỦ ĐỀ
        public async Task<ChuDe> CreateTopicAsync(ChuDe topic)
        {
            // Logic: Gọi hàm AddTopic đã được bổ sung vào IQuizRepository
            _unitOfWork.Quiz.AddTopic(topic);
            await _unitOfWork.CompleteAsync();
            return topic;
        }
    }
}