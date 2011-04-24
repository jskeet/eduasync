using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

namespace Eduasync
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Task<int> task = ReturnValueAsyncWithoutAssistance();
            Console.WriteLine(task.Result);
        }

        static async Task<int> ReturnValueAsyncWithAssistance()
        {
            Task<int> task = Task<int>.Factory.StartNew(() => 5);

            return await task;
        }

        static Task<int> ReturnValueAsyncWithoutAssistance()
        {
            AsyncTaskMethodBuilder<int> builder = AsyncTaskMethodBuilder<int>.Create();

            try
            {
                Task<int> task = Task<int>.Factory.StartNew(() => 5);

                TaskAwaiter<int> awaiter = task.GetAwaiter();
                if (!awaiter.IsCompleted)
                {
                    // Result wasn't available. Add a continuation, and return the builder.
                    awaiter.OnCompleted(() =>
                    {
                        try
                        {
                            builder.SetResult(awaiter.GetResult());
                        }
                        catch (Exception e)
                        {
                            builder.SetException(e);
                        }
                    });
                    return builder.Task;
                }

                // Result was already available: proceed synchronously
                builder.SetResult(awaiter.GetResult());
            }
            catch (Exception e)
            {
                builder.SetException(e);
            }
            return builder.Task;
        }
    }
}
