using System;
using System.Collections.Generic;

namespace CatCode.Commands
{
    /// <summary>
    /// A class for storing and managing currently executing commands.
    /// Used to store currently executing commands,
    /// when complex command management logic is not required 
    /// but access to running commands for stopping them is needed.
    /// </summary>
    /// <summary xml:lang="ru">
    /// Класс для хранения и управления текущими выполняемыми командами.    
    /// Используется для хранения текущих выполняемых команд,
    /// когда не требуется сложная логика управления,
    /// но нужен доступ к запущенным командам для их остановки.
    /// </summary>
    public sealed class CommandHeap
    {
        private bool _isExecuting;
        private readonly HashSet<ICommand> _commands = new();
        public event Action<bool> StateChanged;

        public bool IsExecuting
        {
            get => _isExecuting;
            set
            {
                if (_isExecuting == value)
                    return;
                _isExecuting = value;
                StateChanged?.Invoke(_isExecuting);
            }
        }

        public IEnumerable<ICommand> GetExecutingCommands()
            => _commands;

        public void AddAndExecute(ICommand command)
        {
            _commands.Add(command);
            IsExecuting = true;
            command.AddOnFinished(() =>
            {
                _commands.Remove(command);
                CheckState();
            });
            command.Execute();
        }

        public void Stop()
        {
            foreach (var command in _commands)
                command.Stop();
            _commands.Clear();
            IsExecuting = false;
        }                

        private void CheckState()
        {
            IsExecuting = _commands.Count > 0;
        }
    }
}