using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Nettention.Proud;
using Vector3 = UnityEngine.Vector3;

public partial class NetworkMng : TSingleton<NetworkMng>
{
    // 그룹에 들어왔으면 기존 그룹원에게 캐릭터 정보를 보내줌
    public void NotifyJoinGroup()
    {
        RmiContext Rmi = RmiContext.ReliableSend;
        Rmi.enableLoopback = false;
        Player player = PlayerMng.Instance.MainPlayer;
        string infoData = ParseLib.GetPlayerInfoData(player);
        string statusData = ParseLib.GetStatusData(player);
        string equipData = ParseLib.GetEquipData(player);
        string skillData = ParseLib.GetSkillData(player);

        m_contactP2PProxy.NotifyPlayerJoin(m_groupID, Rmi, player.Character.UniqueID, EAllyType.Friendly, infoData, statusData, equipData, skillData);
    }
    // 그룹원은 신규 그룹원에게 캐릭터 정보를 보내줌
    public void NotifyPlayerInfo(HostID remote)
    {
        RmiContext Rmi = RmiContext.ReliableSend;
        Rmi.enableLoopback = false;

        Player player = PlayerMng.Instance.MainPlayer;
        string infoData = ParseLib.GetPlayerInfoData(player);
        string statusData = ParseLib.GetStatusData(player);
        string equipData = ParseLib.GetEquipData(player);
        string skillData = ParseLib.GetSkillData(player);

        m_contactP2PProxy.NotifyPlayerInfo(remote, Rmi, player.Character.UniqueID, EAllyType.Friendly, infoData, statusData, equipData, skillData);
    }
    public void NotifySendChat(string text)
    {
        if (m_groupID == HostID.HostID_None)
            return;

        RmiContext context = RmiContext.UnreliableSend;
        context.enableLoopback = true;
        m_communityP2PProxy.NotifySendChat(m_groupID, RmiContext.UnreliableSend, text);
    }
    public void NotifyCharacterState_Idle()
    {
        if (m_groupID == HostID.HostID_None)
            return;

        RmiContext rmi = RmiContext.ReliableSend;
        rmi.enableLoopback = true;
        m_behaviorP2PProxy.NotifyCharacterIdle(m_groupID, rmi);
    }
    public void NotifyCharacterState_Move(Vector3 nextPos, Vector3 eulerAngle)
    {
        if (m_groupID == HostID.HostID_None)
            return;

        RmiContext rmi = RmiContext.UnreliableSend;
        rmi.enableLoopback = false;
        m_behaviorP2PProxy.NotifyCharacterMove(m_groupID, rmi, nextPos, eulerAngle);
    }
    public void NotifyCharacterState_Chase(UnityEngine.Vector3 pos, float distance)
    {
        if (m_groupID == HostID.HostID_None)
            return;

        RmiContext rmi = RmiContext.ReliableSend;
        rmi.enableLoopback = false;
        m_behaviorP2PProxy.NotifyCharacterMovePosition(m_groupID, rmi, pos, distance);
    }
    public void NotifyCharacterState_Attack(UnityEngine.Vector3 dir)
    {
        RmiContext rmi = RmiContext.ReliableSend;
        rmi.enableLoopback = true;

        m_behaviorP2PProxy.NotifyCharacterAttack(m_groupID, rmi, dir);
    }
    public void NotifyCharacterState_Skill(UnityEngine.Vector3 dir, int skillHandle)
    {
        RmiContext rmi = RmiContext.ReliableSend;
        rmi.enableLoopback = true;

        Debug.Log("RequestSkill");
        m_behaviorP2PProxy.NotifyCharacterSkill(m_groupID, rmi, dir, skillHandle);
    }
    public void NotifyCharacterState_SkillEnd(UnityEngine.Vector3 dir, int skillHandle)
    {
        RmiContext rmi = RmiContext.ReliableSend;
        rmi.enableLoopback = true;
        m_behaviorP2PProxy.NotifyCharacterSkillEnd(m_groupID, rmi, dir, skillHandle);
    }
    public void NotifyRequestPartyList()
    {
        if (m_groupID == HostID.HostID_None)
            return;

        UIMng.Instance.OPEN = UIMng.UIName.PartyMenu;
        RmiContext Rmi = RmiContext.ReliableSend;
        Rmi.enableLoopback = false;
        m_communityP2PProxy.NotifyRequestPartyList(m_groupID, Rmi);
    }
    public void NotifyRequestJoinPrivateMap(int mapHandle)
    {
        PlayerMng.Instance.CurrParty.ReadyMemberList.Clear();
        PlayerMng.Instance.CurrParty.ReadyMemberList.Add(m_netClient.LocalHostID);
        for (int i = 0; i < PlayerMng.Instance.CurrParty.PartyMemberList.Count; ++i)
        {
            if(PlayerMng.Instance.CurrParty.PartyMemberList[i] != PlayerMng.Instance.CurrParty.PartyHost)
                m_communityP2PProxy.NotifyRequestJoinPrivateMap(PlayerMng.Instance.CurrParty.PartyMemberList[i], RmiContext.ReliableSend, mapHandle);
        }
    }
    public void NotifyReplyJoinPrivateMap(bool isReady)
    {
        m_communityP2PProxy.NotifyReplyJoinPrivateMap(PlayerMng.Instance.CurrParty.PartyHost, RmiContext.ReliableSend, isReady);
    }
    public void NotifyReplyJoinCancle()
    {
        for (int i = 0; i < PlayerMng.Instance.CurrParty.PartyMemberList.Count; ++i)
            if (PlayerMng.Instance.CurrParty.PartyMemberList[i] != PlayerMng.Instance.CurrParty.PartyHost)
                m_communityP2PProxy.NotifyJoinPrivatePortalCancle(PlayerMng.Instance.CurrParty.PartyMemberList[i], RmiContext.ReliableSend);
    }
    public void NotifyReceiveDamage(EAttackType type, int casterID, int targetID, float damage, float hitTime)
    {
        RmiContext rmi = RmiContext.ReliableSend;
        rmi.enableLoopback = true;
        m_battleP2PProxy.NotifyReceiveDamage(m_groupID, rmi, casterID, targetID, type, damage, hitTime);
    }
    public void NotifyRecoveryHP(int casterID, int targetID, float staticRecovery, float dynamicRecovery)
    {
        RmiContext rmi = RmiContext.ReliableSend;
        rmi.enableLoopback = true;
        m_battleP2PProxy.NotifyReceiveRecoveryHP(m_groupID, rmi, casterID, targetID, staticRecovery, dynamicRecovery);
    }
    public void NotifyRecoveryMP(int casterID, int targetID, float staticRecovery, float dynamicRecovery)
    {
        RmiContext rmi = RmiContext.ReliableSend;
        rmi.enableLoopback = true;
        m_battleP2PProxy.NotifyReceiveRecoveryMP(m_groupID, rmi, casterID, targetID, staticRecovery, dynamicRecovery);
    }
    void ReceiveClient()
    {
        m_netClient.P2PMemberJoinHandler = (HostID memberHostID, HostID groupHostID, int memberCount, ByteArray message) =>
        {

        };
        m_netClient.P2PMemberLeaveHandler = (HostID memberHostID, HostID groupHostID, int memberCount) =>
        {
            if (!SceneMng.Instance.IsSceneLoad)
                return;

            if (memberHostID != m_netClient.LocalHostID)
            {
                if (PlayerMng.Instance.PlayerList[memberHostID].Character != null)
                {
                    CharacterMng.Instance.CurrCharacters.Remove(PlayerMng.Instance.PlayerList[memberHostID].Character.UniqueID);
                    Destroy(PlayerMng.Instance.PlayerList[memberHostID].Character.gameObject);
                }
            }
            // PlayerMng.Instance.PlayerList[memberHostID].Character.gameObject.SetActive(false);
        };
       m_contactP2PStub.NotifyPlayerJoin = (Nettention.Proud.HostID remote, Nettention.Proud.RmiContext rmiContext, int uniqueID, EAllyType type, string infoData, string statusData, string equipData, string skillData) =>
        {
            if (!SceneMng.Instance.IsSceneLoad)
                return true;

            if (PlayerMng.Instance.PlayerList.ContainsKey(remote))
                return true;

            Player player = new Player();
            PlayerMng.Instance.PlayerList.Add(remote, player);
            player.HostID = remote;
            ParseLib.SetPlayerData(ref player, uniqueID, type, infoData, statusData, equipData);
            ParseLib.SetSkillData(ref player, skillData);
            NotifyPlayerInfo(remote);
            return true;
        };
        m_contactP2PStub.NotifyPlayerInfo = (Nettention.Proud.HostID remote, Nettention.Proud.RmiContext rmiContext, int uniqueID, EAllyType type, string infoData, string statusData, string equipData, string skillData) =>
        {
            if (!SceneMng.Instance.IsSceneLoad)
                return true;

            if (PlayerMng.Instance.PlayerList.ContainsKey(remote))
                return true;

            Player player = new Player();
            PlayerMng.Instance.PlayerList.Add(remote, player);

            player.HostID = remote;
            ParseLib.SetPlayerData(ref player, uniqueID, type, infoData, statusData, equipData);
            ParseLib.SetSkillData(ref player, skillData);
            return true;
        };
        m_behaviorP2PStub.NotifyCharacterIdle = (Nettention.Proud.HostID remote, Nettention.Proud.RmiContext rmiContext) =>
        {
            //if (!PlayerMng.Instance.PlayerList.ContainsKey(remote))
            //    return true;
            //if (PlayerMng.Instance.PlayerList[remote].Character == null)
            //    return true;
            if (!SceneMng.Instance.IsSceneLoad)
                return true;

            BaseHero character = PlayerMng.Instance.PlayerList[remote].Character;
            if (character.State == BaseCharacter.CharacterState.Death)
                return true;
            character.State = BaseCharacter.CharacterState.Idle;
            return true;
        };
        m_behaviorP2PStub.NotifyCharacterMove = (Nettention.Proud.HostID remote, Nettention.Proud.RmiContext rmiContext, Vector3 pos, Vector3 angle) =>
        {
            //if (!PlayerMng.Instance.PlayerList.ContainsKey(remote))
            //    return true;
            //if (PlayerMng.Instance.PlayerList[remote].Character == null)
            //    return true;
            if (!SceneMng.Instance.IsSceneLoad)
                return true;

            BaseHero character = PlayerMng.Instance.PlayerList[remote].Character;
            if (character.State == BaseCharacter.CharacterState.Death)
                return true;
            if (character.IsStun || character.IsHit || character.IsNuckback)
                return true;
            if (!character.AttackSystem.CompleteAttack)
                return true;
            character.State = BaseCharacter.CharacterState.Move;
            character.transform.eulerAngles = angle;
            character.transform.position = pos;

            return true;
        };
        m_behaviorP2PStub.NotifyCharacterMovePosition = (Nettention.Proud.HostID remote, Nettention.Proud.RmiContext rmiContext, UnityEngine.Vector3 pos, float distance) =>
        {
            //if (!PlayerMng.Instance.PlayerList.ContainsKey(remote))
            //    return true;

            //if (PlayerMng.Instance.PlayerList[remote].Character == null)
            //    return true;

            if (!SceneMng.Instance.IsSceneLoad)
                return true;

            BaseHero character = PlayerMng.Instance.PlayerList[remote].Character;
            if (character.State == BaseCharacter.CharacterState.Death)
                return true;
            if (character.IsStun || character.IsHit || character.IsNuckback)
                return true;
            if (!character.AttackSystem.CompleteAttack)
                return true;
            character.State = BaseCharacter.CharacterState.Chase;
            character.MoveSystem.SetMoveToPosition(pos, distance);
            return true;
        };
        m_behaviorP2PStub.NotifyCharacterAttack = (Nettention.Proud.HostID remote, Nettention.Proud.RmiContext rmiContext, UnityEngine.Vector3 dir) =>
        {
            BaseCharacter character = PlayerMng.Instance.PlayerList[remote].Character;
            if (character.State == BaseCharacter.CharacterState.Death)
                return true;
            if (character.IsStun || character.IsHit || character.IsNuckback)
                return true;
            if (character.AttackSystem.HoldAttack)
                return false;
            character.transform.eulerAngles = dir;
            character.AttackSystem.UseAttack(character.UniqueID, character.StatSystem.GetAttackDamage);
            character.State = BaseCharacter.CharacterState.Battle;
            return true;
        };
        m_behaviorP2PStub.NotifyCharacterSkill = (Nettention.Proud.HostID remote, Nettention.Proud.RmiContext rmiContext, UnityEngine.Vector3 dir, int skillHandle) =>
        {
            BaseCharacter character = PlayerMng.Instance.PlayerList[remote].Character;
            character.transform.eulerAngles = dir;
            if (!character.AttackSystem.SkillDic.ContainsKey(skillHandle))
                CharacterMng.Instance.AddSkill(character, skillHandle);
            character.AttackSystem.UseSkill(skillHandle);
            return true;
        };
        m_behaviorP2PStub.NotifyCharacterSkillEnd = (Nettention.Proud.HostID remote, Nettention.Proud.RmiContext rmiContext, UnityEngine.Vector3 dir, int skillHandle) =>
        {
            if (!PlayerMng.Instance.PlayerList[remote].Character.AttackSystem.SkillDic.ContainsKey(skillHandle))
                return true;
            BaseCharacter character = PlayerMng.Instance.PlayerList[remote].Character;
            if (character.State == BaseCharacter.CharacterState.Death)
                return true;
            BaseSkill skill = character.AttackSystem.SkillDic[skillHandle];
            if ((skill.SkillInfo.Type & ESkillType.Keydown) != 0)
                skill.EndKeydown();
            if ((skill.SkillInfo.Type & ESkillType.Channeling) != 0)
                skill.EndChanneling();
            return true;
        };
        m_behaviorP2PStub.NotifyItemEquip = (Nettention.Proud.HostID remote, Nettention.Proud.RmiContext rmiContext, int itemHandle) =>
        {
            //if (!PlayerMng.Instance.PlayerList.ContainsKey(remote))
            //    return true;
            //if (PlayerMng.Instance.PlayerList[remote].Character == null)
            //    return true;
            if (!SceneMng.Instance.IsSceneLoad)
                return true;

            Item_Base item = ItemMng.Instance.GetItemList[itemHandle].Clone();
            PlayerMng.Instance.PlayerList[remote].Character.StatSystem.SetEquipment(item as Item_Equipment);
            return true;
        };
        m_behaviorP2PStub.NotifyItemUnequip = (Nettention.Proud.HostID remote, Nettention.Proud.RmiContext rmiContext, EItemType itemType) =>
        {
            //if (!PlayerMng.Instance.PlayerList.ContainsKey(remote))
            //    return true;
            //if (PlayerMng.Instance.PlayerList[remote].Character == null)
            //    return true;
            if (!SceneMng.Instance.IsSceneLoad)
                return true;

            PlayerMng.Instance.PlayerList[remote].Character.StatSystem.RemoveEquipment(itemType);
            return true;
        };
        m_communityP2PStub.NotifyRequestPartyList = (Nettention.Proud.HostID remote, Nettention.Proud.RmiContext rmiContext) =>
        {
            if (PlayerMng.Instance.CurrParty == null)
                return true;
            if (PlayerMng.Instance.CurrParty.PartyHost != m_netClient.LocalHostID)
                return true;
            if (PlayerMng.Instance.CurrParty.State != EPartyState.Public)
                return true;
            if (!SceneMng.Instance.IsSceneLoad)
                return true;

            Party party = PlayerMng.Instance.CurrParty;
            m_communityP2PProxy.NotifyReplyPartyList(remote, RmiContext.ReliableSend, party.Handle, party.Name ,party.PartyHost, party.Number, party.PartyMemberList.Count, party.Level);
            return true;
        };
        m_communityP2PStub.NotifyReplyPartyList = (Nettention.Proud.HostID remote, Nettention.Proud.RmiContext rmiContext, int handle, string name, Nettention.Proud.HostID id, int number, int currNumber, int level) =>
        {
            if (!SceneMng.Instance.IsSceneLoad)
                return true;
            if (PlayerMng.Instance.CurrParty != null)
                return true;

            Party party = new Party(handle, id, name, number, level);
            UIMng.Instance.GetUI<PartyMenu>(UIMng.UIName.PartyMenu).SetPartyList(party, currNumber);
            return true;
        };
        m_communityP2PStub.NotifySendChat = (Nettention.Proud.HostID remote, Nettention.Proud.RmiContext rmiContext, string text) =>
        {
            //if (!PlayerMng.Instance.PlayerList.ContainsKey(remote))
            //    return true;
            //if (PlayerMng.Instance.PlayerList[remote].Character == null)
            //    return true;
            if (!SceneMng.Instance.IsSceneLoad)
                return true;

            AddChat(PlayerMng.Instance.PlayerList[remote].Name+ ": "+text);
            UIMng.Instance.GetUI<FieldUI>(UIMng.UIName.FieldUI).SetChatBox(PlayerMng.Instance.PlayerList[remote].Character,text);
            return true;
        };
        m_communityP2PStub.NotifyRequestJoinPrivateMap = (Nettention.Proud.HostID remote, Nettention.Proud.RmiContext rmiContext, int mapHandle) =>
        {
            UIMng.Instance.Open<SelectPopup>(UIMng.UIName.SelectPopup).JoinPopup.Enabled(mapHandle);
            PlayerMng.Instance.CurrParty.MapHandle = mapHandle;
            return true;
        };
        m_communityP2PStub.NotifyReplyJoinPrivateMap = (Nettention.Proud.HostID remote, Nettention.Proud.RmiContext rmiContext, bool isReady) =>
        {
            if (isReady)
            {
                if (!PlayerMng.Instance.CurrParty.ReadyMemberList.Contains(remote))
                {
                    PlayerMng.Instance.CurrParty.ReadyMemberList.Add(remote);
                    UIMng.Instance.GetUI<SelectStage>(UIMng.UIName.SelectStage).OnReady(PlayerMng.Instance.CurrParty.ReadyMemberList.IndexOf(remote));
                }
            }
            else
            {
                SystemMessage.Instance.PushMessage(SystemMessage.MessageType.Sub, "매칭이 취소되었습니다.");
                UIMng.Instance.GetUI<SelectStage>(UIMng.UIName.SelectStage).OnMatchCancle();
                PlayerMng.Instance.CurrParty.ReadyMemberList.Clear();
            }

            return true;
        };
        m_communityP2PStub.NotifyRequestJoinPrivatePortal = (Nettention.Proud.HostID remote, Nettention.Proud.RmiContext rmiContext, int portalNumber) =>
        {
            return true;
        };
        m_communityP2PStub.NotifyReplyJoinPrivatePortal = (Nettention.Proud.HostID remote, Nettention.Proud.RmiContext rmiContext, bool isReady) =>
        {
            return true;
        };
        m_communityP2PStub.NotifyJoinPrivatePortalCancle = (Nettention.Proud.HostID remote, Nettention.Proud.RmiContext rmiContext) =>
        {
            UIMng.Instance.CLOSE = UIMng.UIName.SelectPopup;
            SystemMessage.Instance.PushMessage(SystemMessage.MessageType.Sub, PlayerMng.Instance.PlayerList[remote].Name + "님이 매칭을 취소하였습니다.");
            return true;
        };
        m_battleP2PStub.NotifyReceiveDamage = (Nettention.Proud.HostID remote, Nettention.Proud.RmiContext rmiContext, int casterID, int targetID, EAttackType type, float damage, float hitTime) =>
        {
            SReceiveHandle receiveHandle = new SReceiveHandle(type, casterID, damage, hitTime);
            BaseCharacter character = CharacterMng.Instance.CurrCharacters[targetID];
            character.ReceiveAttack(receiveHandle);
            if (CharacterMng.Instance.CurrCharacters[casterID].tag == "Player")
                if (character.StatSystem.BaseStat.MonsterGrade > EMonsterGrade.MediumBoss)
                    UIMng.Instance.GetUI<Game>(UIMng.UIName.Game).InfoWindow.Enabled(character);
            return true;
        };
        m_battleP2PStub.NotifyReceiveRecoveryHP = (Nettention.Proud.HostID remote, Nettention.Proud.RmiContext rmiContext, int casterID, int targetID, float staticRecovery, float dynamicRecovery) =>
        {
            BaseCharacter character = CharacterMng.Instance.CurrCharacters[targetID];

            if (character.State == BaseCharacter.CharacterState.Death)
                return true;

            if (dynamicRecovery != 0)
                staticRecovery += character.StatSystem.GetHP * dynamicRecovery;

            character.StatSystem.RecoveryHP(staticRecovery);
            
            UIMng.Instance.GetUI<FieldUI>(UIMng.UIName.FieldUI).SetDamageText(character, staticRecovery.ToString("F0"), Color.green);
            return true;
        };
        m_battleP2PStub.NotifyReceiveRecoveryMP = (Nettention.Proud.HostID remote, Nettention.Proud.RmiContext rmiContext, int casterID, int targetID, float staticRecovery, float dynamicRecovery) =>
        {
            BaseCharacter character = CharacterMng.Instance.CurrCharacters[targetID];

            if (character.State == BaseCharacter.CharacterState.Death)
                return true;

            if (dynamicRecovery != 0)
                staticRecovery += character.StatSystem.GetMP * dynamicRecovery;

            character.StatSystem.RecoveryMP(staticRecovery);

            UIMng.Instance.GetUI<FieldUI>(UIMng.UIName.FieldUI).SetDamageText(character, staticRecovery.ToString("F0"), Color.blue);
            return true;
        };
    }
}
