using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestRewordBTN : MonoBehaviour
{
    Image m_icon;
    Text m_number;
    public QuestRewordBTN Init()
    {
        m_icon = transform.Find("Icon").GetComponent<Image>();
        m_number = transform.Find("Text").GetComponent<Text>();
        return this;
    }
    public void Enabled(SQuestReword reword)
    {
        if (reword.Handle == 0)
            m_icon.sprite = Resources.Load<Sprite>("Sprite/Gold");
        else
            m_icon.sprite = Resources.Load<Sprite>(ItemMng.Instance.GetItemList[reword.Handle].Icon);

        if (reword.Value == 1)
            m_number.text = null;
        else
            m_number.text = "x"+reword.Value;

        gameObject.SetActive(true);
    }
    public void Disabled()
    {
        gameObject.SetActive(false);
    }
}
