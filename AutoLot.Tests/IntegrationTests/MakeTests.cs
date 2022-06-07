using System.Linq;
using AutoLot.Dal.Repositories;
using AutoLot.Dal.Repositories.Interfaces;
using AutoLot.Tests.Base;
using AutoLot.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Xunit;
using AutoLot.Tests.Helpers;

namespace AutoLot.Tests.IntegrationTests
{
    [Collection("Integration Tests")]
    public class MakeTests : BaseTest, IClassFixture<EnsureAutoLotDatabaseTestFixture>
    {
        private readonly IMakeRepository _repository;

        public MakeTests(IMakeRepository repository)
        {
            _repository = repository;
        }

        [Fact]
        public void ShouldGetAllMakesAndCarsThatAreYellow()
        {
            var query = _context.Makes.IgnoreQueryFilters()
                .Include(x => x.Cars.Where(x => x.Color == "Yellow"));
            
            var queryString = query.ToQueryString();
            var makes = query.ToList();
            
            Assert.NotNull(makes);
            
            Assert.NotEmpty(makes);
            Assert.NotEmpty(makes.Where(x => x.Cars.Any()));
            
            Assert.Empty(makes.First(m => m.Id == 1).Cars);
            Assert.Empty(makes.First(m => m.Id == 2).Cars);
            Assert.Empty(makes.First(m => m.Id == 3).Cars);
            Assert.Empty(makes.First(m => m.Id == 5).Cars);

            Assert.Single(makes.First(m => m.Id == 4).Cars);
        }

        [Theory]
        [InlineData(1, 1)]
        [InlineData(2, 1)]
        [InlineData(3, 1)]
        [InlineData(4, 2)]
        [InlineData(5, 3)]
        [InlineData(6, 1)]
        public void ShouldGetAllCarsForAMakeExplicitlyWithQueryFilters(int makeId, int carCount)
        {
            var make = _context.Makes.First(x => x.Id == makeId);
            IQueryable<Car> query = _context.Entry(make).Collection(c => c.Cars).Query();
            var queryString = query.ToQueryString();
            
            query.Load();
            
            Assert.Equal(carCount, make.Cars.Count());
        }

        [Theory]
        [InlineData(1, 2)]
        [InlineData(2, 1)]
        [InlineData(3, 1)]
        [InlineData(4, 2)]
        [InlineData(5, 3)]
        [InlineData(6, 1)]
        public void ShouldGetAllCarsForAMakeExplicitly(int makeId, int carCount)
        {
            var make = _context.Makes.First(x => x.Id == makeId);
            IQueryable<Car> query =
                _context.Entry(make).Collection(c => c.Cars).Query().IgnoreQueryFilters();
            
            var queryString = query.IgnoreQueryFilters().ToQueryString();
            
            query.Load();
            
            Assert.Equal(carCount, make.Cars.Count());
        }


        public override void Dispose()
        {
            _repository.Dispose();
        }
    }
}
