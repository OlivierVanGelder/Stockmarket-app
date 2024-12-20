using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class namechangeUser_IdtoUserId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_User_Stocks_AspNetUsers_UserId",
                table: "User_Stocks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_User_Stocks",
                table: "User_Stocks");

            migrationBuilder.DropIndex(
                name: "IX_User_Stocks_UserId",
                table: "User_Stocks");

            migrationBuilder.DropColumn(
                name: "User_Id",
                table: "User_Stocks");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "User_Stocks",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_User_Stocks",
                table: "User_Stocks",
                columns: new[] { "UserId", "StockId" });

            migrationBuilder.AddForeignKey(
                name: "FK_User_Stocks_AspNetUsers_UserId",
                table: "User_Stocks",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_User_Stocks_AspNetUsers_UserId",
                table: "User_Stocks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_User_Stocks",
                table: "User_Stocks");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "User_Stocks",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AddColumn<string>(
                name: "User_Id",
                table: "User_Stocks",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_User_Stocks",
                table: "User_Stocks",
                columns: new[] { "User_Id", "StockId" });

            migrationBuilder.CreateIndex(
                name: "IX_User_Stocks_UserId",
                table: "User_Stocks",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_User_Stocks_AspNetUsers_UserId",
                table: "User_Stocks",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
