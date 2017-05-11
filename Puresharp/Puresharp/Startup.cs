using System;

namespace Puresharp
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple=true, Inherited=true)]
    abstract public partial class Startup : Attribute
    {
        abstract public void Run();
    }
}
