#region Copyright and license information
// Copyright 2012 Jon Skeet
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
    class Program
    {
        private static void Main(string[] args)
        {
            var task = DoNonsenseAsync(5);

            Console.WriteLine("Returned from async method");

            Console.WriteLine("Result: {0}", task.Result);
        }

        private static async Task<int> DoNonsenseAsync(int count)
        {
            Console.WriteLine("Bit tired. Taking a rest...");
            await Task.Delay(1000);

            int total = 0;

            Random rng = new Random();
            using (new LogFrame("Outside loop"))
            {
                for (int i = 0; i < count; i++)
                {
                    using (new LogFrame("Inside loop"))
                    {
                        total += await WaitAndReturnRandomNumber(rng);
                    }
                }
            }

            Console.WriteLine("Finished looping. Will just yield for fun.");
            await Task.Yield();
            return total;
        }

        private static async Task<int> WaitAndReturnRandomNumber(Random rng)
        {
            int number = rng.Next(1000);
            await Task.Delay(number);
            return number;
        }
    }
}
