using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Nettention.Proud;
using Vector3 = UnityEngine.Vector3;
using SimpleJSON;
public enum EPartyState
{
    Public,
    Prviate,
    Start,
}
public class Party
{
    public Party(int handle, HostID host, string name, int number, int level)
    {
        Handle = handle;
        PartyHost = host;
        Name = name;
        Number = number;
        Level = level;
    }
    public bool Join(HostID id)
    {
        if (!PartyMemberList.Contains(id))
        {
            PartyMemberList.Add(id);
            return true;
        }
        return false;
    }
    public bool Leave(HostID id)
    {
        if (PartyMemberList.Contains(id))
        {
            PartyMemberList.Remove(id);
            return true;
        }
        return false;
    }
    public EPartyState State;
    public int Handle;
    public int MapHandle;
    public string Name;
    public int Number;
    public int Level;
    public HostID PartyHost;
    public List<HostID> PartyMemberList = new List<HostID>();
    public List<HostID> ReadyMemberList = new List<HostID>();
}
public class Player
{
    public Player()
    {

    }
    public HostID HostID;

    public BaseHero Character;
    public int CurrMapHandle;
    public bool isReady;

    public string ID;
    public string Name;
    public string Comment;
    public string Email;
    public string PhoneNumber;
    public string Title;

    public HashInt Handle;
    public ECharacterClass Class;
    public HashInt Awakening;
    public HashInt Level;
    public HashInt Exp;
    public HashInt SkillPoint;
    public HashInt StatPoint;

    public HashInt Gold;
    public HashInt Cash;
    public HashInt MaxInventoryCount;

    public IQuickSlotable[] Quickslot = new IQuickSlotable[8];
}
public class PlayerMng : TSingleton<PlayerMng>
{
    public List<int> ExpList = new List<int>();
    public Dictionary<HostID, Player> PlayerList = new Dictionary<HostID, Player>();
    public Player MainPlayer;
    public Party CurrParty;
    CharacterWindow m_characterWindow;
    public CharacterWindow SetCharacterWindow {
        set { m_characterWindow = value; }
    }
    public void AddExp(Player player, int exp)
    {
        BaseCharacter character = player.Character;
        int EXP = exp + MainPlayer.Exp;
        if (EXP >= ExpList[character.StatSystem.Level])
        {
            EXP -= ExpList[character.StatSystem.Level];
            character.StatSystem.Level += 1;
            player.Level += 1;
            player.StatPoint += 3;
            player.SkillPoint += 1;
            EffectMng.Instance.FindEffect("FX/Effect_Levelup", player.Character.transform.position, Vector3.zero, 4);
            character.StatSystem.CurrHP = character.StatSystem.GetHP;
            character.StatSystem.CurrMP = character.StatSystem.GetMP;
            m_characterWindow.SetLevelText = player.Level.ToString();
        }
        MainPlayer.Exp = EXP;
        character.StatSystem.Exp = EXP;
        m_characterWindow.SetEXPText = EXP.ToString("F2");
    }
    public void ClearPlayerList()
    {
        PlayerList.Clear();
    }
    public float GetExpPercent()
    {
        return (float)MainPlayer.Exp / ExpList[MainPlayer.Level] * 100;
    }
    public void Exit()
    {
        foreach(Player player in PlayerList.Values)
        {
            if (player.Character != null)
                DestroyImmediate(player.Character.gameObject);
        }
        PlayerList.Clear();
        MainPlayer = new Player();
    }
    public override void Init()
    {
        MainPlayer = new Player();

        // EXP테이블 불러오기
#if UNITY_EDITOR
        TextAsset Asset = new TextAsset(System.IO.File.ReadAllText("Assets/AssetBundle_Database/DB_EXP.json"));
        JSONNode Node = JSON.Parse(Asset.text);
#else
        JSONNode Node = JSON.Parse(AssetMng.Instance["database"].LoadAsset<TextAsset>("DB_EXP").text);
#endif
        for (int i = 0; i < Node.Count; ++i)
            ExpList.Add(Node[i].AsInt);

        IsLoad = true;
    }
}
