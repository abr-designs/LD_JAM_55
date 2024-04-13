using UnityEngine;

namespace Utilities
{
    /// <summary>
    /// Used for session singletons that are to remain hidden(Dont' destroy object on Load)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [DefaultExecutionOrder(-10000)]
    public class HiddenSingleton<T> : MonoBehaviour where T : Object
    {
        protected internal static T Instance;
        
        private void Awake()
        {
            if (Instance != null)
            {
                Debug.LogError($"Attempted to create Multiple instances of {typeof(T).Name}");
                Destroy(gameObject);
                return;
            }

            Instance = this as T;
        }
    }
}