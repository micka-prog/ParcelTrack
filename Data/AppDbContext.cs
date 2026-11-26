using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ParcelTrack.Models;

namespace ParcelTrack.Data
{
    // The app's door to the database. IdentityDbContext already has the user and role tables.
    public class AppDbContext : IdentityDbContext<IdentityUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Parcel> Parcels { get; set; }

        public DbSet<Branch> Branches { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Identity sets up its own tables here
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Branch>().HasData(
                new Branch { Id = 1, Name = "Half Way Tree", Parish = "St Andrew" },
                new Branch { Id = 2, Name = "Downtown Kingston", Parish = "Kingston" },
                new Branch { Id = 3, Name = "Spanish Town", Parish = "St Catherine" },
                new Branch { Id = 4, Name = "Mandeville", Parish = "Manchester" },
                new Branch { Id = 5, Name = "Montego Bay", Parish = "St James" }
            );

            modelBuilder.Entity<Parcel>().HasData(
                new Parcel { Id = 1, TrackingCode = "PT-100101", Recipient = "Andre Campbell", WeightKg = 1.2, Status = "In transit", BranchId = 1 },
                new Parcel { Id = 2, TrackingCode = "PT-100102", Recipient = "Keisha Brown", WeightKg = 0.5, Status = "Ready for pickup", BranchId = 2 },
                new Parcel { Id = 3, TrackingCode = "PT-100103", Recipient = "Marlon Williams", WeightKg = 3.0, Status = "Received", BranchId = 5 },
                new Parcel { Id = 4, TrackingCode = "PT-100104", Recipient = "Tanya Reid", WeightKg = 7.4, Status = "In transit", BranchId = 4 },
                new Parcel { Id = 5, TrackingCode = "PT-100105", Recipient = "Shanice Gordon", WeightKg = 2.5, Status = "Delivered", BranchId = 3 }
            );

            // No two parcels with the same tracking code
            modelBuilder.Entity<Parcel>()
                .HasIndex(parcel => parcel.TrackingCode)
                .IsUnique();
        }
    }
}
