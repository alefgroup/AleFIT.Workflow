using System.Threading.Tasks;

namespace AleFIT.Workflow.Core
{
    public interface IExecutable<T>
    {
        Task<ExecutionContext<T>> ExecuteAsync(ExecutionContext<T> context);
    }
}
