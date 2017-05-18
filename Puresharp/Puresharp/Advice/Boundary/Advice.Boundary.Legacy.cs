using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Puresharp
{
    public sealed partial class Advice
    {
        public partial class Boundary
        {
            internal class Legacy : Advice.IBoundary
            {
                static public Type Begin(Func<Expression, Expression[], Expression> body)
                {
                    throw new NotImplementedException();
                }

                static public Type Return(Func<Expression, Expression[], Expression, Expression> body)
                {
                    throw new NotImplementedException();
                }
                
                static public Type Throw(Func<Expression, Expression[], Expression, Expression> body)
                {
                    throw new NotImplementedException();
                }

                static public Type Dispose(Func<Expression, Expression[], Expression> body)
                {
                    throw new NotImplementedException();
                }

                public object Instance;
                public object[] Arguments;

                void Advice.IBoundary.Method(MethodBase method, ParameterInfo[] signature)
                {
                    this.Arguments = new object[signature.Length];
                }

                void Advice.IBoundary.Instance<T>(T instance)
                {
                    this.Instance = instance;
                }

                void Advice.IBoundary.Argument<T>(ParameterInfo parameter, ref T value)
                {
                    this.Arguments[parameter.Position] = value;
                }

                void Advice.IBoundary.Begin()
                {
                }

                void Advice.IBoundary.Continue()
                {
                }

                void Advice.IBoundary.Yield()
                {
                }

                void Advice.IBoundary.Return()
                {
                }

                void Advice.IBoundary.Throw(ref Exception exception)
                {
                }

                void Advice.IBoundary.Return<T>(ref T value)
                {
                }

                void Advice.IBoundary.Throw<T>(ref Exception exception, ref T value)
                {
                }

                void IDisposable.Dispose()
                {
                }
            }
        }
    }
}