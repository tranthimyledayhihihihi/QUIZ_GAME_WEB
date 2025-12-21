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
            migrationBuilder.AddColumn<int>(
                name: "AdminDuyetID",
                table: "QuizTuyChinh",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "NgayDuyet",
                table: "QuizTuyChinh",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TrangThai",
                table: "QuizTuyChinh",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "QuizTuyChinhID",
                table: "CauHoi",
                type: "int",
                nullable: true);

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
                columns: new[] { "NgayTao", "QuizTuyChinhID" },
                values: new object[] { new DateTime(2025, 12, 1, 20, 18, 53, 496, DateTimeKind.Local).AddTicks(948), null });

            migrationBuilder.UpdateData(
                table: "CauHoi",
                keyColumn: "CauHoiID",
                keyValue: 2,
                columns: new[] { "NgayTao", "QuizTuyChinhID" },
                values: new object[] { new DateTime(2025, 12, 1, 20, 18, 53, 496, DateTimeKind.Local).AddTicks(951), null });

            migrationBuilder.UpdateData(
                table: "CauHoi",
                keyColumn: "CauHoiID",
                keyValue: 3,
                columns: new[] { "NgayTao", "QuizTuyChinhID" },
                values: new object[] { new DateTime(2025, 12, 1, 20, 18, 53, 496, DateTimeKind.Local).AddTicks(953), null });

            migrationBuilder.UpdateData(
                table: "CauHoi",
                keyColumn: "CauHoiID",
                keyValue: 4,
                columns: new[] { "NgayTao", "QuizTuyChinhID" },
                values: new object[] { new DateTime(2025, 12, 1, 20, 18, 53, 496, DateTimeKind.Local).AddTicks(956), null });

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
                columns: new[] { "AdminDuyetID", "NgayDuyet", "NgayTao", "TrangThai" },
                values: new object[] { null, null, new DateTime(2025, 12, 1, 20, 18, 53, 496, DateTimeKind.Local).AddTicks(981), "Pending" });

            migrationBuilder.CreateIndex(
                name: "IX_CauHoi_QuizTuyChinhID",
                table: "CauHoi",
                column: "QuizTuyChinhID");

            migrationBuilder.AddForeignKey(
                name: "FK_CauHoi_QuizTuyChinh_QuizTuyChinhID",
                table: "CauHoi",
                column: "QuizTuyChinhID",
                principalTable: "QuizTuyChinh",
                principalColumn: "QuizTuyChinhID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CauHoi_QuizTuyChinh_QuizTuyChinhID",
                table: "CauHoi");

            migrationBuilder.DropIndex(
                name: "IX_CauHoi_QuizTuyChinhID",
                table: "CauHoi");

            migrationBuilder.DropColumn(
                name: "AdminDuyetID",
                table: "QuizTuyChinh");

            migrationBuilder.DropColumn(
                name: "NgayDuyet",
                table: "QuizTuyChinh");

            migrationBuilder.DropColumn(
                name: "TrangThai",
                table: "QuizTuyChinh");

            migrationBuilder.DropColumn(
                name: "QuizTuyChinhID",
                table: "CauHoi");

            migrationBuilder.UpdateData(
                table: "Admin",
                keyColumn: "AdminID",
                keyValue: 1,
                column: "NgayTao",
                value: new DateTime(2025, 12, 1, 15, 33, 2, 325, DateTimeKind.Local).AddTicks(4192));

            migrationBuilder.UpdateData(
                table: "CauHoi",
                keyColumn: "CauHoiID",
                keyValue: 1,
                column: "NgayTao",
                value: new DateTime(2025, 12, 1, 15, 33, 2, 325, DateTimeKind.Local).AddTicks(4300));

            migrationBuilder.UpdateData(
                table: "CauHoi",
                keyColumn: "CauHoiID",
                keyValue: 2,
                column: "NgayTao",
                value: new DateTime(2025, 12, 1, 15, 33, 2, 325, DateTimeKind.Local).AddTicks(4304));

            migrationBuilder.UpdateData(
                table: "CauHoi",
                keyColumn: "CauHoiID",
                keyValue: 3,
                column: "NgayTao",
                value: new DateTime(2025, 12, 1, 15, 33, 2, 325, DateTimeKind.Local).AddTicks(4306));

            migrationBuilder.UpdateData(
                table: "CauHoi",
                keyColumn: "CauHoiID",
                keyValue: 4,
                column: "NgayTao",
                value: new DateTime(2025, 12, 1, 15, 33, 2, 325, DateTimeKind.Local).AddTicks(4308));

            migrationBuilder.UpdateData(
                table: "ChuoiNgay",
                keyColumn: "ChuoiID",
                keyValue: 1,
                column: "NgayCapNhatCuoi",
                value: new DateTime(2025, 12, 1, 15, 33, 2, 325, DateTimeKind.Local).AddTicks(4450));

            migrationBuilder.UpdateData(
                table: "ChuoiNgay",
                keyColumn: "ChuoiID",
                keyValue: 2,
                column: "NgayCapNhatCuoi",
                value: new DateTime(2025, 12, 1, 15, 33, 2, 325, DateTimeKind.Local).AddTicks(4453));

            migrationBuilder.UpdateData(
                table: "KetQua",
                keyColumn: "KetQuaID",
                keyValue: 1,
                column: "ThoiGian",
                value: new DateTime(2025, 12, 1, 10, 33, 2, 325, DateTimeKind.Local).AddTicks(4387));

            migrationBuilder.UpdateData(
                table: "KetQua",
                keyColumn: "KetQuaID",
                keyValue: 2,
                column: "ThoiGian",
                value: new DateTime(2025, 12, 1, 14, 33, 2, 325, DateTimeKind.Local).AddTicks(4390));

            migrationBuilder.UpdateData(
                table: "NguoiDung",
                keyColumn: "UserID",
                keyValue: 1,
                column: "NgayDangKy",
                value: new DateTime(2025, 12, 1, 15, 33, 2, 325, DateTimeKind.Local).AddTicks(4103));

            migrationBuilder.UpdateData(
                table: "NguoiDung",
                keyColumn: "UserID",
                keyValue: 2,
                column: "NgayDangKy",
                value: new DateTime(2025, 12, 1, 15, 33, 2, 325, DateTimeKind.Local).AddTicks(4124));

            migrationBuilder.UpdateData(
                table: "NguoiDung",
                keyColumn: "UserID",
                keyValue: 3,
                column: "NgayDangKy",
                value: new DateTime(2025, 12, 1, 15, 33, 2, 325, DateTimeKind.Local).AddTicks(4166));

            migrationBuilder.UpdateData(
                table: "QuizAttempt",
                keyColumn: "QuizAttemptID",
                keyValue: 1,
                columns: new[] { "NgayBatDau", "NgayKetThuc" },
                values: new object[] { new DateTime(2025, 12, 1, 14, 33, 2, 325, DateTimeKind.Local).AddTicks(4352), new DateTime(2025, 12, 1, 15, 33, 2, 325, DateTimeKind.Local).AddTicks(4359) });

            migrationBuilder.UpdateData(
                table: "QuizAttempt",
                keyColumn: "QuizAttemptID",
                keyValue: 2,
                columns: new[] { "NgayBatDau", "NgayKetThuc" },
                values: new object[] { new DateTime(2025, 12, 1, 13, 33, 2, 325, DateTimeKind.Local).AddTicks(4364), new DateTime(2025, 12, 1, 15, 33, 2, 325, DateTimeKind.Local).AddTicks(4365) });

            migrationBuilder.UpdateData(
                table: "QuizChiaSe",
                keyColumn: "QuizChiaSeID",
                keyValue: 1,
                column: "NgayChiaSe",
                value: new DateTime(2025, 12, 1, 15, 33, 2, 325, DateTimeKind.Local).AddTicks(4496));

            migrationBuilder.UpdateData(
                table: "QuizTuyChinh",
                keyColumn: "QuizTuyChinhID",
                keyValue: 1,
                column: "NgayTao",
                value: new DateTime(2025, 12, 1, 15, 33, 2, 325, DateTimeKind.Local).AddTicks(4330));
        }
    }
}
