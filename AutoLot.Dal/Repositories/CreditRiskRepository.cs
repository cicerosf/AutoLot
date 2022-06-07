using AutoLot.Dal.EfStructures;
using AutoLot.Models.Entities;
using AutoLot.Dal.Repositories.Base;
using AutoLot.Dal.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace AutoLot.Dal.Repositories
{
    public class CreditRiskRepository : BaseRepository<CreditRisk>, ICreditRiskRepository
    {
        public CreditRiskRepository(ApplicationDbContext context) : base(context) { }

        public CreditRiskRepository(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    }
}
