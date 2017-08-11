using System.Threading.Tasks;

using AleFIT.Workflow.Builders;
using AleFIT.Workflow.Core;
using AleFIT.Workflow.Core.Helpers;
using AleFIT.Workflow.Test.TestData;

using Xunit;

namespace AleFIT.Workflow.Test
{
    public class ConfigurationTests
    {
        [Fact]
        public async Task ModifyConfiguration_ShouldPassOnInnerWorkflows()
        {
            Task<ExecutionContext<GenericContext<int>>> VerifyConfig(ExecutionContext<GenericContext<int>> c)
            {
                Assert.True(c.Configuration.ContinueOnError);
                Assert.Equal(1, c.Configuration.DegreeOfParallelism);
                return Task.FromResult(c);
            }

            var workflow = WorkflowBuilder<GenericContext<int>>.Create(
                configuration =>
                    {
                        configuration.ContinueOnError = true;
                        configuration.DegreeOfParallelism = 1;
                    })
                .Do(VerifyConfig)
                .Do(WorkflowBuilder<GenericContext<int>>.Create()
                    .Do(VerifyConfig)
                    .Do(WorkflowBuilder<GenericContext<int>>.Create()
                        .Do(VerifyConfig)
                    .Build())
                .Build())
            .Build();

            var context = await workflow.ExecuteAsync(new GenericContext<int>(0));

            Assert.Equal(ExecutionState.Completed, context.State);
            Assert.Equal(5, context.ProcessedActions);
        }
        
        [Fact]
        public async Task ModifyConfigurationInInnerWorkflow_ShouldRestoreWhenCompleted()
        {
            var workflow = WorkflowBuilder<GenericContext<int>>
                .Create(configuration => { configuration.ContinueOnError = false; })
                .Do(c =>
                    {
                        Assert.False(c.Configuration.ContinueOnError);
                        return Task.FromResult(c);
                    })
                .Do(WorkflowBuilder<GenericContext<int>>
                    .Create(configuration => { configuration.ContinueOnError = true; })
                        .Do(c =>
                        {
                            Assert.True(c.Configuration.ContinueOnError);
                            return Task.FromResult(c);
                        })
                    .Build())
                .Do(c =>
                    {
                        Assert.False(c.Configuration.ContinueOnError);
                        return Task.FromResult(c);
                    })
                .Build();

            var context = await workflow.ExecuteAsync(new GenericContext<int>(0));

            Assert.Equal(0, context.Data.SampleData);
            Assert.Equal(ExecutionState.Completed, context.State);
            Assert.Equal(4, context.ProcessedActions);
        }
    }
}
