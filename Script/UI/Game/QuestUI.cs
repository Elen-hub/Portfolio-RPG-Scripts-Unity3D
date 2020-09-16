using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestUI : BaseUI
{
    enum EShowCurrQuestType
    {
        Accept,
        Clear,
    }

    EShowCurrQuestType m_currType;
    Animator m_animator;
    List<QuestBTN> m_questList = new List<QuestBTN>();
    Transform m_grid;
    
    Color m_greyColor = new Color(0.5882f, 0.5882f, 0.5882f);

    Text m_clearText;
    Image m_clearImg;
    Text m_acceptText;
    Image m_acceptImg;

    void SetShowType (EShowCurrQuestType type)
    { 
        switch (m_currType)
        {
            case EShowCurrQuestType.Clear:
                m_clearImg.color = Color.white;
                m_clearText.color = m_greyColor;
                break;
            case EShowCurrQuestType.Accept:
                m_acceptImg.color = Color.white;
                m_acceptText.color = m_greyColor;
                break;
        }
        m_currType = type;
        switch(m_currType)
        {
            case EShowCurrQuestType.Clear:
                m_clearImg.color = m_greyColor;
                m_clearText.color = Color.white;
                break;
            case EShowCurrQuestType.Accept:
                m_acceptImg.color = m_greyColor;
                m_acceptText.color = Color.white;
                break;
        }
        ShowQuest(m_currType);
    }
    protected override void InitUI()
    {
        transform.GetChild(0).Find("Exit").GetComponent<Button>().onClick.AddListener(Close);
        m_animator = GetComponent<Animator>();
        m_grid = GetComponentInChildren<VerticalLayoutGroup>().transform;

        Transform buttonGroup = transform.GetChild(0).Find("SelectTypeButton");
        m_acceptImg = buttonGroup.Find("AcceptType").GetComponent<Image>();
        m_acceptText = buttonGroup.Find("AcceptType").GetComponentInChildren<Text>();
        m_acceptImg.GetComponent<Button>().onClick.AddListener(()=>SetShowType(EShowCurrQuestType.Accept));
        m_clearImg = buttonGroup.Find("ClearType").GetComponent<Image>();
        m_clearText = buttonGroup.Find("ClearType").GetComponentInChildren<Text>();
        m_clearImg.GetComponent<Button>().onClick.AddListener(() => SetShowType(EShowCurrQuestType.Clear));

        UIMng.Instance.Open<QuestInformation>(UIMng.UIName.QuestInformation).Close();
    }
    public override void Open()
    {
        ShowQuest(m_currType);

        if (!transform.GetChild(0).gameObject.activeSelf)
            m_animator.Play("Open");
    }
    public override void Close()
    {
        for(int i =0; i< m_questList.Count; ++i)
            m_questList[i].Disabled();

        UIMng.Instance.CLOSE = UIMng.UIName.QuestInformation;
        m_animator.Play("Close");
    }
    void ShowQuest(EShowCurrQuestType type)
    {
        int i = 0;
        switch(type)
        {
            case EShowCurrQuestType.Accept:
                foreach(Quest quest in CharacterMng.Instance.CurrQuest.Values)
                {
                    if (i < m_questList.Count)
                    {
                        m_questList[i].Enabled(quest);
                    }
                    else
                    {
                        QuestBTN btn = Instantiate(Resources.Load<QuestBTN>("UI/Instance/QuestBTN"), m_grid).Init();
                        btn.Enabled(quest);
                        m_questList.Add(btn);
                    }

                    ++i;
                }
                break;
            case EShowCurrQuestType.Clear:
                foreach (KeyValuePair<int, Quest> quest in CharacterMng.Instance.ClearQuest)
                {
                    if (i < m_questList.Count)
                    {
                        m_questList[i].Enabled(quest.Value);
                    }
                    else
                    {
                        QuestBTN btn = Instantiate(Resources.Load<QuestBTN>("UI/Instance/QuestBTN"), m_grid).Init();
                        btn.Enabled(quest.Value);
                        m_questList.Add(btn);
                    }

                    ++i;
                }
                break;
        }

        for (int j = i; j < m_questList.Count; ++j)
            m_questList[j].Disabled();
    }
}
