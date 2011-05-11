using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Eduasync
{
    public sealed class Coordinator
    {
        private readonly Stack<Action> actions = new Stack<Action>();

        public Coordinator(Action<Coordinator> targetAction)
        {
            actions.Push(() => targetAction(this));
        }

        public Coordinator(Action targetAction)
        {
            actions.Push(targetAction);
        }

        public void Start()
        {
            while (actions.Count > 0)
            {
                actions.Pop().Invoke();
            }
        }

        public Awaiter Gosub(Action targetAction)
        {
            return new Awaiter(this, targetAction);
        }

        public Awaiter Gosub(Action<Coordinator> targetAction)
        {
            return new Awaiter(this, () => targetAction(this));
        }

        public Awaiter Gosub<T>(Action<T> targetAction, T value)
        {
            return new Awaiter(this, () => targetAction(value));
        }

        public Awaiter Gosub<T>(Action<Coordinator, T> targetAction, T value)
        {
            return new Awaiter(this, () => targetAction(this, value));
        }

        public sealed class Awaiter
        {
            private readonly Action targetAction;
            private readonly Coordinator coordinator;

            public Awaiter GetAwaiter()
            {
                return this;
            }

            internal Awaiter(Coordinator coordinator, Action targetAction)
            {
                this.coordinator = coordinator;
                this.targetAction = targetAction;
            }

            public bool IsCompleted { get { return false; } }

            public void OnCompleted(Action continuation)
            {
                coordinator.actions.Push(continuation);
                coordinator.actions.Push(targetAction);
            }

            public void GetResult()
            {
            }
        }
    }
}
