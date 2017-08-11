using System;
using System.Collections.Generic;
using System.Text;

namespace AleFIT.Workflow.Core.Helpers
{
    public static class ExecutionContextExtensions
    {
        public static ExecutionContext<T> AsFailed<T>(this ExecutionContext<T> context)
        {
            context.SetFailed();
            return context;
        }

        public static ExecutionContext<T> AsFailed<T>(this ExecutionContext<T> context, Exception exception)
        {
            if (exception == null) throw new ArgumentNullException(nameof(exception));

            context.SetFailed(exception);
            return context;
        }
    }
}
