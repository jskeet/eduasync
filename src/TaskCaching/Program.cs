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
using System.Linq;
using System.Threading.Tasks;

namespace Eduasync
{
    class Program
    {
        static void Main(string[] args)
        {
            // 8 results:
            // 0: Result = 0, completed synchronously
            // 1: Result = 1, completed synchronously
            // 2: Result = 0, completed synchronously
            // 3: Result = 1, completed synchronously
            // 4: Result = 0, completed asynchronously
            // 5: Result = 1, completed asynchronously
            // 6: Result = 0, completed asynchronously
            // 7: Result = 1, completed asynchronously
            var tasks = Enumerable.Range(0, 8)
                                  .Select(x => SometimesLazy(x))
                                  .ToArray();

            // Prints:
            // tasks[0] == tasks[2]
            // tasks[1] == tasks[3]
            for (int i = 0; i < tasks.Length - 1; i++)
            {
                for (int j = i + 1; j < tasks.Length; j++)
                {
                    if (object.ReferenceEquals(tasks[i], tasks[j]))
                    {
                        Console.WriteLine("tasks[{0}] == tasks[{1}]", i, j);
                    }
                }
            }
        }

        static async Task<int> SometimesLazy(int x)
        {
            if (x > 3)
            {
                await Task.Yield();
            }
            else
            {
                await TaskEx.DontYield();
            }
            return x % 2;
        }
    }
}
