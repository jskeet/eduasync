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

namespace System.Runtime.CompilerServices
{
    public struct AsyncTaskMethodBuilder<T>
    {
        private readonly TaskCompletionSource<T> source;

        private AsyncTaskMethodBuilder(TaskCompletionSource<T> source)
        {
            this.source = source;
        }

        public static AsyncTaskMethodBuilder<T> Create()
        {
            return new AsyncTaskMethodBuilder<T>(new TaskCompletionSource<T>());
        }

        public void SetException(Exception e)
        {
            source.SetException(e);
        }

        public void SetResult(T result)
        {
            source.SetResult(result);
        }

        public Task<T> Task { get { return source.Task; } }
    }
}
