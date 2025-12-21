using QUIZ_GAME_WEB.Models.CoreEntities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QUIZ_GAME_WEB.Models.Interfaces
{
    /// <summary>
    /// Định nghĩa các nghiệp vụ liên quan đến Vai trò (Role) và Quyền hạn (Permission).
    /// </summary>
    public interface IRolePermissionService
    {
        // ===============================================
        // 1. TRUY VẤN (QUERY)
        // ===============================================

        /// <summary>
        /// Lấy Vai trò của một người dùng dựa trên User ID.
        /// </summary>
        /// <param name="userId">ID người dùng.</param>
        /// <returns>VaiTro Entity hoặc NULL nếu không tìm thấy.</returns>
        Task<VaiTro?> GetUserRoleAsync(int userId);

        /// <summary>
        /// Lấy tất cả Quyền hạn mà một người dùng có (dựa trên Vai trò của họ).
        /// </summary>
        /// <param name="userId">ID người dùng.</param>
        /// <returns>Danh sách Quyen hạn.</returns>
        Task<IEnumerable<Quyen>> GetPermissionsByUserIdAsync(int userId);

        /// <summary>
        /// Lấy tất cả Quyền hạn thuộc về một Vai trò cụ thể.
        /// </summary>
        /// <param name="roleId">ID Vai trò.</param>
        /// <returns>Danh sách Quyen hạn.</returns>
        Task<IEnumerable<Quyen>> GetPermissionsByRoleIdAsync(int roleId);

        // ===============================================
        // 2. TÁC VỤ (ACTIONS)
        // ===============================================

        /// <summary>
        /// Gán một Vai trò mới cho người dùng.
        /// </summary>
        /// <param name="userId">ID người dùng cần gán.</param>
        /// <param name="roleId">ID Vai trò mới.</param>
        /// <returns>True nếu thành công.</returns>
        Task<bool> AssignRoleToUserAsync(int userId, int roleId);

        /// <summary>
        /// Cập nhật (đồng bộ hóa) danh sách Quyền hạn cho một Vai trò.
        /// </summary>
        /// <param name="roleId">ID Vai trò cần cập nhật.</param>
        /// <param name="permissionIds">Danh sách các ID Quyền hạn mới.</param>
        /// <returns>True nếu thành công.</returns>
        Task<bool> UpdateRolePermissionsAsync(int roleId, IEnumerable<int> permissionIds);
    }
}