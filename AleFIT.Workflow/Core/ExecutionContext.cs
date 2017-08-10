using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

using AleFIT.Workflow.Core.Configuration;

namespace AleFIT.Workflow.Core
{
    public class ExecutionContext<T>
    {
        private readonly List<Exception> _exceptions;

        private int _processedActions;

        public ExecutionContext(T data, IWorkflowConfiguration configuration)
        {
            Data = data;
            Configuration = configuration;
            State = ExecutionState.Running;
            PersistedExecutionIndexes = new Stack<int>();
            _exceptions = new List<Exception>();
        }

        public T Data { get; set; }

        public IWorkflowConfiguration Configuration { get; internal set; }

        public int ProcessedActions => _processedActions;

        public IReadOnlyCollection<Exception> Exceptions => _exceptions;

        public ExecutionState State { get; private set; }

        internal Stack<int> PersistedExecutionIndexes { get; }

        internal IInternalWorkflowConfiguration InternalConfiguration => (IInternalWorkflowConfiguration)Configuration;

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
            _exceptions.Add(exception);
            State = ExecutionState.Failed;
        }

        internal void SetFailed(IEnumerable<Exception> exceptions)
        {
            _exceptions.AddRange(exceptions);
            State = ExecutionState.Failed;
        }

        internal void SetRunning()
        {
            State = ExecutionState.Running;
        }

        internal void AddException(Exception exception)
        {
            if (exception == null) throw new ArgumentNullException(nameof(exception));

            _exceptions.Add(exception);
        }

        internal void IncrementProcessedActions()
        {
            Interlocked.Increment(ref _processedActions);
        }
    }
}

