using System.Threading;
using System;
using UnityEngine;

namespace CatCode.Commands
{
    public sealed class WaitCommand : Command
    {
        private readonly float _seconds;
        private CancellationTokenSource _cts;

        public static WaitCommand FromSeconds(float seconds)
            => new(seconds);

        public WaitCommand(float seconds)
        {
            _seconds = seconds;
        }

        protected override void OnExecute()
        {
            _ = WaitAsync();
        }

        protected override void OnStop()
            => Cancel();

        private async Awaitable WaitAsync()
        {
            _cts = new CancellationTokenSource();
            try
            {
                await Awaitable.WaitForSecondsAsync(_seconds, _cts.Token);
                Continue();
            }
            catch (OperationCanceledException)
            { }
            finally
            {
                _cts.Dispose();
                _cts = null;
            }
        }

        private void Cancel()
        {
            if (_cts == null)
                return;
            _cts.Cancel();
            _cts.Dispose();
            _cts = null;
        }
    }
}