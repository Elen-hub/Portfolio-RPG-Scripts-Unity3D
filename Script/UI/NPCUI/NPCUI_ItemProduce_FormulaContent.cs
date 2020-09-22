using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NPCUI_ItemProduce_FormulaContent : MonoBehaviour
{
    NPCUI_ItemProduce.DProduceFormulaBTN del_SelectHandle;
    ItemProduceFormula m_produceFormula;
    Image m_icon;
    Text m_name;
    Text m_explanation;

    public NPCUI_ItemProduce_FormulaContent Init(NPCUI_ItemProduce.DProduceFormulaBTN dSelectHandle)
    {
        del_SelectHandle = dSelectHandle;
        m_icon = transform.Find("Image").Find("Icon").GetComponent<Image>();
        m_name = transform.Find("Name").GetComponent<Text>();
        m_explanation = transform.Find("Explanation").GetComponent<Text>();
        GetComponent<Button>().onClick.AddListener(() => del_SelectHandle(m_produceFormula));
        return this;
    }
    public void Enabled(ItemProduceFormula produceFormula)
    {
        Item_Base item = ItemMng.Instance.GetItemList[produceFormula.OutItem.Handle];
        m_produceFormula = produceFormula;
        m_icon.sprite = Resources.Load<Sprite>(item.Icon);
        m_icon.material = Resources.Load<Material>("Material/ItemMaterial_" + item.Rarity);
        m_name.text = item.Name + " (" + ParseLib.GetRairityKorConvert(item.Rarity) + ")";
        m_explanation.text = item.Explanation;
        gameObject.SetActive(true);
    }
    public void Disabled()
    {
        gameObject.SetActive(false);
    }
}
