using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NPCUI_ItemProduce_ItemContent : MonoBehaviour
{
    IItemNumber m_itemNumber;
    Image m_icon;
    Text m_number;
    Text m_name;
    Text m_grade;

    public NPCUI_ItemProduce_ItemContent Init()
    {
        m_icon = transform.Find("Icon").GetComponent<Image>();
        m_number = GetComponentInChildren<Text>();
        m_name = transform.Find("Name").GetComponent<Text>();
        m_grade = transform.Find("Grade").GetComponent<Text>();
        return this;
    }
    public void Enabled(int handle, int number)
    {
        Item_Base item = ItemMng.Instance.GetItemList[handle];
        m_icon.sprite = Resources.Load<Sprite>(item.Icon);
        m_icon.material = Resources.Load<Material>("Material/ItemMaterial_" + item.Rarity);
        if (item is IItemNumber)
            m_number.text = "x" + number;
        else
            m_number.text = null;
        m_name.text = item.Name;
        m_grade.text = ParseLib.GetRairityKorConvert(item.Rarity);
        gameObject.SetActive(true);
    }
    public void Disabled()
    {
        gameObject.SetActive(false);
    }
}
