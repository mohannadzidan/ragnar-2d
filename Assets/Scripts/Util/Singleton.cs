using UnityEngine;


namespace Ragnar.Util
{
    /// <summary>
    /// Inherit from this base class to create a singleton.
    /// e.g. public class MyClassName : Singleton<MyClassName> {}
    /// </summary>
    public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        // Check to see if we're about to be destroyed.
        private static object m_Lock = new object();
        private static T m_Instance;
        /// <summary>
        /// Access singleton instance through this propriety.
        /// </summary>
        public static T current
        {
            get
            {
                lock (m_Lock)
                {
                    if (m_Instance == null && (m_Instance = (T)FindObjectOfType(typeof(T))) == null)
                    {
                        throw new System.Exception($"{typeof(T).Name} not found in the scene!");
                    }
                    return m_Instance;
                }
            }

            protected set
            {
                if (m_Instance && m_Instance != value)
                {
                    throw new System.Exception($"{typeof(T).Name} is a singleton and already instantiated! please make sure that there is only one instance of this object in the scene");
                }
                m_Instance = value;
            }
        }

        void OnDestroy()
        {
            m_Instance = null;
        }


        void OnAppQuit()
        {
            m_Instance = null;
        }
    }
}
