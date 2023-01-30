namespace WineSuite.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Init : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Eventi",
                c => new
                    {
                        IdEvento = c.Int(nullable: false, identity: true),
                        Titolo = c.String(nullable: false, maxLength: 50),
                        Descrizione = c.String(nullable: false),
                        SottoDescrizione = c.String(),
                        IdLuogo = c.Int(),
                        FotoVetrina = c.String(nullable: false, maxLength: 40),
                        Foto1 = c.String(maxLength: 40),
                        Foto2 = c.String(maxLength: 40),
                        Foto3 = c.String(maxLength: 40),
                        Giorno = c.DateTime(nullable: false, storeType: "date"),
                        Ora = c.Time(nullable: false, precision: 7),
                        NrPaxMax = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.IdEvento)
                .ForeignKey("dbo.Luogo", t => t.IdLuogo)
                .Index(t => t.IdLuogo);
            
            CreateTable(
                "dbo.Luogo",
                c => new
                    {
                        IdLuogo = c.Int(nullable: false, identity: true),
                        Descrizione = c.String(nullable: false, maxLength: 50),
                    })
                .PrimaryKey(t => t.IdLuogo);
            
            CreateTable(
                "dbo.Prenotazione",
                c => new
                    {
                        IdPrenotazione = c.Int(nullable: false, identity: true),
                        IdUtente = c.Int(nullable: false),
                        IdEvento = c.Int(nullable: false),
                        Note = c.String(),
                        TotPaxPrenotate = c.Int(),
                        TotDaPagare = c.Decimal(storeType: "money"),
                        TotArrivati = c.Int(),
                        TotPagContanti = c.Decimal(storeType: "money"),
                        TotPagPos = c.Decimal(storeType: "money"),
                        TotPagato = c.Decimal(storeType: "money"),
                    })
                .PrimaryKey(t => t.IdPrenotazione)
                .ForeignKey("dbo.Utenti", t => t.IdUtente)
                .ForeignKey("dbo.Eventi", t => t.IdEvento)
                .Index(t => t.IdUtente)
                .Index(t => t.IdEvento);
            
            CreateTable(
                "dbo.Rel_Tariffa-Prenotazione",
                c => new
                    {
                        IdTariffa = c.Int(nullable: false),
                        IdPrenotazione = c.Int(nullable: false),
                        NrPax = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.IdTariffa, t.IdPrenotazione, t.NrPax })
                .ForeignKey("dbo.Tariffe", t => t.IdTariffa)
                .ForeignKey("dbo.Prenotazione", t => t.IdPrenotazione)
                .Index(t => t.IdTariffa)
                .Index(t => t.IdPrenotazione);
            
            CreateTable(
                "dbo.Tariffe",
                c => new
                    {
                        IdTariffa = c.Int(nullable: false, identity: true),
                        Tariffa = c.Decimal(nullable: false, storeType: "money"),
                        DescrTariffa = c.String(nullable: false, maxLength: 50),
                    })
                .PrimaryKey(t => t.IdTariffa);
            
            CreateTable(
                "dbo.Utenti",
                c => new
                    {
                        IdUtente = c.Int(nullable: false, identity: true),
                        Ruolo = c.String(nullable: false, maxLength: 10),
                        Username = c.String(nullable: false, maxLength: 20),
                        Password = c.String(nullable: false, maxLength: 20),
                        Email = c.String(nullable: false, maxLength: 50),
                        Nome = c.String(nullable: false, maxLength: 30),
                        Cognome = c.String(nullable: false, maxLength: 30),
                        Contatto = c.String(nullable: false, maxLength: 15, fixedLength: true),
                        DataCreazione = c.DateTime(nullable: false, storeType: "date"),
                    })
                .PrimaryKey(t => t.IdUtente);
            
            CreateTable(
                "dbo.Rel_Tariffa-Evento",
                c => new
                    {
                        IdEvento = c.Int(nullable: false),
                        IdTariffa = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.IdEvento, t.IdTariffa })
                .ForeignKey("dbo.Eventi", t => t.IdEvento, cascadeDelete: true)
                .ForeignKey("dbo.Tariffe", t => t.IdTariffa, cascadeDelete: true)
                .Index(t => t.IdEvento)
                .Index(t => t.IdTariffa);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Rel_Tariffa-Evento", "IdTariffa", "dbo.Tariffe");
            DropForeignKey("dbo.Rel_Tariffa-Evento", "IdEvento", "dbo.Eventi");
            DropForeignKey("dbo.Prenotazione", "IdEvento", "dbo.Eventi");
            DropForeignKey("dbo.Prenotazione", "IdUtente", "dbo.Utenti");
            DropForeignKey("dbo.Rel_Tariffa-Prenotazione", "IdPrenotazione", "dbo.Prenotazione");
            DropForeignKey("dbo.Rel_Tariffa-Prenotazione", "IdTariffa", "dbo.Tariffe");
            DropForeignKey("dbo.Eventi", "IdLuogo", "dbo.Luogo");
            DropIndex("dbo.Rel_Tariffa-Evento", new[] { "IdTariffa" });
            DropIndex("dbo.Rel_Tariffa-Evento", new[] { "IdEvento" });
            DropIndex("dbo.Rel_Tariffa-Prenotazione", new[] { "IdPrenotazione" });
            DropIndex("dbo.Rel_Tariffa-Prenotazione", new[] { "IdTariffa" });
            DropIndex("dbo.Prenotazione", new[] { "IdEvento" });
            DropIndex("dbo.Prenotazione", new[] { "IdUtente" });
            DropIndex("dbo.Eventi", new[] { "IdLuogo" });
            DropTable("dbo.Rel_Tariffa-Evento");
            DropTable("dbo.Utenti");
            DropTable("dbo.Tariffe");
            DropTable("dbo.Rel_Tariffa-Prenotazione");
            DropTable("dbo.Prenotazione");
            DropTable("dbo.Luogo");
            DropTable("dbo.Eventi");
        }
    }
}
