namespace WineSuite.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PubblicaInEventi : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Eventi", "Pubblico", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Eventi", "Pubblico");
        }
    }
}
