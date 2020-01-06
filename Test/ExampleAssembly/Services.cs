using Microsoft.Extensions.DependencyInjection;
using Zeeko.BaseDevel.DependencyInjection;

namespace ExampleAssembly
{
    [Injectable(ServiceLifetime.Transient, "Foo", typeof(ISomeService))]
    public class FooService : SomeService
    {
    }

    public interface ISomeService
    {
    }

    [Injectable(ServiceLifetime.Transient, group: InjectableGroups.App)]
    public class SomeService : ISomeService
    {
    }
}

namespace ExampleAssembly.Bar
{
    public class BarService : FooService
    {
    }
}

namespace ExampleAssembly.Other
{
    public class AnotherService : SomeService
    {
    }
}
