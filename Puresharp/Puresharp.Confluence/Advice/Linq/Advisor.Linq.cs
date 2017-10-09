using System;
using System.ComponentModel;
using System.Diagnostics;

namespace Puresharp.Confluence
{
    public sealed partial class Advice
    {
        static public partial class Style
        {
            /// <summary>
            /// Linq.
            /// </summary>
            public interface ILinq
            {
                /// <summary>
                /// GetHashCode.
                /// </summary>
                /// <returns>HashCode</returns>
                [DebuggerHidden]
                [EditorBrowsable(EditorBrowsableState.Never)]
                int GetHashCode();

                /// <summary>
                /// ToString.
                /// </summary>
                /// <returns>String</returns>
                [DebuggerHidden]
                [EditorBrowsable(EditorBrowsableState.Never)]
                string ToString();

                /// <summary>
                /// GetType.
                /// </summary>
                /// <returns>Type</returns>
                [DebuggerHidden]
                [EditorBrowsable(EditorBrowsableState.Never)]
                Type GetType();
            }

            /// <summary>
            /// Linq.
            /// </summary>
            public partial class Linq
            {
            }
        }
    }
}