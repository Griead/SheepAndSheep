
using UnityEngine;
namespace UnityGameFramework.Runtime
{
    public class BaseComponent
    {
        private static BaseComponent m_Instance = new BaseComponent();

        public static BaseComponent Instance
        {
            get
            {
                return m_Instance;
            }
        }

        public bool EditorResourceMode = false;
        public string ApplicableGameVersion = Application.version;
        public int InternalResourceVersion = 0;

    }
}