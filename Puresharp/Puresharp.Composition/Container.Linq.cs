using System;
using System.Linq;
using System.Linq.Expressions;

namespace Puresharp.Composition
{
    public sealed partial class Container
    {
        static private class Linq
        {
            static public Expression Instance<T>(int index)
                where T : class
            {
                return Expression.Call(Expression.Field(Expression.ArrayIndex(Expression.Field(null, Metadata.Field(() => Container.Lookup<T>.Buffer)), Expression.Constant(index)), Metadata<Container.Container1<T>>.Field(_Container => _Container.Instance)), Metadata<Func<T>>.Method(_Function => _Function.Invoke()));
            }
        }
    }

    static public partial class Container<T>
    {
        static private class Linq
        {
            static public Expression Instance<T>()
                where T : class
            {
                return Expression.Call(Expression.Field(null, Metadata.Field(() => Lookup<T>.m_Instance)), Metadata<Func<T>>.Method(_Function => _Function.Invoke()));
            }
        }
    }
}
