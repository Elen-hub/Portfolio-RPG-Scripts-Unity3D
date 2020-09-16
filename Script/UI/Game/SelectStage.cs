using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectStage : BaseUI
{
    public bool IsStart;
    float m_targetTime;
    int m_handle = -1;
    GameObject m_startBTN;
    Text m_startText;

    Image m_mapImg;
    Text m_name;
    Text m_minLevel;
    Text m_level;
    Text m_number;
    Text m_information;

    Transform m_memberGrid;
    Transform m_stageGrid;

    List<SelectStage_MemberBTN> m_memberBTNList = new List<SelectStage_MemberBTN>();
    List<SelectStage_StageBTN> m_stageBTNList = new List<SelectStage_StageBTN>();
    protected override void InitUI()
    {
        m_stageGrid = transform.Find("StageList").GetComponentInChildren<ContentSizeFitter>().transform;
        m_memberGrid = transform.Find("MemberList").GetComponentInChildren<ContentSizeFitter>().transform;
        Transform CurrStageGrid = transform.Find("CurrStage").Find("Status");
        m_mapImg = CurrStageGrid.Find("MapImage").GetComponent<Image>();
        m_name = CurrStageGrid.Find("MapName").GetComponent<Text>();
        m_minLevel = CurrStageGrid.Find("MinLevel").GetComponent<Text>();
        m_level = CurrStageGrid.Find("Level").GetComponent<Text>();
        m_number = CurrStageGrid.Find("Number").GetComponent<Text>();
        m_information = CurrStageGrid.Find("Information").GetComponent<Text>();
        m_startBTN = transform.Find("Start").gameObject;
        m_startText = m_startBTN.GetComponentInChildren<Text>();
        m_startBTN.GetComponent<Button>().onClick.AddListener(OnClickStart);
        transform.Find("Exit").GetComponent<Button>().onClick.AddListener(Close);
        for(int i =0; i<6; ++i)
        {
            SelectStage_MemberBTN btn = Instantiate(Resources.Load<SelectStage_MemberBTN>("UI/Instance/SelectStage_MemberBTN"), m_memberGrid).Init();
            m_memberBTNList.Add(btn);
        }
    }
    public void Open(int number)
    {
        base.Open();

        IsStart = false;
        List<int> HandleList = MapMng.Instance.CurrMap.MatchPortalList[number].HandleList;
        for(int i =0; i<HandleList.Count; ++i)
        {
            if (m_stageBTNList.Count <= i)
            {
                SelectStage_StageBTN btn = Instantiate(Resources.Load<SelectStage_StageBTN>("UI/Instance/SelectStage_StageBTN"), m_stageGrid).Init();
                btn.Enabled(HandleList[i]);
                m_stageBTNList.Add(btn);
                continue;
            }
            m_stageBTNList[i].Enabled(HandleList[i]);
        }
        if (PlayerMng.Instance.CurrParty != null)
        {
            for (int i = 0; i < m_memberBTNList.Count; ++i)
            {
                if (PlayerMng.Instance.CurrParty.PartyMemberList.Count > i)
                    m_memberBTNList[i].Enabled(PlayerMng.Instance.CurrParty.PartyMemberList[i]);
            }
        }
        else
        {
            m_memberBTNList[0].Enabled(PlayerMng.Instance.MainPlayer.HostID);
        }
        SelectHandle(HandleList[0]);
        gameObject.SetActive(true);
    }
    public override void Close()
    {
        if(IsStart)
        {
            SystemMessage.Instance.PushMessage(SystemMessage.MessageType.Sub, "매칭중엔 할 수 없습니다.");
            return;
        }
        base.Close();

        for (int i = 0; i < m_memberBTNList.Count; ++i)
            m_memberBTNList[i].Disabled();
        for (int i = 0; i < m_stageBTNList.Count; ++i)
            m_stageBTNList[i].Disabled();

        gameObject.SetActive(false);
    }
    public void SelectHandle(int handle)
    {
        if(IsStart)
        {
            SystemMessage.Instance.PushMessage(SystemMessage.MessageType.Sub, "매칭중엔 할 수 없습니다.");
            return;
        }
        m_startText.text = "시작";
        m_handle = handle;
        Map map = MapMng.Instance.MapDic[handle];
        m_mapImg.sprite = Resources.Load<Sprite>(map.MapIconPath);
        m_name.text = map.MapName;
        m_minLevel.text = "최소레벨: " + map.MinLevel;
        m_level.text = "적정레벨: " + map.Level;
        m_number.text = "권장인원: " + map.Number;
        m_information.text = map.Information;
        for (int i = 0; i < m_stageBTNList.Count; ++i)
            m_stageBTNList[i].Select = handle;
    }
    void OnClickStart()
    {
        if (IsStart)
        {
            SystemMessage.Instance.PushMessage(SystemMessage.MessageType.Sub, "이미 매칭이 신청됬습니다.");
            return;
        }
        if(m_handle == -1)
        {
            SystemMessage.Instance.PushMessage(SystemMessage.MessageType.Sub, "입장할 맵을 선택해야합니다.");
            return;
        }
        if(PlayerMng.Instance.CurrParty == null || PlayerMng.Instance.CurrParty.PartyMemberList.Count == 1)
        {
            NetworkMng.Instance.RequestCharacterJoinPrivateMap(m_handle);
        }
        else
        {
            if(PlayerMng.Instance.MainPlayer.HostID != PlayerMng.Instance.CurrParty.PartyHost)
            {
                SystemMessage.Instance.PushMessage(SystemMessage.MessageType.Sub, "파티장만이 입장신청을 할 수 있습니다.");
                return;
            }
            for(int i =0; i<PlayerMng.Instance.CurrParty.PartyMemberList.Count; ++i)
            {
                if(PlayerMng.Instance.PlayerList[PlayerMng.Instance.CurrParty.PartyMemberList[i]].Character == null)
                {
                    SystemMessage.Instance.PushMessage(SystemMessage.MessageType.Sub, PlayerMng.Instance.PlayerList[PlayerMng.Instance.CurrParty.PartyMemberList[i]].Name + "님이 같은맵에 존재하지 않습니다.");
                    return;
                }
            }
            m_targetTime = 15;
            IsStart = true;
            NetworkMng.Instance.NotifyRequestJoinPrivateMap(m_handle);
        }
    }
    public void OnReady(int number)
    {
        m_memberBTNList[number].Ready(true);
    }
    public void OnMatchCancle()
    {
        IsStart = false;
        if (PlayerMng.Instance.CurrParty != null)
        {
            for (int i = 0; i < m_memberBTNList.Count; ++i)
            {
                if (PlayerMng.Instance.CurrParty.PartyMemberList.Count > i)
                    m_memberBTNList[i].Ready(false);
            }
        }
        m_startText.text = "시작";
        NetworkMng.Instance.NotifyReplyJoinCancle();
    }
    private void LateUpdate()
    {
        if(IsStart)
        {
            m_startText.text = "대기 (" + m_targetTime.ToString("F0") +")";
            m_targetTime -= Time.deltaTime;
            if (PlayerMng.Instance.CurrParty.ReadyMemberList.Count == PlayerMng.Instance.CurrParty.PartyMemberList.Count)
            {
                NetworkMng.Instance.RequestCharacterJoinPrivateMap(m_handle);
                IsStart = false;
            }
            if (m_targetTime < 0)
            {
                NetworkMng.Instance.NotifyReplyJoinCancle();
                IsStart = false;
            }
        }
    }
}
