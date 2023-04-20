using System.Collections.Generic;

namespace Gopas.Entities;

// TODO: Tohle jsem nestihl opsat, je třeba pak upravit. Podle lektora až nasdílí svoje zdrojáky
public class Product
{
    public int Id { get; set; }
    public string Name { get; set; }
    public ICollection<Product2Tag> Links { get; set; }
    public ICollection<Tag> Tags { get; set; }
}

public class Product2Tag
{
    public int Id { get; set; }
    public Product Product { get; set; }
    public string Name { get; set; }
    public ICollection<Product> Products { get; set; }
}