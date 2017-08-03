using System.Threading.Tasks;
using AleFIT.Workflow.Nodes;

namespace AleFIT.Workflow
{
    public interface IWorkflow<in T> : IExecutable<T>
    {
    }
}