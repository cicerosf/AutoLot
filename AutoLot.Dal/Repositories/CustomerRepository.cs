using System.Collections.Generic;
using System.Linq;
using AutoLot.Dal.EfStructures;
using AutoLot.Models.Entities;
using AutoLot.Dal.Repositories.Base;
using AutoLot.Dal.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AutoLot.Dal.Repositories
{
    public class CustomerRepository : BaseRepository<Customer>, ICustomerRepository
    {
        public CustomerRepository(ApplicationDbContext context) : base(context)
        {
        }

        public CustomerRepository(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public override IEnumerable<Customer> GetAll()
        {
            return Table.Include(c => c.Orders).OrderBy(o => o.PersonalInformation.LastName);
        }
    }
}
