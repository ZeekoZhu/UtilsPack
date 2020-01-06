using System;
using Microsoft.Extensions.DependencyInjection;

namespace Zeeko.BaseDevel.DependencyInjection
{
    public class InjectableDescriptor
    {
        public Type Implementation { get; set; }
        public Type[] ServiceTypes { get; set; }
        public ServiceLifetime Lifetime { get; set; }
        public string Group { get; set; }
        public bool ForwardImplementation { get; set; }
    }
}
