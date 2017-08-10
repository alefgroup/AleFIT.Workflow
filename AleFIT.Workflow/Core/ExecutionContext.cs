using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using AleFIT.Workflow.Core.Configuration;

namespace AleFIT.Workflow.Core
{
    public class ExecutionContext<T>
    {
        public ExecutionContext(T data, IWorkflowConfiguration configuration)
        {
            Data = data;
            Configuration = configuration;
            State = ExecutionState.Running;
            PersistedExecutionIndexes = new Stack<int>();
        }

        public T Data { get; set; }

        public IWorkflowConfiguration Configuration { get; }

        public int ProcessedActions { get; set; }

        public Exception Exception { get; set; }

        public ExecutionState State { get; private set; }

        internal Stack<int> PersistedExecutionIndexes { get; }

        internal void SetPaused()
        {
            State = ExecutionState.Paused;
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

        internal void SetRunning()
        {
            State = ExecutionState.Running;
        }
    }
}
