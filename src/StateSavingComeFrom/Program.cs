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
            Coordinator coordinator = new Coordinator(SimpleEntryPoint);
            coordinator.Start();
        }

        // ------------------ SIMPLE EXAMPLE ------------------
        private static async void SimpleEntryPoint(Coordinator coordinator)
        {
            await coordinator.Execute(SimpleOtherMethod);

            Console.WriteLine("First call to Label(x)");
            await coordinator.Label("x");

            Console.WriteLine("Second call to Label(x)");
            await coordinator.Label("x");

            Console.WriteLine("Registering interesting in y");
            bool firstTime = true;
            await coordinator.ComeFrom("y");

            Console.WriteLine("After ComeFrom(y). FirstTime={0}", firstTime);

            if (firstTime)
            {
                firstTime = false;
                await coordinator.Label("y");
            }
            Console.WriteLine("Finished");
        }

        private static async void SimpleOtherMethod(Coordinator coordinator)
        {
            Console.WriteLine("Start of SimpleOtherMethod");

            int count = 0;
            await coordinator.ComeFrom("x");

            Console.WriteLine("After ComeFrom x in SimpleOtherMethod. count={0}. Returning.",
                              count);
            count++;
        }

        // ------------------ COMPLEX EXAMPLE ------------------
        private static async void ComplexEntryPoint(Coordinator coordinator)
        {
            await coordinator.Execute(SetUpLogComeFrom);
            int count = 0;
            await coordinator.ComeFrom("End");

            Console.WriteLine("Just after ComeFrom(End). count={0}", count);
            if (count == 5)
            {
                Console.WriteLine("Okay, we're done now");
                return;
            }
            count++;

            await coordinator.Label("Log");

            await coordinator.ComeFrom("Middle");
            Console.WriteLine("Just after ComeFrom(Middle). count={0}", count);

            if (count < 3)
            {
                await coordinator.Label("TimestampedLog");
                count++;
                await coordinator.Label("Middle");
            }
            await coordinator.Label("End");
        }

        private static async void SetUpLogComeFrom(Coordinator coordinator)
        {
            await coordinator.ComeFrom("TimestampedLog");
            Console.WriteLine("Message at {0}", DateTime.Now);

            // It would be nice to be able to pass a value, but that's tricky
            // for various reasons.
            await coordinator.ComeFrom("Log");
            Console.WriteLine("We logged something.");
        }
    }
}
