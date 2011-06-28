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
using System.Linq;
using System.Threading.Tasks;

namespace Eduasync
{
    public sealed class Coordinator<T>
    {
        private readonly Queue<Action> actions;
        private readonly Awaitable awaitable;

        private T currentValue;
        private bool valuePresent;

        public Coordinator(params Func<Coordinator<T>, T, Task<T>>[] coroutines)
        {
            // We can't refer to "this" in the variable initializer. We can use
            // the same awaitable for all yield calls.
            this.awaitable = new Awaitable(this);
            actions = new Queue<Action>(coroutines.Select(ConvertCoroutine));
        }

        // Converts a coroutine into an action which consumes the current value,
        // calls the coroutine, and attaches a continuation to it so that the return
        // value is used as the new value.
        private Action ConvertCoroutine(Func<Coordinator<T>, T, Task<T>> coroutine)
        {
            return () =>
            {
                Task<T> task = coroutine(this, ConsumeValue());
                task.ContinueWith(ignored => SupplyValue(task.Result),
                    TaskContinuationOptions.ExecuteSynchronously);
            };
        }

        public Coordinator<T> GetAwaiter()
        {
            return this;
        }

        public T GetResult()
        {
            return ConsumeValue();
        }

        // Force await to yield control
        public bool IsCompleted { get { return false; } }

        public void OnCompleted(Action continuation)
        {
            actions.Enqueue(continuation);
        }

        private T ConsumeValue()
        {
            if (!valuePresent)
            {
                throw new InvalidOperationException
                    ("Attempt to consume value when it isn't present");
            }
            T oldValue = currentValue;
            valuePresent = false;
            currentValue = default(T);
            return oldValue;
        }

        private void SupplyValue(T value)
        {
            if (valuePresent)
            {
                throw new InvalidOperationException
                    ("Attempt to supply value when one is already present");
            }
            currentValue = value;
            valuePresent = true;
        }

        public T Start(T initialValue)
        {
            SupplyValue(initialValue);
            while (actions.Count > 0)
            {
                actions.Dequeue().Invoke();
            }
            return ConsumeValue();
        }

        public Awaitable Yield(T value)
        {
            SupplyValue(value);
            return awaitable;
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
