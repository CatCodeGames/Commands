using System;

namespace CatCode.Commands
{
    /// <summary>
    /// A command that executes an action defined by a delegate.
    /// Useful for performing simple actions within a command.
    /// </summary>

    /// <summary xml:lang="ru">
    /// Команда, выполняющая действие, определенное делегатом.
    /// Полезна для выполнения простых действий в рамках команды.
    ///</summary>
    public sealed class ActionCommand : Command
    {
        private readonly Action _action;

        public ActionCommand(Action action)
            => _action = action;

        protected override void OnExecute()
        {
            _action?.Invoke();
            Continue();
        }

        protected override void OnStop()
        {
        }
    }
}