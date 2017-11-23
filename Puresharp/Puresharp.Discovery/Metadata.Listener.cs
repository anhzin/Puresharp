using System;

namespace Puresharp.Discovery
{
    static public partial class Metadata
    {
        public delegate void Listener<T>(T metadata)
            where T : class;
    }
}
