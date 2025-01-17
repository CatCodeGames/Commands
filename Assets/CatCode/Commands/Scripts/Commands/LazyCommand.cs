using System;

namespace CatCode.Commands
{
    /// <summary>
    /// A command that creates and executes another command just before its execution.
    /// Useful when command creation needs to be deferred until the moment of execution.
    /// </summary>
    /// <summary xml:lang="ru">
    /// Команда, которая создаёт другую команду перед её выполнением.
    /// Используется, когда нужно создать команду непосредственно перед её выполнением.
    /// </summary>
    public sealed class LazyCommand : Command
    {
        private ICommand _command;
        private readonly Func<ICommand> _commandFactory;

        public LazyCommand(Func<ICommand> commandFactory)
        {
            _commandFactory = commandFactory;
        }

        protected override void OnExecute()
        {
            _command = _commandFactory();
            if (_command == null)
            {
                Continue();
                return;
            }
            _command.Finished += OnCommandFinished;
            _command.Execute();
        }

        protected override void OnStop()
        {
            if (_command == null)
                return;
            _command.Finished -= OnCommandFinished;
            _command.Stop();
        }

        private void OnCommandFinished()
        {
            _command.Finished -= OnCommandFinished;
            Continue();
        }
    }
}