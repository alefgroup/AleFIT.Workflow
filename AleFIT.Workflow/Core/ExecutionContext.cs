using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using AleFIT.Workflow.Core.Configuration;

namespace AleFIT.Workflow.Core
{
    public class ExecutionContext<T>
    {
        private readonly Queue<TaskCompletionSource<ExecutionContext<T>>> _pauseCompletionSources = 
            new Queue<TaskCompletionSource<ExecutionContext<T>>>();

        public ExecutionContext(T data, IWorkflowConfiguration configuration)
        {
            Data = data;
            Configuration = configuration;
            State = ExecutionState.Running;
        }

        public T Data { get; set; }

        public IWorkflowConfiguration Configuration { get; }

        public int ProcessedActions { get; set; }

        public Exception Exception { get; set; }

        public void Continue()
        {
            if (State == ExecutionState.Paused && _pauseCompletionSources.Count > 0)
            {
                var completionSource = _pauseCompletionSources.Dequeue();
                State = ExecutionState.Running;
                completionSource.TrySetResult(this);
            }
            else
            {
                throw new InvalidOperationException("Workflow is not in Paused state or there are no actions Pause actions in queue.");
            }
        }

        internal void SetPaused(TaskCompletionSource<ExecutionContext<T>> completionSource)
        {
            State = ExecutionState.Paused;
            _pauseCompletionSources.Enqueue(completionSource);
        }

        internal void SetCompleted()
        {
            State = ExecutionState.Completed;
        }

        internal void SetFailed(Exception exception = null)
        {
            Exception = exception;
            State = ExecutionState.Failed;
        }
    }
}
