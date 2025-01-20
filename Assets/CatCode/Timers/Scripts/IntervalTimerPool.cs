using System;
using UnityEngine.Pool;

namespace CatCode.Timers
{
    public static class IntervalTimerPool
    {
        private static ObjectPool<IntervalTimer>[,] _pools;

        static IntervalTimerPool()
        {
            _pools = new ObjectPool<IntervalTimer>[TimerSystem.UpdateModeCount, TimerSystem.TimeModeCount];
            for (int i = 0; i < TimerSystem.UpdateModeCount; i++)
                for (int j = 0; j < TimerSystem.TimeModeCount; j++)
                {
                    var updateMode = TimerSystem.IndexToUpdateMode(i);
                    var unscaledTime = TimerSystem.IndexToTimeMode(i);
                    _pools[i, j] = new ObjectPool<IntervalTimer>(
                        createFunc: () => new IntervalTimer(1f, null, -1, updateMode, unscaledTime, false),
                        actionOnGet: timer => timer.Reset(),
                        actionOnRelease: timer => timer.Stop(),
                        actionOnDestroy: null,
                        collectionCheck: false);
                }
        }

        public static IntervalTimer Get(
            float interval,
            Action callback,
            int loops = -1,
            UpdateMode updateMode = UpdateMode.RegularUpdate,
            TimeMode timeMode = TimeMode.Scaled,
            bool multiInvokeOnUpdate = false)
        {
            var pool = GetPool(updateMode, timeMode);
            var timer = pool.Get();
            timer.SetInterval(interval)
                .SetCallback(callback)
                .SetLoops(loops);
            timer.MultiInvokeOnUpdate = multiInvokeOnUpdate;
            return timer;
        }

        public static void Release(IntervalTimer timer)
        {
            var updateMode = timer.UpdateMode;
            var timeMode = timer.TimeMode;
            var pool = GetPool(updateMode, timeMode);
            pool.Release(timer);
        }

        private static IObjectPool<IntervalTimer> GetPool(UpdateMode updateMode = UpdateMode.RegularUpdate, TimeMode timeMode = TimeMode.Scaled)
        {
            var updateModeIndex = TimerSystem.UpdateModeToIndex(updateMode);
            var unscaledTimeIndex = TimerSystem.TimeModeToIndex(timeMode);

            return _pools[updateModeIndex, unscaledTimeIndex];
        }
    }
}