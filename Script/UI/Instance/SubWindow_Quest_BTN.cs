using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SubWindow_Quest_BTN : MonoBehaviour
{
    Quest m_quest;
    public Quest GetQuest { get { return m_quest; } }
    EQuestState m_currState;
    Image m_icon;
    Text m_nameText;
    Text m_valueText;
    public void Awake()
    {
        m_icon = transform.Find("Icon").GetComponent<Image>();
        m_nameText = transform.Find("Name").GetComponent<Text>();
        m_valueText = transform.Find("Value").GetComponent<Text>();
        gameObject.SetActive(false);
    }
    public void Enabled(Quest quest)
    {
        m_quest = quest;
        gameObject.SetActive(true);
    }
    public void Disabled()
    {
        m_quest = null;
        gameObject.SetActive(false);
    }
    private void LateUpdate()
    {
        EQuestState state = EQuestState.NoAccept;

        if (CharacterMng.Instance.CurrQuest.ContainsValue(m_quest))
        {
            if (m_quest.CurrQuest == null)
            {
                Disabled();
                return;
            }

            int ClearHandle = m_quest.CurrQuest.ClearHandle;
            int ClearValue = m_quest.CurrQuest.ClearValue;

            m_nameText.text = "<b>[수행중] </b>" + m_quest.CurrQuest.ContentName;

            m_currState = EQuestState.Accept;
            switch (m_quest.CurrQuest.Type)
            {
                case EQuestClearType.Collect:
                    int CurrValue = 0;

                    if (ItemMng.Instance.FindItemInInventory(ClearHandle) != null)
                    {
                        CurrValue = (ItemMng.Instance.FindItemInInventory(ClearHandle) as IItemNumber).Number;

                        if (ClearValue <= CurrValue)
                        {
                            m_nameText.text = "<b><color=#FFFF00>[완료가능] </color></b>" + m_quest.CurrQuest.ContentName;
                            state = EQuestState.PossibleClear;
                        }
                    }
                    m_valueText.text = ItemMng.Instance.GetItemList[ClearHandle].Name + " " + CurrValue + "/" + ClearValue;
                    break;
                case EQuestClearType.Hunt:
                    if (m_quest.CurrQuest.ClearValue <= m_quest.CurrQuest.CurrValue)
                    {
                        m_nameText.text = "<b><color=#FFFF00>[완료가능] </color></b>" + m_quest.CurrQuest.ContentName;
                        state = EQuestState.PossibleClear;
                    }
                    m_valueText.text = CharacterMng.Instance.GetMonsterStat(ClearHandle).Name + " " + m_quest.CurrQuest.CurrValue + "/" + ClearValue;
                    break;
                case EQuestClearType.Talk:
                    state = EQuestState.PossibleClear;
                    m_nameText.text = "<b><color=#FFFF00>[완료가능] </color></b>" + m_quest.CurrQuest.ContentName;
                    m_valueText.text = CharacterMng.Instance.GetNPCStat(m_quest.CurrQuest.ClearNPCHandle).Name + "에게 말을 걸자.";
                    break;
            }
        }
        else
        {
            Disabled();
            return;
        }

        if (m_currState != state)
        {
            m_currState = state;
            m_icon.sprite = Resources.Load<Sprite>("Sprite/Quest_" + m_currState);
        }
    }
}
