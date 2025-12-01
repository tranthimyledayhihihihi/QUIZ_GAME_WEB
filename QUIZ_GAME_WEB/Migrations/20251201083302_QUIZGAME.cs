using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace QUIZ_GAME_WEB.Migrations
{
    /// <inheritdoc />
    public partial class QUIZGAME : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ChuDe",
                columns: table => new
                {
                    ChuDeID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenChuDe = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    MoTa = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    TrangThai = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChuDe", x => x.ChuDeID);
                });

            migrationBuilder.CreateTable(
                name: "ClientKeys",
                columns: table => new
                {
                    ClientKeyID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KeyValue = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    TenUngDung = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NgayHetHan = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientKeys", x => x.ClientKeyID);
                });

            migrationBuilder.CreateTable(
                name: "DoKho",
                columns: table => new
                {
                    DoKhoID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenDoKho = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DiemThuong = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DoKho", x => x.DoKhoID);
                });

            migrationBuilder.CreateTable(
                name: "NguoiDung",
                columns: table => new
                {
                    UserID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenDangNhap = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MatKhau = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    HoTen = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    AnhDaiDien = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    NgayDangKy = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LanDangNhapCuoi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TrangThai = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NguoiDung", x => x.UserID);
                });

            migrationBuilder.CreateTable(
                name: "Quyen",
                columns: table => new
                {
                    QuyenID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenQuyen = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    MoTa = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Quyen", x => x.QuyenID);
                });

            migrationBuilder.CreateTable(
                name: "SystemSettings",
                columns: table => new
                {
                    Key = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MoTa = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemSettings", x => x.Key);
                });

            migrationBuilder.CreateTable(
                name: "TroGiup",
                columns: table => new
                {
                    TroGiupID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenTroGiup = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MoTa = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TroGiup", x => x.TroGiupID);
                });

            migrationBuilder.CreateTable(
                name: "VaiTro",
                columns: table => new
                {
                    VaiTroID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenVaiTro = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MoTa = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VaiTro", x => x.VaiTroID);
                });

            migrationBuilder.CreateTable(
                name: "CauHoi",
                columns: table => new
                {
                    CauHoiID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ChuDeID = table.Column<int>(type: "int", nullable: false),
                    DoKhoID = table.Column<int>(type: "int", nullable: false),
                    NoiDung = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    DapAnA = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    DapAnB = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    DapAnC = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    DapAnD = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    DapAnDung = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    HinhAnh = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CauHoi", x => x.CauHoiID);
                    table.ForeignKey(
                        name: "FK_CauHoi_ChuDe_ChuDeID",
                        column: x => x.ChuDeID,
                        principalTable: "ChuDe",
                        principalColumn: "ChuDeID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CauHoi_DoKho_DoKhoID",
                        column: x => x.DoKhoID,
                        principalTable: "DoKho",
                        principalColumn: "DoKhoID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BXH",
                columns: table => new
                {
                    BXHID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserID = table.Column<int>(type: "int", nullable: false),
                    DiemTuan = table.Column<int>(type: "int", nullable: false),
                    DiemThang = table.Column<int>(type: "int", nullable: false),
                    HangTuan = table.Column<int>(type: "int", nullable: false),
                    HangThang = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BXH", x => x.BXHID);
                    table.ForeignKey(
                        name: "FK_BXH_NguoiDung_UserID",
                        column: x => x.UserID,
                        principalTable: "NguoiDung",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CaiDatNguoiDung",
                columns: table => new
                {
                    SettingID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserID = table.Column<int>(type: "int", nullable: false),
                    AmThanh = table.Column<bool>(type: "bit", nullable: false),
                    NhacNen = table.Column<bool>(type: "bit", nullable: false),
                    ThongBao = table.Column<bool>(type: "bit", nullable: false),
                    NgonNgu = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CaiDatNguoiDung", x => x.SettingID);
                    table.ForeignKey(
                        name: "FK_CaiDatNguoiDung_NguoiDung_UserID",
                        column: x => x.UserID,
                        principalTable: "NguoiDung",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ChuoiNgay",
                columns: table => new
                {
                    ChuoiID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserID = table.Column<int>(type: "int", nullable: false),
                    SoNgayLienTiep = table.Column<int>(type: "int", nullable: false),
                    NgayCapNhatCuoi = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChuoiNgay", x => x.ChuoiID);
                    table.ForeignKey(
                        name: "FK_ChuoiNgay_NguoiDung_UserID",
                        column: x => x.UserID,
                        principalTable: "NguoiDung",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Comment",
                columns: table => new
                {
                    CommentID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserID = table.Column<int>(type: "int", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EntityType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RelatedEntityID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comment", x => x.CommentID);
                    table.ForeignKey(
                        name: "FK_Comment_NguoiDung_UserID",
                        column: x => x.UserID,
                        principalTable: "NguoiDung",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NguoiDungOnline",
                columns: table => new
                {
                    OnlineID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserID = table.Column<int>(type: "int", nullable: false),
                    TrangThai = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ThoiGianCapNhat = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NguoiDungOnline", x => x.OnlineID);
                    table.ForeignKey(
                        name: "FK_NguoiDungOnline_NguoiDung_UserID",
                        column: x => x.UserID,
                        principalTable: "NguoiDung",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PhienDangNhap",
                columns: table => new
                {
                    SessionID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserID = table.Column<int>(type: "int", nullable: false),
                    Token = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ThoiGianDangNhap = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ThoiGianHetHan = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TrangThai = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhienDangNhap", x => x.SessionID);
                    table.ForeignKey(
                        name: "FK_PhienDangNhap_NguoiDung_UserID",
                        column: x => x.UserID,
                        principalTable: "NguoiDung",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "QuizTuyChinh",
                columns: table => new
                {
                    QuizTuyChinhID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserID = table.Column<int>(type: "int", nullable: false),
                    TenQuiz = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    MoTa = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuizTuyChinh", x => x.QuizTuyChinhID);
                    table.ForeignKey(
                        name: "FK_QuizTuyChinh_NguoiDung_UserID",
                        column: x => x.UserID,
                        principalTable: "NguoiDung",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ThanhTuu",
                columns: table => new
                {
                    ThanhTuuID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenThanhTuu = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    MoTa = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BieuTuong = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DieuKien = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NguoiDungID = table.Column<int>(type: "int", nullable: false),
                    AchievementCode = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ThanhTuu", x => x.ThanhTuuID);
                    table.ForeignKey(
                        name: "FK_ThanhTuu_NguoiDung_NguoiDungID",
                        column: x => x.NguoiDungID,
                        principalTable: "NguoiDung",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ThongKeNguoiDung",
                columns: table => new
                {
                    ThongKeID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserID = table.Column<int>(type: "int", nullable: false),
                    Ngay = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SoTran = table.Column<int>(type: "int", nullable: false),
                    SoCauDung = table.Column<int>(type: "int", nullable: false),
                    DiemTrungBinh = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ThongKeNguoiDung", x => x.ThongKeID);
                    table.ForeignKey(
                        name: "FK_ThongKeNguoiDung_NguoiDung_UserID",
                        column: x => x.UserID,
                        principalTable: "NguoiDung",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ThuongNgay",
                columns: table => new
                {
                    ThuongID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserID = table.Column<int>(type: "int", nullable: false),
                    NgayNhan = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PhanThuong = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DiemThuong = table.Column<int>(type: "int", nullable: false),
                    TrangThaiNhan = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ThuongNgay", x => x.ThuongID);
                    table.ForeignKey(
                        name: "FK_ThuongNgay_NguoiDung_UserID",
                        column: x => x.UserID,
                        principalTable: "NguoiDung",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Admin",
                columns: table => new
                {
                    AdminID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserID = table.Column<int>(type: "int", nullable: false),
                    VaiTroID = table.Column<int>(type: "int", nullable: false),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TrangThai = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Admin", x => x.AdminID);
                    table.ForeignKey(
                        name: "FK_Admin_NguoiDung_UserID",
                        column: x => x.UserID,
                        principalTable: "NguoiDung",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Admin_VaiTro_VaiTroID",
                        column: x => x.VaiTroID,
                        principalTable: "VaiTro",
                        principalColumn: "VaiTroID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VaiTro_Quyen",
                columns: table => new
                {
                    VaiTroID = table.Column<int>(type: "int", nullable: false),
                    QuyenID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VaiTro_Quyen", x => new { x.VaiTroID, x.QuyenID });
                    table.ForeignKey(
                        name: "FK_VaiTro_Quyen_Quyen_QuyenID",
                        column: x => x.QuyenID,
                        principalTable: "Quyen",
                        principalColumn: "QuyenID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VaiTro_Quyen_VaiTro_VaiTroID",
                        column: x => x.VaiTroID,
                        principalTable: "VaiTro",
                        principalColumn: "VaiTroID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "QuizNgay",
                columns: table => new
                {
                    QuizNgayID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Ngay = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CauHoiID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuizNgay", x => x.QuizNgayID);
                    table.ForeignKey(
                        name: "FK_QuizNgay_CauHoi_CauHoiID",
                        column: x => x.CauHoiID,
                        principalTable: "CauHoi",
                        principalColumn: "CauHoiID");
                });

            migrationBuilder.CreateTable(
                name: "QuizAttempt",
                columns: table => new
                {
                    QuizAttemptID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserID = table.Column<int>(type: "int", nullable: false),
                    QuizTuyChinhID = table.Column<int>(type: "int", nullable: false),
                    NgayBatDau = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NgayKetThuc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SoCauHoiLam = table.Column<int>(type: "int", nullable: false),
                    SoCauDung = table.Column<int>(type: "int", nullable: false),
                    Diem = table.Column<int>(type: "int", nullable: false),
                    TrangThai = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuizAttempt", x => x.QuizAttemptID);
                    table.ForeignKey(
                        name: "FK_QuizAttempt_NguoiDung_UserID",
                        column: x => x.UserID,
                        principalTable: "NguoiDung",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_QuizAttempt_QuizTuyChinh_QuizTuyChinhID",
                        column: x => x.QuizTuyChinhID,
                        principalTable: "QuizTuyChinh",
                        principalColumn: "QuizTuyChinhID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "QuizChiaSe",
                columns: table => new
                {
                    QuizChiaSeID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    QuizTuyChinhID = table.Column<int>(type: "int", nullable: false),
                    UserGuiID = table.Column<int>(type: "int", nullable: false),
                    UserNhanID = table.Column<int>(type: "int", nullable: true),
                    NgayChiaSe = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuizChiaSe", x => x.QuizChiaSeID);
                    table.ForeignKey(
                        name: "FK_QuizChiaSe_NguoiDung_UserGuiID",
                        column: x => x.UserGuiID,
                        principalTable: "NguoiDung",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_QuizChiaSe_NguoiDung_UserNhanID",
                        column: x => x.UserNhanID,
                        principalTable: "NguoiDung",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_QuizChiaSe_QuizTuyChinh_QuizTuyChinhID",
                        column: x => x.QuizTuyChinhID,
                        principalTable: "QuizTuyChinh",
                        principalColumn: "QuizTuyChinhID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CauSai",
                columns: table => new
                {
                    CauSaiID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserID = table.Column<int>(type: "int", nullable: false),
                    CauHoiID = table.Column<int>(type: "int", nullable: false),
                    QuizAttemptID = table.Column<int>(type: "int", nullable: false),
                    NgaySai = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CauSai", x => x.CauSaiID);
                    table.ForeignKey(
                        name: "FK_CauSai_CauHoi_CauHoiID",
                        column: x => x.CauHoiID,
                        principalTable: "CauHoi",
                        principalColumn: "CauHoiID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CauSai_NguoiDung_UserID",
                        column: x => x.UserID,
                        principalTable: "NguoiDung",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CauSai_QuizAttempt_QuizAttemptID",
                        column: x => x.QuizAttemptID,
                        principalTable: "QuizAttempt",
                        principalColumn: "QuizAttemptID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "KetQua",
                columns: table => new
                {
                    KetQuaID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserID = table.Column<int>(type: "int", nullable: false),
                    QuizAttemptID = table.Column<int>(type: "int", nullable: false),
                    Diem = table.Column<int>(type: "int", nullable: false),
                    SoCauDung = table.Column<int>(type: "int", nullable: false),
                    TongCauHoi = table.Column<int>(type: "int", nullable: false),
                    TrangThaiKetQua = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ThoiGian = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KetQua", x => x.KetQuaID);
                    table.ForeignKey(
                        name: "FK_KetQua_NguoiDung_UserID",
                        column: x => x.UserID,
                        principalTable: "NguoiDung",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_KetQua_QuizAttempt_QuizAttemptID",
                        column: x => x.QuizAttemptID,
                        principalTable: "QuizAttempt",
                        principalColumn: "QuizAttemptID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "ChuDe",
                columns: new[] { "ChuDeID", "MoTa", "TenChuDe", "TrangThai" },
                values: new object[,]
                {
                    { 1, "Các sự kiện và nhân vật lịch sử quan trọng.", "Lịch Sử Việt Nam", true },
                    { 2, "Các bài toán đại số và hình học cơ bản.", "Toán Học Phổ Thông", true },
                    { 3, "Kiến thức vật lý, hóa học, sinh học.", "Khoa Học Tự Nhiên", true }
                });

            migrationBuilder.InsertData(
                table: "DoKho",
                columns: new[] { "DoKhoID", "DiemThuong", "TenDoKho" },
                values: new object[,]
                {
                    { 1, 10, "Dễ" },
                    { 2, 25, "Trung bình" },
                    { 3, 50, "Khó" }
                });

            migrationBuilder.InsertData(
                table: "NguoiDung",
                columns: new[] { "UserID", "AnhDaiDien", "Email", "HoTen", "LanDangNhapCuoi", "MatKhau", "NgayDangKy", "TenDangNhap", "TrangThai" },
                values: new object[,]
                {
                    { 1, null, "superadmin@quiz.com", "Nguyễn Super Admin", null, "hashed_sa_password", new DateTime(2025, 12, 1, 15, 33, 2, 325, DateTimeKind.Local).AddTicks(4103), "admin_sa", true },
                    { 2, null, "player01@quiz.com", "Trần Văn A", null, "hashed_p1_password", new DateTime(2025, 12, 1, 15, 33, 2, 325, DateTimeKind.Local).AddTicks(4124), "player01", true },
                    { 3, null, "player02@quiz.com", "Lê Thị B", null, "hashed_p2_password", new DateTime(2025, 12, 1, 15, 33, 2, 325, DateTimeKind.Local).AddTicks(4166), "player02", true }
                });

            migrationBuilder.InsertData(
                table: "Quyen",
                columns: new[] { "QuyenID", "MoTa", "TenQuyen" },
                values: new object[,]
                {
                    { 1, "Quản lý (Khóa/Mở khóa) tài khoản người dùng.", "ql_nguoi_dung" },
                    { 2, "Thêm, sửa, xóa, duyệt câu hỏi.", "ql_cau_hoi" },
                    { 3, "Truy cập và tạo báo cáo hệ thống.", "ql_baocao" },
                    { 4, "Quản lý vai trò và quyền hạn (Chỉ SuperAdmin).", "ql_vai_tro" }
                });

            migrationBuilder.InsertData(
                table: "TroGiup",
                columns: new[] { "TroGiupID", "MoTa", "TenTroGiup" },
                values: new object[,]
                {
                    { 1, "Loại bỏ hai đáp án sai.", "50/50" },
                    { 2, "Tham khảo ý kiến cộng đồng.", "Hỏi khán giả" }
                });

            migrationBuilder.InsertData(
                table: "VaiTro",
                columns: new[] { "VaiTroID", "MoTa", "TenVaiTro" },
                values: new object[,]
                {
                    { 1, "Quản trị viên cấp cao, toàn quyền hệ thống.", "SuperAdmin" },
                    { 2, "Kiểm duyệt viên, quản lý câu hỏi và người dùng.", "Moderator" },
                    { 3, "Người dùng/Người chơi thông thường.", "Player" }
                });

            migrationBuilder.InsertData(
                table: "Admin",
                columns: new[] { "AdminID", "NgayTao", "TrangThai", "UserID", "VaiTroID" },
                values: new object[] { 1, new DateTime(2025, 12, 1, 15, 33, 2, 325, DateTimeKind.Local).AddTicks(4192), true, 1, 1 });

            migrationBuilder.InsertData(
                table: "BXH",
                columns: new[] { "BXHID", "DiemThang", "DiemTuan", "HangThang", "HangTuan", "UserID" },
                values: new object[,]
                {
                    { 1, 125, 125, 1, 1, 2 },
                    { 2, 25, 25, 2, 2, 3 }
                });

            migrationBuilder.InsertData(
                table: "CaiDatNguoiDung",
                columns: new[] { "SettingID", "AmThanh", "NgonNgu", "NhacNen", "ThongBao", "UserID" },
                values: new object[,]
                {
                    { 1, true, "vi", false, true, 2 },
                    { 2, true, "vi", true, true, 3 }
                });

            migrationBuilder.InsertData(
                table: "CauHoi",
                columns: new[] { "CauHoiID", "ChuDeID", "DapAnA", "DapAnB", "DapAnC", "DapAnD", "DapAnDung", "DoKhoID", "HinhAnh", "NgayTao", "NoiDung" },
                values: new object[,]
                {
                    { 1, 1, "Phan Đình Phùng", "Trần Văn Thời", "Trương Định", "Nguyễn Trung Trực", "C", 1, null, new DateTime(2025, 12, 1, 15, 33, 2, 325, DateTimeKind.Local).AddTicks(4300), "Ai là người phất cờ khởi nghĩa đầu tiên chống Pháp?" },
                    { 2, 1, "1953", "1954", "1975", "1950", "B", 2, null, new DateTime(2025, 12, 1, 15, 33, 2, 325, DateTimeKind.Local).AddTicks(4304), "Chiến dịch Điện Biên Phủ diễn ra năm nào?" },
                    { 3, 2, "3", "9", "3 và -3", "Không có", "C", 1, null, new DateTime(2025, 12, 1, 15, 33, 2, 325, DateTimeKind.Local).AddTicks(4306), "Căn bậc hai của 9 là bao nhiêu?" },
                    { 4, 3, "Đồng", "Vàng", "Nhựa", "Bạc", "C", 2, null, new DateTime(2025, 12, 1, 15, 33, 2, 325, DateTimeKind.Local).AddTicks(4308), "Chất nào sau đây không dẫn điện?" }
                });

            migrationBuilder.InsertData(
                table: "ChuoiNgay",
                columns: new[] { "ChuoiID", "NgayCapNhatCuoi", "SoNgayLienTiep", "UserID" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 12, 1, 15, 33, 2, 325, DateTimeKind.Local).AddTicks(4450), 5, 2 },
                    { 2, new DateTime(2025, 12, 1, 15, 33, 2, 325, DateTimeKind.Local).AddTicks(4453), 2, 3 }
                });

            migrationBuilder.InsertData(
                table: "QuizTuyChinh",
                columns: new[] { "QuizTuyChinhID", "MoTa", "NgayTao", "TenQuiz", "UserID" },
                values: new object[] { 1, "Các câu hỏi tôi thích nhất.", new DateTime(2025, 12, 1, 15, 33, 2, 325, DateTimeKind.Local).AddTicks(4330), "Quiz Của Tôi", 2 });

            migrationBuilder.InsertData(
                table: "VaiTro_Quyen",
                columns: new[] { "QuyenID", "VaiTroID" },
                values: new object[,]
                {
                    { 1, 1 },
                    { 2, 1 },
                    { 3, 1 },
                    { 4, 1 },
                    { 1, 2 },
                    { 2, 2 },
                    { 3, 2 }
                });

            migrationBuilder.InsertData(
                table: "QuizAttempt",
                columns: new[] { "QuizAttemptID", "Diem", "NgayBatDau", "NgayKetThuc", "QuizTuyChinhID", "SoCauDung", "SoCauHoiLam", "TrangThai", "UserID" },
                values: new object[,]
                {
                    { 1, 0, new DateTime(2025, 12, 1, 14, 33, 2, 325, DateTimeKind.Local).AddTicks(4352), new DateTime(2025, 12, 1, 15, 33, 2, 325, DateTimeKind.Local).AddTicks(4359), 1, 0, 0, "Hoàn thành", 2 },
                    { 2, 0, new DateTime(2025, 12, 1, 13, 33, 2, 325, DateTimeKind.Local).AddTicks(4364), new DateTime(2025, 12, 1, 15, 33, 2, 325, DateTimeKind.Local).AddTicks(4365), 1, 0, 0, "Hoàn thành", 3 }
                });

            migrationBuilder.InsertData(
                table: "QuizChiaSe",
                columns: new[] { "QuizChiaSeID", "NgayChiaSe", "QuizTuyChinhID", "UserGuiID", "UserNhanID" },
                values: new object[] { 1, new DateTime(2025, 12, 1, 15, 33, 2, 325, DateTimeKind.Local).AddTicks(4496), 1, 2, 3 });

            migrationBuilder.InsertData(
                table: "QuizNgay",
                columns: new[] { "QuizNgayID", "CauHoiID", "Ngay" },
                values: new object[] { 1, 1, new DateTime(2025, 12, 1, 0, 0, 0, 0, DateTimeKind.Local) });

            migrationBuilder.InsertData(
                table: "CauSai",
                columns: new[] { "CauSaiID", "CauHoiID", "NgaySai", "QuizAttemptID", "UserID" },
                values: new object[] { 1, 2, new DateTime(2025, 12, 1, 0, 0, 0, 0, DateTimeKind.Local), 2, 3 });

            migrationBuilder.InsertData(
                table: "KetQua",
                columns: new[] { "KetQuaID", "Diem", "QuizAttemptID", "SoCauDung", "ThoiGian", "TongCauHoi", "TrangThaiKetQua", "UserID" },
                values: new object[,]
                {
                    { 1, 50, 1, 2, new DateTime(2025, 12, 1, 10, 33, 2, 325, DateTimeKind.Local).AddTicks(4387), 2, "Hoàn thành", 2 },
                    { 2, 25, 2, 1, new DateTime(2025, 12, 1, 14, 33, 2, 325, DateTimeKind.Local).AddTicks(4390), 2, "Hoàn thành", 3 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Admin_UserID",
                table: "Admin",
                column: "UserID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Admin_VaiTroID",
                table: "Admin",
                column: "VaiTroID");

            migrationBuilder.CreateIndex(
                name: "IX_BXH_UserID",
                table: "BXH",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_CaiDatNguoiDung_UserID",
                table: "CaiDatNguoiDung",
                column: "UserID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CauHoi_ChuDeID",
                table: "CauHoi",
                column: "ChuDeID");

            migrationBuilder.CreateIndex(
                name: "IX_CauHoi_DoKhoID",
                table: "CauHoi",
                column: "DoKhoID");

            migrationBuilder.CreateIndex(
                name: "IX_CauSai_CauHoiID",
                table: "CauSai",
                column: "CauHoiID");

            migrationBuilder.CreateIndex(
                name: "IX_CauSai_QuizAttemptID",
                table: "CauSai",
                column: "QuizAttemptID");

            migrationBuilder.CreateIndex(
                name: "IX_CauSai_UserID",
                table: "CauSai",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_ChuoiNgay_UserID",
                table: "ChuoiNgay",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_Comment_UserID",
                table: "Comment",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_KetQua_QuizAttemptID",
                table: "KetQua",
                column: "QuizAttemptID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_KetQua_UserID",
                table: "KetQua",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_NguoiDung_Email",
                table: "NguoiDung",
                column: "Email",
                unique: true,
                filter: "[Email] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_NguoiDung_TenDangNhap",
                table: "NguoiDung",
                column: "TenDangNhap",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_NguoiDungOnline_UserID",
                table: "NguoiDungOnline",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_PhienDangNhap_UserID",
                table: "PhienDangNhap",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_QuizAttempt_QuizTuyChinhID",
                table: "QuizAttempt",
                column: "QuizTuyChinhID");

            migrationBuilder.CreateIndex(
                name: "IX_QuizAttempt_UserID",
                table: "QuizAttempt",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_QuizChiaSe_QuizTuyChinhID",
                table: "QuizChiaSe",
                column: "QuizTuyChinhID");

            migrationBuilder.CreateIndex(
                name: "IX_QuizChiaSe_UserGuiID",
                table: "QuizChiaSe",
                column: "UserGuiID");

            migrationBuilder.CreateIndex(
                name: "IX_QuizChiaSe_UserNhanID",
                table: "QuizChiaSe",
                column: "UserNhanID");

            migrationBuilder.CreateIndex(
                name: "IX_QuizNgay_CauHoiID",
                table: "QuizNgay",
                column: "CauHoiID");

            migrationBuilder.CreateIndex(
                name: "IX_QuizTuyChinh_UserID",
                table: "QuizTuyChinh",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_ThanhTuu_NguoiDungID",
                table: "ThanhTuu",
                column: "NguoiDungID");

            migrationBuilder.CreateIndex(
                name: "IX_ThongKeNguoiDung_UserID",
                table: "ThongKeNguoiDung",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_ThuongNgay_UserID",
                table: "ThuongNgay",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_VaiTro_Quyen_QuyenID",
                table: "VaiTro_Quyen",
                column: "QuyenID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Admin");

            migrationBuilder.DropTable(
                name: "BXH");

            migrationBuilder.DropTable(
                name: "CaiDatNguoiDung");

            migrationBuilder.DropTable(
                name: "CauSai");

            migrationBuilder.DropTable(
                name: "ChuoiNgay");

            migrationBuilder.DropTable(
                name: "ClientKeys");

            migrationBuilder.DropTable(
                name: "Comment");

            migrationBuilder.DropTable(
                name: "KetQua");

            migrationBuilder.DropTable(
                name: "NguoiDungOnline");

            migrationBuilder.DropTable(
                name: "PhienDangNhap");

            migrationBuilder.DropTable(
                name: "QuizChiaSe");

            migrationBuilder.DropTable(
                name: "QuizNgay");

            migrationBuilder.DropTable(
                name: "SystemSettings");

            migrationBuilder.DropTable(
                name: "ThanhTuu");

            migrationBuilder.DropTable(
                name: "ThongKeNguoiDung");

            migrationBuilder.DropTable(
                name: "ThuongNgay");

            migrationBuilder.DropTable(
                name: "TroGiup");

            migrationBuilder.DropTable(
                name: "VaiTro_Quyen");

            migrationBuilder.DropTable(
                name: "QuizAttempt");

            migrationBuilder.DropTable(
                name: "CauHoi");

            migrationBuilder.DropTable(
                name: "Quyen");

            migrationBuilder.DropTable(
                name: "VaiTro");

            migrationBuilder.DropTable(
                name: "QuizTuyChinh");

            migrationBuilder.DropTable(
                name: "ChuDe");

            migrationBuilder.DropTable(
                name: "DoKho");

            migrationBuilder.DropTable(
                name: "NguoiDung");
        }
    }
}
