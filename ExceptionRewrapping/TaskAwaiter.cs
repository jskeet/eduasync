#region Copyright and license information
// Copyright 2011 Jon Skeet
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

using System;
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
