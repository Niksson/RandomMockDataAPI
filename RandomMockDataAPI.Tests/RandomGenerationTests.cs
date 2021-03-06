﻿using FluentAssertions;
using RandomMockDataAPI.Repositories.RandomGenFactory;
using Xunit;

namespace RandomMockDataAPI.Tests
{
    public class RandomGenerationTests
    {
        private readonly IRandomGeneratorFactory _factory = new RandomGeneratorFactory();
        private readonly int _seed = 1000;

        [Fact]
        public void StringGenerationStabilityTest()
        {
            // Arrange
            var testingGen1 = _factory.CreateStringGenerator(_seed);
            var testingGen2 = _factory.CreateStringGenerator(_seed);

            // Act
            var value1 = testingGen1.GetNext();
            var value2 = testingGen2.GetNext();

            value1.Should().Be(value2);
        }

        [Fact]
        public void MultipleStringGenerationStabilityTest()
        {
            var testingGen1 = _factory.CreateStringGenerator(_seed);
            var testingGen2 = _factory.CreateStringGenerator(_seed);

            var value1 = testingGen1.Skip(10000).GetValues(10000);
            var value2 = testingGen2.Skip(10000).GetValues(10000);

            value1.Should().BeEquivalentTo(value2);
        }

        [Fact]
        public void BoolGenerationStabilityTest()
        {
            // Arrange
            var testingGen1 = _factory.CreateBooleanGenerator(_seed);
            var testingGen2 = _factory.CreateBooleanGenerator(_seed);

            // Act
            var value1 = testingGen1.GetNext();
            var value2 = testingGen2.GetNext();

            value1.Should().Be(value2);
        }

        [Fact]
        public void DateTimeGenerationStabilityTest()
        {
            // Arrange
            var testingGen1 = _factory.CreateDateTimeGenerator(_seed);
            var testingGen2 = _factory.CreateDateTimeGenerator(_seed);

            // Act
            var value1 = testingGen1.GetNext();
            var value2 = testingGen2.GetNext();

            value1.Should().Be(value2);
        }
    }
}