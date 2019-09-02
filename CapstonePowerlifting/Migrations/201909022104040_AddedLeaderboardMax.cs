namespace CapstonePowerlifting.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedLeaderboardMax : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.LeaderboardMaxes",
                c => new
                    {
                        LeaderboardMaxId = c.Int(nullable: false, identity: true),
                        UserName = c.String(),
                        Age = c.Int(nullable: false),
                        Weight = c.Double(nullable: false),
                        Squat = c.Double(nullable: false),
                        Bench = c.Double(nullable: false),
                        Deadlift = c.Double(nullable: false),
                        Total = c.Double(nullable: false),
                        Wilks = c.Double(nullable: false),
                        ApplicationId = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.LeaderboardMaxId)
                .ForeignKey("dbo.AspNetUsers", t => t.ApplicationId)
                .Index(t => t.ApplicationId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.LeaderboardMaxes", "ApplicationId", "dbo.AspNetUsers");
            DropIndex("dbo.LeaderboardMaxes", new[] { "ApplicationId" });
            DropTable("dbo.LeaderboardMaxes");
        }
    }
}
