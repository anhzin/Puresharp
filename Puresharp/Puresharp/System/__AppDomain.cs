using System;
using System.ComponentModel;
using System.Reflection;
using System.Reflection.Emit;

namespace Puresharp
{
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    static internal class __AppDomain
    {
        static public ModuleBuilder DefineDynamicModule(this AppDomain domain)
        {
            var _identity = Guid.NewGuid().ToString("N");
            return domain.DefineDynamicAssembly(new AssemblyName(string.Concat(Runtime<Assembly>.Type.Name, _identity)), AssemblyBuilderAccess.Run).DefineDynamicModule(string.Concat(Runtime<Module>.Type.Name, _identity), false);
        }
    }
}
