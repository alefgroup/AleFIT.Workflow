using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using AleFIT.Workflow.Builders;
using AleFIT.Workflow.Core;
using AleFIT.Workflow.Test.Mocks;
using AleFIT.Workflow.Test.TestData;

using Xunit;

namespace AleFIT.Workflow.Test
{
    public class IfElseWorkflowNodeTests
    {
        [Fact]
        public async Task ConditionTrue_ShouldExecuteIfTrueAction()
        {
            var workflow = WorkflowBuilder<GenericContext<int>>.Create()
                .If(c => Task.FromResult(true), new DecrementNode(), new IncrementNode()).Build();

            var context = await workflow.ExecuteAsync(new GenericContext<int>(1));

            Assert.Equal(ExecutionState.Completed, context.State);
            Assert.Equal(0, context.Data.SampleData);
        }

        [Fact]
        public async Task ConditionFalse_ShouldExecuteIfFalseAction()
        {
            var workflow = WorkflowBuilder<GenericContext<int>>.Create()
                .If(c => Task.FromResult(false), new DecrementNode(), new IncrementNode()).Build();

            var context = await workflow.ExecuteAsync(new GenericContext<int>(1));

            Assert.Equal(ExecutionState.Completed, context.State);
            Assert.Equal(2, context.Data.SampleData);
        }

        [Fact]
        public async Task ConditionTrue_ShouldExecuteAllIfTrueActions()
        {
            var workflow = WorkflowBuilder<GenericContext<int>>.Create()
                .If(c => Task.FromResult(true), 
                    WorkflowBuilder<GenericContext<int>>.Create()
                        .Do(new IncrementNode())
                        .Do(new IncrementNode())
                        .Do(new IncrementNode())
                    .Build())
                .Else(new DecrementNode())
            .Build();

            var context = await workflow.ExecuteAsync(new GenericContext<int>(1));

            Assert.Equal(ExecutionState.Completed, context.State);
            Assert.Equal(4, context.Data.SampleData);
        }

        [Fact]
        public async Task ElseIfConditionTrue_ShouldExecuteElseIfActions()
        {
            var workflow = WorkflowBuilder<GenericContext<int>>.Create()
                .If(c => Task.FromResult(false), new IncrementNode())
                .ElseIf(c => Task.FromResult(false), new IncrementNode())
                .ElseIf(c => Task.FromResult(false), new IncrementNode())
                .ElseIf(c => Task.FromResult(true), new DecrementNode()) // only this should be executed
                .Else(new IncrementNode())
            .Build();

            var context = await workflow.ExecuteAsync(new GenericContext<int>(1));

            Assert.Equal(ExecutionState.Completed, context.State);
            Assert.Equal(0, context.Data.SampleData);
        }

        [Fact]
        public async Task ElseIf_LambdaActions_ShouldExecuteNormally()
        {
            var workflow = WorkflowBuilder<GenericContext<int>>.Create()
                .If(c => Task.FromResult(false),
                    c =>
                        {
                            c.Data.SampleData++;
                            return Task.FromResult(c);
                        }).ElseIf(
                    c => Task.FromResult(false),
                    c =>
                        {
                            c.Data.SampleData++;
                            return Task.FromResult(c);
                    }).ElseIf(
                    c => Task.FromResult(false),
                    c =>
                        {
                            c.Data.SampleData++;
                            return Task.FromResult(c);
                    }).ElseIf(
                    c => Task.FromResult(true),
                    c =>
                        {
                            c.Data.SampleData--;
                            return Task.FromResult(c);
                        }) // only this should be executed
                .Else(new IncrementNode())
            .Build();

            var context = await workflow.ExecuteAsync(new GenericContext<int>(1));

            Assert.Equal(ExecutionState.Completed, context.State);
            Assert.Equal(0, context.Data.SampleData);
        }

        [Fact]
        public async Task ElseIf_NodeCondition_ShouldExecuteNormally()
        {
            var workflow = WorkflowBuilder<GenericContext<int>>.Create().If(
                new FixedConditionalNode<GenericContext<int>>(true),
                new IncrementNode(),
                new DecrementNode()).Build();

            var context = await workflow.ExecuteAsync(new GenericContext<int>(1));

            Assert.Equal(ExecutionState.Completed, context.State);
            Assert.Equal(2, context.Data.SampleData);
        }

        [Fact]
        public async Task Else_CollectionOfActions_ShouldExecuteThemAll()
        {
            var elseNodes = new[] { new DecrementNode(), new DecrementNode(), new DecrementNode() };

            var workflow = WorkflowBuilder<GenericContext<int>>.Create()
                .If(new FixedConditionalNode<GenericContext<int>>(false), new IncrementNode()).Else(elseNodes)
                .Build();

            var context = await workflow.ExecuteAsync(new GenericContext<int>(3));

            Assert.Equal(ExecutionState.Completed, context.State);
            Assert.Equal(0, context.Data.SampleData);
        }

        [Fact]
        public async Task ElseIf_ConditionAndActionNode_ShouldExecuteNormally()
        {
            var workflow = WorkflowBuilder<GenericContext<int>>.Create()
                .If(new FixedConditionalNode<GenericContext<int>>(false), new DecrementNode())
                .ElseIf(new FixedConditionalNode<GenericContext<int>>(true), new IncrementNode())
                .Else(new DecrementNode())
                .Build();

            var context = await workflow.ExecuteAsync(new GenericContext<int>(0));

            Assert.Equal(ExecutionState.Completed, context.State);
            Assert.Equal(1, context.Data.SampleData);
        }
    }
}
