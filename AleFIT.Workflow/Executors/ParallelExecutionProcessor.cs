using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using AleFIT.Workflow.Core;
using AleFIT.Workflow.Core.Helpers;

namespace AleFIT.Workflow.Executors
{
    /// <summary>
    /// Execution processor that executes actions in parallel.
    /// </summary>
    internal class ParallelExecutionProcessor<T> : IExecutionProcessor<T>
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

        public async Task<ExecutionContext<T>> ProcessAsync(
            ExecutionContext<T> context, IReadOnlyList<IExecutable<T>> executables, int executionIndex)
        {
            try
            {
                // skip actions since executionIndex and execute them in parallel with defined degree of parallelism
                await executables.Skip(executionIndex).ForEachAsync(
                    context.Configuration.DegreeOfParallelism,
                    executable => ProcessSingle(context, executable)).ConfigureAwait(false);
            }
            catch (AggregateException)
            {
                // don't register caught aggregated exception because all potential exceptions are already caught
                if (!context.Configuration.ContinueOnError)
                {
                    context.SetFailed();
                }
            }

            return context;
        }

        private async Task<ExecutionContext<T>> ProcessSingle(ExecutionContext<T> context, IExecutable<T> executable)
        {
            try
            {
                context = await executable.ExecuteAsync(context).ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                context.AddException(exception);
                if (!context.Configuration.ContinueOnError)
                {
                    throw;
                }
            }
            finally
            {
                context.IncrementProcessedActions();
            }

            return context;
        }
    }
}
