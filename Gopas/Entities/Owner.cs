using System.Collections.Generic;

namespace Gopas.Entities;

public class Owner
{
    public int Id { get; set; }
    private string _firstName;
    public string FirstName { get => _firstName; set => _firstName = value; } // příklad pro UsePropertyAccessMode
    public string LastName { get; set; }
    public Address ContactAddress { get; set; }
    public Address ShippingAddress { get; set; }
    public Address BillingAddress { get; set; }
    public ICollection<Dog> Dogs { get; set; }
    public bool IsActive { get; set; }
    public Duration Duration { get; set; }

    // viz. Souběžnost
    public byte[] Version { get; set; }
}