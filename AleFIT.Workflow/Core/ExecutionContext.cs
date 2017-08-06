using System;

using AleFIT.Workflow.Core.Configuration;

namespace AleFIT.Workflow.Core
{
    public class ExecutionContext<T>
    {
        public ExecutionContext(T data, IWorkflowConfiguration configuration)
        {
            Data = data;
            Configuration = configuration;
        }

        public T Data { get; set; }

        public IWorkflowConfiguration Configuration { get; }

        public int ProcessedActions { get; set; }

        public Exception Exception { get; set; }
    }
}
