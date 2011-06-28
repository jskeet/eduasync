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
using System.Collections.Generic;

namespace Eduasync
{
    internal class Program
    {
        private static readonly Queue<int> values = new Queue<int>();

        private static void Main(string[] args)
        {
            Coordinator coordinator = new Coordinator();
            coordinator.Start(Producer);
        }

        private static async void Producer(Coordinator coordinator)
        {
            for (int i = 0; i < 10; i++)
            {
                Console.WriteLine("Enqueuing {0}", i);
                values.Enqueue(i);
                if (values.Count == 3)
                {
                    Console.WriteLine("Queue full; switching to consumer");
                    await coordinator.YieldTo(Consumer);
                }
            }
            // Dequeue any remaining items
            await coordinator.YieldTo(Consumer);
        }

        private static async void Consumer(Coordinator coordinator)
        {
            int loopCount = 0;
            while (true)
            {
                Console.WriteLine("    Entering consumer loop; count {0}", loopCount);
                loopCount++;
                while (values.Count != 0)
                {
                    Console.WriteLine("    Dequeued {0}", values.Dequeue());
                }
                Console.WriteLine("    Queue empty; switching to producer");
                await coordinator.YieldTo(Producer);
            }
        }
    }
}
