using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eduasync
{
    public struct TaskAwaiter<T>
    {
        private readonly Task<T> task;

        internal TaskAwaiter(Task<T> task)
        {
            this.task = task;
        }

        public bool IsCompleted { get { return task.IsCompleted; } }

        public void OnCompleted(Action action)
        {
            task.ContinueWith(ignored => action(), TaskScheduler.Current);
        }

        public T GetResult()
        {
            try
            {
                return task.Result;
            }
            catch (AggregateException aggregate)
            {
                if (aggregate.InnerExceptions.Count > 0)
                {
                    // Loses the proper stack trace. Oops. For workarounds, see
                    // See http://bradwilson.typepad.com/blog/2008/04/small-decisions.html
                    throw aggregate.InnerExceptions[0];
                }
                else
                {
                    // Nothing better to do, really...
                    throw;
                }
            }
        }
    }
}
