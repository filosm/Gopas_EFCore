using Gopas.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Linq;

namespace Gopas;

internal class TretiDen
{
    public static void Main()
    {
        using var db = new MyContext();
        db.Database.EnsureDeleted();
        db.Database.EnsureCreated();

        // Transakce
        //using (var tx = db.Database.BeginTransaction(System.Data.IsolationLevel.Snapshot))
        //{
        //    db.Owners.Add(new Owner { FirstName = "Test", LastName = "Test" });            

        //    db.SaveChanges(false); // < je to kvůli tomu, když by mi vylítla nějaká EX tak jí mohu zpracovat a nezmizí z ChangeTrackeru
        //    tx.Commit();
        //    db.ChangeTracker.AcceptAllChanges(); // Až zde akceptujeme změny a zmizí z ChangeTrackeru
        //};

        //using (var connection = db.Database.GetDbConnection())
        //{
        //    var sqlConn = (SqlConnection) connection;
        //    using(var tx = sqlConn.BeginTransaction("Test"))
        //    {
        //        db.Database.UseTransaction(tx);
        //        db.Set<Owner>().Load();
        //    }

        //    // nestihl jsem dopsat
        //}

        //db.Owners.Add(new Owner { FirstName = "Test", LastName = "Test" });
        
        // Soubežnost - řešení kolizí        
        var item = db.Owners.First();
        item.FirstName += "_01";
        // v tento okamžik provede změnu na entitě nějaký jiný proces
        SaveChangesWithColisionSolving(db);
    }

    // Soubežnost - řešení kolizí
    // Toto by asi bylo někde v repositáři, kde by se prováděl update
    private static int SaveChangesWithColisionSolving(MyContext db)
    {
        const int tryCount = 10;
        for (int i = 0; i < tryCount; i++) // 5 opakování jen moje invence, klidně while
        {
            try
            {
                return db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                foreach (var item in ex.Entries)
                {
                    // Typ: Store wins
                    //StoreWinsType(item);

                    // Typ: Client wins
                    ClientWinsType(item);
                }
                if (i >= tryCount)
                {
                    throw;
                }
            }
        }
        
        return 0;
    }

    private static void StoreWinsType(EntityEntry entity)
    {        
        entity.Reload(); // načte se položka znovu z DB
    }

    private static void ClientWinsType(EntityEntry entity)
    {
        entity.OriginalValues.SetValues(entity.GetDatabaseValues());
    }
}