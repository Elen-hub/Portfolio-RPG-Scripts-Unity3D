using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NPCUI_ItemUpgrade_Reforgin : MonoBehaviour
{
    public NPCUI_ItemUpgrade_Reforgin Init()
    {
        return this;
    }

    public void Enabled()
    {
        gameObject.SetActive(true);
    }
    public void Disabled()
    {
        gameObject.SetActive(false);
    }
}
