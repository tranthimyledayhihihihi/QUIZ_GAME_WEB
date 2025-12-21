using Microsoft.EntityFrameworkCore;
using QUIZ_GAME_WEB.Models.CoreEntities;
using QUIZ_GAME_WEB.Models.QuizModels;
using QUIZ_GAME_WEB.Models.ResultsModels;
using QUIZ_GAME_WEB.Models.SocialRankingModels;
using System;
using System.Linq;

namespace QUIZ_GAME_WEB.Data
{
    public class QuizGameContext : DbContext
    {
        public QuizGameContext(DbContextOptions<QuizGameContext> options) : base(options) { }

        // === 1. DbSet Core Entities ===
        public async Task<int> CompleteAsync()
        {
            return await base.SaveChangesAsync();
        }
        public DbSet<SystemSetting> SystemSettings { get; set; } = null!;
        public DbSet<Admin> Admins { get; set; } = null!;
        public DbSet<NguoiDung> NguoiDungs { get; set; } = null!;
        public DbSet<VaiTro> VaiTros { get; set; } = null!;
        public DbSet<Quyen> Quyens { get; set; } = null!;
        public DbSet<VaiTro_Quyen> VaiTroQuyens { get; set; } = null!;
        public DbSet<CaiDatNguoiDung> CaiDatNguoiDungs { get; set; } = null!;
        public DbSet<PhienDangNhap> PhienDangNhaps { get; set; } = null!;
        public DbSet<Comment> Comments { get; set; } = null!;

        // === 2. DbSet Quiz Entities ===
        public DbSet<CauHoi> CauHois { get; set; } = null!;
        public DbSet<ChuDe> ChuDes { get; set; } = null!;
        public DbSet<DoKho> DoKhos { get; set; } = null!;
        public DbSet<TroGiup> TroGiups { get; set; } = null!;
        public DbSet<QuizTuyChinh> QuizTuyChinhs { get; set; } = null!;
        public DbSet<QuizNgay> QuizNgays { get; set; } = null!;
        public DbSet<QuizChiaSe> QuizChiaSes { get; set; } = null!;
        public DbSet<QuizAttempt> QuizAttempts { get; set; } = null!;

        // === 3. DbSet Results & Social Entities ===
        public DbSet<KetQua> KetQuas { get; set; } = null!;
        public DbSet<CauSai> CauSais { get; set; } = null!;
        public DbSet<ChuoiNgay> ChuoiNgays { get; set; } = null!;
        public DbSet<ThanhTuu> ThanhTuus { get; set; } = null!;
        public DbSet<ThongKeNguoiDung> ThongKeNguoiDungs { get; set; } = null!;
        public DbSet<ThuongNgay> ThuongNgays { get; set; } = null!;
        public DbSet<BXH> BXHs { get; set; } = null!;
        public DbSet<NguoiDungOnline> NguoiDungOnlines { get; set; } = null!;
        public DbSet<ClientKey> ClientKeys { get; set; } = null!;
        public DbSet<TranDauTrucTiep> TranDauTrucTieps { get; set; } = null!;
        public DbSet<TranDauCauHoi> TranDauCauHois { get; set; } = null!;
        public DbSet<TraLoiTrucTiep> TraLoiTrucTieps { get; set; } = null!;
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Bổ sung: Thiết lập khóa chính phức hợp cho bảng nối TranDau_CauHoi
            //modelBuilder.Entity<TranDauCauHoi>()
            //    .HasKey(tc => new { tc.TranDauID, tc.CauHoiID });
            // Bạn cũng nên thiết lập mối quan hệ:
            /*
            modelBuilder.Entity<TranDauCauHoi>()
                .HasOne(tc => tc.TranDau)
                .WithMany(t => t.TranDauCauHois)
                .HasForeignKey(tc => tc.TranDauID);

            modelBuilder.Entity<TranDauCauHoi>()
                .HasOne(tc => tc.CauHoi)
                .WithMany(c => c.TranDauCauHois)
                .HasForeignKey(tc => tc.CauHoiID);
            */
            // Bổ sung: Cấu hình khóa chính tự tăng cho DoKho
            modelBuilder.Entity<DoKho>()
                .HasKey(d => d.DoKhoID); // Đảm bảo khóa chính được xác định
            modelBuilder.Entity<DoKho>()
                .Property(d => d.DoKhoID)
                .ValueGeneratedOnAdd();
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<TranDauTrucTiep>().ToTable("TranDauTrucTiep");
            modelBuilder.Entity<TranDauCauHoi>().ToTable("TranDau_CauHoi");
            modelBuilder.Entity<TraLoiTrucTiep>().ToTable("TraLoiTrucTiep");

            // === 1. Mapping table names (Giữ nguyên) ===
            modelBuilder.Entity<Admin>().ToTable("Admin");
            modelBuilder.Entity<NguoiDung>().ToTable("NguoiDung");
            modelBuilder.Entity<VaiTro>().ToTable("VaiTro");
            modelBuilder.Entity<Quyen>().ToTable("Quyen");
            modelBuilder.Entity<VaiTro_Quyen>().ToTable("VaiTro_Quyen");
            modelBuilder.Entity<CaiDatNguoiDung>().ToTable("CaiDatNguoiDung");
            modelBuilder.Entity<PhienDangNhap>().ToTable("PhienDangNhap");
            modelBuilder.Entity<Comment>().ToTable("Comment");
            modelBuilder.Entity<CauHoi>().ToTable("CauHoi");
            modelBuilder.Entity<ChuDe>().ToTable("ChuDe");
            modelBuilder.Entity<DoKho>().ToTable("DoKho");
            modelBuilder.Entity<TroGiup>().ToTable("TroGiup");
            modelBuilder.Entity<QuizTuyChinh>().ToTable("QuizTuyChinh");
            modelBuilder.Entity<QuizNgay>().ToTable("QuizNgay");
            modelBuilder.Entity<QuizChiaSe>().ToTable("QuizChiaSe");
            modelBuilder.Entity<QuizAttempt>().ToTable("QuizAttempt");
            modelBuilder.Entity<KetQua>().ToTable("KetQua");
            modelBuilder.Entity<CauSai>().ToTable("CauSai");
            modelBuilder.Entity<ChuoiNgay>().ToTable("ChuoiNgay");
            modelBuilder.Entity<ThanhTuu>().ToTable("ThanhTuu");
            modelBuilder.Entity<ThongKeNguoiDung>().ToTable("ThongKeNguoiDung");
            modelBuilder.Entity<ThuongNgay>().ToTable("ThuongNgay");
            modelBuilder.Entity<BXH>().ToTable("BXH");
            modelBuilder.Entity<NguoiDungOnline>().ToTable("NguoiDungOnline");

            // ===============================================
            // ✅ THIẾT LẬP QUAN HỆ KHÓA NGOẠI
            // ===============================================

            // === 2. N:N VaiTro_Quyen (Giữ nguyên) ===
            modelBuilder.Entity<VaiTro_Quyen>()
                .HasKey(vq => new { vq.VaiTroID, vq.QuyenID });

            // === 3. 1:N VaiTro ↔ NguoiDung (QUAN HỆ PHÂN QUYỀN) ===
            modelBuilder.Entity<NguoiDung>()
                .HasOne(u => u.VaiTro)
                .WithMany(v => v.NguoiDungs)
                .HasForeignKey(u => u.VaiTroID)
                .IsRequired(false); // Cho phép NULL nếu vai trò không bắt buộc (tùy chọn)

            // === 4. 1:1 Admin ↔ NguoiDung (Giữ nguyên) ===
            modelBuilder.Entity<Admin>()
                .HasOne(a => a.User)
                .WithOne(u => u.Admin)
                .HasForeignKey<Admin>(a => a.UserID);

            // === 5. 1:N NguoiDung ↔ CauSai (FK trên bảng CauSai) ===
            modelBuilder.Entity<CauSai>()
                .HasOne(cs => cs.NguoiDung) // Giả định CauSai có Navigation Property NguoiDung
                .WithMany(u => u.CauSais)
                .HasForeignKey(cs => cs.UserID);

            // === 6. 1:N QuizTuyChinh ↔ CauHoi (UGC - FK trên bảng CauHoi) ===
            modelBuilder.Entity<CauHoi>()
                .HasOne(ch => ch.QuizTuyChinh)
                .WithMany(qt => qt.CauHois)
                .HasForeignKey(ch => ch.QuizTuyChinhID)
                .IsRequired(false) // Cho phép câu hỏi tồn tại độc lập
                .OnDelete(DeleteBehavior.SetNull);

            // === 7. 1:N QuizChiaSe (Giữ nguyên) ===
            modelBuilder.Entity<QuizChiaSe>()
                .HasOne(q => q.UserGui)
                .WithMany(u => u.QuizChiaSesGui)
                .HasForeignKey(q => q.UserGuiID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<QuizChiaSe>()
                .HasOne(q => q.UserNhan)
                .WithMany(u => u.QuizChiaSesNhan)
                .HasForeignKey(q => q.UserNhanID)
                .OnDelete(DeleteBehavior.Restrict);

            // === 8. Unique Indexes (Giữ nguyên) ===
            modelBuilder.Entity<NguoiDung>().HasIndex(u => u.TenDangNhap).IsUnique();
            modelBuilder.Entity<NguoiDung>().HasIndex(u => u.Email).IsUnique();


            // ===============================================
            // ✅ SEED DATA (Sửa lỗi thiếu VaiTroID)
            // ===============================================

            // NguoiDung (Bổ sung VaiTroID)
            modelBuilder.Entity<NguoiDung>().HasData(
                new NguoiDung { UserID = 1, TenDangNhap = "admin_sa", MatKhau = "hashed_sa_password", Email = "superadmin@quiz.com", HoTen = "Nguyễn Super Admin", TrangThai = true, VaiTroID = 1 }, // SuperAdmin
                new NguoiDung { UserID = 2, TenDangNhap = "player01", MatKhau = "hashed_p1_password", Email = "player01@quiz.com", HoTen = "Trần Văn A", TrangThai = true, VaiTroID = 3 }, // Player
                new NguoiDung { UserID = 3, TenDangNhap = "player02", MatKhau = "hashed_p2_password", Email = "player02@quiz.com", HoTen = "Lê Thị B", TrangThai = true, VaiTroID = 3 }  // Player
            );

            // === 9. Seed Data ===
            // VaiTro
            modelBuilder.Entity<VaiTro>().HasData(
                new VaiTro { VaiTroID = 1, TenVaiTro = "SuperAdmin", MoTa = "Quản trị viên cấp cao, toàn quyền hệ thống." },
                new VaiTro { VaiTroID = 2, TenVaiTro = "Moderator", MoTa = "Kiểm duyệt viên, quản lý câu hỏi và người dùng." },
                new VaiTro { VaiTroID = 3, TenVaiTro = "Player", MoTa = "Người dùng/Người chơi thông thường." }
            );

            // Quyen
            modelBuilder.Entity<Quyen>().HasData(
                new Quyen { QuyenID = 1, TenQuyen = "ql_nguoi_dung", MoTa = "Quản lý (Khóa/Mở khóa) tài khoản người dùng." },
                new Quyen { QuyenID = 2, TenQuyen = "ql_cau_hoi", MoTa = "Thêm, sửa, xóa, duyệt câu hỏi." },
                new Quyen { QuyenID = 3, TenQuyen = "ql_baocao", MoTa = "Truy cập và tạo báo cáo hệ thống." },
                new Quyen { QuyenID = 4, TenQuyen = "ql_vai_tro", MoTa = "Quản lý vai trò và quyền hạn (Chỉ SuperAdmin)." }
            );

            // VaiTro_Quyen
            modelBuilder.Entity<VaiTro_Quyen>().HasData(
                new VaiTro_Quyen { VaiTroID = 1, QuyenID = 1 },
                new VaiTro_Quyen { VaiTroID = 1, QuyenID = 2 },
                new VaiTro_Quyen { VaiTroID = 1, QuyenID = 3 },
                new VaiTro_Quyen { VaiTroID = 1, QuyenID = 4 },
                new VaiTro_Quyen { VaiTroID = 2, QuyenID = 1 },
                new VaiTro_Quyen { VaiTroID = 2, QuyenID = 2 },
                new VaiTro_Quyen { VaiTroID = 2, QuyenID = 3 }
            );

            // Admin
            modelBuilder.Entity<Admin>().HasData(
                new Admin { AdminID = 1, UserID = 1, VaiTroID = 1, TrangThai = true }
            );

            // CaiDatNguoiDung
            modelBuilder.Entity<CaiDatNguoiDung>().HasData(
                new CaiDatNguoiDung { SettingID = 1, UserID = 2, AmThanh = true, NhacNen = false, ThongBao = true, NgonNgu = "vi" },
                new CaiDatNguoiDung { SettingID = 2, UserID = 3, AmThanh = true, NhacNen = true, ThongBao = true, NgonNgu = "vi" }
            );

            // ChuDe
            modelBuilder.Entity<ChuDe>().HasData(
                new ChuDe { ChuDeID = 1, TenChuDe = "Lịch Sử Việt Nam", MoTa = "Các sự kiện và nhân vật lịch sử quan trọng.", TrangThai = true },
                new ChuDe { ChuDeID = 2, TenChuDe = "Toán Học Phổ Thông", MoTa = "Các bài toán đại số và hình học cơ bản.", TrangThai = true },
                new ChuDe { ChuDeID = 3, TenChuDe = "Khoa Học Tự Nhiên", MoTa = "Kiến thức vật lý, hóa học, sinh học.", TrangThai = true }
            );

            // DoKho
            modelBuilder.Entity<DoKho>().HasData(
                new DoKho { DoKhoID = 1, TenDoKho = "Dễ", DiemThuong = 10 },
                new DoKho { DoKhoID = 2, TenDoKho = "Trung bình", DiemThuong = 25 },
                new DoKho { DoKhoID = 3, TenDoKho = "Khó", DiemThuong = 50 }
            );

            // TroGiup
            modelBuilder.Entity<TroGiup>().HasData(
                new TroGiup { TroGiupID = 1, TenTroGiup = "50/50", MoTa = "Loại bỏ hai đáp án sai." },
                new TroGiup { TroGiupID = 2, TenTroGiup = "Hỏi khán giả", MoTa = "Tham khảo ý kiến cộng đồng." }
            );

            // CauHoi
            modelBuilder.Entity<CauHoi>().HasData(
                new CauHoi { CauHoiID = 1, ChuDeID = 1, DoKhoID = 1, NoiDung = "Ai là người phất cờ khởi nghĩa đầu tiên chống Pháp?", DapAnA = "Phan Đình Phùng", DapAnB = "Trần Văn Thời", DapAnC = "Trương Định", DapAnD = "Nguyễn Trung Trực", DapAnDung = "C" },
                new CauHoi { CauHoiID = 2, ChuDeID = 1, DoKhoID = 2, NoiDung = "Chiến dịch Điện Biên Phủ diễn ra năm nào?", DapAnA = "1953", DapAnB = "1954", DapAnC = "1975", DapAnD = "1950", DapAnDung = "B" },
                new CauHoi { CauHoiID = 3, ChuDeID = 2, DoKhoID = 1, NoiDung = "Căn bậc hai của 9 là bao nhiêu?", DapAnA = "3", DapAnB = "9", DapAnC = "3 và -3", DapAnD = "Không có", DapAnDung = "C" },
                new CauHoi { CauHoiID = 4, ChuDeID = 3, DoKhoID = 2, NoiDung = "Chất nào sau đây không dẫn điện?", DapAnA = "Đồng", DapAnB = "Vàng", DapAnC = "Nhựa", DapAnD = "Bạc", DapAnDung = "C" }
            );

            // QuizTuyChinh - PHẢI TẠO TRƯỚC QuizAttempt
            modelBuilder.Entity<QuizTuyChinh>().HasData(
                new QuizTuyChinh { QuizTuyChinhID = 1, UserID = 2, TenQuiz = "Quiz Của Tôi", MoTa = "Các câu hỏi tôi thích nhất.", NgayTao = DateTime.Now }
            );

            // QuizAttempt - ĐÃ SỬA: Thêm QuizTuyChinhID = 1
            modelBuilder.Entity<QuizAttempt>().HasData(
                new QuizAttempt
                {
                    QuizAttemptID = 1,
                    UserID = 2,
                    QuizTuyChinhID = 1,  // ← ĐÃ THÊM
                    NgayBatDau = DateTime.Now.AddHours(-1),
                    NgayKetThuc = DateTime.Now,
                    TrangThai = "Hoàn thành",
                    SoCauHoiLam = 0,
                    SoCauDung = 0,
                    Diem = 0
                },
                new QuizAttempt
                {
                    QuizAttemptID = 2,
                    UserID = 3,
                    QuizTuyChinhID = 1,  // ← ĐÃ THÊM
                    NgayBatDau = DateTime.Now.AddHours(-2),
                    NgayKetThuc = DateTime.Now,
                    TrangThai = "Hoàn thành",
                    SoCauHoiLam = 0,
                    SoCauDung = 0,
                    Diem = 0
                }
            );

            // KetQua - ĐÃ SỬA: Mỗi QuizAttempt chỉ có 1 KetQua (quan hệ 1:1)
            modelBuilder.Entity<KetQua>().HasData(
                new KetQua { KetQuaID = 1, UserID = 2, Diem = 50, SoCauDung = 2, TongCauHoi = 2, TrangThaiKetQua = "Hoàn thành", ThoiGian = DateTime.Now.AddHours(-5), QuizAttemptID = 1 },
                new KetQua { KetQuaID = 2, UserID = 3, Diem = 25, SoCauDung = 1, TongCauHoi = 2, TrangThaiKetQua = "Hoàn thành", ThoiGian = DateTime.Now.AddHours(-1), QuizAttemptID = 2 }
            );

            // CauSai
            modelBuilder.Entity<CauSai>().HasData(
                new CauSai { CauSaiID = 1, UserID = 3, CauHoiID = 2, QuizAttemptID = 2, NgaySai = DateTime.Now.Date }
            );

            // BXH
            modelBuilder.Entity<BXH>().HasData(
                new BXH { BXHID = 1, UserID = 2, DiemTuan = 125, DiemThang = 125, HangTuan = 1, HangThang = 1 },
                new BXH { BXHID = 2, UserID = 3, DiemTuan = 25, DiemThang = 25, HangTuan = 2, HangThang = 2 }
            );

            // ChuoiNgay
            modelBuilder.Entity<ChuoiNgay>().HasData(
                new ChuoiNgay { ChuoiID = 1, UserID = 2, SoNgayLienTiep = 5, NgayCapNhatCuoi = DateTime.Now },
                new ChuoiNgay { ChuoiID = 2, UserID = 3, SoNgayLienTiep = 2, NgayCapNhatCuoi = DateTime.Now }
            );

            // QuizNgay
            modelBuilder.Entity<QuizNgay>().HasData(
                new QuizNgay { QuizNgayID = 1, CauHoiID = 1, Ngay = DateTime.Now.Date }
            );

            // QuizChiaSe
            modelBuilder.Entity<QuizChiaSe>().HasData(
                new QuizChiaSe { QuizChiaSeID = 1, QuizTuyChinhID = 1, UserGuiID = 2, UserNhanID = 3, NgayChiaSe = DateTime.Now }
            );
            // === 10. Seed Data cho Comment ===
            modelBuilder.Entity<Comment>().HasData(
                new Comment
                {
                    CommentID = 1,
                    UserID = 1,
                    Content = "Chào mừng mọi người đến với hệ thống Quiz Game!",
                    NgayTao = DateTime.Now,
                    EntityType = "System",
                    RelatedEntityID = 0
                },
                new Comment
                {
                    CommentID = 2,
                    UserID = 2,
                    Content = "Câu hỏi lịch sử này rất thú vị.",
                    NgayTao = DateTime.Now,
                    EntityType = "CauHoi",
                    RelatedEntityID = 1
                },
                new Comment
                {
                    CommentID = 3,
                    UserID = 3,
                    Content = "Mình vừa hoàn thành bộ đề tùy chỉnh của bạn!",
                    NgayTao = DateTime.Now,
                    EntityType = "QuizTuyChinh",
                    RelatedEntityID = 1
                }
            );

            // === 11. Seed Data cho ThongKeNguoiDung ===
            modelBuilder.Entity<ThongKeNguoiDung>().HasData(
                new ThongKeNguoiDung
                {
                    ThongKeID = 1,
                    UserID = 2,
                    Ngay = DateTime.Now.Date,
                    SoTran = 5,
                    SoCauDung = 40,
                    DiemTrungBinh = 8.0
                },
                new ThongKeNguoiDung
                {
                    ThongKeID = 2,
                    UserID = 3,
                    Ngay = DateTime.Now.Date,
                    SoTran = 3,
                    SoCauDung = 21,
                    DiemTrungBinh = 7.0
                }
            );
        }
    }
}