using Microsoft.EntityFrameworkCore;
using QUIZ_GAME_WEB.Models;

namespace QUIZ_GAME_WEB.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<NguoiDung> NguoiDung { get; set; }
        public DbSet<CaiDatNguoiDung> CaiDatNguoiDung { get; set; }
        public DbSet<PhienDangNhap> PhienDangNhap { get; set; }
    }
}
