using System;
using System.Threading.Tasks;

namespace AleFIT.Workflow.Core
{
    internal class ConditionallyExecutable<T> : IConditionallyExecutable<T>
    {
        private readonly Func<T, Task<bool>> _condition;
        private readonly Func<T, Task<T>> _actionIfTrue;

        public ConditionallyExecutable(Func<T, Task<bool>> condition, Func<T, Task<T>> actionIfTrue)
        {
            _condition = condition ?? throw new ArgumentNullException(nameof(condition));
            _actionIfTrue = actionIfTrue ?? throw new ArgumentNullException(nameof(actionIfTrue));
        }

        public async Task<T> ExecuteAsync(T data) => await _actionIfTrue(data).ConfigureAwait(false);

        public async Task<bool> ShouldExecuteAsync(T data) => await _condition(data).ConfigureAwait(false);
    }
}
