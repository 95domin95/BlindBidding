using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace BlindBidding.Data.Migrations
{
    public partial class Migration4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Auctions_Bids_AcceptedBidBidId",
                table: "Auctions");

            migrationBuilder.DropForeignKey(
                name: "FK_Auctions_Categories_CategoryId",
                table: "Auctions");

            migrationBuilder.DropForeignKey(
                name: "FK_Categories_Categories_SubcategoryOfCategoryId",
                table: "Categories");

            migrationBuilder.DropIndex(
                name: "IX_Auctions_AcceptedBidBidId",
                table: "Auctions");

            migrationBuilder.DropColumn(
                name: "AcceptedBidBidId",
                table: "Auctions");

            migrationBuilder.RenameColumn(
                name: "SubcategoryOfCategoryId",
                table: "Categories",
                newName: "SubcategoryOfId");

            migrationBuilder.RenameIndex(
                name: "IX_Categories_SubcategoryOfCategoryId",
                table: "Categories",
                newName: "IX_Categories_SubcategoryOfId");

            migrationBuilder.AddColumn<int>(
                name: "AuctionId",
                table: "Bids",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "CategoryId",
                table: "Auctions",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Bids_AuctionId",
                table: "Bids",
                column: "AuctionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Auctions_Categories_CategoryId",
                table: "Auctions",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "CategoryId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Bids_Auctions_AuctionId",
                table: "Bids",
                column: "AuctionId",
                principalTable: "Auctions",
                principalColumn: "AuctionId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Categories_Categories_SubcategoryOfId",
                table: "Categories",
                column: "SubcategoryOfId",
                principalTable: "Categories",
                principalColumn: "CategoryId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Auctions_Categories_CategoryId",
                table: "Auctions");

            migrationBuilder.DropForeignKey(
                name: "FK_Bids_Auctions_AuctionId",
                table: "Bids");

            migrationBuilder.DropForeignKey(
                name: "FK_Categories_Categories_SubcategoryOfId",
                table: "Categories");

            migrationBuilder.DropIndex(
                name: "IX_Bids_AuctionId",
                table: "Bids");

            migrationBuilder.DropColumn(
                name: "AuctionId",
                table: "Bids");

            migrationBuilder.RenameColumn(
                name: "SubcategoryOfId",
                table: "Categories",
                newName: "SubcategoryOfCategoryId");

            migrationBuilder.RenameIndex(
                name: "IX_Categories_SubcategoryOfId",
                table: "Categories",
                newName: "IX_Categories_SubcategoryOfCategoryId");

            migrationBuilder.AlterColumn<int>(
                name: "CategoryId",
                table: "Auctions",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddColumn<int>(
                name: "AcceptedBidBidId",
                table: "Auctions",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Auctions_AcceptedBidBidId",
                table: "Auctions",
                column: "AcceptedBidBidId",
                unique: true,
                filter: "[AcceptedBidBidId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Auctions_Bids_AcceptedBidBidId",
                table: "Auctions",
                column: "AcceptedBidBidId",
                principalTable: "Bids",
                principalColumn: "BidId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Auctions_Categories_CategoryId",
                table: "Auctions",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "CategoryId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Categories_Categories_SubcategoryOfCategoryId",
                table: "Categories",
                column: "SubcategoryOfCategoryId",
                principalTable: "Categories",
                principalColumn: "CategoryId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
