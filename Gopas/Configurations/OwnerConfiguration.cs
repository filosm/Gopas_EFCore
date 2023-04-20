using Gopas.Convertors;
using Gopas.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace Gopas.Configurations;

public class OwnerConfiguration : IEntityTypeConfiguration<Owner>
{
    public void Configure(EntityTypeBuilder<Owner> builder)
    {
        // Možnosti nastavení pro tabulky a jednotlivé proměnné

        builder.ToTable("T_Owner");
        builder.Property(x => x.LastName)
            .HasColumnName("LastName")
            .HasMaxLength(200) // nvaChar(max) se obtížně indexuje
            .IsUnicode(true)
            .IsRequired();

        builder.Property(x => x.LastName)
                .HasColumnName("FirstName")
                .HasMaxLength(200)
                .IsUnicode(true)
                .IsRequired();

        builder.HasMany(x => x.Dogs)
            .WithOne(x => x.Owner)
            .IsRequired(false)
            .HasForeignKey(x => x.OwnerId)
            .OnDelete(DeleteBehavior.SetNull);

        // Generování klíčů
        // - na traně SQL serveru
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).UseIdentityColumn(); // vygeneruje se (moc toho k tomu nebylo řečeno, případně pogooglit)
        builder.Property(x => x.Id).UseHiLo("SEQ_ID");    // mohu přidělovat rozsahy. využívá se, když je více zdrojů/konzumentů.
                                                          // Funguje to trochu jinak při insertu. Vygeneruje se dávka a ta se pak pro insertu využívá a negeneruje se Id při každém insertu, ale použije se jedna z vygenerovaných hodnot
                                                          // využívají se k tomu "sql sekvence", v případě, že by do DB přistupovalo více aplikací, mohou se pak stát úzkým hrdlem při vkládání. To je však při extrémním počtu přístupů
                                                          // - na straně aplikace
                                                          // využívají se většinou GUID. Nevýhoda: blbě se řadí

        // UsePropertyAccessMode
        builder.Property(x => x.FirstName)
            .UsePropertyAccessMode(PropertyAccessMode.Field); // Nastavuji zda se jde přes property či rovnou do field (když jde před property, mohou se v setteru dělat validace apod)

        // Backing fields
        // TZV: Shadow property
        // - není reprezentovaná v objektu ale v tabulce existuje. využívá se třeba pro traccing values, když nechci aby je někdo viděl
        builder.Property<DateTimeOffset>("LastUpdated");

        // OwnTypes
        // objekt Address nemá ID
        builder.OwnsOne(x => x.ContactAddress); // vloží se přímo do objektu Owners a nevytváří se další tabulka
        builder.OwnsOne(x => x.BillingAddress); // vloží se přímo do objektu Owners a nevytváří se další tabulka
        builder.OwnsOne(x => x.ShippingAddress); // vloží se přímo do objektu Owners a nevytváří se další tabulka

        // Model level filters
        builder.HasQueryFilter(x => x.IsActive);    // Do jakého koli dotazu se vždy tato podmínka přidá
                                                    // využívá se pro naznačené třídění a nebo například, když je v DB více uživatelů a chci podle uživatele filtrovat co může vidět (MultiTenant prostředí)

        // konverze
        builder.Property(x => x.Duration)
            .HasConversion<DurationConverter>(/*new DurationComparer()*/); // DurationComparer - využívá se k tomu aby se neprováděl například update, když je hodnota stejná.
                                                                           // Tak jak je to teď bez konvertoru se provede update vždy při změně i když je to stejná hodnota
        
        
    }
}