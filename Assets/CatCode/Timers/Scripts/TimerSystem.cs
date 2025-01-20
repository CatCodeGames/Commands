using System;
using UnityEngine;
using UnityEngine.PlayerLoop;


namespace CatCode.Timers
{

    public static class TimerSystem
    {
        private static IntervalTimer.TimerUpdateSystem[,] _systems;

        public const int UpdateModeCount = 3;
        public const int TimeModeCount = 2;

        public static readonly int RegularUpdateIndex;
        public static readonly int FixedUpdateIndex;
        public static readonly int LateUpdateIndex;

        public static readonly int UnscaledTimeIndex;
        public static readonly int ScaledTimeIndex;

        static TimerSystem()
        {
            RegularUpdateIndex = (int)UpdateMode.RegularUpdate;
            FixedUpdateIndex = (int)UpdateMode.FixedUpdate;
            LateUpdateIndex = (int)UpdateMode.LateUpdate;

            ScaledTimeIndex = (int)TimeMode.Scaled;
            UnscaledTimeIndex = (int)TimeMode.Unscaled;

            _systems = new IntervalTimer.TimerUpdateSystem[UpdateModeCount, TimeModeCount];
            for (int i = 0; i < UpdateModeCount; i++)
                for (int j = 0; j < TimeModeCount; j++)
                    _systems[i, j] = new IntervalTimer.TimerUpdateSystem();
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        private static void Initializer()
        {
            PlayerLoopUtils.AddLoopSystem<IntervalTimer.TimerUpdateSystem>(new[] { typeof(Update) }, Update);
            PlayerLoopUtils.AddLoopSystem<IntervalTimer.TimerUpdateSystem>(new[] { typeof(FixedUpdate) }, FixedUpdate);
            PlayerLoopUtils.AddLoopSystem<IntervalTimer.TimerUpdateSystem>(new[] { typeof(PreLateUpdate) }, LateUpdate);
        }


        public static void RegisterTimer(IntervalTimer timer, UpdateMode updateMode, TimeMode timeMode)
        {
            var system = Get(updateMode, timeMode);
            system.Add(timer);
        }

        public static void ScheduleCleaningSystem(UpdateMode updateMode, TimeMode timeMode)
        {
            var system = Get(updateMode, timeMode);
            system.ScheduleInactiveTimersRemoval();
        }

        private static void Update()
        {
            _systems[RegularUpdateIndex, ScaledTimeIndex].Update(Time.deltaTime);
            _systems[RegularUpdateIndex, UnscaledTimeIndex].Update(Time.unscaledDeltaTime);
        }

        private static void LateUpdate()
        {
            _systems[LateUpdateIndex, ScaledTimeIndex].Update(Time.deltaTime);
            _systems[LateUpdateIndex, UnscaledTimeIndex].Update(Time.unscaledDeltaTime);
        }
        private static void FixedUpdate()
        {
            _systems[FixedUpdateIndex, ScaledTimeIndex].Update(Time.fixedDeltaTime);
            _systems[FixedUpdateIndex, UnscaledTimeIndex].Update(Time.fixedUnscaledDeltaTime);
        }

        private static IntervalTimer.TimerUpdateSystem Get(UpdateMode updateMode, TimeMode timeMode)
        {
            var updateModeIndex = UpdateModeToIndex(updateMode);
            var timeModeIndex = TimeModeToIndex(timeMode);
            return _systems[updateModeIndex, timeModeIndex];
        }


        public static int UpdateModeToIndex(UpdateMode mode)
            => (int)mode;

        public static int TimeModeToIndex(TimeMode mode)
            => (int)mode;

        public static UpdateMode IndexToUpdateMode(int index)
            => (UpdateMode)index;

        public static TimeMode IndexToTimeMode(int index)
            => (TimeMode)index;
    }
}