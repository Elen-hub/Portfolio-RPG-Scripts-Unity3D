using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TSingleton <T> : MonoBehaviour {

    static T m_Instance;
    public static bool IsLoad;
    public abstract void Init();
    public static T Instance
    {
        get
        {
            if (m_Instance != null)
                return m_Instance;

            GameObject obj = new GameObject(typeof(T).ToString(), typeof(T));
            DontDestroyOnLoad(obj);
            m_Instance = obj.GetComponent<T>();

            return m_Instance;
        }
    }
}
