using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EMeshEffectType
{
    Fire,
}
public class EffectMng : TSingleton<EffectMng>
{
    Dictionary<string, List<BaseEffect>> m_effectMemoryDic = new Dictionary<string, List<BaseEffect>>();
    Dictionary<string, List<BaseMissile>> m_missileMemoryDic = new Dictionary<string, List<BaseMissile>>();
    public BaseEffect FindEffect(string effectPath, Transform effectPivot, float time)
    {
        if (!m_effectMemoryDic.ContainsKey(effectPath))
            m_effectMemoryDic.Add(effectPath, new List<BaseEffect>());

        BaseEffect e = null;

        for (int i = 0; i < m_effectMemoryDic[effectPath].Count; ++i)
        {
            if (m_effectMemoryDic[effectPath][i].gameObject.activeSelf)
                continue;
            e = m_effectMemoryDic[effectPath][i];
            break;
        }
        if (e == null)
        {
            e = Instantiate(Resources.Load<BaseEffect>("Effect/" + effectPath), effectPivot.position, Quaternion.identity, transform);
            e.Init(time);
            m_effectMemoryDic[effectPath].Add(e);
        }
        e.Enabled(effectPivot);
        e.ResetTargetTime = time;
        return e;
    }
    public BaseEffect FindEffect(string effectPath, Vector3 effectPos, Vector3 eulerAngle, float time)
    {
        if (!m_effectMemoryDic.ContainsKey(effectPath))
            m_effectMemoryDic.Add(effectPath, new List<BaseEffect>());

        BaseEffect e = null;

        for (int i = 0; i < m_effectMemoryDic[effectPath].Count; ++i)
        {
            if (m_effectMemoryDic[effectPath][i].gameObject.activeSelf)
                continue;
            e = m_effectMemoryDic[effectPath][i];
            break;
        }
        if (e == null)
        {
            e = Instantiate(Resources.Load<BaseEffect>("Effect/" + effectPath), effectPos, Quaternion.Euler(eulerAngle), transform);
            e.Init(time);
            m_effectMemoryDic[effectPath].Add(e);
        }
        e.Enabled(effectPos, eulerAngle);
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
            m_missileMemoryDic.Add(missilePath, new List<BaseMissile>());

        BaseMissile e = null;

        for(int i =0; i<m_missileMemoryDic[missilePath].Count; ++i)
        {
            if (m_missileMemoryDic[missilePath][i].gameObject.activeSelf)
                continue;

            e = m_missileMemoryDic[missilePath][i];
            break;
        }
        if(e == null)
        {
            e = Instantiate(Resources.Load<BaseMissile>("Missile/" + missilePath), transform);
            e.Init(speed);
            m_missileMemoryDic[missilePath].Add(e);
        }
        return e as T;
    }
    public override void Init()
    {
        IsLoad = true;
    }
}
