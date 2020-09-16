using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public enum EItemActiveType
{
    RecoveryStaticHP,
    RecoveryStaticMP,
    RecoveryDynamicHP,
    RecoveryDynamicMP,
    RecycleHP,
    RecycleMP,
    ReturnSelectTown,
    LevelUp,
    StatPointUp,
    SkillPointUp,
}
public partial class ItemMng : TSingleton<ItemMng>
{
    Dictionary<EItemActiveType, UnityAction<Nettention.Proud.HostID, string>> m_activeActionDic = new Dictionary<EItemActiveType, UnityAction<Nettention.Proud.HostID, string>>();
    List<IItemActive> CoolTimeList = new List<IItemActive>();
    public bool IsCoolTimeComplete(Item_Base content)
    {
        IItemActive item = content as IItemActive;

        if (item == null)
            return false;
        
        bool isCoolTime = !CoolTimeList.Contains(item);
        if(!isCoolTime) SystemMessage.Instance.PushMessage(SystemMessage.MessageType.Sub, (int)(item.CoolTime - item.ElapsedTime) + "초 후 사용 가능합니다.");
        return isCoolTime;
    }
    public bool IsNumberCheck(Item_Base content)
    {
        IItemNumber number = content as IItemNumber;
        if (number == null)
            return false;
        if (number.Number <= 0)
        {
            SystemMessage.Instance.PushMessage(SystemMessage.MessageType.Sub, "개수가 부족합니다.");
            Inventory.Remove(content);
            return false;
        }

        return true;
    }
    public void ItemUseAction(Nettention.Proud.HostID remote, Item_Base content)
    {
        IItemActive item = content as IItemActive;
        EItemActiveType type = item.ActionHandle;
        string value = item.ActionValue;
        m_activeActionDic[type](remote, value);
        item.ElapsedTime = 0;
        CoolTimeList.Add(item);

        IItemNumber number = content as IItemNumber;
        number.Number -= 1;
        if (number.Number <= 0)
            Inventory.Remove(content);
    }

    void RecoveryStaticHP(Nettention.Proud.HostID remote, string value)
    {
        BaseCharacter character = PlayerMng.Instance.PlayerList[remote].Character;
        NetworkMng.Instance.NotifyRecoveryHP(character.UniqueID, character.UniqueID, float.Parse(value), 0);
    }
    void RecoveryStaticMP(Nettention.Proud.HostID remote, string value)
    {
        BaseCharacter character = PlayerMng.Instance.PlayerList[remote].Character;
        NetworkMng.Instance.NotifyRecoveryMP(character.UniqueID, character.UniqueID, float.Parse(value), 0);
    }
    void RecoveryDynamicHP(Nettention.Proud.HostID remote, string value)
    {
        BaseCharacter character = PlayerMng.Instance.PlayerList[remote].Character;
        NetworkMng.Instance.NotifyRecoveryHP(character.UniqueID, character.UniqueID, 0, float.Parse(value));
    }
    void RecoveryDynamicMP(Nettention.Proud.HostID remote, string value)
    {
        BaseCharacter character = PlayerMng.Instance.PlayerList[remote].Character;
        NetworkMng.Instance.NotifyRecoveryMP(character.UniqueID, character.UniqueID, 0, float.Parse(value));
    }
    void RecycleHP(Nettention.Proud.HostID remote, string value)
    {

    }
    void RecycleMP(Nettention.Proud.HostID remote, string value)
    {

    }
    void ReturnSelectTown(Nettention.Proud.HostID remote, string value)
    {
        //MapMng.Instance.SetCurrMap(int.Parse(value));
        //Map map = MapMng.Instance.CurrMap;
        //SceneMng.Instance.SetGameScene(map.SceneName, () => PlayerMng.Instance.MainPlayer.Character.MoveSystem.SetPosition = new Vector3(map.WayPoint.x, 0, map.WayPoint.y));
        UIMng.Instance.CLOSE = UIMng.UIName.Inventory;
    }
    void LevelUp(Nettention.Proud.HostID remote, string value)
    {
        PlayerMng.Instance.AddExp(PlayerMng.Instance.PlayerList[remote], PlayerMng.Instance.ExpList[PlayerMng.Instance.PlayerList[remote].Level]);
    }
    void SkillPointUp(Nettention.Proud.HostID remote, string value)
    {
        PlayerMng.Instance.PlayerList[remote].SkillPoint += int.Parse(value);
    }
    void StatPointUp(Nettention.Proud.HostID remote, string value)
    {
        PlayerMng.Instance.PlayerList[remote].StatPoint += int.Parse(value);
    }
    private void LateUpdate()
    {
        for (int i = CoolTimeList.Count-1; i >= 0; --i)
        {
            CoolTimeList[i].ElapsedTime += Time.deltaTime;
            if (CoolTimeList[i].CoolTime < CoolTimeList[i].ElapsedTime)
                CoolTimeList.RemoveAt(i);
        }
    }
}
