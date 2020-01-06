using System.Linq;
using ExampleAssembly;
using ExampleAssembly.Bar;
using ExampleAssembly.Other;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Zeeko.BaseDevel.DependencyInjection;

namespace Test.DependencyInjectionTests
{
    public class InjectableTests
    {
        [Fact]
        public void ResolveServiceTest()
        {
            var services = new ServiceCollection();
            services.AddInjectables<InjectableTests>(@group: new[] { InjectableGroups.App });
            var provider = services.BuildServiceProvider();
            var someSvc = provider.GetService<ISomeService>();
            Assert.NotNull(someSvc);
            Assert.True(someSvc is SomeService);
            var someSvcConcrete = provider.GetService<SomeService>();
            Assert.NotNull(someSvcConcrete);
        }

        [Fact]
        public void LifeTimeTest()
        {
            var services = new ServiceCollection();
            services.AddInjectables<InjectableTests>(@group: new[] { InjectableGroups.App });
            var provider = services.BuildServiceProvider();
            var someSvc = provider.GetService<ISomeService>();
            Assert.NotNull(someSvc);
            Assert.True(someSvc is SomeService);
            var anotherSvc = provider.GetService<ISomeService>();
            Assert.NotEqual(someSvc, anotherSvc);
        }

        [Fact]
        public void GroupTest()
        {
            var services = new ServiceCollection();
            services.AddInjectables<InjectableTests>(@group: new[] { InjectableGroups.Test });
            var provider = services.BuildServiceProvider();
            var someSvc = provider.GetService<ISomeService>();
            Assert.NotNull(someSvc);
            Assert.True(someSvc is FakeService);
            var impls = provider.GetServices<ISomeService>().ToList();
            Assert.Single(impls);
        }

        /// <summary>
        /// 当然也有可能匹配到多个 Service
        /// </summary>
        [Fact]
        public void GroupMatchMultiple()
        {
            var services = new ServiceCollection();
            services.AddInjectables<InjectableTests>(@group: new[] { InjectableGroups.Test, InjectableGroups.App });
            var provider = services.BuildServiceProvider();
            var someSvc = provider.GetService<ISomeService>();
            Assert.NotNull(someSvc);
            var impls = provider.GetServices<ISomeService>().ToList();
            Assert.Equal(3, impls.Count);
        }

        [Fact]
        public void GroupNotMatchTest()
        {
            var services = new ServiceCollection();
            services.AddInjectables<InjectableTests>(@group: new[] { InjectableGroups.Default });
            var provider = services.BuildServiceProvider();
            var someSvc = provider.GetService<ISomeService>();
            Assert.Null(someSvc);
            var impls = provider.GetServices<ISomeService>().ToList();
            Assert.Empty(impls);
        }

        [Fact]
        public void NamespaceTest()
        {
            var services = new ServiceCollection();
            services.AddInjectables<InjectableTests>(
                @group: new[] { InjectableGroups.Test, InjectableGroups.App },
                namespaces: new[] { "ExampleAssembly" });
            var provider = services.BuildServiceProvider();
            var someSvc = provider.GetService<ISomeService>();
            Assert.NotNull(someSvc);
            Assert.True(someSvc is SomeService);
        }

        [Fact]
        public void NamespaceNotMatchTest()
        {
            var services = new ServiceCollection();
            services.AddInjectables<InjectableTests>(
                @group: new[] { InjectableGroups.Test, InjectableGroups.App },
                namespaces: new[] { "FooBar" });
            var provider = services.BuildServiceProvider();
            var someSvc = provider.GetService<ISomeService>();
            Assert.Null(someSvc);
        }

        [Fact]
        public void SpecificServiceType()
        {
            var services = new ServiceCollection();
            services.AddInjectables<InjectableTests>(@group: new[] { "Foo" });
            var provider = services.BuildServiceProvider();
            var someSvc = provider.GetService<ISomeService>();
            Assert.NotNull(someSvc);
            Assert.True(someSvc is FooService);
            var someSvcByBaseType = provider.GetService<SomeService>();
            Assert.Null(someSvcByBaseType);
        }

        [Fact]
        public void InheritInjectableFromBase()
        {
            var services = new ServiceCollection();
            services.AddInjectables<InjectableTests>(
                @group: new[] { "Foo" },
                namespaces: new[] { "ExampleAssembly.Bar" }
            );
            var provider = services.BuildServiceProvider();
            var someSvc = provider.GetService<ISomeService>();
            Assert.NotNull(someSvc);
            Assert.True(someSvc is BarService);
            var someByConcrete = provider.GetService<BarService>();
            Assert.Null(someByConcrete);

            var impls = provider.GetServices<ISomeService>().ToList();
            Assert.Single(impls);
        }

        [Fact]
        public void BaseTypeAsService()
        {
            var services = new ServiceCollection();
            services.AddInjectables<InjectableTests>(
                @group: new[] { "App" },
                namespaces: new[] { "ExampleAssembly.Other" }
            );
            var provider = services.BuildServiceProvider();
            var someSvc = provider.GetService<ISomeService>();
            Assert.NotNull(someSvc);
            Assert.True(someSvc is AnotherService);
            var someByBase = provider.GetService<SomeService>();
            Assert.NotNull(someByBase);
            var someByInterface = provider.GetService<ISomeService>();
            Assert.NotNull(someByInterface);
        }

        [Fact]
        public void ForwardImplementation()
        {
            var services = new ServiceCollection();
            services.AddInjectables<InjectableTests>(
                @group: new[] { "Forward" }
            );
            var provider = services.BuildServiceProvider();
            var someSvc = provider.GetService<ISomeService>();
            var fakeSvc = provider.GetService<FakeService>();
            var forwardedSvc = provider.GetService<ForwardedService>();

            someSvc.Should().NotBeNull();
            someSvc.Should().BeOfType<ForwardedService>();
            fakeSvc.Should().NotBeNull();
            fakeSvc.Should().BeOfType<ForwardedService>();
            forwardedSvc.Should().NotBeNull();
            forwardedSvc.Should().BeOfType<ForwardedService>();

            someSvc.Should().Be(fakeSvc);
            fakeSvc.Should().Be(forwardedSvc);
        }
    }
}
