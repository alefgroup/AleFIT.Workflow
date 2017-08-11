using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using AleFIT.Workflow.Core;

namespace AleFIT.Workflow.Executors
{
    internal class SequentialExecutionProcessor<T> : IExecutionProcessor<T>
    {
        public async Task<ExecutionContext<T>> ProcessAsync(ExecutionContext<T> context, IReadOnlyList<IExecutable<T>> executables)
        {
            int index = 0;
            if (context.State == ExecutionState.Paused)
            {
                index = context.PersistedExecutionIndexes.Pop();
            }

            return await ProcessAsync(context, executables, index).ConfigureAwait(false);
        }

        public async Task<ExecutionContext<T>> ProcessAsync(ExecutionContext<T> context, IReadOnlyList<IExecutable<T>> executables, int executionIndex)
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
                            if (context.Configuration.ContinueOnError) continue;
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
