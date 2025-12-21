using QUIZ_GAME_WEB.Models.CoreEntities;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace QUIZ_GAME_WEB.Models.Interfaces
{
    /// <summary>
    /// Interface Repository cho VaiTro (Roles).
    /// </summary>
    public interface IRoleRepository : IGenericRepository<VaiTro>
    {
        /// <summary>
        /// Lấy thông tin chi tiết VaiTro, bao gồm các Quyền hạn (Permissions) liên quan.
        /// </summary>
        /// <param name="roleId">ID của Vai trò.</param>
        /// <returns>VaiTro Entity kèm theo các Quyen hạn.</returns>
        Task<VaiTro?> GetRoleWithPermissionsAsync(int roleId);
    }
}