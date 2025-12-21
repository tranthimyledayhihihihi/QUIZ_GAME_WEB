// ------------------------------------------------------------------
// FILE: Data/AppDbContextFactory.cs (ĐÃ SỬA LỖI ĐƯỜNG DẪN CẤU HÌNH)
// ------------------------------------------------------------------
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using QUIZ_GAME_WEB.Data;
using System.IO;

// Giả sử namespace của bạn là QUIZ_GAME_WEB.Data
namespace QUIZ_GAME_WEB.Data
{
    public class AppDbContextFactory : IDesignTimeDbContextFactory<QuizGameContext>
    {
        public QuizGameContext CreateDbContext(string[] args)
        {
            // 1. TÌM ĐƯỜNG DẪN GỐC CỦA PROJECT
            // Kỹ thuật này duyệt qua các thư mục cha để tìm tên project (hoặc thư mục chứa appsettings.json)
            var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";

            var basePath = Directory.GetCurrentDirectory();
            // Điều chỉnh đường dẫn nếu cần. Ví dụ, nếu bạn chạy lệnh 'dotnet ef' từ thư mục chứa file .csproj
            // và appsettings.json nằm ở thư mục gốc của project (thường là cấp trên một cấp nếu file .csproj nằm trong thư mục Data)
            // Trong nhiều trường hợp, Directory.GetCurrentDirectory() là đủ nếu lệnh được chạy đúng chỗ.

            // 2. KHỞI TẠO CONFIGURATION
            var configuration = new ConfigurationBuilder()
                // Sử dụng basePath mặc định (hoặc đã điều chỉnh)
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{environmentName}.json", optional: true, reloadOnChange: true)
                .Build();

            // 3. LẤY CHUỖI KẾT NỐI
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            if (string.IsNullOrEmpty(connectionString))
            {
                // Thêm thông báo lỗi rõ ràng nếu không tìm thấy chuỗi kết nối
                throw new InvalidOperationException("Không tìm thấy chuỗi kết nối 'DefaultConnection' trong appsettings.json.");
            }

            // 4. TẠO DbContextOptions
            var optionsBuilder = new DbContextOptionsBuilder<QuizGameContext>();
            optionsBuilder.UseSqlServer(connectionString);

            return new QuizGameContext(optionsBuilder.Options);
        }
    }
}