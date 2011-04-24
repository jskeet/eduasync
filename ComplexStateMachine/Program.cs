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
            Task<int> task = WriteValuesAsyncWithStateMachine(10);
            Console.WriteLine(task.Result);
        }

        private static async Task<int> WriteValuesAsyncWithAssistance(int loopCount)
        {
            using (TextWriter writer = File.CreateText("output.txt"))
            {
                int sum = 0;
                for (int i = 0; i < loopCount; i++)
                {
                    Task<int> valueFetcher = Task.Factory.StartNew(() => 1);

                    for (int j = 0; j < 3; j++)
                    {
                        try
                        {
                            int value = await valueFetcher;
                            writer.WriteLine("Got value {0}", value);
                            sum += value;
                            break; // Break out of the inner for loop
                        }
                        catch (Exception)
                        {
                            Console.WriteLine("Oops... retrying");
                        }
                    }
                }
                return sum;
            }
        }

        private static Task<int> WriteValuesAsyncWithStateMachine(int loopCount)
        {
            StateMachine stateMachine = new StateMachine(0);
            stateMachine.loopCount = loopCount;
            stateMachine.moveNextDelegate = stateMachine.MoveNext;
            stateMachine.builder = AsyncTaskMethodBuilder<int>.Create();
            stateMachine.MoveNext();
            return stateMachine.builder.Task;
        }

        private class StateMachine
        {
            // Fields representing local variables
            public int i;
            public int j;
            public int sum;
            public int value;
            public Task<int> valueFetcher;
            public TextWriter writer;

            // Field representing async method parameters
            public int loopCount;

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
                    bool doFinallyBodies = true;
                    int tmpState = state;

                    // Effectively a three way switch: 1, -1, 0
                    if (tmpState != 1)
                    {
                        if (state == -1)
                        {
                            return;
                        }
                        this.writer = File.CreateText("output.txt");
                    }
                    try
                    {
                        tmpState = state;
                        if (tmpState == 1)
                        {
                            goto Label_ResumePoint;
                        }
                        sum = 0;
                        i = 0;
                      Label_ResumePoint: // This shouldn't quite be here... see below
                        while (i < loopCount)
                        {
                            // Not in generated code:
                            if (state == 1)
                            {
                                goto Label_ResumePoint2;
                            }
                            // Back to generated code

                            valueFetcher = Task.Factory.StartNew(() => 1);
                            j = 0;

                            // Still not in the generated code, and still not quite right... we don't want the j test here
                          Label_ResumePoint2:
                            // Back to generated code again...
                            while (j < 3)
                            {
                              // We want Label_ResumePoint to be here really
                                try
                                {
                                    tmpState = state;
                                    if (tmpState != 1)
                                    {
                                        awaiter = valueFetcher.GetAwaiter();
                                        if (!awaiter.IsCompleted)
                                        {
                                            state = 1;
                                            doFinallyBodies = false;
                                            awaiter.OnCompleted(moveNextDelegate);
                                            return;
                                        }
                                    }
                                    else
                                    {
                                        state = 0;
                                    }
                                    int awaitResult = awaiter.GetResult();
                                    awaiter = new TaskAwaiter<int>();
                                    value = awaitResult;
                                    writer.WriteLine("Got value {0}", value);
                                    sum += value;
                                    break;
                                }
                                catch (Exception)
                                {
                                    Console.WriteLine("Oops... retrying");
                                }
                                j++;
                            }
                            i++;
                        }
                        result = sum;
                    }
                    finally
                    {
                        if (doFinallyBodies && writer != null)
                        {
                            writer.Dispose();
                        }
                    }
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
            private bool disposing;
            public int loopCountCopy;

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
