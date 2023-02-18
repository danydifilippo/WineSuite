namespace WineSuite.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddColEliminato : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Eventi", "Eliminato", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Eventi", "Eliminato");
        }
    }
}
