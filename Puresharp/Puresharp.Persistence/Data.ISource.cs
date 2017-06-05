using System;
using System.Linq;

namespace Puresharp.Persistence
{
    static public partial class Data
    {
        public interface ISource<T> : IQueryable<T>
        {
            void Add(T item);
            void Remove(T item);
        }
    }
}
