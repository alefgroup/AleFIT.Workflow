using System.Threading.Tasks;
using AleFIT.Workflow.Builders;
using AleFIT.Workflow.Core;
using AleFIT.Workflow.Model;
using AleFIT.Workflow.Nodes;

namespace AleFIT.Workflow
{
    class Program
    {
        public async Task Process()
        {
            var builder = WorkflowBuilder<Data>.Create(config =>
                {
                    config.ContinueOnError = true;
                });

            var workflow = builder
                .Do(new Validator())
                .Do(new SaveToDatabase())
                .Do(new SendToIse())
                .If(new SuccessEvaluator(), //if
                        new ReportSuccess(), //when true
                        new ReportFailure()) //when false
                .If(context => Task.FromResult(true), WorkflowBuilder<Data>.Create()
                        .Do(Task.FromResult)
                        .Do(Task.FromResult)
                        .Do(Task.FromResult)
                    .Build(), WorkflowBuilder<Data>.Create()
                        .Do(Task.FromResult)
                        .Do(Task.FromResult)
                        .Do(Task.FromResult)
                    .Build())
                .Build();

            var result = await workflow.ExecuteAsync(new Data());
        }
    }

    public class Validator : IExecutable<ExecutionContext<Data>>
    {
        public Task<ExecutionContext<Data>> ExecuteAsync(ExecutionContext<Data> data)
        {
            throw new System.NotImplementedException();
        }
    }

    public class SaveToDatabase : IExecutable<ExecutionContext<Data>>
    {
        public Task<ExecutionContext<Data>> ExecuteAsync(ExecutionContext<Data> data)
        {
            throw new System.NotImplementedException();
        }
    }
    
    public class SendToIse : IExecutable<ExecutionContext<Data>>
    {
        public Task<ExecutionContext<Data>> ExecuteAsync(ExecutionContext<Data> data)
        {
            throw new System.NotImplementedException();
        }
    }

    public class Rollback : IExecutable<ExecutionContext<Data>>
    {
        public Task<ExecutionContext<Data>> ExecuteAsync(ExecutionContext<Data> data)
        {
            throw new System.NotImplementedException();
        }
    }

    public class SuccessEvaluator : ICondition<ExecutionContext<Data>>
    {
        public Task<bool> Evaluate(ExecutionContext<Data> context)
        {
            throw new System.NotImplementedException();
        }
    }

    public class ReportSuccess : IExecutable<ExecutionContext<Data>>
    {
        public Task<ExecutionContext<Data>> ExecuteAsync(ExecutionContext<Data> data)
        {
            throw new System.NotImplementedException();
        }
    }

    public class ReportFailure : IExecutable<ExecutionContext<Data>>
    {
        public Task<ExecutionContext<Data>> ExecuteAsync(ExecutionContext<Data> data)
        {
            throw new System.NotImplementedException();
        }
    }

    public class Data
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Ip { get; set; }
    }
}
