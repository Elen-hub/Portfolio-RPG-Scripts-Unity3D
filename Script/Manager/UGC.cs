using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;
using System;

public class UGC : TSingleton<UGC>
{
    bool m_isCollect;
    int m_frameCount;
    public override void Init()
    {
    }
    public bool UseRuntimeCollect {
        set {
            Collect();
            m_isCollect = value;
        }
    }
    void Collect()
    {
        m_frameCount = 0;
        UnityEngine.Scripting.GarbageCollector.GCMode = GarbageCollector.Mode.Enabled;
        System.GC.Collect();
        UnityEngine.Scripting.GarbageCollector.GCMode = GarbageCollector.Mode.Disabled;
    }
#if !UNITY_EDITOR
    const int m_maxSize = 0b10000000000 * 0b10000000000 * 0b10100;
    void LateUpdate()
    {
        if (++m_frameCount < 240)
            return;

        // 주기적으로 수집
        if (m_isCollect)
            Collect();
        // 한번에 수집
        else if (GC.GetTotalMemory(false) > m_maxSize)
        {
            Collect();
        }
    }
#endif
}
