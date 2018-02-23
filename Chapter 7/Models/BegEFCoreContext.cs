using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace EquineTracker.Models {
    public partial class BegEFCoreContext : DbContext {
        public virtual DbSet<Event> Event { get; set; }
        public virtual DbSet<Horse> Horse { get; set; }
        public virtual DbSet<Location> Location { get; set; }
        public virtual DbSet<Result> Result { get; set; }

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    if (!optionsBuilder.IsConfigured)
        //    {
        //        #warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
        //        optionsBuilder.UseSqlServer(@"Server=localhost;Database=BegEFCore;User ID=sa;Password=password;");
        //    }
        //}

        public BegEFCoreContext(DbContextOptions<BegEFCoreContext> options) : base(options) {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            modelBuilder.Entity<Event>(entity => {
                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.EventDate).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.HasOne(d => d.Location)
                    .WithMany(p => p.Event)
                    .HasForeignKey(d => d.LocationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Event_Location");
            });

            modelBuilder.Entity<Horse>(entity => {
                entity.Property(e => e.Breed).HasMaxLength(50);

                entity.Property(e => e.Height).HasColumnType("decimal(3, 1)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Value).HasColumnType("decimal(18, 0)");
            });

            modelBuilder.Entity<Location>(entity => {
                entity.Property(e => e.City).HasMaxLength(50);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.State)
                    .HasMaxLength(2)
                    .IsUnicode(false);

                entity.Property(e => e.StreetAddress).HasMaxLength(50);

                entity.Property(e => e.ZipCode)
                    .HasMaxLength(10)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Result>(entity => {
                entity.Property(e => e.Class)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Score).HasColumnType("decimal(18, 4)");

                entity.HasOne(d => d.Event)
                    .WithMany(p => p.Result)
                    .HasForeignKey(d => d.EventId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Result_Event");

                entity.HasOne(d => d.Horse)
                    .WithMany(p => p.Result)
                    .HasForeignKey(d => d.HorseId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Result_Horse");
            });
        }
    }
}