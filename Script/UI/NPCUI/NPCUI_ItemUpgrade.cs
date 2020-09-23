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
    EItemUpgradeType m_currViewType;
    Item_Base m_selectItem;
    NPCUI_ItemUpgrade_Evolution m_evolutionView;
    NPCUI_ItemUpgrade_Reforgin m_reforginView;
    NPCUI_ItemUpgrade_MagicJewal m_magicJewalView;
    public void Init()
    {
        m_evolutionView = GetComponent<NPCUI_ItemUpgrade_Evolution>().Init();
        m_reforginView = GetComponent<NPCUI_ItemUpgrade_Reforgin>().Init();
        m_magicJewalView = GetComponent<NPCUI_ItemUpgrade_MagicJewal>().Init();
        transform.Find("Exit").GetComponent<Button>().onClick.AddListener(Disabled);
        gameObject.SetActive(false);
    }
    public void Enabled()
    {
        m_currViewType = EItemUpgradeType.Evolution;
        m_selectItem = null;
        gameObject.SetActive(true);
    }
    public void Disabled()
    {
        gameObject.SetActive(false);
    }
}
