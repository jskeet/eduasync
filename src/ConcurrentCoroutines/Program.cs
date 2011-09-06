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
using System.Threading;

namespace Eduasync
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var coordinator = new Coordinator();
            for (int i = 0; i < 5; i++)
            {
                // Avoid capturing the index variable
                int copy = i;
                coordinator.Add(x => CreateCoroutine(copy, x));
            };
            coordinator.Start(3);
        }

        private static async void CreateCoroutine(int index, Coordinator coordinator)
        {
            Console.WriteLine("Starting Coroutine {0} on thread {1}",
                              index, Thread.CurrentThread.ManagedThreadId);
            Thread.Sleep(200);
            Console.WriteLine("Coroutine {0} yielding", index);

            await coordinator;

            Console.WriteLine("Coroutine {0} continuing on thread {1}",
                              index, Thread.CurrentThread.ManagedThreadId);

            Thread.Sleep(200);
            Console.WriteLine("Coroutine {0} yielding", index);

            await coordinator;

            Console.WriteLine("Coroutine {0} continuing (again) on thread {1}",
                              index, Thread.CurrentThread.ManagedThreadId);
            Thread.Sleep(200);
            Console.WriteLine("Coroutine {0} exiting", index);
        }

        private static async void SecondCoroutine(Coordinator coordinator)
        {
            Console.WriteLine("    Starting SecondCoroutine");
            Console.WriteLine("    Yielding from SecondCoroutine...");

            await coordinator;

            Console.WriteLine("    Returned to SecondCoroutine");
            Console.WriteLine("    Yielding from SecondCoroutine again...");

            await coordinator;

            Console.WriteLine("    Returned to SecondCoroutine again");
            Console.WriteLine("    Finished SecondCoroutine");
        }

        private static async void ThirdCoroutine(Coordinator coordinator)
        {
            Console.WriteLine("        Starting ThirdCoroutine");
            Console.WriteLine("        Yielding from ThirdCoroutine...");

            await coordinator;

            Console.WriteLine("        Returned to ThirdCoroutine");
            Console.WriteLine("        Finished ThirdCoroutine...");
        }
    }
}
