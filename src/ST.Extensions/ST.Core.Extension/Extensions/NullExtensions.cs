using System;
using System.Collections.Generic;
using System.Text;

namespace ST.Core.Extensions
{
    public static class NullExtensions
    {
        public static bool IsNull(this object obj)
        {
            return obj == null;
        }
    }
}
