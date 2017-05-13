using System;
using System.Linq.Expressions;

namespace Puresharp
{
    public partial class Composition<X>
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
