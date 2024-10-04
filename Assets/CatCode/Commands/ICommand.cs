using System;

namespace CatCode.Commands
{
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