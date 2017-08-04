using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AleFIT.Workflow.Core;
using AleFIT.Workflow.Model;
using AleFIT.Workflow.Nodes;

namespace AleFIT.Workflow.Builders
{
    public class WorkflowBuilder<T> : IWorkflowBuilder<T>, IConditionalWorkflowBuilder<T>
    {
        private readonly IList<Func<ExecutionContext<T>, Task<ExecutionContext<T>>>> _workflowNodes = 
            new List<Func<ExecutionContext<T>, Task<ExecutionContext<T>>>>();
        private IFullConditionalNodeBuilder<ExecutionContext<T>> _fullConditionalBuilder;
        private readonly WorkflowConfiguration _configuration;

        private WorkflowBuilder(WorkflowConfiguration configuration)
        {
            _configuration = configuration;
        }

        public static IWorkflowBuilder<T> Create()
        {
            return new WorkflowBuilder<T>(WorkflowConfiguration.CreateDefault());
        }

        public static IWorkflowBuilder<T> Create(Action<WorkflowConfiguration> configureAction)
        {
            if (configureAction == null) throw new ArgumentNullException(nameof(configureAction));

            var configuration = WorkflowConfiguration.CreateDefault();
            configureAction(configuration);

            //clone the configuration to ensure no more changes can be propagated into it
            return new WorkflowBuilder<T>(configuration.Clone());
        }
        
        public IWorkflowBuilder<T> Do(IExecutable<ExecutionContext<T>> node)
        {
            if (node == null) throw new ArgumentNullException(nameof(node));

            return Do(node.ExecuteAsync);
        }

        public IWorkflowBuilder<T> Do(Func<ExecutionContext<T>, Task<ExecutionContext<T>>> actionToExecute)
        {
            if (actionToExecute == null) throw new ArgumentNullException(nameof(actionToExecute));

            _workflowNodes.Add(actionToExecute);
            return this;
        }

        public IWorkflowBuilder<T> If(IConditionallyExecutable<ExecutionContext<T>> condition, 
            Func<ExecutionContext<T>, Task<ExecutionContext<T>>> actionIfFalse)
        {
            if (condition == null) throw new ArgumentNullException(nameof(condition));
            if (actionIfFalse == null) throw new ArgumentNullException(nameof(actionIfFalse));

            return If(condition.ShouldExecuteAsync, condition.ExecuteAsync, actionIfFalse);
        }

        public IWorkflowBuilder<T> If(Func<ExecutionContext<T>, Task<bool>> condition, 
            IExecutable<ExecutionContext<T>> actionIfTrue, 
            IExecutable<ExecutionContext<T>> actionIfFalse)
        {
            if (condition == null) throw new ArgumentNullException(nameof(condition));
            if (actionIfTrue == null) throw new ArgumentNullException(nameof(actionIfTrue));
            if (actionIfFalse == null) throw new ArgumentNullException(nameof(actionIfFalse));

            return If(condition, actionIfTrue.ExecuteAsync, actionIfFalse.ExecuteAsync);
        }

        public IWorkflowBuilder<T> If(ICondition<ExecutionContext<T>> condition, 
            IExecutable<ExecutionContext<T>> actionIfTrue, 
            IExecutable<ExecutionContext<T>> actionIfFalse)
        {
            if (condition == null) throw new ArgumentNullException(nameof(condition));
            if (actionIfTrue == null) throw new ArgumentNullException(nameof(actionIfTrue));
            if (actionIfFalse == null) throw new ArgumentNullException(nameof(actionIfFalse));

            return If(condition.Evaluate, actionIfTrue.ExecuteAsync, actionIfFalse.ExecuteAsync);
        }

        public IWorkflowBuilder<T> If(IConditionallyExecutable<ExecutionContext<T>> trueBranch, IExecutable<ExecutionContext<T>> elseBranch)
        {
            if (trueBranch == null) throw new ArgumentNullException(nameof(trueBranch));
            if (elseBranch == null) throw new ArgumentNullException(nameof(elseBranch));

            return If(trueBranch.ShouldExecuteAsync, trueBranch.ExecuteAsync, elseBranch.ExecuteAsync);
        }

        public IWorkflowBuilder<T> If(Func<ExecutionContext<T>, Task<bool>> condition, 
            IWorkflow<T> workflowIfTrue,
            IWorkflow<T> workflowIfFalse)
        {
            if (condition == null) throw new ArgumentNullException(nameof(condition));
            if (workflowIfTrue == null) throw new ArgumentNullException(nameof(workflowIfTrue));
            if (workflowIfFalse == null) throw new ArgumentNullException(nameof(workflowIfFalse));

            return If(condition, workflowIfTrue.ExecuteAsync, workflowIfFalse.ExecuteAsync);
        }

        public IWorkflowBuilder<T> If(Func<ExecutionContext<T>, Task<bool>> condition, 
            Func<ExecutionContext<T>, Task<ExecutionContext<T>>> actionIfTrue, 
            Func<ExecutionContext<T>, Task<ExecutionContext<T>>> actionIfFalse)
        {
            if (condition == null) throw new ArgumentNullException(nameof(condition));
            if (actionIfTrue == null) throw new ArgumentNullException(nameof(actionIfTrue));
            if (actionIfFalse == null) throw new ArgumentNullException(nameof(actionIfFalse));

            var conditionalNode = ConditionalNodeBuilder<ExecutionContext<T>>
                .Create(condition, actionIfTrue)
                .Else(actionIfFalse);

            _workflowNodes.Add(conditionalNode.ExecuteAsync);
            return this;
        }

        public IConditionalWorkflowBuilder<T> If(IConditionallyExecutable<ExecutionContext<T>> node)
        {
            return If(node.ShouldExecuteAsync, node.ExecuteAsync);
        }

        public IConditionalWorkflowBuilder<T> If(Func<ExecutionContext<T>, Task<bool>> condition, 
            Func<ExecutionContext<T>, Task<ExecutionContext<T>>> actionIfTrue)
        {
            _fullConditionalBuilder = ConditionalNodeBuilder<ExecutionContext<T>>.Create(condition, actionIfTrue);
            return this;
        }

        public IConditionalWorkflowBuilder<T> ElseIf(IConditionallyExecutable<ExecutionContext<T>> node)
        {
            return ElseIf(node.ShouldExecuteAsync, node.ExecuteAsync);
        }

        public IConditionalWorkflowBuilder<T> ElseIf(Func<ExecutionContext<T>, Task<bool>> condition, 
            Func<ExecutionContext<T>, Task<ExecutionContext<T>>> actionIfTrue)
        {
            _fullConditionalBuilder.ElseIf(condition, actionIfTrue);
            return this;
        }

        public IWorkflowBuilder<T> Else(IExecutable<ExecutionContext<T>> node)
        {
            return Else(node.ExecuteAsync);
        }

        public IWorkflowBuilder<T> Else(Func<ExecutionContext<T>, Task<ExecutionContext<T>>> actionToExecute)
        {
            _workflowNodes.Add(_fullConditionalBuilder.Else(actionToExecute).ExecuteAsync);
            return this;
        }

        public IWorkflow<T> Build()
        {
            return new Workflow<T>(_workflowNodes, _configuration);
        }

    }
}
