using System.Linq;
using AutoLot.Dal.Repositories;
using AutoLot.Dal.Repositories.Interfaces;
using AutoLot.Tests.Base;
using AutoLot.Tests.Helpers;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace AutoLot.Tests.IntegrationTests
{
    [Collection("Integration Tests")]
    public class OrderTests : BaseTest, IClassFixture<EnsureAutoLotDatabaseTestFixture>
    {
        private readonly IOrderRepository _repository;

        public OrderTests(IOrderRepository repository)
        {
            _repository = repository;
        }

        [Fact]
        public void ShouldGetAllViewModels()
        {
            var queryString = _context.Orders.ToQueryString();
            //TODO: check it later
            //var orders = _context.CustomerOrderViewModel.ToList();
            var orders = _context.Orders.ToList();

            Assert.NotEmpty(orders);
            Assert.Equal(5, orders.Count);
        }

        [Theory]
        [InlineData("Black", 2)]
        [InlineData("Rust", 1)]
        [InlineData("Yellow", 1)]
        [InlineData("Green", 0)]
        [InlineData("Pink", 1)]
        [InlineData("Brown", 0)]
        public void ShouldGetAllViewModelsByColor(string color, int expectedCount)
        {
            var query = _repository.GetOrdersViewModels().Where(x => x.Color == color);
            var qs = query.ToQueryString();
            var orders = query.ToList();
            Assert.Equal(expectedCount, orders.Count);
        }

        public override void Dispose()
        {
            _repository.Dispose();
        }
    }
}
