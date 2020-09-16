using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SubWindow_Map : MonoBehaviour
{
    Image m_playerImg;
    List<SubWindow_Map_DynamicIcon> m_dynamicIcon = new List<SubWindow_Map_DynamicIcon>();
    List<SubWindow_Map_StaticIcon> m_staticIcon = new List<SubWindow_Map_StaticIcon>();
    Transform m_mapGrid;
    Image m_mapImg;
    Vector3 m_coord;
    Text m_nameText;
    Text m_coordText;
    public SubWindow_Map Init()
    {
        m_mapGrid = transform.Find("Mask").Find("Map");
        m_mapImg = m_mapGrid.GetComponent<Image>();
        m_nameText = transform.Find("MapName").GetComponent<Text>();
        m_coordText = transform.Find("Coord").GetComponent<Text>();
        return this;
    }
    public void SetMap()
    {
        Map map = MapMng.Instance.CurrMap;
        m_nameText.text = map.MapName;
        m_mapImg.sprite = Resources.Load<Sprite>(map.MapImgPath);

        for(int i =0; i<m_staticIcon.Count; ++i)
            m_staticIcon[i].Disabled();
        for (int i = 0; i < m_dynamicIcon.Count; ++i)
            m_dynamicIcon[i].Disabled();
        for (int i = 0; i< map.PortalList.Count; ++i)
            SetStaticIcon(SubWindow_Map_StaticIcon.EMapStaticIconOption.TownPortal, map.PortalList[i].Coord);
        for(int i = 0; i<map.MatchPortalList.Count; ++i)
            SetStaticIcon(SubWindow_Map_StaticIcon.EMapStaticIconOption.BattlePortal, map.MatchPortalList[i].Coord);
    }
    public void SetStaticIcon(SubWindow_Map_StaticIcon.EMapStaticIconOption option, Vector3 pos)
    {
        for (int i = 0; i < m_staticIcon.Count; ++i)
        {
            if (!m_staticIcon[i].IsActive)
            {
                m_staticIcon[i].Enabled(option, pos);
                return;
            }
        }
        SubWindow_Map_StaticIcon icon = Instantiate(Resources.Load<SubWindow_Map_StaticIcon>("UI/Instance/MapStaticIcon"), m_mapGrid).Init();
        icon.Enabled(option, pos);
        m_staticIcon.Add(icon);
    }
    public void SetDynamicIcon(SubWindow_Map_DynamicIcon.EMapIconOption option, BaseCharacter character)
    {
        for(int i=0; i< m_dynamicIcon.Count; ++i)
        {
            if(!m_dynamicIcon[i].IsActive)
            {
                m_dynamicIcon[i].Enabled(option, character);
                return;
            }
        }
        SubWindow_Map_DynamicIcon icon = Instantiate(Resources.Load<SubWindow_Map_DynamicIcon>("UI/Instance/MapDynamicIcon"), m_mapGrid).Init();
        icon.Enabled(option, character);
        m_dynamicIcon.Add(icon);
    }
    public void Enabled()
    {
        gameObject.SetActive(true);
    }
    public void Disabled()
    {
        gameObject.SetActive(false);
    }
    private void LateUpdate()
    {
        m_coord = PlayerMng.Instance.MainPlayer.Character.transform.position;
        m_coordText.text = m_coord.x.ToString("F0") + " , " + m_coord.z.ToString("F0");

        if(PlayerMng.Instance.CurrParty == null)
        {
            for (int i = 0; i < m_dynamicIcon.Count; ++i)
            {
                if (m_dynamicIcon[i].Target == PlayerMng.Instance.MainPlayer.Character)
                    continue;
            }
            SetDynamicIcon(SubWindow_Map_DynamicIcon.EMapIconOption.Player, PlayerMng.Instance.MainPlayer.Character);
        }
        else
        {
            for(int i =0; i< PlayerMng.Instance.CurrParty.PartyMemberList.Count; ++i)
            {
                if (!PlayerMng.Instance.PlayerList.ContainsKey(PlayerMng.Instance.CurrParty.PartyMemberList[i]))
                    continue;

                BaseCharacter character = PlayerMng.Instance.PlayerList[PlayerMng.Instance.CurrParty.PartyMemberList[i]].Character;
                if (character != null)
                {
                    for (int j = 0; j < m_dynamicIcon.Count; ++j)
                    {
                        if (m_dynamicIcon[j].Target == character)
                            return;
                    }
                    switch(character.AllyType)
                    {
                        case EAllyType.Player:
                            SetDynamicIcon(SubWindow_Map_DynamicIcon.EMapIconOption.Player, character);
                            break;
                        case EAllyType.Friendly:
                            SetDynamicIcon(SubWindow_Map_DynamicIcon.EMapIconOption.Ally, character);
                            break;
                        case EAllyType.Hostile:
                            SetDynamicIcon(SubWindow_Map_DynamicIcon.EMapIconOption.Enermy, character);
                            break;
                    }
                }
            }
        }
    }
}
