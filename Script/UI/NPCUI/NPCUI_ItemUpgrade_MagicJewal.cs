using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NPCUI_ItemUpgrade_MagicJewal : MonoBehaviour
{
    public NPCUI_ItemUpgrade_MagicJewal Init()
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
