using Gopas.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System;

namespace Gopas;

public class PrvniDen
{
    public static void Main()
    {
        using var db = new MyContext();
        db.Database.EnsureDeleted();
        db.Database.EnsureCreated();
        var value = "test";
        db.Owners.Where(o => o.FirstName == "").Load(); // když vložím konstantu, bude v přeloženém dotazu také jako konstanta, ale když tam dám proměnnou, tak tam bude reprezentovaná proměnnou (lepší pro cachování)
        const string TableName = "T_Owners"; // V query nemůže být jméno tabulky jako proměnná. Proto se i když je to vložené jako parametr nakonec vloží jako konstata

        db.Owners.FromSqlInterpolated($"select * {TableName} where FirstName = {value}") // velká výhoda že si můžu napsat svůj model. Metoda vrací queryable, takže mohu dělat x dalších operací.
            .OrderBy(x => x.FirstName)
            .Load();

        db.Owners
            .Where(x => x.FirstName != null)
            .Include(x => x.Dogs) // natáhne všechny položky a ne vždy je to žádoucí, když jich bude veliké množství
            .Include(x => x.Dogs.Take(2)) // podmínený include, může tam být třeba i Where
            .Where(x => EF.Functions.Random() == 10) // EF.Functions má v sobě hafo metod, které umí SQL, ale nemá je LINQ a lze je využít jak je zde naznačené
            .Where(x => MyContext.Foo() == 10) // Moje vlastní funkce definovaná v kontextu (může mít i vstupní parametry >> např: Foo(15))
            .Select(x => new { x.FirstName, x.LastName }) // tzv. projekce (a to do anonymního objektu)
                                                          //.AsNoTracking() // EF core zahodí vazbu na entitu a nemohu jí už jednoduše updatovat. Šteří systémové prostředy, když slouží jen ke čtení.
            .AsNoTrackingWithIdentityResolution() // když bych vyčetl jednu entitu 2x tak se bude jednat vždy o rozdílné instance, tento příkaz tomu zabrání a bude dělat se jednat vždy o stejnou instanci
            .Load();

        db.Owners
            .AsSplitQuery() // rozdělí na dva dotazy. Nejprve načtě vlastníky a pak načte teprve psi z danou podmínkou (použije inner join namísto left join)
            .Include(x => x.Dogs)
            .Where(x => x.Id > 0)
            .ToList();

        // Tohle nedělá lookup (nepočítá hash a nehledá v cache), je to rychlejší než klasická konstrukce query a používá se když je velké množství stejných dotazů
        var query = EF.CompileQuery<MyContext, int, Owner[]>((context, count) => context.Owners.OrderBy(x => x.FirstName).Take(count).ToArray());
        var data = query(db, 10);

        // lazyloading
        var dogs = db.Dogs.ToList();
        foreach (var item in dogs)
        {
            if (true) // nějaká podmínka (třeba zda má pes zaplaceno)
            {
                db.Entry(item).Reference(x => x.Owner).Load(); //.IsLoaded
            }
        }

        // metoda Add
        var dog = new Dog { Name = "Alik", DateOfBirth = DateTimeOffset.UtcNow, OwnerId = 1 };
        db.Add(dog);
        db.SaveChanges();

        // mazání
        db.Remove(dog);
        db.SaveChanges();

        // dávkové mazání
        db.Dogs.Where(x => x.Id < 0).ExecuteDelete(); // bacha, když se nedá where, smaže to všechno!!

        // update
        dog = db.Dogs.OrderBy(x => x.Id).Last();
        dog.DateOfBirth = DateTimeOffset.UtcNow;
        db.SaveChanges();
    }
}