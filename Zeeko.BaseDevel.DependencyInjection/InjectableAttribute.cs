using System;
using Microsoft.Extensions.DependencyInjection;

namespace Zeeko.BaseDevel.DependencyInjection
{
    [AttributeUsage(
        AttributeTargets.Class
    )]
    public class InjectableAttribute : Attribute
    {
        public Type[] ServiceTypes { get; }
        public ServiceLifetime ServiceLifetime { get; }
        public string Group { get; }
        public bool Forward { get; set; }

        public InjectableAttribute(
            ServiceLifetime lifetime = ServiceLifetime.Transient,
            string group = InjectableGroups.Default,
            params Type[] services
        )
        {
            ServiceLifetime = lifetime;
            Group = group;
            ServiceTypes = services ?? Array.Empty<Type>();
        }
    }
}
