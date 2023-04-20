using Gopas.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gopas.Configurations;

// Pro Table spliting
// tzn: že entita OwnerLite nemá vlastní tabulku, ale je to výcuc z tabulky Owner
public class OwnerLiteConfiguration : IEntityTypeConfiguration<OwnerLite>
{
    public void Configure(EntityTypeBuilder<OwnerLite> builder)
    {
        builder.ToTable("T_Owner");
        builder.HasOne(x => x.OwnerWithDetails) // zéroveň obsahuje vazbu na obecnou nentitu se všemi sloupci
            .WithOne()
            .HasForeignKey<Owner>(o => o.Id);

        builder.Navigation(o => o.OwnerWithDetails).IsRequired();
        builder.Property(x => x.Id).HasColumnName("Id");
        builder.Property(x => x.FirstName).HasColumnName("FirstName");
        builder.Property(x => x.LastName).HasColumnName("LastName");
    }
}