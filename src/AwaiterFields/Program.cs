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

using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Eduasync
{
    class Program
    {
        static void Main(string[] args)
        {
            SingleAwaiter();
            ManyAwaiters();
        }

        static async void ManyAwaiters()
        {
            await new ClassAwaitable();
            await new ClassAwaitableWithCritical();
            await new InterfaceAwaitable();
            await Task.Delay(10);
            await new StructAwaitable();
            await Task.Run(() => { });
            await Task.Yield();
            await Task.Run(() => 5);
        }

        // The equivalent state machine (in terms of fields) as the one for ManyAwaiters
        private struct MultiAwaiterStateMachine : IAsyncStateMachine
        {
            public int state;
            public AsyncVoidMethodBuilder builder;
            private object stack;

            // Used for the first three awaiters, all reference types
            private object awaiter1;
            private StructAwaiter awaiter2;
            // Used for both Task.Delay and the non-generic Task.Run
            private TaskAwaiter awaiter3;
            private YieldAwaitable.YieldAwaiter awaiter4;
            private TaskAwaiter<int> awaiter5;

            public void MoveNext() {} 
            public void SetStateMachine(IAsyncStateMachine stateMachine) {}
        }



        static async void SingleAwaiter()
        {
            await new ClassAwaitableWithCritical();
        }

        // The equivalent state machine (in terms of fields) as the one for SingleAwaiter
        private struct SingleAwaiterStateMachine : IAsyncStateMachine
        {
            public int sstate;
            public AsyncVoidMethodBuilder builder;
            private object stack;
            private object awaiter;

            public void MoveNext() {} 
            public void SetStateMachine(IAsyncStateMachine stateMachine) {}
        }
    }
}
