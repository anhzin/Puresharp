using System;

namespace Puresharp.Confluence
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple=false, Inherited=true)]
    abstract public partial class Startup : Attribute
    {
        //Flag only static void method.
        //how to define startup order!? => explicit usage of class container of others Startup method!
        //Attribute is considered ad explicit usage! and assembly order too! 
    }
}
