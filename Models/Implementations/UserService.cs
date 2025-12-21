// Models/Implementations/UserService.cs
using QUIZ_GAME_WEB.Models.CoreEntities;
using QUIZ_GAME_WEB.Models.Interfaces;

namespace QUIZ_GAME_WEB.Models.Implementations
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;

        public UserService(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        public async Task<NguoiDung?> GetUserByIdAsync(int userId)
        {
            return await _unitOfWork.Users.GetByIdAsync(userId);
        }

        public async Task<string> GetUserRoleAsync(int userId)
        {
            var role = await _unitOfWork.Users.GetRoleByUserIdAsync(userId);
            return role?.TenVaiTro ?? "Player";
        }
    }
}