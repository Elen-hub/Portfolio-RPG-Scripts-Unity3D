using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text;

public class SubWindow_Quest : MonoBehaviour
{
    SubWindow_Quest_BTN[] m_btnList;
    public SubWindow_Quest Init()
    {
        m_btnList = GetComponentsInChildren<SubWindow_Quest_BTN>(true);
        Disabled();
        return this;
    }
    public void Enabled()
    {
        gameObject.SetActive(true);
    }
    public void Disabled()
    {
        gameObject.SetActive(false);
    }
    private void LateUpdate()
    {
        int i = 0;
        foreach(KeyValuePair<int , Quest> Quest in CharacterMng.Instance.CurrQuest)
        {
            if(m_btnList[i].GetQuest == null)
            {
                m_btnList[i].Enabled(Quest.Value);
                ++i;
                continue;
            }

            if (Quest.Value == m_btnList[i].GetQuest)
            {
                ++i;
                continue;
            }
            else
                m_btnList[i].Enabled(Quest.Value);

            ++i;
        }
        for(int j = i; j<m_btnList.Length; ++j)
            m_btnList[j].Disabled();
    }
}
