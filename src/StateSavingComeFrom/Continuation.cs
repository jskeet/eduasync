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
using System.Reflection;

namespace Eduasync
{
    /// <summary>
    /// This hack allows a continuation to be executed more than once,
    /// contrary to the C# spec. It does this using reflection to store the
    /// value of the "state" field within the generated class. NEVER, EVER, EVER
    /// try to use this in real code. It's purely for fun.
    /// </summary>
    internal sealed class Continuation : IEquatable<Continuation>
    {
        private readonly int savedState;
        private readonly object target;
        private readonly FieldInfo field;
        private readonly Action action;

        internal Continuation(Action action)
        {
            // TODO: Use generics to create a delegate for each type. Much speedier,
            // but more complicated.
            target = action.Target;
            field = target.GetType().GetField("<>1__state", BindingFlags.Instance | BindingFlags.NonPublic);
            savedState = (int) field.GetValue(target);
            this.action = action;
        }

        internal void Execute()
        {
            field.SetValue(target, savedState);
            action();
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Continuation);
        }

        public override int GetHashCode()
        {
            return savedState ^ target.GetHashCode();
        }

        public bool Equals(Continuation other)
        {
            if (other == null)
            {
                return false;
            }
            // Reference equality is fine here. We don't care about the action,
            // as we know it will always be MoveNext.
            return savedState == other.savedState && target == other.target;
        }
    }
}
