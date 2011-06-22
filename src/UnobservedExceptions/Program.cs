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
using System.IO;

namespace Eduasync
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Task<int> task = CauseTwoFailures();

            try
            {
                Console.WriteLine(task.Result);
            }
            catch (AggregateException)
            {
                Console.WriteLine("Oops, it failed");
            }

            GC.Collect();
            GC.WaitForPendingFinalizers();

            Console.WriteLine("All okay - exiting");
        }

        private static async Task<int> CauseTwoFailures()
        {
            Task<int> firstTask = Task<int>.Factory.StartNew(() => { throw new InvalidOperationException(); });
            Task<int> secondTask = Task<int>.Factory.StartNew(() => { throw new InvalidOperationException(); });

            int firstValue = await firstTask;
            int secondValue = await secondTask;

            return firstValue + secondValue;
        }
    }
}
