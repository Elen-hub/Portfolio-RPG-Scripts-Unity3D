using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SubWindow_Map_DynamicIcon : MonoBehaviour
{
    public enum EMapIconOption
    {
        Player,
        Ally,
        Enermy,
        NPC,
    }

    Vector3 m_pos;
    Dictionary<EMapIconOption, GameObject> m_iconOptionDic = new Dictionary<EMapIconOption, GameObject>();
    public bool IsActive = true;
    EMapIconOption m_currOption;
    public BaseCharacter Target;

    public SubWindow_Map_DynamicIcon Init()
    {
        m_iconOptionDic.Add(EMapIconOption.Player, transform.Find("Map_Icon_Player").gameObject);
        m_iconOptionDic.Add(EMapIconOption.Ally, transform.Find("Map_Icon_Ally").gameObject);
        m_iconOptionDic.Add(EMapIconOption.Enermy, transform.Find("Map_Icon_Enermy").gameObject);
        m_iconOptionDic.Add(EMapIconOption.NPC, transform.Find("Map_Icon_NPC").gameObject);
        return this;
    }
    public void Enabled(EMapIconOption option, BaseCharacter character)
    {
        m_iconOptionDic[m_currOption].SetActive(false);
        m_iconOptionDic[option].SetActive(true);
        m_currOption = option;
        Target = character;
        gameObject.SetActive(true);
    }
    public void Disabled()
    {
        Target = null;
        gameObject.SetActive(false);
    }
    private void LateUpdate()
    {
        if (Target == null || Target.State == BaseCharacter.CharacterState.Death)
        {
            Disabled();
            return;
        }
        m_pos.x = Target.transform.position.x * MapMng.Instance.CurrMap.CoordScaleFactorX;
        m_pos.y = Target.transform.position.z * MapMng.Instance.CurrMap.CoordScaleFactorY;
        transform.localPosition = m_pos;
    }
}
