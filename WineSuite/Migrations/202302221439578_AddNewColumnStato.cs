namespace WineSuite.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddNewColumnStato : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Prenotazione", "Stato", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Prenotazione", "Stato");
        }
    }
}
