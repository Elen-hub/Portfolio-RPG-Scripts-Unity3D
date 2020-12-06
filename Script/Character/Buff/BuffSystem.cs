using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BuffSystem : MonoBehaviour
{
    BaseCharacter m_character;
    List<Buff> m_buffList = new List<Buff>();

    public void Init()
    {
        m_character = GetComponent<BaseCharacter>();
    }
    public void SetBuff(Buff buff, BaseEffect effect = null)
    {
        if((buff.BuffOption & EBuffOption.Single) != 0)
        {
            for(int i =0; i<m_buffList.Count; ++i)
            {
                if (m_buffList[i].BuffOption == buff.BuffOption && m_buffList[i].BuffType == buff.BuffType && m_buffList[i].ParamsType == buff.ParamsType)
                {
                    buff.Enabled(effect);
                    m_buffList[i].Disabled();
                    m_buffList.Add(buff);
                    if (transform.tag == "Player")
                        UIMng.Instance.GetUI<Game>(UIMng.UIName.Game).CharacterWindow.EnabledBuff(buff);
                    return;
                }
            }
        }
        if ((buff.BuffOption & EBuffOption.Continue) != 0)
        {
            for (int i = 0; i < m_buffList.Count; ++i)
            {
                if (m_buffList[i].BuffOption == buff.BuffOption && m_buffList[i].BuffType == buff.BuffType && m_buffList[i].ParamsType == buff.ParamsType)
                {
                    if (!m_buffList[i].IsStart)
                        break;

                    m_buffList[i].Continue(effect);
                    return;
                }
            }
        }
        buff.Enabled(effect);
        m_buffList.Add(buff);
        if(transform.tag == "Player")
            UIMng.Instance.GetUI<Game>(UIMng.UIName.Game).CharacterWindow.EnabledBuff(buff);
    }
    public void Disabled()
    {
        foreach (Buff buff in m_buffList)
            buff.Disabled();
    }
    public Buff FindBuffType(EBuffType type)
    {
        for (int i = 0; i < m_buffList.Count; ++i)
            if (m_buffList[i].BuffType == type)
                return m_buffList[i];

        return null;
    }
    private void Update()
    {
        if (m_character.State == BaseCharacter.CharacterState.Death)
            return;

        for (int i = m_buffList.Count-1; i >= 0; --i)
        {
            if (m_buffList[i].IsStart) m_buffList[i].MoveFrame();
            else m_buffList.RemoveAt(i);
        }
    }
}
