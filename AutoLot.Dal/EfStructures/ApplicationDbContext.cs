using System;
using System.Collections;
using System.Collections.Generic;
using AutoLot.Models.Entities;
using AutoLot.Models.Entities.Owned;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using AutoLot.Dal.Exceptions;
using AutoLot.Models.ViewModels;

namespace AutoLot.Dal.EfStructures
{
    public partial class ApplicationDbContext : DbContext
    {
        public DbSet<SeriLogEntry>? LogEntries { get; set; }
        public DbSet<CreditRisk>? CreditRisks { get; set; }
        public DbSet<Customer>? Customers { get; set; }
        public DbSet<Car>? Cars { get; set; }
        public DbSet<Make>? Makes { get; set; }
        public DbSet<Order>? Orders { get; set; }
        public DbSet<CustomerOrderViewModel> CustomerOrderViewModel { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            ChangeTracker.Tracked += ChangeTracker_Tracked;
            ChangeTracker.StateChanged += ChangeTracker_StateChanged;

            base.SavingChanges += (sender, args) =>
            {
                Console.WriteLine($"Saving changes for {((ApplicationDbContext)sender)!.Database!.GetDbConnection()}");
            };

            base.SavedChanges += (sender, args) =>
            {
                Console.WriteLine($"Saved {args!.EntitiesSavedCount} changes for" +
                    $"{((ApplicationDbContext)sender)!.Database!.GetConnectionString()}");
            };

            base.SaveChangesFailed += (sender, args) =>
            {
                Console.WriteLine($"An exception occurred! {args.Exception.Message} entities");
            };
        }

        private void ChangeTracker_StateChanged(object? sender, EntityStateChangedEventArgs e)
        {
            if (e.Entry.Entity is not Car c)
            {
                return;
            }

            var action = string.Empty;

            Console.WriteLine($"Car {c.PetName} state was {e.OldState} before the state changed to {e.NewState}");

            switch (e.NewState)
            {
                case EntityState.Detached:
                    break;
                case EntityState.Unchanged:
                    action = e.OldState switch
                    {
                        EntityState.Added => "Added",
                        EntityState.Modified => "Edited",
                        _ => action
                    };
                    Console.WriteLine($"The object was {action}.");
                    break;
                case EntityState.Deleted:
                    break;
                case EntityState.Modified:
                    break;
                case EntityState.Added:
                    break;
                default:
                    break;
            }
        }

        private void ChangeTracker_Tracked(object? sender, EntityTrackedEventArgs e)
        {
            var source = e.FromQuery ? "Database" : "Code";
            if (e.Entry.Entity is Car c)
            {
                Console.WriteLine($"Car entry {c.PetName} was added from {source}");
            }
        }

        public override int SaveChanges()
        {
            try
            {
                return base.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                throw new CustomConcurrencyException("A concurrency erros happened.", ex);
            }
            catch (RetryLimitExceededException ex)
            {
                throw new CustomRetryLimitExceededException("There is a problema with SqlServer", ex);
            }
            catch (DbUpdateException ex)
            {
                throw new CustomDbUpdateException("An error occurred while updating the database", ex);
            }
            catch (Exception ex)
            {
                throw new CustomException("An erros occurred while updating the database.", ex);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "Latin1_General_CI_AS");

            modelBuilder.Entity<SeriLogEntry>(entiy =>
            {
                entiy.Property(e => e.Properties).HasColumnType("Xml");
                entiy.Property(e => e.TimeStamp).HasDefaultValueSql("GetDate()");
            });

            modelBuilder.Entity<CreditRisk>(entity =>
            {
                entity.HasOne(d => d.CustomerNavigation)
                    .WithMany(p => p!.CreditRisks)
                    .HasForeignKey(d => d.CustomerId)
                    .HasConstraintName("FK_CreditRisks_Customers");

                entity.OwnsOne(o => o.PersonalInformation,
                    pd =>
                    {
                        pd.Property<string>(nameof(Person.FirstName)).HasColumnName(nameof(Person.FirstName))
                            .HasColumnType("VarChar(50)");
                        pd.Property<string>(nameof(Person.LastName)).HasColumnName(nameof(Person.LastName))
                            .HasColumnType("VarChar(50)");
                        pd.Property(p => p.FullName).HasColumnName(nameof(Person.FullName))
                            .HasComputedColumnSql("[LastName] + ', ' + [FirstName]");
                    });

                
            });

            modelBuilder.Entity<Customer>(entity =>
            {
                entity.OwnsOne(o => o.PersonalInformation,
                    pd =>
                    {
                        pd.Property(p => p.FirstName).HasColumnName(nameof(Person.FirstName));
                        pd.Property(p => p.LastName).HasColumnName(nameof(Person.LastName));
                        pd.Property(p => p.FullName).HasColumnName(nameof(Person.FullName)).
                                    HasComputedColumnSql("[LastName] + ', ' + [FirstName]");
                    });
            });

            modelBuilder.Entity<Make>(entity =>
            {
                entity.HasMany(e => e.Cars)
                      .WithOne(c => c.MakeNavigation!)
                      .HasForeignKey(k => k.MakeId)
                      .OnDelete(DeleteBehavior.Restrict)
                      .HasConstraintName("FK_Make_Inventory");
            });

            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasOne(d => d.CarNavigation)
                    .WithMany(p => p!.Orders)
                    .HasForeignKey(d => d.CarId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Orders_Inventory");

                entity.HasOne(d => d.CustomerNavigation)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.CustomerId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_Orders_Customers");
            });

            modelBuilder.Entity<Order>().HasQueryFilter(e => e.CarNavigation.IsDrivable);

            modelBuilder.Entity<Car>(entity =>
            {
                entity.HasQueryFilter(c => c.IsDrivable);
                entity.Property(p => p.IsDrivable).HasField("_isDrivable").HasDefaultValue(true);

                entity.HasOne(d => d.MakeNavigation)
                      .WithMany(p => p.Cars)
                      .HasForeignKey(d => d.MakeId)
                      .OnDelete(DeleteBehavior.ClientSetNull)
                      .HasConstraintName("FK_Make_Inventory");
            });

            modelBuilder.Entity<CustomerOrderViewModel>(entity =>
            {
                entity.HasNoKey().ToView("CustomerOrderView", "dbo");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
