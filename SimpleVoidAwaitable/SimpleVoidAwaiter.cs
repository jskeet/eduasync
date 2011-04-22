using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Eduasync
{
    public struct SimpleVoidAwaiter
    {
        public bool IsCompleted { get { return true; } }

        public void OnCompleted(Action continuation)
        {
        }

        public void GetResult()
        {
        }
    }
}
