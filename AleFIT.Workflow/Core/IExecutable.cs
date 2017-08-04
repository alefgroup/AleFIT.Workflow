using System.Threading.Tasks;

namespace AleFIT.Workflow.Core
{
    public interface IExecutable<T>
    {
        Task<T> ExecuteAsync(T data);
    }
}
