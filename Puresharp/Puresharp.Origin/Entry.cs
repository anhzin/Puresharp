using System;

namespace Puresharp.Origin
{
    [AttributeUsage(AttributeTargets.Constructor | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class Entry : Attribute
    {
    }
}
