using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuffIcon : MonoBehaviour
{
    Buff m_buff;
    Image m_image;
    Text m_text;
    public BuffIcon Init()
    {
        m_image = GetComponentInChildren<Image>();
        m_text = GetComponentInChildren<Text>();
        return this;
    }
    public void Enabled(Buff buff)
    {
        m_buff = buff;
        m_image.sprite = Resources.Load<Sprite>(buff.IconPath);
        gameObject.SetActive(true);
    }
    public void Disabled()
    {
        UIMng.Instance.GetUI<Game>(UIMng.UIName.Game).CharacterWindow.MemoryList.Add(this);
        gameObject.SetActive(false);
    }
    private void LateUpdate()
    {
        if(m_buff == null || !m_buff.IsStart)
        {
            Disabled();
            return;
        }
        m_text.text = (m_buff.DurationTime - m_buff.ElapsedTime).ToString("F0");
    }
}
