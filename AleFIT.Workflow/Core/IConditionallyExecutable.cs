using System.Threading.Tasks;

namespace AleFIT.Workflow.Core
{
    public interface IConditionallyExecutable<T> : IExecutable<T>
    {
        Task<bool> ShouldExecuteAsync(T data);
    }
}
