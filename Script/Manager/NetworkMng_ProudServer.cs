using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Nettention.Proud;
using Vector3 = UnityEngine.Vector3;

public partial class NetworkMng : TSingleton<NetworkMng>
{
    public void NetworkConnect()
    {
        NetworkState = ENetworkState.Connecting;
        m_netClient.Connect(m_connectionParam);
    }
    public void RequestLoginAccount(string id, bool isTest = false)
    {
        UIMng.Instance.OPEN = UIMng.UIName.Loading;
        m_accountProxy.RequestConnectServer(HostID.HostID_Server, RmiContext.ReliableSend, id);
        m_accountStub.ReplyConnectServer = (Nettention.Proud.HostID remote, Nettention.Proud.RmiContext rmiContext, bool isConnect, int hostID, string values) =>
        {
            UIMng.Instance.CLOSE = UIMng.UIName.Loading;
            if(isConnect)
            {   
                PlayerMng.Instance.MainPlayer.ID = id;
                m_plugin.SetUUID(id);

                if (!isTest)
                {
                    GameSystem.ID = id;
                    GameSystem.SaveFileStream();

                    UIMng.Instance.GetUI<Title>(UIMng.UIName.Title).Login.Close();
                }

                RequestPlayerCharactersInformation(isTest);
            }
            else
            {

            }
            return true;
        };
    }
    public void RequestCreateCharacter(string id, string name, int handle)
    {
        UIMng.Instance.OPEN = UIMng.UIName.Loading;
        m_accountProxy.RequestCreateCharacter(HostID.HostID_Server, RmiContext.ReliableSend, id, name, handle);
        m_accountStub.ReplyCreateCharacter = (Nettention.Proud.HostID remote, Nettention.Proud.RmiContext rmiContext, bool isCreate, string values) =>
        {
            UIMng.Instance.CLOSE = UIMng.UIName.Loading;
            if (isCreate)
            {
                UIMng.Instance.GetUI<Title>(UIMng.UIName.Title).CreateAccount.CreateCharacter.Close();
                RequestPlayerCharactersInformation();
                SystemMessage.Instance.PushMessage(SystemMessage.MessageType.Main, "캐릭터가 생성되었습니다.");
            }
            else
            {
                switch(values)
                {
                    case "0":
                        SystemMessage.Instance.PushMessage(SystemMessage.MessageType.Sub, "이름은 2글자 미만이 될 수 없습니다.");
                        break;
                    case "1":
                        SystemMessage.Instance.PushMessage(SystemMessage.MessageType.Sub, "이름은 12글자를 초과 할 수 없습니다.");
                        break;
                    case "2":
                        SystemMessage.Instance.PushMessage(SystemMessage.MessageType.Sub, "이름은 특수문자를 사용 할 수 없습니다.");
                        break;
                    case "3":
                        SystemMessage.Instance.PushMessage(SystemMessage.MessageType.Sub, "이미 존재하는 이름입니다.");
                        break;
                    case "4":
                        SystemMessage.Instance.PushMessage(SystemMessage.MessageType.Sub, "캐릭터는 최대 4개까지 생성 가능합니다.");
                        break;
                }
            }
            return true;
        };
    }
    public void RequestDeleteCharacter(string id, string name)
    {
        UIMng.Instance.OPEN = UIMng.UIName.Loading;
        m_accountProxy.RequestDeleteCharacter(HostID.HostID_Server, RmiContext.ReliableSend, id, name);
        m_accountStub.ReplyDeleteCharacters = (Nettention.Proud.HostID remote, Nettention.Proud.RmiContext rmiContext, bool isDelete) =>
        {
            if (isDelete)
            {
                SystemMessage.Instance.PushMessage(SystemMessage.MessageType.Main, "캐릭터가 삭제되었습니다.");
                RequestPlayerCharactersInformation();
            }

            return true;
        };
    }
    public void RequestPlayerCharactersInformation(bool isTest = false)
    {
        UIMng.Instance.OPEN = UIMng.UIName.Loading;
        m_accountProxy.RequestPlayerCharactersInformation(HostID.HostID_Server, RmiContext.ReliableSend);
        m_accountStub.ReplyPlayerCharactersInformation = (Nettention.Proud.HostID remote, Nettention.Proud.RmiContext rmiContext, bool isSearch, string characterValues) =>
        {
            UIMng.Instance.CLOSE = UIMng.UIName.Loading;
            if (isSearch)
            {
                if (isTest)
                {
                    if (characterValues != "")
                    {
                        string[] datas = characterValues.Split('/')[0].Split(',');
                        int handle = int.Parse(datas[0]);
                        string name = datas[1];
                        PlayerMng.Instance.MainPlayer.Handle = handle;
                        PlayerMng.Instance.MainPlayer.Name = name;
                        RequestLoginCharacter(name, true);
                    }
                }
                else
                UIMng.Instance.GetUI<Title>(UIMng.UIName.Title).SelectCharacter.Open(characterValues);
            }
            else
            {
                UIMng.Instance.GetUI<Title>(UIMng.UIName.Title).Login.Open();
            }
            return true;
        };
    }
    public void RequestLoginCharacter(string name, bool isTest = false)
    {
        UIMng.Instance.OPEN = UIMng.UIName.Loading;
        m_contactProxy.RequestLoginCharacter(HostID.HostID_Server, RmiContext.ReliableSend, name);
        m_contactStub.ReplyLoginCharacter = (Nettention.Proud.HostID remote, Nettention.Proud.RmiContext rmiContext, bool isContact) =>
        {
            UIMng.Instance.CLOSE = UIMng.UIName.Loading;
            if (isContact)
            {
                m_plugin.SetNickname(name);
                SoundMng.Instance.StopMainMusic(0.5f);
                RequestGetCharacter(PlayerMng.Instance.MainPlayer);
                RequestGetCharacterQuest();
                RequestGetCharacterInventory();
                RequestPlayerCharacterSkill();
                RequestGetCharacterQuickSlot();
                RequestPostSearch();
                RequestCharacterJoinMap(isTest);
            }
            return true;
        };
    }
    public void RequestLogoutCharacter()
    {
        if (PlayerMng.Instance.MainPlayer.Character != null)
        {
            UIMng.Instance.OPEN = UIMng.UIName.Loading;
            m_contactProxy.RequestLogoutCharacter(HostID.HostID_Server, RmiContext.ReliableSend);
        }
        m_contactStub.ReplyLogoutCharacter = (Nettention.Proud.HostID remote, Nettention.Proud.RmiContext rmiContext, bool isContact) =>
        {
            UIMng.Instance.OPEN = UIMng.UIName.Loading;
            if (isContact)
            {
                PlayerMng.Instance.ClearPlayerList();
                PlayerMng.Instance.PlayerList.Add(m_netClient.LocalHostID, PlayerMng.Instance.MainPlayer);
                CharacterMng.Instance.ClearCharacterList();
                ItemMng.Instance.ClearItemObject();
                UIMng.Instance.CLOSE = UIMng.UIName.Option;
                UIMng.Instance.CLOSE = UIMng.UIName.Game;
                Destroy(PlayerMng.Instance.MainPlayer.Character.gameObject);
                PlayerMng.Instance.MainPlayer.Name = null;
                PlayerMng.Instance.MainPlayer.Character = null;
                SceneMng.Instance.SetCurrScene(EScene.TitleScene);
            }
            return true;
        };
    }
    public void RequestUseStatPoint(EStatType type)
    {
        UIMng.Instance.OPEN = UIMng.UIName.Loading;
        m_alterProxy.RequestStatusUseStatPoint(HostID.HostID_Server, RmiContext.ReliableSend, type);
        m_alterStub.ReplyStatusUseStatPoint = (Nettention.Proud.HostID remote, Nettention.Proud.RmiContext rmiContext, bool isSuccess) =>
        {
            UIMng.Instance.CLOSE = UIMng.UIName.Loading;
            if (isSuccess)
            {
                PlayerMng.Instance.MainPlayer.StatPoint -= 1;
                switch(type)
                {
                    case EStatType.STR:
                        PlayerMng.Instance.MainPlayer.Character.StatSystem.STR += 1;
                        break;
                    case EStatType.DEX:
                        PlayerMng.Instance.MainPlayer.Character.StatSystem.DEX += 1;
                        break;
                    case EStatType.INT:
                        PlayerMng.Instance.MainPlayer.Character.StatSystem.INT += 1;
                        break;
                    case EStatType.WIS:
                        PlayerMng.Instance.MainPlayer.Character.StatSystem.WIS += 1;
                        break;
                    case EStatType.CON:
                        PlayerMng.Instance.MainPlayer.Character.StatSystem.CON += 1;
                        break;
                }
            }
            return true;
        };
    }
    public void RequestUseSkillPoint(SkillInfo info)
    {
        ECharacterClass Class = PlayerMng.Instance.MainPlayer.Character.StatSystem.BaseStat.Class;
        int Awakening = PlayerMng.Instance.MainPlayer.Character.StatSystem.BaseStat.Awakening;
        int Level = PlayerMng.Instance.MainPlayer.Level;
        int SkillPoint = PlayerMng.Instance.MainPlayer.SkillPoint;

        if ((info.CharacterClass != Class && info.CharacterClass != 0))
        {
            return;
        }
        if(info.SkillPoint > SkillPoint)
        {
            SystemMessage.Instance.PushMessage(SystemMessage.MessageType.Sub, "스킬포인트가 부족합니다.");
            return;
        }
        if (info.CharacterAwakening > Awakening)
        {
            SystemMessage.Instance.PushMessage(SystemMessage.MessageType.Sub, "해당전직은 배울 수 없습니다.");
            return;
        }
        if(info.Level > Level)
        {
            SystemMessage.Instance.PushMessage(SystemMessage.MessageType.Sub, "레벨이 부족합니다.");
            return;
        }
        UIMng.Instance.OPEN = UIMng.UIName.Loading;
        m_alterProxy.RequestStatusUseSkillPoint(HostID.HostID_Server, RmiContext.ReliableSend, info.Handle);
        m_alterStub.ReplyStatusUseSkillPoint = (Nettention.Proud.HostID remote, Nettention.Proud.RmiContext rmiContext, bool isSuccess) =>
        {
            UIMng.Instance.CLOSE = UIMng.UIName.Loading;
            if(isSuccess)
            {
                PlayerMng.Instance.MainPlayer.SkillPoint -= info.SkillPoint;
                CharacterMng.Instance.AddSkill(PlayerMng.Instance.MainPlayer.Character, info.Handle);
                UIMng.Instance.Open<Skill>(UIMng.UIName.Skill).Information.Open();
            }
            return true;
        };
    }
    public void RequestGetCharacter(Player player)
    {
        UIMng.Instance.OPEN = UIMng.UIName.Loading;
        m_informationProxy.RequestGetCharacter(HostID.HostID_Server, RmiContext.ReliableSend, PlayerMng.Instance.MainPlayer.Name);
        m_informationStub.ReplyGetCharacter = (Nettention.Proud.HostID remote, Nettention.Proud.RmiContext rmiContext, bool isSearch, string infoData, string statusData, string equipData) =>
        {
            UIMng.Instance.CLOSE = UIMng.UIName.Loading;
            if (isSearch) 
                ParseLib.SetPlayerData(ref player, -1, EAllyType.Player, infoData, statusData, equipData);
            return true;
        };
    }
    public void RequestGetCharacterInventory()
    {
        UIMng.Instance.OPEN = UIMng.UIName.Loading;
        m_informationProxy.RequestGetCharacterInventory(HostID.HostID_Server, RmiContext.ReliableSend, PlayerMng.Instance.MainPlayer.Name);
        m_informationStub.ReplyGetCharacterInventory = (Nettention.Proud.HostID remote, Nettention.Proud.RmiContext rmiContext, bool isSearch, string data) =>
        {
            UIMng.Instance.CLOSE = UIMng.UIName.Loading;
            if (isSearch)
            {
                ItemMng.Instance.Inventory.Clear();
                string[] values = data.Split('/');
                for(int i =0; i<values.Length; ++i)
                {
                    string[] var = values[i].Split(',');
                    int Handle = int.Parse(var[0]);
                    int Number = int.Parse(var[1]);
                    Item_Base Item = ItemMng.Instance.GetItemList[Handle].Clone();
                    if (Item is IItemNumber)
                        (Item as IItemNumber).Number = Number;
                    ItemMng.Instance.Inventory.Add(Item);
                }
            }
            return true;
        };
    }
    public void RequestGetCharacterQuest()
    {
        UIMng.Instance.OPEN = UIMng.UIName.Loading;
        m_informationProxy.RequestGetCharacterQuest(HostID.HostID_Server, RmiContext.ReliableSend, PlayerMng.Instance.MainPlayer.Name);
        m_informationStub.ReplyGetCharacterQuest = (Nettention.Proud.HostID remote, Nettention.Proud.RmiContext rmiContext, bool isSearch, string data) =>
        {
            UIMng.Instance.CLOSE = UIMng.UIName.Loading;
            if (isSearch) CharacterMng.Instance.InitQuest(data);
            return true;
        };
    }
    public void RequestGetCharacterQuickSlot()
    {
        UIMng.Instance.OPEN = UIMng.UIName.Loading;
        m_informationProxy.RequestGetQuickSlot(HostID.HostID_Server, RmiContext.ReliableSend, PlayerMng.Instance.MainPlayer.Name);
        m_informationStub.ReplyGetCharacterQuickSlot = (Nettention.Proud.HostID remote, Nettention.Proud.RmiContext rmiContext, bool isSearch, string data) =>
        {
            UIMng.Instance.CLOSE = UIMng.UIName.Loading;
            if (isSearch)
            {
                string[] quickSlotData = data.Split('/');
                for(int i =0; i<quickSlotData.Length; ++i)
                {
                    string[] parse = quickSlotData[i].Split(',');
                    int number = int.Parse(parse[0]);
                    bool isItem = bool.Parse(parse[1]);
                    int handle = int.Parse(parse[2]);
                    if(isItem)
                    {
                        for(int j = 0; j< ItemMng.Instance.Inventory.Count; ++j)
                        {
                            Item_Base item = ItemMng.Instance.Inventory[j];
                            if (item.Handle == handle)
                            {
                                PlayerMng.Instance.MainPlayer.Quickslot[number] = item as IQuickSlotable;
                                break;
                            }
                        }
                    }
                    else
                    {
                        foreach(BaseSkill skill in PlayerMng.Instance.MainPlayer.Character.AttackSystem.SkillDic.Values)
                        {
                            if(skill.SkillInfo.Handle == handle)
                            {
                                PlayerMng.Instance.MainPlayer.Quickslot[number] = skill as IQuickSlotable;
                                break;
                            }
                        }
                    }
                }
            }
            return true;
        };
    }
    public void RequestDeleteQuickSlot(int number)
    {
        UIMng.Instance.OPEN = UIMng.UIName.Loading;
        m_alterProxy.RequestDeleteQuickSlot(HostID.HostID_Server, RmiContext.ReliableSend, number);
        m_alterStub.ReplyDeleteQuickSlot = (Nettention.Proud.HostID remote, Nettention.Proud.RmiContext rmiContext, bool isSuccess) =>
        {
            UIMng.Instance.CLOSE = UIMng.UIName.Loading;
            if (isSuccess)
            {
                PlayerMng.Instance.MainPlayer.Quickslot[number] = null;
                UIMng.Instance.GetUI<Game>(UIMng.UIName.Game).InputWindow.ResetQuickSlot(number);
            }
            return true;
        };
    }
    public void RequestSetQuickSlot<T>(int number, T item, bool isItem, int handle) where T : class
    {
        UIMng.Instance.OPEN = UIMng.UIName.Loading;
        m_alterProxy.RequestSetQuickSlot(HostID.HostID_Server, RmiContext.ReliableSend, number, isItem, handle);
        m_alterStub.ReplySetQuickSlot = (Nettention.Proud.HostID remote, Nettention.Proud.RmiContext rmiContext, bool isSuccess) =>
        {
            UIMng.Instance.CLOSE = UIMng.UIName.Loading;
            if (isSuccess)
            {
                PlayerMng.Instance.MainPlayer.Quickslot[number] = item as IQuickSlotable;
                UIMng.Instance.GetUI<Game>(UIMng.UIName.Game).InputWindow.ResetQuickSlot(number);
            }
            return true;
        };
    }
    public void RequestPlayerCharacterSkill()
    {
        UIMng.Instance.OPEN = UIMng.UIName.Loading;
        m_informationProxy.RequestGetCharacterSkill(HostID.HostID_Server, RmiContext.ReliableSend, PlayerMng.Instance.MainPlayer.Name);
        m_informationStub.ReplyGetCharacterSkill = (Nettention.Proud.HostID remote, Nettention.Proud.RmiContext rmiContext, bool isSearch, string data) =>
        {
            if (isSearch)
            {
                UIMng.Instance.CLOSE = UIMng.UIName.Loading;
                string[] skillData = data.Split('/');
                for(int i =0; i<skillData.Length; ++i)
                {
                    string[] parse = skillData[i].Split(',');
                    int handle = int.Parse(parse[0]);
                    int level = int.Parse(parse[1]);
                    CharacterMng.Instance.AddSkill(PlayerMng.Instance.MainPlayer.Character, handle);
                }
            }
            return true;
        };
    }
    public void RequestQuestAccept(int handle)
    {
        if(CharacterMng.Instance.CurrQuest.ContainsKey(handle))
            return;
        if (!CharacterMng.Instance.AcceptCheck(handle, PlayerMng.Instance.MainPlayer.Character.StatSystem.Level))
            return;
        UIMng.Instance.OPEN = UIMng.UIName.Loading;
        m_alterProxy.RequestQuestAccept(HostID.HostID_Server, RmiContext.ReliableSend, handle);
        m_alterStub.ReplyQuestAccept = (Nettention.Proud.HostID remote, Nettention.Proud.RmiContext rmiContext, bool isSuccess) =>
        {
            UIMng.Instance.CLOSE = UIMng.UIName.Loading;
            if (isSuccess)
            {
                CharacterMng.Instance.AcceptQuest(handle);
                SystemMessage.Instance.PushMessage(SystemMessage.MessageType.Main, "임무를 수락하였습니다.");
            }
            return true;
        };
    }
    public void RequestQuestDelete(int handle)
    {
        if (!CharacterMng.Instance.CurrQuest.ContainsKey(handle))
            return;
        if (!CharacterMng.Instance.CurrQuest[handle].Accept)
            return;
        UIMng.Instance.OPEN = UIMng.UIName.Loading;
        m_alterProxy.RequestQuestDelete(HostID.HostID_Server, RmiContext.ReliableSend, handle);
        m_alterStub.ReplyQuestDelete = (Nettention.Proud.HostID remote, Nettention.Proud.RmiContext rmiContext, bool isSuccess) =>
        {
            UIMng.Instance.CLOSE = UIMng.UIName.Loading;
            if (isSuccess)
            {
                CharacterMng.Instance.DeleteQuest(handle);
                UIMng.Instance.OPEN = UIMng.UIName.QuestUI;
            }
            return true;
        };
    }
    public void RequestQuestClear(int handle, BaseNPC npc)
    {
        if (!CharacterMng.Instance.CurrQuest.ContainsKey(handle))
            return;
        if (!CharacterMng.Instance.CurrQuest[handle].Accept)
            return;
        if (CharacterMng.Instance.CurrQuest[handle].Clear)
            return;
        if (ItemMng.Instance.Inventory.Count + CharacterMng.Instance.CurrQuest[handle].CurrQuest.Reword.Count > ItemMng.Instance.MaxInventorySize)
        {
            SystemMessage.Instance.PushMessage(SystemMessage.MessageType.Sub, "인벤토리 공간이 부족합니다.");
            return;
        }
        UIMng.Instance.OPEN = UIMng.UIName.Loading;
        m_alterProxy.RequestQuestClear(HostID.HostID_Server, RmiContext.ReliableSend, handle);
        m_alterStub.ReplyQuestClear = (Nettention.Proud.HostID remote, Nettention.Proud.RmiContext rmiContext, bool isSuccess) =>
        {
            UIMng.Instance.CLOSE = UIMng.UIName.Loading;
            if (isSuccess)
            {
                SystemMessage.Instance.PushMessage(SystemMessage.MessageType.Main, CharacterMng.Instance.CurrQuest[handle].CurrQuest.ContentName + "을(를) 완수하였습니다.");
                List<SQuestReword> Rewords = CharacterMng.Instance.CurrQuest[handle].CurrQuest.Reword;
                int rewordGold = CharacterMng.Instance.CurrQuest[handle].CurrQuest.RewordGold;
                int rewordEXP = CharacterMng.Instance.CurrQuest[handle].CurrQuest.RewordEXP;
                for (int i = 0; i < Rewords.Count; ++i)
                {
                    if (ItemMng.Instance.GetItemList.ContainsKey(Rewords[i].Handle))
                    {
                        Item_Base Item = ItemMng.Instance.GetItemList[Rewords[i].Handle].Clone();
                        if (Item is IItemNumber)
                            (Item as IItemNumber).Number = Rewords[i].Value;
                        ItemMng.Instance.AddItem(Item);
                    }
                }
                if(rewordGold > 0)
                {
                    PlayerMng.Instance.MainPlayer.Gold += rewordGold;
                }
                if(rewordEXP > 0)
                {
                    PlayerMng.Instance.AddExp(PlayerMng.Instance.MainPlayer, rewordEXP);
                }
                SQuestReword[] reword = CharacterMng.Instance.CurrQuest[handle].CurrQuest.Reword.ToArray();
                if(reword.Length > 0)
                    UIMng.Instance.Open<SelectPopup>(UIMng.UIName.SelectPopup).RewordPopup.Enabled(rewordEXP, reword);

                CharacterMng.Instance.CompleteQuest(handle);
                UIMng.Instance.Open<NPCUI>(UIMng.UIName.NPCUI).Enabled(npc);
            }
            return true;
        };
    }
    public void RequestItemUse(Item_Base item)
    {
        if(!ItemMng.Instance.IsCoolTimeComplete(item))
            return;
        if (!ItemMng.Instance.IsNumberCheck(item))
            return;;
        m_alterProxy.RequestItemUse(HostID.HostID_Server, RmiContext.ReliableSend, item.Handle);
        m_alterStub.ReplyItemUse = (Nettention.Proud.HostID remote, Nettention.Proud.RmiContext rmiContext, bool isSuccess) =>
        {
            if (isSuccess) ItemMng.Instance.ItemUseAction(m_netClient.LocalHostID, item);
            return true;
        };
    }
    public void RequestItemEquip(int number)
    {
        BaseHero character = PlayerMng.Instance.MainPlayer.Character;
        Item_Base item = ItemMng.Instance.Inventory[number];
        if (item.Type < (EItemType)3)
            return;
        IItemLevel level = item as IItemLevel;
        if (character.StatSystem.Level < level.Level)
            return;
        UIMng.Instance.OPEN = UIMng.UIName.Loading;
        m_alterProxy.RequestItemEquip(HostID.HostID_Server, RmiContext.ReliableSend, number);
        m_alterStub.ReplyItemEquip = (Nettention.Proud.HostID remote, Nettention.Proud.RmiContext rmiContext, bool isSuccess) =>
        {
            UIMng.Instance.CLOSE = UIMng.UIName.Loading;
            if (isSuccess)
            {
                ItemMng.Instance.EquipItem(number);
                RmiContext Rmi = RmiContext.ReliableSend;
                Rmi.enableLoopback = false;
                m_behaviorP2PStub.NotifyItemEquip(m_groupID, Rmi, item.Handle);
            }
            return true;
        };
    }
    public void RequestItemUnequip(EItemType type)
    {
        BaseHero character = PlayerMng.Instance.MainPlayer.Character;
        if(character.StatSystem.GetEquipment(type) == null)
            return;
        if (!ItemMng.Instance.CheckInventoryCount(character.StatSystem.GetEquipment(type)))
            return;
        UIMng.Instance.OPEN = UIMng.UIName.Loading;
        m_alterProxy.RequestItemUnEquip(HostID.HostID_Server, RmiContext.ReliableSend, type);
        m_alterStub.ReplyItemUnEquip = (Nettention.Proud.HostID remote, Nettention.Proud.RmiContext rmiContext, bool isSuccess) =>
        {
            UIMng.Instance.CLOSE = UIMng.UIName.Loading;
            if (isSuccess)
            {
                ItemMng.Instance.DisArmament(type);
                RmiContext Rmi = RmiContext.ReliableSend;
                Rmi.enableLoopback = false;
                m_behaviorP2PStub.NotifyItemUnequip(m_groupID, Rmi, type);
            }
            return true;
        };
    }
    public void RequestItemBuy(int handle)
    {
        UIMng.Instance.OPEN = UIMng.UIName.Loading;
        m_alterProxy.RequestItemBuy(HostID.HostID_Server, RmiContext.ReliableSend, handle);
        m_alterStub.ReplyItemBuy = (Nettention.Proud.HostID remote, Nettention.Proud.RmiContext rmiContext, bool isSuccess) =>
        {
            UIMng.Instance.CLOSE = UIMng.UIName.Loading;
            if (isSuccess)
            {
                Item_Base item = ItemMng.Instance.GetItemList[handle].Clone();
                ItemMng.Instance.AddItem(item);
                PlayerMng.Instance.MainPlayer.Gold -= item.Price;
            }
            return true;
        };
    }
    public void RequestItemBuyNumber(int handle, int number)
    {
        UIMng.Instance.OPEN = UIMng.UIName.Loading;
        m_alterProxy.RequestItemBuyNumber(HostID.HostID_Server, RmiContext.ReliableSend, handle, number);
        m_alterStub.ReplyItemBuyNumber = (Nettention.Proud.HostID remote, Nettention.Proud.RmiContext rmiContext, bool isSuccess) =>
        {
            UIMng.Instance.CLOSE = UIMng.UIName.Loading;
            if (isSuccess)
            {
                Item_Base item = ItemMng.Instance.GetItemList[handle].Clone();
                (item as IItemNumber).Number = number;
                ItemMng.Instance.AddItem(item);
                PlayerMng.Instance.MainPlayer.Gold -= item.Price * number;
            }
            return true;
        };
    }
    public void RequestItemSell(int inventoryNum)
    {
        UIMng.Instance.OPEN = UIMng.UIName.Loading;
        m_alterProxy.RequestItemSell(HostID.HostID_Server, RmiContext.ReliableSend, inventoryNum);
        m_alterStub.ReplyItemSell = (Nettention.Proud.HostID remote, Nettention.Proud.RmiContext rmiContext, bool isSuccess) =>
        {
            UIMng.Instance.CLOSE = UIMng.UIName.Loading;
            if (isSuccess)
            {
                Item_Base item = ItemMng.Instance.Inventory[inventoryNum];
                PlayerMng.Instance.MainPlayer.Gold += item.Price / 5;
                ItemMng.Instance.RemoveItem(item);
            }
            return true;
        };
    }
    public void RequestItemSellNumber(int inventoryNum, int number)
    {
        UIMng.Instance.OPEN = UIMng.UIName.Loading;
        m_alterProxy.RequestItemSellNumber(HostID.HostID_Server, RmiContext.ReliableSend, inventoryNum, number);
        m_alterStub.ReplyItemSellNumber = (Nettention.Proud.HostID remote, Nettention.Proud.RmiContext rmiContext, bool isSuccess) =>
        {
            UIMng.Instance.CLOSE = UIMng.UIName.Loading;
            if (isSuccess)
            {
                Item_Base item = ItemMng.Instance.Inventory[inventoryNum];
                PlayerMng.Instance.MainPlayer.Gold += item.Price * number / 5;
                ItemMng.Instance.RemoveItem(item, number);
            }
            return true;
        };
    }
    public void RequestItemDelete(int inventoryNum)
    {
        UIMng.Instance.OPEN = UIMng.UIName.Loading;
        m_alterProxy.RequestItemDelete(HostID.HostID_Server, RmiContext.ReliableSend, inventoryNum);
        m_alterStub.ReplyItemDelete = (Nettention.Proud.HostID remote, Nettention.Proud.RmiContext rmiContext, bool isSuccess) =>
        {
            UIMng.Instance.CLOSE = UIMng.UIName.Loading;
            if (isSuccess) ItemMng.Instance.RemoveItem(inventoryNum);
            return true;
        };
    }
    public void RequestItemDeleteNumber(int inventoryNum, int number)
    {
        UIMng.Instance.OPEN = UIMng.UIName.Loading;
        m_alterProxy.RequestItemDeleteNumber(HostID.HostID_Server, RmiContext.ReliableSend, inventoryNum, number);
        m_alterStub.ReplyItemDeleteNumber = (Nettention.Proud.HostID remote, Nettention.Proud.RmiContext rmiContext, bool isSuccess) =>
        {
            UIMng.Instance.CLOSE = UIMng.UIName.Loading;
            if (isSuccess) ItemMng.Instance.RemoveItem(inventoryNum, number);
            return true;
        };
    }
    public void RequestProduceItem(int handle)
    {
        UIMng.Instance.OPEN = UIMng.UIName.Loading;
        m_alterProxy.RequestItemProduce(HostID.HostID_Server, RmiContext.ReliableSend, handle);
        m_alterStub.ReplyItemProduce = (Nettention.Proud.HostID remote, Nettention.Proud.RmiContext rmiContext, bool isSuccess, int produceHandle, int errorCode) =>
        {
            UIMng.Instance.CLOSE = UIMng.UIName.Loading;
            Debug.Log(isSuccess);
            if (isSuccess)
            {
                ItemMng.Instance.GetItemProduceFormula(produceHandle);
            }
            return true;
        };
    }
    public void RequestInventorySort()
    {
        UIMng.Instance.OPEN = UIMng.UIName.Loading;
        m_alterProxy.RequestInvenvorySort(HostID.HostID_Server, RmiContext.ReliableSend);
        m_alterStub.ReplyInventorySort = (Nettention.Proud.HostID remote, Nettention.Proud.RmiContext rmiContext, bool isSuccess) =>
        {
            UIMng.Instance.CLOSE = UIMng.UIName.Loading;
            if (isSuccess) ItemMng.Instance.Inventory.Sort((x, y) => ItemMng.Instance.IComparerItem(x, y));
            return true;
        };
    }
    public void RequestGetReword(int uniqueID)
    {
        m_battleProxy.RequestGetReword(HostID.HostID_Server, RmiContext.ReliableSend, uniqueID);
    }
    public void RequestChangePublicChannel(int channel)
    {
        if (m_currChannel == channel)
            return;

        UIMng.Instance.OPEN = UIMng.UIName.Loading;
        m_contactProxy.RequestChangeChannel(HostID.HostID_Server, RmiContext.ReliableSend, channel);
        m_contactStub.ReplyChangeChannel = (Nettention.Proud.HostID remote, Nettention.Proud.RmiContext rmiContext, bool isChange, HostID groupID) =>
        {
            UIMng.Instance.CLOSE = UIMng.UIName.Loading;
            if (!isChange)
            {
                SystemMessage.Instance.PushMessage(SystemMessage.MessageType.Sub, "채널변경에 실패했습니다.");
                return true;
            }
            m_groupID = groupID;
            m_currChannel = channel;
            UIMng.Instance.Open<Game>(UIMng.UIName.Game).DeviceWindow.CurrChannel = channel;
            ItemMng.Instance.ClearItemObject();
            PlayerMng.Instance.ClearPlayerList();
            PlayerMng.Instance.PlayerList.Add(m_netClient.LocalHostID, PlayerMng.Instance.MainPlayer);
            CharacterMng.Instance.ClearCharacterList();
            CharacterMng.Instance.CurrCharacters.Add(PlayerMng.Instance.MainPlayer.Character.UniqueID, PlayerMng.Instance.MainPlayer.Character);
            NotifyJoinGroup();

            return true;
        };
    }
    public void RequestCharacterJoinMap(bool isTest = false)
    {
        UIMng.Instance.OPEN = UIMng.UIName.Loading;
        m_behaviorProxy.RequestCharacterJoinMap(HostID.HostID_Server, RmiContext.ReliableSend);
        m_behaviorStub.ReplyCharacterJoinMap = (Nettention.Proud.HostID remote, Nettention.Proud.RmiContext rmiContext, bool isSuccess, Nettention.Proud.HostID groupID, int uniqueID, int channel, int mapHandle, UnityEngine.Vector3 pos) =>
        {
            UIMng.Instance.CLOSE = UIMng.UIName.Loading;
            if (isSuccess) 
            {
                if (!isTest)
                    UIMng.Instance.CLOSE = UIMng.UIName.Title;

                m_currChannel = channel;
                m_groupID = groupID;
                PlayerMng.Instance.MainPlayer.Character.UniqueID = uniqueID;
                CameraMng.Instance.SetCamera(CameraMng.CameraStyle.Player);
                CameraMng.Fade_ON(1f);
                MapMng.Instance.SetCurrMap(PlayerMng.Instance.MainPlayer.CurrMapHandle);
                Map map = MapMng.Instance.CurrMap;
                SceneMng.Instance.SetGameScene(map.SceneName, StartAfterAction);
                PlayerMng.Instance.MainPlayer.Character.MoveSystem.SetPosition = pos;
            }
            return true;
        };
    }
    public void RequestCharacterJoinPublicPortal(int number)
    {
        UIMng.Instance.OPEN = UIMng.UIName.Loading;
        m_behaviorProxy.RequestCharacterJoinPublicPortal(HostID.HostID_Server, RmiContext.ReliableSend, MapMng.Instance.CurrMap.Handle, number);
    }
    public void RequestCharacterJoinPrivateMap(int handle)
    {
        m_behaviorProxy.RequestCharacterJoinPrivateMap(HostID.HostID_Server, RmiContext.ReliableSend, handle);
    }
    public void RequestCreateParty(string name, int number, int level)
    {
        if (PlayerMng.Instance.CurrParty != null)
            return;
        UIMng.Instance.OPEN = UIMng.UIName.Loading;
        m_behaviorProxy.RequestPartyCreate(HostID.HostID_Server, RmiContext.ReliableSend, name, number, level);
        m_behaviorStub.ReplyPartyCreate = (Nettention.Proud.HostID remote, Nettention.Proud.RmiContext rmiContext, bool isSuccess, int handle) =>
        {
            UIMng.Instance.CLOSE = UIMng.UIName.Loading;
            if (isSuccess)
            {
                PlayerMng.Instance.CurrParty = new Party(handle, PlayerMng.Instance.MainPlayer.HostID, name, number, level);
                PlayerMng.Instance.CurrParty.PartyMemberList.Add(m_netClient.LocalHostID);
                UIMng.Instance.OPEN = UIMng.UIName.PartyMenu;
            }
            return true;
        };
    }
    public void RequestJoinParty(int handle)
    {
        if (PlayerMng.Instance.CurrParty != null)
            return;
        UIMng.Instance.OPEN = UIMng.UIName.Loading;
        m_behaviorProxy.RequestPartyJoin(HostID.HostID_Server, RmiContext.ReliableSend, handle);
        m_behaviorStub.ReplyPartyJoin = (Nettention.Proud.HostID remote, Nettention.Proud.RmiContext rmiContext, bool isSuccess, string name, Nettention.Proud.HostID id, int number, int level, string members) =>
        {
            UIMng.Instance.CLOSE = UIMng.UIName.Loading;
            if (isSuccess)
            {
                PlayerMng.Instance.CurrParty = new Party(handle, id, name, number, level);
                string[] member = members.Split('/');
                for (int i = 0; i < member.Length; ++i)
                {
                    string[] status = member[i].Split(',');
                    HostID ID = (HostID)int.Parse(status[0]);
                    PlayerMng.Instance.CurrParty.PartyMemberList.Add(ID);
                    if (!PlayerMng.Instance.PlayerList.ContainsKey(ID))
                    {
                        Player Player = new Player();
                        Player.HostID = ID;
                        Player.Name = status[1];
                        Player.Handle = int.Parse(status[2]);
                        Player.Level = int.Parse(status[3]);
                        PlayerMng.Instance.PlayerList.Add(ID, Player);
                    }
                }
                PlayerMng.Instance.CurrParty.Join(m_netClient.LocalHostID);
            }
            else
            {
                switch(members)
                {
                    case "0":
                        SystemMessage.Instance.PushMessage(SystemMessage.MessageType.Sub, "이미 가입중인 파티가 있습니다.");
                        break;
                    case "1":
                        SystemMessage.Instance.PushMessage(SystemMessage.MessageType.Sub, "존재하지 않는 파티입니다.");
                        break;
                    case "2":
                        SystemMessage.Instance.PushMessage(SystemMessage.MessageType.Sub, "인원이 가득 찼습니다.");
                        break;
                    case "3":
                        SystemMessage.Instance.PushMessage(SystemMessage.MessageType.Sub, "레벨이 부족합니다.");
                        break;
                }
            }
            UIMng.Instance.OPEN = UIMng.UIName.PartyMenu;
            return true;
        };
    }
    public void RequestLeaveParty()
    {
        if (PlayerMng.Instance.CurrParty == null)
            return;
        UIMng.Instance.OPEN = UIMng.UIName.Loading;
        m_behaviorProxy.RequestPartyLeave(HostID.HostID_Server, RmiContext.ReliableSend);
        m_behaviorStub.ReplyPartyLeave = (Nettention.Proud.HostID remote, Nettention.Proud.RmiContext rmiContext, bool isSuccess) =>
        {
            UIMng.Instance.CLOSE = UIMng.UIName.Loading;
            if (isSuccess)
            {
                PlayerMng.Instance.CurrParty = null;
                UIMng.Instance.OPEN = UIMng.UIName.PartyMenu;
            }
            return true;
        };
    }
    public void RequestAppointParty(HostID id)
    {
        if (PlayerMng.Instance.CurrParty == null)
            return;

        if (PlayerMng.Instance.CurrParty.PartyHost != PlayerMng.Instance.MainPlayer.HostID)
            return;

        m_behaviorProxy.RequestPartyAppointHost(HostID.HostID_Server, RmiContext.ReliableSend, id);
    }
    public void RequestChangePartyState(EPartyState state)
    {
        if (PlayerMng.Instance.CurrParty == null)
            return;

        if (PlayerMng.Instance.CurrParty.PartyHost != PlayerMng.Instance.MainPlayer.HostID)
            return;

        m_behaviorProxy.RequestPartySetState(HostID.HostID_Server, RmiContext.ReliableSend, state);
    }
    public void RequestExitPrivateMap()
    {
        if (MapMng.Instance.CurrMap.Type == EMapType.Public)
            return;

        UIMng.Instance.OPEN = UIMng.UIName.Loading;
        m_behaviorProxy.RequestCharacterExitPrivateMap(HostID.HostID_Server, RmiContext.ReliableSend);
        m_behaviorStub.ReplyCharacterExitPrivateMap = (Nettention.Proud.HostID remote, Nettention.Proud.RmiContext rmiContext, bool isSuccess, Nettention.Proud.HostID groupID, int uniqueID, int channel, int mapHandle, UnityEngine.Vector3 pos) =>
        {
            UIMng.Instance.CLOSE = UIMng.UIName.Loading;
            if (isSuccess)
            {
                m_enermyData = "";
                m_groupID = groupID;
                m_currChannel = channel;
                PlayerMng.Instance.MainPlayer.Character.UniqueID = uniqueID;
                UIMng.Instance.CLOSE = UIMng.UIName.Game;
                PlayerMng.Instance.MainPlayer.CurrMapHandle = mapHandle;
                MapMng.Instance.SetCurrMap(PlayerMng.Instance.MainPlayer.CurrMapHandle);
                Map map = MapMng.Instance.CurrMap;
                SceneMng.Instance.SetGameScene(map.SceneName, ContinueAfterAction);
                PlayerMng.Instance.MainPlayer.Character.MoveSystem.SetPosition = pos;
                if (PlayerMng.Instance.MainPlayer.Character.State == BaseCharacter.CharacterState.Death)
                    StartCoroutine(PlayerMng.Instance.MainPlayer.Character.RevivalAfterTime(5));
            }
            else
            {

            }
            return true;
        };
    }
    public void RequestEnermyKill(int uniqueID)
    {
        m_battleProxy.RequestEnermyKill(HostID.HostID_Server, RmiContext.ReliableSend, uniqueID);
    }
    public void RequestEnermyInstantiate(int handle, Vector3 pos, float angle)
    {
        m_battleProxy.RequestEnermyInstantiate(HostID.HostID_Server, RmiContext.ReliableSend, handle, pos, angle);
    }
    void StartAfterAction()
    {
        // PlayerMng.Instance.MainPlayer.Character = CharacterMng.Instance.InstantiateHero(PlayerMng.Instance.MainPlayer.Handle, 0, EAllyType.Player, Vector3.zero, new Dictionary<EItemType, Item_Equipment>(), PlayerMng.Instance.MainPlayer.Name);
        ItemMng.Instance.ClearItemObject();
        PlayerMng.Instance.ClearPlayerList();
        PlayerMng.Instance.PlayerList.Add(m_netClient.LocalHostID, PlayerMng.Instance.MainPlayer);
        CharacterMng.Instance.ClearCharacterList();
        CharacterMng.Instance.CurrCharacters.Add(PlayerMng.Instance.MainPlayer.Character.UniqueID, PlayerMng.Instance.MainPlayer.Character);
        UIMng.Instance.Open<Game>(UIMng.UIName.Game).DeviceWindow.CurrChannel = m_currChannel;
        CameraMng.Instance.SetCamera(CameraMng.CameraStyle.Player | CameraMng.CameraStyle.UI);
        Map map = MapMng.Instance.CurrMap;
        SoundMng.Instance.PlayMainMusic(map.BGM, 1f);
        InstantiatePortal(map);
        NotifyJoinGroup();
    }
    void ContinueAfterAction()
    {
        ItemMng.Instance.ClearItemObject();
        PlayerMng.Instance.ClearPlayerList();
        PlayerMng.Instance.PlayerList.Add(m_netClient.LocalHostID, PlayerMng.Instance.MainPlayer);
        CharacterMng.Instance.ClearCharacterList();
        CharacterMng.Instance.CurrCharacters.Add(PlayerMng.Instance.MainPlayer.Character.UniqueID, PlayerMng.Instance.MainPlayer.Character);
        CameraMng.Fade_OFF(1f);
        UIMng.Instance.Open<Game>(UIMng.UIName.Game).DeviceWindow.CurrChannel = m_currChannel;
        CameraMng.Instance.SetCamera(CameraMng.CameraStyle.Player | CameraMng.CameraStyle.UI);
        Map map = MapMng.Instance.CurrMap;
        SoundMng.Instance.PlayMainMusic(map.BGM, 1f);
        InstantiatePortal(map);
        InstantiateEnermy(map);
        NotifyJoinGroup();
    }
    void InstantiatePortal(Map map)
    {
        if (map.MatchPortalList.Count != 0)
        {
            MatchPortal MatchPortal = Resources.Load<MatchPortal>("Content/MatchPortal");

            for (int i = 0; i < map.MatchPortalList.Count; ++i)
                Instantiate(MatchPortal, map.MatchPortalList[i].Coord, Quaternion.identity).Number = i;
        }
        if (map.PortalList.Count != 0)
        {
            if (map.Type == EMapType.Public)
            {
                WayPortal WayPortal = Resources.Load<WayPortal>("Content/WayPortal");
                for (int i = 0; i < map.PortalList.Count; ++i)
                    Instantiate(WayPortal, map.PortalList[i].Coord, Quaternion.identity).Number = i;
            }
            else
            {
                PrivatePortal PrivatePortal = Resources.Load<PrivatePortal>("Content/PrivatePortal");
                for (int i = 0; i < map.PortalList.Count; ++i)
                    Instantiate(PrivatePortal, map.PortalList[i].Coord, Quaternion.identity).Handle = map.PortalList[i].Handle;
            }
        }
    }
    void InstantiateEnermy(Map map)
    {
        if (m_enermyData == "")
            return;

        string[] enermy = m_enermyData.Split(',');
        for (int i = 0; i < enermy.Length; ++i)
        {
            int uniqueID = int.Parse(enermy[i]);
            CharacterMng.Instance.InstantiateMonster(MapMng.Instance.CurrMap.MonsterList[i].Handle, uniqueID, EAllyType.Hostile, map.MonsterList[i].Coord, map.MonsterList[i].Angle);
        }
    }
    void ReceiveServer()
    {
        m_netClient.JoinServerCompleteHandler = (ErrorInfo info, ByteArray replyFromSetver) =>
        {
            if (info.errorType == ErrorType.Ok)
            {
                NetworkState = ENetworkState.ConnectOn;
                PlayerMng.Instance.PlayerList.Add(m_netClient.GetLocalHostID(), PlayerMng.Instance.MainPlayer);
                PlayerMng.Instance.MainPlayer.HostID = m_netClient.GetLocalHostID();
            }
            else
            {
                NetworkState = ENetworkState.Offline;
            }
        };
        m_netClient.LeaveServerHandler = (ErrorInfo info) =>
        {
            DisconnectServer();
        };
        m_netClient.ServerOfflineHandler = (RemoteOfflineEventArgs args) =>
        {
            NetworkState = ENetworkState.Offline;
            m_netClient.Disconnect();
        };
        m_behaviorStub.NotifyRespawnEnermy = (Nettention.Proud.HostID remote, Nettention.Proud.RmiContext rmiContext, int handle, int uniqueID, UnityEngine.Vector3 pos, float angle, float respawnTime) =>
        {
            Map map = MapMng.Instance.CurrMap;
            CharacterMng.Instance.InstantiateMonster(handle, uniqueID, EAllyType.Hostile, pos, angle);
            return true;
        };
        m_behaviorStub.NotifyPartyAppointHost = (Nettention.Proud.HostID remote, Nettention.Proud.RmiContext rmiContext, Nettention.Proud.HostID id) =>
        {
            PlayerMng.Instance.CurrParty.PartyHost = id;
            UIMng.Instance.OPEN = UIMng.UIName.PartyMenu;
            return true;
        };
        m_behaviorStub.NotifyPartyJoinMember = (Nettention.Proud.HostID remote, Nettention.Proud.RmiContext rmiContext, Nettention.Proud.HostID id, string name, int handle, int level) =>
        {
            if (!PlayerMng.Instance.PlayerList.ContainsKey(id))
            {
                Player player = new Player();
                player.HostID = id;
                player.Name = name;
                player.Handle = handle;
                player.Level = level;
                PlayerMng.Instance.PlayerList.Add(id, player);
            }
            PlayerMng.Instance.CurrParty.Join(id);
            return true;
        };
        m_behaviorStub.NotifyPartyLeaveMember = (Nettention.Proud.HostID remote, Nettention.Proud.RmiContext rmiContext, Nettention.Proud.HostID id) =>
        {
            PlayerMng.Instance.CurrParty.Leave(id);
            return true;
        };
        m_behaviorStub.ReplyCharacterJoinPublicPortal = (Nettention.Proud.HostID remote, Nettention.Proud.RmiContext rmiContext, bool isSuccess, Nettention.Proud.HostID groupID, int uniqueID, int channel, int mapHandle, UnityEngine.Vector3 pos) =>
        {
            UIMng.Instance.CLOSE = UIMng.UIName.Loading;
            if (isSuccess)
            {
                m_groupID = groupID;
                m_currChannel = channel;
                PlayerMng.Instance.MainPlayer.Character.UniqueID = uniqueID;
                UIMng.Instance.CLOSE = UIMng.UIName.Game;
                PlayerMng.Instance.MainPlayer.CurrMapHandle = mapHandle;
                MapMng.Instance.SetCurrMap(PlayerMng.Instance.MainPlayer.CurrMapHandle);
                Map map = MapMng.Instance.CurrMap;
                SceneMng.Instance.SetGameScene(map.SceneName, ContinueAfterAction);
                PlayerMng.Instance.MainPlayer.Character.MoveSystem.SetPosition = pos;
            }
            return true;
        };
        m_behaviorStub.ReplyCharacterJoinPrivateMap = (Nettention.Proud.HostID remote, Nettention.Proud.RmiContext rmiContext, bool isSuccess, Nettention.Proud.HostID groupID, int uniqueID, int channel, int mapHandle, UnityEngine.Vector3 pos, string enermyData) =>
        {
            if (isSuccess)
            {
                if (PlayerMng.Instance.CurrParty != null)
                {
                    PlayerMng.Instance.CurrParty.State = EPartyState.Start;
                    if (PlayerMng.Instance.CurrParty.PartyHost == m_netClient.LocalHostID)
                    {
                        UIMng.Instance.CLOSE = UIMng.UIName.SelectStage;
                        PlayerMng.Instance.CurrParty.ReadyMemberList.Clear();
                    }
                    else
                    {
                        UIMng.Instance.CLOSE = UIMng.UIName.SelectPopup;
                    }
                }
                else UIMng.Instance.CLOSE = UIMng.UIName.SelectStage;

                m_enermyData = enermyData;
                m_groupID = groupID;
                m_currChannel = channel;
                PlayerMng.Instance.MainPlayer.Character.UniqueID = uniqueID;
                UIMng.Instance.CLOSE = UIMng.UIName.Game;
                PlayerMng.Instance.MainPlayer.CurrMapHandle = mapHandle;
                MapMng.Instance.SetCurrMap(PlayerMng.Instance.MainPlayer.CurrMapHandle);
                Map map = MapMng.Instance.CurrMap;
                SceneMng.Instance.SetGameScene(map.SceneName, ContinueAfterAction);
                PlayerMng.Instance.MainPlayer.Character.MoveSystem.SetPosition = pos;
            }
            else
            {
                SystemMessage.Instance.PushMessage(SystemMessage.MessageType.Sub, "매칭이 취소되었습니다.");
                if (PlayerMng.Instance.CurrParty != null)
                {
                    if (PlayerMng.Instance.CurrParty.PartyHost == m_netClient.LocalHostID)
                    {
                        UIMng.Instance.GetUI<SelectStage>(UIMng.UIName.SelectStage).OnMatchCancle();
                        PlayerMng.Instance.CurrParty.ReadyMemberList.Clear();
                    }
                    else
                    {
                        UIMng.Instance.CLOSE = UIMng.UIName.SelectPopup;
                    }
                }
                else UIMng.Instance.GetUI<SelectStage>(UIMng.UIName.SelectStage).OnMatchCancle();
            }
            return true;
        };
        m_battleStub.NotifyEnermyKill = (Nettention.Proud.HostID remote, Nettention.Proud.RmiContext rmiContext, bool isSuccess, int uniqueID, int exp, int gold, string itemArr) =>
        {
            if (isSuccess)
            {
                // 사냥퀘스트체크
                foreach (Quest quest in CharacterMng.Instance.CurrQuest.Values)
                {
                    if (quest.CurrQuest.Type == EQuestClearType.Hunt && quest.CurrQuest.ClearHandle == CharacterMng.Instance.CurrCharacters[uniqueID].StatSystem.BaseStat.Handle)
                        quest.CurrQuest.CurrValue += 1;
                }
                if (itemArr != "")
                {
                    Vector3 Pos = CharacterMng.Instance.CurrCharacters[uniqueID].transform.position;
                    string[] itemArrs = itemArr.Split('/');
                    for (int i = 0; i < itemArrs.Length; ++i)
                    {
                        string[] item = itemArr.Split(',');
                        ItemMng.Instance.DropItem(int.Parse(item[0]), int.Parse(item[1]), Pos, int.Parse(item[2]));
                    }
                }
                if(exp != 0)
                {
                    PlayerMng.Instance.AddExp(PlayerMng.Instance.MainPlayer, exp);
                }
                if(gold != 0)
                {
                    PlayerMng.Instance.MainPlayer.Gold += gold;
                }
                CharacterMng.Instance.RemoveMonster(uniqueID);
            }
            return true;
        };
        m_battleStub.NotifyGetReword = (Nettention.Proud.HostID remote, Nettention.Proud.RmiContext rmiContext, bool isSuccess, Nettention.Proud.HostID getID, int uniqueID) =>
        {
            if (!isSuccess)
                return true;
            
            ItemMng.Instance.GetItem(uniqueID, PlayerMng.Instance.PlayerList[getID].Character, getID == m_netClient.LocalHostID);
            return true;
        };
        m_battleStub.NotifyEnermyInstantiate = (Nettention.Proud.HostID remote, Nettention.Proud.RmiContext rmiContext, bool isSuccess, int uniqueID, int handle, UnityEngine.Vector3 pos, float angle) =>
        {
            if (!isSuccess)
                return true;

            CharacterMng.Instance.InstantiateMonster(handle, uniqueID, EAllyType.Hostile, pos, angle);
            return true;
        };
    }
}
