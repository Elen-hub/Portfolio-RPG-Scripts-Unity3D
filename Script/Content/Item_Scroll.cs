using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class Item_Scroll : Item_Base, IItemNumber, IItemLevel, IItemActive, IQuickSlotable
{
    public EItemActiveType ActionHandle { get; set; }
    public string ActionValue { get; set; }
    public float CoolTime { get; set; }
    public float ElapsedTime { get; set; }
    public int Level { get; set; }
    public int Number { get; set; }
}
