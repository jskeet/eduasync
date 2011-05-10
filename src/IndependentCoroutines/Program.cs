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

namespace Eduasync
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var coordinator = new Coordinator { FirstCoroutine, SecondCoroutine };
            coordinator.Start();
        }

        private static async void FirstCoroutine(Coordinator coordinator)
        {
            Console.WriteLine("Starting FirstCoroutine");

            Console.WriteLine("Yielding from FirstCoroutine...");
            await coordinator;

            Console.WriteLine("Returned to FirstCoroutine");
            Console.WriteLine("Yielding from FirstCoroutine again...");

            await coordinator;

            Console.WriteLine("Returned to FirstCoroutine again");
            Console.WriteLine("Finished FirstCoroutine");
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
    }
}
