using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bloggie.Web.Migrations
{
    /// <inheritdoc />
    public partial class RenameTagTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BlogPostComment_BlogPosts_BlogPostId",
                table: "BlogPostComment");

            migrationBuilder.DropForeignKey(
                name: "FK_BlogPostLike_BlogPosts_BlogPostId",
                table: "BlogPostLike");

            migrationBuilder.DropTable(
                name: "BlogPostTag");

            migrationBuilder.DropIndex(
                name: "IX_BlogPostLike_BlogPostId",
                table: "BlogPostLike");

            migrationBuilder.DropIndex(
                name: "IX_BlogPostComment_BlogPostId",
                table: "BlogPostComment");

            migrationBuilder.AddColumn<Guid>(
                name: "BlogId",
                table: "BlogPostLike",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "BlogId",
                table: "BlogPostComment",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "BlogTag",
                columns: table => new
                {
                    BlogPostsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TagsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlogTag", x => new { x.BlogPostsId, x.TagsId });
                    table.ForeignKey(
                        name: "FK_BlogTag_BlogPosts_BlogPostsId",
                        column: x => x.BlogPostsId,
                        principalTable: "BlogPosts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BlogTag_Tags_TagsId",
                        column: x => x.TagsId,
                        principalTable: "Tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BlogPostLike_BlogId",
                table: "BlogPostLike",
                column: "BlogId");

            migrationBuilder.CreateIndex(
                name: "IX_BlogPostComment_BlogId",
                table: "BlogPostComment",
                column: "BlogId");

            migrationBuilder.CreateIndex(
                name: "IX_BlogTag_TagsId",
                table: "BlogTag",
                column: "TagsId");

            migrationBuilder.AddForeignKey(
                name: "FK_BlogPostComment_BlogPosts_BlogId",
                table: "BlogPostComment",
                column: "BlogId",
                principalTable: "BlogPosts",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BlogPostLike_BlogPosts_BlogId",
                table: "BlogPostLike",
                column: "BlogId",
                principalTable: "BlogPosts",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BlogPostComment_BlogPosts_BlogId",
                table: "BlogPostComment");

            migrationBuilder.DropForeignKey(
                name: "FK_BlogPostLike_BlogPosts_BlogId",
                table: "BlogPostLike");

            migrationBuilder.DropTable(
                name: "BlogTag");

            migrationBuilder.DropIndex(
                name: "IX_BlogPostLike_BlogId",
                table: "BlogPostLike");

            migrationBuilder.DropIndex(
                name: "IX_BlogPostComment_BlogId",
                table: "BlogPostComment");

            migrationBuilder.DropColumn(
                name: "BlogId",
                table: "BlogPostLike");

            migrationBuilder.DropColumn(
                name: "BlogId",
                table: "BlogPostComment");

            migrationBuilder.CreateTable(
                name: "BlogPostTag",
                columns: table => new
                {
                    BlogPostsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TagsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlogPostTag", x => new { x.BlogPostsId, x.TagsId });
                    table.ForeignKey(
                        name: "FK_BlogPostTag_BlogPosts_BlogPostsId",
                        column: x => x.BlogPostsId,
                        principalTable: "BlogPosts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BlogPostTag_Tags_TagsId",
                        column: x => x.TagsId,
                        principalTable: "Tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BlogPostLike_BlogPostId",
                table: "BlogPostLike",
                column: "BlogPostId");

            migrationBuilder.CreateIndex(
                name: "IX_BlogPostComment_BlogPostId",
                table: "BlogPostComment",
                column: "BlogPostId");

            migrationBuilder.CreateIndex(
                name: "IX_BlogPostTag_TagsId",
                table: "BlogPostTag",
                column: "TagsId");

            migrationBuilder.AddForeignKey(
                name: "FK_BlogPostComment_BlogPosts_BlogPostId",
                table: "BlogPostComment",
                column: "BlogPostId",
                principalTable: "BlogPosts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BlogPostLike_BlogPosts_BlogPostId",
                table: "BlogPostLike",
                column: "BlogPostId",
                principalTable: "BlogPosts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
