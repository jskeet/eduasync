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

namespace Eduasync
{
    public class TimeMachine
    {
        private int currentTime = 0;
        private readonly SortedList<int, Action> actions = new SortedList<int, Action>();

        public int CurrentTime { get { return currentTime; } }

        public void AdvanceBy(int time)
        {
            AdvanceTo(currentTime + time);
        }

        public void AdvanceTo(int time)
        {
            // Okay, not terribly efficient, but it's simple.
            foreach (var entry in actions)
            {
                if (entry.Key > currentTime && entry.Key <= time)
                {
                    entry.Value();
                }
            }
            currentTime = time;
        }

        public Task<T> AddSuccessTask<T>(int time, T result)
        {
            TaskCompletionSource<T> tcs = new TaskCompletionSource<T>();
            actions[time] = () => tcs.SetResult(result);
            return tcs.Task;
        }

        public Task<T> AddCancelTask<T>(int time)
        {
            TaskCompletionSource<T> tcs = new TaskCompletionSource<T>();
            actions[time] = () => tcs.SetCanceled();
            return tcs.Task;
        }

        public Task<T> AddFaultingTask<T>(int time, Exception e)
        {
            TaskCompletionSource<T> tcs = new TaskCompletionSource<T>();
            actions[time] = () => tcs.SetException(e);
            return tcs.Task;
        }
    }
}
