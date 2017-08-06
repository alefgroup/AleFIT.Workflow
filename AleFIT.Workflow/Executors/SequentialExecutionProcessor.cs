using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using AleFIT.Workflow.Core;

namespace AleFIT.Workflow.Executors
{
    internal class SequentialExecutionProcessor<T> : IExecutionProcessor<T>
    {
        public async Task<ExecutionContext<T>> ProcessAsync(ExecutionContext<T> context, IExecutable<T> executable)
        {
            try
            {
                return await executable.ExecuteAsync(context).ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                context.Exception = exception;
                return context;
            }
            finally
            {
                context.ProcessedActions++;
            }
        }

        public async Task<ExecutionContext<T>> ProcessAsync(ExecutionContext<T> context, IEnumerable<IExecutable<T>> executables)
        {
            foreach (var executable in executables)
            {
                try
                {
                    context = await executable.ExecuteAsync(context).ConfigureAwait(false);
                }
                catch (Exception exception)
                {
                    context.Exception = exception;
                    if (!context.Configuration.ContinueOnError)
                    {
                        break;
                    }
                }
                finally
                {
                    context.ProcessedActions++;
                }
            }

            return context;
        }
    }
}
