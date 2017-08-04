using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AleFIT.Workflow
{
    internal class ConditionallyExecutable<T> : IConditionallyExecutable<T>
    {
        private readonly Func<T, Task<bool>> _condition;
        private readonly Func<T, Task> _actionIfTrue;

        public ConditionallyExecutable(Func<T, Task<bool>> condition, Func<T, Task> actionIfTrue)
        {
            _condition = condition ?? throw new ArgumentNullException(nameof(condition));
            _actionIfTrue = actionIfTrue ?? throw new ArgumentNullException(nameof(actionIfTrue));
        }

        public async Task ExecuteAsync(T data) => await _actionIfTrue(data).ConfigureAwait(false);

        public async Task<bool> ShouldExecuteAsync(T data) => await _condition(data).ConfigureAwait(false);
    }
}
