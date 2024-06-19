using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SMEE.AOI.FTP.DatabaseSlim.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AncestorSessions",
                columns: table => new
                {
                    SessionId = table.Column<string>(nullable: false),
                    NewSessionId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AncestorSessions", x => x.SessionId);
                });

            migrationBuilder.CreateTable(
                name: "SessionCommands",
                columns: table => new
                {
                    SessionId = table.Column<string>(nullable: false),
                    AncestorSessionId = table.Column<string>(nullable: true),
                    State = table.Column<string>(nullable: false),
                    CreateTime = table.Column<DateTime>(type: "datetime2(0)", nullable: true),
                    StartTime = table.Column<DateTime>(type: "datetime2(0)", nullable: true),
                    DoneTime = table.Column<DateTime>(type: "datetime2(0)", nullable: true),
                    OperType = table.Column<string>(nullable: false),
                    OperParam = table.Column<string>(nullable: true),
                    ErrorMsg = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SessionCommands", x => x.SessionId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AncestorSessions");

            migrationBuilder.DropTable(
                name: "SessionCommands");
        }
    }
}
