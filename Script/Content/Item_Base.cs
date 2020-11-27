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
    All,
} // 아이템의 타입을 열거합니다.
public enum EItemRarity
{
    Normal,
    Magic,
    Unique,
    Relic,
    Legend,
    Infinity,
} // 아이템의 회귀도를 열거합니다.

public interface IItemNumber{ // 아이템을 중복소지하여 카운팅이 가능하게하는 인터페이스입니다.
    int Number { get; set; }
}

public interface IItemEquipment{ // 아이템의 장착을 가능케하며 장착했을때 능력치를 정의하는 인터페이스입니다.
    Stat Stat { get; set; }
}

public interface IItemLevel{ // 아이템의 사용에 있어서 레벨제한을 구현하게하는 인터페이스입니다. (장비, 물약등) 
    int Level { get; set; }
}

public interface IItemActive{ // 아이템이 소비아이템으로 취급될 수 있는지, 사용했을때 효능과 값, 쿨타임을 정의할 수 있게하는 인터페이스입니다.
    EItemActiveType ActionHandle { get; set; }
    string ActionValue { get; set; }
    float ElapsedTime { get; set; }
    float CoolTime { get; set; }
}
public interface IQuickSlotable{ // 아이템 혹은 스킬이 퀵슬롯에 등록될 수 있는지 판단하게하는 인터페이스입니다.
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