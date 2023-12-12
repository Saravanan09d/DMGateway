using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DynamicTableCreation.Migrations
{
    /// <inheritdoc />
    public partial class DEC11 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EntityListMetadataModels",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EntityName = table.Column<string>(type: "text", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<int>(type: "integer", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EntityListMetadataModels", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "logParents",
                columns: table => new
                {
                    ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    RecordCount = table.Column<int>(type: "integer", nullable: false),
                    PassCount = table.Column<int>(type: "integer", nullable: false),
                    FailCount = table.Column<int>(type: "integer", nullable: false),
                    User_Id = table.Column<int>(type: "integer", nullable: false),
                    FileName = table.Column<string>(type: "text", nullable: false),
                    Entity_Id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_logParents", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "UserRoleModel",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RoleName = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoleModel", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserTableModel",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    RoleId = table.Column<int>(type: "integer", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    Password = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserTableModel", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EntityColumnListMetadataModels",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EntityColumnName = table.Column<string>(type: "text", nullable: false),
                    EntityId = table.Column<int>(type: "integer", nullable: false),
                    Datatype = table.Column<string>(type: "text", nullable: false),
                    Length = table.Column<int>(type: "integer", nullable: false),
                    MinLength = table.Column<int>(type: "integer", nullable: true),
                    MaxLength = table.Column<int>(type: "integer", nullable: true),
                    MaxRange = table.Column<int>(type: "integer", nullable: true),
                    MinRange = table.Column<int>(type: "integer", nullable: true),
                    DateMinValue = table.Column<string>(type: "text", nullable: false),
                    DateMaxValue = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    IsNullable = table.Column<bool>(type: "boolean", nullable: false),
                    DefaultValue = table.Column<string>(type: "text", nullable: false),
                    ListEntityId = table.Column<int>(type: "integer", nullable: false),
                    ListEntityKey = table.Column<int>(type: "integer", nullable: false),
                    ListEntityValue = table.Column<int>(type: "integer", nullable: false),
                    True = table.Column<string>(type: "text", nullable: false),
                    False = table.Column<string>(type: "text", nullable: false),
                    ColumnPrimaryKey = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<int>(type: "integer", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EntityColumnListMetadataModels", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EntityColumnListMetadataModels_EntityListMetadataModels_Ent~",
                        column: x => x.EntityId,
                        principalTable: "EntityListMetadataModels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "logChilds",
                columns: table => new
                {
                    ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ParentID = table.Column<int>(type: "integer", nullable: false),
                    ErrorMessage = table.Column<string>(type: "text", nullable: false),
                    Filedata = table.Column<string>(type: "text", nullable: false),
                    ErrorRowNumber = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_logChilds", x => x.ID);
                    table.ForeignKey(
                        name: "FK_logChilds_logParents_ParentID",
                        column: x => x.ParentID,
                        principalTable: "logParents",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EntityColumnListMetadataModels_EntityId",
                table: "EntityColumnListMetadataModels",
                column: "EntityId");

            migrationBuilder.CreateIndex(
                name: "IX_logChilds_ParentID",
                table: "logChilds",
                column: "ParentID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EntityColumnListMetadataModels");

            migrationBuilder.DropTable(
                name: "logChilds");

            migrationBuilder.DropTable(
                name: "UserRoleModel");

            migrationBuilder.DropTable(
                name: "UserTableModel");

            migrationBuilder.DropTable(
                name: "EntityListMetadataModels");

            migrationBuilder.DropTable(
                name: "logParents");
        }
    }
}
