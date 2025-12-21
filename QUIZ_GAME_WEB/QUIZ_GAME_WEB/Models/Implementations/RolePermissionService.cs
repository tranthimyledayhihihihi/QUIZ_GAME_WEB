// Models/Implementations/RolePermissionService.cs
using Microsoft.EntityFrameworkCore;
using QUIZ_GAME_WEB.Data;
using QUIZ_GAME_WEB.Models.CoreEntities;
using QUIZ_GAME_WEB.Models.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QUIZ_GAME_WEB.Models.Implementations
{
    public class RolePermissionService : IRolePermissionService
    {
        private readonly QuizGameContext _context;

        public RolePermissionService(QuizGameContext context)
        {
            _context = context;
        }

        // ==========================
        // 1. LẤY ROLE CỦA USER
        // ==========================
        public async Task<VaiTro?> GetUserRoleAsync(int userId)
        {
            // Admin 1:1 với NguoiDung, có VaiTroID 
            var admin = await _context.Admins
                                      .Include(a => a.VaiTro)
                                      .FirstOrDefaultAsync(a => a.UserID == userId && a.TrangThai);

            return admin?.VaiTro;
        }

        // ==========================
        // 2. LẤY QUYỀN CỦA USER
        // ==========================
        public async Task<IEnumerable<Quyen>> GetPermissionsByUserIdAsync(int userId)
        {
            var role = await GetUserRoleAsync(userId);
            if (role == null)
                return Enumerable.Empty<Quyen>();

            return await GetPermissionsByRoleIdAsync(role.VaiTroID);
        }

        // ==========================
        // 3. LẤY QUYỀN THEO ROLE
        // ==========================
        public async Task<IEnumerable<Quyen>> GetPermissionsByRoleIdAsync(int roleId)
        {
            // N:N VaiTro_Quyen ↔ Quyen đã được cấu hình trong OnModelCreating :contentReference[oaicite:3]{index=3}
            return await (from vtq in _context.VaiTroQuyens
                          join q in _context.Quyens on vtq.QuyenID equals q.QuyenID
                          where vtq.VaiTroID == roleId
                          select q)
                         .AsNoTracking()
                         .ToListAsync();
        }

        // ==========================
        // 4. GÁN ROLE CHO USER
        // ==========================
        public async Task<bool> AssignRoleToUserAsync(int userId, int roleId)
        {
            var user = await _context.NguoiDungs.FindAsync(userId);
            if (user == null) return false;

            var role = await _context.VaiTros.FindAsync(roleId);
            if (role == null) return false;

            var adminEntry = await _context.Admins.FirstOrDefaultAsync(a => a.UserID == userId);

            if (adminEntry == null)
            {
                adminEntry = new Admin
                {
                    UserID = userId,
                    VaiTroID = roleId,
                    TrangThai = true
                };
                await _context.Admins.AddAsync(adminEntry);
            }
            else
            {
                adminEntry.VaiTroID = roleId;
                adminEntry.TrangThai = true;
                _context.Admins.Update(adminEntry);
            }

            await _context.SaveChangesAsync();
            return true;
        }

        // ==========================
        // 5. CẬP NHẬT QUYỀN CỦA ROLE
        // ==========================
        public async Task<bool> UpdateRolePermissionsAsync(int roleId, IEnumerable<int> permissionIds)
        {
            var role = await _context.VaiTros.FindAsync(roleId);
            if (role == null) return false;

            var newPermissionIds = permissionIds.Distinct().ToList();

            // Lấy các mapping hiện tại
            var existingMappings = await _context.VaiTroQuyens
                                                 .Where(vtq => vtq.VaiTroID == roleId)
                                                 .ToListAsync();

            var existingPermissionIds = existingMappings
                                        .Select(m => m.QuyenID)
                                        .ToList();

            // 1. Xóa các quyền không còn trong list mới
            var mappingsToRemove = existingMappings
                .Where(m => !newPermissionIds.Contains(m.QuyenID))
                .ToList();

            if (mappingsToRemove.Count > 0)
            {
                _context.VaiTroQuyens.RemoveRange(mappingsToRemove);
            }

            // 2. Thêm các quyền mới chưa có mapping
            var permissionIdsToAdd = newPermissionIds
                .Where(id => !existingPermissionIds.Contains(id))
                .ToList();

            if (permissionIdsToAdd.Count > 0)
            {
                foreach (var permissionId in permissionIdsToAdd)
                {
                    _context.VaiTroQuyens.Add(new VaiTro_Quyen
                    {
                        VaiTroID = roleId,
                        QuyenID = permissionId
                    });
                }
            }

            await _context.SaveChangesAsync();
            return true;
        }
    }
}
