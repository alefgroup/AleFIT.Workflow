using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using AleFIT.Workflow.Builders;
using AleFIT.Workflow.Core;
using AleFIT.Workflow.Core.Helpers;
using AleFIT.Workflow.Test.Mocks;
using AleFIT.Workflow.Test.TestData;

using Xunit;

namespace AleFIT.Workflow.Test
{
    public class ExceptionHandlingTests
    {
        [Fact]
        public async Task ThrowException_ContinueOnErrorTrue_ShouldProcessRest()
        {
            var workflow = WorkflowBuilder<GenericContext<int>>
                .Create(configuration => { configuration.ContinueOnError = true; }).Do(new IncrementNode())
                .Do(new ThrowExceptionNode<GenericContext<int>>()).Do(new IncrementNode()).Build();

            var context = await workflow.ExecuteAsync(new GenericContext<int>(1));

            Assert.Equal(3, context.ProcessedActions);
            Assert.Equal(ExecutionState.Completed, context.State);
            Assert.NotEmpty(context.Exceptions);
            Assert.Equal(3, context.Data.SampleData);
        }

        [Fact]
        public async Task ThrowException_ContinueOnErrorFalse_ShouldStopProcessing()
        {
            var workflow = WorkflowBuilder<GenericContext<int>>
                .Create(configuration => { configuration.ContinueOnError = false; }).Do(new IncrementNode())
                .Do(new ThrowExceptionNode<GenericContext<int>>()).Do(new IncrementNode()).Build();

            var context = await workflow.ExecuteAsync(new GenericContext<int>(1));

            Assert.Equal(2, context.ProcessedActions);
            Assert.Equal(ExecutionState.Failed, context.State);
            Assert.NotEmpty(context.Exceptions);
            Assert.Equal(2, context.Data.SampleData);
        }

        [Fact]
        public async Task ThrowInInnerWorkflow_ContinueOnErrorTrue_ShouldProcessRest()
        {
            var workflow = WorkflowBuilder<GenericContext<int>>
                .Create(configuration => { configuration.ContinueOnError = true; }).Do(new IncrementNode()).Do(
                    WorkflowBuilder<GenericContext<int>>.Create().Do(new IncrementNode())
                        .Do(new ThrowExceptionNode<GenericContext<int>>()).Do(new IncrementNode())
                        .Do(new DoNothingNode<GenericContext<int>>()).Build()).Do(new IncrementNode()).Build();

            var context = await workflow.ExecuteAsync(new GenericContext<int>(1));

            Assert.Equal(7, context.ProcessedActions);
            Assert.Equal(ExecutionState.Completed, context.State);
            Assert.NotEmpty(context.Exceptions);
            Assert.Equal(5, context.Data.SampleData);
        }

        [Fact]
        public async Task ThrowInInnerWorkflow_ContinueOnErrorFalse_ShouldStopProcessing()
        {
            var workflow = WorkflowBuilder<GenericContext<int>>
                .Create(configuration => { configuration.ContinueOnError = false; }).Do(new IncrementNode()).Do(
                    WorkflowBuilder<GenericContext<int>>.Create().Do(new IncrementNode())
                        .Do(new ThrowExceptionNode<GenericContext<int>>()).Do(new IncrementNode())
                        .Do(new DoNothingNode<GenericContext<int>>()).Build()).Do(new IncrementNode()).Build();

            var context = await workflow.ExecuteAsync(new GenericContext<int>(1));

            Assert.Equal(4, context.ProcessedActions);
            Assert.Equal(ExecutionState.Failed, context.State);
            Assert.NotEmpty(context.Exceptions);
            Assert.Equal(3, context.Data.SampleData);
        }

        [Fact]
        public async Task ThrowInInnerWorkflow_InnerHasContinueOnErrorTrue_ShouldProcessRest()
        {
            var workflow = WorkflowBuilder<GenericContext<int>>
                .Create(configuration => { configuration.ContinueOnError = false; }).Do(new IncrementNode()).Do(
                    WorkflowBuilder<GenericContext<int>>
                        .Create(configuration => { configuration.ContinueOnError = true; }).Do(new IncrementNode())
                        .Do(new ThrowExceptionNode<GenericContext<int>>()).Do(new IncrementNode())
                        .Do(new DoNothingNode<GenericContext<int>>()).Build()).Do(new IncrementNode()).Build();

            var context = await workflow.ExecuteAsync(new GenericContext<int>(1));

            Assert.Equal(7, context.ProcessedActions);
            Assert.Equal(ExecutionState.Completed, context.State);
            Assert.NotEmpty(context.Exceptions);
            Assert.Equal(5, context.Data.SampleData);
        }

        [Fact]
        public async Task ThrowInParallelNode_ContinueOnErrorFalse_ShouldNotProcessRest()
        {
            var workflow = WorkflowBuilder<GenericContext<int>>
                .Create(configuration => { configuration.ContinueOnError = false; })
                .DoInParallel(new IncrementNode(), new ThrowExceptionNode<GenericContext<int>>())
                .Do(new IncrementNode()).Do(new IncrementNode()).Build();

            var context = await workflow.ExecuteAsync(new GenericContext<int>(1));

            Assert.Equal(ExecutionState.Failed, context.State);
            Assert.NotEmpty(context.Exceptions);
            Assert.InRange(context.Data.SampleData, 1, 2);
        }

        [Fact]
        public async Task ThrowInIfNode_ShouldNotProcessEitherIfTrueOrIfFalseActions()
        {
            var workflow = WorkflowBuilder<GenericContext<int>>
                .Create(configuration => { configuration.ContinueOnError = false; }).If(
                    c => throw new Exception(),
                    new IncrementNode(),
                    new IncrementNode()).Build();

            var context = await workflow.ExecuteAsync(new GenericContext<int>(0));

            Assert.Equal(ExecutionState.Failed, context.State);
            Assert.Equal(1, context.ProcessedActions);
            Assert.Equal(0, context.Data.SampleData);
        }

        [Fact]
        public async Task SetContextAsFailed_ContinueOnErrorFalse_ShouldNotProcessRest()
        {
            var workflow = WorkflowBuilder<GenericContext<int>>
                .Create(configuration => { configuration.ContinueOnError = false; })
                .Do(c => Task.FromResult(c.AsFailed()))
                .Do(new IncrementNode())
                .Build();

            var context = await workflow.ExecuteAsync(new GenericContext<int>(0));

            Assert.Equal(ExecutionState.Failed, context.State);
            Assert.Equal(1, context.ProcessedActions);
            Assert.Equal(0, context.Data.SampleData);
        }

        [Fact]
        public async Task SetContextAsFailed_ContinueOnErrorTrue_ShouldProcessRest()
        {
            var workflow = WorkflowBuilder<GenericContext<int>>
                .Create(configuration => { configuration.ContinueOnError = true; })
                .Do(c => Task.FromResult(c.AsFailed()))
                .Do(new IncrementNode())
            .Build();

            var context = await workflow.ExecuteAsync(new GenericContext<int>(0));

            Assert.Equal(ExecutionState.Completed, context.State);
            Assert.Equal(2, context.ProcessedActions);
            Assert.Equal(1, context.Data.SampleData);
        }

        [Fact]
        public async Task SetContextAsFailed_RetryNode_ShouldRetry()
        {
            var workflow = WorkflowBuilder<GenericContext<int>>
                .Create(configuration => { configuration.ContinueOnError = false; })
                .DoWithRetry(3,
                    c =>
                        {
                            if (c.Data.SampleData < 3)
                            {
                                c.Data.SampleData++;
                                return Task.FromResult(c.AsFailed());
                            }

                            return Task.FromResult(c);
                        })
            .Build();

            var context = await workflow.ExecuteAsync(new GenericContext<int>(1));

            Assert.Equal(ExecutionState.Completed, context.State);
            Assert.Equal(4, context.ProcessedActions);
            Assert.Equal(3, context.Data.SampleData);
        }
    }
}
