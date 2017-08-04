using System;
using System.Collections.Generic;
using System.Text;

namespace AleFIT.Workflow.Test.TestData
{
    public enum SampleState
    {
        Ready,
        Running,
        Paused,
        Failed,
        Succeeded
    }

    public class EnumStateData
    {
        public EnumStateData(SampleState state)
        {
            State = state;
        }

        public SampleState State { get; }
    }
}
