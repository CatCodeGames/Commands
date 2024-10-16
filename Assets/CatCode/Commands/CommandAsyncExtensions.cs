using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace CatCode.Commands
{
    public static class CommandAsyncExtensions
    {
        public static Awaitable StartedToAwaitable(this ICommand command, CancellationToken token)
        {
            var tcs = new AwaitableCompletionSource();
            CancellationTokenRegistration ctr = default;

            if (command.State > CommandState.Idle)
                tcs.SetResult();
            else
            {
                ctr = token.Register(OnCancel);
                command.Started += OnStarted;
            }
            return tcs.Awaitable;

            void OnStarted()
            {
                command.Started -= OnStarted;
                ctr.Dispose();
                tcs.TrySetResult();
            }

            void OnCancel()
            {
                command.Started -= OnStarted;
                ctr.Dispose();
                tcs.TrySetCanceled();
            }
        }

        public static Awaitable FinishToAwaitable(this ICommand command, CancellationToken token)
        {
            var tcs = new AwaitableCompletionSource();
            CancellationTokenRegistration ctr = default;

            if (command.State == CommandState.Finished)
                tcs.SetResult();
            else
            {
                ctr = token.Register(OnCancel);
                command.Finished += OnFinished;
            }
            return tcs.Awaitable;

            void OnFinished()
            {
                command.Finished -= OnFinished;
                ctr.Dispose();
                tcs.TrySetResult();
            }
            void OnCancel()
            {
                command.Finished -= OnFinished;
                ctr.Dispose();
                tcs.TrySetCanceled();
            }
        }


        public static Task StartToTask(this ICommand command, CancellationToken token)
        {
            if (command.State > CommandState.Idle)
                return Task.CompletedTask;
            var tcs = new TaskCompletionSource<bool>();
            CancellationTokenRegistration ctr = default;
            ctr = token.Register(OnCancel);
            command.Started += OnStarted;
            return tcs.Task;

            void OnStarted()
            {
                command.Started -= OnStarted;
                ctr.Dispose();
                tcs.TrySetResult(true);
            }

            void OnCancel()
            {
                command.Started -= OnStarted;
                ctr.Dispose();
                tcs.TrySetCanceled();
            }
        }

        public static Task FinishToTask(this ICommand command, CancellationToken token)
        {
            if (command.State == CommandState.Finished)
                return Task.CompletedTask;

            var tcs = new TaskCompletionSource<bool>();
            CancellationTokenRegistration ctr = default;
            ctr = token.Register(OnCancel);
            command.Finished += OnFinished;
            return tcs.Task;

            void OnFinished()
            {
                command.Finished -= OnFinished;
                ctr.Dispose();
                tcs.TrySetResult(true);
            }
            void OnCancel()
            {
                command.Finished -= OnFinished;
                ctr.Dispose();
                tcs.TrySetCanceled();
            }
        }
    }
}