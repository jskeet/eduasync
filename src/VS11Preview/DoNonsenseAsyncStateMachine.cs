#region Copyright and license information
// Copyright 2012 Jon Skeet
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
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Eduasync
{
    // Really this would be a private nested struct
    internal struct DoNonsenseAsyncStateMachine : IStateMachine
    {
        // Appears in all state machines: basically the program counter
        // when the method is "paused"
        private int state;

        // An array containing whatever the current awaiter is.
        private object awaiter;
        
        // Yes, it's really a set of public fields. But hey, this is a private
        // compiler-generated type.
        public AsyncTaskMethodBuilder<int> builder;
        public Action MoveNextDelegate;

        // Local variables / parameters, hoisted into an instance variable for persistence
        // across calls. Purely local variables don't need to be public, but the parameter
        // does as the async method skeleton has to set it.
        public Random rng;
        public int count;
        public int total;
        public int i;
        public LogFrame outerLogFrame;
        public LogFrame innerLogFrame;

        // Maintains the value of total before the call to WaitAndReturnRandomNumber.
        // Somewhat unclear why it's necessary...
        private object stack;

        public void SetMoveNextDelegate(Action action)
        {
            this.MoveNextDelegate = action;
        }

        public void MoveNext()
        {
            int result;
            try
            {
                Action continuation;
                TaskAwaiter localTaskAwaiter;
                YieldAwaitable.YieldAwaiter[] yieldAwaiterArray;
                bool doFinallyBodies = true;
                switch (this.state)
                {
                    case 1:
                        break;
                    case 2:
                        goto Await2CompletedTrampoline1;
                    case 3:
                        goto Await3Completed;
                    default:
                        if (this.state != -1)
                        {
                            Console.WriteLine("Bit tired. Taking a rest...");
                            localTaskAwaiter = Task.Delay(1000).GetAwaiter();
                            if (localTaskAwaiter.IsCompleted)
                            {
                                goto Await1Completed;
                            }
                            this.state = 1;
                            TaskAwaiter[] taskAwaiterArray = { localTaskAwaiter };
                            this.awaiter = taskAwaiterArray;
                            continuation = this.MoveNextDelegate;
                            if (continuation == null)
                            {
                                // Force the task to be created before there's any possible race.
                                Task<int> task = this.builder.Task;

                                continuation = MoveNext;
                                // Careful here: set the continuation field in the boxed copy.
                                ((IStateMachine)continuation.Target).SetMoveNextDelegate(continuation);
                            }
                            taskAwaiterArray[0].OnCompleted(continuation);
                            doFinallyBodies = false;
                        }
                        return;
                }
                // For state == 1, i.e. continuing from first await
                localTaskAwaiter = ((TaskAwaiter[]) this.awaiter)[0];
                this.awaiter = null;
                this.state = 0;
              Await1Completed:
                localTaskAwaiter.GetResult();
                localTaskAwaiter = default(TaskAwaiter);
                this.total =  0;
                this.rng = new Random();
                this.outerLogFrame = new LogFrame("Outside loop");
              // We can't jump into the middle of a try block...
              Await2CompletedTrampoline1:
                try
                {
                    int localState = this.state;
                    bool fakeTrampoline2 = false;
                    if (localState == 2)
                    {
                        // Can't actually do this...
                        // goto Await2CompletedTrampoline2;
                        // So let's fake it...
                        fakeTrampoline2 = true;
                    }
                    if (!fakeTrampoline2)
                    {
                        this.i = 0;
                    }
                    while (fakeTrampoline2 || i < this.count)
                    {
                        if (!fakeTrampoline2)
                        {
                            this.innerLogFrame = new LogFrame("Inside loop");
                        }
                        fakeTrampoline2 = false;
                      // We still can't jump into the middle of a try block...
                      // The C# compiler can generate code to jump here, but it's not
                      // valid C#...
                      // Await2CompletedTrampoline2:
                        try
                        {
                            int totalBefore = 0; // Initialization isn't present in IL, but I believe it's implicit...
                            TaskAwaiter<int> localTaskInt32Awaiter;
                            localState = this.state;
                            // Can only be because state = 0, i.e. we've gone back to the top of the loop
                            if (localState != 2)
                            {
                                localTaskInt32Awaiter = GeneratedProgram.WaitAndReturnRandomNumber(this.rng).GetAwaiter();
                                if (!localTaskInt32Awaiter.IsCompleted)
                                {
                                    totalBefore = total;
                                    this.stack = totalBefore; // Unclear why we need this...
                                    this.state = 2;
                                    TaskAwaiter<int>[] taskInt32AwaiterArray = { localTaskInt32Awaiter };
                                    this.awaiter = taskInt32AwaiterArray;

                                    continuation = this.MoveNextDelegate;
                                    if (continuation == null)
                                    {
                                        // Force the task to be created before there's any possible race.
                                        Task<int> task = this.builder.Task;

                                        continuation = MoveNext;
                                        // Careful here: set the continuation field in the boxed copy.
                                        ((IStateMachine)continuation.Target).SetMoveNextDelegate(continuation);
                                    }
                                    taskInt32AwaiterArray[0].OnCompleted(continuation);
                                    doFinallyBodies = false;
                                    return;
                                }
                            }
                            else
                            {
                                // Trampolined in from the continuation in state 2
                                totalBefore = (int)this.stack;
                                this.stack = null;
                                localTaskInt32Awaiter = ((TaskAwaiter<int>[])this.awaiter)[0];
                                this.awaiter = null;
                                this.state = 0;
                            }
                            int localTaskInt32Result = localTaskInt32Awaiter.GetResult();
                            localTaskInt32Awaiter = default(TaskAwaiter<int>);
                            totalBefore += localTaskInt32Result;
                            this.total = totalBefore;
                        }
                        finally
                        {
                            if (doFinallyBodies && this.innerLogFrame != null)
                            {
                                this.innerLogFrame.Dispose();
                            }
                        }
                        this.i++;
                    }
                }
                finally
                {
                    if (doFinallyBodies && this.outerLogFrame != null)
                    {
                        this.outerLogFrame.Dispose();
                    }
                }
                Console.WriteLine("Finished looping. Will just yield for fun.");
                YieldAwaitable.YieldAwaiter localYieldAwaiter = Task.Yield().GetAwaiter();
                if (localYieldAwaiter.IsCompleted)
                {
                    goto Await3CompletedImmediately;
                }
                this.state = 3;
                yieldAwaiterArray = new YieldAwaitable.YieldAwaiter[] { localYieldAwaiter };
                this.awaiter = yieldAwaiterArray;
                
                continuation = this.MoveNextDelegate;
                if (continuation == null)
                {
                    // Force the task to be created before there's any possible race.
                    Task<int> task = this.builder.Task;

                    continuation = MoveNext;
                    // Careful here: set the continuation field in the boxed copy.
                    ((IStateMachine)continuation.Target).SetMoveNextDelegate(continuation);
                }
                yieldAwaiterArray[0].OnCompleted(continuation);
                doFinallyBodies = false;
                return;
              Await3Completed:
                 localYieldAwaiter = ((YieldAwaitable.YieldAwaiter[]) this.awaiter)[0];
                 this.awaiter = null;
                 this.state = 0;
              Await3CompletedImmediately:
                 localYieldAwaiter.GetResult();
                 localYieldAwaiter = default(YieldAwaitable.YieldAwaiter);
                result = this.total;
            }
            catch (Exception e)
            {
                this.state = -1;
                this.builder.SetException(e);
                return;
            }
            this.state = -1;
            this.builder.SetResult(result);
        }
    }
}
