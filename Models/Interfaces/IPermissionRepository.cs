using QUIZ_GAME_WEB.Models.CoreEntities;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace QUIZ_GAME_WEB.Models.Interfaces
{
    /// <summary>
    /// Interface Repository cho Quyền hạn (Permissions).
    /// </summary>
    // ✅ Kế thừa từ IGenericRepository<Quyen> (Giả định Entity Quyen tồn tại)
    public interface IPermissionRepository : IGenericRepository<Quyen>
    {
        /// <summary>
        /// Đồng bộ hóa (thêm/xóa) danh sách Quyền hạn (Permission) cho một Vai trò (Role).
        /// </summary>
        /// <param name="roleId">ID Vai trò.</param>
        /// <param name="permissionIds">Danh sách các ID Quyền hạn mới cần gán.</param>
        /// <returns>True nếu thành công.</returns>
        Task<bool> SyncRolePermissionsAsync(int roleId, List<int> permissionIds);
    }
}