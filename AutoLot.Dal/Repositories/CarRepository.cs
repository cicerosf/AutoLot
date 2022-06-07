using System.Collections.Generic;
using System.Data;
using System.Linq;
using AutoLot.Dal.EfStructures;
using AutoLot.Models.Entities;
using AutoLot.Dal.Repositories.Base;
using AutoLot.Dal.Repositories.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace AutoLot.Dal.Repositories
{
    public class CarRepository : BaseRepository<Car>, ICarRepository
    {
        public CarRepository(ApplicationDbContext context) : base(context) { }

        public CarRepository(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public override IEnumerable<Car> GetAll()
        {
            return Table.Include(c => c.MakeNavigation).OrderBy(c => c.PetName);
        }

        public override IEnumerable<Car> GetAllIgnoreQueryFilters()
        {
            return Table.Include(c => c.MakeNavigation)
                        .OrderBy(c => c.PetName)
                        .IgnoreQueryFilters();
        }

        public IEnumerable<Car> GetAllBy(int makeId)
        {
            return Table.Where(c => c.MakeId == makeId)
                        .Include(c => c.MakeNavigation)
                        .OrderBy(c => c.PetName);
        }

        public override Car? Find(int? id)
        {
            return Table.IgnoreQueryFilters()
                        .Where(c => c.Id == id)
                        .Include(c => c.MakeNavigation)
                        .FirstOrDefault();
        }

        public string GetPetName(int Id)
        {
            var parameterId = new SqlParameter
            {
                ParameterName = "@cardId",
                SqlDbType = SqlDbType.Int,
                Value = Id
            };

            var parameterName = new SqlParameter
            {
                ParameterName = "@petName",
                SqlDbType=SqlDbType.NVarChar,
                Size = 50,
                Direction = ParameterDirection.Output
            };
            
            _ = dbContext.Database.ExecuteSqlRaw
                    ("EXEC [dbo].[GetPetName] @carId, @petName OUTPUT", parameterId, parameterName);

            return (string)parameterName.Value;

        }
    }
}
