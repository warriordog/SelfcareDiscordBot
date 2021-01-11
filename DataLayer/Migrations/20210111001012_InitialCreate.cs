using Microsoft.EntityFrameworkCore.Migrations;

namespace SelfcareBot.DataLayer.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                "KnownUsers",
                table => new
                {
                    Id = table.Column<int>("INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DiscordId = table.Column<ulong>("INTEGER", nullable: false),
                    Username = table.Column<string>("text",      nullable: false),
                    Discriminator = table.Column<string>("text", nullable: false)
                },
                constraints: table =>
                             {
                                 table.PrimaryKey("PK_KnownUsers", x => x.Id);
                                 table.UniqueConstraint("AK_KnownUsers_DiscordId", x => x.DiscordId);
                             }
            );

            migrationBuilder.CreateTable(
                "UserScores",
                table => new
                {
                    Id = table.Column<int>("INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    KnownUserId = table.Column<int>("INTEGER", nullable: false),
                    Category = table.Column<string>("text", nullable: false),
                    Score = table.Column<int>("INTEGER", nullable: false)
                },
                constraints: table =>
                             {
                                 table.PrimaryKey("PK_UserScores", x => x.Id);
                                 table.ForeignKey("FK_UserScores_KnownUsers_KnownUserId", x => x.KnownUserId, "KnownUsers", "Id", onDelete: ReferentialAction.Cascade);
                             }
            );

            migrationBuilder.CreateIndex("IX_UserScores_KnownUserId", "UserScores", "KnownUserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable("UserScores");

            migrationBuilder.DropTable("KnownUsers");
        }
    }
}