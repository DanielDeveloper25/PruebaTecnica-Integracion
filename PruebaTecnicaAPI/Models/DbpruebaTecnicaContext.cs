using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace PruebaTecnicaAPI.Models;

public partial class DbpruebaTecnicaContext : DbContext
{
    public DbpruebaTecnicaContext()
    {
    }

    public DbpruebaTecnicaContext(DbContextOptions<DbpruebaTecnicaContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Phone> Phones { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder){}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Phone>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Phones__3214EC07C2C6386E");

            entity.Property(e => e.CityCode).HasMaxLength(10);
            entity.Property(e => e.CountryCode).HasMaxLength(10);
            entity.Property(e => e.Number).HasMaxLength(20);

            entity.HasOne(d => d.User).WithMany(p => p.Phones)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_Phones_Users");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Users__3214EC071D033CE0");

            entity.HasIndex(e => e.Email, "UQ__Users__A9D10534BA23FC4B").IsUnique();

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.Created)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Email).HasMaxLength(150);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.LastLogin)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Modified)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Password).HasMaxLength(255);
            entity.Property(e => e.Token).HasMaxLength(500);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
