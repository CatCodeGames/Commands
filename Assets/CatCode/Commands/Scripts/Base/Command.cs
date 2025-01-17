using System;

namespace CatCode.Commands
{
    /// <summary>
    /// An abstract command that implements the core logic for execution and stopping, as well as event invocation.
    /// Used as a base class for creating specific commands.
    /// </summary>
    /// <summary xml:lang="ru">
    /// Абстрактная команда, реализующая основную логику запуска и остановки, а также вызов событий.
    /// Используется как базовый класс для создания конкретных команд.
    /// </summary>
    public abstract class Command : ICommand
    {
        private CommandState _state;

        public CommandState State => _state;
        public bool IsFinished => _state == CommandState.Finished;

        public event Action Started;
        public event Action Finished;
        public event Action Stopped;

        public void Execute()
        {
            if (_state == CommandState.Executing || _state == CommandState.Finished)
                return;
            _state = CommandState.Executing;

            Started?.Invoke();
            OnExecute();
        }

        public void Stop()
        {
            if (_state != CommandState.Executing)
                return;
            OnStop();
            _state = CommandState.Stopped;
            Stopped?.Invoke();
        }

        protected void Continue()
        {
            _state = CommandState.Finished;
            Finished?.Invoke();
        }

        protected abstract void OnExecute();
        protected abstract void OnStop();
    }
}