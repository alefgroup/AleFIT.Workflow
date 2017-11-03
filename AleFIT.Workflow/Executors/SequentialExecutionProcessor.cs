using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using AleFIT.Workflow.Core;

namespace AleFIT.Workflow.Executors
{
    /// <summary>
    /// Execution processor that executes actions in sequence.
    /// </summary>
    internal class SequentialExecutionProcessor<T> : IExecutionProcessor<T>
    {
        public async Task<ExecutionContext<T>> ProcessAsync(ExecutionContext<T> context, IReadOnlyList<IExecutable<T>> executables)
        {
            int index = 0;
            if (context.State == ExecutionState.Paused)
            {
                // use persisted index if execution is paused so it continues from the action where it was paused
                index = context.PersistedExecutionIndexes.Pop();
            }

            return await ProcessAsync(context, executables, index).ConfigureAwait(false);
        }

        public async Task<ExecutionContext<T>> ProcessAsync(ExecutionContext<T> context, IReadOnlyList<IExecutable<T>> executables, int executionIndex)
        {
            // iterate actions from provided index
            for (var index = executionIndex; index < executables.Count; index++)
            {
                try
                {
                    context = await executables[index].ExecuteAsync(context).ConfigureAwait(false);

                    // check state after execution
                    switch (context.State)
                    {
                        case ExecutionState.Completed:
                        case ExecutionState.Running:
                            context.IncrementProcessedActions();
                            continue;
                        case ExecutionState.Paused:
                            // persist current execution index
                            context.PersistedExecutionIndexes.Push(index);
                            return context;
                        case ExecutionState.Failed:
                            context.IncrementProcessedActions();

                            // check whether to end the execution based on configuration
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
                    
                    // just register caught exception and continue execution
                    context.AddException(exception);
                }
            }

            context.SetCompleted();
            return context;
        }
    }
}
