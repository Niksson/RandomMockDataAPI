using FluentAssertions;
using GenericMockApi.Repositories.RandomGenFactory;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace MainProjectTests
{
    public class NumericRandomGenerationTests
    {
        private readonly IRandomGeneratorFactory _factory = new RandomGeneratorFactory();

        [Fact]
        public void IntGenerationStabilityTest()
        {
            // Arrange
            var testingGen1 = _factory.CreateNumericGenerator(1000);
            var testingGen2 = _factory.CreateNumericGenerator(1000);
            

            // Act
            var value1 = testingGen1.GetNext();
            var value2 = testingGen2.GetNext();

            // Assert
            value1.Should().Be(value2, "random generators should be stable with the same seed");
        }

        [Fact]
        public void FloatGenerationStabilityTest()
        {
            // Arrange
            var testingGen1 = _factory.CreateNumericGenerator(1000);
            var testingGen2 = _factory.CreateNumericGenerator(1000);


            // Act
            var value1 = testingGen1.GetNext();
            var value2 = testingGen2.GetNext();

            // Assert
            value1.Should().Be(value2, "random generators should be stable with the same seed");
        }

        [Fact]
        public void DecimalGenerationStabilityTest()
        {
            // Arrange
            var testingGen1 = _factory.CreateNumericGenerator(1000);
            var testingGen2 = _factory.CreateNumericGenerator(1000);


            // Act
            var value1 = testingGen1.GetNext();
            var value2 = testingGen2.GetNext();

            // Assert
            value1.Should().Be(value2, "random generators should be stable with the same seed");
        }

        [Fact]
        public void DoubleGenerationStabilityTest()
        {
            // Arrange
            var testingGen1 = _factory.CreateNumericGenerator(1000);
            var testingGen2 = _factory.CreateNumericGenerator(1000);


            // Act
            var value1 = testingGen1.GetNext();
            var value2 = testingGen2.GetNext();

            // Assert
            value1.Should().Be(value2, "random generators should be stable with the same seed");
        }

        [Fact]
        public void ByteGenerationStabilityTest()
        {
            // Arrange
            var testingGen1 = _factory.CreateNumericGenerator(1000);
            var testingGen2 = _factory.CreateNumericGenerator(1000);


            // Act
            var value1 = testingGen1.GetNext();
            var value2 = testingGen2.GetNext();

            // Assert
            value1.Should().Be(value2, "random generators should be stable with the same seed");
        }

        [Fact]
        public void LongGenerationStabilityTest()
        {
            // Arrange
            var testingGen1 = _factory.CreateNumericGenerator(1000);
            var testingGen2 = _factory.CreateNumericGenerator(1000);


            // Act
            var value1 = testingGen1.GetNext();
            var value2 = testingGen2.GetNext();

            // Assert
            value1.Should().Be(value2, "random generators should be stable with the same seed");
        }

        [Fact]
        public void UintGenerationStabilityTest()
        {
            // Arrange
            var testingGen1 = _factory.CreateNumericGenerator(1000);
            var testingGen2 = _factory.CreateNumericGenerator(1000);


            // Act
            var value1 = testingGen1.GetNext();
            var value2 = testingGen2.GetNext();

            // Assert
            value1.Should().Be(value2, "random generators should be stable with the same seed");
        }

        [Fact]
        public void UlongGenerationStabilityTest()
        {
            // Arrange
            var testingGen1 = _factory.CreateNumericGenerator(1000);
            var testingGen2 = _factory.CreateNumericGenerator(1000);


            // Act
            var value1 = testingGen1.GetNext();
            var value2 = testingGen2.GetNext();

            // Assert
            value1.Should().Be(value2, "random generators should be stable with the same seed");
        }

        [Fact]
        public void RandomGenerationSkipStability()
        {
            // Arrange
            var testingGen1 = _factory.CreateNumericGenerator(1000);
            var testingGen2 = _factory.CreateNumericGenerator(1000);

            // Act
            var value1 = testingGen1.Skip(10000).GetNext();
            var value2 = testingGen2.Skip(10000).GetNext();

            value1.Should().Be(value2, "skipping the same amount of elements should lead to the same sequence");
        }
    }
}
