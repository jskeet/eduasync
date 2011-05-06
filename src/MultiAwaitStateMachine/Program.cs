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
using System.IO;

namespace Eduasync
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Task<int> task = Sum3ValuesAsyncWithAssistance();
            Console.WriteLine(task.Result);
        }

        private static async Task<int> Sum3ValuesAsyncWithAssistance()
        {
            Task<int> task1 = Task.Factory.StartNew(() => 1);
            Task<int> task2 = Task.Factory.StartNew(() => 2);
            Task<int> task3 = Task.Factory.StartNew(() => 3);


            int value1 = await task1;
            int value2 = await task2;
            int value3 = await task3;

            return value1 + value2 + value3;
        }

        private static Task<int> Sum3ValuesAsyncWithStateMachine()
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
            public Task<int> task1;
            public Task<int> task2;
            public Task<int> task3;
            public int value1;
            public int value2;
            public int value3;

            // Fields representing awaiters
            private TaskAwaiter<int> awaiter1;
            private TaskAwaiter<int> awaiter2;
            private TaskAwaiter<int> awaiter3;

            // Fields common to all async state machines
            public AsyncTaskMethodBuilder<int> builder;
            private int state;
            public Action moveNextDelegate;

            public StateMachine(int state)
            {
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
                    switch (state)
                    {
                        case 1:
                            break;

                        case 2:
                            goto Label_Awaiter2Continuation;

                        case 3:
                            goto Label_Awaiter3Continuation;

                        default:
                            if (state != -1)
                            {
                                task1 = Task.Factory.StartNew(() => 1);
                                task2 = Task.Factory.StartNew(() => 2);
                                task3 = Task.Factory.StartNew(() => 3);

                                awaiter1 = task1.GetAwaiter();
                                if (awaiter1.IsCompleted)
                                {
                                    goto Label_GetAwaiter1Result;
                                }
                                state = 1;
                                doFinallyBodies = false;
                                awaiter1.OnCompleted(moveNextDelegate);
                            }
                            return;
                    }
                    state = 0;
                Label_GetAwaiter1Result:
                    int awaitResult1 = awaiter1.GetResult();
                    awaiter1 = new TaskAwaiter<int>();
                    value1 = awaitResult1;

                    awaiter2 = task2.GetAwaiter();
                    if (awaiter2.IsCompleted)
                    {
                        goto Label_GetAwaiter2Result;
                    }
                    state = 2;
                    doFinallyBodies = false;
                    awaiter2.OnCompleted(moveNextDelegate);
                    return;
                Label_Awaiter2Continuation:
                    state = 0;
                Label_GetAwaiter2Result:
                    int awaitResult2 = awaiter2.GetResult();
                    awaiter2 = new TaskAwaiter<int>();
                    value2 = awaitResult2;

                    awaiter3 = task3.GetAwaiter();
                    if (awaiter3.IsCompleted)
                    {
                        goto Label_GetAwaiter3Result;
                    }
                    state = 3;
                    doFinallyBodies = false;
                    awaiter3.OnCompleted(moveNextDelegate);
                    return;
                Label_Awaiter3Continuation:
                    state = 0;
                Label_GetAwaiter3Result:
                    int awaitResult3 = awaiter3.GetResult();
                    awaiter3 = new TaskAwaiter<int>();
                    value3 = awaitResult3;

                    result = value1 + value2 + value3;
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
