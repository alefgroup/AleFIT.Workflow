using System;
using System.Collections.Generic;
using System.Text;

namespace AleFIT.Workflow
{
    public class ExecutionContext<T>
    {
        internal ExecutionContext(T data)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));
            Data = data;
        }

        public T Data { get; }

        public Exception Exception { get; internal set; }

        public bool Successful { get; internal set; }
    }
}
