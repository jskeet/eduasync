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
    internal class Program
    {
        private static void Main(string[] args)
        {
            var pipeline = PipelineCoordinator.StartWith<int>(ProduceItems)
                                              .Then<int>(FilterItems)
                                              .Then<string>(TransformItems)
                                              .EndWith(DumpItems);

            pipeline.Start();
        }

        private async static void ProduceItems(PipelineSink<int> sink)
        {
            for (int i = 0; i < 10; i++)
            {
                await sink.Yield(i);
            }
        }

        private async static void FilterItems(PipelineSource<int> source, PipelineSink<int> sink)
        {
            Tuple<bool, int> current = await source.Receive();

            while (current.Item1)
            {
                int value = current.Item2;
                // Yield even numbers once, and multiples of 3 once (so 0 and 6 will be yielded twice each)
                if (value % 2 == 0)
                {
                    await sink.Yield(value);
                }
                if (value % 3 == 0)
                {
                    await sink.Yield(value);
                }
                current = await source.Receive();
            }
        }

        private async static void TransformItems(PipelineSource<int> source, PipelineSink<string> sink)
        {
            Tuple<bool, int> current = await source.Receive();

            while (current.Item1)
            {
                await sink.Yield("Got " + current.Item2);
                current = await source.Receive();
            }
        }

        private async static void DumpItems(PipelineSource<string> source)
        {
            Tuple<bool, string> current = await source.Receive();

            while (current.Item1)
            {
                Console.WriteLine("Received message = '{0}'", current.Item2);
                current = await source.Receive();
            }
        }
    }
}
