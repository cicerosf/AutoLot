using System.Collections.Generic;
using System.Linq;
using AutoLot.Dal.Exceptions;
using AutoLot.Dal.Repositories;
using AutoLot.Tests.Base;
using AutoLot.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Storage;
using Xunit;
using AutoLot.Tests.Helpers;

namespace AutoLot.Tests.IntegrationTests
{
    [Collection("Integration Tests")]
    public class CarTests : BaseTest, IClassFixture<EnsureAutoLotDatabaseTestFixture>
    {
        [Theory]
        [InlineData(1, 2)]
        [InlineData(2, 1)]
        [InlineData(3, 1)]
        [InlineData(4, 2)]
        [InlineData(5, 3)]
        [InlineData(6, 1)]
        public void ShouldGetTheCarsByMake(int makeId, int expectedCount)
        {
            IQueryable<Car> query =
                _context.Cars.IgnoreQueryFilters().Where(x => x.MakeId == makeId);

            var qs = query.ToQueryString();
            var cars = query.ToList();
            Assert.Equal(expectedCount, cars.Count);
        }

        [Fact]
        public void ShouldReturnDrivableCarsWithQueryFilterSet()
        {
            IQueryable<Car> query = _context.Cars;
            var queryString = query.ToQueryString();
            var cars = query.ToList();

            Assert.NotEmpty(cars);

            Assert.Equal(9, cars.Count);
        }

        [Fact]
        public void ShouldGetAllOfTheCars()
        {
            IQueryable<Car> query = _context.Cars.IgnoreQueryFilters();
            var queryString = query.ToQueryString();
            var cars = query.ToList();

            Assert.Equal(10, cars.Count);
        }

        [Fact]
        public void ShouldGetAllOrdersExceptFiltered()
        {
            var query = _context.Orders.AsQueryable();
            var queryString = query.ToQueryString();
            var orders = query.ToList();

            Assert.NotEmpty(orders);

            Assert.Equal(4, orders.Count);
        }

        [Fact]
        public void ShouldGetAllOfTheCarsWithMakes()
        {
            IIncludableQueryable<Car, Make?> query =
                _context.Cars.Include(c => c.MakeNavigation);

            var queryString = query.ToQueryString();
            var cars = query.ToList();

            Assert.Equal(9, cars.Count);
        }

        [Fact]
        public void ShouldGetCarsOnOrderWithRelatedProperties()
        {
            IIncludableQueryable<Car, Customer?> query = _context.Cars
                .Where(c => c.Orders.Any())
                .Include(c => c.MakeNavigation)
                .Include(c => c.Orders).ThenInclude(o => o.CustomerNavigation);
            
            var queryString = query.ToQueryString();
            var cars = query.ToList();
            
            Assert.Equal(4, cars.Count);
            
            cars.ForEach(c =>
            {
                Assert.NotNull(c.MakeNavigation);
                Assert.NotNull(c.Orders.ToList()[0].CustomerNavigation);
            });
        }

        [Fact]
        public void ShouldGetCarsOnOrderWithRelatedPropertiesAsSplitQuery()
        {
            IQueryable<Car> query = _context.Cars.Where(c => c.Orders.Any())
                .Include(c => c.MakeNavigation)
                .Include(c => c.Orders).ThenInclude(o => o.CustomerNavigation)
                .AsSplitQuery();
            
            var cars = query.ToList();
            
            Assert.Equal(4, cars.Count);
            
            cars.ForEach(c =>
            {
                Assert.NotNull(c.MakeNavigation);
                Assert.NotNull(c.Orders.ToList()[0].CustomerNavigation);
            });
        }

        [Fact]
        public void ShouldGetReferenceRelatedInformationExplicitly()
        {
            var car = _context.Cars.First(x => x.Id == 1);
            
            Assert.Null(car.MakeNavigation);
            
            var query = _context.Entry(car).Reference(c => c.MakeNavigation).Query();
            var queryString = query.ToQueryString();
            query.Load();
            
            Assert.NotNull(car.MakeNavigation);
        }

        [Fact]
        public void ShouldGetCollectionRelatedInformationExplicitly()
        {
            var car = _context.Cars.First(x => x.Id == 1);
            
            Assert.Empty(car.Orders);
            
            var query = _context.Entry(car).Collection(c => c.Orders).Query();
            var qs = query.ToQueryString();
            query.Load();
            
            Assert.Single(car.Orders);
        }

        [Fact]
        public void ShouldNotGetTheLemonsUsingFromSql()
        {
            var entity = _context.Model.FindEntityType($"{typeof(Car).FullName}");
            
            var tableName = entity.GetTableName();
            var schemaName = entity.GetSchema();

            var cars = _context.Cars.FromSqlRaw($"Select * from {schemaName}.{tableName}").ToList();
            
            Assert.Equal(9, cars.Count);
        }

        [Fact]
        public void ShouldGetTheCarsUsingFromSqlWithIgnoreQueryFilters()
        {
            var entity = _context.Model.FindEntityType($"{typeof(Car).FullName}");
            
            var tableName = entity.GetTableName();
            var schemaName = entity.GetSchema();
            
            var cars = _context.Cars.FromSqlRaw($"Select * from {schemaName}.{tableName}")
                .IgnoreQueryFilters().ToList();
            
            Assert.Equal(10, cars.Count);
        }

        [Fact]
        public void ShouldGetOneCarUsingInterpolation()
        {
            var carId = 1;
            var car = _context.Cars
                .FromSqlInterpolated($"Select * from dbo.Inventory where Id = {carId}")
                .Include(x => x.MakeNavigation).First();
            
            Assert.Equal("Black", car.Color);
            
            Assert.Equal("VW", car.MakeNavigation.Name);
        }

        [Theory]
        [InlineData(1, 1)]
        [InlineData(2, 1)]
        [InlineData(3, 1)]
        [InlineData(4, 2)]
        [InlineData(5, 3)]
        [InlineData(6, 1)]
        public void ShouldGetTheCarsByMakeUsingFromSql(int makeId, int expectedCount)
        {
            var entity = _context.Model.FindEntityType($"{typeof(Car).FullName}");
            
            var tableName = entity.GetTableName();
            var schemaName = entity.GetSchema();
            
            var cars = _context.Cars.FromSqlRaw($"Select * from {schemaName}.{tableName}")
                .Where(x => x.MakeId == makeId).ToList();
            
            Assert.Equal(expectedCount, cars.Count);
        }

        [Fact]
        public void ShouldGetTheCountOfCars()
        {
            var count = _context.Cars.Count();

            Assert.Equal(9, count);
        }

        [Fact]
        public void ShouldGetTheCountOfCarsIgnoreQueryFilters()
        {
            var count = _context.Cars.IgnoreQueryFilters().Count();

            Assert.Equal(10, count);
        }

        [Theory]
        [InlineData(1, 1)]
        [InlineData(2, 1)]
        [InlineData(3, 1)]
        [InlineData(4, 2)]
        [InlineData(5, 3)]
        [InlineData(6, 1)]
        public void ShouldGetTheCountOfCarsByMakeP1(int makeId, int expectedCount)
        {
            var count = _context.Cars.Count(x => x.MakeId == makeId);

            Assert.Equal(expectedCount, count);
        }

        [Theory]
        [InlineData(1, 1)]
        [InlineData(2, 1)]
        [InlineData(3, 1)]
        [InlineData(4, 2)]
        [InlineData(5, 3)]
        [InlineData(6, 1)]
        public void ShouldGetTheCountOfCarsByMakeP2(int makeId, int expectedCount)
        {
            var count = _context.Cars.Where(x => x.MakeId == makeId).Count();

            Assert.Equal(expectedCount, count);
        }

        [Theory]
        [InlineData(1, true)]
        [InlineData(11, false)]
        public void ShouldCheckForAnyCarsWithMake(int makeId, bool expectedResult)
        {
            var result = _context.Cars.Any(x => x.MakeId == makeId);

            Assert.Equal(expectedResult, result);
        }

        [Theory]
        [InlineData(1, false)]
        [InlineData(11, false)]
        public void ShouldCheckForAllCarsWithMake(int makeId, bool expectedResult)
        {
            var result = _context.Cars.All(x => x.MakeId == makeId);

            Assert.Equal(expectedResult, result);
        }

        [Theory]
        [InlineData(1, "Zippy")]
        [InlineData(2, "Rusty")]
        [InlineData(3, "Mel")]
        [InlineData(4, "Clunker")]
        [InlineData(5, "Bimmer")]
        [InlineData(6, "Hank")]
        [InlineData(7, "Pinky")]
        [InlineData(8, "Pete")]
        [InlineData(9, "Brownie")]
        public void ShouldGetValueFromStoredProc(int id, string expectedName)
        {
            Assert.Equal(expectedName, new CarRepository(_context).GetPetName(id));
        }

    }
}
