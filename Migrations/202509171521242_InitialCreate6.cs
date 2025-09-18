namespace FoodWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate6 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.UserAchievements", "AchievementId", "dbo.Achievements");
            DropForeignKey("dbo.UserChallenges", "ChallengeId", "dbo.Challenges");
            DropIndex("dbo.UserAchievements", new[] { "UserId", "AchievementId" });
            DropIndex("dbo.UserChallenges", new[] { "UserId", "ChallengeId" });
            DropTable("dbo.Achievements");
            DropTable("dbo.UserAchievements");
            DropTable("dbo.Challenges");
            DropTable("dbo.UserChallenges");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.UserChallenges",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        ChallengeId = c.Int(nullable: false),
                        CurrentProgress = c.Int(nullable: false),
                        IsCompleted = c.Boolean(nullable: false),
                        JoinedAt = c.DateTime(nullable: false),
                        CompletedAt = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Challenges",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(nullable: false, maxLength: 100),
                        Description = c.String(maxLength: 500),
                        RequiredProduct = c.String(nullable: false),
                        RequiredQuantity = c.Int(nullable: false),
                        RewardPoints = c.Decimal(nullable: false, precision: 18, scale: 2),
                        StartDate = c.DateTime(nullable: false),
                        EndDate = c.DateTime(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        CreatedAt = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.UserAchievements",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        AchievementId = c.Int(nullable: false),
                        IsUnlocked = c.Boolean(nullable: false),
                        UnlockedAt = c.DateTime(),
                        Progress = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Achievements",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(nullable: false, maxLength: 100),
                        Description = c.String(maxLength: 500),
                        AchievementType = c.String(nullable: false),
                        RequiredProduct = c.String(),
                        RequiredQuantity = c.Int(),
                        RequiredAmount = c.Decimal(precision: 18, scale: 2),
                        RequiredChallenges = c.Int(),
                        RewardPoints = c.Decimal(nullable: false, precision: 18, scale: 2),
                        BadgeIcon = c.String(),
                        IsActive = c.Boolean(nullable: false),
                        CreatedAt = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateIndex("dbo.UserChallenges", new[] { "UserId", "ChallengeId" }, unique: true);
            CreateIndex("dbo.UserAchievements", new[] { "UserId", "AchievementId" }, unique: true);
            AddForeignKey("dbo.UserChallenges", "ChallengeId", "dbo.Challenges", "Id", cascadeDelete: true);
            AddForeignKey("dbo.UserAchievements", "AchievementId", "dbo.Achievements", "Id", cascadeDelete: true);
        }
    }
}
