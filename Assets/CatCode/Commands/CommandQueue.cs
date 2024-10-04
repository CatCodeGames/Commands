using System;
using System.Collections.Generic;

namespace CatCode.Commands
{
    public enum CommandAddMode
    {
        Last,
        Next
    }

    public class CommandQueue : ICommand
    {
        private readonly LinkedList<ICommand> _commands = new();

        private CommandState _state;
        private bool _executeOnAdd;

        public event Action Started;
        public event Action Finished;
        public event Action Stopped;

        private ICommand _executingCommand = null;

        public CommandState State => _state;
        public bool IsFinished => _state == CommandState.Finished;

        public bool ExecuteOnAdd { get => _executeOnAdd; set => _executeOnAdd = value; }

        public CommandQueue SetExecuteOnAdd(bool value)
        {
            _executeOnAdd = value;
            return this;
        }

        public CommandQueue Add(ICommand command, CommandAddMode mode = CommandAddMode.Last)
        {
            if (command == null)
                return this;

            switch (mode)
            {
                case CommandAddMode.Next: _commands.AddFirst(command); break;
                case CommandAddMode.Last: _commands.AddLast(command); break;
            }
            if (_executeOnAdd)
                Execute();
            return this;
        }

        public void Execute()
        {
            if (_executingCommand != null)
                return;

            if (_state == CommandState.Idle)
            {
                _state = CommandState.Executing;
                Started?.Invoke();
            }
            else if (_executingCommand == null && _commands.Count == 0)
            {
                _state = CommandState.Finished;
                Finished?.Invoke(); 
                return;
            }

            var command = _commands.First.Value;
            _commands.RemoveFirst();
            if (command == null)
                Execute();
            _executingCommand = command;
            command.Finished += OnCommandFinished;
            command.Execute();
        }

        public void Stop()
        {
            if (_state != CommandState.Executing)
                return;
            if (_executingCommand != null)
            {
                _executingCommand.Finished -= OnCommandFinished;
                _executingCommand.Stop();
            }
            _state = CommandState.Stopped;
            Stopped?.Invoke();
        }

        private void OnCommandFinished()
        {
            _executingCommand.Finished -= OnCommandFinished;
            _executingCommand = null;
            Execute();
        }
    }
}
