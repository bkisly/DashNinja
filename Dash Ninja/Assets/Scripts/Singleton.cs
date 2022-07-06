using System.Linq;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Component
{
    private static T _instance;
    public static T Instance
    {
        get
        {
            if(_instance == null)
            {
                T[] objs = FindObjectsOfType(typeof(T)) as T[];

                if (objs.Length > 0) _instance = objs.First();
                if (objs.Length > 1) Debug.LogError($"There is more than one {typeof(T).Name} in the scene.");
                if(_instance == null)
                {
                    GameObject obj = new() { hideFlags = HideFlags.HideAndDontSave };
                    _instance = obj.AddComponent<T>();
                }
            }

            return _instance;
        }
    }
}
