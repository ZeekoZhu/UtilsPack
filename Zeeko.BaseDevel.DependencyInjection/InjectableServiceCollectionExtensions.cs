using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Zeeko.BaseDevel.DependencyInjection
{
    public static class InjectableServiceCollectionExtensions
    {
        private static IEnumerable<Assembly> GetAssemblies(Type startup)
        {
            var list = new HashSet<string>();
            var stack = new Stack<Assembly>();

            stack.Push(startup.Assembly);

            do
            {
                var asm = stack.Pop();

                yield return asm;

                foreach (var reference in asm.GetReferencedAssemblies())
                {
                    var name = AppDomain.CurrentDomain.ApplyPolicy(reference.FullName);
                    if (!list.Contains(name))
                    {
                        try
                        {
                            stack.Push(Assembly.Load(name));
                            list.Add(name);
                        }
                        catch
                        {
                            // ignore
                        }
                    }
                }
            } while (stack.Count > 0);
        }

        private static IEnumerable<InjectableDescriptor> GetAllInjectableTypes(IEnumerable<Assembly> assemblies)
        {
            InjectableDescriptor ParseTypeInfo(Type implType, InjectableAttribute compAttr)
            {
                var services = new List<Type>();
                if (compAttr.ServiceTypes.Length == 0)
                {
                    var implInfo = implType.GetTypeInfo();
                    // Implemented interface as service
                    services.AddRange(implInfo.ImplementedInterfaces);
                    // Base type as a service
                    if (implInfo.BaseType != typeof(object))
                    {
                        services.Add(implInfo.BaseType);
                    }

                    // Itself as a service
                    services.Add(implType);
                }
                else
                {
                    // Specify implemented service type via attribute
                    services.AddRange(compAttr.ServiceTypes);
                }

                var descriptor = new InjectableDescriptor
                {
                    ServiceTypes = services.ToArray(),
                    Group = compAttr.Group,
                    Implementation = implType,
                    Lifetime = compAttr.ServiceLifetime,
                    ForwardImplementation = compAttr.Forward
                };
                return descriptor;
            }

            var types = assemblies.SelectMany(asm => asm.GetExportedTypes())
                .Where(t => t.IsClass && t.IsAbstract == false)
                .Select(
                    t =>
                    {
                        var compAttr = t.GetCustomAttributes()
                            .SingleOrDefault(x => x is InjectableAttribute);

                        if (compAttr == null && t.BaseType != null)
                        {
                            var baseCompAttr = t.BaseType.GetCustomAttributes()
                                .SingleOrDefault(x => x is InjectableAttribute);
                            return (t, baseCompAttr as InjectableAttribute);
                        }

                        return (t, compAttr as InjectableAttribute);
                    })
                .Where(t => t.Item2 != null)
                .Select(t => ParseTypeInfo(t.Item1, t.Item2));
            return types;
        }

        /// <summary>
        /// 扫描指定类型所在程序集及其引用的程序集中被 <see cref="InjectableAttribute"/> 标记的类型并添加到 DI 中
        /// </summary>
        /// <param name="services"></param>
        /// <param name="group">需要添加的分组，默认为 "Default"</param>
        /// <param name="namespaces">通过名称空间的前缀来过滤类型，null 或者空数组表示不进行过滤</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IServiceCollection AddInjectables<T>(
            this IServiceCollection services,
            string[] group = null,
            string[] namespaces = null)
        {
            namespaces ??= Array.Empty<string>();
            @group ??= new[] { InjectableGroups.Default };
            var assemblies = GetAssemblies(typeof(T));
            var components = GetAllInjectableTypes(assemblies)
                .Where(
                    comp =>
                    {
                        var isInGroup =
                            group.Any(g => comp.Group == g);
                        var isInNamespace =
                            namespaces.Length <= 0
                            || namespaces.Any(
                                ns =>
                                    comp.Implementation.Namespace != null
                                    && comp.Implementation.Namespace.StartsWith(ns));
                        return isInGroup && isInNamespace;
                    });
            IEnumerable<ServiceDescriptor> CollectionSelector(InjectableDescriptor comp)
            {
                if (comp.ForwardImplementation == false)
                {
                    foreach (var service in comp.ServiceTypes)
                    {
                        yield return new ServiceDescriptor(service, comp.Implementation, comp.Lifetime);
                    }
                }
                else
                {
                    yield return new ServiceDescriptor(comp.Implementation, comp.Implementation, comp.Lifetime);
                    foreach (var service in comp.ServiceTypes)
                    {
                        if (service != comp.Implementation)
                        {
                            yield return new ServiceDescriptor(
                                service,
                                sp => sp.GetRequiredService(comp.Implementation),
                                comp.Lifetime);
                        }
                    }
                }
            }
            var descriptors =
                components
                    .SelectMany(CollectionSelector);
            foreach (var descriptor in descriptors)
            {
                services.Add(descriptor);
            }

            return services;
        }
    }
}
