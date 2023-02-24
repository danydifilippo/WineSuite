namespace WineSuite.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTableRel_Tariffescelte : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Rel_TariffeScelte_Pren",
                c => new
                    {
                        IdPrenotazione = c.Int(nullable: false),
                        IdTariffa = c.Int(nullable: false),
                        NrPax = c.Int(nullable: false),
                        Prenotazione_IdPrenotazione = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.IdPrenotazione)
                .ForeignKey("dbo.Tariffe", t => t.IdTariffa)
                .ForeignKey("dbo.Prenotazione", t => t.Prenotazione_IdPrenotazione)
                .Index(t => t.IdTariffa)
                .Index(t => t.Prenotazione_IdPrenotazione);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Rel_TariffeScelte_Pren", "Prenotazione_IdPrenotazione", "dbo.Prenotazione");
            DropForeignKey("dbo.Rel_TariffeScelte_Pren", "IdTariffa", "dbo.Tariffe");
            DropIndex("dbo.Rel_TariffeScelte_Pren", new[] { "Prenotazione_IdPrenotazione" });
            DropIndex("dbo.Rel_TariffeScelte_Pren", new[] { "IdTariffa" });
            DropTable("dbo.Rel_TariffeScelte_Pren");
        }
    }
}
