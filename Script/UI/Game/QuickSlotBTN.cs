using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class QuickSlotBTN : MonoBehaviour
{
    int m_number;

    IQuickSlotable m_content;
    Image m_icon;

    bool m_doubleTouch;
    float m_elapsedTime;
    public QuickSlotBTN Init(int number)
    {
        GetComponent<Button>().onClick.AddListener(OnClickChange);
        m_icon = GetComponent<Image>();
        m_number = number;
        return this;
    }
    public void Open()
    {
        m_content = PlayerMng.Instance.MainPlayer.Quickslot[m_number];

        if (m_content != null)
            m_icon.sprite = Resources.Load<Sprite>(m_content.Icon);
        else // 8번 퀵슬롯은 아무것도 지정하지 않을시 기본공격으로 활성화됨.
            m_icon.sprite = Resources.Load<Sprite>("Sprite/WhilteBTN");
    }
    public void OnClickChange()
    {
        QuickSlot QuickSlot = UIMng.Instance.GetUI<QuickSlot>(UIMng.UIName.QuickSlot);

        if (!QuickSlot.IsChange)
        {
            if(!m_doubleTouch)
            {
                m_doubleTouch = true;
                m_elapsedTime = 0;
            }
            else
            {
                m_doubleTouch = false;
                NetworkMng.Instance.RequestDeleteQuickSlot(m_number);
                Open();
            }
            return;
        }

        QuickSlot.IsChange = false;

        if (QuickSlot.Content != null)
        {
            NetworkMng.Instance.RequestSetQuickSlot(m_number, QuickSlot.Content, QuickSlot.IsItem, QuickSlot.Handle);
            QuickSlot.Close();
        }
    }
    private void LateUpdate()
    {
        if (m_doubleTouch)
        {
            m_elapsedTime += Time.deltaTime;
            if(m_elapsedTime>GameSystem.DoubleTouchTime)
                m_doubleTouch = false;
        }
    }
}
