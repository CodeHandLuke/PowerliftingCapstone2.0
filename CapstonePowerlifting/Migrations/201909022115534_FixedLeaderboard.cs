namespace CapstonePowerlifting.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FixedLeaderboard : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.LeaderboardMaxes", "ApplicationId", "dbo.AspNetUsers");
            DropIndex("dbo.LeaderboardMaxes", new[] { "ApplicationId" });
            AddColumn("dbo.LeaderboardMaxes", "UserId", c => c.Int(nullable: false));
            DropColumn("dbo.LeaderboardMaxes", "ApplicationId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.LeaderboardMaxes", "ApplicationId", c => c.String(maxLength: 128));
            DropColumn("dbo.LeaderboardMaxes", "UserId");
            CreateIndex("dbo.LeaderboardMaxes", "ApplicationId");
            AddForeignKey("dbo.LeaderboardMaxes", "ApplicationId", "dbo.AspNetUsers", "Id");
        }
    }
}
