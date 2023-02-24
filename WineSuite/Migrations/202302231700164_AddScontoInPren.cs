namespace WineSuite.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddScontoInPren : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Prenotazione", "Sconto", c => c.Decimal(storeType: "money"));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Prenotazione", "Sconto");
        }
    }
}
