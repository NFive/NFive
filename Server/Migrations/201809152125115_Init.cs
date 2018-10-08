namespace NFive.Server.Migrations
{
	using System.Data.Entity.Migrations;
    
    public partial class Init : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.BootHistories",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Created = c.DateTime(nullable: false, precision: 0),
                        LastActive = c.DateTime(nullable: false, precision: 0),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        License = c.String(nullable: false, maxLength: 40, unicode: false),
                        SteamId = c.Long(),
                        Name = c.String(nullable: false, maxLength: 32, unicode: false),
                        Created = c.DateTime(nullable: false, precision: 0),
                        Deleted = c.DateTime(precision: 0),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.License, unique: true)
                .Index(t => t.SteamId, unique: true);
        }
        
        public override void Down()
        {
            DropIndex("dbo.Users", new[] { "SteamId" });
            DropIndex("dbo.Users", new[] { "License" });
            DropTable("dbo.Users");
            DropTable("dbo.BootHistories");
        }
    }
}
