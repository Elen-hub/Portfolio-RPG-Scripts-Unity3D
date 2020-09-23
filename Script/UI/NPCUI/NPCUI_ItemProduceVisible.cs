using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class NPCUI_ItemProduceVisible : MonoBehaviour
{
    Image m_outItemImage;
    Text m_outItemNumber;
    Text m_outItemName;
    Text m_outItemLevel;
    Text m_outItemPrice;
    Text m_outItemTime;
    Text m_outItemCoolTime;
    NPCUI_ItemProduce_ItemContent[] m_materialContents = new NPCUI_ItemProduce_ItemContent[6];
    Text[] m_materialText = new Text[6];
    Text[] m_materialGrade = new Text[6];
    Text[] m_materialNumber = new Text[6];
    ItemProduceFormula m_produceFormula;
    Text m_producingStateText;
    Image m_producingProgressBar;
    Image m_coolTimeProgressBar;

    bool m_isProducing;
    float m_produceElapsedTime;
    public NPCUI_ItemProduceVisible Init()
    {
        Transform outItem = transform.Find("ItemProduceOutItem");
        m_outItemImage = outItem.Find("OutItem").Find("Icon").GetComponent<Image>();
        m_outItemNumber = outItem.Find("OutItem").Find("Text").GetComponent<Text>();
        m_outItemName = outItem.Find("Name").GetComponent<Text>();
        m_outItemLevel = outItem.Find("Dynamic").Find("Level").GetComponent<Text>();
        m_outItemPrice = outItem.Find("Dynamic").Find("Price").GetComponent<Text>();
        m_outItemTime = outItem.Find("Dynamic").Find("Time").GetComponent<Text>();
        m_outItemCoolTime = outItem.Find("Dynamic").Find("CoolTime").GetComponent<Text>();
        m_producingProgressBar = transform.Find("ProducingBar").GetComponent<Image>();
        m_coolTimeProgressBar = transform.Find("CoolTimeBar").GetComponent<Image>();

        Transform material = transform.Find("ItemProduceMaterial").Find("Materials");
        for (int i =0; i<6; ++i)
        {
            m_materialContents[i] = material.GetChild(i).GetComponent<NPCUI_ItemProduce_ItemContent>().Init();
            m_materialText[i] = m_materialContents[i].transform.Find("Name").GetComponent<Text>();
            m_materialGrade[i] = m_materialContents[i].transform.Find("Grade").GetComponent<Text>();
            m_materialNumber[i] = m_materialContents[i].transform.Find("Text").GetComponent<Text>();
        }

        Button btn = transform.Find("ProduceStart").GetComponent<Button>();
        btn.onClick.AddListener(OnClickProduce);
        m_producingStateText = btn.GetComponentInChildren<Text>();
        return this;
    }
    public void Enabled(ItemProduceFormula formula)
    {
        if(formula == null)
        {
            gameObject.SetActive(false);
            return;
        }
        Item_Base item = ItemMng.Instance.GetItemList[formula.OutItem.Handle];
        m_outItemImage.sprite = Resources.Load<Sprite>(item.Icon);
        m_outItemImage.material = Resources.Load<Material>("Material/ItemMaterial_" + item.Rarity);
        if (item is IItemNumber)
            m_outItemNumber.text = "x" + formula.OutItem.Number;
        else
            m_outItemNumber.text = null;

        m_outItemName.text = item.Name + " 조합법";
        m_outItemLevel.text = formula.Level + "이상";
        m_outItemPrice.text = formula.Gold + "골드";
        m_outItemTime.text = formula.ProduceTime + "초";
        m_outItemCoolTime.text = formula.CoolTime + "초";

        int i;
        for(i = 0; i< formula.InItem.Count; ++i)
            m_materialContents[i].Enabled(formula.InItem[i].Handle, formula.InItem[i].Number);

        for (i = i; i < 6; ++i)
            m_materialContents[i].Disabled();

        m_producingProgressBar.fillAmount = 0;

        m_produceFormula = formula;
        gameObject.SetActive(true);
    }
    public void Disabled()
    {
        m_isProducing = false;
        gameObject.SetActive(false);
    }
    void OnClickProduce()
    {
        if (m_isProducing)
            return;
        if(!m_produceFormula.PossibleProduceItem())
            return;

        m_produceElapsedTime = 0;
        m_isProducing = true;
        m_producingStateText.text = "제작중";
    }
    void LateUpdate()
    {
        m_coolTimeProgressBar.fillAmount = m_produceFormula.GetCurrCoolTime;
        if (m_isProducing)
        {
            m_produceElapsedTime += Time.deltaTime;
            m_producingProgressBar.fillAmount = m_produceElapsedTime / m_produceFormula.ProduceTime;
            if (m_produceElapsedTime > m_produceFormula.ProduceTime)
            {
                m_isProducing = false;
                m_producingStateText.text = "제작";
                NetworkMng.Instance.RequestProduceItem(m_produceFormula.Handle);
            }
        }
    }
}
