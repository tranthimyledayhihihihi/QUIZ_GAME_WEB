using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QUIZ_GAME_WEB.Migrations
{
    /// <inheritdoc />
    public partial class AddUGCFieldsToQuiz : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Admin",
                keyColumn: "AdminID",
                keyValue: 1,
                column: "NgayTao",
                value: new DateTime(2025, 12, 1, 20, 42, 56, 721, DateTimeKind.Local).AddTicks(8600));

            migrationBuilder.UpdateData(
                table: "CauHoi",
                keyColumn: "CauHoiID",
                keyValue: 1,
                column: "NgayTao",
                value: new DateTime(2025, 12, 1, 20, 42, 56, 721, DateTimeKind.Local).AddTicks(8695));

            migrationBuilder.UpdateData(
                table: "CauHoi",
                keyColumn: "CauHoiID",
                keyValue: 2,
                column: "NgayTao",
                value: new DateTime(2025, 12, 1, 20, 42, 56, 721, DateTimeKind.Local).AddTicks(8699));

            migrationBuilder.UpdateData(
                table: "CauHoi",
                keyColumn: "CauHoiID",
                keyValue: 3,
                column: "NgayTao",
                value: new DateTime(2025, 12, 1, 20, 42, 56, 721, DateTimeKind.Local).AddTicks(8702));

            migrationBuilder.UpdateData(
                table: "CauHoi",
                keyColumn: "CauHoiID",
                keyValue: 4,
                column: "NgayTao",
                value: new DateTime(2025, 12, 1, 20, 42, 56, 721, DateTimeKind.Local).AddTicks(8704));

            migrationBuilder.UpdateData(
                table: "ChuoiNgay",
                keyColumn: "ChuoiID",
                keyValue: 1,
                column: "NgayCapNhatCuoi",
                value: new DateTime(2025, 12, 1, 20, 42, 56, 721, DateTimeKind.Local).AddTicks(8862));

            migrationBuilder.UpdateData(
                table: "ChuoiNgay",
                keyColumn: "ChuoiID",
                keyValue: 2,
                column: "NgayCapNhatCuoi",
                value: new DateTime(2025, 12, 1, 20, 42, 56, 721, DateTimeKind.Local).AddTicks(8863));

            migrationBuilder.UpdateData(
                table: "KetQua",
                keyColumn: "KetQuaID",
                keyValue: 1,
                column: "ThoiGian",
                value: new DateTime(2025, 12, 1, 15, 42, 56, 721, DateTimeKind.Local).AddTicks(8803));

            migrationBuilder.UpdateData(
                table: "KetQua",
                keyColumn: "KetQuaID",
                keyValue: 2,
                column: "ThoiGian",
                value: new DateTime(2025, 12, 1, 19, 42, 56, 721, DateTimeKind.Local).AddTicks(8806));

            migrationBuilder.UpdateData(
                table: "NguoiDung",
                keyColumn: "UserID",
                keyValue: 1,
                column: "NgayDangKy",
                value: new DateTime(2025, 12, 1, 20, 42, 56, 721, DateTimeKind.Local).AddTicks(8554));

            migrationBuilder.UpdateData(
                table: "NguoiDung",
                keyColumn: "UserID",
                keyValue: 2,
                column: "NgayDangKy",
                value: new DateTime(2025, 12, 1, 20, 42, 56, 721, DateTimeKind.Local).AddTicks(8576));

            migrationBuilder.UpdateData(
                table: "NguoiDung",
                keyColumn: "UserID",
                keyValue: 3,
                column: "NgayDangKy",
                value: new DateTime(2025, 12, 1, 20, 42, 56, 721, DateTimeKind.Local).AddTicks(8579));

            migrationBuilder.UpdateData(
                table: "QuizAttempt",
                keyColumn: "QuizAttemptID",
                keyValue: 1,
                columns: new[] { "NgayBatDau", "NgayKetThuc" },
                values: new object[] { new DateTime(2025, 12, 1, 19, 42, 56, 721, DateTimeKind.Local).AddTicks(8740), new DateTime(2025, 12, 1, 20, 42, 56, 721, DateTimeKind.Local).AddTicks(8745) });

            migrationBuilder.UpdateData(
                table: "QuizAttempt",
                keyColumn: "QuizAttemptID",
                keyValue: 2,
                columns: new[] { "NgayBatDau", "NgayKetThuc" },
                values: new object[] { new DateTime(2025, 12, 1, 18, 42, 56, 721, DateTimeKind.Local).AddTicks(8750), new DateTime(2025, 12, 1, 20, 42, 56, 721, DateTimeKind.Local).AddTicks(8751) });

            migrationBuilder.UpdateData(
                table: "QuizChiaSe",
                keyColumn: "QuizChiaSeID",
                keyValue: 1,
                column: "NgayChiaSe",
                value: new DateTime(2025, 12, 1, 20, 42, 56, 721, DateTimeKind.Local).AddTicks(8900));

            migrationBuilder.UpdateData(
                table: "QuizTuyChinh",
                keyColumn: "QuizTuyChinhID",
                keyValue: 1,
                column: "NgayTao",
                value: new DateTime(2025, 12, 1, 20, 42, 56, 721, DateTimeKind.Local).AddTicks(8723));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Admin",
                keyColumn: "AdminID",
                keyValue: 1,
                column: "NgayTao",
                value: new DateTime(2025, 12, 1, 20, 35, 17, 156, DateTimeKind.Local).AddTicks(1876));

            migrationBuilder.UpdateData(
                table: "CauHoi",
                keyColumn: "CauHoiID",
                keyValue: 1,
                column: "NgayTao",
                value: new DateTime(2025, 12, 1, 20, 35, 17, 156, DateTimeKind.Local).AddTicks(1956));

            migrationBuilder.UpdateData(
                table: "CauHoi",
                keyColumn: "CauHoiID",
                keyValue: 2,
                column: "NgayTao",
                value: new DateTime(2025, 12, 1, 20, 35, 17, 156, DateTimeKind.Local).AddTicks(1961));

            migrationBuilder.UpdateData(
                table: "CauHoi",
                keyColumn: "CauHoiID",
                keyValue: 3,
                column: "NgayTao",
                value: new DateTime(2025, 12, 1, 20, 35, 17, 156, DateTimeKind.Local).AddTicks(1963));

            migrationBuilder.UpdateData(
                table: "CauHoi",
                keyColumn: "CauHoiID",
                keyValue: 4,
                column: "NgayTao",
                value: new DateTime(2025, 12, 1, 20, 35, 17, 156, DateTimeKind.Local).AddTicks(1964));

            migrationBuilder.UpdateData(
                table: "ChuoiNgay",
                keyColumn: "ChuoiID",
                keyValue: 1,
                column: "NgayCapNhatCuoi",
                value: new DateTime(2025, 12, 1, 20, 35, 17, 156, DateTimeKind.Local).AddTicks(2105));

            migrationBuilder.UpdateData(
                table: "ChuoiNgay",
                keyColumn: "ChuoiID",
                keyValue: 2,
                column: "NgayCapNhatCuoi",
                value: new DateTime(2025, 12, 1, 20, 35, 17, 156, DateTimeKind.Local).AddTicks(2107));

            migrationBuilder.UpdateData(
                table: "KetQua",
                keyColumn: "KetQuaID",
                keyValue: 1,
                column: "ThoiGian",
                value: new DateTime(2025, 12, 1, 15, 35, 17, 156, DateTimeKind.Local).AddTicks(2058));

            migrationBuilder.UpdateData(
                table: "KetQua",
                keyColumn: "KetQuaID",
                keyValue: 2,
                column: "ThoiGian",
                value: new DateTime(2025, 12, 1, 19, 35, 17, 156, DateTimeKind.Local).AddTicks(2060));

            migrationBuilder.UpdateData(
                table: "NguoiDung",
                keyColumn: "UserID",
                keyValue: 1,
                column: "NgayDangKy",
                value: new DateTime(2025, 12, 1, 20, 35, 17, 156, DateTimeKind.Local).AddTicks(1837));

            migrationBuilder.UpdateData(
                table: "NguoiDung",
                keyColumn: "UserID",
                keyValue: 2,
                column: "NgayDangKy",
                value: new DateTime(2025, 12, 1, 20, 35, 17, 156, DateTimeKind.Local).AddTicks(1856));

            migrationBuilder.UpdateData(
                table: "NguoiDung",
                keyColumn: "UserID",
                keyValue: 3,
                column: "NgayDangKy",
                value: new DateTime(2025, 12, 1, 20, 35, 17, 156, DateTimeKind.Local).AddTicks(1859));

            migrationBuilder.UpdateData(
                table: "QuizAttempt",
                keyColumn: "QuizAttemptID",
                keyValue: 1,
                columns: new[] { "NgayBatDau", "NgayKetThuc" },
                values: new object[] { new DateTime(2025, 12, 1, 19, 35, 17, 156, DateTimeKind.Local).AddTicks(2033), new DateTime(2025, 12, 1, 20, 35, 17, 156, DateTimeKind.Local).AddTicks(2037) });

            migrationBuilder.UpdateData(
                table: "QuizAttempt",
                keyColumn: "QuizAttemptID",
                keyValue: 2,
                columns: new[] { "NgayBatDau", "NgayKetThuc" },
                values: new object[] { new DateTime(2025, 12, 1, 18, 35, 17, 156, DateTimeKind.Local).AddTicks(2042), new DateTime(2025, 12, 1, 20, 35, 17, 156, DateTimeKind.Local).AddTicks(2042) });

            migrationBuilder.UpdateData(
                table: "QuizChiaSe",
                keyColumn: "QuizChiaSeID",
                keyValue: 1,
                column: "NgayChiaSe",
                value: new DateTime(2025, 12, 1, 20, 35, 17, 156, DateTimeKind.Local).AddTicks(2139));

            migrationBuilder.UpdateData(
                table: "QuizTuyChinh",
                keyColumn: "QuizTuyChinhID",
                keyValue: 1,
                column: "NgayTao",
                value: new DateTime(2025, 12, 1, 20, 35, 17, 156, DateTimeKind.Local).AddTicks(2015));
        }
    }
}
