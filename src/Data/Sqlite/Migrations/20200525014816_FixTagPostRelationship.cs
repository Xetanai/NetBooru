using Microsoft.EntityFrameworkCore.Migrations;

namespace NetBooru.Web.Migrations
{
    public partial class FixTagPostRelationship : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("PRAGMA foreign_keys=off");

            migrationBuilder.RenameTable(
                name: "Tags",
                newName: "_Tags_old"
            );

            migrationBuilder.CreateTable(
                name: "Tags",
                columns: table => new
                {
                    Id = table.Column<ulong>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(nullable: false),
                    CategoryId = table.Column<ulong>(nullable: true)
                },
                constraints: table =>
                {
                    table.ForeignKey(
                        name: "FK_Tags_TagCategories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "TagCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.Sql(
                "INSERT INTO Tags (Id, Name, CategoryId) SELECT Id, Name, CategoryId FROM _Tags_old");

            migrationBuilder.DropTable(
                name: "_Tags_old");

            migrationBuilder.RenameTable(
                name: "Users",
                newName: "_Users_old"
            );

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<ulong>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Username = table.Column<string>(nullable: false),
                    HasAvatar = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.Sql(
                "INSERT INTO Users (Id, Username, HasAvatar) SELECT Id, Username, HasAvatar FROM _Users_old");

            migrationBuilder.CreateTable(
                name: "PostTag",
                columns: table => new
                {
                    PostId = table.Column<ulong>(type: "INTEGER", nullable: false),
                    TagId = table.Column<ulong>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PostTag", x => new { x.PostId, x.TagId });
                    table.ForeignKey(
                        name: "FK_PostTag_Posts_PostId",
                        column: x => x.PostId,
                        principalTable: "Posts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PostTag_Tags_TagId",
                        column: x => x.TagId,
                        principalTable: "Tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PostTag_TagId",
                table: "PostTag",
                column: "TagId");

            migrationBuilder.Sql("PRAGMA foreign_keys=on");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PostTag");

            migrationBuilder.AddColumn<string>(
                name: "PasswordHash",
                table: "Users",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<ulong>(
                name: "PostId",
                table: "Tags",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.Sql("PRAGMA foreign_keys=off");

            migrationBuilder.RenameTable(
                name: "Tags",
                newName: "_Tags_old"
            );

            migrationBuilder.CreateTable(
                name: "Tags",
                columns: table => new
                {
                    Id = table.Column<ulong>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(nullable: false),
                    CategoryId = table.Column<ulong>(nullable: true),
                    PostId = table.Column<ulong>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tags", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tags_TagCategories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "TagCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Tags_Posts_PostId",
                        column: x => x.PostId,
                        principalTable: "Posts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.Sql(
                "INSERT INTO Tags SELECT * FROM _Tags_old");

            migrationBuilder.Sql("PRAGMA foreign_keys=on");
        }
    }
}
