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

namespace Eduasync
{
    public sealed class PipelineCoordinator
    {
        private readonly LinkedList<Action> actions = new LinkedList<Action>();
        private bool executeHead = true;

        internal void AddInitialAction(Action action)
        {
            actions.AddLast(action);
        }

        internal void AddYieldContinuation(Action action)
        {
            actions.AddLast(action);
            executeHead = true;
        }

        internal void AddReceiveContinuation(Action action)
        {
            actions.AddFirst(action);
            executeHead = false;
        }

        public void Start()
        {
            while (actions.Count != 0)
            {
                // TODO: Work out if this is really correct. I think it is,
                // but I'm not sure I really understand it. Never a good sign...
                Action action;
                if (executeHead)
                {
                    action = actions.First.Value;
                    actions.RemoveFirst();
                }
                else
                {
                    action = actions.Last.Value;
                    actions.RemoveLast();                    
                }
                action();
            }
        }

        public static Incomplete<T> StartWith<T>(Action<PipelineSink<T>> sourceAction)
        {
            PipelineCoordinator coordinator = new PipelineCoordinator();
            PipelineSink<T> sink = new PipelineSink<T>(coordinator);
            coordinator.AddInitialAction(() => sourceAction(sink));
            return new Incomplete<T>(sink, coordinator);
        }

        public sealed class Incomplete<T>
        {
            private PipelineSink<T> currentSink;
            private PipelineCoordinator coordinator;

            internal Incomplete(PipelineSink<T> currentSink, PipelineCoordinator coordinator)
            {
                this.currentSink = currentSink;
                this.coordinator = coordinator;
            }

            public Incomplete<TResult> Then<TResult>(Action<PipelineSource<T>, PipelineSink<TResult>> connector)
            {
                PipelineSource<T> source = new PipelineSource<T>(coordinator);
                currentSink.AttachSource(source);
                PipelineSink<TResult> newSink = new PipelineSink<TResult>(coordinator);
                coordinator.AddInitialAction(() => connector(source, newSink));
                return new Incomplete<TResult>(newSink, coordinator);
            }

            public PipelineCoordinator EndWith(Action<PipelineSource<T>> sinkAction)
            {
                PipelineSource<T> source = new PipelineSource<T>(coordinator);
                currentSink.AttachSource(source);
                coordinator.AddInitialAction(() => sinkAction(source));
                return coordinator;
            }
        }
    }
}
