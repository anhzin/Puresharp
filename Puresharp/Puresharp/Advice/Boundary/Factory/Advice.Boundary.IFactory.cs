using System;

namespace Puresharp
{
    public sealed partial class Advice
    {
        public partial class Boundary
        {
            public interface IFactory
            {
                Advice.IBoundary Create();
            }
        }
    }
}