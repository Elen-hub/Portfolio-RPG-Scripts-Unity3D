using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SubWindow_Map_StaticIcon : MonoBehaviour
{
    public enum EMapStaticIconOption
    {
        BattlePortal,
        TownPortal,
    }

    Dictionary<EMapStaticIconOption, GameObject> m_iconOptionDic = new Dictionary<EMapStaticIconOption, GameObject>();
    public bool IsActive = true;
    EMapStaticIconOption m_currOption;

    public SubWindow_Map_StaticIcon Init()
    {
        m_iconOptionDic.Add(EMapStaticIconOption.BattlePortal, transform.Find("Map_Icon_BattlePortal").gameObject);
        m_iconOptionDic.Add(EMapStaticIconOption.TownPortal, transform.Find("Map_Icon_TownPortal").gameObject);
        return this;
    }
    public void Enabled(EMapStaticIconOption option, Vector3 pos)
    {
        transform.localPosition = new Vector3(pos.x * MapMng.Instance.CurrMap.CoordScaleFactorX, pos.z * MapMng.Instance.CurrMap.CoordScaleFactorY);
        m_iconOptionDic[m_currOption].SetActive(false);
        m_iconOptionDic[option].SetActive(true);
        m_currOption = option;
        gameObject.SetActive(true);
    }
    public void Disabled()
    {
        gameObject.SetActive(false);
    }
}
