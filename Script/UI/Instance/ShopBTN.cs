using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopBTN : MonoBehaviour
{
    Item_Base m_item;

    Image m_iconImg;
    Image m_cornorImg;
    Text m_nameText;
    Text m_explanationText;
    Text m_typeText;
    Text m_priceText;
    public ShopBTN Init()
    {
        GetComponent<Button>().onClick.AddListener(OnClickInformation);
        m_iconImg = transform.Find("Icon").Find("Img").GetComponent<Image>();
        m_cornorImg = transform.Find("Icon").Find("Cornor").GetComponent<Image>();
        m_nameText = transform.Find("NameText").GetComponent<Text>();
        m_typeText = transform.Find("TypeText").GetComponent<Text>();
        m_explanationText = transform.Find("ExplanationText").GetComponent<Text>();
        m_priceText = transform.Find("Price").GetComponentInChildren<Text>();
        return this;
    }
    public void Enabled(int handle)
    {
        m_item = ItemMng.Instance.GetItemList[handle].Clone();
        m_iconImg.sprite = Resources.Load<Sprite>(m_item.Icon);
        m_nameText.text = m_item.Name;
        m_explanationText.text = m_item.Explanation;
        m_priceText.text = m_item.Price + "Gold";

        switch (m_item.Type)
        {
            case EItemType.Weapon:
            case EItemType.Armor:
            case EItemType.Gloves:
            case EItemType.Shoes:
            case EItemType.Ring:
            case EItemType.Necklace:
                m_typeText.text = "장비";
                break;
            case EItemType.Potion:
                m_typeText.text = "소비";
                break;
            case EItemType.Scroll:
                m_typeText.text = "스크롤";
                break;
            case EItemType.Other:
                m_typeText.text = "재료";
                break;
        }

        switch (m_item.Rarity)
        {
            case EItemRarity.Normal:
                m_cornorImg.color = Color.white;
                break;
            case EItemRarity.Magic:
                m_cornorImg.color = GameSystem.ColorMagic;
                break;
            case EItemRarity.Unique:
                m_cornorImg.color = GameSystem.ColorUnique;
                break;
            case EItemRarity.Relic:
                m_cornorImg.color = GameSystem.ColorRelic;
                break;
            case EItemRarity.Legend:
                m_cornorImg.color = GameSystem.ColorLegend;
                break;
            case EItemRarity.Infinity:
                m_cornorImg.color = GameSystem.ColorInfinity; ;
                break;
        }

        gameObject.SetActive(true);
    }
    public void Disabled()
    {
        m_item = null;
        gameObject.SetActive(false);
    }
    void OnClickInformation()
    {
        UIMng.Instance.Open<ItemInformation>(UIMng.UIName.ItemInformation).Open(m_item, EItemInformationOption.Buy);
    }
}
