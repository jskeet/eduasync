using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Eduasync
{
    public struct SimpleInt32Awaitable
    {
        public SimpleInt32Awaiter GetAwaiter()
        {
            return new SimpleInt32Awaiter();
        }
    }
}
