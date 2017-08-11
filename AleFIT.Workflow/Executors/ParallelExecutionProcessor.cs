using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using AleFIT.Workflow.Core;
using AleFIT.Workflow.Core.Helpers;

namespace AleFIT.Workflow.Executors
{
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
                await executables.Skip(executionIndex).ForEachAsync(
                    context.Configuration.DegreeOfParallelism,
                    executable => ProcessSingle(context, executable)).ConfigureAwait(false);
            }
            catch (AggregateException)
            {
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
                context = await executable.ExecuteAsync(context);
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
