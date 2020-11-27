using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MemoryPool<T> where T : MonoBehaviour
{
    Queue<T> m_pool;
    public MemoryPool<T> Init()
    {
        m_pool = new Queue<T>();
        return this;
    }
    public void Register(T item)
    {
        m_pool.Enqueue(item);
    }
    public T GetItem()
    {
        return m_pool.Count > 0 ? m_pool.Dequeue() : null;
    }
    public void Clear()
    {
        EffectMng.Instance.StartCoroutine(IEClear());
    }
    IEnumerator IEClear()
    {
        while(m_pool.Count> 0)
        {
            yield return null;
            MonoBehaviour.DestroyImmediate(m_pool.Dequeue().gameObject);
        }
    }
}
