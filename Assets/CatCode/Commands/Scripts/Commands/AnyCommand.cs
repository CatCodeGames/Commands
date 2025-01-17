using System;
using System.Collections.Generic;

namespace CatCode.Commands
{
    /// <summary>
    /// A command that initiates multiple commands and completes when any one of them finishes.    
    /// Additionally, allows specifying what to do with the remaining unfinished commands.
    /// </summary>
    /// <summary xml:lang="ru">
    /// Команда, которая запускает несколько команд и завершается при завершении хотя бы одной из них.    
    /// Также позволяет указать, что делать с оставшимися незавершёнными командами.
    /// </summary>
    public sealed class AnyCommand : Command
    {
        private readonly List<ICommand> _commands = new();
        private Action<ICommand> _unfinishedCommandHandler = StopCommand;

        public AnyCommand(IEnumerable<ICommand> commands)
        {
            if (commands != null)
                _commands.AddRange(commands);
        }

        public AnyCommand()
        {
        }

        public AnyCommand AddCommand(ICommand command)
        {
            if (command != null)
                _commands.Add(command);
            return this;
        }

        public AnyCommand SetUnfinishedCommandHandler(Action<ICommand> unfinishedCommandHandler)
        {
            _unfinishedCommandHandler = unfinishedCommandHandler;
            return this;
        }

        protected override void OnExecute()
        {
            for (int i = 0; i < _commands.Count; i++)
                _commands[i].Execute();

            for (int i = 0; i < _commands.Count; i++)
                _commands[i].Finished += OnCommandFinished;

            for (int i = 0; i < _commands.Count; i++)
                if (_commands[i].IsFinished)
                {
                    Unsubscribe();
                    Continue();
                    return;
                }
        }

        private void Unsubscribe()
        {
            for (int i = 0; i < _commands.Count; i++)
                _commands[i].Finished -= OnCommandFinished;
        }

        protected override void OnStop()
        {
            Unsubscribe();

            for (int i = 0; i < _commands.Count; i++)
                _commands[i].Stop();
        }

        private void OnCommandFinished()
        {
            if (!HasFinihedCommand())
                return;

            Unsubscribe();

            for (int i = 0; i < _commands.Count; i++)
            {
                var command = _commands[i];
                if (!command.IsFinished)
                    _unfinishedCommandHandler?.Invoke(command);
            }

            Continue();
        }

        private bool HasFinihedCommand()
        {
            for (int i = 0; i < _commands.Count; i++)
                if (_commands[i].IsFinished)
                    return true;
            return false;
        }

        private static void StopCommand(ICommand command)
            => command.Stop();
    }
}