using GameFramework.Debugger;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFramework
{
    public class GameFrameworkEntry
    {
        public static T GetModule<T>() where T : class
        {
            string name = typeof(T).Name;

            if(name.Equals("IDebuggerManager"))
                return new DebuggerManager() as T;

            return null;
        }
    }
}

