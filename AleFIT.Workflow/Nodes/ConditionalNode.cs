﻿using System;
using System.Threading.Tasks;

using AleFIT.Workflow.Core;

namespace AleFIT.Workflow.Nodes
{
    internal class ConditionalNode<T> : IConditional<T>
    {
        private readonly Func<ExecutionContext<T>, Task<bool>> _condition;

        public ConditionalNode(Func<ExecutionContext<T>, Task<bool>> condition)
        {
            _condition = condition;
        }

        public async Task<bool> EvaluateAsync(ExecutionContext<T> context) => await _condition(context).ConfigureAwait(false);
    }
}
