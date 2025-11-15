using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend_auto_market.Migrations
{
    /// <inheritdoc />
    public partial class RebameVehiclesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VehiclePhotos_Vehicles_VehicleListingId",
                table: "VehiclePhotos");

            migrationBuilder.DropForeignKey(
                name: "FK_Vehicles_BodyTypes_BodyTypeId",
                table: "Vehicles");

            migrationBuilder.DropForeignKey(
                name: "FK_Vehicles_Cities_CityId",
                table: "Vehicles");

            migrationBuilder.DropForeignKey(
                name: "FK_Vehicles_Users_UserId",
                table: "Vehicles");

            migrationBuilder.DropForeignKey(
                name: "FK_Vehicles_VehicleConditions_ConditionId",
                table: "Vehicles");

            migrationBuilder.DropForeignKey(
                name: "FK_Vehicles_VehicleModels_ModelId",
                table: "Vehicles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Vehicles",
                table: "Vehicles");

            migrationBuilder.RenameTable(
                name: "Vehicles",
                newName: "VehicleListings");

            migrationBuilder.RenameIndex(
                name: "IX_Vehicles_UserId",
                table: "VehicleListings",
                newName: "IX_VehicleListings_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Vehicles_ModelId",
                table: "VehicleListings",
                newName: "IX_VehicleListings_ModelId");

            migrationBuilder.RenameIndex(
                name: "IX_Vehicles_ConditionId",
                table: "VehicleListings",
                newName: "IX_VehicleListings_ConditionId");

            migrationBuilder.RenameIndex(
                name: "IX_Vehicles_CityId",
                table: "VehicleListings",
                newName: "IX_VehicleListings_CityId");

            migrationBuilder.RenameIndex(
                name: "IX_Vehicles_BodyTypeId",
                table: "VehicleListings",
                newName: "IX_VehicleListings_BodyTypeId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_VehicleListings",
                table: "VehicleListings",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_VehicleListings_BodyTypes_BodyTypeId",
                table: "VehicleListings",
                column: "BodyTypeId",
                principalTable: "BodyTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_VehicleListings_Cities_CityId",
                table: "VehicleListings",
                column: "CityId",
                principalTable: "Cities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_VehicleListings_Users_UserId",
                table: "VehicleListings",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_VehicleListings_VehicleConditions_ConditionId",
                table: "VehicleListings",
                column: "ConditionId",
                principalTable: "VehicleConditions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_VehicleListings_VehicleModels_ModelId",
                table: "VehicleListings",
                column: "ModelId",
                principalTable: "VehicleModels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_VehiclePhotos_VehicleListings_VehicleListingId",
                table: "VehiclePhotos",
                column: "VehicleListingId",
                principalTable: "VehicleListings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VehicleListings_BodyTypes_BodyTypeId",
                table: "VehicleListings");

            migrationBuilder.DropForeignKey(
                name: "FK_VehicleListings_Cities_CityId",
                table: "VehicleListings");

            migrationBuilder.DropForeignKey(
                name: "FK_VehicleListings_Users_UserId",
                table: "VehicleListings");

            migrationBuilder.DropForeignKey(
                name: "FK_VehicleListings_VehicleConditions_ConditionId",
                table: "VehicleListings");

            migrationBuilder.DropForeignKey(
                name: "FK_VehicleListings_VehicleModels_ModelId",
                table: "VehicleListings");

            migrationBuilder.DropForeignKey(
                name: "FK_VehiclePhotos_VehicleListings_VehicleListingId",
                table: "VehiclePhotos");

            migrationBuilder.DropPrimaryKey(
                name: "PK_VehicleListings",
                table: "VehicleListings");

            migrationBuilder.RenameTable(
                name: "VehicleListings",
                newName: "Vehicles");

            migrationBuilder.RenameIndex(
                name: "IX_VehicleListings_UserId",
                table: "Vehicles",
                newName: "IX_Vehicles_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_VehicleListings_ModelId",
                table: "Vehicles",
                newName: "IX_Vehicles_ModelId");

            migrationBuilder.RenameIndex(
                name: "IX_VehicleListings_ConditionId",
                table: "Vehicles",
                newName: "IX_Vehicles_ConditionId");

            migrationBuilder.RenameIndex(
                name: "IX_VehicleListings_CityId",
                table: "Vehicles",
                newName: "IX_Vehicles_CityId");

            migrationBuilder.RenameIndex(
                name: "IX_VehicleListings_BodyTypeId",
                table: "Vehicles",
                newName: "IX_Vehicles_BodyTypeId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Vehicles",
                table: "Vehicles",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_VehiclePhotos_Vehicles_VehicleListingId",
                table: "VehiclePhotos",
                column: "VehicleListingId",
                principalTable: "Vehicles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Vehicles_BodyTypes_BodyTypeId",
                table: "Vehicles",
                column: "BodyTypeId",
                principalTable: "BodyTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Vehicles_Cities_CityId",
                table: "Vehicles",
                column: "CityId",
                principalTable: "Cities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Vehicles_Users_UserId",
                table: "Vehicles",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Vehicles_VehicleConditions_ConditionId",
                table: "Vehicles",
                column: "ConditionId",
                principalTable: "VehicleConditions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Vehicles_VehicleModels_ModelId",
                table: "Vehicles",
                column: "ModelId",
                principalTable: "VehicleModels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
