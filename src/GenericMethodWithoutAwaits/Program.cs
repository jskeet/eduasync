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

namespace Eduasync
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            DoNothingAsync();
        }

        // Warning CS1998 is about a method with no awaits in... exactly what we're trying to
        // achieve!
#pragma warning disable 1998
        private static async Task<int> DoNothingAsync()
        {
            return 5;
        }
#pragma warning restore 1998
    }
}
