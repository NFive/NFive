namespace NFive.Server.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class Init : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Sessions",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        IpAddress = c.String(nullable: false, maxLength: 15, unicode: false),
                        Connected = c.DateTime(nullable: false, precision: 0),
                        Disconnected = c.DateTime(precision: 0),
                        DisconnectReason = c.String(maxLength: 200, unicode: false),
                        UserId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        SteamId = c.Long(nullable: false),
                        Name = c.String(nullable: false, maxLength: 32, unicode: false),
                        AcceptedRules = c.DateTime(precision: 0),
                        Created = c.DateTime(nullable: false, precision: 0),
                        Deleted = c.DateTime(precision: 0),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.SteamId, unique: true);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Sessions", "UserId", "dbo.Users");
            DropIndex("dbo.Users", new[] { "SteamId" });
            DropIndex("dbo.Sessions", new[] { "UserId" });
            DropTable("dbo.Users");
            DropTable("dbo.Sessions");
        }
    }
}
