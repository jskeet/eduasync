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
            var coordinator = new Coordinator<string> { FirstCoroutine, SecondCoroutine };
            coordinator.Start();
        }

        private static async void FirstCoroutine(Coordinator<string> coordinator)
        {
            Console.WriteLine("Starting FirstCoroutine");

            Console.WriteLine("Yielding 'x1' from FirstCoroutine...");
            string received = await coordinator.Yield("x1");

            Console.WriteLine("Returned to FirstCoroutine with value {0}", received);
            Console.WriteLine("Yielding 'x2' from FirstCoroutine...");

            received = await coordinator.Yield("x2");

            Console.WriteLine("Returned to FirstCoroutine with value {0}", received);
            Console.WriteLine("Finished FirstCoroutine");
        }

        private static async void SecondCoroutine(Coordinator<string> coordinator)
        {
            Console.WriteLine("    Starting SecondCoroutine");

            Console.WriteLine("    Yielding 'y1' from SecondCoroutine...");
            string received = await coordinator.Yield("y1");

            Console.WriteLine("    Returned to SecondCoroutine with value {0}", received);
            Console.WriteLine("    Yielding 'y2' from SecondCoroutine...");

            received = await coordinator.Yield("y2");

            Console.WriteLine("    Returned to SecondCoroutine with value {0}", received);

            // FirstCoroutine has finished now - so we'll get y3 back to ourselves
            Console.WriteLine("    Yielding 'y3' from SecondCoroutine...");
            received = await coordinator.Yield("y3");

            Console.WriteLine("    Returned to SecondCoroutine with value {0}", received);
            Console.WriteLine("    Finished SecondCoroutine");
        }
    }
}
