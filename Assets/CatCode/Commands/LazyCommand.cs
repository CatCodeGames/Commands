using System;

namespace CatCode.Commands
{
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