using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Puresharp.Discovery
{
    public class Knowledge
    {
        static private object m_Handle = new object();
        static private List<MethodBase> m_Methods = new List<MethodBase>();
        static private Action<MethodBase> m_Expanded;

        static public event Action<MethodBase> Expanded
        {
            add
            {
                lock (Knowledge.m_Handle)
                {
                    Knowledge.m_Expanded += value;
                    foreach (var _method in Knowledge.m_Methods) { value(_method); }
                }
            }
            remove
            {
                lock (Knowledge.m_Handle)
                {
                    Knowledge.m_Expanded -= value;
                }
            }
        }

        static private void Expand(MethodBase method)
        {
            lock (Knowledge.m_Handle)
            {
                Knowledge.m_Methods.Add(method);
                Knowledge.m_Expanded?.Invoke(method);
            }
        }
    }
}
