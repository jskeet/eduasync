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
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Eduasync
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Task<int> task = ReturnValueAsyncWithStateMachine();
            Console.WriteLine(task.Result);
        }

        private static async Task<int> ReturnValueAsyncWithAssistance()
        {
            Task<int> task = Task<int>.Factory.StartNew(() => 5);

            return await task;
        }
        
        private static Task<int> ReturnValueAsyncWithStateMachine()
        {
            StateMachine stateMachine = new StateMachine(0);
            stateMachine.moveNextDelegate = stateMachine.MoveNext;
            stateMachine.builder = AsyncTaskMethodBuilder<int>.Create();
            stateMachine.MoveNext();
            return stateMachine.builder.Task;
        }

        [CompilerGenerated]
        private sealed class StateMachine
        {
            // Fields representing local variables
            public Task<int> task;

            // Fields representing awaiters
            private TaskAwaiter<int> awaiter;

            // Fields common to all async state machines
            public AsyncTaskMethodBuilder<int> builder;
            private int state;
            public Action moveNextDelegate;

            public StateMachine(int state)
            {
                // Pointless: will always be 0. Expect this to be removed from later builds.
                this.state = state;
            }

            public void MoveNext()
            {
                int result;
                try
                {
#pragma warning disable 0219 // doFinallyBodies is never used
                    bool doFinallyBodies = true;
#pragma warning restore
                    if (state != 1)
                    {
                        if (state != -1)
                        {
                            task = Task<int>.Factory.StartNew(() => 5);
                            awaiter = task.GetAwaiter();
                            if (awaiter.IsCompleted)
                            {
                                goto Label_GetResult;
                            }
                            state = 1;
                            doFinallyBodies = false;
                            awaiter.OnCompleted(moveNextDelegate);
                        }
                        return;
                    }
                    state = 0;
                  Label_GetResult: // target of state=1
                    int awaitResult = awaiter.GetResult();
                    awaiter = default(TaskAwaiter<int>);
                    result = awaitResult;
                }
                catch (Exception e)
                {
                    state = -1;
                    builder.SetException(e);
                    return;
                }
                state = -1;
                builder.SetResult(result);
            }


            // Obsolete: will be removed from later builds.
#pragma warning disable 0414
            private bool disposing;
#pragma warning restore

            [DebuggerHidden]
            public void Dispose()
            {
                disposing = true;
                MoveNext();
                state = -1;
            }
        }
    }
}
