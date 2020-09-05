using FluentAssertions;
using GenericMockApi.Repositories;
using MainProject.Models;
using MainProjectWebApi.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xunit;

namespace MainProjectTests
{
    public class MockServiceReflectionTests
    {
        private readonly string AssemblyDirectory;

        public MockServiceReflectionTests()
        {
            AssemblyDirectory = @"C:\Users\nickm\Projects\MassonNS\GenericMockApi\bin\Debug\netcoreapp3.1\";
        }

        public TypeFinder SetupTypeFinder()
        {
            var services = new ServiceCollection();
            services.AddMemoryCache();
            var sp = services.BuildServiceProvider();

            var cache = sp.GetRequiredService<IMemoryCache>();
            var typeFinder = new TypeFinder(cache);

            return typeFinder;
        }

        [Fact]
        public void GettingTypeTestUnit()
        {
            // Arrange
            var typeFinder = SetupTypeFinder();
            Directory.SetCurrentDirectory(AssemblyDirectory);

            // Act
            var type = typeFinder.GetTypeFromAssemblies("Unit");
            type.Name.Should().Be(nameof(Unit));
        }

        [Fact]
        public void GettingTypeTestUnitEvent()
        {
            // Arrange
            var typeFinder = SetupTypeFinder();

            // Act
            var type = typeFinder.GetTypeFromAssemblies("UnitEvent");
            type.Name.Should().Be(nameof(UnitEvent));
        }

    }
}
