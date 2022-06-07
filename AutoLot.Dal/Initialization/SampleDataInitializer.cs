using System;
using System.Collections.Generic;
using System.Linq;
using AutoLot.Dal.EfStructures;
using AutoLot.Models.Entities;
using AutoLot.Models.Entities.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace AutoLot.Dal.Initialization
{
    public static class SampleDataInitializer
    {
        public static void DropAndCreateDatabase(ApplicationDbContext dbContext)
        {
            dbContext.Database.EnsureDeleted();
            dbContext.Database.Migrate();
        }

        public static void InitializeData(ApplicationDbContext dbContext)
        {
            DropAndCreateDatabase(dbContext);
            SeedData(dbContext);
        }

        public static void ClearAndReseedDatabase(ApplicationDbContext context)
        {
            ClearData(context);
            SeedData(context);
        }

        internal static void ClearData(ApplicationDbContext dbContext)
        {
            var entities = new[]
            {
                typeof(Order).FullName,
                typeof(Customer).FullName,
                typeof(Car).FullName,
                typeof(Make).FullName,
                typeof(CreditRisk).FullName
            };

            foreach (var entity in entities)
            {
                var entityName = dbContext.Model.FindEntityType(entity);
                var tableName = entityName.GetTableName();
                var schemaName = entityName.GetSchema();

                dbContext.Database.ExecuteSqlRaw($"DELETE FROM {schemaName}.{tableName}");
                dbContext.Database.ExecuteSqlRaw
                    ($"DBCC CHECKIDENT (\"{schemaName}.{tableName}\", RESEED, 1); ");
            }
        }

        internal static void SeedData(ApplicationDbContext dbContext)
        {
            try
            {
                ProcessInsert(dbContext, dbContext.Customers!, SampleData.Customers);
                ProcessInsert(dbContext, dbContext.Makes!, SampleData.Makes);
                ProcessInsert(dbContext, dbContext.Cars!, SampleData.Inventory);
                ProcessInsert(dbContext, dbContext.Orders!, SampleData.Orders);
                ProcessInsert(dbContext, dbContext.CreditRisks!, SampleData.CreditRisks);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        private static void ProcessInsert<T>(ApplicationDbContext dbContext,
                                             DbSet<T> table,
                                             List<T> records) where T : BaseEntity
        {
            if (table.Any())
            {
                return;
            }

            IExecutionStrategy strategy = dbContext.Database.CreateExecutionStrategy();
            strategy.Execute(() =>
            {
                using var transaction = dbContext.Database.BeginTransaction();
                try
                {
                    var metaData = dbContext.Model.FindEntityType(typeof(T).FullName);
                    dbContext.Database.ExecuteSqlRaw(
                        $"SET IDENTITY_INSERT {metaData.GetSchema()}.{metaData.GetTableName()} ON");

                    table.AddRange(records);
                    dbContext.SaveChanges();
                    dbContext.Database.ExecuteSqlRaw(
                        $"SET IDENTITY_INSERT {metaData.GetSchema()}.{metaData.GetTableName()} OFF");
                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                }
            });
        }
    }
}
