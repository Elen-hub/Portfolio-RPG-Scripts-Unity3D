using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectPopup_PrivateJoin : SelectPopup_Base
{
    float m_targetTime;

    GameObject m_readyBTN;
    GameObject m_cancleBTN;

    Image m_mapImg;
    Text m_mapName;
    public override void Init()
    {
        base.Init();

        m_readyBTN = transform.Find("Success").gameObject;
        m_cancleBTN = transform.Find("Exit").gameObject;
        m_mapImg = transform.Find("MapContent").GetComponent<Image>();
        m_mapName = transform.Find("MapName").GetComponent<Text>();
        m_readyBTN.GetComponent<Button>().onClick.AddListener(OnClickReady);
        m_cancleBTN.GetComponent<Button>().onClick.AddListener(Exit);
        gameObject.SetActive(false);
    }
    public void Enabled(int mapHandle)
    {
        m_targetTime = 15;
        m_status.text = "파티장이 이동을 신청했습니다.";
        m_readyBTN.SetActive(true);
        m_cancleBTN.SetActive(true);
        m_mapName.text = MapMng.Instance.MapDic[mapHandle].MapName;
        m_mapImg.sprite = Resources.Load<Sprite>(MapMng.Instance.MapDic[mapHandle].MapIconPath);
        gameObject.SetActive(true);
    }
    public void Disabled()
    {
        gameObject.SetActive(false);
    }
    void OnClickReady()
    {
        if (PlayerMng.Instance.CurrParty == null || PlayerMng.Instance.CurrParty.PartyHost == PlayerMng.Instance.MainPlayer.HostID)
            return;

        m_readyBTN.SetActive(false);
        m_status.text = "파티원의 수락을 기다리는 중입니다.";
        NetworkMng.Instance.NotifyReplyJoinPrivateMap(true);
    }
    void Exit()
    {
        UIMng.Instance.CLOSE = UIMng.UIName.SelectPopup;
        NetworkMng.Instance.NotifyReplyJoinPrivateMap(false);
    }
    private void LateUpdate()
    {
        if(m_targetTime >0)
            m_targetTime -= Time.deltaTime;
        m_successText.text = "준비 (" + m_targetTime.ToString("F0") + ")";
    }
}
