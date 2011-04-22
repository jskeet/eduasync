using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Eduasync
{
    public struct SimpleVoidAwaitable
    {
        public SimpleVoidAwaiter GetAwaiter()
        {
            return new SimpleVoidAwaiter();
        }
    }
}
