using FluentAssertions;
using GenericMockApi.Repositories.RandomGenerators;
using GenericMockApi.Repositories.RandomGenFactory;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace MainProjectTests
{
    public class ObjectRandomGenerationTests
    {
        public class Parent
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public string AdditionalProperty => $"Id {Id}, name {Name}, description {Description}";
            public string[] StringCollectionProperty { get; set; }
            public int ReadOnlyProperty { get; }
            public IEnumerable<TestType> TestCollectionProperty { get; set; }
            public IEnumerable<int> IntCollectionProperty { get; set; }
            public bool BooleanProp { get; set; }
        }

        public class Child
        {
            public int Id { get; set; }
            public Parent Parent { get; set; }
            public int ParentId { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public string AdditionalProperty => $"Id {Id}, name {Name}, description {Description}, parent id {ParentId}, parent: {Parent.AdditionalProperty}";
            public DateTime DateTimeProp { get; set; }
        }

        public class TestType
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }

        public class TestTypeWithCollection
        {
            public string[] Name { get; set; }
        }

        [Fact]
        public void ObjectGenerationTest()
        {
            // Arrange
            var parentGenerator = new RandomObjectGenerator<Parent>(1000, 3);
            var childGenerator = new RandomObjectGenerator<Child>(1000, 3);
            // Act
            var parentValue = parentGenerator.GetNext();
            var childValue = childGenerator.GetNext();

            Console.WriteLine(JsonConvert.SerializeObject(childValue));
        }

        [Fact]
        public void LargeCollectionNumberGenerationTest()
        {
            var generator = new RandomCollectionGenerator<IEnumerable<int>>(1000, 0);

            for (int i = 0; i < 100; i++)
            {
                generator.GetValues(100000);
            }

        }

        [Fact]
        public void LargeObjectNumberGenerationTest()
        {
            var childGenerator = new RandomObjectGenerator<Child>(1000, 0);

            for (int i = 0; i < 350000000; i++)
            {
                childGenerator.GetNext();
            }

            //childGenerator.GetValues(10000000);
        }

        [Fact]
        public void LargeObjectNumberFactoryGenerationTest()
        {
            var factory = new RandomGeneratorFactory();
            dynamic childGenerator = factory.CreateObjectGenerator(typeof(Child), 1000, 0);
            childGenerator.GetValues(1);
        }

        [Fact]
        public void ObjectGenerationStabilityTest()
        {
            // Arrange
            var childGenerator1 = new RandomObjectGenerator<Child>(1000, 3);
            var childGenerator2 = new RandomObjectGenerator<Child>(1000, 3);
            // Act
            var childValue1 = childGenerator1.GetNext();
            var childValue2 = childGenerator2.GetNext();

            // Assert
            childValue1.Should().BeEquivalentTo(childValue2);
        }

        [Fact]
        public void ObjectGenerationStabilityTestWithSkip()
        {
            // Arrange
            var childGenerator1 = new RandomObjectGenerator<Child>(1000, 3);
            var childGenerator2 = new RandomObjectGenerator<Child>(1000, 3).Skip(10);

            var biggerList = new List<Child>();
            var lesserList = new List<Child>();

            // Act
            for (int i = 0; i < 25; i++)
            {
                biggerList.Add(childGenerator1.GetNext());
            }

            for (int i = 10; i < 15; i++)
            {
                lesserList.Add(childGenerator2.GetNext());
            }

            // Assert
            lesserList.Should().BeEquivalentTo(biggerList.Skip(10).Take(5).ToList());
        }
    }
}
