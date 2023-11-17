
using UnityEngine;
namespace UnityGameFramework.Runtime
{
    public class SettingComponent
    {
        private static SettingComponent m_Instance = new SettingComponent();

        public static SettingComponent Instance
        {
            get
            {
                return m_Instance;
            }
        }



        public bool GetBool(string name, bool defaultValue)
        {
            return PlayerPrefs.GetInt(name, defaultValue ? 1 : 0) == 1;
        }

        public void SetBool(string name, bool cValue)
        {
            PlayerPrefs.SetInt(name, cValue ? 1 : 0);
        }


        public float GetFloat(string name, float defaultValue)
        {
            return PlayerPrefs.GetFloat(name, defaultValue);
        }

        public void SetFloat(string name, float cValue)
        {
            PlayerPrefs.SetFloat(name, cValue);
        }

    }
}