// Models/Implementations/ApiKeyValidationService.cs

using QUIZ_GAME_WEB.Models.Interfaces;
using System.Threading.Tasks;
using System;
// Cần thêm using QUIZ_GAME_WEB.Models.CoreEntities; nếu ClientKey nằm ở đó

namespace QUIZ_GAME_WEB.Models.Implementations
{
    public class ApiKeyValidationService : IApiKeyValidationService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ApiKeyValidationService(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        public async Task<bool> IsValidApiKey(string userApiKey)
        {
            // 1. GỌI REPOSITORY MỘT LẦN VÀ LƯU KẾT QUẢ VÀO BIẾN CỤC BỘ
            // Đã sửa tên hàm để đồng bộ với IClientKeyRepository
            var clientKey = await _unitOfWork.ClientKeys.GetKeyByValueAsync(userApiKey); // 👈 ĐÃ SỬA TÊN

            // 2. KIỂM TRA TỒN TẠI VÀ TRẠNG THÁI ACTIVE
            if (clientKey == null || !clientKey.IsActive)
            {
                return false;
            }

            // 3. KIỂM TRA NGÀY HẾT HẠN (Sử dụng biến cục bộ)
            if (clientKey.NgayHetHan.HasValue && clientKey.NgayHetHan.Value < DateTime.Now)
            {
                return false; // Key đã hết hạn
            }

            // Nếu vượt qua tất cả các kiểm tra
            return true;
        }
    }
}