using QUIZ_GAME_WEB.Data;
using QUIZ_GAME_WEB.Models.CoreEntities;
using QUIZ_GAME_WEB.Models.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace QUIZ_GAME_WEB.Models.Implementations
{
    // Kế thừa GenericRepository<Quyen> và triển khai IPermissionRepository
    public class PermissionRepository : GenericRepository<Quyen>, IPermissionRepository
    {
        private new readonly QuizGameContext _context;

        public PermissionRepository(QuizGameContext context) : base(context)
        {
            _context = context;
        }

        // Triển khai hàm đồng bộ hóa Quyền hạn cho Vai trò
        public async Task<bool> SyncRolePermissionsAsync(int roleId, List<int> permissionIds)
        {
            var role = await _context.VaiTros
                                     .Include(r => r.VaiTro_Quyens)
                                     .FirstOrDefaultAsync(r => r.VaiTroID == roleId);

            if (role == null) return false;

            // Xóa các quyền cũ không còn trong danh sách mới
            var currentPermissions = role.VaiTro_Quyens.ToList();
            var permissionsToRemove = currentPermissions
                .Where(vp => !permissionIds.Contains(vp.QuyenID));

            _context.VaiTroQuyens.RemoveRange(permissionsToRemove);

            // Thêm các quyền mới chưa tồn tại
            var existingPermissionIds = currentPermissions.Select(vp => vp.QuyenID).ToList();
            var permissionsToAdd = permissionIds
                .Where(pId => !existingPermissionIds.Contains(pId))
                .Select(pId => new VaiTro_Quyen { VaiTroID = roleId, QuyenID = pId });

            await _context.VaiTroQuyens.AddRangeAsync(permissionsToAdd);

            // Không cần SaveChangesAsync ở đây, UnitOfWork sẽ gọi CompleteAsync sau
            return true;
        }
    }
}