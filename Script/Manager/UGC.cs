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
        UnityEngine.Scripting.GarbageCollector.GCMode = GarbageCollector.Mode.Enabled;
    }
    public bool UseRuntimeCollect {
        set {
            if (m_isCollect = value)
            {
                m_frameCount = 0;
                return;
            }
            Collect();
        }
    }
    void Collect()
    {
        UnityEngine.Scripting.GarbageCollector.GCMode = GarbageCollector.Mode.Enabled;
        System.GC.Collect();
        UnityEngine.Scripting.GarbageCollector.GCMode = GarbageCollector.Mode.Disabled;
    }
#if !UNITY_EDITOR
    void LateUpdate()
    {
        // 주기적으로 수집
        if (m_isCollect)
        {
            if (++m_frameCount > 240)
            {
                m_frameCount = 0;
                Collect();
            }
        }
        // 한번에 수집
        else
        {
            if (GC.GetTotalMemory(false) > 20000000)
                Collect();
        }
    }
#endif
}
