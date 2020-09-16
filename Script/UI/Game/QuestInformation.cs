using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestInformation : BaseUI
{
    Quest m_quest;
    EQuestState m_currState;

    Image m_icon;
    Text m_nameText;
    Text m_scriptText;
    Text m_valueText;

    List<QuestRewordBTN> m_rewordList = new List<QuestRewordBTN>();
    Transform m_grid;

    GameObject m_declineBTN;
    protected override void InitUI()
    {
        transform.Find("Exit").GetComponent<Button>().onClick.AddListener(Close);
        m_icon = transform.Find("Icon").GetComponent<Image>();
        m_nameText = transform.Find("QuestName").GetComponent<Text>();
        m_scriptText = transform.Find("Scripts").GetComponent<Text>();
        m_valueText = transform.Find("ClearValue").Find("Text").GetComponent<Text>();

        Transform BTNGroup = transform.Find("BTNGroup");
        m_declineBTN = BTNGroup.Find("Decline").gameObject;
        m_declineBTN.GetComponent<Button>().onClick.AddListener
            (()=>UIMng.Instance.Open<SelectPopup>(UIMng.UIName.SelectPopup).
            NormalPopup.Enabled(OnClickRemove, "확인", null, "취소", "임무를 포기하시겠습니까?"));

        m_grid = transform.Find("RewordGrid");
    }
    public void Open(Quest quest)
    {
        m_quest = quest;
        m_nameText.text = quest.CurrQuest.ContentName;
        m_scriptText.text = string.Join("\r", quest.CurrQuest.Scripts);

        int ClearHandle = quest.CurrQuest.ClearHandle;
        int ClearValue = quest.CurrQuest.ClearValue;
        if (quest.CurrQuest != null)
        {
            for (int i = 0; i < quest.CurrQuest.Reword.Count; ++i)
            {
                if (m_rewordList.Count <= i)
                    m_rewordList.Add(Instantiate(Resources.Load<QuestRewordBTN>("UI/Instance/QuestRewordBTN"), m_grid).Init());

                m_rewordList[i].Enabled(quest.CurrQuest.Reword[i]);
            }
        }
        else
        {
            // 완료된 퀘스트라면
        }
    }
    void OnClickRemove()
    {
        NetworkMng.Instance.RequestQuestDelete(m_quest.Handle);
        Close();
    }
    public override void Open()
    {
        gameObject.SetActive(true);
    }
    public override void Close()
    {
        gameObject.SetActive(false);
        for (int i = 0; i < m_rewordList.Count; ++i)
            m_rewordList[i].Disabled();
        m_quest = null;
    }
    void LateUpdate()
    {
        EQuestState state = EQuestState.NoAccept;
        if (CharacterMng.Instance.ClearQuest.ContainsValue(m_quest))
        {
            state = EQuestState.Clear;
        }
        else if (CharacterMng.Instance.CurrQuest.ContainsValue(m_quest))
        {
            int ClearHandle = m_quest.CurrQuest.ClearHandle;
            int ClearValue = m_quest.CurrQuest.ClearValue;

            m_currState = EQuestState.Accept;
            switch (m_quest.CurrQuest.Type)
            {
                case EQuestClearType.Collect:
                    int CurrValue = 0;

                    if (ItemMng.Instance.GetItemInInventory(ClearHandle) != null)
                    {
                        CurrValue = (ItemMng.Instance.GetItemInInventory(ClearHandle) as IItemNumber).Number;

                        if (ClearValue <= CurrValue)
                            state = EQuestState.PossibleClear;
                    }
                    m_valueText.text = ItemMng.Instance.GetItemList[ClearHandle].Name + " " + CurrValue + "/" + ClearValue;
                    break;
                case EQuestClearType.Hunt:
                    if (m_quest.CurrQuest.ClearValue <= m_quest.CurrQuest.CurrValue)
                        state = EQuestState.PossibleClear;

                    m_valueText.text = CharacterMng.Instance.GetMonsterStat(ClearHandle).Name + " " + m_quest.CurrQuest.CurrValue + "/" + ClearValue;
                    break;
                case EQuestClearType.Talk:
                    state = EQuestState.PossibleClear;
                    m_valueText.text = CharacterMng.Instance.GetNPCStat(m_quest.CurrQuest.ClearNPCHandle).Name + "에게 말을 걸자.";
                    break;
            }
        }
        else
            m_currState = EQuestState.NoAccept;

        if (m_currState != state)
        {
            m_currState = state;
            m_icon.sprite = Resources.Load<Sprite>("Sprite/Quest_" + m_currState);
        }
    }
}
