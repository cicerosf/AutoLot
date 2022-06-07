using System;
using AutoLot.Dal.Initialization;

namespace AutoLot.Tests.Helpers
{
    public sealed class EnsureAutoLotDatabaseTestFixture : IDisposable
    {
        public EnsureAutoLotDatabaseTestFixture()
        {
            var configuration = TestHelpers.GetConfiguration();
            var context = TestHelpers.GetDbContext(configuration);
            SampleDataInitializer.ClearAndReseedDatabase(context);
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
