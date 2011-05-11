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
    public sealed class PipelineSource<T>
    {
        private bool haveResult = false;
        private T currentResult = default(T);
        private readonly PipelineCoordinator coordinator;

        public PipelineSource(PipelineCoordinator coordinator)
        {
            this.coordinator = coordinator;
        }

        public PipelineSource<T> Receive()
        {
            return this;
        }

        public PipelineSource<T> GetAwaiter()
        {
            return this;
        }

        public bool IsCompleted { get { return haveResult; } }

        public void OnCompleted(Action continuation)
        {
            coordinator.AddReceiveContinuation(continuation);
        }

        public Tuple<bool, T> GetResult()
        {
            var ret = Tuple.Create(haveResult, currentResult);
            currentResult = default(T);
            haveResult = false;
            return ret;
        }

        internal void SetResult(T result)
        {
            if (haveResult)
            {
                throw new InvalidOperationException("Source already has a result");
            }
            haveResult = true;
            currentResult = result;
        }
    }
}
