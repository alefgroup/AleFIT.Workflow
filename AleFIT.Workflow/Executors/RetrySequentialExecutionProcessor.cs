using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using AleFIT.Workflow.Core;

namespace AleFIT.Workflow.Executors
{
    internal class RetrySequentialExecutionProcessor<T> : IExecutionProcessor<T>
    {
        public Task<ExecutionContext<T>> ProcessAsync(ExecutionContext<T> context, IReadOnlyList<IExecutable<T>> executables)
        {
            int index = 0;
            if (context.State == ExecutionState.Paused)
            {
                index = context.PersistedExecutionIndexes.Pop();
            }

            return ProcessAsync(context, executables, index);
        }

        public async Task<ExecutionContext<T>> ProcessAsync(ExecutionContext<T> context, IReadOnlyList<IExecutable<T>> executables, int executionIndex)
        {
            int currentRetryCount = -1;
            int maxRetryCount = context.InternalConfiguration.MaxRetryCount;

            ExecutionContext<T> result;
            do
            {
                context.SetCompleted();
                result = await ProcessWithRetriesAsync(context, executables, executionIndex).ConfigureAwait(false);
                currentRetryCount++;
            }
            while (result.State == ExecutionState.Failed && currentRetryCount < maxRetryCount);

            return result;
        }

        private async Task<ExecutionContext<T>> ProcessWithRetriesAsync(
            ExecutionContext<T> context,
            IReadOnlyList<IExecutable<T>> executables,
            int executionIndex)
        {
            for (var index = executionIndex; index < executables.Count; index++)
            {
                try
                {
                    context = await executables[index].ExecuteAsync(context).ConfigureAwait(false);

                    switch (context.State)
                    {
                        case ExecutionState.Completed:
                        case ExecutionState.Running:
                            context.IncrementProcessedActions();
                            continue;
                        case ExecutionState.Paused:
                            context.PersistedExecutionIndexes.Push(index);
                            return context;
                        case ExecutionState.Failed:
                            context.IncrementProcessedActions();
                            return context;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
                catch (Exception exception)
                {
                    context.IncrementProcessedActions();
                    if (!context.Configuration.ContinueOnError)
                    {
                        context.SetFailed(exception);
                        return context;
                    }

                    context.AddException(exception);
                }
            }

            context.SetCompleted();
            return context;
        }
    }
}
