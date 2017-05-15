using System;
using System.Linq.Expressions;

namespace Puresharp
{
    public sealed partial class Composition
    {
        static private class Linq
        {
            static public Expression Instance<T>(int index)
                where T : class
            {
                return Expression.Call(Expression.Field(Expression.ArrayIndex(Expression.Field(null, Runtime.Field(() => Composition.Lookup<T>.Buffer)), Expression.Constant(index)), Runtime<Composition.Container<T>>.Field(_Container => _Container.Instance)), Runtime<Func<T>>.Method(_Function => _Function.Invoke()));
            }
        }
    }

    static public partial class Composition<X>
    {
        static private class Linq
        {
            static public Expression Instance<T>()
                where T : class
            {
                return Expression.Call(Expression.Field(null, Runtime.Field(() => Composition<X>.Lookup<T>.m_Instance)), Runtime<Func<T>>.Method(_Function => _Function.Invoke()));
            }
        }
    }
}
