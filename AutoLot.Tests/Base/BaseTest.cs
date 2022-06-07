using System;
using System.Data;
using AutoLot.Dal.EfStructures;
using AutoLot.Tests.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;

namespace AutoLot.Tests.Base
{
    public abstract class BaseTest : IDisposable
    {
        protected readonly IConfiguration _configuration;
        protected readonly ApplicationDbContext _context;

        protected BaseTest()
        {
            _configuration = TestHelpers.GetConfiguration();
            _context = TestHelpers.GetDbContext(_configuration);
        }

        protected void ExecuteInATransaction(Action actionToExecute)
        {
            var strategy = _context.Database.CreateExecutionStrategy();
            strategy.Execute(() =>
            {
                using var transaction = _context.Database.BeginTransaction();
                actionToExecute();
                transaction.Rollback();
            });
        }

        protected void ExecuteInASharedTransaction(Action<IDbContextTransaction> actionToExecute)
        {
            var strategy = _context.Database.CreateExecutionStrategy();
            strategy.Execute(() =>
            {
                using IDbContextTransaction transaction =
                    _context.Database.BeginTransaction(IsolationLevel.ReadUncommitted);

                actionToExecute(transaction);

                transaction.Rollback();
            });
        }

        public virtual void Dispose()
        {
            _context.Dispose();
        }
    }
}
