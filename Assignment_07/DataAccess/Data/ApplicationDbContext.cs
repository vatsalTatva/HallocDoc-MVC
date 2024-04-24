using System;
using System.Collections.Generic;
using DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Data;

public partial class ApplicationDbContext : DbContext
{
    public ApplicationDbContext()
    {
    }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Book> Books { get; set; }

    public virtual DbSet<Borrower> Borrowers { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("User ID = postgres;Password=vats123;Server=localhost;Port=5432;Database=LibraryManagementSystem_db;Integrated Security=true;Pooling=true;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Book>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("pk_book");

            entity.ToTable("book");

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.Author)
                .HasMaxLength(128)
                .HasColumnName("author");
            entity.Property(e => e.Bookname)
                .HasMaxLength(128)
                .HasColumnName("bookname");
            entity.Property(e => e.Borrowerid).HasColumnName("borrowerid");
            entity.Property(e => e.Borrowername)
                .HasMaxLength(128)
                .HasColumnName("borrowername");
            entity.Property(e => e.City)
                .HasMaxLength(128)
                .HasColumnName("city");
            entity.Property(e => e.Dateofissue)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("dateofissue");
            entity.Property(e => e.Genere)
                .HasMaxLength(128)
                .HasColumnName("genere");

            entity.HasOne(d => d.Borrower).WithMany(p => p.Books)
                .HasForeignKey(d => d.Borrowerid)
                .HasConstraintName("fk_book");
        });

        modelBuilder.Entity<Borrower>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("pk_borrower");

            entity.ToTable("borrower");

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.City)
                .HasMaxLength(128)
                .HasColumnName("city");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
