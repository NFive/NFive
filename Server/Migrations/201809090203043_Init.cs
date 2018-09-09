namespace NFive.Server.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Init : DbMigration
    {
        public override void Up()
        {
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
            DropIndex("dbo.Users", new[] { "SteamId" });
            DropTable("dbo.Users");
        }
    }
}
