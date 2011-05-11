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

namespace Eduasync
{
    public sealed class Coordinator<T> : IEnumerable
    {
        private readonly Queue<Action> actions = new Queue<Action>();
        private readonly Stack<T> values = new Stack<T>();

        private readonly Awaitable awaitable;

        public Coordinator()
        {
            // We can't refer to "this" in the variable initializer. We can use
            // the same awaitable for all yield calls.
            this.awaitable = new Awaitable(this);
        }

        public Coordinator<T> GetAwaiter()
        {
            return this;
        }

        public T GetResult()
        {
            return values.Pop();
        }

        // Force await to yield control
        public bool IsCompleted { get { return false; } }

        public void OnCompleted(Action continuation)
        {
            actions.Enqueue(continuation);
        }

        // Used by collection initializer to specify the coroutines to run
        public void Add(Action<Coordinator<T>> coroutine)
        {
            actions.Enqueue(() => coroutine(this));
        }

        public void Start()
        {
            while (actions.Count > 0)
            {
                actions.Dequeue().Invoke();
            }
        }

        public Awaitable Yield(T value)
        {
            values.Push(value);
            return awaitable;
        }

        // Required for collection initializers, but we don't really want
        // to expose anything.
        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotSupportedException("IEnumerable only supported to enable collection initializers");
        }

        // Using a separate type forces a call to Yield in order to await
        public sealed class Awaitable
        {
            private readonly Coordinator<T> coordinator;

            internal Awaitable(Coordinator<T> coordinator)
            {
                this.coordinator = coordinator;
            }

            public Coordinator<T> GetAwaiter()
            {
                return coordinator;
            }
        }
    }
}
