using Gopas.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace Gopas;

// Není tady všechno ze druhého dne, plno věcí se upravovalo v kódu z předchozího dne.
// Ale co šlo, tak jsem cpal sem, aby tam toho nebylo až moc.
public class DruhyDen
{
    public static void Main()
    {
        using var db = new MyContext();        
        db.Database.EnsureCreated();

        db.Owners.Where(x => EF.Property<DateTimeOffset>(x, "LastUpdated") == DateTimeOffset.Now).Load(); // přístup k rpoměnným, které nejsou reprezentované v objektu (Oweners), ale v DB existuje

        //db.OwnersLite.Add(new OwnerLite
        //{
        //    FirstName = "Test",
        //    LastName = "Test",
        //    Dogs = new[]
        //    {
        //        new Dog
        //        {
        //            Name = "Alik",
        //            DateOfBirth = DateTimeOffset.Now,
        //        }
        //    }
        //});

        //db.SaveChanges();

        var items = db.OwnersLite.Include(o => o.Dogs).ToList();
        foreach (var item in items)
        {
            Console.WriteLine($"Id: {item.Id}, FirstName: {item.FirstName}, Dogs: {item.Dogs.Count}");
        }

        // pro konverze
        var owner = db.Owners.First();
        owner.Duration = new Duration(453);
        db.SaveChanges();

        // využití KeyLess (HasNoKey)
        db.Set<MojeNeco>().FromSqlRaw("Select FirstName as Name from T_Owner")
            .Where(x => x.Name != string.Empty)
            .Load();

        // Query tags (neřekl k čemu to je, tak asi zase pogooglit)
        db.Set<MojeNeco>().FromSqlRaw("Select FirstName as Name from T_Owner")
            .Where(x => x.Name != string.Empty)
            .TagWithCallSite()
            .Load();
    }
}