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
            return task.Result;
        }
    }
}
