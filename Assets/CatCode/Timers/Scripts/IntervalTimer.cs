using System;


namespace CatCode.Timers
{
    public sealed partial class IntervalTimer
    {
        private readonly UpdateMode _updateMode;
        private readonly TimeMode _timeMode;

        private bool _inSystem = false;
        private bool _isActive = false;

        public float ElapsedTime = 0f;
        public float Interval = 0f;

        public int TotalLoops = -1;
        public int CompletedLoops = 0;

        public bool MultiInvokeOnUpdate = false;
        public Action Callback = null;

        public float ElapsedRatio
            => ElapsedTime / Interval;

        public UpdateMode UpdateMode => _updateMode;
        public TimeMode TimeMode => _timeMode;
        public bool IsActive => _isActive;

        public IntervalTimer(
            float interval, Action callback, int loops = -1,
            UpdateMode updateMode = UpdateMode.RegularUpdate,
            TimeMode timeMode = TimeMode.Scaled,
            bool multiInvokeOnUpdate = false)
        {
            _updateMode = updateMode;
            _timeMode = timeMode;

            Interval = interval;
            TotalLoops = loops;
            TotalLoops = loops;
            Callback = callback;
            MultiInvokeOnUpdate = multiInvokeOnUpdate;
        }

        public void Start()
        {
            if (_isActive || Callback == null)
                return;
            _isActive = true;
            if (!_inSystem)
                TimerSystem.RegisterTimer(this, _updateMode, _timeMode);
        }

        public void Stop()
        {
            if (!_isActive)
                return;
            _isActive = false;
            if (_inSystem)
                TimerSystem.ScheduleCleaningSystem(_updateMode, _timeMode);
        }

        public void Reset()
        {
            ElapsedTime = 0f;
            CompletedLoops = 0;
        }

        public IntervalTimer SetInterval(float interval)
        {
            Interval = interval;
            return this;
        }

        public IntervalTimer SetCallback(Action callback)
        {
            Callback = callback;
            return this;
        }

        public IntervalTimer SetLoops(int loops)
        {
            TotalLoops = loops;
            return this;
        }

        public IntervalTimer SetMultiInvokeOnUpdate(bool value)
        {
            MultiInvokeOnUpdate = value;
            return this;
        }
    }
}