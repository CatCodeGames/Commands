using System;

namespace CatCode.Commands
{
    /// <summary>
    /// A command with behavior defined by execution and stopping delegates.
    /// Allows dynamically specifying the execution and stopping logic of the command.
    /// </summary>
    /// <summary xml:lang="ru">
    /// Команда с поведением, заданным делегатами выполнения и остановки.
    /// Позволяет динамически задавать логику выполнения и остановки команды.
    /// </summary>
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