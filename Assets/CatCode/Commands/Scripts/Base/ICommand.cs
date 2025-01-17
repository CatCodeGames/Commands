using System;

namespace CatCode.Commands
{
    /// <summary>
    /// Interface for all commands.
    /// </summary>
    /// <summary xml:lang="ru">
    /// Интерфейс для всех команд. 
    /// </summary>
    public interface ICommand
    {
        CommandState State { get; }

        bool IsFinished { get; }

        event Action Started;
        event Action Finished;
        event Action Stopped;

        void Execute();
        void Stop();
    }
}