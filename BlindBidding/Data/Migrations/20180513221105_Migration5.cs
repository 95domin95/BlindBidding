using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace BlindBidding.Data.Migrations
{
    public partial class Migration5 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BidId",
                table: "Auctions",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Auctions_BidId",
                table: "Auctions",
                column: "BidId");

            migrationBuilder.AddForeignKey(
                name: "FK_Auctions_Bids_BidId",
                table: "Auctions",
                column: "BidId",
                principalTable: "Bids",
                principalColumn: "BidId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Auctions_Bids_BidId",
                table: "Auctions");

            migrationBuilder.DropIndex(
                name: "IX_Auctions_BidId",
                table: "Auctions");

            migrationBuilder.DropColumn(
                name: "BidId",
                table: "Auctions");
        }
    }
}
