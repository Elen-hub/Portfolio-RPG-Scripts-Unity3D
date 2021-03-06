﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NPCUI_ItemUpgrade_UpgradeInfo : MonoBehaviour
{
    Item_Equipment m_item;
    public NPCUI_ItemUpgrade_UpgradeInfo Init()
    {
        return this;
    }

    public void Enabled(Item_Equipment item)
    {
        m_item = item;
        gameObject.SetActive(true);
    }
    public void Disabled()
    {
        gameObject.SetActive(false);
    }
}
