using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectPopup_Reword : SelectPopup_Base
{
    GameObject[] m_itemObj = new GameObject[4];
    Image[] m_itemImg = new Image[4];
    Text[] m_text = new Text[4];
    public override void Init()
    {
        base.Init();

        for (int i = 0; i < 4; ++i)
        {
            m_itemObj[i] = transform.Find("ItemContent").GetChild(i).gameObject;
            m_itemImg[i] = m_itemObj[i].transform.Find("ItemIcon").GetComponent<Image>();
            m_text[i] = m_itemObj[i].GetComponentInChildren<Text>();
        }

        transform.Find("Exit").GetComponent<Button>().onClick.AddListener(Exit);
        gameObject.SetActive(false);
    }
    public void Enabled(int gold = 0, params SQuestReword[] reword)
    {
        int IN = 0;
        if(gold > 0)
        {
            m_itemImg[0].sprite = Resources.Load<Sprite>("Icon/Item/Gold");
            m_text[0].text = gold.ToString();
            IN += 1;
        }
        for (int i = IN; i < 4; ++i)
        {
            if (reword.Length > i)
            {
                if (reword[i].Handle != 0)
                {
                    m_itemImg[i].sprite = Resources.Load<Sprite>(ItemMng.Instance.GetItemList[reword[i].Handle].Icon);
                    if (reword[i].Value != 0)
                        m_text[i].text = reword[i].Value.ToString();
                    else
                        m_text[i].text = null;

                    m_itemObj[i].SetActive(true);
                    continue;
                }
            }
            m_itemObj[i].SetActive(false);
        }
        gameObject.SetActive(true);
    }
    public void Disabled()
    {
        gameObject.SetActive(false);
    }
    void Exit()
    {
        UIMng.Instance.CLOSE = UIMng.UIName.SelectPopup;
    }
}
