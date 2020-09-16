using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SubWindow_Party : MonoBehaviour
{
    List<SubWindow_PartyBTN> m_partyBTNList = new List<SubWindow_PartyBTN>();
    public SubWindow_Party Init()
    {
        m_partyBTNList.AddRange(GetComponentsInChildren<SubWindow_PartyBTN>());
        for (int i = 0; i < m_partyBTNList.Count; ++i)
            m_partyBTNList[i].Init();

        Disabled();
        return this;
    }
    public void Enabled()
    {
        SetPartyBTN();
        gameObject.SetActive(true);
    }
    public void Disabled()
    {
        gameObject.SetActive(false);
    }
    void SetPartyBTN()
    {
        for (int i = 0; i < m_partyBTNList.Count; ++i)
        {
            if (PlayerMng.Instance.CurrParty != null)
            {
                if (PlayerMng.Instance.CurrParty.PartyMemberList.Count > i)
                {
                    m_partyBTNList[i].Enabled(PlayerMng.Instance.PlayerList[PlayerMng.Instance.CurrParty.PartyMemberList[i]]);
                    continue;
                }
            }

            m_partyBTNList[i].Disabled();
        }
    }
    private void LateUpdate()
    {
        if (PlayerMng.Instance.CurrParty == null)
            return;
        for(int i =0; i<PlayerMng.Instance.CurrParty.PartyMemberList.Count; ++i)
        {
            if (PlayerMng.Instance.PlayerList[PlayerMng.Instance.CurrParty.PartyMemberList[i]] == m_partyBTNList[i].Player)
                continue;

            SetPartyBTN();
        }
    }
}
