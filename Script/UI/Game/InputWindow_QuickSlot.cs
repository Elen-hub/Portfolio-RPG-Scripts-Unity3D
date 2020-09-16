using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputWindow_QuickSlot : MonoBehaviour
{
    InputWindow_QuickSlotBTN[] m_quickSlotBTN = new InputWindow_QuickSlotBTN[8];
    public void Init()
    {
        for(int i= 0; i<transform.childCount; ++i)
        {
            m_quickSlotBTN[i] = transform.GetChild(i).GetComponent<InputWindow_QuickSlotBTN>();
            m_quickSlotBTN[i].Init(i);
        }
        m_quickSlotBTN[7] = transform.parent.Find("AttackButton").GetComponent<InputWindow_QuickSlotBTN>();
        m_quickSlotBTN[7].Init(7);
    }
    public void Enabled()
    {
        for (int i = 0; i < m_quickSlotBTN.Length; ++i)
            m_quickSlotBTN[i].Enabled();
    }
    public void Disabled()
    {

    }
    public void ResetQuickSlot(int number)
    {
        m_quickSlotBTN[number].Enabled();
    }
}
