using System;

namespace CatCode.Commands
{
    public sealed class DynamicCommand : Command
    {
        private readonly Action<Action> _onExecute;
        private readonly Action _onStop;

        public DynamicCommand(Action<Action> onExecute, Action onStop)
        {
            _onExecute = onExecute;
            _onStop = onStop;
        }

        protected override void OnExecute()
        {
            if (_onExecute == null)
            {
                Continue();
                return;
            }
            _onExecute(Continue);
        }

        protected override void OnStop()
        {
            _onStop?.Invoke();
        }
    }
}