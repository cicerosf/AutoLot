using System;
using System.Collections.Generic;
using AutoLot.Models.Entities;
using AutoLot.Dal.Repositories.Base;

namespace AutoLot.Dal.Repositories.Interfaces
{
    public interface ICarRepository : IRepository<Car>
    {
        IEnumerable<Car> GetAllBy(int makeId);
        string GetPetName(int Id);
    }
}
