using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_Equipment : Item_Base, IItemEquipment, IItemLevel
{
    public int Level { get; set; }
    public int Value { get; set; }
    public Stat Stat { get; set; }
}
