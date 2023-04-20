using System.Collections.Generic;

namespace Gopas.Entities;

public class OwnerLite
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public ICollection<Dog> Dogs { get; set; }
    public Owner OwnerWithDetails { get; set; } // obsahuje vazbu na obecnou nentitu se všemi sloupci
}