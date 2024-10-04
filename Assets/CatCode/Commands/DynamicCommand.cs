using System;

namespace CatCode.Commands
{
    public sealed class DynamicCommand : Command
    {
        private readonly Action<Action> _onExecute;
        private readonly Action _onStop;

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