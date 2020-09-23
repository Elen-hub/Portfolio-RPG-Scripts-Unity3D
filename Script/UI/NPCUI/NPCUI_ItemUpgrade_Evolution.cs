using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NPCUI_ItemUpgrade_Evolution : MonoBehaviour
{
    public NPCUI_ItemUpgrade_Evolution Init()
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
