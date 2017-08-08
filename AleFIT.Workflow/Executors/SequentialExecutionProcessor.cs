using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using AleFIT.Workflow.Core;

namespace AleFIT.Workflow.Executors
{
    internal class SequentialExecutionProcessor<T> : IExecutionProcessor<T>
    {
        public async Task<ExecutionContext<T>> ProcessAsync(ExecutionContext<T> context, IEnumerable<IExecutable<T>> executables)
        {
            foreach (var executable in executables)
            {
                try
                {
                    context = await executable.ExecuteAsync(context).ConfigureAwait(false);
                    if (context.State == ExecutionState.Paused)
                    {
                        // TODO: udrzovat index zpracovavane akce a k nemu se pak vratit
                        return context;
                    }
                }
                catch (Exception exception)
                {
                    if (!context.Configuration.ContinueOnError)
                    {
                        context.SetFailed(exception);
                        return context;
                    }

                    context.Exception = exception;
                }
                finally
                {
                    context.ProcessedActions++;
                }
            }

            context.SetCompleted();
            return context;
        }

        private void PersistExecutionState()
        {
            
        }
    }
}
