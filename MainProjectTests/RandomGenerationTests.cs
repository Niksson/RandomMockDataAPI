using FluentAssertions;
using GenericMockApi.Repositories.RandomGeneratorFactory;
using Xunit;

namespace MainProjectTests
{
    public class RandomGenerationTests
    {
        private readonly IRandomGeneratorFactory _factory = new RandomGeneratorFactory();
        private readonly int _seed = 1000;

        [Fact]
        public void StringGenerationStabilityTest()
        {
            // Arrange
            var testingGen1 = _factory.CreateString(_seed);
            var testingGen2 = _factory.CreateString(_seed);

            // Act
            var value1 = testingGen1.GetNext();
            var value2 = testingGen2.GetNext();

            value1.Should().Be(value2);
        }

        [Fact]
        public void MultipleStringGenerationStabilityTest()
        {
            var testingGen1 = _factory.CreateString(_seed);
            var testingGen2 = _factory.CreateString(_seed);

            var value1 = testingGen1.Skip(10000).GetValues(10000);
            var value2 = testingGen2.Skip(10000).GetValues(10000);

            value1.Should().BeEquivalentTo(value2);
        }

        [Fact]
        public void BoolGenerationStabilityTest()
        {
            // Arrange
            var testingGen1 = _factory.CreateBoolean(_seed);
            var testingGen2 = _factory.CreateBoolean(_seed);

            // Act
            var value1 = testingGen1.GetNext();
            var value2 = testingGen2.GetNext();

            value1.Should().Be(value2);
        }

        [Fact]
        public void DateTimeGenerationStabilityTest()
        {
            // Arrange
            var testingGen1 = _factory.CreateDateTime(_seed);
            var testingGen2 = _factory.CreateDateTime(_seed);

            // Act
            var value1 = testingGen1.GetNext();
            var value2 = testingGen2.GetNext();

            value1.Should().Be(value2);
        }
    }
}