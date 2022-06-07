using Xunit;

namespace AutoLot.Tests
{
    public class SampleTests
    {
        [Fact]
        public void Add_SimpleFactTest()
        {
            Assert.Equal(5, 3 + 2);
        }

        [Theory]
        [InlineData(5, 3, 2)]
        [InlineData(0, 1, -1)]
        public void Add_SimpleTheoryTest(int expectedResult, int value1, int value2)
        {
            Assert.Equal(expectedResult, value1 + value2);
        }
    }
}
