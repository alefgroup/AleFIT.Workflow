using System;

namespace AleFIT.Workflow.Model
{
    public class ExecutionContext<T>
    {
        internal ExecutionContext(T data)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));
            Data = data;
            Successful = true;
            ProcessedNodes = 0;
        }

        public T Data { get; }

        public Exception Exception { get; internal set; }

        public bool Successful { get; internal set; }

        public int ProcessedNodes { get; internal set; }

        internal void SetFaulted(Exception exception)
        {
            Exception = exception;
            Successful = false;
        }
    }
}
