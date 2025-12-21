using QUIZ_GAME_WEB.Data;
using QUIZ_GAME_WEB.Models.CoreEntities;
using QUIZ_GAME_WEB.Models.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace QUIZ_GAME_WEB.Models.Implementations
{
    // Kế thừa GenericRepository<VaiTro> và triển khai IRoleRepository
    public class RoleRepository : GenericRepository<VaiTro>, IRoleRepository
    {
        // Sử dụng 'new' để giải quyết cảnh báo ẩn (_context)
        private new readonly QuizGameContext _context;

        public RoleRepository(QuizGameContext context) : base(context)
        {
            _context = context;
        }

        // Triển khai hàm đặc thù của IRoleRepository
        public async Task<VaiTro?> GetRoleWithPermissionsAsync(int roleId)
        {
            // Bao gồm (Include) các quyền hạn liên quan để trả về đầy đủ thông tin
            return await _context.VaiTros
                                 .Include(r => r.VaiTro_Quyens) // Giả định có Entity VaiTroQuyen nối VaiTro và Quyen
                                 .FirstOrDefaultAsync(r => r.VaiTroID == roleId);
        }
    }
}