using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Zeeko.UtilsPack
{
    /// <summary>
    /// 提供简单的 object to object 的映射功能
    /// </summary>
    public static class LowBMapper
    {
        private static readonly SortedDictionary<ulong, Delegate> MappersCollection = new SortedDictionary<ulong, Delegate>();

        public static MapContext<TI> Map<TI>(this TI src)
        {
            return new MapContext<TI>(src);
        }


        public class MapContext<TI>
        {
            private readonly TI _src;

            public MapContext(TI src)
            {
                _src = src;
            }

            public TO To<TO>()
            {
                var srcType = typeof(TI);
                var targetType = typeof(TO);
                var key = ComputeMapperKey(srcType, targetType);

                if (MappersCollection.TryGetValue(key, out var del) == false)
                {
                    var param = Expression.Parameter(srcType, "src");
                    var srcMembers = param.Type.GetFieldsAndProperties();
                    var targetMembers = targetType.GetFieldsAndProperties();
                    var memberAssignments = targetMembers.Join(srcMembers, mem => mem.Name, mem => mem.Name,
                        (targetMember, sourceMember) =>
                            Expression.Bind(targetMember, Expression.PropertyOrField(param, sourceMember.Name)));

                    var init = Expression.MemberInit(Expression.New(targetType), memberAssignments);
                    del = Expression.Lambda<Func<TI, TO>>(init, param).Compile();

                    MappersCollection.Add(key, del);
                }
                return ((Func<TI, TO>)del)(_src);
            }
        }

        static ulong ComputeMapperKey(Type src, Type target)
        {
            return ((ulong)src.GetHashCode() << 32) | (uint)target.GetHashCode();
        }
    }
}
