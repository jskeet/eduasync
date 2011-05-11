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
            for (int i = 0; i < 10; i++)
            {
                if (i % 2 == 0)
                {
                    Console.WriteLine("Gosub HandleEven");
                    await coordinator.Gosub(HandleEven, i);
                }
                else
                {
                    Console.WriteLine("Gosub HandleEven");
                    await coordinator.Gosub(HandleOdd, i);
                }
            }
        }

        private static async void HandleEven(Coordinator coordinator, int value)
        {
            await coordinator.Gosub(Print, value + " is even");
        }

        private static async void HandleOdd(Coordinator coordinator, int value)
        {
            await coordinator.Gosub(Print, value + " is odd");
        }

        private static void Print(string message)
        {
            Console.WriteLine("Message: {0}", message);
        }
    }
}
