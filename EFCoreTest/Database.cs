// SPDX-License-Identifier: LicenseRef-arfinityProprietary
// SPDX-FileCopyrightText: © 2022 arfinity GmbH <contact@arfinity.io>

using Microsoft.EntityFrameworkCore;
using Vogen;

namespace EFCoreTest;

[ValueObject<string>(Conversions.EfCoreValueConverter)]
public partial class TestValueObject{}

public class TestEntity
{
    public int Id { get; set; } 
    public List<TestOwned> OwnedInternal { get; set; } = new List<TestOwned>();
}

public record TestOwned(string Value, string ValueTwo)
{
    public TestOwned() : this("", "") {}
}

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TestEntity>().HasKey(x => x.Id);
        modelBuilder.Entity<TestEntity>().OwnsMany<TestOwned>(x => x.OwnedInternal, navigationBuilder =>
        {
            navigationBuilder.ToTable("OwnedTable");
            navigationBuilder.Property(x => x.Value).HasColumnName("Value");
            navigationBuilder.Property(x => x.ValueTwo).HasColumnName("ValueTwo");
            navigationBuilder.WithOwner().HasForeignKey("OwnerKey");
            navigationBuilder.HasKey("Value", "ValueTwo", "OwnerKey");
        });
    }
}