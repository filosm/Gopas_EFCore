using System.ComponentModel.DataAnnotations;
using System;

namespace Gopas.Entities;

public class Dog
{
    public int Id { get; set; }
    public int? OwnerId { get; set; } // Nemusí být, ale je lepší pro lepší práci s Dogs

    [MaxLength(100)] // ne vše lze v atributech opsat, ale jsou vidět hned v definici objektu
    public string Name { get; set; }
    public DateTimeOffset DateOfBirth { get; set; }
    public Owner Owner { get; set; }
}