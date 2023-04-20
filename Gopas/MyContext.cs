using Gopas.Configurations;
using Gopas.Entities;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace Gopas;

partial class MyContext
{
    [DbFunction(Name = "Test", IsBuiltIn = true)] // Mohu si definovat vlastní funkci, kterou pak mohu využít v dotazu
    public static int Foo()
    {
        return 10;
    }
}

partial class MyContext : DbContext
{
    public DbSet<Owner> Owners { get; set; }
    public DbSet<OwnerLite> OwnersLite { get; set; }
    public DbSet<Dog> Dogs { get; set; }
    public DbSet<MojeNeco> Neco { get; set; }
    

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        if (optionsBuilder.IsConfigured)
        {
            return;
        }

        var builder = new SqlConnectionStringBuilder();
        builder.DataSource = ".";
        builder.InitialCatalog = "Gopas";
        builder.IntegratedSecurity = true;
        builder.TrustServerCertificate = true;
        builder.ConnectRetryCount = 0;
        optionsBuilder.UseSqlServer(builder.ToString(), o =>
        {
            //o.MinBatchSize(2);  // nejsem si jistý, ale řekl bych, žeto funguje tak, že když dělám něco s více záznami, mohu omezit na kolika záznamech se to vždy provádí a vytvoří se dávka
            //o.MaxBatchSize(20); // pokud je položek více, rozdělí se do více dávek
        });
        optionsBuilder.EnableSensitiveDataLogging(true); // může se hodit, pro útočníka jsou logy celkem dobrý zdroj informací
        optionsBuilder.LogTo(Console.WriteLine);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfiguration(new OwnerConfiguration());

        // Konvence - obecné
        /* Konvence je vhodné dávat na začátek
         * Konvence psát separátně projednotilivé typy a psát je bez vzájemných závislostí
         * jsou méně přehledné, než když je to popsané u jednotlivých entit
        */
        //foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        //{
        //    entityType.SetTableName($"T_{entityType.DisplayName()}");
        //    foreach (var prop in entityType.GetProperties())
        //    {
        //        if (prop.ClrType == typeof(string))
        //        {
        //            prop.SetMaxLength(1000);
        //        }
        //    }
        //}

        // vytvoření sekvence pro UseHiLo
        modelBuilder.HasSequence("SEQ_ID")
            .IncrementsBy(50);

        // HasNoKey
        modelBuilder.Entity<MojeNeco>().HasNoKey();

        // Seeding
        modelBuilder.Entity<Owner>().HasData(new Owner
        {
            Id = -1,
            IsActive = true,
            FirstName = "Test",
        });

        // Shared entities
        var testConf = modelBuilder.SharedTypeEntity<Dictionary<string, object>>("Test"); // Toto je obecný zápis. Namísto Dictonary tam může být konkrétní typ
                                                                                          // a to třeba TestPropertyBag
        testConf.Property<int>("Id");
        testConf.Property<string>("Name");
    }
}