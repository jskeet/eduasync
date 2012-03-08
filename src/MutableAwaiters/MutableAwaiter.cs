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
    public struct MutableAwaiter : INotifyCompletion
    {
        private string message;

        public MutableAwaiter(string message)
        {
            this.message = message;
        }

        public bool IsCompleted
        {
            get
            { 
                message = "Set in IsCompleted";
                return false;
            }
        }

        public void OnCompleted(Action action)
        {
            message = "Set in OnCompleted";
            // Ick! Completes inline. Never mind, it's only a demo...
            action();
        }

        public string GetResult()
        {
            return message;
        }
    }
}
