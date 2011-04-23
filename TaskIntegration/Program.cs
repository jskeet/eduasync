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

using System.Threading.Tasks;
using System;
using System.Threading;
namespace Eduasync
{
    internal class Program
    {
        private static readonly DateTimeOffset StartTime = DateTimeOffset.UtcNow;

        private static void Main(string[] args)
        {
            Log("In Main, before SumAsync call");
            Task<int> task = SumAsync();
            Log("In Main, after SumAsync returned");

            int result = task.Result;
            Log("Final result: " + result);
        }

        private static async Task<int> SumAsync()
        {
            Task<int> task1 = Task.Factory.StartNew(() => { Thread.Sleep(500); return 10; });
            Task<int> task2 = Task.Factory.StartNew(() => { Thread.Sleep(750); return 5; });

            Log("In SumAsync, before awaits");
           
            int value1 = await task1;
            int value2 = await task2;

            Log("In SumAsync, ater awaits");

            return value1 + value2;
        }

        private static void Log(string text)
        {
            Console.WriteLine("Thread={0}. Time={1}ms. Message={2}",
                              Thread.CurrentThread.ManagedThreadId,
                              (long)(DateTimeOffset.UtcNow - StartTime).TotalMilliseconds,
                              text);
        }
    }
}
