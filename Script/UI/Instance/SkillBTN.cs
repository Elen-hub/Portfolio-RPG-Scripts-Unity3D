using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillBTN : MonoBehaviour
{
    Color m_greyColor = new Color(0.5f, 0.5f, 0.5f);
    Color m_redColor = new Color(1, 0.2f, 0.2f);
    SkillInfo m_info;
    Image m_icon;
    public SkillBTN Init()
    {
        m_icon = transform.Find("Icon").GetComponent<Image>();
        GetComponent<Button>().onClick.AddListener(OnClickInfo);
        return this;
    }
    void OnClickInfo()
    {
        
        UIMng.Instance.GetUI<Skill>(UIMng.UIName.Skill).Information.Open(m_info);
    }
    public void Open(SkillInfo info)
    {
        Sprite sprite = Resources.Load<Sprite>(info.Icon);

        bool isTrue = true;
        if (!PlayerMng.Instance.MainPlayer.Character.AttackSystem.SkillDic.ContainsKey(info.Handle))
        {
            m_icon.color = m_greyColor;
            isTrue = false;
        }

        if (PlayerMng.Instance.MainPlayer.Level < info.Level)
        {
            m_icon.color = m_redColor;
            isTrue = false;
        }

        if(isTrue)
            m_icon.color = Color.white;

        m_info = info;
        if(sprite != null)
        {
            m_icon.sprite = sprite;
            m_icon.enabled = true;
        }
        else
        {
            m_icon.sprite = null;
            m_icon.enabled = false;
        }
        m_icon.sprite = Resources.Load<Sprite>(info.Icon);
        gameObject.SetActive(true);
    }
    public void Close()
    {
        gameObject.SetActive(false);
    }
}
