namespace CatCode.Commands
{
    /// <summary>
    /// A wrapper for another command that manages its execution and completion.
    /// </summary>
    /// <summary xml:lang="ru">
    /// Обёртка для другой команды, которая управляет её выполнением и завершением.
    /// </summary>
    public sealed class WrapCommand : Command
    {
        private readonly ICommand _command;

        public WrapCommand(ICommand command)
        {
            _command = command;
        }

        protected override void OnExecute()
        {
            if (_command == null)
            {
                Continue();
                return;
            }
            _command.Finished += OnCommandFinished;
            _command.Execute();
        }

        private void OnCommandFinished()
        {
            _command.Finished -= OnCommandFinished;
            Continue();
        }

        protected override void OnStop()
        {
            _command.Finished -= OnCommandFinished;
            _command.Stop();
        }
    }
}