using System.Collections.Generic;

namespace CatCode.Commands
{
    /// <summary>
    /// A command that initiates multiple commands concurrently and waits for all of them to complete.    
    /// </summary>
    /// <summary xml:lang="ru">
    /// Команда, которая запускает несколько команд одновременно и ожидает их полного завершения.    
    /// </summary>
    public sealed class CommandGroup : Command
    {
        private readonly List<ICommand> _commands = new();
        
        public CommandGroup()
        {
        }

        public CommandGroup Add(ICommand command)
        {
            _commands.Add(command);
            return this;
        }

        protected override void OnExecute()
        {
            for (int i = 0; i < _commands.Count; i++)
                _commands[i].Finished += OnCommandFinished;

            for (int i = 0; i < _commands.Count; i++)
                _commands[i].Execute();
        }

        protected override void OnStop()
        {            
            for (int i = 0; i < _commands.Count; i++)
                _commands[i].Finished -= OnCommandFinished;

            for (int i = 0; i < _commands.Count; i++)
                _commands[i].Stop();
        }

        private void OnCommandFinished()
        {
            for (int i = 0; i < _commands.Count; i++)
                if (!_commands[i].IsFinished)
                    return;
            Continue();
        }
    }
}
