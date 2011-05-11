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
            Coordinator coordinator = new Coordinator(EntryPoint);
            coordinator.Start();
        }

        private static async void EntryPoint(Coordinator coordinator)
        {
            RegisterForFirstLabel(coordinator);
            RegisterForFirstLabelAgain(coordinator);
            RegisterForSecondLabel(coordinator);

            Console.WriteLine("Hitting first label...");
            await coordinator.Label("FirstLabel");

            Console.WriteLine("Hitting second label...");
            await coordinator.Label("SecondLabel");
        }

        private static async void RegisterForFirstLabel(Coordinator coordinator)
        {
            Console.WriteLine("Registering interest");
            await coordinator.ComeFrom("FirstLabel");

            Console.WriteLine("Looks like the first label was hit! (Let's hit it again.)");
            await coordinator.Label("FirstLabel");

            Console.WriteLine("Done with the first label now");
        }

        private static async void RegisterForFirstLabelAgain(Coordinator coordinator)
        {
            await coordinator.ComeFrom("FirstLabel");

            Console.WriteLine("Looks like the first label was hit a second time!");
        }

        private static async void RegisterForSecondLabel(Coordinator coordinator)
        {
            await coordinator.ComeFrom("SecondLabel");

            Console.WriteLine("Second label hit too");
        }
    }
}
