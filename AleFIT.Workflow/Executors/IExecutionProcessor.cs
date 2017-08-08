using System.Collections.Generic;
using System.Threading.Tasks;

using AleFIT.Workflow.Core;

namespace AleFIT.Workflow.Executors
{
    internal interface IExecutionProcessor<T>
    {
        Task<ExecutionContext<T>> ProcessAsync(ExecutionContext<T> context, IEnumerable<IExecutable<T>> executables);
    }
}
