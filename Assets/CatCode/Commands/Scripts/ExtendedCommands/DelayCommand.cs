using CatCode.Timers;

namespace CatCode.Commands
{
    public sealed class DelayCommand : Command
    {
        private readonly IntervalTimer _timer;

        public DelayCommand(float interval, UpdateMode updateMode = UpdateMode.RegularUpdate, TimeMode timeMode = TimeMode.Scaled)
        {
            _timer = IntervalTimerPool.Get(interval, OnTimerElapsed, 1, updateMode, timeMode, false);
        }

        protected override void OnExecute()
        {
            _timer.Start();
        }

        protected override void OnStop()
        {
            _timer.Stop();
        }

        private void OnTimerElapsed()
        {
            _timer.Stop();
            IntervalTimerPool.Release(_timer);
            Continue();
        }
    }
}