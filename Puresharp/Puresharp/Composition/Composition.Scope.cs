using System;
using System.Threading;

namespace Puresharp
{
    public partial class Composition<X>
    {
        private class Scope<T> : IDisposable
        {
            private AsyncLocal<Func<T>> m_Instance;

            public Scope(Func<T> instance)
            {
                this.m_Instance = new AsyncLocal<Func<T>>();
                this.m_Instance.Value = new Func<T>(() => 
                {
                    var _instance = instance();
                    this.m_Instance.Value = new Func<T>(() => _instance);
                    return _instance;
                });
            }

            public T Instance
            {
                get { return this.m_Instance.Value(); }
            }

            public void Dispose()
            {
                if (this.m_Instance == null) { return; }
                this.m_Instance.Value = null;
                this.m_Instance = null;
            }
        }
    }
}
