using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NPCUI : BaseUI
{
    [System.Flags]
    public enum ENpcButtonOption
    {
        None = 0,
        Accept = 1,
        Cancle = 2,
        Shop = 4,
        Quest = 8,
        Complete = 16,
        Negative = 32,
        Train = 64,
    }

    Quest m_quest;

    public NPCUI_Shop ShopUI;
    public NPCUI_Quest QuestUI;
    public NPCUI_Train TrainUI;

    GameObject m_acceptBTN;
    GameObject m_cancleBTN;
    GameObject m_shopBTN;
    GameObject m_questBTN;
    GameObject m_completeBTN;
    GameObject m_negativeBTN;
    GameObject m_trainBTN;

    Queue<string> m_scriptsQueue = new Queue<string>();
    BaseNPC m_npc;
    Text m_name;
    Text m_script;
    bool m_touch;

    protected override void InitUI()
    {
        Transform ButtonGrid = transform.Find("ButtonGrid");
        m_name = transform.Find("Name").GetComponent<Text>();
        m_script = transform.Find("Script").GetComponent<Text>();
        m_acceptBTN =  ButtonGrid.Find("Accept").gameObject;
        m_acceptBTN.GetComponent<Button>().onClick.AddListener(OnClickAccept);
        m_cancleBTN = ButtonGrid.Find("Cancle").gameObject;
        m_cancleBTN.GetComponent<Button>().onClick.AddListener(OnClickCancle);
        m_shopBTN = ButtonGrid.Find("Shop").gameObject;
        m_shopBTN.GetComponent<Button>().onClick.AddListener(OnClickShop);
        m_questBTN = ButtonGrid.Find("Quest").gameObject;
        m_questBTN.GetComponent<Button>().onClick.AddListener(OnClickQuest);
        m_completeBTN = ButtonGrid.Find("Complete").gameObject;
        m_completeBTN.GetComponent<Button>().onClick.AddListener(OnClickComplete);
        m_negativeBTN = ButtonGrid.Find("Negative").gameObject;
        m_negativeBTN.GetComponent<Button>().onClick.AddListener(OnClickNegative);
        m_trainBTN = ButtonGrid.Find("Train").gameObject;
        m_trainBTN.GetComponent<Button>().onClick.AddListener(OnClickTrain);

        ShopUI = GetComponentInChildren<NPCUI_Shop>();
        ShopUI.Init();
        QuestUI = GetComponentInChildren<NPCUI_Quest>();
        QuestUI.Init();
        TrainUI = GetComponentInChildren<NPCUI_Train>();
        TrainUI.Init();
    }
    public void Enabled(Quest quest)
    {
        ShopUI.Close();
        QuestUI.Close();
        TrainUI.Close();

        m_script.text = null;
        m_touch = false;
        m_quest = quest;
        m_scriptsQueue = new Queue<string>(quest.CurrQuest.Scripts);
        StartCoroutine(ReadScript(false));
        ShowButton(ENpcButtonOption.None);
    }
    public void Enabled(BaseNPC npc)
    {
        ShowButton(ENpcButtonOption.None);
        m_npc = npc;
        m_name.text = npc.StatSystem.Name;
        m_quest = null;     // 2020 02 17 테스트추가
        m_script.text = null;
        m_touch = false;
        // 현제 진행중인 퀘스트의 클리어조건이 해당 NPC 만남이거나 클리어조건이 완료될때
        foreach (Quest Quest in CharacterMng.Instance.CurrQuest.Values)
        {
            m_name.text = npc.StatSystem.Name;
            m_script.text = null;
            m_touch = false;
            if (Quest.CurrQuest == null)
                break;
            if (Quest.CurrQuest.ClearNPCHandle == npc.Handle)
            {
                int ClearHandle = 0;
                gameObject.SetActive(true);

                switch (Quest.CurrQuest.Type)
                {
                    case EQuestClearType.Collect:
                        ClearHandle = Quest.CurrQuest.ClearHandle;
                        int ClearValue = Quest.CurrQuest.ClearValue;
                        if (ItemMng.Instance.GetItemInInventory(ClearHandle) != null)
                        {
                            if(ClearValue <= (ItemMng.Instance.GetItemInInventory(ClearHandle) as IItemNumber).Number)
                            {
                                m_quest = Quest;
                                m_scriptsQueue = new Queue<string>(Quest.CurrQuest.ClearScripts);
                                StartCoroutine(ReadScript(true));
                                return;
                            }
                        }
                        break;
                    case EQuestClearType.Hunt:
                        if(Quest.CurrQuest.ClearValue <= Quest.CurrQuest.CurrValue)
                        {
                            m_quest = Quest;
                            m_scriptsQueue = new Queue<string>(Quest.CurrQuest.ClearScripts);
                            StartCoroutine(ReadScript(true));
                            return;
                        }
                        break;
                    case EQuestClearType.Talk:
                        m_quest = Quest;
                        m_scriptsQueue = new Queue<string>(Quest.CurrQuest.ClearScripts);
                        StartCoroutine(ReadScript(true));
                        return;
                }
            }
        }
        if (npc.StatSystem.QuestList != null)
            m_questBTN.SetActive(npc.StatSystem.QuestList.Count != 0);
        ShowButton(npc.StatSystem.Option);

        string comment = npc.StatSystem.Scripts[Random.Range(0, npc.StatSystem.Scripts.Count)];
        m_scriptsQueue.Clear();
        m_scriptsQueue.Enqueue(comment);
        gameObject.SetActive(true);
        StartCoroutine(ReadScript(false));
    }
    public void ShowButton(ENpcButtonOption option)
    {
        m_acceptBTN.SetActive((option & ENpcButtonOption.Accept) != 0);
        m_cancleBTN.SetActive((option & ENpcButtonOption.Cancle) != 0);
        m_questBTN.SetActive((option & ENpcButtonOption.Quest) != 0);
        m_shopBTN.SetActive((option & ENpcButtonOption.Shop) != 0);
        m_completeBTN.SetActive((option & ENpcButtonOption.Complete) != 0);
        m_negativeBTN.SetActive((option & ENpcButtonOption.Negative) != 0);
        m_trainBTN.SetActive((option & ENpcButtonOption.Train) != 0);
    }
    public void ShowButton(ENpcOption option)
    {
        m_acceptBTN.SetActive(false);
        m_completeBTN.SetActive(false);
        m_cancleBTN.SetActive(true);
        m_negativeBTN.SetActive(false);
        m_shopBTN.SetActive((option & ENpcOption.Shop) != 0);
        m_trainBTN.SetActive((option & ENpcOption.Train) != 0);
    }
    public override void Close()
    {
        ShopUI.Close();
        QuestUI.Close();
        TrainUI.Close();
        m_scriptsQueue.Clear();
        m_quest = null;
        m_npc = null;
        gameObject.SetActive(false);
        UIMng.Instance.OPEN = UIMng.UIName.Game;
    }
    void OnClickShop()
    {
        ShopUI.Enabled(m_npc.StatSystem.ShopHandle);
        UIMng.Instance.Open<Inventory>(UIMng.UIName.Inventory).Open(true);
    }
    void OnClickQuest()
    {
        QuestUI.Enabled(m_npc.StatSystem.QuestList);
    }
    void OnClickTrain()
    {
        TrainUI.Enabled(m_npc.StatSystem.TrainHandle);
        ShopUI.Enabled(m_npc.StatSystem.ShopHandle);
    }
    IEnumerator ReadScript(bool isClear)
    {
        yield return null;

        WaitForSeconds wait = new WaitForSeconds(0.03f);

        if (m_quest == null) ShowButton(m_npc.StatSystem.Option);

        while (m_scriptsQueue.Count > 0)
        {
            string Str = m_scriptsQueue.Dequeue();
            char[] characters = Str.ToCharArray();
            for (int i = 0; i < characters.Length; ++i)
            {
                if (m_touch)
                {
                    m_script.text = Str;
                    m_touch = false;
                    break;
                }

                m_script.text += characters[i];
                yield return wait;
            }

            while(!m_touch)
                yield return wait;

            if (m_scriptsQueue.Count != 0)
            {
                m_touch = false;
                m_script.text = null;
            }
        }

        if (isClear)
            ShowButton(ENpcButtonOption.Complete | ENpcButtonOption.Negative);
        else
            if (m_quest != null)
                ShowButton(ENpcButtonOption.Accept | ENpcButtonOption.Negative);
    }
    void OnClickAccept()
    {
        if(CharacterMng.Instance.CurrQuest.Count >=10)
        {
            m_quest = null;
            SystemMessage.Instance.PushMessage(SystemMessage.MessageType.Sub, "퀘스트는 10개까지 받을 수 있습니다.");
            Enabled(m_npc);
            return;
        }
        NetworkMng.Instance.RequestQuestAccept(m_quest.Handle);
        m_quest = null;
        Enabled(m_npc);
    }
    void OnClickCancle()
    {
        Close();
    }
    void OnClickComplete()
    {
        NetworkMng.Instance.RequestQuestClear(m_quest.Handle, m_npc);
        //m_quest = null;    2020 02 17 테스트삭제
        //Enabled(m_npc);    2020 02 17 테스트삭제
    }
    void OnClickNegative()
    {
        m_quest = null;
        m_questBTN.SetActive(m_npc.StatSystem.QuestList != null);
        ShowButton(m_npc.StatSystem.Option);
    }
    private void LateUpdate()
    {
        if (Input.GetMouseButtonDown(0))
            m_touch = true;
    }
}
