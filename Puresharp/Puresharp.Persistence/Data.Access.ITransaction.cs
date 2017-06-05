using System;

namespace Puresharp.Persistence
{
    static public partial class Data
    {
        static public partial class Access
        {
            public interface ITransaction
            {
                void Commit();
            }
        }
    }
}
