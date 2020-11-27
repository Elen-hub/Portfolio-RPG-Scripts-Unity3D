using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EMeshEffectType
{
    Fire,
}
public class EffectMng : TSingleton<EffectMng>
{
    Dictionary<string, Stack<BaseMissile>> m_missileMemoryDic = new Dictionary<string, Stack<BaseMissile>>();
    Dictionary<string, MemoryPool<BaseEffect>> m_effectMemoryPool = new Dictionary<string, MemoryPool<BaseEffect>>();
    public BaseEffect FindEffect(string effectPath, Transform effectPivot, float time)
    {
        if (!m_effectMemoryPool.ContainsKey(effectPath))
            m_effectMemoryPool.Add(effectPath, new MemoryPool<BaseEffect>().Init());

        MemoryPool<BaseEffect> pool = m_effectMemoryPool[effectPath];
        BaseEffect e = m_effectMemoryPool[effectPath].GetItem();
        if(!e) e = Instantiate(Resources.Load<BaseEffect>("Effect/" + effectPath), transform).Init(pool.Register, time);
        e.Enabled(effectPivot);
        e.ResetTargetTime = time;
        return e;
    }
    public BaseEffect FindEffect(string effectPath, Vector3 effectPos, Vector3 eulerAngle, float time)
    {
        if (!m_effectMemoryPool.ContainsKey(effectPath))
            m_effectMemoryPool.Add(effectPath, new MemoryPool<BaseEffect>().Init());

        MemoryPool<BaseEffect> pool = m_effectMemoryPool[effectPath];
        BaseEffect e = m_effectMemoryPool[effectPath].GetItem();
        if(!e) e = Instantiate(Resources.Load<BaseEffect>("Effect/" + effectPath), transform).Init(pool.Register, time);
        e.Enabled(effectPos, eulerAngle);
        e.ResetTargetTime = time;
        return e;
    }
    public PSMeshRendererUpdater FindMeshEffect(Transform target, EMeshEffectType type)
    {
        if (target == null)
            return null;

        PSMeshRendererUpdater renderer = target.GetComponentInChildren<PSMeshRendererUpdater>();
        if (renderer != null)
            Destroy(renderer);

        renderer = Instantiate(Resources.Load<PSMeshRendererUpdater>("Effect/Mesh/" + type), target);
        renderer.transform.localPosition = Vector3.zero;
        renderer.transform.localRotation = Quaternion.identity;
        renderer.UpdateMeshEffect(target.gameObject);

        return renderer;
    }
    public T FindMissile<T>(string missilePath, float speed) where T : BaseMissile
    {
        if (!m_missileMemoryDic.ContainsKey(missilePath))
            m_missileMemoryDic.Add(missilePath, new Stack<BaseMissile>());

        Stack<BaseMissile> missileStack = m_missileMemoryDic[missilePath];
        BaseMissile e = null;

        if (missileStack.Count > 0)
            e = missileStack.Pop();
        else
            e = Instantiate(Resources.Load<BaseMissile>("Missile/" + missilePath), transform).Init(ref missileStack, speed);

        return e as T;
    }
    public override void Init()
    {
        IsLoad = true;
    }
}
