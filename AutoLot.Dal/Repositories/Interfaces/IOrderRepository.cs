using System;
using System.Collections.Generic;
using System.Linq;
using AutoLot.Models.Entities;
using AutoLot.Dal.Repositories.Base;
using AutoLot.Models.ViewModels;


namespace AutoLot.Dal.Repositories.Interfaces
{
    public interface IOrderRepository : IRepository<Order>
    {
        IQueryable<CustomerOrderViewModel> GetOrdersViewModels();
    }
}
