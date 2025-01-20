using System;
using System.Text;
using UnityEngine;
using UnityEngine.LowLevel;
using static UnityEngine.LowLevel.PlayerLoopSystem;


namespace CatCode
{
    public static class PlayerLoopUtils
    {
        private static StringBuilder _stringBuilder = new();

        public static void LogCurrentPlayerLoop()
        {
            var currentLoop = PlayerLoop.GetCurrentPlayerLoop();
            _stringBuilder.Clear();
            LogPlayerLoopSystem(currentLoop, 0);
            Debug.Log(_stringBuilder.ToString());
        }

        private static void LogPlayerLoopSystem(PlayerLoopSystem system, int indentLevel)
        {
            string indent = new(' ', indentLevel * 2);
            if (system.type != null)
                _stringBuilder
                    .Append(' ', indentLevel * 2)
                    .AppendLine($"{indent}{system.type}");

            if (system.subSystemList == null || system.subSystemList.Length == 0)
                return;
            foreach (var subSystem in system.subSystemList)
                LogPlayerLoopSystem(subSystem, indentLevel + 1);
        }


        public static bool AddLoopSystem<T>(Type[] _route, UpdateFunction update)
        {
            var currentLoopSystem = PlayerLoop.GetCurrentPlayerLoop();

            var loopSystem = new PlayerLoopSystem()
            {
                type = typeof(T),
                updateDelegate = update
            };

            if (AddLoopSystem(ref currentLoopSystem, _route, loopSystem))
            {
                PlayerLoop.SetPlayerLoop(currentLoopSystem);
                return true;
            }
            else
                return false;
        }

        public static bool RemoveLoopSystem<T>(Type[] _route)
        {
            var currentLoopSystem = PlayerLoop.GetCurrentPlayerLoop();
            var loopSystemType = typeof(T);
            if (RemoveLoopSystem(ref currentLoopSystem, _route, loopSystemType))
            {
                PlayerLoop.SetPlayerLoop(currentLoopSystem);
                return true;
            }
            return false;
        }

        private static bool AddLoopSystem(ref PlayerLoopSystem rootSystem, Type[] _route, PlayerLoopSystem loopSystem)
        {
            return ModifyLoopSystem(ref rootSystem, _route, (ref PlayerLoopSystem targetSystem) =>
            {
                var subSystems = targetSystem.subSystemList;
                subSystems = subSystems.InsertLast(loopSystem);
                targetSystem.subSystemList = subSystems;
            });
        }

        private static bool RemoveLoopSystem(ref PlayerLoopSystem rootSystem, Type[] _route, Type loopSystemType)
        {
            return ModifyLoopSystem(ref rootSystem, _route, (ref PlayerLoopSystem targetSystem) =>
            {
                var index = -1;
                var subSystems = targetSystem.subSystemList;
                for (int i = 0; i < subSystems.Length; i++)
                    if (subSystems[i].type == loopSystemType)
                        index = i;
                if (index == -1)
                    return;
                subSystems = subSystems.RemoveAt(index);
                targetSystem.subSystemList = subSystems;
            });
        }

        private static bool ModifyLoopSystem(ref PlayerLoopSystem rootSystem, Type[] _route, RefAction<PlayerLoopSystem> action)
        {
            var subSystems = rootSystem.subSystemList;
            ref var targetSystem = ref rootSystem;
            var flag = false;
            for (int i = 0; i < _route.Length; i++)
            {
                var targetType = _route[i];
                for (int j = 0; j < subSystems.Length; j++)
                {
                    ref var subSystem = ref subSystems[j];
                    if (subSystem.type == _route[i])
                    {
                        targetSystem = ref subSystem;
                        flag = true;
                        break;
                    }
                }
                if (!flag)
                    break;
            }
            if (flag)
                action(ref targetSystem);
            return flag;
        }
    }
}