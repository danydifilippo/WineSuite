namespace WineSuite.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddAddressInPlace : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Luogo", "Indirizzo", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Luogo", "Indirizzo");
        }
    }
}
