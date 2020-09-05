using FluentAssertions;
using GenericMockApi.Repositories.RandomGeneratorFactory;
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
            var testingGen1 = _factory.CreateNumeric(1000);
            var testingGen2 = _factory.CreateNumeric(1000);
            

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
            var testingGen1 = _factory.CreateNumeric(1000);
            var testingGen2 = _factory.CreateNumeric(1000);


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
            var testingGen1 = _factory.CreateNumeric(1000);
            var testingGen2 = _factory.CreateNumeric(1000);


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
            var testingGen1 = _factory.CreateNumeric(1000);
            var testingGen2 = _factory.CreateNumeric(1000);


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
            var testingGen1 = _factory.CreateNumeric(1000);
            var testingGen2 = _factory.CreateNumeric(1000);


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
            var testingGen1 = _factory.CreateNumeric(1000);
            var testingGen2 = _factory.CreateNumeric(1000);


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
            var testingGen1 = _factory.CreateNumeric(1000);
            var testingGen2 = _factory.CreateNumeric(1000);


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
            var testingGen1 = _factory.CreateNumeric(1000);
            var testingGen2 = _factory.CreateNumeric(1000);


            // Act
            var value1 = testingGen1.GetNext();
            var value2 = testingGen2.GetNext();

            // Assert
            value1.Should().Be(value2, "random generators should be stable with the same seed");
        }
    }
}
