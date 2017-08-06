using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using AleFIT.Workflow.Core;

namespace AleFIT.Workflow
{
    public interface IWorkflow<T> : IExecutable<T>
    {
        Task<ExecutionContext<T>> ExecuteAsync(T data);
    }
}
