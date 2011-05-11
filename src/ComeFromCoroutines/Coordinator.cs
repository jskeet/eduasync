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

using System.Collections.Generic;
using System;

namespace Eduasync
{
    public sealed class Coordinator
    {
        private readonly Dictionary<string, Queue<Action>> labelActions = new Dictionary<string, Queue<Action>>();
        
        private readonly Stack<Action> stack = new Stack<Action>();

        public Coordinator(Action<Coordinator> targetAction)
        {
            stack.Push(() => targetAction(this));
        }

        public Coordinator(Action targetAction)
        {
            stack.Push(targetAction);
        }


        public void Start()
        {
            while (stack.Count > 0)
            {
                stack.Pop().Invoke();
            }
        }

        public ComeFromAwaiter ComeFrom(string label)
        {
            Queue<Action> actionsForLabel;
            if (!labelActions.TryGetValue(label, out actionsForLabel))
            {
                actionsForLabel = new Queue<Action>();
                labelActions[label] = actionsForLabel;
            }
            return new ComeFromAwaiter(actionsForLabel);
        }

        public LabelAwaiter Label(string label)
        {
            Queue<Action> actionsForLabel;
            if (!labelActions.TryGetValue(label, out actionsForLabel))
            {
                actionsForLabel = new Queue<Action>();
                labelActions[label] = actionsForLabel;
            }
            // If there are no actions queued, we'll keep going. Otherwise,
            // we'll jump to the relevant ComeFrom.
            return new LabelAwaiter(this, actionsForLabel.Count == 0 ? null : actionsForLabel.Dequeue());
        }

        public sealed class LabelAwaiter
        {
            private readonly Action pendingAction;
            private readonly Coordinator coordinator;

            internal LabelAwaiter(Coordinator coordinator, Action pendingAction)
            {
                this.pendingAction = pendingAction;
                this.coordinator = coordinator;
            }

            public LabelAwaiter GetAwaiter()
            {
                return this;
            }

            public bool IsCompleted { get { return pendingAction == null; } }

            public void GetResult()
            {
            }

            public void OnCompleted(Action continuation)
            {
                coordinator.stack.Push(continuation);
                coordinator.stack.Push(pendingAction);
            }
        }

        public sealed class ComeFromAwaiter
        {
            private readonly Queue<Action> labelActions;

            internal ComeFromAwaiter(Queue<Action> labelActions)
            {
                this.labelActions = labelActions;
            }

            public ComeFromAwaiter GetAwaiter()
            {
                return this;
            }

            public bool IsCompleted { get { return false; } }

            public void GetResult()
            {
            }

            public void OnCompleted(Action continuation)
            {
                labelActions.Enqueue(continuation);
            }
        }
    }
}
