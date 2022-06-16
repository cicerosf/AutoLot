using AutoLot.Dal.EfStructures;
using AutoLot.Models.Entities;
using AutoLot.Dal.Repositories.Base;
using AutoLot.Dal.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using AutoLot.Models.ViewModels;

namespace AutoLot.Dal.Repositories
{
    public class OrderRepository : BaseRepository<Order>, IOrderRepository
    {
        public OrderRepository(ApplicationDbContext context) : base(context)
        {
        }

        internal OrderRepository(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public IQueryable<CustomerOrderViewModel> GetOrdersViewModels()
        {
            return dbContext.CustomerOrderViewModel!.AsQueryable();
        }
    }
}
