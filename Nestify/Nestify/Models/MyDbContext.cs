using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Nestify.Models;

public partial class MyDbContext : DbContext
{
    public MyDbContext()
    {
    }

    public MyDbContext(DbContextOptions<MyDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<ContactInquiry> ContactInquiries { get; set; }

    public virtual DbSet<FavoriteProperty> FavoriteProperties { get; set; }

    public virtual DbSet<InteriorDesignInquiry> InteriorDesignInquiries { get; set; }

    public virtual DbSet<Location> Locations { get; set; }

    public virtual DbSet<Package> Packages { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<Property> Properties { get; set; }

    public virtual DbSet<PropertyFeature> PropertyFeatures { get; set; }

    public virtual DbSet<PropertyInquiry> PropertyInquiries { get; set; }

    public virtual DbSet<Sublocation> Sublocations { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=DESKTOP-5RUPPFV;Database=Nestify;Trusted_Connection=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ContactInquiry>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ContactI__3214EC270F2CD4C2");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Email).HasMaxLength(320);
            entity.Property(e => e.Name).HasMaxLength(255);
            entity.Property(e => e.PhoneNumber).HasMaxLength(20);
            entity.Property(e => e.SubmittedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
        });

        modelBuilder.Entity<FavoriteProperty>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Favorite__3214EC276FC57C73");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.PropertyId).HasColumnName("PropertyID");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Property).WithMany(p => p.FavoriteProperties)
                .HasForeignKey(d => d.PropertyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__FavoriteP__Prope__6EF57B66");

            entity.HasOne(d => d.User).WithMany(p => p.FavoriteProperties)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__FavoriteP__UserI__6E01572D");
        });

        modelBuilder.Entity<InteriorDesignInquiry>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Interior__3214EC2757273621");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Email).HasMaxLength(320);
            entity.Property(e => e.Name).HasMaxLength(255);
            entity.Property(e => e.PhoneNumber).HasMaxLength(20);
            entity.Property(e => e.SubmittedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
        });

        modelBuilder.Entity<Location>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Location__3214EC27768A7733");

            entity.HasIndex(e => e.LocationName, "UQ__Location__F946BB84FB0C5583").IsUnique();

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.LocationName).HasMaxLength(255);
        });

        modelBuilder.Entity<Package>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Packages__3214EC2744BCF46E");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.Price).HasColumnType("decimal(10, 2)");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Payments__3214EC2723AD5C97");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Amount).HasColumnType("decimal(12, 2)");
            entity.Property(e => e.PackageId).HasColumnName("PackageID");
            entity.Property(e => e.PaymentDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.PaymentMethod).HasMaxLength(50);
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Package).WithMany(p => p.Payments)
                .HasForeignKey(d => d.PackageId)
                .HasConstraintName("FK_Payments_Packages");

            entity.HasOne(d => d.User).WithMany(p => p.Payments)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Payments__UserID__74AE54BC");
        });

        modelBuilder.Entity<Property>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Properti__3214EC27C093F382");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Bathrooms).HasDefaultValue(1);
            entity.Property(e => e.FacingDirection)
                .HasMaxLength(100)
                .HasDefaultValue("South");
            entity.Property(e => e.ImageUrl1)
                .HasMaxLength(500)
                .HasColumnName("ImageURL1");
            entity.Property(e => e.ImageUrl2)
                .HasMaxLength(500)
                .HasColumnName("ImageURL2");
            entity.Property(e => e.ImageUrl3)
                .HasMaxLength(500)
                .HasColumnName("ImageURL3");
            entity.Property(e => e.ImageUrl4)
                .HasMaxLength(500)
                .HasColumnName("ImageURL4");
            entity.Property(e => e.ImageUrl5)
                .HasMaxLength(500)
                .HasColumnName("ImageURL5");
            entity.Property(e => e.IsFeatured).HasDefaultValue(false);
            entity.Property(e => e.Latitude).HasColumnType("decimal(9, 6)");
            entity.Property(e => e.Longitude).HasColumnType("decimal(9, 6)");
            entity.Property(e => e.LotArea).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.LotDimensions).HasMaxLength(255);
            entity.Property(e => e.PaymentId).HasColumnName("PaymentID");
            entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.PriceType)
                .HasMaxLength(100)
                .HasDefaultValue("Total");
            entity.Property(e => e.PropertyName).HasMaxLength(255);
            entity.Property(e => e.PropertyStatus).HasMaxLength(20);
            entity.Property(e => e.PropertyType)
                .HasMaxLength(100)
                .HasDefaultValue("Villa");
            entity.Property(e => e.PublishDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Size).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.SubLocationId).HasColumnName("SubLocationID");
            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.VideoUrl)
                .HasMaxLength(500)
                .HasColumnName("VideoURL");

            entity.HasOne(d => d.Payment).WithMany(p => p.Properties)
                .HasForeignKey(d => d.PaymentId)
                .HasConstraintName("FK_Properties_Payments");

            entity.HasOne(d => d.SubLocation).WithMany(p => p.Properties)
                .HasForeignKey(d => d.SubLocationId)
                .HasConstraintName("FK_Properties_Sublocations");

            entity.HasOne(d => d.User).WithMany(p => p.Properties)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Propertie__UserI__5070F446");
        });

        modelBuilder.Entity<PropertyFeature>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Property__3214EC2739A3917D");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.FeatureName).HasMaxLength(255);
            entity.Property(e => e.PropertyId).HasColumnName("PropertyID");
            entity.Property(e => e.Size).HasMaxLength(50);

            entity.HasOne(d => d.Property).WithMany(p => p.PropertyFeatures)
                .HasForeignKey(d => d.PropertyId)
                .HasConstraintName("FK__PropertyF__Prope__59063A47");
        });

        modelBuilder.Entity<PropertyInquiry>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Property__3214EC27F0F7B427");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Email).HasMaxLength(320);
            entity.Property(e => e.Name).HasMaxLength(255);
            entity.Property(e => e.Phone).HasMaxLength(20);
            entity.Property(e => e.PreferredDate).HasColumnType("datetime");
            entity.Property(e => e.PropertyId).HasColumnName("PropertyID");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValue("Pending");
            entity.Property(e => e.SubmittedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Property).WithMany(p => p.PropertyInquiries)
                .HasForeignKey(d => d.PropertyId)
                .HasConstraintName("FK__PropertyI__Prope__5CD6CB2B");

            entity.HasOne(d => d.User).WithMany(p => p.PropertyInquiries)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_PropertyInquiries_Users");
        });

        modelBuilder.Entity<Sublocation>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Sublocat__3214EC27E8E343BB");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.LocationId).HasColumnName("LocationID");
            entity.Property(e => e.SublocationName).HasMaxLength(255);

            entity.HasOne(d => d.Location).WithMany(p => p.Sublocations)
                .HasForeignKey(d => d.LocationId)
                .HasConstraintName("FK__Sublocati__Locat__5629CD9C");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Users__3214EC27E5CDF043");

            entity.HasIndex(e => e.Email, "UQ__Users__A9D1053437FE8D88").IsUnique();

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.BusinessAddress).HasMaxLength(255);
            entity.Property(e => e.BusinessName).HasMaxLength(255);
            entity.Property(e => e.City).HasMaxLength(100);
            entity.Property(e => e.Country).HasMaxLength(100);
            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.FirstName).HasMaxLength(100);
            entity.Property(e => e.LastName).HasMaxLength(100);
            entity.Property(e => e.PasswordHash).HasMaxLength(512);
            entity.Property(e => e.PhoneNumber).HasMaxLength(20);
            entity.Property(e => e.ProfileImageUrl)
                .HasMaxLength(500)
                .HasColumnName("ProfileImageURL");
            entity.Property(e => e.Role).HasMaxLength(20);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
