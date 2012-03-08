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
using System.Runtime.CompilerServices;

namespace Eduasync
{
    public static class TaskEx
    {
        /// <summary>
        /// This is the opposite of Task.Yield; it's something you can await, but it will
        /// always be completed.
        /// </summary>
        internal static ImmediateAwaitable DontYield()
        {
            return new ImmediateAwaitable();
        }

        public struct ImmediateAwaitable
        {
            public ImmediateAwaiter GetAwaiter()
            {
                return new ImmediateAwaiter();
            }
        }

        public struct ImmediateAwaiter : INotifyCompletion
        {
            public bool IsCompleted { get { return true; } }

            public void OnCompleted(Action continuation)
            {
                // We don't expect this to ever be called...
                throw new InvalidOperationException();
            }

            public void GetResult()
            {
            }
        }
    }
}
