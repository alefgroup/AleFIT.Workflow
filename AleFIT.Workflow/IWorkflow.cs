using System.Threading.Tasks;
using AleFIT.Workflow.Core;
using AleFIT.Workflow.Model;

namespace AleFIT.Workflow
{
    public interface IWorkflow<T> : IExecutable<ExecutionContext<T>>
    {
        Task<ExecutionContext<T>> ExecuteAsync(T data);
    }
}