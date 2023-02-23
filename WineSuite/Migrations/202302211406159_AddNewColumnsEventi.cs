namespace WineSuite.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddNewColumnsEventi : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Eventi", "TotPrenotazioni", c => c.Int());
            AddColumn("dbo.Eventi", "TotPaxPrenotate", c => c.Int());
            AddColumn("dbo.Eventi", "TotPaxArrivate", c => c.Int());
            AddColumn("dbo.Eventi", "TotPagato", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.Eventi", "TotContanti", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.Eventi", "TotPos", c => c.Decimal(precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Eventi", "TotPos");
            DropColumn("dbo.Eventi", "TotContanti");
            DropColumn("dbo.Eventi", "TotPagato");
            DropColumn("dbo.Eventi", "TotPaxArrivate");
            DropColumn("dbo.Eventi", "TotPaxPrenotate");
            DropColumn("dbo.Eventi", "TotPrenotazioni");
        }
    }
}
