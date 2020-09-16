using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PartyMenu : BaseUI
{
    int m_selectHandle = -1;
    public int SelectHandle
    {
        get { return m_selectHandle; }
        set
        {
            if(m_selectHandle != -1)
                m_partyList[m_selectHandle].ChangeColor(Color.white);

            m_selectHandle = value;
            m_partyList[m_selectHandle].ChangeColor(Color.red);
        }
    }
    List<MemberListBTN> m_memberList = new List<MemberListBTN>();
    List<PartyListBTN> m_partyList = new List<PartyListBTN>();

    Transform m_partyListGrid;
    GameObject m_btnGrid;
    GameObject m_refreshBTN;
    GameObject m_exitPartyBTN;
    GameObject m_createPartyBTN;
    GameObject m_joinBTN;
    GameObject m_member;
    GameObject m_party;
    GameObject m_createParty;

    InputField m_nameField;
    Dropdown m_numberDown;
    InputField m_levelField;
    protected override void InitUI()
    {
        transform.Find("Exit").GetComponent<Button>().onClick.AddListener(Close);
        Transform ButtonGrid = transform.Find("ButtonGrid");
        m_joinBTN = ButtonGrid.Find("Join").gameObject;
        m_joinBTN.GetComponent<Button>().onClick.AddListener(OnClickJoin);
        m_refreshBTN = ButtonGrid.Find("Refresh").gameObject;
        m_refreshBTN.GetComponent<Button>().onClick.AddListener(OnClickRefresh);
        m_createPartyBTN = ButtonGrid.Find("Create").gameObject;
        m_createPartyBTN.GetComponent<Button>().onClick.AddListener(OnClickCreate);
        m_exitPartyBTN = ButtonGrid.Find("Exit").gameObject;
        m_exitPartyBTN.GetComponent<Button>().onClick.AddListener(OnClickExit);
        m_btnGrid = ButtonGrid.gameObject;

        m_member = transform.Find("MemberList").gameObject;
        m_party = transform.Find("PartyList").gameObject;
        m_partyListGrid = m_party.GetComponentInChildren<VerticalLayoutGroup>().transform;
        Transform MemberGrid = transform.Find("MemberList").GetComponentInChildren<VerticalLayoutGroup>().transform;
        MemberListBTN btn = Resources.Load<MemberListBTN>("UI/Instance/MemberListBTN");
        for (int i = 0; i < 6; ++i)
            m_memberList.Add(Instantiate(btn, MemberGrid).Init());

        Transform CreateParty = transform.Find("CreateParty");
        m_createParty = CreateParty.gameObject;
        m_nameField = CreateParty.Find("NameField").GetComponent<InputField>();
        m_numberDown = CreateParty.Find("NumberSelect").GetComponent<Dropdown>();
        m_levelField = CreateParty.Find("LevelField").GetComponent<InputField>();

        CreateParty.Find("CreateButton").GetComponent<Button>().onClick.AddListener(OnClickCreateSubmit);
        CreateParty.Find("ExitButton").GetComponent<Button>().onClick.AddListener(OnClickCancle);
    }
    public override void Open()
    {
        if(m_selectHandle != -1)
            m_selectHandle = -1;
        for (int i = 0; i < m_partyList.Count; ++i)
            m_partyList[i].Disabled();
        for (int i = 0; i < m_memberList.Count; ++i)
            m_memberList[i].Disabled();
        m_createParty.SetActive(false);
        m_createPartyBTN.SetActive(PlayerMng.Instance.CurrParty == null);
        m_joinBTN.SetActive(PlayerMng.Instance.CurrParty == null);
        m_refreshBTN.SetActive(PlayerMng.Instance.CurrParty == null);
        m_exitPartyBTN.SetActive(PlayerMng.Instance.CurrParty != null);

        m_member.SetActive(PlayerMng.Instance.CurrParty != null);
        m_party.SetActive(PlayerMng.Instance.CurrParty == null);
        m_btnGrid.SetActive(true);

        gameObject.SetActive(true);
    }
    public override void Close()
    {
        gameObject.SetActive(false);
    }
    void SetPartyMember()
    {
        for (int i = 0; i < m_memberList.Count; ++i)
        {
            if (PlayerMng.Instance.CurrParty.PartyMemberList.Count > i)
            {
                m_memberList[i].Enabled(PlayerMng.Instance.PlayerList[PlayerMng.Instance.CurrParty.PartyMemberList[i]]);
                continue;
            }

            m_memberList[i].Disabled();
        }
    }
    public void SetPartyList(Party party, int currNumber)
    {
        for (int i = 0; i < m_partyList.Count; ++i)
        {
            if (!m_partyList[i].gameObject.activeSelf)
            {
                m_partyList[i].Enabled(party, i, currNumber);
                return;
            }
        }
        PartyListBTN btn = Instantiate(Resources.Load<PartyListBTN>("UI/Instance/PartyListBTN"), m_partyListGrid).Init();
        btn.Enabled(party, m_partyList.Count, currNumber);
        m_partyList.Add(btn);
    }
    public void SetLevel()
    {
        if(PlayerMng.Instance.MainPlayer.Level < int.Parse(m_levelField.text))
        {
            SystemMessage.Instance.PushMessage(SystemMessage.MessageType.Sub, "자신보다 높은 레벨로 설정 할 수 없습니다.");
            PlayerMng.Instance.MainPlayer.Level = 0;
            return;
        }
    }
    public void SetNumber()
    {
        
    }
    void OnClickCreate()
    {
        m_nameField.text = null;
        m_numberDown.value = 0;
        m_levelField.text = "0";
        m_party.SetActive(false);
        m_createParty.SetActive(true);
        m_btnGrid.SetActive(false);
    }
    void OnClickCreateSubmit()
    {
        string name = m_nameField.text;
        if (name == "")
            name = "우리 같이 파티해요!";
        int number = m_numberDown.value + 1;
        int level = int.Parse(m_levelField.text);
        NetworkMng.Instance.RequestCreateParty(name, number, level);
    }
    void OnClickCancle()
    {
        m_party.SetActive(true);
        m_createParty.SetActive(false);
        m_btnGrid.SetActive(true);
    }
    void OnClickJoin()
    {
        if (SelectHandle == -1)
            return;

        if (PlayerMng.Instance.MainPlayer.Level < m_partyList[SelectHandle].Party.Level)
        {
            SystemMessage.Instance.PushMessage(SystemMessage.MessageType.Sub, "파티에 참가하기위한 레벨이 부족합니다.");
            return;
        }
        
        NetworkMng.Instance.RequestJoinParty(m_partyList[SelectHandle].Party.Handle);
    }
    void OnClickRefresh()
    {
        NetworkMng.Instance.NotifyRequestPartyList();
    }
    void OnClickExit()
    {
        NetworkMng.Instance.RequestLeaveParty();
    }
    private void LateUpdate()
    {
        if (PlayerMng.Instance.CurrParty != null)
        {
            for (int i = 0; i < PlayerMng.Instance.CurrParty.PartyMemberList.Count; ++i)
            {
                if (PlayerMng.Instance.PlayerList[PlayerMng.Instance.CurrParty.PartyMemberList[i]] == m_memberList[i].Player)
                    continue;

                SetPartyMember();
            }
        }
    }
}
