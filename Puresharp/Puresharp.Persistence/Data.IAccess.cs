using System;

namespace Puresharp.Persistence
{
    static public partial class Data
    {
        public interface IAccess
        {
            Data.Access.ITransaction Transaction { get; }
            Data.ISource<T> Query<T>();
        }
    }
}
