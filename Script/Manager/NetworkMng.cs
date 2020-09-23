using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayNANOO;
using Nettention.Proud;
public enum ENetworkState
{
    Standby,
    LogginOn,
    Connecting,
    ConnectOn,
    Offline,
}
public partial class NetworkMng : TSingleton<NetworkMng>
{
    HostID m_groupID;
    string m_serverAddress = "15.164.245.51";
    // string m_serverAddress = "192.168.219.100";
    public string GetIPAddress { get { return m_serverAddress; } set { m_serverAddress = value; } }
    // string m_serverAddress = "localhost";
    Guid m_guidVersion = new Guid("{0x118ccf78,0x764b,0x419e,{0xae,0xd,0x19,0xe1,0x75,0x65,0x16,0xb0}}");
    NetClient m_netClient;
    NetConnectionParam m_connectionParam;
    public UnityEngine.Events.UnityAction<string> AddChat;
    string m_enermyData = "";

    DB_Account_C2S.Proxy m_accountProxy = new DB_Account_C2S.Proxy();
    DB_Account_S2C.Stub m_accountStub = new DB_Account_S2C.Stub();
    DB_Information_C2S.Proxy m_informationProxy = new DB_Information_C2S.Proxy();
    DB_Information_S2C.Stub m_informationStub = new DB_Information_S2C.Stub();
    DB_Alter_C2S.Proxy m_alterProxy = new DB_Alter_C2S.Proxy();
    DB_Alter_S2C.Stub m_alterStub = new DB_Alter_S2C.Stub();
    Net_Contact_C2S.Proxy m_contactProxy = new Net_Contact_C2S.Proxy();
    Net_Contact_S2C.Stub m_contactStub = new Net_Contact_S2C.Stub();
    Net_Behavior_C2S.Proxy m_behaviorProxy = new Net_Behavior_C2S.Proxy();
    Net_Behavior_S2C.Stub m_behaviorStub = new Net_Behavior_S2C.Stub();
    Net_Battle_C2S.Proxy m_battleProxy = new Net_Battle_C2S.Proxy();
    Net_Battle_S2C.Stub m_battleStub = new Net_Battle_S2C.Stub();


    Net_Behavior_C2C.Proxy m_behaviorP2PProxy = new Net_Behavior_C2C.Proxy();
    Net_Behavior_C2C.Stub m_behaviorP2PStub = new Net_Behavior_C2C.Stub();
    Net_Contact_C2C.Proxy m_contactP2PProxy = new Net_Contact_C2C.Proxy();
    Net_Contact_C2C.Stub m_contactP2PStub = new Net_Contact_C2C.Stub();
    Net_Status_C2C.Proxy m_statusP2PProxy = new Net_Status_C2C.Proxy();
    Net_Status_C2C.Stub m_statusP2PStub = new Net_Status_C2C.Stub();
    Net_Community_C2C.Proxy m_communityP2PProxy = new Net_Community_C2C.Proxy();
    Net_Community_C2C.Stub m_communityP2PStub = new Net_Community_C2C.Stub();
    Net_Battle_C2C.Proxy m_battleP2PProxy = new Net_Battle_C2C.Proxy();
    Net_Battle_C2C.Stub m_battleP2PStub = new Net_Battle_C2C.Stub();

    int m_currChannel = -1;
    public ENetworkState NetworkState;
    public override void Init()
    {
        m_connectionParam = new NetConnectionParam();
        m_connectionParam.serverIP = m_serverAddress;
        m_connectionParam.clientAddrAtServer = m_serverAddress;
        m_connectionParam.serverPort = 15001;
        m_connectionParam.protocolVersion = m_guidVersion;

        m_netClient = new NetClient();

        m_netClient.AttachProxy(m_accountProxy);
        m_netClient.AttachStub(m_accountStub);
        m_netClient.AttachProxy(m_informationProxy);
        m_netClient.AttachStub(m_informationStub);
        m_netClient.AttachProxy(m_alterProxy);
        m_netClient.AttachStub(m_alterStub);
        m_netClient.AttachProxy(m_contactProxy);
        m_netClient.AttachStub(m_contactStub);
        m_netClient.AttachProxy(m_behaviorProxy);
        m_netClient.AttachStub(m_behaviorStub);
        m_netClient.AttachProxy(m_battleProxy);
        m_netClient.AttachStub(m_battleStub);

        m_netClient.AttachProxy(m_behaviorP2PProxy);
        m_netClient.AttachStub(m_behaviorP2PStub);
        m_netClient.AttachProxy(m_contactP2PProxy);
        m_netClient.AttachStub(m_contactP2PStub);
        //m_netClient.AttachProxy(m_statusP2PProxy);
        //m_netClient.AttachStub(m_statusP2PStub);
        m_netClient.AttachProxy(m_communityP2PProxy);
        m_netClient.AttachStub(m_communityP2PStub);
        m_netClient.AttachProxy(m_battleP2PProxy);
        m_netClient.AttachStub(m_battleP2PStub);

        ReceiveServer();
        ReceiveClient();
        
        m_plugin = Plugin.GetInstance();
        m_plugin.SetLanguage(Configure.PN_LANG_KO);
        m_plugin.transform.SetParent(transform);

        SubscriptionArr.Add(0, "elen-79BCE49A-23130C14");
        SubscriptionArr.Add(1, "elen-CBB5EB93-195790CD");

        IsLoad = true;
    }
    public int GetPing()
    {
        return m_netClient.GetLastReliablePingMs(HostID.HostID_Server);
    }
    public void DisconnectServer()
    {
        m_netClient.Disconnect();
        if (PlayerMng.Instance.MainPlayer.Character != null)
        {
            UIMng.Instance.CLOSE = UIMng.UIName.Game;
            Destroy(PlayerMng.Instance.MainPlayer.Character.gameObject);
            PlayerMng.Instance.MainPlayer.ID = null;
            PlayerMng.Instance.MainPlayer.Name = null;
            PlayerMng.Instance.MainPlayer.Character = null;
        }
        NetworkState = ENetworkState.Offline;
        SceneMng.Instance.SetCurrScene(EScene.TitleScene);
    }
    void Update()
    {
        if (m_netClient != null)
            m_netClient.FrameMove();
    }
    private void OnDestroy()
    {
        if (m_netClient != null)
        {
            m_netClient.Disconnect();
            m_netClient.Dispose();
        }
    }
    public void Exit()
    {
        if (m_netClient != null)
            m_netClient.Disconnect();

        m_groupID = HostID.HostID_None;
    }
}
