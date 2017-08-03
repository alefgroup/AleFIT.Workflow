using System.Threading.Tasks;

namespace AleFIT.Workflow
{
    public interface IConditionallyExecutable<in T> : IExecutable<T>
    {
        Task<bool> ShouldExecuteAsync(T data);
    }
}
