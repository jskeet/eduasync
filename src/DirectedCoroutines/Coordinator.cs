using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Eduasync
{
    internal sealed class Coordinator
    {
        private readonly Dictionary<Action<Coordinator>, Action> continuationMap =
            new Dictionary<Action<Coordinator>, Action>();

        private Action<Coordinator> currentMethod;
        private Action<Coordinator> nextMethod;
        private Action nextAction;

        public void Start(Action<Coordinator> method)
        {
            YieldTo(method);
            while (nextAction != null)
            {
                currentMethod = nextMethod;
                nextMethod = null;
                Action tmp = nextAction;
                nextAction = null;
                tmp();
            }
        }

        public Coordinator YieldTo(Action<Coordinator> method)
        {
            Action action;
            if (!continuationMap.TryGetValue(method, out action))
            {
                action = () => method(this);
            }
            nextAction = action;
            nextMethod = method;
            return this;
        }

        public Coordinator GetAwaiter()
        {
            return this;
        }

        public bool IsCompleted { get { return false; } }

        public void OnCompleted(Action continuation)
        {
            continuationMap[currentMethod] = continuation;
        }

        public void GetResult()
        {
        }
    }
}
