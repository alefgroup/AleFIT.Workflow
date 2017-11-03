using System;
using System.Collections.Generic;
using System.Threading;

using AleFIT.Workflow.Core.Configuration;

namespace AleFIT.Workflow.Core
{
    /// <summary>
    /// The context that is used to hold information about a specific execution.
    /// </summary>
    /// <typeparam name="T">Data that are carried through the execution.</typeparam>
    public class ExecutionContext<T>
    {
        private readonly List<Exception> _exceptions;

        private int _processedActions;

        internal ExecutionContext(T data, IWorkflowConfiguration configuration)
        {
            Data = data;
            Configuration = configuration;
            State = ExecutionState.Running;
            PersistedExecutionIndexes = new Stack<int>();
            _exceptions = new List<Exception>();
        }

        /// <summary>
        /// Data object carried through the execution.
        /// </summary>
        public T Data { get; }

        /// <summary>
        /// Configuration used during this execution.
        /// </summary>
        public IWorkflowConfiguration Configuration { get; internal set; }

        /// <summary>
        /// A number of currently processed workflow actions.
        /// </summary>
        public int ProcessedActions => _processedActions;

        /// <summary>
        /// A collection of exceptions that occurred during the execution.
        /// </summary>
        public IReadOnlyCollection<Exception> Exceptions => _exceptions;

        /// <summary>
        /// Current state of the execution.
        /// </summary>
        public ExecutionState State { get; private set; }

        /// <summary>
        /// Collection processed action indexes used to hold an execution state when the execution is paused.
        /// </summary>
        internal Stack<int> PersistedExecutionIndexes { get; }

        /// <summary>
        /// Configuration with more internally available values.
        /// </summary>
        internal IInternalWorkflowConfiguration InternalConfiguration => (IInternalWorkflowConfiguration)Configuration;

        /// <summary>
        /// Sets this context's state to <see cref="ExecutionState.Paused"/>
        /// </summary>
        internal void SetPaused()
        {
            State = ExecutionState.Paused;
        }

        /// <summary>
        /// Sets this context's state to <see cref="ExecutionState.Completed"/>
        /// </summary>
        internal void SetCompleted()
        {
            State = ExecutionState.Completed;
        }

        /// <summary>
        /// Sets this context's state to <see cref="ExecutionState.Failed"/> and registering an optional exception
        /// in <see cref="Exceptions"/> collection.
        /// </summary>
        internal void SetFailed(Exception exception = null)
        {
            _exceptions.Add(exception);
            State = ExecutionState.Failed;
        }

        /// <summary>
        /// Sets this context's state to <see cref="ExecutionState.Failed"/> and registering a collection of exceptions
        /// in <see cref="Exceptions"/> collection.
        /// </summary>
        internal void SetFailed(IEnumerable<Exception> exceptions)
        {
            _exceptions.AddRange(exceptions);
            State = ExecutionState.Failed;
        }

        /// <summary>
        /// Sets this context's state to <see cref="ExecutionState.Running"/>
        /// </summary>
        internal void SetRunning()
        {
            State = ExecutionState.Running;
        }

        /// <summary>
        /// Registers and exception in <see cref="Exceptions"/> collection.
        /// </summary>
        internal void AddException(Exception exception)
        {
            if (exception == null) throw new ArgumentNullException(nameof(exception));

            _exceptions.Add(exception);
        }

        /// <summary>
        /// Increments <see cref="ProcessedActions"/> in thread-safe way.
        /// </summary>
        internal void IncrementProcessedActions()
        {
            Interlocked.Increment(ref _processedActions);
        }
    }
}

