using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;


namespace CatCode.Timers
{
    public sealed partial class IntervalTimer
    {
        public sealed class TimerUpdateSystem
        {
            private bool _isActive = false;
            private bool _removalScheduled = false;
            private readonly List<IntervalTimer> _timers = new();
            private readonly List<IntervalTimer> _timersToAdd = new();

            public void Add(IntervalTimer timer)
            {
                if (timer.TotalLoops != -1 && timer.CompletedLoops >= timer.TotalLoops)
                    return;
                timer._inSystem = true;
                _timersToAdd.Add(timer);
                _isActive = true;
            }

            public void ScheduleInactiveTimersRemoval()
                => _removalScheduled = true;

            public void Update(float deltaTimer)
            {
                if (!_isActive)
                    return;
                UpdateTimers(deltaTimer);
                AddTimers();
                RemoveInactiveTimers();
            }

            private void AddTimers()
            {
                if (_timersToAdd.Count == 0)
                    return;
                _timers.AddRange(_timersToAdd);
                _timersToAdd.Clear();
            }

            private void RemoveInactiveTimers()
            {
                if (!_removalScheduled)
                    return;

                for (int i = _timers.Count - 1; i >= 0; i--)
                {
                    var timer = _timers[i];
                    if (timer._isActive)
                        continue;
                    _timers.RemoveAt(i);
                    timer._inSystem = false;
                }

                _removalScheduled = false;

                if (_timers.Count == 0)
                    _isActive = false;
            }


            private void UpdateTimers(float deltaTime)
            {
                for (int i = 0; i < _timers.Count; i++)
                {
                    var timer = _timers[i];
                    if (!timer._isActive)
                    {
                        _removalScheduled = true;
                        continue;
                    }

                    timer.ElapsedTime += deltaTime;
                    if (timer.ElapsedTime < timer.Interval)
                        continue;
                    if (timer.MultiInvokeOnUpdate)
                        HandleMultiInvoke(timer, deltaTime);
                    else
                        HandleSingleInvoke(timer, deltaTime);
                }
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private void HandleMultiInvoke(IntervalTimer timer, in float deltaTime)
            {
                if (timer.TotalLoops < 0)
                {
                    while (timer.ElapsedTime >= timer.Interval)
                    {
                        timer.ElapsedTime -= timer.Interval;
                        timer.CompletedLoops++;
                        timer.Callback();
                    }
                }
                else
                {
                    while (timer.ElapsedTime >= timer.Interval
                        && timer.CompletedLoops < timer.TotalLoops)
                    {
                        timer.ElapsedTime -= timer.Interval;
                        timer.CompletedLoops++;
                        timer.Callback();
                    }

                    if (timer.CompletedLoops >= timer.TotalLoops)
                    {
                        timer._isActive = false;
                        _removalScheduled = true;
                    }
                }
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private void HandleSingleInvoke(IntervalTimer timer, float deltaTime)
            {
                timer.ElapsedTime -= Mathf.Max(deltaTime, timer.Interval);
                timer.CompletedLoops++;
                timer.Callback();

                if (timer.TotalLoops < 0)
                    return;
                if (timer.CompletedLoops >= timer.TotalLoops)
                {
                    timer._isActive = false;
                    _removalScheduled = true;
                }
            }
        }
    }
}