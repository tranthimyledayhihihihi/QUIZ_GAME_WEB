using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QUIZ_GAME_WEB.Migrations
{
    /// <inheritdoc />
    public partial class AddUGCFieldsToQuizTuyChinh : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Admin",
                keyColumn: "AdminID",
                keyValue: 1,
                column: "NgayTao",
                value: new DateTime(2025, 12, 1, 21, 3, 42, 791, DateTimeKind.Local).AddTicks(9521));

            migrationBuilder.UpdateData(
                table: "CauHoi",
                keyColumn: "CauHoiID",
                keyValue: 1,
                column: "NgayTao",
                value: new DateTime(2025, 12, 1, 21, 3, 42, 791, DateTimeKind.Local).AddTicks(9671));

            migrationBuilder.UpdateData(
                table: "CauHoi",
                keyColumn: "CauHoiID",
                keyValue: 2,
                column: "NgayTao",
                value: new DateTime(2025, 12, 1, 21, 3, 42, 791, DateTimeKind.Local).AddTicks(9677));

            migrationBuilder.UpdateData(
                table: "CauHoi",
                keyColumn: "CauHoiID",
                keyValue: 3,
                column: "NgayTao",
                value: new DateTime(2025, 12, 1, 21, 3, 42, 791, DateTimeKind.Local).AddTicks(9679));

            migrationBuilder.UpdateData(
                table: "CauHoi",
                keyColumn: "CauHoiID",
                keyValue: 4,
                column: "NgayTao",
                value: new DateTime(2025, 12, 1, 21, 3, 42, 791, DateTimeKind.Local).AddTicks(9681));

            migrationBuilder.UpdateData(
                table: "ChuoiNgay",
                keyColumn: "ChuoiID",
                keyValue: 1,
                column: "NgayCapNhatCuoi",
                value: new DateTime(2025, 12, 1, 21, 3, 42, 791, DateTimeKind.Local).AddTicks(9858));

            migrationBuilder.UpdateData(
                table: "ChuoiNgay",
                keyColumn: "ChuoiID",
                keyValue: 2,
                column: "NgayCapNhatCuoi",
                value: new DateTime(2025, 12, 1, 21, 3, 42, 791, DateTimeKind.Local).AddTicks(9860));

            migrationBuilder.UpdateData(
                table: "KetQua",
                keyColumn: "KetQuaID",
                keyValue: 1,
                column: "ThoiGian",
                value: new DateTime(2025, 12, 1, 16, 3, 42, 791, DateTimeKind.Local).AddTicks(9775));

            migrationBuilder.UpdateData(
                table: "KetQua",
                keyColumn: "KetQuaID",
                keyValue: 2,
                column: "ThoiGian",
                value: new DateTime(2025, 12, 1, 20, 3, 42, 791, DateTimeKind.Local).AddTicks(9778));

            migrationBuilder.UpdateData(
                table: "NguoiDung",
                keyColumn: "UserID",
                keyValue: 1,
                column: "NgayDangKy",
                value: new DateTime(2025, 12, 1, 21, 3, 42, 791, DateTimeKind.Local).AddTicks(9434));

            migrationBuilder.UpdateData(
                table: "NguoiDung",
                keyColumn: "UserID",
                keyValue: 2,
                column: "NgayDangKy",
                value: new DateTime(2025, 12, 1, 21, 3, 42, 791, DateTimeKind.Local).AddTicks(9464));

            migrationBuilder.UpdateData(
                table: "NguoiDung",
                keyColumn: "UserID",
                keyValue: 3,
                column: "NgayDangKy",
                value: new DateTime(2025, 12, 1, 21, 3, 42, 791, DateTimeKind.Local).AddTicks(9466));

            migrationBuilder.UpdateData(
                table: "QuizAttempt",
                keyColumn: "QuizAttemptID",
                keyValue: 1,
                columns: new[] { "NgayBatDau", "NgayKetThuc" },
                values: new object[] { new DateTime(2025, 12, 1, 20, 3, 42, 791, DateTimeKind.Local).AddTicks(9736), new DateTime(2025, 12, 1, 21, 3, 42, 791, DateTimeKind.Local).AddTicks(9741) });

            migrationBuilder.UpdateData(
                table: "QuizAttempt",
                keyColumn: "QuizAttemptID",
                keyValue: 2,
                columns: new[] { "NgayBatDau", "NgayKetThuc" },
                values: new object[] { new DateTime(2025, 12, 1, 19, 3, 42, 791, DateTimeKind.Local).AddTicks(9746), new DateTime(2025, 12, 1, 21, 3, 42, 791, DateTimeKind.Local).AddTicks(9747) });

            migrationBuilder.UpdateData(
                table: "QuizChiaSe",
                keyColumn: "QuizChiaSeID",
                keyValue: 1,
                column: "NgayChiaSe",
                value: new DateTime(2025, 12, 1, 21, 3, 42, 791, DateTimeKind.Local).AddTicks(9918));

            migrationBuilder.UpdateData(
                table: "QuizTuyChinh",
                keyColumn: "QuizTuyChinhID",
                keyValue: 1,
                column: "NgayTao",
                value: new DateTime(2025, 12, 1, 21, 3, 42, 791, DateTimeKind.Local).AddTicks(9712));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Admin",
                keyColumn: "AdminID",
                keyValue: 1,
                column: "NgayTao",
                value: new DateTime(2025, 12, 1, 20, 18, 53, 496, DateTimeKind.Local).AddTicks(799));

            migrationBuilder.UpdateData(
                table: "CauHoi",
                keyColumn: "CauHoiID",
                keyValue: 1,
                column: "NgayTao",
                value: new DateTime(2025, 12, 1, 20, 18, 53, 496, DateTimeKind.Local).AddTicks(948));

            migrationBuilder.UpdateData(
                table: "CauHoi",
                keyColumn: "CauHoiID",
                keyValue: 2,
                column: "NgayTao",
                value: new DateTime(2025, 12, 1, 20, 18, 53, 496, DateTimeKind.Local).AddTicks(951));

            migrationBuilder.UpdateData(
                table: "CauHoi",
                keyColumn: "CauHoiID",
                keyValue: 3,
                column: "NgayTao",
                value: new DateTime(2025, 12, 1, 20, 18, 53, 496, DateTimeKind.Local).AddTicks(953));

            migrationBuilder.UpdateData(
                table: "CauHoi",
                keyColumn: "CauHoiID",
                keyValue: 4,
                column: "NgayTao",
                value: new DateTime(2025, 12, 1, 20, 18, 53, 496, DateTimeKind.Local).AddTicks(956));

            migrationBuilder.UpdateData(
                table: "ChuoiNgay",
                keyColumn: "ChuoiID",
                keyValue: 1,
                column: "NgayCapNhatCuoi",
                value: new DateTime(2025, 12, 1, 20, 18, 53, 496, DateTimeKind.Local).AddTicks(1094));

            migrationBuilder.UpdateData(
                table: "ChuoiNgay",
                keyColumn: "ChuoiID",
                keyValue: 2,
                column: "NgayCapNhatCuoi",
                value: new DateTime(2025, 12, 1, 20, 18, 53, 496, DateTimeKind.Local).AddTicks(1095));

            migrationBuilder.UpdateData(
                table: "KetQua",
                keyColumn: "KetQuaID",
                keyValue: 1,
                column: "ThoiGian",
                value: new DateTime(2025, 12, 1, 15, 18, 53, 496, DateTimeKind.Local).AddTicks(1035));

            migrationBuilder.UpdateData(
                table: "KetQua",
                keyColumn: "KetQuaID",
                keyValue: 2,
                column: "ThoiGian",
                value: new DateTime(2025, 12, 1, 19, 18, 53, 496, DateTimeKind.Local).AddTicks(1037));

            migrationBuilder.UpdateData(
                table: "NguoiDung",
                keyColumn: "UserID",
                keyValue: 1,
                column: "NgayDangKy",
                value: new DateTime(2025, 12, 1, 20, 18, 53, 496, DateTimeKind.Local).AddTicks(751));

            migrationBuilder.UpdateData(
                table: "NguoiDung",
                keyColumn: "UserID",
                keyValue: 2,
                column: "NgayDangKy",
                value: new DateTime(2025, 12, 1, 20, 18, 53, 496, DateTimeKind.Local).AddTicks(771));

            migrationBuilder.UpdateData(
                table: "NguoiDung",
                keyColumn: "UserID",
                keyValue: 3,
                column: "NgayDangKy",
                value: new DateTime(2025, 12, 1, 20, 18, 53, 496, DateTimeKind.Local).AddTicks(774));

            migrationBuilder.UpdateData(
                table: "QuizAttempt",
                keyColumn: "QuizAttemptID",
                keyValue: 1,
                columns: new[] { "NgayBatDau", "NgayKetThuc" },
                values: new object[] { new DateTime(2025, 12, 1, 19, 18, 53, 496, DateTimeKind.Local).AddTicks(1002), new DateTime(2025, 12, 1, 20, 18, 53, 496, DateTimeKind.Local).AddTicks(1008) });

            migrationBuilder.UpdateData(
                table: "QuizAttempt",
                keyColumn: "QuizAttemptID",
                keyValue: 2,
                columns: new[] { "NgayBatDau", "NgayKetThuc" },
                values: new object[] { new DateTime(2025, 12, 1, 18, 18, 53, 496, DateTimeKind.Local).AddTicks(1014), new DateTime(2025, 12, 1, 20, 18, 53, 496, DateTimeKind.Local).AddTicks(1014) });

            migrationBuilder.UpdateData(
                table: "QuizChiaSe",
                keyColumn: "QuizChiaSeID",
                keyValue: 1,
                column: "NgayChiaSe",
                value: new DateTime(2025, 12, 1, 20, 18, 53, 496, DateTimeKind.Local).AddTicks(1137));

            migrationBuilder.UpdateData(
                table: "QuizTuyChinh",
                keyColumn: "QuizTuyChinhID",
                keyValue: 1,
                column: "NgayTao",
                value: new DateTime(2025, 12, 1, 20, 18, 53, 496, DateTimeKind.Local).AddTicks(981));
        }
    }
}
