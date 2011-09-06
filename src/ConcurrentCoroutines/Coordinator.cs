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
using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Threading;

namespace Eduasync
{
    public sealed class Coordinator : IEnumerable
    {
        private readonly BlockingCollection<Action> actions =
            new BlockingCollection<Action>(new ConcurrentQueue<Action>());

        // Used by collection initializer to specify the coroutines to run
        public void Add(Action<Coordinator> coroutine)
        {
            actions.Add(() => coroutine(this));
        }

        // Required for collection initializers, but we don't really want
        // to expose anything.
        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotSupportedException("IEnumerable only supported to enable collection initializers");
        }
        
        // Execute actions in the queue until it's empty. Actions add *more*
        // actions (continuations) to the queue by awaiting this coordinator.
        public void Start(int threads)
        {
            // An alternative is to use Parallel.ForEach, but that needs a bit more work.
            // See http://blogs.msdn.com/b/pfxteam/archive/2010/04/06/9990420.aspx
            // We could use Parallel.For, but it would be nice to really force as many
            // threads as we've been asked to create.

            List<Thread> threadList = new List<Thread>();
            for (int i = 0; i < threads; i++)
            {
                Thread t = new Thread(ProcessActions);
                t.Start();
                threadList.Add(t);
            }
            // Now block until we're done
            foreach (Thread t in threadList)
            {
                t.Join();
            }
        }

        private void ProcessActions()
        {
            // As soon as the queue is empty, we're done. We always add to the
            // collection *before* returning from the async method, and the thread
            // doing that adding is about to check again. The number of threads is
            // therefore never less than the number of active async methods.
            Action action;
            while (actions.TryTake(out action))
            {
                action();
            }
        }

        // Used by await expressions to get an awaiter
        public Coordinator GetAwaiter()
        {
            return this;
        }

        // Force await to yield control
        public bool IsCompleted { get { return false; } }

        public void OnCompleted(Action continuation)
        {
            // Put the continuation at the end of the queue, ready to
            // execute when the other coroutines have had a go.
            actions.Add(continuation);
        }

        public void GetResult()
        {
            // Our await expressions are void, and we never need to throw
            // an exception, so this is a no-op.
        }
    }
}
