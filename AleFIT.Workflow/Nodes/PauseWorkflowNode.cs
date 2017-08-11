using System.Threading.Tasks;

using AleFIT.Workflow.Core;

namespace AleFIT.Workflow.Nodes
{
    internal class PauseWorkflowNode<T> : IExecutable<T>
    {
        public Task<ExecutionContext<T>> ExecuteAsync(ExecutionContext<T> context)
        {
            if (context.State == ExecutionState.Running)
            {
                context.SetPaused();
            }
            else
            {
                // continue when pause is executed again on already paused context
                context.SetRunning();
            }
            
            return Task.FromResult(context);
        }
    }
}
