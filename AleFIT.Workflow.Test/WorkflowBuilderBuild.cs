using System;
using System.Threading.Tasks;
using AleFIT.Workflow.Builders;
using AleFIT.Workflow.Model;
using AleFIT.Workflow.Test.TestData;
using AleFIT.Workflow.Test.TestNodes;
using Moq;
using Xunit;

namespace AleFIT.Workflow.Test
{
    public class WorkflowBuilderBuild
    {
        [Fact]
        public async Task Empty_ProduceEmptyWorkflow()
        {
            var workflow = WorkflowBuilder<EnumStateData>.CreateEmpty().Build();

            var data = new EnumStateData(SampleState.Ready);

            var result = await workflow.ExecuteAsync(data);

            Assert.True(result.Successful);
            Assert.Equal(data.State, result.Data.State);
        }

        [Fact]
        public async Task WithSingleDelegateNode_ProduceValidWorkflow()
        {
            var workflow = WorkflowBuilder<EnumStateData>.CreateEmpty()
                .Do(Task.FromResult)
            .Build();

            var data = new EnumStateData(SampleState.Ready);

            var result = await workflow.ExecuteAsync(data);

            Assert.True(result.Successful);
            Assert.Equal(1, result.ProcessedNodes);
            Assert.Equal(data.State, result.Data.State);
        }

        [Fact]
        public async Task WithMultipleDelegateNodes_ProduceValidWorkflow()
        {
            var workflow = WorkflowBuilder<EnumStateData>.CreateEmpty()
                .Do(Task.FromResult)
                .Do(Task.FromResult)
                .Do(Task.FromResult)
                .Do(Task.FromResult)
            .Build();

            var data = new EnumStateData(SampleState.Ready);

            var result = await workflow.ExecuteAsync(data);

            Assert.True(result.Successful);
            Assert.Equal(4, result.ProcessedNodes);
            Assert.Equal(data.State, result.Data.State);
        }

        [Fact]
        public async Task WithSingleInterfaceNode_ProduceSingleNodeWorkflow()
        {
            var data = new EnumStateData(SampleState.Ready);

            var workflow = WorkflowBuilder<EnumStateData>.CreateEmpty()
                .Do(new DoNothingNode<EnumStateData>())
            .Build();

            var result = await workflow.ExecuteAsync(data);

            Assert.True(result.Successful);
            Assert.Equal(1, result.ProcessedNodes);
            Assert.Equal(data.State, result.Data.State);
        }
        
        [Fact]
        public async Task WithMultipleInterfaceNodes_ProduceValidWorkflow()
        {
            var data = new EnumStateData(SampleState.Ready);

            var workflow = WorkflowBuilder<EnumStateData>.CreateEmpty()
                .Do(new DoNothingNode<EnumStateData>())
                .Do(new DoNothingNode<EnumStateData>())
                .Do(new DoNothingNode<EnumStateData>())
            .Build();

            var result = await workflow.ExecuteAsync(data);

            Assert.True(result.Successful);
            Assert.Equal(3, result.ProcessedNodes);
            Assert.Equal(data.State, result.Data.State);
        }

        [Fact]
        public async Task WithSingleInterfaceNodeUsedMultipleTimes_ProduceValidWorkflow()
        {
            var data = new EnumStateData(SampleState.Ready);
            var node = new DoNothingNode<EnumStateData>();

            var workflow = WorkflowBuilder<EnumStateData>.CreateEmpty()
                .Do(node)
                .Do(node)
                .Do(node)
            .Build();

            var result = await workflow.ExecuteAsync(data);

            Assert.True(result.Successful);
            Assert.Equal(3, result.ProcessedNodes);
            Assert.Equal(data.State, result.Data.State);
        }

        [Fact]
        public async Task WithSingleDelegateNodeUserMultipleTimes_ProduceValidWorkflow()
        {
            var data = new EnumStateData(SampleState.Ready);
            var node = new Func<ExecutionContext<EnumStateData>, Task<ExecutionContext<EnumStateData>>>(Task.FromResult);

            var workflow = WorkflowBuilder<EnumStateData>.CreateEmpty()
                .Do(node)
                .Do(node)
                .Do(node)
            .Build();

            var result = await workflow.ExecuteAsync(data);

            Assert.True(result.Successful);
            Assert.Equal(3, result.ProcessedNodes);
            Assert.Equal(data.State, result.Data.State);
        }

        [Fact]
        public async Task WithSingleIfDelegateNode_ProduceValidWorkflow()
        {
            var data = new EnumStateData(SampleState.Ready);

            var workflow = WorkflowBuilder<EnumStateData>.CreateEmpty()
                .If(context => Task.FromResult(true),
                    Task.FromResult,
                    Task.FromResult)
                .Build();

            var result = await workflow.ExecuteAsync(data);

            Assert.True(result.Successful);
            Assert.Equal(1, result.ProcessedNodes);
            Assert.Equal(data.State, result.Data.State);
        }

        [Fact]
        public async Task WithSingleIfInterfaceNode_ProduceValidWorkflow()
        {
            var data = new EnumStateData(SampleState.Ready);

            var workflow = WorkflowBuilder<EnumStateData>.CreateEmpty()
                .If(new FixedResultConditionalNode<ExecutionContext<EnumStateData>>(true),
                    new DoNothingNode<EnumStateData>(),
                    new DoNothingNode<EnumStateData>())
                .Build();

            var result = await workflow.ExecuteAsync(data);

            Assert.True(result.Successful);
            Assert.Equal(1, result.ProcessedNodes);
            Assert.Equal(data.State, result.Data.State);
        }

        [Fact]
        public async Task WithSingleIfInterfaceNodeUseMultipleTimes_ProduceValidWorkflow()
        {
            var data = new EnumStateData(SampleState.Ready);

            var workNode = new DoNothingNode<EnumStateData>();
            var conditionNode = new FixedResultConditionalNode<ExecutionContext<EnumStateData>>(true);

            var workflow = WorkflowBuilder<EnumStateData>.CreateEmpty()
                .If(conditionNode, workNode, workNode)
                .If(conditionNode, workNode, workNode)
                .If(conditionNode, workNode, workNode)
            .Build();

            var result = await workflow.ExecuteAsync(data);

            Assert.True(result.Successful);
            Assert.Equal(3, result.ProcessedNodes);
            Assert.Equal(data.State, result.Data.State);
        }

        [Fact]
        public async Task WithSingleIfWorkflow_ProduceValidWorkflow()
        {
            var data = new EnumStateData(SampleState.Ready);

            var conditionNode = new FixedResultConditionalNode<ExecutionContext<EnumStateData>>(true);

            var workflow = WorkflowBuilder<EnumStateData>.CreateEmpty()
                .If(conditionNode, 
                    WorkflowBuilder<EnumStateData>.CreateEmpty().Build(),
                    WorkflowBuilder<EnumStateData>.CreateEmpty().Build())
            .Build();

            var result = await workflow.ExecuteAsync(data);

            Assert.True(result.Successful);
            //if node counts as processed node, but worklows are empty
            Assert.Equal(1, result.ProcessedNodes);
            Assert.Equal(data.State, result.Data.State);
        }

        [Fact]
        public async Task WithMultipleNestedNodes_ProduceValidWorkflow()
        {

            var data = new EnumStateData(SampleState.Ready);

            var workNode = new DoNothingNode<EnumStateData>();
            var conditionNode = new FixedResultConditionalNode<ExecutionContext<EnumStateData>>(true);
            var nestedWorkflow = WorkflowBuilder<EnumStateData>.CreateEmpty()
                .Do(workNode)
                .Do(workNode)
                .Do(workNode)
                .Build();

            var workflow = WorkflowBuilder<EnumStateData>.CreateEmpty()
                .Do(workNode) //1
                .Do(workNode) //2
                .If(conditionNode, nestedWorkflow, nestedWorkflow) //6 
                .If(conditionNode, nestedWorkflow, nestedWorkflow) //10
                .Do(workNode) //11
            .Build();

            var result = await workflow.ExecuteAsync(data);

            Assert.True(result.Successful);
            Assert.Equal(9, result.ProcessedNodes);
            Assert.Equal(data.State, result.Data.State);
        }
    }
}
