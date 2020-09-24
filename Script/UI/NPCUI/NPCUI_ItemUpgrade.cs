using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum EItemUpgradeType
{ 
    Evolution,
    Reforgin,
    MagicJewal,
}

public class UI_ItemUpgrade : MonoBehaviour
{
    Item_Equipment m_selectItem;
    NPCUI_ItemUpgrade_UpgradeInfo m_upgradeInfo;
    List<NPCUI_ItemUpgrade_UpgradeContent> m_contentList = new List<NPCUI_ItemUpgrade_UpgradeContent>();
    public void Init()
    {
        m_upgradeInfo = GetComponent<NPCUI_ItemUpgrade_UpgradeInfo>().Init();
        transform.Find("Exit").GetComponent<Button>().onClick.AddListener(Disabled);
        gameObject.SetActive(false);
    }
    public void Enabled()
    {
        m_selectItem = null;
        gameObject.SetActive(true);
    }
    public void Disabled()
    {
        gameObject.SetActive(false);
    }
}
