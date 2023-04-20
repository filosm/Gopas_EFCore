using System.Collections.Generic;

namespace Gopas.Entities;

public class Tag
{
    public int Id { get; set; }
    public string Name { get; set; }
    public ICollection<Product> Products { get; set; }
    public ICollection<Product2Tag> Links { get; set; }
}