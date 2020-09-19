using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;
using System;

public partial class ItemMng : TSingleton<ItemMng>
{
    Dictionary<int, Item_Base> m_itemDic = new Dictionary<int, Item_Base>();
    public Dictionary<int, Item_Base> GetItemList { get { return m_itemDic; } }
    Dictionary<int, Item_Object> ItemObjectList = new Dictionary<int, Item_Object>();
    List<Item_Object> ItemObjectMemoryList = new List<Item_Object>();
    public HashInt MaxInventorySize = 10;
    public List<Item_Base> Inventory = new List<Item_Base>();
    public void ClearItemObject()
    {
        foreach (Item_Object item in ItemObjectList.Values)
        {
            item.Disabled();
            ItemObjectMemoryList.Add(item);
        }
        ItemObjectList.Clear();
    }
    public Item_Base FindItemInInventory(int handle)
    {
        for (int i = 0; i < Inventory.Count; ++i)
            if (Inventory[i].Handle == handle)
                return Inventory[i];

        return null;
    }
    public int IComparerItem(Item_Base start , Item_Base to)
    {
        if ((int)start.Type == (int)to.Type)
        {
            if ((int)start.Rarity > (int)to.Rarity)
                return -1;
            else
                return 1;
        }
        else
        {
            if ((int)start.Type > (int)to.Type)
                return -1;
            else
                return 1;
        }
    }
    public void GetItem(int uniqueID, BaseCharacter character, bool isAddItem)
    {
        Item_Object item = ItemObjectList[uniqueID];
        item.Get(character);

        if(isAddItem)
            AddItem(item.Item);

        ItemObjectList.Remove(uniqueID);
        ItemObjectMemoryList.Add(item);
    }
    public void DropItem(int uniqueID, int handle, Vector3 position, object value = null)
    {
        Item_Base item = m_itemDic[handle].Clone();
        item.UniqueID = uniqueID;
        switch(item.Type)
        {
            case EItemType.Other:
            case EItemType.Potion:
            case EItemType.Scroll:
                (item as IItemNumber).Number = (int)value;
                break;
        }
        Item_Object Object;
        for(int i =0; i<ItemObjectMemoryList.Count; ++i)
        {
            if(ItemObjectMemoryList[i].Item == null)
            {
                Object = ItemObjectMemoryList[i];
                Object.Enabled(item, position);
                ItemObjectList.Add(uniqueID, Object);
                ItemObjectMemoryList.Remove(Object);
                return;
            }
        }
        Object = Instantiate(Resources.Load<Item_Object>("ETC/Item_Object"), transform);
        ItemObjectList.Add(uniqueID, Object);
        Object.Init();
        Object.Enabled(item, position);
    }
    public bool CheckInventoryCount(Item_Base item)
    {
        IItemNumber itemNumber = item as IItemNumber;
        if (itemNumber != null)
            for (int i = 0; i < Inventory.Count; ++i)
                if (Inventory[i].Handle == item.Handle)
                    return true;

        return Inventory.Count < MaxInventorySize;
    }
    public void AddItem(Item_Base item)
    {
        string text = "<color=" + GameSystem.ColorRarity[(int)item.Rarity] + ">" + item.Name + "</color>";
        IItemNumber itemNumber = item as IItemNumber;

        if (itemNumber != null)
        {
            for (int i = 0; i < Inventory.Count; ++i)
                if (Inventory[i].Handle == item.Handle)
                {
                    ((IItemNumber)Inventory[i]).Number += itemNumber.Number;
                    text += "를(을) " + itemNumber.Number + "개 획득하였습니다.";
                    if((int)item.Rarity >1)
                        NetworkMng.Instance.AddChat(text);
                    return;
                }
        }

        Inventory.Add(item);

        if (itemNumber == null)
            text += "를(을) 획득하였습니다.";
        else
            text += "를(을) " + itemNumber.Number + "개 획득하였습니다.";

        if ((int)item.Rarity > 1)
            NetworkMng.Instance.AddChat(text);
    }
    public void RemoveItem(Item_Base item)
    {
        Inventory.Remove(item);
    }
    public void RemoveItem(Item_Base item, int number)
    {
        IItemNumber ItemNumber = item as IItemNumber;
        ItemNumber.Number -= number;
        if (ItemNumber.Number <= 0)
            Inventory.Remove(item);
    }
    public void RemoveItem(int number)
    {
        Inventory.RemoveAt(number);
    }
    public void RemoveItem(int inventoryNum, int number)
    {
        IItemNumber ItemNumber = Inventory[inventoryNum] as IItemNumber;
        ItemNumber.Number -= number;
        if (ItemNumber.Number <= 0)
            Inventory.RemoveAt(inventoryNum);
    }
    public void EquipItem(int number)
    {
        Item_Base item = Inventory[number];
        Item_Equipment EquipItem = item as Item_Equipment;

        Item_Equipment outItem = PlayerMng.Instance.MainPlayer.Character.StatSystem.SetEquipment(EquipItem);
        if (outItem != null)
            Inventory.Add(outItem);

        Inventory.Remove(item);
    }
    public void DisArmament(EItemType type)
    {
        Item_Equipment outItem = PlayerMng.Instance.MainPlayer.Character.StatSystem.RemoveEquipment(type);
        if(outItem != null)
            Inventory.Add(outItem);
    }
    public override void Init()
    {
        ItemActionInit();
        ItemProduceInit();

#if UNITY_EDITOR
        TextAsset Asset = new TextAsset(System.IO.File.ReadAllText("Assets/AssetBundle_Database/DB_Item.json"));
        JSONNode Node = JSON.Parse(Asset.text);
#else
        JSONNode Node = JSON.Parse(AssetMng.Instance["database"].LoadAsset<TextAsset>("DB_Item").text);
#endif
        for (int i = 0; i < Node.Count; ++i)
        {
            Item_Base Item;

            switch ((EItemType)int.Parse(Node[i]["Type"]))
            {
                case EItemType.Other:
                    Item = new Item_Other();
                    break;
                case EItemType.Potion:
                    Item = new Item_Potion();
                    break;
                case EItemType.Scroll:
                    Item = new Item_Scroll();
                    break;
                case EItemType.Weapon:
                case EItemType.Armor:
                case EItemType.Gloves:
                case EItemType.Shoes:
                case EItemType.Ring:
                case EItemType.Necklace:
                    Item = new Item_Equipment();
                    break;
                default:
                    Item = new Item_Base();
                    break;
            }
            Item.Handle = int.Parse(Node[i]["Handle"].Value);
            Item.Name = Node[i]["Name"].Value;
            Item.Explanation = Node[i]["Explanation"].Value;
            Item.Price = int.Parse(Node[i]["Price"].Value);
            Item.Type = (EItemType)int.Parse(Node[i]["Type"]);
            Item.Rarity = (EItemRarity)int.Parse(Node[i]["Rarity"].Value);
            Item.Icon = Node[i]["Icon"].Value;
            IItemLevel Level = Item as IItemLevel;
            if (Level != null)
            {
                Level.Level = int.Parse(Node[i]["Level"]);
            }
            IItemActive Active = Item as IItemActive;
            if (Active != null)
            {
                Active.ActionHandle = (EItemActiveType)int.Parse(Node[i]["ActionHandle"]);
                Active.ActionValue =Node[i]["ActionValue"];
                Active.CoolTime = float.Parse(Node[i]["CoolTime"]);
                Active.ElapsedTime = Active.CoolTime;
            }
            IItemEquipment Stat = Item as IItemEquipment;
            if (Stat != null)
            {
                Stat.Stat = new Stat()
                {
                    STR = float.Parse(Node[i]["STR"]),
                    DEX = float.Parse(Node[i]["DEX"]),
                    INT = float.Parse(Node[i]["INT"]),
                    WIS = float.Parse(Node[i]["WIS"]),
                    CON = float.Parse(Node[i]["CON"]),
                    HP = float.Parse(Node[i]["HP"]),
                    RecoveryHP = float.Parse(Node[i]["RecoveryHP"]),
                    Resistance = float.Parse(Node[i]["Resistance"]),
                    MP = float.Parse(Node[i]["MP"]),
                    RecoveryMP = float.Parse(Node[i]["RecoveryMP"]),
                    CoolTime = float.Parse(Node[i]["CoolTime"]),
                    CriticalPro = float.Parse(Node[i]["CriticalPro"]),
                    CriticalDamage = float.Parse(Node[i]["CriticalDamage"]),
                    AttackSpeed = float.Parse(Node[i]["AttackSpeed"]),
                    MoveSpeed = float.Parse(Node[i]["MoveSpeed"]),
                    MoveSpeedPro = float.Parse(Node[i]["MoveSpeedPro"]),
                    AttackDamage = float.Parse(Node[i]["AttackDamage"]),
                    AttackDamagePro = float.Parse(Node[i]["AttackDamagePro"]),
                    Defence = float.Parse(Node[i]["Defence"]),
                    DefencePro = float.Parse(Node[i]["DefencePro"]),
                    SkillDamagePro = float.Parse(Node[i]["SkillDamagePro"])
                };
            }
            m_itemDic.Add(Item.Handle, Item);
        }

        IsLoad = true;
    }
    public void Exit()
    {
        Inventory.Clear();
    }
}
