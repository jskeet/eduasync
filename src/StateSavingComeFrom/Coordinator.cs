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
        private readonly Dictionary<string, Continuation> labels = new Dictionary<string, Continuation>();

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

        public ExecuteAwaiter Execute(Action<Coordinator> action)
        {
            return new ExecuteAwaiter(() => action(this), this);
        }

        public ComeFromAwaiter ComeFrom(string label)
        {
            return new ComeFromAwaiter(label, this);
        }

        public LabelAwaiter Label(string label)
        {
            Continuation continuation;
            labels.TryGetValue(label, out continuation);
            return new LabelAwaiter(continuation, this);
        }

        public class ExecuteAwaiter
        {
            private readonly Action action;
            private readonly Coordinator coordinator;

            internal ExecuteAwaiter(Action action, Coordinator coordinator)
            {
                this.action = action;
                this.coordinator = coordinator;
            }

            public ExecuteAwaiter GetAwaiter()
            {
                return this;
            }

            // Always yield
            public bool IsCompleted { get { return false; } }

            public void OnCompleted(Action callerContinuation)
            {
                // We want to execute the action continuation, then get back here,
                // allowing any extra continuations put on the stack *within* the action
                // to be executed.
                coordinator.stack.Push(callerContinuation);
                coordinator.stack.Push(action);
            }

            public void GetResult()
            {
            }
        }

        public class LabelAwaiter
        {
            private readonly Continuation continuation;
            private readonly Coordinator coordinator;

            internal LabelAwaiter(Continuation continuation, Coordinator coordinator)
            {
                this.continuation = continuation;
                this.coordinator = coordinator;
            }

            public LabelAwaiter GetAwaiter()
            {
                return this;
            }

            // If there's no continuation to execute, just breeze through.
            public bool IsCompleted { get { return continuation == null; } }

            public void OnCompleted(Action action)
            {
                // We want to execute the ComeFrom continuation, then get back here.
                coordinator.stack.Push(action);
                coordinator.stack.Push(continuation.Execute);
            }

            public void GetResult()
            {
            }
        }

        /// <summary>
        /// This *must* be a struct, as we need awaiter.GetResult() to work even
        /// when the awaiter variable has been set to default(ComeFromAwaiter). Good
        /// job we don't need any state.
        /// </summary>
        public struct ComeFromAwaiter
        {
            private readonly string label;
            private readonly Coordinator coordinator;

            internal ComeFromAwaiter(string label, Coordinator coordinator)
            {
                this.label = label;
                this.coordinator = coordinator;
            }

            public ComeFromAwaiter GetAwaiter()
            {
                return this;
            }

            // We *always* want to be given the continuation
            public bool IsCompleted { get { return false; } }

            public void OnCompleted(Action action)
            {
                Continuation continuation;
                if (!coordinator.labels.TryGetValue(label, out continuation))
                {
                    // First time coming from this label. Always succeeds.
                    continuation = new Continuation(action);
                    coordinator.labels[label] = continuation;
                }
                else
                {
                    if (!continuation.Equals(new Continuation(action)))
                    {
                        throw new InvalidOperationException("Additional continuation detected for label " + label);
                    }
                    // Okay, we've seen this one before. Nothing to see here, move on.
                }
                // We actually want to continue from where we were: we're only really marking the
                // ComeFrom point.
                coordinator.stack.Push(action);
            }

            public void GetResult()
            {
            }
        }
    }
}
