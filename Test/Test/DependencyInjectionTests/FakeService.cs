using ExampleAssembly;
using Microsoft.Extensions.DependencyInjection;
using Zeeko.BaseDevel.DependencyInjection;

namespace Test.DependencyInjectionTests
{
    [Injectable(ServiceLifetime.Transient, group: InjectableGroups.Test)]
    public class FakeService : ISomeService
    {
    }

    [Injectable(ServiceLifetime.Scoped, "Forward", Forward = true)]
    public class ForwardedService : FakeService
    {
    }

}
