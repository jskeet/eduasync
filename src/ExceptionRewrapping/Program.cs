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

using System.Threading.Tasks;
using System;
using System.IO;

namespace Eduasync
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Task<int> task = AwaitMultipleFailures();
            Console.WriteLine("Number of errors: {0}", task.Result);
        }

        private static async Task<int> AwaitMultipleFailures()
        {
            try
            {
                await CauseMultipleFailures().WithAllExceptions();
            }
            catch (AggregateException e)
            {
                Console.WriteLine("Caught arbitrary exception: {0}", e);
                return e.InnerExceptions.Count;
            }
            // Nothing went wrong, remarkably!
            return 0;
        }

        private static Task<int> CauseMultipleFailures()
        {
            Exception[] exceptions = { new IOException(), new ArgumentException() };
            TaskCompletionSource<int> tcs = new TaskCompletionSource<int>();
            tcs.SetException(exceptions);
            return tcs.Task;
        }
    }
}
