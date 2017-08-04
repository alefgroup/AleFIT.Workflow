using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AleFIT.Workflow.Nodes;

namespace AleFIT.Workflow.Test.TestNodes
{
    public class FixedResultConditionalNode<T> : ICondition<T>
    {
        private readonly bool _result;

        public FixedResultConditionalNode(bool result)
        {
            _result = result;
        }

        public Task<bool> Evaluate(T context)
        {
            return Task.FromResult(_result);
        }
    }
}
