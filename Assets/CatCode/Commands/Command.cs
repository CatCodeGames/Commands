using System;

namespace CatCode.Commands
{
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