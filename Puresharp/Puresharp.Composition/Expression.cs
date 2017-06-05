using System;
using System.Linq;
using System.Linq.Expressions;

namespace Puresharp.Composition
{
    static internal class Expression<T>
    {
        static private ParameterExpression m_Parameter = Expression.Parameter(Metadata<T>.Type);

        static public ParameterExpression Parameter
        {
            get { return Expression<T>.m_Parameter; }
        }
    }
}
