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
            Coordinator coordinator = new Coordinator(EntryPoint);
            coordinator.Start();
        }

        private static async void EntryPoint(Coordinator coordinator)
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
