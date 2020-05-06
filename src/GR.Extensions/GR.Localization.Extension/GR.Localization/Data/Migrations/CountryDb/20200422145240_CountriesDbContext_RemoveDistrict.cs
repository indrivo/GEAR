using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GR.Localization.Data.Migrations.CountryDb
{
    public partial class CountriesDbContext_RemoveDistrict : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Districts",
                schema: "Localization");

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("01217f05-7835-4201-95d2-88e8b8cba693"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(182), new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(182) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("0157a8ae-ea59-47ac-895c-56666e06a0e1"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 88, DateTimeKind.Utc).AddTicks(2046), new DateTime(2020, 4, 22, 14, 52, 39, 88, DateTimeKind.Utc).AddTicks(2045) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("02322cf7-2ef8-4e1e-9b78-3848349c1296"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(8125), new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(8124) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("030c4a96-8e05-4883-af1d-718449051743"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 86, DateTimeKind.Utc).AddTicks(8542), new DateTime(2020, 4, 22, 14, 52, 39, 86, DateTimeKind.Utc).AddTicks(8541) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("035dde65-a582-43af-b47c-e6413fd4e881"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 88, DateTimeKind.Utc).AddTicks(4432), new DateTime(2020, 4, 22, 14, 52, 39, 88, DateTimeKind.Utc).AddTicks(4432) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("03beb5af-8897-4df8-a1e5-ea49c91e2955"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 88, DateTimeKind.Utc).AddTicks(2120), new DateTime(2020, 4, 22, 14, 52, 39, 88, DateTimeKind.Utc).AddTicks(2119) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("05e84190-9cd1-4959-9322-25c2b1a62220"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 86, DateTimeKind.Utc).AddTicks(9472), new DateTime(2020, 4, 22, 14, 52, 39, 86, DateTimeKind.Utc).AddTicks(9472) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("08aed749-f7a3-41b5-b88f-a7773f5c3155"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(6262), new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(6262) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("0bb8c15c-a4f6-4dce-8451-b1e5815b5186"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 88, DateTimeKind.Utc).AddTicks(3928), new DateTime(2020, 4, 22, 14, 52, 39, 88, DateTimeKind.Utc).AddTicks(3928) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("0cba73ae-9042-4b2a-bab6-6c3185764234"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 86, DateTimeKind.Utc).AddTicks(4370), new DateTime(2020, 4, 22, 14, 52, 39, 86, DateTimeKind.Utc).AddTicks(4370) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("0d622cb6-c520-4aea-acc2-c91dd5b62c9c"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 88, DateTimeKind.Utc).AddTicks(4690), new DateTime(2020, 4, 22, 14, 52, 39, 88, DateTimeKind.Utc).AddTicks(4690) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("10055208-fe88-46e9-80b6-e6a56a09b168"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 88, DateTimeKind.Utc).AddTicks(2265), new DateTime(2020, 4, 22, 14, 52, 39, 88, DateTimeKind.Utc).AddTicks(2265) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("1066262e-d59c-4a0f-a687-cf5c1637ec29"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 77, DateTimeKind.Utc).AddTicks(7328), new DateTime(2020, 4, 22, 14, 52, 39, 77, DateTimeKind.Utc).AddTicks(6726) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("11750caa-2337-4a7b-9e6e-fcbdc4b1a4c6"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(2379), new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(2379) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("12f0754e-1431-48bc-a8ab-619aaff44c58"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 88, DateTimeKind.Utc).AddTicks(1696), new DateTime(2020, 4, 22, 14, 52, 39, 88, DateTimeKind.Utc).AddTicks(1696) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("1398338a-0d71-452e-895d-15f4da7ef403"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 86, DateTimeKind.Utc).AddTicks(9402), new DateTime(2020, 4, 22, 14, 52, 39, 86, DateTimeKind.Utc).AddTicks(9402) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("1582d1ff-8978-423a-9f93-1cb8c6193804"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 88, DateTimeKind.Utc).AddTicks(4361), new DateTime(2020, 4, 22, 14, 52, 39, 88, DateTimeKind.Utc).AddTicks(4361) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("15d53299-341d-40d1-8444-032030d215e9"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(5925), new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(5924) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("15eff380-20c7-4558-895c-54ba17871876"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 86, DateTimeKind.Utc).AddTicks(1328), new DateTime(2020, 4, 22, 14, 52, 39, 86, DateTimeKind.Utc).AddTicks(1328) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("1696bb6c-d6a7-4632-ba0b-72a4f414c8a9"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 88, DateTimeKind.Utc).AddTicks(3005), new DateTime(2020, 4, 22, 14, 52, 39, 88, DateTimeKind.Utc).AddTicks(3005) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("1726b8e1-71ac-4ded-826f-690f36e3d604"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(5638), new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(5638) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("17fbf326-d021-43d4-9b4d-05f797308af2"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(8594), new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(8594) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("182008f1-d0e7-488c-beb7-df44a0c3186a"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 86, DateTimeKind.Utc).AddTicks(8842), new DateTime(2020, 4, 22, 14, 52, 39, 86, DateTimeKind.Utc).AddTicks(8842) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("18c02c8f-b1ad-4f35-ac0b-9d48a0669d60"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(3220), new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(3220) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("18c3b29d-a45f-4ad6-852f-0f15801d8570"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(7026), new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(7026) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("18d18037-4977-42b1-a73d-0b5cbb32995c"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 88, DateTimeKind.Utc).AddTicks(1910), new DateTime(2020, 4, 22, 14, 52, 39, 88, DateTimeKind.Utc).AddTicks(1909) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("18e435e8-bf1b-424d-adc8-5cb949bb23b5"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(1313), new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(1313) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("1947c227-efb2-4b8a-b625-cbef979d9c30"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(3433), new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(3433) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("1995b10e-fb35-484f-abe5-e322cb92bb04"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(9430), new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(9430) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("19ec185a-a771-4b56-8687-4289e1dcdbc1"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(6115), new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(6114) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("1c4653b4-3d39-46f9-aa93-7e499ba603ba"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(5066), new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(5066) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("1c5f6d4f-a519-4095-a2d1-4575e32d1cf0"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(2450), new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(2450) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("1cda93a0-d3ef-4c53-ac50-b7a7b73abf51"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(837), new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(837) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("1d6271ae-3ad6-4836-81b4-3be1405f03f4"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 88, DateTimeKind.Utc).AddTicks(4290), new DateTime(2020, 4, 22, 14, 52, 39, 88, DateTimeKind.Utc).AddTicks(4290) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("1f841bb5-ca53-4034-bed4-9c381c8c30bb"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(5565), new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(5564) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("1f8eb873-4f12-4aff-a30a-ff0b576f6966"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(4483), new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(4482) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("20125248-13af-47f9-bd32-58cf07ee2a21"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(6691), new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(6691) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("20b0b0de-5c3b-4bfe-b01d-93ea9f638738"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 86, DateTimeKind.Utc).AddTicks(1645), new DateTime(2020, 4, 22, 14, 52, 39, 86, DateTimeKind.Utc).AddTicks(1645) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("20bfcca0-1df5-4f79-b33f-55178e6755f6"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(4009), new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(4009) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("2107778f-bac0-47b2-b556-ad528865d593"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(3078), new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(3077) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("21d20ec8-60e0-4961-acd7-191c23e45914"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(5418), new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(5418) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("232fed5a-c4fa-4382-ab39-4743ba2c00ef"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 88, DateTimeKind.Utc).AddTicks(1767), new DateTime(2020, 4, 22, 14, 52, 39, 88, DateTimeKind.Utc).AddTicks(1767) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("23d1efb0-d504-4b17-b8aa-782b0473a8cf"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(9319), new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(9319) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("261df7b9-2bbd-422f-95ba-ffb319ab8822"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 88, DateTimeKind.Utc).AddTicks(488), new DateTime(2020, 4, 22, 14, 52, 39, 88, DateTimeKind.Utc).AddTicks(488) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("264300cb-38c2-4eff-ad3b-4f8ea66654c2"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 86, DateTimeKind.Utc).AddTicks(4898), new DateTime(2020, 4, 22, 14, 52, 39, 86, DateTimeKind.Utc).AddTicks(4898) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("2670f213-d73c-4948-bc78-01982365c270"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 88, DateTimeKind.Utc).AddTicks(1476), new DateTime(2020, 4, 22, 14, 52, 39, 88, DateTimeKind.Utc).AddTicks(1476) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("268747f5-fead-4398-b046-43ae2ca51012"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 88, DateTimeKind.Utc).AddTicks(1840), new DateTime(2020, 4, 22, 14, 52, 39, 88, DateTimeKind.Utc).AddTicks(1839) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("26daf397-0a9d-44af-ae74-9940d456c3e5"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 86, DateTimeKind.Utc).AddTicks(1407), new DateTime(2020, 4, 22, 14, 52, 39, 86, DateTimeKind.Utc).AddTicks(1407) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("27120855-8e09-42f3-8b5c-d8323ac4ef8f"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(4558), new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(4558) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("27d521a1-6712-4299-ad77-943a6398042a"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 86, DateTimeKind.Utc).AddTicks(4671), new DateTime(2020, 4, 22, 14, 52, 39, 86, DateTimeKind.Utc).AddTicks(4671) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("286ca324-9ed4-4b8c-b4fc-a0f0ea80e581"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(6956), new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(6956) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("291a766f-6bb7-4065-8cec-1ca3ace2d8e5"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(1943), new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(1943) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("2b1181ea-7f0a-47ce-902c-9be62633eac4"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(8339), new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(8338) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("30fecce2-317f-4d97-aeec-f0a0dad1de94"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 86, DateTimeKind.Utc).AddTicks(8368), new DateTime(2020, 4, 22, 14, 52, 39, 86, DateTimeKind.Utc).AddTicks(8367) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("3177d852-5deb-489a-a3ee-e9e5a9c402a0"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(4631), new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(4631) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("31d589d2-5935-47a9-89af-0d268c1eb414"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(1454), new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(1453) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("338dcf64-83d0-42ee-bd94-144144d9ffba"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 88, DateTimeKind.Utc).AddTicks(3222), new DateTime(2020, 4, 22, 14, 52, 39, 88, DateTimeKind.Utc).AddTicks(3222) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("33cd468e-0e16-4c79-b209-e522b52f89b6"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 88, DateTimeKind.Utc).AddTicks(560), new DateTime(2020, 4, 22, 14, 52, 39, 88, DateTimeKind.Utc).AddTicks(560) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("3472258b-0b58-4685-af92-8dbe932ab87d"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 86, DateTimeKind.Utc).AddTicks(8695), new DateTime(2020, 4, 22, 14, 52, 39, 86, DateTimeKind.Utc).AddTicks(8695) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("38054345-534d-4490-b17e-dfb7a2588deb"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 88, DateTimeKind.Utc).AddTicks(3655), new DateTime(2020, 4, 22, 14, 52, 39, 88, DateTimeKind.Utc).AddTicks(3655) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("38b1356a-9e47-480c-8d61-cdd7b9b38259"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 88, DateTimeKind.Utc).AddTicks(2775), new DateTime(2020, 4, 22, 14, 52, 39, 88, DateTimeKind.Utc).AddTicks(2775) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("3bd22ab9-6136-4106-94ad-60133641a150"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 86, DateTimeKind.Utc).AddTicks(4519), new DateTime(2020, 4, 22, 14, 52, 39, 86, DateTimeKind.Utc).AddTicks(4519) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("4049e1b0-7629-43d4-93f1-d9516907cc26"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 88, DateTimeKind.Utc).AddTicks(1331), new DateTime(2020, 4, 22, 14, 52, 39, 88, DateTimeKind.Utc).AddTicks(1331) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("4565ffeb-1d26-49fc-82b3-156dd5bbed22"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(2014), new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(2014) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("45b8db8c-8136-4e73-a923-1d19a32102ea"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(4223), new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(4223) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("45dbf8fa-2776-4722-a199-0fba104f9e37"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 86, DateTimeKind.Utc).AddTicks(7225), new DateTime(2020, 4, 22, 14, 52, 39, 86, DateTimeKind.Utc).AddTicks(7225) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("4650b837-4a79-4e9c-89ec-620b6e8ec075"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 88, DateTimeKind.Utc).AddTicks(4220), new DateTime(2020, 4, 22, 14, 52, 39, 88, DateTimeKind.Utc).AddTicks(4219) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("48c21470-c276-4ffc-bc29-7a2ce0126d78"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 88, DateTimeKind.Utc).AddTicks(2194), new DateTime(2020, 4, 22, 14, 52, 39, 88, DateTimeKind.Utc).AddTicks(2193) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("48c3a204-2cbf-4752-8ac5-2ce167be4772"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(399), new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(398) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("49520892-e40e-4294-af7d-c79d4edee393"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(8482), new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(8482) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("4b985e4c-fb54-4a66-890d-075d3104d032"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 88, DateTimeKind.Utc).AddTicks(3584), new DateTime(2020, 4, 22, 14, 52, 39, 88, DateTimeKind.Utc).AddTicks(3584) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("4ba13eda-8479-408f-a5b9-696d474d04b1"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(2160), new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(2160) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("4c1f43c7-47f4-403b-8543-96cf07ce10de"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(3333), new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(3332) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("4d5a36ed-2da3-48aa-88c7-24329188bcdf"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(9581), new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(9580) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("506333bb-aea4-40e0-910b-6191972e5371"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(4299), new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(4298) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("518fe0de-b9c6-4690-bef5-3f74d95940a6"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(6766), new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(6766) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("520af57c-bbde-4475-8253-dc6ce18323c9"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(4154), new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(4153) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("5226e4cf-40ad-42e0-996c-83daf464d3bf"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 86, DateTimeKind.Utc).AddTicks(4448), new DateTime(2020, 4, 22, 14, 52, 39, 86, DateTimeKind.Utc).AddTicks(4447) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("52488d7f-eb89-4c34-9099-e9e25fd54164"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 86, DateTimeKind.Utc).AddTicks(8772), new DateTime(2020, 4, 22, 14, 52, 39, 86, DateTimeKind.Utc).AddTicks(8772) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("5297f23d-a50f-4f75-91c2-8719d4c58cc2"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(2594), new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(2594) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("5331378c-2a7e-496d-b64f-fc19f1cc79b9"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 86, DateTimeKind.Utc).AddTicks(1036), new DateTime(2020, 4, 22, 14, 52, 39, 86, DateTimeKind.Utc).AddTicks(1035) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("53430b22-1351-4a76-a56f-d562c3784df5"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 88, DateTimeKind.Utc).AddTicks(1260), new DateTime(2020, 4, 22, 14, 52, 39, 88, DateTimeKind.Utc).AddTicks(1259) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("5472c425-37ea-4ede-836c-8eb4ce20d343"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(9869), new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(9869) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("54ece6d5-f31c-40b7-acc0-728bb78fa2b6"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(7599), new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(7599) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("55285133-f5c0-4853-82b9-4d656cc4f9af"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(5783), new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(5782) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("554fda2f-d0e4-4c2c-b522-359806fc2c21"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 88, DateTimeKind.Utc).AddTicks(3508), new DateTime(2020, 4, 22, 14, 52, 39, 88, DateTimeKind.Utc).AddTicks(3508) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("56139794-ac33-46e2-8cc5-88dc4cab2350"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 86, DateTimeKind.Utc).AddTicks(7708), new DateTime(2020, 4, 22, 14, 52, 39, 86, DateTimeKind.Utc).AddTicks(7708) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("59709a12-0413-431c-9ac7-89795004d386"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(626), new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(625) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("598800ff-029f-4aa2-9c28-985b17cc77c9"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 88, DateTimeKind.Utc).AddTicks(1068), new DateTime(2020, 4, 22, 14, 52, 39, 88, DateTimeKind.Utc).AddTicks(1068) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("5c9a109e-e4c6-434b-bc67-2ed0c6b0713c"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(1527), new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(1527) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("5fca9a3c-8b1c-4167-91ce-800f6c1391b6"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(6549), new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(6548) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("61c24c18-c184-49a6-bda4-f329a1c2234c"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 86, DateTimeKind.Utc).AddTicks(9134), new DateTime(2020, 4, 22, 14, 52, 39, 86, DateTimeKind.Utc).AddTicks(9133) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("61cce8e3-606a-4c38-a4e3-0fd01bd51382"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(7168), new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(7168) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("62428d43-58b1-42c7-b743-88f8271f4ea0"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 86, DateTimeKind.Utc).AddTicks(813), new DateTime(2020, 4, 22, 14, 52, 39, 86, DateTimeKind.Utc).AddTicks(809) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("64fb541b-c7c6-4a45-b66c-8ce10ed2edcc"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 88, DateTimeKind.Utc).AddTicks(1405), new DateTime(2020, 4, 22, 14, 52, 39, 88, DateTimeKind.Utc).AddTicks(1405) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("650788ab-d279-43c5-b0a6-2ec539077286"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(2090), new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(2090) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("655a45c2-6bac-409b-8365-797a76a48082"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(1598), new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(1598) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("6a438f58-7f38-4f67-abde-4d2bc3b71a6b"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(4370), new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(4370) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("6a859e88-2b9c-45c0-b516-48d34f9f7a1d"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 88, DateTimeKind.Utc).AddTicks(4071), new DateTime(2020, 4, 22, 14, 52, 39, 88, DateTimeKind.Utc).AddTicks(4071) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("6bc284e5-8a0f-4447-a9ce-8408867c2c33"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 86, DateTimeKind.Utc).AddTicks(7302), new DateTime(2020, 4, 22, 14, 52, 39, 86, DateTimeKind.Utc).AddTicks(7302) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("6c781014-069e-4ae4-8703-8f5f40e6a7c4"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 86, DateTimeKind.Utc).AddTicks(1480), new DateTime(2020, 4, 22, 14, 52, 39, 86, DateTimeKind.Utc).AddTicks(1480) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("6c86eca5-53f6-4527-9841-102426936a3c"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(5137), new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(5137) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("704fbf04-c50b-409e-a1ee-9f9020cba139"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 86, DateTimeKind.Utc).AddTicks(8915), new DateTime(2020, 4, 22, 14, 52, 39, 86, DateTimeKind.Utc).AddTicks(8915) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("7094bc11-813b-4aa8-80dd-3d87432638f2"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 88, DateTimeKind.Utc).AddTicks(2631), new DateTime(2020, 4, 22, 14, 52, 39, 88, DateTimeKind.Utc).AddTicks(2631) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("70a296e5-64c9-4c57-8315-68e2db2d2a19"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 86, DateTimeKind.Utc).AddTicks(7100), new DateTime(2020, 4, 22, 14, 52, 39, 86, DateTimeKind.Utc).AddTicks(7099) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("7787a3fd-3b6b-4916-9b36-2c628e627f07"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 86, DateTimeKind.Utc).AddTicks(9209), new DateTime(2020, 4, 22, 14, 52, 39, 86, DateTimeKind.Utc).AddTicks(9209) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("79393535-5677-4396-8411-fbc548f8d3aa"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 86, DateTimeKind.Utc).AddTicks(4083), new DateTime(2020, 4, 22, 14, 52, 39, 86, DateTimeKind.Utc).AddTicks(4083) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("79cfc791-c994-4fe6-8bec-6b8592c6770d"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 86, DateTimeKind.Utc).AddTicks(8987), new DateTime(2020, 4, 22, 14, 52, 39, 86, DateTimeKind.Utc).AddTicks(8987) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("7a685a86-d5f5-4689-bf79-a5a522f16eef"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(8265), new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(8265) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("7b0b8cea-0398-4360-845b-af3801520d3d"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(2709), new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(2708) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("7cb41b35-8582-410b-b6ed-be7c4d314aec"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(695), new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(695) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("7d9dadcf-7c09-47ff-8c15-de7fef89cab5"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(4847), new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(4847) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("7e326793-5f28-4c8a-aef2-a7aad130fcdc"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 88, DateTimeKind.Utc).AddTicks(3854), new DateTime(2020, 4, 22, 14, 52, 39, 88, DateTimeKind.Utc).AddTicks(3853) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("7eaae013-3794-459e-b44d-9445a7176fc3"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 88, DateTimeKind.Utc).AddTicks(4908), new DateTime(2020, 4, 22, 14, 52, 39, 88, DateTimeKind.Utc).AddTicks(4908) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("7f04c733-b3ad-4588-9444-7142872c33cf"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(9505), new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(9505) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("7f9de85a-ef0b-4870-afac-98751d6ed1b5"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(7759), new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(7759) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("80347a52-a17f-495a-a848-ed4e97b12bf3"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 86, DateTimeKind.Utc).AddTicks(1570), new DateTime(2020, 4, 22, 14, 52, 39, 86, DateTimeKind.Utc).AddTicks(1570) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("806dff4c-7b5a-4e13-8df2-106b9f4ccfca"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(1099), new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(1099) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("809dc6f6-d1ce-4ef1-bf01-3bfafe556c76"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 86, DateTimeKind.Utc).AddTicks(1202), new DateTime(2020, 4, 22, 14, 52, 39, 86, DateTimeKind.Utc).AddTicks(1201) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("80cfc6ab-5d75-4b61-87a1-686af93494cf"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 86, DateTimeKind.Utc).AddTicks(6796), new DateTime(2020, 4, 22, 14, 52, 39, 86, DateTimeKind.Utc).AddTicks(6796) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("80e5f7ec-0c3d-4c02-a6a5-60c32c667843"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(7977), new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(7977) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("831eb79a-3223-4098-b3e9-f7576c2d5ca3"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 88, DateTimeKind.Utc).AddTicks(225), new DateTime(2020, 4, 22, 14, 52, 39, 88, DateTimeKind.Utc).AddTicks(225) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("83829704-28ea-42a9-8e69-a9161201734f"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(9940), new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(9939) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("8423becb-d485-4dd5-9de4-b1f29f301b64"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 86, DateTimeKind.Utc).AddTicks(8470), new DateTime(2020, 4, 22, 14, 52, 39, 86, DateTimeKind.Utc).AddTicks(8470) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("86db84e8-60f1-489d-8ba9-e1bcbad0dba5"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 88, DateTimeKind.Utc).AddTicks(2342), new DateTime(2020, 4, 22, 14, 52, 39, 88, DateTimeKind.Utc).AddTicks(2341) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("86dd3243-429e-4800-ab41-492f5d0844d8"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 88, DateTimeKind.Utc).AddTicks(3152), new DateTime(2020, 4, 22, 14, 52, 39, 88, DateTimeKind.Utc).AddTicks(3152) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("890edd50-e797-4606-8bdb-dfc3ad6fa6bb"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 88, DateTimeKind.Utc).AddTicks(3078), new DateTime(2020, 4, 22, 14, 52, 39, 88, DateTimeKind.Utc).AddTicks(3078) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("896ad135-4bee-45f7-98ff-560228a09d4c"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 88, DateTimeKind.Utc).AddTicks(1185), new DateTime(2020, 4, 22, 14, 52, 39, 88, DateTimeKind.Utc).AddTicks(1184) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("8ad8ef36-72a4-4a84-bbe5-ae1086c917f6"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(5855), new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(5854) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("8ae22438-46b7-4e46-a85d-b6f63fa562d1"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(9102), new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(9101) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("8c254a88-ad6a-430a-b76e-4bed9574e839"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 88, DateTimeKind.Utc).AddTicks(4505), new DateTime(2020, 4, 22, 14, 52, 39, 88, DateTimeKind.Utc).AddTicks(4504) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("8db004ea-9235-44e1-9004-1064c88bb39d"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(52), new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(52) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("8f843487-2a14-46a8-8eeb-49a4b4240466"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(7310), new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(7310) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("8fa0678c-e8eb-4f8a-88e2-407ac6655fe8"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 88, DateTimeKind.Utc).AddTicks(778), new DateTime(2020, 4, 22, 14, 52, 39, 88, DateTimeKind.Utc).AddTicks(777) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("8fe68854-0883-4e15-8728-168db7133594"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 86, DateTimeKind.Utc).AddTicks(4204), new DateTime(2020, 4, 22, 14, 52, 39, 86, DateTimeKind.Utc).AddTicks(4204) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("90cc4fbb-1a9c-472f-8e4b-f9143ca383f3"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 88, DateTimeKind.Utc).AddTicks(2559), new DateTime(2020, 4, 22, 14, 52, 39, 88, DateTimeKind.Utc).AddTicks(2559) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("915b66cc-301c-47c4-bbb0-a097ba0e992f"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 88, DateTimeKind.Utc).AddTicks(3364), new DateTime(2020, 4, 22, 14, 52, 39, 88, DateTimeKind.Utc).AddTicks(3364) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("91f85c3d-0fef-4a81-80d9-6c3d52188335"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 88, DateTimeKind.Utc).AddTicks(2488), new DateTime(2020, 4, 22, 14, 52, 39, 88, DateTimeKind.Utc).AddTicks(2488) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("92bfdd07-3809-44ae-9782-357ff8c17cc5"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(8817), new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(8817) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("92d0fdf7-b8b0-4972-abe5-9c91ab9f1e20"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(767), new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(767) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("975d1fb3-14d7-46a7-90ad-114e465adb32"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 86, DateTimeKind.Utc).AddTicks(9909), new DateTime(2020, 4, 22, 14, 52, 39, 86, DateTimeKind.Utc).AddTicks(9908) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("97d76114-4e72-4ba6-8ea0-b5fa1076dd85"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(3867), new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(3867) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("9a21db7c-4a4a-48cd-adf4-d30776d717c2"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(1673), new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(1673) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("9ab72b3d-aa55-4efc-92ea-671ab56fe547"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(2855), new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(2855) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("9bf5491b-3d24-49bc-952c-068d8e0bd63a"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 86, DateTimeKind.Utc).AddTicks(7517), new DateTime(2020, 4, 22, 14, 52, 39, 86, DateTimeKind.Utc).AddTicks(7517) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("9c3266f7-ae5a-4b65-b876-f5efd6ca2744"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 86, DateTimeKind.Utc).AddTicks(4752), new DateTime(2020, 4, 22, 14, 52, 39, 86, DateTimeKind.Utc).AddTicks(4752) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("9c5bfd4a-b441-40a4-92bf-1dbd51cd0c38"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 86, DateTimeKind.Utc).AddTicks(1123), new DateTime(2020, 4, 22, 14, 52, 39, 86, DateTimeKind.Utc).AddTicks(1122) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("9ea3dc31-7aab-45b6-b3e4-d4b808f275de"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 86, DateTimeKind.Utc).AddTicks(8075), new DateTime(2020, 4, 22, 14, 52, 39, 86, DateTimeKind.Utc).AddTicks(8074) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("a1124e8b-1651-4e8e-8f38-00419204ba26"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(2303), new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(2303) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("a2ac4090-0348-4054-99e0-79582350b10d"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 88, DateTimeKind.Utc).AddTicks(2924), new DateTime(2020, 4, 22, 14, 52, 39, 88, DateTimeKind.Utc).AddTicks(2924) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("a2ce4d93-d99c-42ab-91b4-f46fc5b42cc9"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(8959), new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(8958) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("a2eac142-0e9c-4286-bced-cbd18dc3e8ff"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 88, DateTimeKind.Utc).AddTicks(3999), new DateTime(2020, 4, 22, 14, 52, 39, 88, DateTimeKind.Utc).AddTicks(3998) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("a2f7e5a5-b487-46b1-b566-762cd8622ccd"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(8410), new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(8410) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("a3e220f4-8815-47b1-80b5-328b23d64638"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(2232), new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(2232) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("a46ef95a-9cd6-4d13-aa1c-4b58df0582b7"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 88, DateTimeKind.Utc).AddTicks(1621), new DateTime(2020, 4, 22, 14, 52, 39, 88, DateTimeKind.Utc).AddTicks(1621) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("a4ae064a-33a7-4ea2-96c2-96b2d96e8dc5"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 88, DateTimeKind.Utc).AddTicks(2414), new DateTime(2020, 4, 22, 14, 52, 39, 88, DateTimeKind.Utc).AddTicks(2413) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("a62094fc-c620-4a14-a2e6-7e9fc90b248a"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(3793), new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(3793) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("a89f5d82-cdb6-415d-a0ac-06805dd23a07"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 88, DateTimeKind.Utc).AddTicks(921), new DateTime(2020, 4, 22, 14, 52, 39, 88, DateTimeKind.Utc).AddTicks(921) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("a8dca457-5ab8-4c98-b85c-4ad4445eb9bc"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 88, DateTimeKind.Utc).AddTicks(3437), new DateTime(2020, 4, 22, 14, 52, 39, 88, DateTimeKind.Utc).AddTicks(3436) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("aac36e76-7d76-48ae-b5b6-e9a50246fe43"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(553), new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(552) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("acee03c8-e413-4e58-9c58-d7fd4d4712b0"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(3722), new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(3722) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("ad32b25a-1a86-402c-b6c5-2351301ebdb3"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 88, DateTimeKind.Utc).AddTicks(12), new DateTime(2020, 4, 22, 14, 52, 39, 88, DateTimeKind.Utc).AddTicks(12) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("af72340e-f2d5-46d7-ab06-eb7ea88fd980"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 88, DateTimeKind.Utc).AddTicks(3781), new DateTime(2020, 4, 22, 14, 52, 39, 88, DateTimeKind.Utc).AddTicks(3780) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("b140296c-061b-46da-aa03-ddbccbecd0e5"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 86, DateTimeKind.Utc).AddTicks(6718), new DateTime(2020, 4, 22, 14, 52, 39, 86, DateTimeKind.Utc).AddTicks(6718) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("b163bcf6-f11c-4633-a9e5-1dd33f16361a"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 86, DateTimeKind.Utc).AddTicks(9615), new DateTime(2020, 4, 22, 14, 52, 39, 86, DateTimeKind.Utc).AddTicks(9615) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("b30ad8f5-fe8d-4871-b5af-9bb2079d629b"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 86, DateTimeKind.Utc).AddTicks(7878), new DateTime(2020, 4, 22, 14, 52, 39, 86, DateTimeKind.Utc).AddTicks(7877) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("b37e7b1e-4bab-4e48-bb1d-c58b7354addb"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(7529), new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(7529) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("b5770fa3-0442-495b-83a1-101edcfdc4db"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(7906), new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(7906) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("b5bdb2e7-5806-4e2e-ad35-f73b335d9802"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(5339), new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(5338) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("b67d2214-8360-4187-95ed-62c7de73a4e6"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(4922), new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(4922) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("b79cce99-fe7b-45aa-bbb5-968dbb25a759"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(6622), new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(6622) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("b80a6b0c-20b4-4523-bf90-9872181ffee3"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 88, DateTimeKind.Utc).AddTicks(343), new DateTime(2020, 4, 22, 14, 52, 39, 88, DateTimeKind.Utc).AddTicks(342) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("ba6bcff8-2632-4042-9238-47da4eeb278f"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 86, DateTimeKind.Utc).AddTicks(6950), new DateTime(2020, 4, 22, 14, 52, 39, 86, DateTimeKind.Utc).AddTicks(6949) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("bb1e04ce-3c66-4a6c-b449-356934a27b34"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(8196), new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(8196) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("bb3ad3c6-d820-4aac-9e90-6d77b16606bc"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(3006), new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(3006) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("bc6f1d4b-a71d-4a03-a57f-f5c52f90cb55"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(8053), new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(8053) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("bea4675d-4226-431d-b903-981a545a7d40"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(1866), new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(1866) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("bed32dd6-7fb6-47e9-9d4f-b73d2b22bc2e"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(1244), new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(1244) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("bf14da6d-ec15-4263-94f9-4896b4aebf85"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(4080), new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(4080) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("bff41a03-c3d3-4858-89fa-56c88b26d7c5"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(8672), new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(8671) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("c03516f2-1053-4e15-9213-b2c7f9c1c2a4"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(3527), new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(3527) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("c0b51e2b-1cf6-4f49-8d71-438e8e331c26"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(4777), new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(4777) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("c27db605-b81f-47e8-9a1b-9b8162da97ca"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(1022), new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(1022) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("c2a001e1-7368-42fc-82d2-4571d8413993"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 88, DateTimeKind.Utc).AddTicks(156), new DateTime(2020, 4, 22, 14, 52, 39, 88, DateTimeKind.Utc).AddTicks(155) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("c3541832-7aba-4125-b8b7-805262dbbed3"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 86, DateTimeKind.Utc).AddTicks(6870), new DateTime(2020, 4, 22, 14, 52, 39, 86, DateTimeKind.Utc).AddTicks(6870) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("c372dc96-f012-499e-a248-7b4056d6cf9c"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(6043), new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(6043) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("c4233b12-f07f-4601-a84b-bc5388d6ab90"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(3640), new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(3640) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("c62ac057-cdce-435d-9da0-ac9ed5a571cc"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 86, DateTimeKind.Utc).AddTicks(8212), new DateTime(2020, 4, 22, 14, 52, 39, 86, DateTimeKind.Utc).AddTicks(8211) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("c7335ffb-dbe2-47d9-9ee0-2959a2914b22"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(5488), new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(5488) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("c793d9cc-2df0-42dd-9fc3-215dfcc437d3"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 86, DateTimeKind.Utc).AddTicks(9062), new DateTime(2020, 4, 22, 14, 52, 39, 86, DateTimeKind.Utc).AddTicks(9061) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("c7f402d8-66bb-41f2-a4b9-a10535037d67"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(255), new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(255) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("c8162e28-b2e6-424b-b8a4-4a2b7a5ce689"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(4702), new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(4702) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("c850f373-b3d6-4433-8c3c-8336686389ce"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 88, DateTimeKind.Utc).AddTicks(2702), new DateTime(2020, 4, 22, 14, 52, 39, 88, DateTimeKind.Utc).AddTicks(2701) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("ca659864-42d4-4944-af8b-8cb7bc15e72a"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 86, DateTimeKind.Utc).AddTicks(8623), new DateTime(2020, 4, 22, 14, 52, 39, 86, DateTimeKind.Utc).AddTicks(8623) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("cb9e4fd0-0ebd-41d0-8d59-12b9f3c078dc"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 86, DateTimeKind.Utc).AddTicks(4826), new DateTime(2020, 4, 22, 14, 52, 39, 86, DateTimeKind.Utc).AddTicks(4826) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("cca5c121-a29a-4257-8456-c2d91778921e"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(6882), new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(6881) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("ce7bf492-c066-4f8a-9028-4ff873052ae9"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 88, DateTimeKind.Utc).AddTicks(4761), new DateTime(2020, 4, 22, 14, 52, 39, 88, DateTimeKind.Utc).AddTicks(4761) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("cefc54c5-a59c-4977-aff0-11c33309b8b0"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(8887), new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(8887) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("d37b446b-04be-452e-b9db-efb14e7de4b8"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(6405), new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(6405) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("d3dc90bb-cb90-4d4c-81c4-a914fb853f0d"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(327), new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(327) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("d51c4fb4-6348-4aaa-aa65-cb1ba62e8843"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(7099), new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(7099) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("d6a2916c-4198-46e3-b9a8-78422f14d95d"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(9029), new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(9029) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("d6ebbe9e-90eb-4a9c-9531-45551a153cda"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(9794), new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(9793) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("d752aaf6-30e6-4f6a-b510-183479272f79"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(7385), new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(7385) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("d75fc2a3-0d8c-404d-bd63-1f3741c3e2c7"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(7833), new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(7833) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("d7952e90-e80d-4d76-9ff7-cbd075427aea"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 86, DateTimeKind.Utc).AddTicks(6613), new DateTime(2020, 4, 22, 14, 52, 39, 86, DateTimeKind.Utc).AddTicks(6613) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("da9e8996-d49f-4ebf-b68f-2196f5ccbcb9"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(6190), new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(6189) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("dc24e5d2-1500-4020-a191-c7a639495d4b"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(2928), new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(2927) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("de02761e-2a0c-461d-9cf5-d01863ce7f7b"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 88, DateTimeKind.Utc).AddTicks(992), new DateTime(2020, 4, 22, 14, 52, 39, 88, DateTimeKind.Utc).AddTicks(992) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("de9dcd72-a831-4412-939d-ee7ce7e49f91"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 86, DateTimeKind.Utc).AddTicks(7594), new DateTime(2020, 4, 22, 14, 52, 39, 86, DateTimeKind.Utc).AddTicks(7593) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("e2f5dc79-67fc-4bf5-a209-c749c33a612d"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(478), new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(478) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("e33dbf61-1b00-45e6-8271-ed6da0fbc8ea"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 88, DateTimeKind.Utc).AddTicks(83), new DateTime(2020, 4, 22, 14, 52, 39, 88, DateTimeKind.Utc).AddTicks(82) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("e3a93d41-4e63-4039-acbd-d74a5ca42382"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 86, DateTimeKind.Utc).AddTicks(9834), new DateTime(2020, 4, 22, 14, 52, 39, 86, DateTimeKind.Utc).AddTicks(9834) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("e4dab6bc-3a0e-4b83-8612-75fec85ab72c"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(4994), new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(4993) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("e5c90c4c-e728-4e0d-a3ec-3fabb346c58e"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 86, DateTimeKind.Utc).AddTicks(9983), new DateTime(2020, 4, 22, 14, 52, 39, 86, DateTimeKind.Utc).AddTicks(9982) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("e6167271-e650-4da0-a49a-ad0f2c8e10d0"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 86, DateTimeKind.Utc).AddTicks(9546), new DateTime(2020, 4, 22, 14, 52, 39, 86, DateTimeKind.Utc).AddTicks(9546) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("e6c1e3a8-6433-46bc-be31-11081c5455d1"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 86, DateTimeKind.Utc).AddTicks(9758), new DateTime(2020, 4, 22, 14, 52, 39, 86, DateTimeKind.Utc).AddTicks(9757) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("e8750ddd-d85e-46c0-b182-976f180b96ca"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 86, DateTimeKind.Utc).AddTicks(4297), new DateTime(2020, 4, 22, 14, 52, 39, 86, DateTimeKind.Utc).AddTicks(4296) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("e97eea91-0450-4a9d-a9e9-3a5696009f92"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(5210), new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(5209) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("ea785967-d335-4190-8d04-4ed4f39192d9"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 86, DateTimeKind.Utc).AddTicks(7373), new DateTime(2020, 4, 22, 14, 52, 39, 86, DateTimeKind.Utc).AddTicks(7373) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("ec432743-7546-443e-97d3-91e6747081ec"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(2523), new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(2522) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("ec629336-db76-4ba9-9382-2602e3ab537c"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(6479), new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(6479) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("ed039891-2c07-413b-94c1-47083baba1ff"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 88, DateTimeKind.Utc).AddTicks(848), new DateTime(2020, 4, 22, 14, 52, 39, 88, DateTimeKind.Utc).AddTicks(848) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("ed86de41-ae26-488b-b568-91eb96b1ae46"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(6336), new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(6335) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("eee885bf-5731-4c9e-9357-5c185decab94"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(3937), new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(3937) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("ef610d09-2fcc-4a7b-9d6e-4635af253ea1"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(5711), new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(5711) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("efd68efc-40c6-474f-b1df-a00add7d9478"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(3150), new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(3150) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("f00b0b06-0204-4cda-84fe-707ca8f6ed6e"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(8743), new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(8743) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("f1cf93c1-3e27-48af-acb3-d8f0a28d385d"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(9724), new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(9723) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("f20967d9-a1a9-418f-bca6-d3a80f9930f9"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(9247), new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(9247) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("f4857c7f-06fc-44c2-b5b1-53e93b96cc1d"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 88, DateTimeKind.Utc).AddTicks(634), new DateTime(2020, 4, 22, 14, 52, 39, 88, DateTimeKind.Utc).AddTicks(634) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("f4f32977-b379-4a16-8a1b-ea9ab145d7af"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 86, DateTimeKind.Utc).AddTicks(9687), new DateTime(2020, 4, 22, 14, 52, 39, 86, DateTimeKind.Utc).AddTicks(9687) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("f51bfd85-0306-47bc-828b-2855ed83970b"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 86, DateTimeKind.Utc).AddTicks(9328), new DateTime(2020, 4, 22, 14, 52, 39, 86, DateTimeKind.Utc).AddTicks(9327) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("f5229c5c-0973-4b13-9791-fe1cc0b7bf91"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(7456), new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(7456) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("f544239f-afc9-45c9-a32d-5b0775a32331"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(9651), new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(9651) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("f5cf9979-6f32-4f77-b254-7f3dca3d10ca"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 88, DateTimeKind.Utc).AddTicks(1549), new DateTime(2020, 4, 22, 14, 52, 39, 88, DateTimeKind.Utc).AddTicks(1549) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("f60f98b5-1603-4af0-a91d-eb7b5ebc1262"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(2781), new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(2781) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("f61936ad-8598-4e40-a271-8e1b58219f70"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 88, DateTimeKind.Utc).AddTicks(3294), new DateTime(2020, 4, 22, 14, 52, 39, 88, DateTimeKind.Utc).AddTicks(3294) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("f6224a58-889c-4863-82b0-5a55c17b9eea"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 88, DateTimeKind.Utc).AddTicks(414), new DateTime(2020, 4, 22, 14, 52, 39, 88, DateTimeKind.Utc).AddTicks(413) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("f6b996cf-9d8e-4e3d-b5b8-6a6a0cb3ff28"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 88, DateTimeKind.Utc).AddTicks(704), new DateTime(2020, 4, 22, 14, 52, 39, 88, DateTimeKind.Utc).AddTicks(704) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("f7e50e30-2eb9-46a4-b5c2-cf93930b6e6d"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(7240), new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(7239) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("f87d66e6-f9b6-41e0-9d5c-00e709c32994"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 88, DateTimeKind.Utc).AddTicks(4144), new DateTime(2020, 4, 22, 14, 52, 39, 88, DateTimeKind.Utc).AddTicks(4143) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("f8d52de3-8485-4f1d-a6b9-6f74f5991332"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 86, DateTimeKind.Utc).AddTicks(7024), new DateTime(2020, 4, 22, 14, 52, 39, 86, DateTimeKind.Utc).AddTicks(7023) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("f9a1c8ad-5e4a-4d9c-8f38-d84f36266d4e"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(1385), new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(1385) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("fb6ed9fb-f8c1-4b94-b1d1-df8c2601935d"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 88, DateTimeKind.Utc).AddTicks(4837), new DateTime(2020, 4, 22, 14, 52, 39, 88, DateTimeKind.Utc).AddTicks(4837) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("fd20c039-6df1-4f11-8a69-6b5af70ac180"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 88, DateTimeKind.Utc).AddTicks(4614), new DateTime(2020, 4, 22, 14, 52, 39, 88, DateTimeKind.Utc).AddTicks(4613) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("fdf65fb5-483f-46ed-b7f7-7b51dadce79e"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(9172), new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(9172) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("fe36c089-d363-4191-bc3d-28b68d5a8b53"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 86, DateTimeKind.Utc).AddTicks(7447), new DateTime(2020, 4, 22, 14, 52, 39, 86, DateTimeKind.Utc).AddTicks(7446) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("fe409e05-85b0-4bc9-af33-7ed1d9f2f62c"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(953), new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(952) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("fed44b42-36c8-4dd8-8a3e-e351f6d54ccc"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(1171), new DateTime(2020, 4, 22, 14, 52, 39, 87, DateTimeKind.Utc).AddTicks(1171) });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Districts",
                schema: "Localization",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Author = table.Column<string>(nullable: true),
                    Changed = table.Column<DateTime>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    Location = table.Column<string>(nullable: true),
                    ModifiedBy = table.Column<string>(nullable: true),
                    Name = table.Column<string>(maxLength: 450, nullable: false),
                    StateOrProvinceId = table.Column<Guid>(nullable: false),
                    TenantId = table.Column<Guid>(nullable: true),
                    Type = table.Column<string>(maxLength: 450, nullable: true),
                    Version = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Districts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Districts_StateOrProvinces_StateOrProvinceId",
                        column: x => x.StateOrProvinceId,
                        principalSchema: "Localization",
                        principalTable: "StateOrProvinces",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("01217f05-7835-4201-95d2-88e8b8cba693"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 245, DateTimeKind.Utc).AddTicks(553), new DateTime(2020, 3, 22, 14, 5, 31, 245, DateTimeKind.Utc).AddTicks(553) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("0157a8ae-ea59-47ac-895c-56666e06a0e1"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 246, DateTimeKind.Utc).AddTicks(4621), new DateTime(2020, 3, 22, 14, 5, 31, 246, DateTimeKind.Utc).AddTicks(4621) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("02322cf7-2ef8-4e1e-9b78-3848349c1296"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 245, DateTimeKind.Utc).AddTicks(9993), new DateTime(2020, 3, 22, 14, 5, 31, 245, DateTimeKind.Utc).AddTicks(9993) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("030c4a96-8e05-4883-af1d-718449051743"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 244, DateTimeKind.Utc).AddTicks(8690), new DateTime(2020, 3, 22, 14, 5, 31, 244, DateTimeKind.Utc).AddTicks(8690) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("035dde65-a582-43af-b47c-e6413fd4e881"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 246, DateTimeKind.Utc).AddTicks(7422), new DateTime(2020, 3, 22, 14, 5, 31, 246, DateTimeKind.Utc).AddTicks(7421) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("03beb5af-8897-4df8-a1e5-ea49c91e2955"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 246, DateTimeKind.Utc).AddTicks(4708), new DateTime(2020, 3, 22, 14, 5, 31, 246, DateTimeKind.Utc).AddTicks(4708) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("05e84190-9cd1-4959-9322-25c2b1a62220"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 244, DateTimeKind.Utc).AddTicks(9754), new DateTime(2020, 3, 22, 14, 5, 31, 244, DateTimeKind.Utc).AddTicks(9753) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("08aed749-f7a3-41b5-b88f-a7773f5c3155"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 245, DateTimeKind.Utc).AddTicks(7869), new DateTime(2020, 3, 22, 14, 5, 31, 245, DateTimeKind.Utc).AddTicks(7868) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("0bb8c15c-a4f6-4dce-8451-b1e5815b5186"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 246, DateTimeKind.Utc).AddTicks(6837), new DateTime(2020, 3, 22, 14, 5, 31, 246, DateTimeKind.Utc).AddTicks(6836) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("0cba73ae-9042-4b2a-bab6-6c3185764234"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 244, DateTimeKind.Utc).AddTicks(4138), new DateTime(2020, 3, 22, 14, 5, 31, 244, DateTimeKind.Utc).AddTicks(4138) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("0d622cb6-c520-4aea-acc2-c91dd5b62c9c"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 246, DateTimeKind.Utc).AddTicks(7713), new DateTime(2020, 3, 22, 14, 5, 31, 246, DateTimeKind.Utc).AddTicks(7713) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("10055208-fe88-46e9-80b6-e6a56a09b168"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 246, DateTimeKind.Utc).AddTicks(4874), new DateTime(2020, 3, 22, 14, 5, 31, 246, DateTimeKind.Utc).AddTicks(4873) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("1066262e-d59c-4a0f-a687-cf5c1637ec29"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 236, DateTimeKind.Utc).AddTicks(2228), new DateTime(2020, 3, 22, 14, 5, 31, 236, DateTimeKind.Utc).AddTicks(1561) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("11750caa-2337-4a7b-9e6e-fcbdc4b1a4c6"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 245, DateTimeKind.Utc).AddTicks(3013), new DateTime(2020, 3, 22, 14, 5, 31, 245, DateTimeKind.Utc).AddTicks(3012) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("12f0754e-1431-48bc-a8ab-619aaff44c58"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 246, DateTimeKind.Utc).AddTicks(4205), new DateTime(2020, 3, 22, 14, 5, 31, 246, DateTimeKind.Utc).AddTicks(4205) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("1398338a-0d71-452e-895d-15f4da7ef403"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 244, DateTimeKind.Utc).AddTicks(9671), new DateTime(2020, 3, 22, 14, 5, 31, 244, DateTimeKind.Utc).AddTicks(9670) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("1582d1ff-8978-423a-9f93-1cb8c6193804"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 246, DateTimeKind.Utc).AddTicks(7338), new DateTime(2020, 3, 22, 14, 5, 31, 246, DateTimeKind.Utc).AddTicks(7338) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("15d53299-341d-40d1-8444-032030d215e9"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 245, DateTimeKind.Utc).AddTicks(7530), new DateTime(2020, 3, 22, 14, 5, 31, 245, DateTimeKind.Utc).AddTicks(7529) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("15eff380-20c7-4558-895c-54ba17871876"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 244, DateTimeKind.Utc).AddTicks(3440), new DateTime(2020, 3, 22, 14, 5, 31, 244, DateTimeKind.Utc).AddTicks(3439) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("1696bb6c-d6a7-4632-ba0b-72a4f414c8a9"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 246, DateTimeKind.Utc).AddTicks(5683), new DateTime(2020, 3, 22, 14, 5, 31, 246, DateTimeKind.Utc).AddTicks(5683) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("1726b8e1-71ac-4ded-826f-690f36e3d604"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 245, DateTimeKind.Utc).AddTicks(7135), new DateTime(2020, 3, 22, 14, 5, 31, 245, DateTimeKind.Utc).AddTicks(7134) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("17fbf326-d021-43d4-9b4d-05f797308af2"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 246, DateTimeKind.Utc).AddTicks(607), new DateTime(2020, 3, 22, 14, 5, 31, 246, DateTimeKind.Utc).AddTicks(607) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("182008f1-d0e7-488c-beb7-df44a0c3186a"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 244, DateTimeKind.Utc).AddTicks(9029), new DateTime(2020, 3, 22, 14, 5, 31, 244, DateTimeKind.Utc).AddTicks(9028) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("18c02c8f-b1ad-4f35-ac0b-9d48a0669d60"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 245, DateTimeKind.Utc).AddTicks(3978), new DateTime(2020, 3, 22, 14, 5, 31, 245, DateTimeKind.Utc).AddTicks(3978) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("18c3b29d-a45f-4ad6-852f-0f15801d8570"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 245, DateTimeKind.Utc).AddTicks(8771), new DateTime(2020, 3, 22, 14, 5, 31, 245, DateTimeKind.Utc).AddTicks(8771) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("18d18037-4977-42b1-a73d-0b5cbb32995c"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 246, DateTimeKind.Utc).AddTicks(4532), new DateTime(2020, 3, 22, 14, 5, 31, 246, DateTimeKind.Utc).AddTicks(4531) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("18e435e8-bf1b-424d-adc8-5cb949bb23b5"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 245, DateTimeKind.Utc).AddTicks(1871), new DateTime(2020, 3, 22, 14, 5, 31, 245, DateTimeKind.Utc).AddTicks(1870) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("1947c227-efb2-4b8a-b625-cbef979d9c30"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 245, DateTimeKind.Utc).AddTicks(4242), new DateTime(2020, 3, 22, 14, 5, 31, 245, DateTimeKind.Utc).AddTicks(4241) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("1995b10e-fb35-484f-abe5-e322cb92bb04"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 246, DateTimeKind.Utc).AddTicks(1591), new DateTime(2020, 3, 22, 14, 5, 31, 246, DateTimeKind.Utc).AddTicks(1591) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("19ec185a-a771-4b56-8687-4289e1dcdbc1"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 245, DateTimeKind.Utc).AddTicks(7697), new DateTime(2020, 3, 22, 14, 5, 31, 245, DateTimeKind.Utc).AddTicks(7696) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("1c4653b4-3d39-46f9-aa93-7e499ba603ba"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 245, DateTimeKind.Utc).AddTicks(6125), new DateTime(2020, 3, 22, 14, 5, 31, 245, DateTimeKind.Utc).AddTicks(6125) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("1c5f6d4f-a519-4095-a2d1-4575e32d1cf0"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 245, DateTimeKind.Utc).AddTicks(3095), new DateTime(2020, 3, 22, 14, 5, 31, 245, DateTimeKind.Utc).AddTicks(3095) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("1cda93a0-d3ef-4c53-ac50-b7a7b73abf51"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 245, DateTimeKind.Utc).AddTicks(1352), new DateTime(2020, 3, 22, 14, 5, 31, 245, DateTimeKind.Utc).AddTicks(1352) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("1d6271ae-3ad6-4836-81b4-3be1405f03f4"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 246, DateTimeKind.Utc).AddTicks(7257), new DateTime(2020, 3, 22, 14, 5, 31, 246, DateTimeKind.Utc).AddTicks(7257) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("1f841bb5-ca53-4034-bed4-9c381c8c30bb"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 245, DateTimeKind.Utc).AddTicks(7050), new DateTime(2020, 3, 22, 14, 5, 31, 245, DateTimeKind.Utc).AddTicks(7049) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("1f8eb873-4f12-4aff-a30a-ff0b576f6966"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 245, DateTimeKind.Utc).AddTicks(5449), new DateTime(2020, 3, 22, 14, 5, 31, 245, DateTimeKind.Utc).AddTicks(5449) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("20125248-13af-47f9-bd32-58cf07ee2a21"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 245, DateTimeKind.Utc).AddTicks(8430), new DateTime(2020, 3, 22, 14, 5, 31, 245, DateTimeKind.Utc).AddTicks(8430) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("20b0b0de-5c3b-4bfe-b01d-93ea9f638738"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 244, DateTimeKind.Utc).AddTicks(3804), new DateTime(2020, 3, 22, 14, 5, 31, 244, DateTimeKind.Utc).AddTicks(3804) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("20bfcca0-1df5-4f79-b33f-55178e6755f6"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 245, DateTimeKind.Utc).AddTicks(4897), new DateTime(2020, 3, 22, 14, 5, 31, 245, DateTimeKind.Utc).AddTicks(4897) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("2107778f-bac0-47b2-b556-ad528865d593"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 245, DateTimeKind.Utc).AddTicks(3812), new DateTime(2020, 3, 22, 14, 5, 31, 245, DateTimeKind.Utc).AddTicks(3812) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("21d20ec8-60e0-4961-acd7-191c23e45914"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 245, DateTimeKind.Utc).AddTicks(6877), new DateTime(2020, 3, 22, 14, 5, 31, 245, DateTimeKind.Utc).AddTicks(6876) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("232fed5a-c4fa-4382-ab39-4743ba2c00ef"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 246, DateTimeKind.Utc).AddTicks(4292), new DateTime(2020, 3, 22, 14, 5, 31, 246, DateTimeKind.Utc).AddTicks(4292) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("23d1efb0-d504-4b17-b8aa-782b0473a8cf"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 246, DateTimeKind.Utc).AddTicks(1504), new DateTime(2020, 3, 22, 14, 5, 31, 246, DateTimeKind.Utc).AddTicks(1503) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("261df7b9-2bbd-422f-95ba-ffb319ab8822"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 246, DateTimeKind.Utc).AddTicks(2813), new DateTime(2020, 3, 22, 14, 5, 31, 246, DateTimeKind.Utc).AddTicks(2813) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("264300cb-38c2-4eff-ad3b-4f8ea66654c2"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 244, DateTimeKind.Utc).AddTicks(6709), new DateTime(2020, 3, 22, 14, 5, 31, 244, DateTimeKind.Utc).AddTicks(6708) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("2670f213-d73c-4948-bc78-01982365c270"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 246, DateTimeKind.Utc).AddTicks(3954), new DateTime(2020, 3, 22, 14, 5, 31, 246, DateTimeKind.Utc).AddTicks(3953) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("268747f5-fead-4398-b046-43ae2ca51012"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 246, DateTimeKind.Utc).AddTicks(4445), new DateTime(2020, 3, 22, 14, 5, 31, 246, DateTimeKind.Utc).AddTicks(4445) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("26daf397-0a9d-44af-ae74-9940d456c3e5"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 244, DateTimeKind.Utc).AddTicks(3530), new DateTime(2020, 3, 22, 14, 5, 31, 244, DateTimeKind.Utc).AddTicks(3530) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("27120855-8e09-42f3-8b5c-d8323ac4ef8f"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 245, DateTimeKind.Utc).AddTicks(5535), new DateTime(2020, 3, 22, 14, 5, 31, 245, DateTimeKind.Utc).AddTicks(5535) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("27d521a1-6712-4299-ad77-943a6398042a"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 244, DateTimeKind.Utc).AddTicks(4441), new DateTime(2020, 3, 22, 14, 5, 31, 244, DateTimeKind.Utc).AddTicks(4441) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("286ca324-9ed4-4b8c-b4fc-a0f0ea80e581"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 245, DateTimeKind.Utc).AddTicks(8686), new DateTime(2020, 3, 22, 14, 5, 31, 245, DateTimeKind.Utc).AddTicks(8685) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("291a766f-6bb7-4065-8cec-1ca3ace2d8e5"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 245, DateTimeKind.Utc).AddTicks(2511), new DateTime(2020, 3, 22, 14, 5, 31, 245, DateTimeKind.Utc).AddTicks(2511) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("2b1181ea-7f0a-47ce-902c-9be62633eac4"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 246, DateTimeKind.Utc).AddTicks(240), new DateTime(2020, 3, 22, 14, 5, 31, 246, DateTimeKind.Utc).AddTicks(240) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("30fecce2-317f-4d97-aeec-f0a0dad1de94"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 244, DateTimeKind.Utc).AddTicks(8524), new DateTime(2020, 3, 22, 14, 5, 31, 244, DateTimeKind.Utc).AddTicks(8524) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("3177d852-5deb-489a-a3ee-e9e5a9c402a0"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 245, DateTimeKind.Utc).AddTicks(5619), new DateTime(2020, 3, 22, 14, 5, 31, 245, DateTimeKind.Utc).AddTicks(5618) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("31d589d2-5935-47a9-89af-0d268c1eb414"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 245, DateTimeKind.Utc).AddTicks(2038), new DateTime(2020, 3, 22, 14, 5, 31, 245, DateTimeKind.Utc).AddTicks(2038) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("338dcf64-83d0-42ee-bd94-144144d9ffba"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 246, DateTimeKind.Utc).AddTicks(5931), new DateTime(2020, 3, 22, 14, 5, 31, 246, DateTimeKind.Utc).AddTicks(5931) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("33cd468e-0e16-4c79-b209-e522b52f89b6"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 246, DateTimeKind.Utc).AddTicks(2900), new DateTime(2020, 3, 22, 14, 5, 31, 246, DateTimeKind.Utc).AddTicks(2899) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("3472258b-0b58-4685-af92-8dbe932ab87d"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 244, DateTimeKind.Utc).AddTicks(8863), new DateTime(2020, 3, 22, 14, 5, 31, 244, DateTimeKind.Utc).AddTicks(8862) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("38054345-534d-4490-b17e-dfb7a2588deb"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 246, DateTimeKind.Utc).AddTicks(6589), new DateTime(2020, 3, 22, 14, 5, 31, 246, DateTimeKind.Utc).AddTicks(6589) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("38b1356a-9e47-480c-8d61-cdd7b9b38259"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 246, DateTimeKind.Utc).AddTicks(5512), new DateTime(2020, 3, 22, 14, 5, 31, 246, DateTimeKind.Utc).AddTicks(5512) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("3bd22ab9-6136-4106-94ad-60133641a150"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 244, DateTimeKind.Utc).AddTicks(4354), new DateTime(2020, 3, 22, 14, 5, 31, 244, DateTimeKind.Utc).AddTicks(4354) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("4049e1b0-7629-43d4-93f1-d9516907cc26"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 246, DateTimeKind.Utc).AddTicks(3789), new DateTime(2020, 3, 22, 14, 5, 31, 246, DateTimeKind.Utc).AddTicks(3789) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("4565ffeb-1d26-49fc-82b3-156dd5bbed22"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 245, DateTimeKind.Utc).AddTicks(2593), new DateTime(2020, 3, 22, 14, 5, 31, 245, DateTimeKind.Utc).AddTicks(2593) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("45b8db8c-8136-4e73-a923-1d19a32102ea"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 245, DateTimeKind.Utc).AddTicks(5148), new DateTime(2020, 3, 22, 14, 5, 31, 245, DateTimeKind.Utc).AddTicks(5148) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("45dbf8fa-2776-4722-a199-0fba104f9e37"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 244, DateTimeKind.Utc).AddTicks(7588), new DateTime(2020, 3, 22, 14, 5, 31, 244, DateTimeKind.Utc).AddTicks(7587) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("4650b837-4a79-4e9c-89ec-620b6e8ec075"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 246, DateTimeKind.Utc).AddTicks(7172), new DateTime(2020, 3, 22, 14, 5, 31, 246, DateTimeKind.Utc).AddTicks(7172) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("48c21470-c276-4ffc-bc29-7a2ce0126d78"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 246, DateTimeKind.Utc).AddTicks(4793), new DateTime(2020, 3, 22, 14, 5, 31, 246, DateTimeKind.Utc).AddTicks(4792) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("48c3a204-2cbf-4752-8ac5-2ce167be4772"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 245, DateTimeKind.Utc).AddTicks(804), new DateTime(2020, 3, 22, 14, 5, 31, 245, DateTimeKind.Utc).AddTicks(804) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("49520892-e40e-4294-af7d-c79d4edee393"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 246, DateTimeKind.Utc).AddTicks(525), new DateTime(2020, 3, 22, 14, 5, 31, 246, DateTimeKind.Utc).AddTicks(525) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("4b985e4c-fb54-4a66-890d-075d3104d032"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 246, DateTimeKind.Utc).AddTicks(6503), new DateTime(2020, 3, 22, 14, 5, 31, 246, DateTimeKind.Utc).AddTicks(6503) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("4ba13eda-8479-408f-a5b9-696d474d04b1"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 245, DateTimeKind.Utc).AddTicks(2760), new DateTime(2020, 3, 22, 14, 5, 31, 245, DateTimeKind.Utc).AddTicks(2760) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("4c1f43c7-47f4-403b-8543-96cf07ce10de"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 245, DateTimeKind.Utc).AddTicks(4094), new DateTime(2020, 3, 22, 14, 5, 31, 245, DateTimeKind.Utc).AddTicks(4093) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("4d5a36ed-2da3-48aa-88c7-24329188bcdf"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 246, DateTimeKind.Utc).AddTicks(1759), new DateTime(2020, 3, 22, 14, 5, 31, 246, DateTimeKind.Utc).AddTicks(1758) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("506333bb-aea4-40e0-910b-6191972e5371"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 245, DateTimeKind.Utc).AddTicks(5233), new DateTime(2020, 3, 22, 14, 5, 31, 245, DateTimeKind.Utc).AddTicks(5233) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("518fe0de-b9c6-4690-bef5-3f74d95940a6"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 245, DateTimeKind.Utc).AddTicks(8518), new DateTime(2020, 3, 22, 14, 5, 31, 245, DateTimeKind.Utc).AddTicks(8518) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("520af57c-bbde-4475-8253-dc6ce18323c9"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 245, DateTimeKind.Utc).AddTicks(5064), new DateTime(2020, 3, 22, 14, 5, 31, 245, DateTimeKind.Utc).AddTicks(5063) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("5226e4cf-40ad-42e0-996c-83daf464d3bf"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 244, DateTimeKind.Utc).AddTicks(4265), new DateTime(2020, 3, 22, 14, 5, 31, 244, DateTimeKind.Utc).AddTicks(4265) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("52488d7f-eb89-4c34-9099-e9e25fd54164"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 244, DateTimeKind.Utc).AddTicks(8948), new DateTime(2020, 3, 22, 14, 5, 31, 244, DateTimeKind.Utc).AddTicks(8947) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("5297f23d-a50f-4f75-91c2-8719d4c58cc2"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 245, DateTimeKind.Utc).AddTicks(3308), new DateTime(2020, 3, 22, 14, 5, 31, 245, DateTimeKind.Utc).AddTicks(3308) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("5331378c-2a7e-496d-b64f-fc19f1cc79b9"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 244, DateTimeKind.Utc).AddTicks(3089), new DateTime(2020, 3, 22, 14, 5, 31, 244, DateTimeKind.Utc).AddTicks(3088) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("53430b22-1351-4a76-a56f-d562c3784df5"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 246, DateTimeKind.Utc).AddTicks(3704), new DateTime(2020, 3, 22, 14, 5, 31, 246, DateTimeKind.Utc).AddTicks(3703) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("5472c425-37ea-4ede-836c-8eb4ce20d343"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 246, DateTimeKind.Utc).AddTicks(2095), new DateTime(2020, 3, 22, 14, 5, 31, 246, DateTimeKind.Utc).AddTicks(2095) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("54ece6d5-f31c-40b7-acc0-728bb78fa2b6"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 245, DateTimeKind.Utc).AddTicks(9488), new DateTime(2020, 3, 22, 14, 5, 31, 245, DateTimeKind.Utc).AddTicks(9487) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("55285133-f5c0-4853-82b9-4d656cc4f9af"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 245, DateTimeKind.Utc).AddTicks(7302), new DateTime(2020, 3, 22, 14, 5, 31, 245, DateTimeKind.Utc).AddTicks(7301) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("554fda2f-d0e4-4c2c-b522-359806fc2c21"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 246, DateTimeKind.Utc).AddTicks(6354), new DateTime(2020, 3, 22, 14, 5, 31, 246, DateTimeKind.Utc).AddTicks(6354) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("56139794-ac33-46e2-8cc5-88dc4cab2350"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 244, DateTimeKind.Utc).AddTicks(8097), new DateTime(2020, 3, 22, 14, 5, 31, 244, DateTimeKind.Utc).AddTicks(8096) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("59709a12-0413-431c-9ac7-89795004d386"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 245, DateTimeKind.Utc).AddTicks(1058), new DateTime(2020, 3, 22, 14, 5, 31, 245, DateTimeKind.Utc).AddTicks(1058) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("598800ff-029f-4aa2-9c28-985b17cc77c9"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 246, DateTimeKind.Utc).AddTicks(3535), new DateTime(2020, 3, 22, 14, 5, 31, 246, DateTimeKind.Utc).AddTicks(3534) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("5c9a109e-e4c6-434b-bc67-2ed0c6b0713c"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 245, DateTimeKind.Utc).AddTicks(2123), new DateTime(2020, 3, 22, 14, 5, 31, 245, DateTimeKind.Utc).AddTicks(2122) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("5fca9a3c-8b1c-4167-91ce-800f6c1391b6"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 245, DateTimeKind.Utc).AddTicks(8205), new DateTime(2020, 3, 22, 14, 5, 31, 245, DateTimeKind.Utc).AddTicks(8205) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("61c24c18-c184-49a6-bda4-f329a1c2234c"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 244, DateTimeKind.Utc).AddTicks(9411), new DateTime(2020, 3, 22, 14, 5, 31, 244, DateTimeKind.Utc).AddTicks(9410) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("61cce8e3-606a-4c38-a4e3-0fd01bd51382"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 245, DateTimeKind.Utc).AddTicks(8934), new DateTime(2020, 3, 22, 14, 5, 31, 245, DateTimeKind.Utc).AddTicks(8934) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("62428d43-58b1-42c7-b743-88f8271f4ea0"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 244, DateTimeKind.Utc).AddTicks(2849), new DateTime(2020, 3, 22, 14, 5, 31, 244, DateTimeKind.Utc).AddTicks(2844) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("64fb541b-c7c6-4a45-b66c-8ce10ed2edcc"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 246, DateTimeKind.Utc).AddTicks(3868), new DateTime(2020, 3, 22, 14, 5, 31, 246, DateTimeKind.Utc).AddTicks(3868) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("650788ab-d279-43c5-b0a6-2ec539077286"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 245, DateTimeKind.Utc).AddTicks(2679), new DateTime(2020, 3, 22, 14, 5, 31, 245, DateTimeKind.Utc).AddTicks(2678) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("655a45c2-6bac-409b-8365-797a76a48082"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 245, DateTimeKind.Utc).AddTicks(2251), new DateTime(2020, 3, 22, 14, 5, 31, 245, DateTimeKind.Utc).AddTicks(2251) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("6a438f58-7f38-4f67-abde-4d2bc3b71a6b"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 245, DateTimeKind.Utc).AddTicks(5364), new DateTime(2020, 3, 22, 14, 5, 31, 245, DateTimeKind.Utc).AddTicks(5364) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("6a859e88-2b9c-45c0-b516-48d34f9f7a1d"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 246, DateTimeKind.Utc).AddTicks(7004), new DateTime(2020, 3, 22, 14, 5, 31, 246, DateTimeKind.Utc).AddTicks(7004) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("6bc284e5-8a0f-4447-a9ce-8408867c2c33"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 244, DateTimeKind.Utc).AddTicks(7670), new DateTime(2020, 3, 22, 14, 5, 31, 244, DateTimeKind.Utc).AddTicks(7669) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("6c781014-069e-4ae4-8703-8f5f40e6a7c4"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 244, DateTimeKind.Utc).AddTicks(3616), new DateTime(2020, 3, 22, 14, 5, 31, 244, DateTimeKind.Utc).AddTicks(3616) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("6c86eca5-53f6-4527-9841-102426936a3c"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 245, DateTimeKind.Utc).AddTicks(6295), new DateTime(2020, 3, 22, 14, 5, 31, 245, DateTimeKind.Utc).AddTicks(6294) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("704fbf04-c50b-409e-a1ee-9f9020cba139"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 244, DateTimeKind.Utc).AddTicks(9117), new DateTime(2020, 3, 22, 14, 5, 31, 244, DateTimeKind.Utc).AddTicks(9116) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("7094bc11-813b-4aa8-80dd-3d87432638f2"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 246, DateTimeKind.Utc).AddTicks(5299), new DateTime(2020, 3, 22, 14, 5, 31, 246, DateTimeKind.Utc).AddTicks(5299) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("70a296e5-64c9-4c57-8315-68e2db2d2a19"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 244, DateTimeKind.Utc).AddTicks(7500), new DateTime(2020, 3, 22, 14, 5, 31, 244, DateTimeKind.Utc).AddTicks(7500) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("7787a3fd-3b6b-4916-9b36-2c628e627f07"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 244, DateTimeKind.Utc).AddTicks(9502), new DateTime(2020, 3, 22, 14, 5, 31, 244, DateTimeKind.Utc).AddTicks(9502) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("79393535-5677-4396-8411-fbc548f8d3aa"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 244, DateTimeKind.Utc).AddTicks(3891), new DateTime(2020, 3, 22, 14, 5, 31, 244, DateTimeKind.Utc).AddTicks(3890) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("79cfc791-c994-4fe6-8bec-6b8592c6770d"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 244, DateTimeKind.Utc).AddTicks(9199), new DateTime(2020, 3, 22, 14, 5, 31, 244, DateTimeKind.Utc).AddTicks(9199) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("7a685a86-d5f5-4689-bf79-a5a522f16eef"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 246, DateTimeKind.Utc).AddTicks(156), new DateTime(2020, 3, 22, 14, 5, 31, 246, DateTimeKind.Utc).AddTicks(156) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("7b0b8cea-0398-4360-845b-af3801520d3d"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 245, DateTimeKind.Utc).AddTicks(3393), new DateTime(2020, 3, 22, 14, 5, 31, 245, DateTimeKind.Utc).AddTicks(3393) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("7cb41b35-8582-410b-b6ed-be7c4d314aec"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 245, DateTimeKind.Utc).AddTicks(1143), new DateTime(2020, 3, 22, 14, 5, 31, 245, DateTimeKind.Utc).AddTicks(1142) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("7d9dadcf-7c09-47ff-8c15-de7fef89cab5"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 245, DateTimeKind.Utc).AddTicks(5871), new DateTime(2020, 3, 22, 14, 5, 31, 245, DateTimeKind.Utc).AddTicks(5870) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("7e326793-5f28-4c8a-aef2-a7aad130fcdc"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 246, DateTimeKind.Utc).AddTicks(6755), new DateTime(2020, 3, 22, 14, 5, 31, 246, DateTimeKind.Utc).AddTicks(6754) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("7eaae013-3794-459e-b44d-9445a7176fc3"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 246, DateTimeKind.Utc).AddTicks(7968), new DateTime(2020, 3, 22, 14, 5, 31, 246, DateTimeKind.Utc).AddTicks(7967) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("7f04c733-b3ad-4588-9444-7142872c33cf"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 246, DateTimeKind.Utc).AddTicks(1674), new DateTime(2020, 3, 22, 14, 5, 31, 246, DateTimeKind.Utc).AddTicks(1673) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("7f9de85a-ef0b-4870-afac-98751d6ed1b5"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 245, DateTimeKind.Utc).AddTicks(9569), new DateTime(2020, 3, 22, 14, 5, 31, 245, DateTimeKind.Utc).AddTicks(9569) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("80347a52-a17f-495a-a848-ed4e97b12bf3"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 244, DateTimeKind.Utc).AddTicks(3716), new DateTime(2020, 3, 22, 14, 5, 31, 244, DateTimeKind.Utc).AddTicks(3716) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("806dff4c-7b5a-4e13-8df2-106b9f4ccfca"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 245, DateTimeKind.Utc).AddTicks(1619), new DateTime(2020, 3, 22, 14, 5, 31, 245, DateTimeKind.Utc).AddTicks(1619) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("809dc6f6-d1ce-4ef1-bf01-3bfafe556c76"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 244, DateTimeKind.Utc).AddTicks(3347), new DateTime(2020, 3, 22, 14, 5, 31, 244, DateTimeKind.Utc).AddTicks(3347) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("80cfc6ab-5d75-4b61-87a1-686af93494cf"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 244, DateTimeKind.Utc).AddTicks(7009), new DateTime(2020, 3, 22, 14, 5, 31, 244, DateTimeKind.Utc).AddTicks(7008) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("80e5f7ec-0c3d-4c02-a6a5-60c32c667843"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 245, DateTimeKind.Utc).AddTicks(9824), new DateTime(2020, 3, 22, 14, 5, 31, 245, DateTimeKind.Utc).AddTicks(9823) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("831eb79a-3223-4098-b3e9-f7576c2d5ca3"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 246, DateTimeKind.Utc).AddTicks(2560), new DateTime(2020, 3, 22, 14, 5, 31, 246, DateTimeKind.Utc).AddTicks(2560) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("83829704-28ea-42a9-8e69-a9161201734f"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 246, DateTimeKind.Utc).AddTicks(2179), new DateTime(2020, 3, 22, 14, 5, 31, 246, DateTimeKind.Utc).AddTicks(2178) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("8423becb-d485-4dd5-9de4-b1f29f301b64"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 244, DateTimeKind.Utc).AddTicks(8609), new DateTime(2020, 3, 22, 14, 5, 31, 244, DateTimeKind.Utc).AddTicks(8609) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("86db84e8-60f1-489d-8ba9-e1bcbad0dba5"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 246, DateTimeKind.Utc).AddTicks(4961), new DateTime(2020, 3, 22, 14, 5, 31, 246, DateTimeKind.Utc).AddTicks(4960) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("86dd3243-429e-4800-ab41-492f5d0844d8"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 246, DateTimeKind.Utc).AddTicks(5851), new DateTime(2020, 3, 22, 14, 5, 31, 246, DateTimeKind.Utc).AddTicks(5850) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("890edd50-e797-4606-8bdb-dfc3ad6fa6bb"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 246, DateTimeKind.Utc).AddTicks(5766), new DateTime(2020, 3, 22, 14, 5, 31, 246, DateTimeKind.Utc).AddTicks(5766) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("896ad135-4bee-45f7-98ff-560228a09d4c"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 246, DateTimeKind.Utc).AddTicks(3622), new DateTime(2020, 3, 22, 14, 5, 31, 246, DateTimeKind.Utc).AddTicks(3622) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("8ad8ef36-72a4-4a84-bbe5-ae1086c917f6"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 245, DateTimeKind.Utc).AddTicks(7443), new DateTime(2020, 3, 22, 14, 5, 31, 245, DateTimeKind.Utc).AddTicks(7443) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("8ae22438-46b7-4e46-a85d-b6f63fa562d1"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 246, DateTimeKind.Utc).AddTicks(1204), new DateTime(2020, 3, 22, 14, 5, 31, 246, DateTimeKind.Utc).AddTicks(1203) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("8c254a88-ad6a-430a-b76e-4bed9574e839"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 246, DateTimeKind.Utc).AddTicks(7547), new DateTime(2020, 3, 22, 14, 5, 31, 246, DateTimeKind.Utc).AddTicks(7547) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("8db004ea-9235-44e1-9004-1064c88bb39d"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 245, DateTimeKind.Utc).AddTicks(472), new DateTime(2020, 3, 22, 14, 5, 31, 245, DateTimeKind.Utc).AddTicks(472) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("8f843487-2a14-46a8-8eeb-49a4b4240466"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 245, DateTimeKind.Utc).AddTicks(9100), new DateTime(2020, 3, 22, 14, 5, 31, 245, DateTimeKind.Utc).AddTicks(9099) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("8fa0678c-e8eb-4f8a-88e2-407ac6655fe8"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 246, DateTimeKind.Utc).AddTicks(3145), new DateTime(2020, 3, 22, 14, 5, 31, 246, DateTimeKind.Utc).AddTicks(3145) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("8fe68854-0883-4e15-8728-168db7133594"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 244, DateTimeKind.Utc).AddTicks(3972), new DateTime(2020, 3, 22, 14, 5, 31, 244, DateTimeKind.Utc).AddTicks(3972) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("90cc4fbb-1a9c-472f-8e4b-f9143ca383f3"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 246, DateTimeKind.Utc).AddTicks(5215), new DateTime(2020, 3, 22, 14, 5, 31, 246, DateTimeKind.Utc).AddTicks(5215) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("915b66cc-301c-47c4-bbb0-a097ba0e992f"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 246, DateTimeKind.Utc).AddTicks(6098), new DateTime(2020, 3, 22, 14, 5, 31, 246, DateTimeKind.Utc).AddTicks(6098) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("91f85c3d-0fef-4a81-80d9-6c3d52188335"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 246, DateTimeKind.Utc).AddTicks(5133), new DateTime(2020, 3, 22, 14, 5, 31, 246, DateTimeKind.Utc).AddTicks(5133) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("92bfdd07-3809-44ae-9782-357ff8c17cc5"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 246, DateTimeKind.Utc).AddTicks(869), new DateTime(2020, 3, 22, 14, 5, 31, 246, DateTimeKind.Utc).AddTicks(869) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("92d0fdf7-b8b0-4972-abe5-9c91ab9f1e20"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 245, DateTimeKind.Utc).AddTicks(1266), new DateTime(2020, 3, 22, 14, 5, 31, 245, DateTimeKind.Utc).AddTicks(1265) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("975d1fb3-14d7-46a7-90ad-114e465adb32"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 245, DateTimeKind.Utc).AddTicks(258), new DateTime(2020, 3, 22, 14, 5, 31, 245, DateTimeKind.Utc).AddTicks(258) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("97d76114-4e72-4ba6-8ea0-b5fa1076dd85"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 245, DateTimeKind.Utc).AddTicks(4729), new DateTime(2020, 3, 22, 14, 5, 31, 245, DateTimeKind.Utc).AddTicks(4729) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("9a21db7c-4a4a-48cd-adf4-d30776d717c2"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 245, DateTimeKind.Utc).AddTicks(2341), new DateTime(2020, 3, 22, 14, 5, 31, 245, DateTimeKind.Utc).AddTicks(2341) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("9ab72b3d-aa55-4efc-92ea-671ab56fe547"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 245, DateTimeKind.Utc).AddTicks(3559), new DateTime(2020, 3, 22, 14, 5, 31, 245, DateTimeKind.Utc).AddTicks(3559) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("9bf5491b-3d24-49bc-952c-068d8e0bd63a"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 244, DateTimeKind.Utc).AddTicks(7920), new DateTime(2020, 3, 22, 14, 5, 31, 244, DateTimeKind.Utc).AddTicks(7920) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("9c3266f7-ae5a-4b65-b876-f5efd6ca2744"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 244, DateTimeKind.Utc).AddTicks(4532), new DateTime(2020, 3, 22, 14, 5, 31, 244, DateTimeKind.Utc).AddTicks(4531) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("9c5bfd4a-b441-40a4-92bf-1dbd51cd0c38"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 244, DateTimeKind.Utc).AddTicks(3188), new DateTime(2020, 3, 22, 14, 5, 31, 244, DateTimeKind.Utc).AddTicks(3188) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("9ea3dc31-7aab-45b6-b3e4-d4b808f275de"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 244, DateTimeKind.Utc).AddTicks(8267), new DateTime(2020, 3, 22, 14, 5, 31, 244, DateTimeKind.Utc).AddTicks(8267) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("a1124e8b-1651-4e8e-8f38-00419204ba26"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 245, DateTimeKind.Utc).AddTicks(2925), new DateTime(2020, 3, 22, 14, 5, 31, 245, DateTimeKind.Utc).AddTicks(2924) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("a2ac4090-0348-4054-99e0-79582350b10d"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 246, DateTimeKind.Utc).AddTicks(5595), new DateTime(2020, 3, 22, 14, 5, 31, 246, DateTimeKind.Utc).AddTicks(5594) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("a2ce4d93-d99c-42ab-91b4-f46fc5b42cc9"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 246, DateTimeKind.Utc).AddTicks(1035), new DateTime(2020, 3, 22, 14, 5, 31, 246, DateTimeKind.Utc).AddTicks(1035) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("a2eac142-0e9c-4286-bced-cbd18dc3e8ff"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 246, DateTimeKind.Utc).AddTicks(6921), new DateTime(2020, 3, 22, 14, 5, 31, 246, DateTimeKind.Utc).AddTicks(6921) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("a2f7e5a5-b487-46b1-b566-762cd8622ccd"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 246, DateTimeKind.Utc).AddTicks(322), new DateTime(2020, 3, 22, 14, 5, 31, 246, DateTimeKind.Utc).AddTicks(322) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("a3e220f4-8815-47b1-80b5-328b23d64638"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 245, DateTimeKind.Utc).AddTicks(2844), new DateTime(2020, 3, 22, 14, 5, 31, 245, DateTimeKind.Utc).AddTicks(2843) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("a46ef95a-9cd6-4d13-aa1c-4b58df0582b7"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 246, DateTimeKind.Utc).AddTicks(4119), new DateTime(2020, 3, 22, 14, 5, 31, 246, DateTimeKind.Utc).AddTicks(4119) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("a4ae064a-33a7-4ea2-96c2-96b2d96e8dc5"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 246, DateTimeKind.Utc).AddTicks(5043), new DateTime(2020, 3, 22, 14, 5, 31, 246, DateTimeKind.Utc).AddTicks(5042) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("a62094fc-c620-4a14-a2e6-7e9fc90b248a"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 245, DateTimeKind.Utc).AddTicks(4643), new DateTime(2020, 3, 22, 14, 5, 31, 245, DateTimeKind.Utc).AddTicks(4642) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("a89f5d82-cdb6-415d-a0ac-06805dd23a07"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 246, DateTimeKind.Utc).AddTicks(3312), new DateTime(2020, 3, 22, 14, 5, 31, 246, DateTimeKind.Utc).AddTicks(3312) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("a8dca457-5ab8-4c98-b85c-4ad4445eb9bc"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 246, DateTimeKind.Utc).AddTicks(6255), new DateTime(2020, 3, 22, 14, 5, 31, 246, DateTimeKind.Utc).AddTicks(6254) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("aac36e76-7d76-48ae-b5b6-e9a50246fe43"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 245, DateTimeKind.Utc).AddTicks(977), new DateTime(2020, 3, 22, 14, 5, 31, 245, DateTimeKind.Utc).AddTicks(977) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("acee03c8-e413-4e58-9c58-d7fd4d4712b0"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 245, DateTimeKind.Utc).AddTicks(4556), new DateTime(2020, 3, 22, 14, 5, 31, 245, DateTimeKind.Utc).AddTicks(4556) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("ad32b25a-1a86-402c-b6c5-2351301ebdb3"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 246, DateTimeKind.Utc).AddTicks(2261), new DateTime(2020, 3, 22, 14, 5, 31, 246, DateTimeKind.Utc).AddTicks(2260) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("af72340e-f2d5-46d7-ab06-eb7ea88fd980"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 246, DateTimeKind.Utc).AddTicks(6674), new DateTime(2020, 3, 22, 14, 5, 31, 246, DateTimeKind.Utc).AddTicks(6674) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("b140296c-061b-46da-aa03-ddbccbecd0e5"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 244, DateTimeKind.Utc).AddTicks(6926), new DateTime(2020, 3, 22, 14, 5, 31, 244, DateTimeKind.Utc).AddTicks(6925) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("b163bcf6-f11c-4633-a9e5-1dd33f16361a"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 244, DateTimeKind.Utc).AddTicks(9921), new DateTime(2020, 3, 22, 14, 5, 31, 244, DateTimeKind.Utc).AddTicks(9921) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("b30ad8f5-fe8d-4871-b5af-9bb2079d629b"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 244, DateTimeKind.Utc).AddTicks(8185), new DateTime(2020, 3, 22, 14, 5, 31, 244, DateTimeKind.Utc).AddTicks(8184) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("b37e7b1e-4bab-4e48-bb1d-c58b7354addb"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 245, DateTimeKind.Utc).AddTicks(9395), new DateTime(2020, 3, 22, 14, 5, 31, 245, DateTimeKind.Utc).AddTicks(9395) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("b5770fa3-0442-495b-83a1-101edcfdc4db"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 245, DateTimeKind.Utc).AddTicks(9737), new DateTime(2020, 3, 22, 14, 5, 31, 245, DateTimeKind.Utc).AddTicks(9737) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("b5bdb2e7-5806-4e2e-ad35-f73b335d9802"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 245, DateTimeKind.Utc).AddTicks(6788), new DateTime(2020, 3, 22, 14, 5, 31, 245, DateTimeKind.Utc).AddTicks(6788) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("b67d2214-8360-4187-95ed-62c7de73a4e6"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 245, DateTimeKind.Utc).AddTicks(5958), new DateTime(2020, 3, 22, 14, 5, 31, 245, DateTimeKind.Utc).AddTicks(5957) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("b79cce99-fe7b-45aa-bbb5-968dbb25a759"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 245, DateTimeKind.Utc).AddTicks(8287), new DateTime(2020, 3, 22, 14, 5, 31, 245, DateTimeKind.Utc).AddTicks(8286) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("b80a6b0c-20b4-4523-bf90-9872181ffee3"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 246, DateTimeKind.Utc).AddTicks(2643), new DateTime(2020, 3, 22, 14, 5, 31, 246, DateTimeKind.Utc).AddTicks(2642) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("ba6bcff8-2632-4042-9238-47da4eeb278f"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 244, DateTimeKind.Utc).AddTicks(7188), new DateTime(2020, 3, 22, 14, 5, 31, 244, DateTimeKind.Utc).AddTicks(7188) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("bb1e04ce-3c66-4a6c-b449-356934a27b34"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 246, DateTimeKind.Utc).AddTicks(74), new DateTime(2020, 3, 22, 14, 5, 31, 246, DateTimeKind.Utc).AddTicks(74) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("bb3ad3c6-d820-4aac-9e90-6d77b16606bc"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 245, DateTimeKind.Utc).AddTicks(3727), new DateTime(2020, 3, 22, 14, 5, 31, 245, DateTimeKind.Utc).AddTicks(3727) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("bc6f1d4b-a71d-4a03-a57f-f5c52f90cb55"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 245, DateTimeKind.Utc).AddTicks(9909), new DateTime(2020, 3, 22, 14, 5, 31, 245, DateTimeKind.Utc).AddTicks(9908) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("bea4675d-4226-431d-b903-981a545a7d40"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 245, DateTimeKind.Utc).AddTicks(2425), new DateTime(2020, 3, 22, 14, 5, 31, 245, DateTimeKind.Utc).AddTicks(2424) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("bed32dd6-7fb6-47e9-9d4f-b73d2b22bc2e"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 245, DateTimeKind.Utc).AddTicks(1786), new DateTime(2020, 3, 22, 14, 5, 31, 245, DateTimeKind.Utc).AddTicks(1786) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("bf14da6d-ec15-4263-94f9-4896b4aebf85"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 245, DateTimeKind.Utc).AddTicks(4982), new DateTime(2020, 3, 22, 14, 5, 31, 245, DateTimeKind.Utc).AddTicks(4981) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("bff41a03-c3d3-4858-89fa-56c88b26d7c5"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 246, DateTimeKind.Utc).AddTicks(700), new DateTime(2020, 3, 22, 14, 5, 31, 246, DateTimeKind.Utc).AddTicks(700) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("c03516f2-1053-4e15-9213-b2c7f9c1c2a4"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 245, DateTimeKind.Utc).AddTicks(4380), new DateTime(2020, 3, 22, 14, 5, 31, 245, DateTimeKind.Utc).AddTicks(4380) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("c0b51e2b-1cf6-4f49-8d71-438e8e331c26"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 245, DateTimeKind.Utc).AddTicks(5787), new DateTime(2020, 3, 22, 14, 5, 31, 245, DateTimeKind.Utc).AddTicks(5787) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("c27db605-b81f-47e8-9a1b-9b8162da97ca"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 245, DateTimeKind.Utc).AddTicks(1530), new DateTime(2020, 3, 22, 14, 5, 31, 245, DateTimeKind.Utc).AddTicks(1530) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("c2a001e1-7368-42fc-82d2-4571d8413993"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 246, DateTimeKind.Utc).AddTicks(2476), new DateTime(2020, 3, 22, 14, 5, 31, 246, DateTimeKind.Utc).AddTicks(2475) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("c3541832-7aba-4125-b8b7-805262dbbed3"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 244, DateTimeKind.Utc).AddTicks(7098), new DateTime(2020, 3, 22, 14, 5, 31, 244, DateTimeKind.Utc).AddTicks(7098) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("c372dc96-f012-499e-a248-7b4056d6cf9c"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 245, DateTimeKind.Utc).AddTicks(7614), new DateTime(2020, 3, 22, 14, 5, 31, 245, DateTimeKind.Utc).AddTicks(7614) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("c4233b12-f07f-4601-a84b-bc5388d6ab90"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 245, DateTimeKind.Utc).AddTicks(4469), new DateTime(2020, 3, 22, 14, 5, 31, 245, DateTimeKind.Utc).AddTicks(4469) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("c62ac057-cdce-435d-9da0-ac9ed5a571cc"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 244, DateTimeKind.Utc).AddTicks(8435), new DateTime(2020, 3, 22, 14, 5, 31, 244, DateTimeKind.Utc).AddTicks(8435) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("c7335ffb-dbe2-47d9-9ee0-2959a2914b22"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 245, DateTimeKind.Utc).AddTicks(6959), new DateTime(2020, 3, 22, 14, 5, 31, 245, DateTimeKind.Utc).AddTicks(6958) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("c793d9cc-2df0-42dd-9fc3-215dfcc437d3"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 244, DateTimeKind.Utc).AddTicks(9283), new DateTime(2020, 3, 22, 14, 5, 31, 244, DateTimeKind.Utc).AddTicks(9283) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("c7f402d8-66bb-41f2-a4b9-a10535037d67"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 245, DateTimeKind.Utc).AddTicks(636), new DateTime(2020, 3, 22, 14, 5, 31, 245, DateTimeKind.Utc).AddTicks(636) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("c8162e28-b2e6-424b-b8a4-4a2b7a5ce689"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 245, DateTimeKind.Utc).AddTicks(5702), new DateTime(2020, 3, 22, 14, 5, 31, 245, DateTimeKind.Utc).AddTicks(5702) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("c850f373-b3d6-4433-8c3c-8336686389ce"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 246, DateTimeKind.Utc).AddTicks(5421), new DateTime(2020, 3, 22, 14, 5, 31, 246, DateTimeKind.Utc).AddTicks(5421) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("ca659864-42d4-4944-af8b-8cb7bc15e72a"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 244, DateTimeKind.Utc).AddTicks(8781), new DateTime(2020, 3, 22, 14, 5, 31, 244, DateTimeKind.Utc).AddTicks(8780) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("cb9e4fd0-0ebd-41d0-8d59-12b9f3c078dc"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 244, DateTimeKind.Utc).AddTicks(4614), new DateTime(2020, 3, 22, 14, 5, 31, 244, DateTimeKind.Utc).AddTicks(4614) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("cca5c121-a29a-4257-8456-c2d91778921e"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 245, DateTimeKind.Utc).AddTicks(8605), new DateTime(2020, 3, 22, 14, 5, 31, 245, DateTimeKind.Utc).AddTicks(8604) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("ce7bf492-c066-4f8a-9028-4ff873052ae9"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 246, DateTimeKind.Utc).AddTicks(7799), new DateTime(2020, 3, 22, 14, 5, 31, 246, DateTimeKind.Utc).AddTicks(7798) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("cefc54c5-a59c-4977-aff0-11c33309b8b0"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 246, DateTimeKind.Utc).AddTicks(950), new DateTime(2020, 3, 22, 14, 5, 31, 246, DateTimeKind.Utc).AddTicks(950) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("d37b446b-04be-452e-b9db-efb14e7de4b8"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 245, DateTimeKind.Utc).AddTicks(8036), new DateTime(2020, 3, 22, 14, 5, 31, 245, DateTimeKind.Utc).AddTicks(8036) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("d3dc90bb-cb90-4d4c-81c4-a914fb853f0d"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 245, DateTimeKind.Utc).AddTicks(718), new DateTime(2020, 3, 22, 14, 5, 31, 245, DateTimeKind.Utc).AddTicks(717) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("d51c4fb4-6348-4aaa-aa65-cb1ba62e8843"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 245, DateTimeKind.Utc).AddTicks(8851), new DateTime(2020, 3, 22, 14, 5, 31, 245, DateTimeKind.Utc).AddTicks(8851) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("d6a2916c-4198-46e3-b9a8-78422f14d95d"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 246, DateTimeKind.Utc).AddTicks(1118), new DateTime(2020, 3, 22, 14, 5, 31, 246, DateTimeKind.Utc).AddTicks(1118) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("d6ebbe9e-90eb-4a9c-9531-45551a153cda"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 246, DateTimeKind.Utc).AddTicks(2008), new DateTime(2020, 3, 22, 14, 5, 31, 246, DateTimeKind.Utc).AddTicks(2008) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("d752aaf6-30e6-4f6a-b510-183479272f79"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 245, DateTimeKind.Utc).AddTicks(9185), new DateTime(2020, 3, 22, 14, 5, 31, 245, DateTimeKind.Utc).AddTicks(9185) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("d75fc2a3-0d8c-404d-bd63-1f3741c3e2c7"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 245, DateTimeKind.Utc).AddTicks(9656), new DateTime(2020, 3, 22, 14, 5, 31, 245, DateTimeKind.Utc).AddTicks(9655) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("d7952e90-e80d-4d76-9ff7-cbd075427aea"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 244, DateTimeKind.Utc).AddTicks(6835), new DateTime(2020, 3, 22, 14, 5, 31, 244, DateTimeKind.Utc).AddTicks(6834) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("da9e8996-d49f-4ebf-b68f-2196f5ccbcb9"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 245, DateTimeKind.Utc).AddTicks(7785), new DateTime(2020, 3, 22, 14, 5, 31, 245, DateTimeKind.Utc).AddTicks(7785) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("dc24e5d2-1500-4020-a191-c7a639495d4b"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 245, DateTimeKind.Utc).AddTicks(3641), new DateTime(2020, 3, 22, 14, 5, 31, 245, DateTimeKind.Utc).AddTicks(3640) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("de02761e-2a0c-461d-9cf5-d01863ce7f7b"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 246, DateTimeKind.Utc).AddTicks(3445), new DateTime(2020, 3, 22, 14, 5, 31, 246, DateTimeKind.Utc).AddTicks(3445) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("de9dcd72-a831-4412-939d-ee7ce7e49f91"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 244, DateTimeKind.Utc).AddTicks(8011), new DateTime(2020, 3, 22, 14, 5, 31, 244, DateTimeKind.Utc).AddTicks(8010) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("e2f5dc79-67fc-4bf5-a209-c749c33a612d"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 245, DateTimeKind.Utc).AddTicks(889), new DateTime(2020, 3, 22, 14, 5, 31, 245, DateTimeKind.Utc).AddTicks(888) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("e33dbf61-1b00-45e6-8271-ed6da0fbc8ea"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 246, DateTimeKind.Utc).AddTicks(2348), new DateTime(2020, 3, 22, 14, 5, 31, 246, DateTimeKind.Utc).AddTicks(2348) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("e3a93d41-4e63-4039-acbd-d74a5ca42382"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 245, DateTimeKind.Utc).AddTicks(172), new DateTime(2020, 3, 22, 14, 5, 31, 245, DateTimeKind.Utc).AddTicks(172) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("e4dab6bc-3a0e-4b83-8612-75fec85ab72c"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 245, DateTimeKind.Utc).AddTicks(6042), new DateTime(2020, 3, 22, 14, 5, 31, 245, DateTimeKind.Utc).AddTicks(6042) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("e5c90c4c-e728-4e0d-a3ec-3fabb346c58e"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 245, DateTimeKind.Utc).AddTicks(383), new DateTime(2020, 3, 22, 14, 5, 31, 245, DateTimeKind.Utc).AddTicks(382) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("e6167271-e650-4da0-a49a-ad0f2c8e10d0"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 244, DateTimeKind.Utc).AddTicks(9839), new DateTime(2020, 3, 22, 14, 5, 31, 244, DateTimeKind.Utc).AddTicks(9839) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("e6c1e3a8-6433-46bc-be31-11081c5455d1"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 245, DateTimeKind.Utc).AddTicks(86), new DateTime(2020, 3, 22, 14, 5, 31, 245, DateTimeKind.Utc).AddTicks(86) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("e8750ddd-d85e-46c0-b182-976f180b96ca"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 244, DateTimeKind.Utc).AddTicks(4054), new DateTime(2020, 3, 22, 14, 5, 31, 244, DateTimeKind.Utc).AddTicks(4054) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("e97eea91-0450-4a9d-a9e9-3a5696009f92"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 245, DateTimeKind.Utc).AddTicks(6683), new DateTime(2020, 3, 22, 14, 5, 31, 245, DateTimeKind.Utc).AddTicks(6683) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("ea785967-d335-4190-8d04-4ed4f39192d9"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 244, DateTimeKind.Utc).AddTicks(7754), new DateTime(2020, 3, 22, 14, 5, 31, 244, DateTimeKind.Utc).AddTicks(7754) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("ec432743-7546-443e-97d3-91e6747081ec"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 245, DateTimeKind.Utc).AddTicks(3224), new DateTime(2020, 3, 22, 14, 5, 31, 245, DateTimeKind.Utc).AddTicks(3224) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("ec629336-db76-4ba9-9382-2602e3ab537c"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 245, DateTimeKind.Utc).AddTicks(8119), new DateTime(2020, 3, 22, 14, 5, 31, 245, DateTimeKind.Utc).AddTicks(8118) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("ed039891-2c07-413b-94c1-47083baba1ff"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 246, DateTimeKind.Utc).AddTicks(3230), new DateTime(2020, 3, 22, 14, 5, 31, 246, DateTimeKind.Utc).AddTicks(3230) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("ed86de41-ae26-488b-b568-91eb96b1ae46"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 245, DateTimeKind.Utc).AddTicks(7954), new DateTime(2020, 3, 22, 14, 5, 31, 245, DateTimeKind.Utc).AddTicks(7954) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("eee885bf-5731-4c9e-9357-5c185decab94"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 245, DateTimeKind.Utc).AddTicks(4814), new DateTime(2020, 3, 22, 14, 5, 31, 245, DateTimeKind.Utc).AddTicks(4814) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("ef610d09-2fcc-4a7b-9d6e-4635af253ea1"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 245, DateTimeKind.Utc).AddTicks(7220), new DateTime(2020, 3, 22, 14, 5, 31, 245, DateTimeKind.Utc).AddTicks(7219) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("efd68efc-40c6-474f-b1df-a00add7d9478"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 245, DateTimeKind.Utc).AddTicks(3895), new DateTime(2020, 3, 22, 14, 5, 31, 245, DateTimeKind.Utc).AddTicks(3894) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("f00b0b06-0204-4cda-84fe-707ca8f6ed6e"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 246, DateTimeKind.Utc).AddTicks(783), new DateTime(2020, 3, 22, 14, 5, 31, 246, DateTimeKind.Utc).AddTicks(783) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("f1cf93c1-3e27-48af-acb3-d8f0a28d385d"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 246, DateTimeKind.Utc).AddTicks(1926), new DateTime(2020, 3, 22, 14, 5, 31, 246, DateTimeKind.Utc).AddTicks(1925) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("f20967d9-a1a9-418f-bca6-d3a80f9930f9"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 246, DateTimeKind.Utc).AddTicks(1374), new DateTime(2020, 3, 22, 14, 5, 31, 246, DateTimeKind.Utc).AddTicks(1374) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("f4857c7f-06fc-44c2-b5b1-53e93b96cc1d"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 246, DateTimeKind.Utc).AddTicks(2982), new DateTime(2020, 3, 22, 14, 5, 31, 246, DateTimeKind.Utc).AddTicks(2982) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("f4f32977-b379-4a16-8a1b-ea9ab145d7af"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 245, DateTimeKind.Utc).AddTicks(3), new DateTime(2020, 3, 22, 14, 5, 31, 245, DateTimeKind.Utc).AddTicks(3) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("f51bfd85-0306-47bc-828b-2855ed83970b"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 244, DateTimeKind.Utc).AddTicks(9584), new DateTime(2020, 3, 22, 14, 5, 31, 244, DateTimeKind.Utc).AddTicks(9584) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("f5229c5c-0973-4b13-9791-fe1cc0b7bf91"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 245, DateTimeKind.Utc).AddTicks(9271), new DateTime(2020, 3, 22, 14, 5, 31, 245, DateTimeKind.Utc).AddTicks(9270) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("f544239f-afc9-45c9-a32d-5b0775a32331"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 246, DateTimeKind.Utc).AddTicks(1840), new DateTime(2020, 3, 22, 14, 5, 31, 246, DateTimeKind.Utc).AddTicks(1840) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("f5cf9979-6f32-4f77-b254-7f3dca3d10ca"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 246, DateTimeKind.Utc).AddTicks(4035), new DateTime(2020, 3, 22, 14, 5, 31, 246, DateTimeKind.Utc).AddTicks(4035) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("f60f98b5-1603-4af0-a91d-eb7b5ebc1262"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 245, DateTimeKind.Utc).AddTicks(3475), new DateTime(2020, 3, 22, 14, 5, 31, 245, DateTimeKind.Utc).AddTicks(3475) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("f61936ad-8598-4e40-a271-8e1b58219f70"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 246, DateTimeKind.Utc).AddTicks(6015), new DateTime(2020, 3, 22, 14, 5, 31, 246, DateTimeKind.Utc).AddTicks(6014) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("f6224a58-889c-4863-82b0-5a55c17b9eea"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 246, DateTimeKind.Utc).AddTicks(2727), new DateTime(2020, 3, 22, 14, 5, 31, 246, DateTimeKind.Utc).AddTicks(2726) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("f6b996cf-9d8e-4e3d-b5b8-6a6a0cb3ff28"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 246, DateTimeKind.Utc).AddTicks(3065), new DateTime(2020, 3, 22, 14, 5, 31, 246, DateTimeKind.Utc).AddTicks(3065) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("f7e50e30-2eb9-46a4-b5c2-cf93930b6e6d"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 245, DateTimeKind.Utc).AddTicks(9015), new DateTime(2020, 3, 22, 14, 5, 31, 245, DateTimeKind.Utc).AddTicks(9015) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("f87d66e6-f9b6-41e0-9d5c-00e709c32994"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 246, DateTimeKind.Utc).AddTicks(7088), new DateTime(2020, 3, 22, 14, 5, 31, 246, DateTimeKind.Utc).AddTicks(7087) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("f8d52de3-8485-4f1d-a6b9-6f74f5991332"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 244, DateTimeKind.Utc).AddTicks(7407), new DateTime(2020, 3, 22, 14, 5, 31, 244, DateTimeKind.Utc).AddTicks(7407) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("f9a1c8ad-5e4a-4d9c-8f38-d84f36266d4e"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 245, DateTimeKind.Utc).AddTicks(1955), new DateTime(2020, 3, 22, 14, 5, 31, 245, DateTimeKind.Utc).AddTicks(1954) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("fb6ed9fb-f8c1-4b94-b1d1-df8c2601935d"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 246, DateTimeKind.Utc).AddTicks(7884), new DateTime(2020, 3, 22, 14, 5, 31, 246, DateTimeKind.Utc).AddTicks(7883) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("fd20c039-6df1-4f11-8a69-6b5af70ac180"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 246, DateTimeKind.Utc).AddTicks(7632), new DateTime(2020, 3, 22, 14, 5, 31, 246, DateTimeKind.Utc).AddTicks(7632) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("fdf65fb5-483f-46ed-b7f7-7b51dadce79e"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 246, DateTimeKind.Utc).AddTicks(1287), new DateTime(2020, 3, 22, 14, 5, 31, 246, DateTimeKind.Utc).AddTicks(1286) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("fe36c089-d363-4191-bc3d-28b68d5a8b53"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 244, DateTimeKind.Utc).AddTicks(7838), new DateTime(2020, 3, 22, 14, 5, 31, 244, DateTimeKind.Utc).AddTicks(7837) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("fe409e05-85b0-4bc9-af33-7ed1d9f2f62c"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 245, DateTimeKind.Utc).AddTicks(1434), new DateTime(2020, 3, 22, 14, 5, 31, 245, DateTimeKind.Utc).AddTicks(1434) });

            migrationBuilder.UpdateData(
                schema: "Localization",
                table: "StateOrProvinces",
                keyColumn: "Id",
                keyValue: new Guid("fed44b42-36c8-4dd8-8a3e-e351f6d54ccc"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2020, 3, 22, 14, 5, 31, 245, DateTimeKind.Utc).AddTicks(1705), new DateTime(2020, 3, 22, 14, 5, 31, 245, DateTimeKind.Utc).AddTicks(1704) });

            migrationBuilder.CreateIndex(
                name: "IX_Districts_StateOrProvinceId",
                schema: "Localization",
                table: "Districts",
                column: "StateOrProvinceId");
        }
    }
}
