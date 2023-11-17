using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace UnityGameFramework.Runtime
{ 
    public class GameEntry 
    {
        public static T GetComponent<T>() where T : class
        {
            string name = typeof(T).Name;
             
            switch (name)
            {
                case "SettingComponent":
                    return SettingComponent.Instance as T;
                case "DebuggerComponent":
                    return DebuggerComponent.Instance as T;
                case "BaseComponent":
                    return BaseComponent.Instance as T;
                case "ResourceComponent":
                    return ResourceComponent.Instance as T; 
                    
            }
            return null;
        }
    }
}
