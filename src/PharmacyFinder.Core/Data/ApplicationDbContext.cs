using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PharmacyFinder.Core.Models;
using System.Reflection;

namespace PharmacyFinder.Infrastructure.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Pharmacy> Pharmacies { get; set; }
        public DbSet<Medicine> Medicines { get; set; }
        public DbSet<MedicineStock> MedicineStocks { get; set; }
        public DbSet<OperatingHour> OperatingHours { get; set; }
        public DbSet<Prescription> Prescriptions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            ConfigureEntities(modelBuilder);
            SeedData(modelBuilder);
        }

        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            configurationBuilder.Properties<decimal>().HavePrecision(18, 2);
            configurationBuilder.Properties<decimal?>().HavePrecision(18, 2);
        }

        private void ConfigureEntities(ModelBuilder modelBuilder)
        {
            // ApplicationUser configuration
            modelBuilder.Entity<ApplicationUser>(entity =>
            {
                entity.HasKey(u => u.Id);
                
                // One-to-one: User to Pharmacy (for pharmacy owners)
                entity.HasOne(u => u.Pharmacy)
                      .WithOne(p => p.Owner)
                      .HasForeignKey<Pharmacy>(p => p.OwnerId)
                      .OnDelete(DeleteBehavior.Cascade);
                
                // One-to-many: User to Prescriptions (for customers)
                entity.HasMany(u => u.Prescriptions)
                      .WithOne(p => p.Customer)
                      .HasForeignKey(p => p.CustomerId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Pharmacy configuration
            modelBuilder.Entity<Pharmacy>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.HasIndex(p => p.LicenseNumber).IsUnique();
                entity.HasIndex(p => new { p.Latitude, p.Longitude });
                
                entity.Property(p => p.RegisteredAt).HasDefaultValueSql("GETUTCDATE()");
                
                // One-to-many: Pharmacy to OperatingHours
                entity.HasMany(p => p.OperatingHours)
                      .WithOne(oh => oh.Pharmacy)
                      .HasForeignKey(oh => oh.PharmacyId)
                      .OnDelete(DeleteBehavior.Cascade);
                
                // One-to-many: Pharmacy to MedicineStocks
                entity.HasMany(p => p.MedicineStocks)
                      .WithOne(ms => ms.Pharmacy)
                      .HasForeignKey(ms => ms.PharmacyId)
                      .OnDelete(DeleteBehavior.Cascade);
                
                // One-to-many: Pharmacy to Prescriptions
                entity.HasMany(p => p.Prescriptions)
                      .WithOne(pr => pr.Pharmacy)
                      .HasForeignKey(pr => pr.PharmacyId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // Medicine configuration
            modelBuilder.Entity<Medicine>(entity =>
            {
                entity.HasKey(m => m.Id);
                entity.HasIndex(m => m.Name);
                entity.HasIndex(m => m.GenericName);
                
                entity.Property(m => m.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
                
                // One-to-many: Medicine to MedicineStocks
                entity.HasMany(m => m.MedicineStocks)
                      .WithOne(ms => ms.Medicine)
                      .HasForeignKey(ms => ms.MedicineId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // ✅ FIXED: MedicineStock configuration with unique constraint
            modelBuilder.Entity<MedicineStock>(entity =>
            {
                entity.HasKey(ms => ms.Id);
                
                // Unique constraint to prevent duplicate pharmacy-medicine combinations
                entity.HasIndex(ms => new { ms.PharmacyId, ms.MedicineId }).IsUnique();
                
                entity.Property(ms => ms.LastUpdated).HasDefaultValueSql("GETUTCDATE()");
                
                // Check constraints
                entity.HasCheckConstraint("CK_MedicineStock_Quantity", "[Quantity] >= 0");
                entity.HasCheckConstraint("CK_MedicineStock_Price", "[Price] > 0");
            });

            // OperatingHour configuration
            modelBuilder.Entity<OperatingHour>(entity =>
            {
                entity.HasKey(oh => oh.Id);
                
                // Unique constraint for pharmacy day combination
                entity.HasIndex(oh => new { oh.PharmacyId, oh.DayOfWeek }).IsUnique();
            });

            // Prescription configuration
            modelBuilder.Entity<Prescription>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.HasIndex(p => p.Status);
                entity.HasIndex(p => p.UploadedAt);
                
                entity.Property(p => p.UploadedAt).HasDefaultValueSql("GETUTCDATE()");
            });
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            // Seed initial medicines
            modelBuilder.Entity<Medicine>().HasData(
                new Medicine 
                { 
                    Id = 1, 
                    Name = "Paracetamol", 
                    GenericName = "Acetaminophen", 
                    Manufacturer = "Generic Pharma",
                    Form = MedicineForm.Tablet,
                    Strength = "500",
                    Unit = "mg",
                    RequiresPrescription = false,
                    Description = "Pain reliever and fever reducer"
                },
                new Medicine 
                { 
                    Id = 2, 
                    Name = "Ibuprofen", 
                    GenericName = "Ibuprofen", 
                    Manufacturer = "Generic Pharma",
                    Form = MedicineForm.Tablet,
                    Strength = "400",
                    Unit = "mg",
                    RequiresPrescription = false,
                    Description = "Nonsteroidal anti-inflammatory drug"
                },
                new Medicine 
                { 
                    Id = 3, 
                    Name = "Amoxicillin", 
                    GenericName = "Amoxicillin", 
                    Manufacturer = "Generic Pharma",
                    Form = MedicineForm.Capsule,
                    Strength = "500",
                    Unit = "mg",
                    RequiresPrescription = true,
                    Description = "Antibiotic used to treat bacterial infections"
                },
                new Medicine 
                { 
                    Id = 4, 
                    Name = "Ventolin Inhaler", 
                    GenericName = "Salbutamol", 
                    Manufacturer = "Respiratory Care",
                    Form = MedicineForm.Inhaler,
                    Strength = "100",
                    Unit = "mcg",
                    RequiresPrescription = true,
                    Description = "Bronchodilator for asthma relief"
                }
            );
        }
    }
}