using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel;

namespace Gopas.Dedicnost;

public class Dedicnost
{
    public static void Main()
    {
        using var db = new MyContext();
        db.Database.EnsureCreated();

        db.Kola.Add(new Kolo { Name = "SP", Vaha = 15 });
        db.Auta.Add(new Auto { Name = "Q7", PocteLidi = 3 });
    }
}

class MyContext : DbContext
{
    public DbSet<Vozidlo> Vozidla { get; set; }
    public DbSet<Auto> Auta { get; set; }
    public DbSet<Kolo> Kola { get; set; }

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
        optionsBuilder.UseSqlServer(builder.ToString(), o => {});
        optionsBuilder.EnableSensitiveDataLogging(true); // může se hodit, pro útočníka jsou logy celkem dobrý zdroj informací
        optionsBuilder.LogTo(Console.WriteLine);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Vozidlo>(b =>
        {
            b.ToTable("TPH_Vozidla");
        });

        modelBuilder.Entity<Auto>(b =>
        {
            b.ToTable("TPH_Vozidla");
            b.HasBaseType<Vozidlo>();
            b.HasDiscriminator<string>("D").HasValue("AU"); // << Podle toho se pak pozná, zda je to kolo a nebo auto
        });

        modelBuilder.Entity<Kolo>(b =>
        {
            b.ToTable("TPH_Vozidla");
            b.HasBaseType<Vozidlo>();
            b.HasDiscriminator<string>("D").HasValue("KO"); // << Podle toho se pak pozná, zda je to kolo a nebo auto
        });
    }
}

abstract class Vozidlo
{
    public int Id { get; set; }
    public string Name { get; set; }
}

class Auto : Vozidlo
{
    public int PocteLidi { get; set; }
}

class Kolo : Vozidlo
{
    public float Vaha { get; set; }
}
