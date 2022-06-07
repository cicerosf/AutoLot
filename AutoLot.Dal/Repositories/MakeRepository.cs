using System.Collections.Generic;
using System.Linq;
using AutoLot.Dal.EfStructures;
using AutoLot.Models.Entities;
using AutoLot.Dal.Repositories.Base;
using AutoLot.Dal.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AutoLot.Dal.Repositories
{
    public class MakeRepository : BaseRepository<Make>, IMakeRepository
    {
        public MakeRepository(ApplicationDbContext context) : base(context)
        {
        }

        public MakeRepository(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public override IEnumerable<Make> GetAll()
        {
            return Table.OrderBy(o => o.Name);
        }

        public override IEnumerable<Make> GetAllIgnoreQueryFilters()
        {
            return Table.IgnoreQueryFilters().OrderBy(o => o.Name);
        }
    }
}
