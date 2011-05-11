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
using System.Collections.Generic;

namespace Eduasync
{
    public sealed class Coordinator
    {
        private readonly Stack<Action> actions = new Stack<Action>();

        public Coordinator(Action<Coordinator> targetAction)
        {
            actions.Push(() => targetAction(this));
        }

        public Coordinator(Action targetAction)
        {
            actions.Push(targetAction);
        }

        public void Start()
        {
            while (actions.Count > 0)
            {
                actions.Pop().Invoke();
            }
        }

        public Awaiter Gosub(Action targetAction)
        {
            return new Awaiter(this, targetAction);
        }

        public Awaiter Gosub(Action<Coordinator> targetAction)
        {
            return new Awaiter(this, () => targetAction(this));
        }

        public Awaiter Gosub<T>(Action<T> targetAction, T value)
        {
            return new Awaiter(this, () => targetAction(value));
        }

        public Awaiter Gosub<T>(Action<Coordinator, T> targetAction, T value)
        {
            return new Awaiter(this, () => targetAction(this, value));
        }

        public sealed class Awaiter
        {
            private readonly Action targetAction;
            private readonly Coordinator coordinator;

            public Awaiter GetAwaiter()
            {
                return this;
            }

            internal Awaiter(Coordinator coordinator, Action targetAction)
            {
                this.coordinator = coordinator;
                this.targetAction = targetAction;
            }

            public bool IsCompleted { get { return false; } }

            public void OnCompleted(Action continuation)
            {
                coordinator.actions.Push(continuation);
                coordinator.actions.Push(targetAction);
            }

            public void GetResult()
            {
            }
        }
    }
}
