using System;
using System.Collections.Generic;
using System.Text;

namespace AleFIT.Workflow.Test.TestData
{
    public class GenericContext<T>
    {
        public GenericContext(T sampleData)
        {
            SampleData = sampleData;
        }

        public T SampleData { get; set; }
    }
}
