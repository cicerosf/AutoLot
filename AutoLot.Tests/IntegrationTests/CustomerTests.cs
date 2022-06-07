using System.Collections.Generic;
using System;
using System.Linq;
using System.Linq.Expressions;
using AutoLot.Tests.Base;
using AutoLot.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Xunit;
using AutoLot.Tests.Helpers;

namespace AutoLot.Tests.IntegrationTests
{
    [Collection("Integration Tests")]
    public class CustomerTests : BaseTest, IClassFixture<EnsureAutoLotDatabaseTestFixture>
    {
        [Fact]
        public void ShouldGetAllOfTheCustomers()
        {
            var queryString = _context.Customers.ToQueryString();
            var customers = _context.Customers.ToList();

            Assert.Equal(5, customers.Count);
        }

        [Fact]
        public void ShouldGetCustomersWithLastNameW()
        {
            IQueryable<Customer> query =
                _context.Customers.Where(c => 
                c.PersonalInformation.LastName.StartsWith("w", StringComparison.OrdinalIgnoreCase));

            var queryString = query.ToQueryString();
            var customers = query.ToList();

            Assert.Equal(2, customers.Count);
        }

        [Fact]
        public void ShouldGetCustomersWithLastNameWAndFirstNameM()
        {
            var query = _context.Customers
                .Where
                (c => c.PersonalInformation.LastName.StartsWith("W", StringComparison.OrdinalIgnoreCase))
                .Where
                (c => c.PersonalInformation.FirstName.StartsWith("M", StringComparison.OrdinalIgnoreCase));

            var queryString = query.ToQueryString();
            var customers = query.ToList();

            Assert.Single(customers);
        }

        [Fact]
        public void ShouldGetCustomerCountsWithLastNameWOrH()
        {
            IQueryable<Customer> query = _context.Customers
                .Where(x => x.PersonalInformation.LastName.StartsWith("W") ||
                    x.PersonalInformation.LastName.StartsWith("H"));

            var queryString = query.ToQueryString();      
            List<Customer> customers = query.ToList();
            
            Assert.Equal(3, customers.Count);
        }

        [Fact]
        public void ShouldGetCustomersWithLastNameWOrH()
        {
            IQueryable<Customer> query = _context.Customers
                .Where(x => x.PersonalInformation.LastName.StartsWith("W") ||
                    x.PersonalInformation.LastName.StartsWith("H"));

            var queryString = query.ToQueryString();          
            List<Customer> customers = query.ToList();
            
            Assert.Equal(3, customers.Count);
        }

        [Fact]
        public void ShouldGetCustomersWithLastNameLikeWOrH()
        {
            IQueryable<Customer> query = _context.Customers
                .Where(x => EF.Functions.Like(x.PersonalInformation.LastName, "W%") ||
                    EF.Functions.Like(x.PersonalInformation.LastName, "H%"));
            
            var queryString = query.ToQueryString();     
            List<Customer> customers = query.ToList();
            
            Assert.Equal(3, customers.Count);
        }

        [Fact]
        public void ShouldSortByLastNameThenFirstName()
        {
            var query = _context.Customers
                .OrderBy(x => x.PersonalInformation.LastName)
                .ThenBy(x => x.PersonalInformation.FirstName);
            
            var queryString = query.ToQueryString();
            var customers = query.ToList();
            
            //if only one customer, nothing to test
            if (customers.Count <= 1) { return; }

            for (int x = 0; x < customers.Count - 1; x++)
            {
                var person = customers[x].PersonalInformation;
                var personCompared = customers[x + 1].PersonalInformation;
                
                var compareLastName = string.Compare(
                    person.LastName,
                    personCompared.LastName,
                    StringComparison.CurrentCultureIgnoreCase);
                
                Assert.True(compareLastName <= 0);
                
                if (compareLastName != 0) continue;
                
                var compareFirstName = string.Compare(
                    person.FirstName,
                    personCompared.FirstName,
                    StringComparison.CurrentCultureIgnoreCase);
                
                Assert.True(compareFirstName <= 0);
            }
        }

        [Fact]
        public void ShouldSortByFirstNameThenLastNameUsingReverse()
        {
            var query = _context.Customers
                .OrderBy(x => x.PersonalInformation.LastName)
                .ThenBy(x => x.PersonalInformation.FirstName)
                .Reverse();
            
            var quqeryString = query.ToQueryString();
            var customers = query.ToList();
            
            //if only one customer, nothing to test
            if (customers.Count <= 1) { return; }
            
            for (int x = 0; x < customers.Count - 1; x++)
            {
                var person = customers[x].PersonalInformation;
                var personCompared = customers[x + 1].PersonalInformation;
                
                var compareLastName = string.Compare(
                    person.LastName,
                    personCompared.LastName,
                    StringComparison.CurrentCultureIgnoreCase);
                
                Assert.True(compareLastName >= 0);
                
                if (compareLastName != 0) continue;
                var compareFirstName = string.Compare(
                    person.FirstName,
                    personCompared.FirstName,
                    StringComparison.CurrentCultureIgnoreCase);
                
                Assert.True(compareFirstName >= 0);
            }
        }

        [Fact]
        public void GetFirstMatchingRecordDatabaseOrder()
        {
            //Gets the first record, database order
            var customer = _context.Customers.First();
            Assert.Equal(1, customer.Id);
        }

        [Fact]
        public void GetFirstMatchingRecordNameOrder()
        {
            //Gets the first record, lastname, first name order
            var customer = _context.Customers
                .OrderBy(x => x.PersonalInformation.LastName)
                .ThenBy(x => x.PersonalInformation.FirstName)
                .First();
            
            Assert.Equal(1, customer.Id);
        }

        [Fact]
        public void FirstShouldThrowExceptionIfNoneMatch()
        {
            Assert.Throws<InvalidOperationException>(() =>
                _context.Customers.First(c => c.Id == 10));
        }

        [Fact]
        public void FirstOrDefaultShouldReturnDefaultIfNoneMatch()
        {
            Expression<Func<Customer, bool>> expression = (x => x.Id == 10);
            
            var customer = _context.Customers.FirstOrDefault(expression);
            
            Assert.Null(customer);
        }

        [Fact]
        public void GetLastMatchingRecordNameOrder()
        {
            //Gets the last record, lastname desc, first name desc order
            var customer = _context.Customers
                .OrderBy(x => x.PersonalInformation.LastName)
                .ThenBy(x => x.PersonalInformation.FirstName)
                .Last();

            Assert.Equal(4, customer.Id);
        }

        [Fact]
        public void GetOneMatchingRecordWithSingle()
        {
            var customer = _context.Customers.Single(x => x.Id == 1);

            Assert.Equal(1, customer.Id);
        }

        [Fact]
        public void SingleShouldThrowExceptionIfNoneMatch()
        {
            Assert.Throws<InvalidOperationException>(() =>
                _context.Customers.Single(x => x.Id == 10));
        }

        [Fact]
        public void SingleShouldThrowExceptionIfMoreThenOneMatch()
        {
            Assert.Throws<InvalidOperationException>(() =>
                _context.Customers.Single());
        }
        [Fact]
        public void SingleOrDefaultShouldThrowExceptionIfMoreThenOneMatch()
        {
            Assert.Throws<InvalidOperationException>(() =>
                _context.Customers.SingleOrDefault());
        }

        [Fact]
        public void SingleOrDefaultShouldReturnDefaultIfNoneMatch()
        {
            var customer = _context.Customers.SingleOrDefault(x => x.Id == 10);
            
            Assert.Null(customer);
        }

    }
}
