using System;

namespace CatCode.Commands
{
    public static class CommandExtensions
    {
        public static T AddOnFinished<T>(this T command, Action onFinished) where T : ICommand
        {
            command.Finished += onFinished;
            return command;
        }

        public static T AddOnStarted<T>(this T command, Action onFinished) where T : ICommand
        {
            command.Started += onFinished;
            return command;
        }

        public static T AddOnStopped<T>(this T command, Action onFinished) where T : ICommand
        {
            command.Stopped += onFinished;
            return command;
        }

        public static bool IsFinished(this ICommand command)
            => command.State == CommandState.Finished;
    }
}