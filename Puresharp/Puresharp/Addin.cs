using System;

namespace Puresharp
{
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple=true, Inherited=true)]
    public class Addin : Attribute
    {
        public readonly Type Extension;
        public readonly Type Type;

        public Addin(Type extension, Type type)
        {
            this.Extension = extension;
            this.Type = type;
        }
    }
}
