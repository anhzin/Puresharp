using System;
using System.Linq.Expressions;

namespace Puresharp
{
    static public partial class Composition
    {
        static private class Linq
        {
            static public Expression Instance<T>()
                where T : class
            {
                return Expression.Call(Expression.Field(null, Runtime.Field(() => Composition.Lookup<T>.m_Instance)), Runtime<Func<T>>.Method(_Function => _Function.Invoke()));
            }
        }
    }
}
