using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public enum EItemType
{
    Other,
    Scroll,
    Potion,
    Weapon,
    Armor,
    Gloves,
    Shoes,
    Ring,
    Necklace,
}
public enum EItemRarity
{
    Normal,
    Magic,
    Unique,
    Relic,
    Legend,
    Infinity,
}

public interface IItemNumber
{
    int Number { get; set; }
}

public interface IItemEquipment
{
    Stat Stat { get; set; }
}

public interface IItemLevel
{
    int Level { get; set; }
}

public interface IItemActive
{
    EItemActiveType ActionHandle { get; set; }
    string ActionValue { get; set; }
    float ElapsedTime { get; set; }
    float CoolTime { get; set; }
}
public interface IQuickSlotable
{
    string Icon { get; set; }
    float ElapsedTime { get; set; }
    float CoolTime { get; set; }
}

[System.Serializable]
public class Item_Base
{
    public int Handle;
    public int UniqueID;
    public EItemType Type;
    public EItemRarity Rarity;
    public string Name;
    public string Explanation;
    public string Icon { get; set; }
    public int Price;

    public Item_Base Clone()
    {
        return MemberwiseClone() as Item_Base;
    }
}