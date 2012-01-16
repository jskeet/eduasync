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
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Eduasync
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Task task = PrintTasksAsync();
            task.Wait();
        }

        private static async Task PrintTasksAsync()
        {
            Random rng = new Random();
            var values = Enumerable.Range(0, 10).Select(_ => rng.Next(3000)).ToList();
            Console.WriteLine("Initial order: {0}", string.Join(" ", values));

            var tasks = values.Select(DelayAsync);

            var ordered = OrderByCompletion(tasks);

            Console.WriteLine("In order of completion:");
            await ForEach(ordered, Console.WriteLine);
        }

        /// <summary>
        /// Returns a task which delays (asynchronously) by the given number of milliseconds,
        /// then return that same number back.
        /// </summary>
        private static async Task<int> DelayAsync(int delayMillis)
        {
            await TaskEx.Delay(delayMillis);
            return delayMillis;
        }

        /// <summary>
        /// Returns a sequence of tasks which will be observed to complete with the same set
        /// of results as the given input tasks, but in the order in which the original tasks complete.
        /// </summary>
        private static IEnumerable<Task<T>> OrderByCompletion<T>(IEnumerable<Task<T>> inputTasks)
        {
            // Copy the input so we know it'll be stable, and we don't evaluate it twice
            var inputTaskList = inputTasks.ToList();

            // Could use Enumerable.Range here, if we wanted...
            var completionSourceList = new List<TaskCompletionSource<T>>(inputTaskList.Count);
            for (int i = 0; i < inputTaskList.Count; i++)
            {
                completionSourceList.Add(new TaskCompletionSource<T>());
            }

            // At any one time, this is "the index of the box we've just filled".
            // It would be nice to make it nextIndex and start with 0, but Interlocked.Increment
            // returns the incremented value...
            int prevIndex = -1;

            // Now comes the magic...
            foreach (var inputTask in inputTaskList)
            {
                inputTask.ContinueWith(completed =>
                {
                    var source = completionSourceList[Interlocked.Increment(ref prevIndex)];
                    PropagateStatus(completed, source);
                },
                CancellationToken.None,
                TaskContinuationOptions.ExecuteSynchronously,
                // TODO: Work out whether this is really the right scheduler to use
                TaskScheduler.Default);
            }

            return completionSourceList.Select(source => source.Task);
        }

        /// <summary>
        /// Propagates the status of the given task (which must be completed) to a task completion source
        /// (which should not be).
        /// </summary>
        private static void PropagateStatus<T>(Task<T> completedTask,
            TaskCompletionSource<T> completionSource)
        {
            switch (completedTask.Status)
            {
                case TaskStatus.Canceled:
                    completionSource.TrySetCanceled();
                    break;
                case TaskStatus.Faulted:
                    completionSource.TrySetException(completedTask.Exception.InnerExceptions);
                    break;
                case TaskStatus.RanToCompletion:
                    completionSource.TrySetResult(completedTask.Result);
                    break;
                default:
                    // TODO: Work out whether this is really appropriate. Could set
                    // an exception in the completion source, of course...
                    throw new ArgumentException("Task was not completed");
            }
        }

        /// <summary>
        /// Executes the given action on each of the tasks in turn, in the order of
        /// the sequence. The action is passed the result of each task.
        /// </summary>
        private static async Task ForEach<T>(IEnumerable<Task<T>> tasks, Action<T> action)
        {
            foreach (var task in tasks)
            {
                T value = await task;
                action(value);
            }
        }
    }
}
