using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum EQuestState
{
    None,
    NoAccept,
    Accept,
    PossibleClear,
    Clear,
}

public class QuestBTN : MonoBehaviour
{
    Image m_iconImg;
    Text m_mainText;
    Text m_chapterText;
    Quest m_quest;
    EQuestState m_currState;
    public QuestBTN Init()
    {
        m_iconImg = transform.Find("Img").GetComponent<Image>();
        m_mainText = transform.Find("MainName").GetComponent<Text>();
        m_chapterText = transform.Find("ChapterName").GetComponent<Text>();
        GetComponent<Button>().onClick.AddListener(OnClickQuest);
        return this;
    }
    public void Enabled(Quest quest)
    {
        m_quest = quest;
        if (quest.CurrQuest != null)
            m_chapterText.text = quest.CurrQuest.ContentName + " (" + quest.Chapter + "/" + quest.QuestDic.Count + ")";
        else
            m_chapterText.text = "완수일: " + quest.ClearDate;

        gameObject.SetActive(true);
    }
    public void Disabled()
    {
        gameObject.SetActive(false);
    }
    void OnClickQuest()
    {
        if (m_currState != EQuestState.Clear)
        {
            if (m_quest.Accept)
                UIMng.Instance.Open<QuestInformation>(UIMng.UIName.QuestInformation).Open(m_quest);
            else
                UIMng.Instance.Open<NPCUI>(UIMng.UIName.NPCUI).Enabled(m_quest);
        }
    }
    private void LateUpdate()
    {
        EQuestState state = EQuestState.NoAccept;
        if (CharacterMng.Instance.ClearQuest.ContainsValue(m_quest))
        {
            m_mainText.text = "<b>[완료]</b> " + m_quest.Name;
            state = EQuestState.Clear;
        }
        else if (CharacterMng.Instance.CurrQuest.ContainsValue(m_quest))
        {
            int ClearHandle = 0;
            m_currState = EQuestState.Accept;
            m_mainText.text = "<b>[수행중]</b> " + m_quest.Name;
            switch (m_quest.CurrQuest.Type)
            {
                case EQuestClearType.Collect:
                    ClearHandle = m_quest.CurrQuest.ClearHandle;
                    int ClearValue = m_quest.CurrQuest.ClearValue;
                    if (ItemMng.Instance.FindItemInInventory(ClearHandle) != null)
                        if (ClearValue <= (ItemMng.Instance.FindItemInInventory(ClearHandle) as IItemNumber).Number)
                        {
                            m_mainText.text = "<b><color=#FFFF00>[완료가능] </color></b>" + m_quest.Name;
                            state = EQuestState.PossibleClear;
                        }
                    break;
                case EQuestClearType.Hunt:
                    if (m_quest.CurrQuest.ClearValue <= m_quest.CurrQuest.CurrValue)
                    {
                        m_mainText.text = "<b><color=#FFFF00>[완료가능]</color></b> " + m_quest.Name;
                        state = EQuestState.PossibleClear;
                    }
                    break;
                case EQuestClearType.Talk:
                    {
                        m_mainText.text = "<b><color=#FFFF00>[완료가능]</color></b> " + m_quest.Name;
                        state = EQuestState.PossibleClear;
                    }
                    break;
            }
        }
        else
        {
            m_mainText.text = m_quest.Name;
            m_currState = EQuestState.NoAccept;
        }

        if(m_currState != state)
        {
            m_currState = state;
            m_iconImg.sprite = Resources.Load<Sprite>("Sprite/Quest_" + m_currState);
        }
    }
}
