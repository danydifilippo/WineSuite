namespace WineSuite.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddColumnDateBooking : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Prenotazione", "DataPrenotazione", c => c.DateTime(storeType: "date"));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Prenotazione", "DataPrenotazione");
        }
    }
}
