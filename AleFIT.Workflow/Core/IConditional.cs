using System.Threading.Tasks;

namespace AleFIT.Workflow.Core
{
    public interface IConditional<T>
    {
        Task<bool> EvaluateAsync(ExecutionContext<T> context);
    }
}
