using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MemberListBTN : MonoBehaviour
{
    public Player Player;
    GameObject m_hostIcon;
    GameObject m_appointHostBTN;
    GameObject m_exitBTN;
    Text m_nameText;
    Text m_jobText;
    public MemberListBTN Init()
    {
        m_nameText = transform.Find("NameText").GetComponent<Text>();
        m_jobText = transform.Find("JobText").GetComponent<Text>();
        m_hostIcon = transform.Find("HostIcon").gameObject;
        m_appointHostBTN = transform.Find("AppointHostBTN").gameObject;
        m_exitBTN = transform.Find("ExitBTN").gameObject;
        return this;
    }
    public void Enabled(Player player)
    {
        Player = player;
        m_nameText.text = player.Name;
        m_jobText.text = "Lv." + player.Level + " "+ ParseLib.GetClassKorConvert(player.Character.StatSystem.BaseStat.Class);
        m_hostIcon.SetActive(player.HostID == PlayerMng.Instance.CurrParty.PartyHost);
        m_appointHostBTN.SetActive(player.HostID == PlayerMng.Instance.CurrParty.PartyHost && PlayerMng.Instance.PlayerList[player.HostID] != player);
        m_exitBTN.SetActive(player.HostID == PlayerMng.Instance.CurrParty.PartyHost && PlayerMng.Instance.PlayerList[player.HostID] != player);
        gameObject.SetActive(true);
    }
    public void Disabled()
    {
        Player = null;
        gameObject.SetActive(false);
    }
}
