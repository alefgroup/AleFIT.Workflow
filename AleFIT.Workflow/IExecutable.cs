using System.Threading.Tasks;

namespace AleFIT.Workflow
{
    public interface IExecutable<in T>
    {
        Task ExecuteAsync(T data);
    }
}
