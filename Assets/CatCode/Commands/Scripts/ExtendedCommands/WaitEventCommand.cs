using System;

namespace CatCode.Commands
{
    public sealed class WaitEventCommand : Command
    {
        private readonly Action<Action> _subscribe;
        private readonly Action<Action> _unsubscribe;

        public WaitEventCommand(Action<Action> subscribe, Action<Action> unsubscribe)
        {
            _subscribe = subscribe;
            _unsubscribe = unsubscribe;
        }

        protected override void OnExecute()
        {
            _subscribe(Handler);
        }

        protected override void OnStop()
        {
            _unsubscribe(Handler);
        }

        private void Handler()
        {
            _unsubscribe(Handler);
            Continue();
        }
    }
}