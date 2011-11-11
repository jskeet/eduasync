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

namespace Eduasync
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Task<int> task = ReturnValueAsyncWithoutAssistance();
            Console.WriteLine(task.Result);
        }

        private static async Task<int> ReturnValueAsyncWithAssistance()
        {
            Task<int> task = Task<int>.Factory.StartNew(() => 5);

            return await task + 1;
        }

        private static Task<int> ReturnValueAsyncWithoutAssistance()
        {
            AsyncTaskMethodBuilder<int> builder = AsyncTaskMethodBuilder<int>.Create();

            try
            {
                Task<int> task = Task<int>.Factory.StartNew(() => 5);

                TaskAwaiter<int> awaiter = task.GetAwaiter();
                if (!awaiter.IsCompleted)
                {
                    // Result wasn't available. Add a continuation, and return the builder.
                    // We really want: awaiter.OnCompleted(goto afterAwait);
                    awaiter.OnCompleted(() =>
                    {
                        try
                        {
                            int tmp2 = awaiter.GetResult();
                            builder.SetResult(tmp2 + 1);
                        }
                        catch (Exception e)
                        {
                            builder.SetException(e);
                        }
                    });
                    return builder.Task;
                }

                // Result was already available: proceed synchronously
                afterAwait:
                int tmp = awaiter.GetResult();
                builder.SetResult(tmp + 1);
            }
            catch (Exception e)
            {
                builder.SetException(e);
            }
            return builder.Task;
        }
    }
}
