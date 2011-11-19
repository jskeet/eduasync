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
using System.Threading.Tasks;
using System.Threading;

namespace Eduasync
{
    public static class MoreTaskEx
    {
        public static Task<T> WhenMajority<T>(params Task<T>[] tasks)
        {
            return WhenMajority((IEnumerable<Task<T>>) tasks);
        }

        public static Task<T> WhenMajority<T>(IEnumerable<Task<T>> tasks)
        {
            if (tasks == null)
            {
                throw new ArgumentNullException("tasks");
            }
            List<Task<T>> taskList = new List<Task<T>>(tasks);
            if (taskList.Count == 0)
            {
                throw new ArgumentException("Empty sequence of tasks");
            }
            foreach (var task in taskList)
            {
                if (task == null)
                {
                    throw new ArgumentException("Null task in sequence");
                }
            }
            return WhenMajorityImpl(taskList);
        }

        private static async Task<T> WhenMajorityImpl<T>(List<Task<T>> tasks)
        {
            // Need a real majority - so for 4 or 5 tasks, must have 3 equal results.
            int majority = (tasks.Count / 2) + 1;
            int failures = 0;
            int bestCount = 0;
            
            Dictionary<T, int> results = new Dictionary<T,int>();
            List<Exception> exceptions = new List<Exception>();
            while (true)
            {
                await TaskEx.WhenAny(tasks);
                var newTasks = new List<Task<T>>();
                foreach (var task in tasks)
                {
                    switch (task.Status)
                    {
                        case TaskStatus.Canceled:
                            failures++;
                            break;
                        case TaskStatus.Faulted:
                            failures++;
                            exceptions.Add(task.Exception.Flatten());
                            break;
                        case TaskStatus.RanToCompletion:
                            int count;
                            // Doesn't matter whether it was there before or not - we want 0 if not anyway
                            results.TryGetValue(task.Result, out count);
                            count++;
                            if (count > bestCount)
                            {
                                bestCount = count;
                                if (count >= majority)
                                {
                                    return task.Result;
                                }
                            }
                            results[task.Result] = count;
                            break;
                        default:
                            // Keep going next time. may not be appropriate for Created
                            newTasks.Add(task);
                            break;
                    }
                }
                // The new list of tasks to wait for
                tasks = newTasks;

                // If we can't possibly work, bail out.
                if (tasks.Count + bestCount < majority)
                {
                    throw new AggregateException("No majority result possible", exceptions);
                }
            }
        }
    }
}
