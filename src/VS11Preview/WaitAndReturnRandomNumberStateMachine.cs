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
    internal struct WaitAndReturnRandomNumberStateMachine : IStateMachine
    {
        // Appears in all state machines: basically the program counter
        // when the method is "paused"
        private int state;

        // One per awaiter...
        private object awaiter;
        
        // Yes, it's really a set of public fields. But hey, this is a private
        // compiler-generated type.
        public AsyncTaskMethodBuilder<int> builder;
        public Action MoveNextDelegate;

        // Local variables / parameters, hoisted into an instance variable for persistence
        // across calls. Purely local variables don't need to be public, but the parameter
        // does as the async method skeleton has to set it.
        public int number;
        public Random rng;

        // Unused in this case...
        // private object stack;

        public void SetMoveNextDelegate(Action action)
        {
            this.MoveNextDelegate = action;
        }

        public void MoveNext()
        {
            int result;
            try
            {
                TaskAwaiter localTaskAwaiter;
                if (state != 1)
                {
                    if (state != -1)
                    {
                        this.number = this.rng.Next(1000);
                        
                        localTaskAwaiter = Task.Delay(this.number).GetAwaiter();
                        if (localTaskAwaiter.IsCompleted)
                        {
                            goto AwaitCompleted;
                        }
                        this.state = 1;
                        TaskAwaiter[] awaiterArray = { localTaskAwaiter };
                        this.awaiter = awaiterArray;
                        Action continuation = this.MoveNextDelegate;
                        if (continuation == null)
                        {
                            // Force the task to be created before there's any possible race.
                            Task<int> task = this.builder.Task;

                            continuation = MoveNext;

                            // Careful here: set the continuation field in the boxed copy.
                            ((IStateMachine) continuation.Target).SetMoveNextDelegate(continuation);
                        }

                        // Why not use localTaskAwaiter here? Not sure...
                        awaiterArray[0].OnCompleted(continuation);
                    }
                    return;
                }
                localTaskAwaiter = ((TaskAwaiter[]) this.awaiter)[0];
                this.awaiter = null;
                this.state = 0;
              AwaitCompleted:
                localTaskAwaiter.GetResult();
                localTaskAwaiter = default(TaskAwaiter);
                result = this.number;
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
