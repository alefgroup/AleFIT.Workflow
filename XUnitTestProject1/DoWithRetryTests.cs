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
    public class DoWithRetryTests
    {
        [Fact]
        public async Task DoWithRetry_NoFailedActions_ShouldFinishNormally()
        {
            var workflow = WorkflowBuilder<GenericContext<int>>.Create()
                .DoWithRetry(int.MaxValue, new IncrementNode())
                .Build();

            var context = await workflow.ExecuteAsync(new GenericContext<int>(1));

            Assert.Equal(ExecutionState.Completed, context.State);
            Assert.Equal(2, context.ProcessedActions);
            Assert.Equal(2, context.Data.SampleData);
        }

        [Fact]
        public void DoWithRetry_NegativeRetryCount_ShouldThrowAnError()
        {
            Assert.Throws<ArgumentException>(
                () =>
                    {
                        WorkflowBuilder<GenericContext<int>>.Create()
                        .DoWithRetry(-1, new IncrementNode())
                        .Build();
                    });
        }

        [Fact]
        public async Task DoWithRetry_ZeroRetryCount_ShouldNotRetryExecution()
        {
            var workflow = WorkflowBuilder<GenericContext<int>>.Create()
                .DoWithRetry(0, new ThrowExceptionNode<GenericContext<int>>())
                .Build();

            var context = await workflow.ExecuteAsync(new GenericContext<int>(1));

            Assert.Equal(ExecutionState.Failed, context.State);
            Assert.Equal(2, context.ProcessedActions);
            Assert.Equal(1, context.Data.SampleData);
        }

        [Fact]
        public async Task DoWithRetry_FiveRetryCounts_SucceedAfterFour_ShouldSucceedOnLastRetry()
        {
            var workflow = WorkflowBuilder<GenericContext<int>>.Create()
                .DoWithRetry(5, new SucceedAfterNumberOfExecutionsNode<GenericContext<int>>(4))
                .Build();

            var context = await workflow.ExecuteAsync(new GenericContext<int>(1));

            Assert.Equal(ExecutionState.Completed, context.State);
            Assert.Equal(6, context.ProcessedActions);
            Assert.Equal(1, context.Data.SampleData);
        }

        [Fact]
        public async Task DoWithRetry_FiveRetryCounts_SucceedAfterSix_ShouldFail()
        {
            var workflow = WorkflowBuilder<GenericContext<int>>.Create()
                .DoWithRetry(5, new SucceedAfterNumberOfExecutionsNode<GenericContext<int>>(6))
                .Build();

            var context = await workflow.ExecuteAsync(new GenericContext<int>(1));

            Assert.Equal(ExecutionState.Failed, context.State);
            Assert.Equal(7, context.ProcessedActions);
            Assert.Equal(1, context.Data.SampleData);
        }
    }
}
