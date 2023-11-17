
using UnityEngine;
namespace UnityGameFramework.Runtime
{
    public class ResourceComponent
    {
        private static ResourceComponent m_Instance = new ResourceComponent();

        public static ResourceComponent Instance
        {
            get
            {
                return m_Instance;
            }
        }


        public string ApplicableGameVersion = Application.version;
        public int InternalResourceVersion = 0;
    }
}