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
using System.Threading.Tasks;

namespace Eduasync
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var coordinator = new Coordinator<string>(FirstCoroutine,
                                                      SecondCoroutine,
                                                      ThirdCoroutine);
            string finalResult = coordinator.Start("m1");
            Console.WriteLine("Final result: {0}", finalResult);
        }

        private static async Task<string> FirstCoroutine(
            Coordinator<string> coordinator,
            string initialValue)
        {
            Console.WriteLine("Starting FirstCoroutine with initial value {0}",
                              initialValue);
            Console.WriteLine("Yielding 'x1' from FirstCoroutine...");

            string received = await coordinator.Yield("x1");

            Console.WriteLine("Returned to FirstCoroutine with value {0}", received);
            Console.WriteLine("Yielding 'x2' from FirstCoroutine...");

            received = await coordinator.Yield("x2");

            Console.WriteLine("Returned to FirstCoroutine with value {0}", received);
            Console.WriteLine("Finished FirstCoroutine");
            return "x3";
        }

        private static async Task<string> SecondCoroutine(
            Coordinator<string> coordinator,
            string initialValue)
        {
            Console.WriteLine("    Starting SecondCoroutine with initial value {0}",
                              initialValue);
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
            return "y4";
        }

        private static async Task<string> ThirdCoroutine(
            Coordinator<string> coordinator,
            string initialValue)
        {
            Console.WriteLine("        Starting ThirdCoroutine with initial value {0}",
                              initialValue);
            Console.WriteLine("        Yielding 'z1' from ThirdCoroutine...");

            string received = await coordinator.Yield("z1");

            Console.WriteLine("        Returned to ThirdCoroutine with value {0}", received);
            Console.WriteLine("        Finished ThirdCoroutine...");
            return "z2";
        }
    }
}
