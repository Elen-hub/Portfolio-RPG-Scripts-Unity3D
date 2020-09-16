using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : BaseUI
{
    enum EShowInventoryType
    {
        All,
        Equip,
        Potion,
        Scroll,
        Other,
    }

    EShowInventoryType m_currType;
    Animator m_animator;
    List<InventoryBTN> m_inventoryList = new List<InventoryBTN>();
    Transform m_grid;
    Text m_numberText;
    Image m_numberImg;
    
    Color m_greyColor = new Color(0.5882f, 0.5882f, 0.5882f);
    Image m_allImg;
    Text m_allText;
    Image m_equipImg;
    Text m_equipText;
    Image m_potionImg;
    Text m_potionText;
    Image m_scrollImg;
    Text m_scrollText;
    Image m_otherImg;
    Text m_otherText;

    Text m_goldText;
    Text m_cashText;

    float m_sortTime = 30;
    bool m_isShop;
    void SetShowType (EShowInventoryType type)
    { 
        switch (m_currType)
        {
            case EShowInventoryType.All:
                m_allImg.color = Color.white;
                m_allText.color = m_greyColor;
                break;
            case EShowInventoryType.Equip:
                m_equipImg.color = Color.white;
                m_equipText.color = m_greyColor;
                break;
            case EShowInventoryType.Potion:
                m_potionImg.color = Color.white;
                m_potionText.color = m_greyColor;
                break;
            case EShowInventoryType.Scroll:
                m_scrollImg.color = Color.white;
                m_scrollText.color = m_greyColor;
                break;
            case EShowInventoryType.Other:
                m_otherImg.color = Color.white;
                m_otherText.color = m_greyColor;
                break;
        }
        m_currType = type;
        switch(m_currType)
        {
            case EShowInventoryType.All:
                m_allImg.color = m_greyColor;
                m_allText.color = Color.white;
                break;
            case EShowInventoryType.Equip:
                m_equipImg.color = m_greyColor;
                m_equipText.color = Color.white;
                break;
            case EShowInventoryType.Potion:
                m_potionImg.color = m_greyColor;
                m_potionText.color = Color.white;
                break;
            case EShowInventoryType.Scroll:
                m_scrollImg.color = m_greyColor;
                m_scrollText.color = Color.white;
                break;
            case EShowInventoryType.Other:
                m_otherImg.color = m_greyColor;
                m_otherText.color = Color.white;
                break;
        }
    }
    protected override void InitUI()
    {
        transform.GetChild(0).Find("Exit").GetComponent<Button>().onClick.AddListener(Close);
        m_animator = GetComponent<Animator>();
        m_grid = GetComponentInChildren<GridLayoutGroup>().transform;

        m_numberText = transform.GetChild(0).Find("NumberText").GetComponent<Text>();
        m_numberImg = transform.GetChild(0).Find("NumberBar").Find("Progress").GetComponent<Image>();
        transform.GetChild(0).Find("Sort").GetComponent<Button>().onClick.AddListener(OnClickSort);

        m_goldText = transform.GetChild(0).Find("GoldRect").GetComponentInChildren<Text>();
        m_cashText = transform.GetChild(0).Find("CashRect").GetComponentInChildren<Text>();

        Transform buttonGroup = transform.GetChild(0).Find("SelectTypeButton");
        m_allImg = buttonGroup.Find("AllType").GetComponent<Image>();
        m_allText = buttonGroup.Find("AllType").GetComponentInChildren<Text>();
        m_allImg.GetComponent<Button>().onClick.AddListener(()=>SetShowType(EShowInventoryType.All));
        m_equipImg = buttonGroup.Find("EquipType").GetComponent<Image>();
        m_equipText = buttonGroup.Find("EquipType").GetComponentInChildren<Text>();
        m_equipImg.GetComponent<Button>().onClick.AddListener(() => SetShowType(EShowInventoryType.Equip));
        m_potionImg = buttonGroup.Find("PotionType").GetComponent<Image>();
        m_potionText = buttonGroup.Find("PotionType").GetComponentInChildren<Text>();
        m_potionImg.GetComponent<Button>().onClick.AddListener(() => SetShowType(EShowInventoryType.Potion));
        m_scrollImg = buttonGroup.Find("ScrollType").GetComponent<Image>();
        m_scrollText = buttonGroup.Find("ScrollType").GetComponentInChildren<Text>();
        m_scrollImg.GetComponent<Button>().onClick.AddListener(() => SetShowType(EShowInventoryType.Scroll));
        m_otherImg = buttonGroup.Find("OtherType").GetComponent<Image>();
        m_otherText = buttonGroup.Find("OtherType").GetComponentInChildren<Text>();
        m_otherImg.GetComponent<Button>().onClick.AddListener(() => SetShowType(EShowInventoryType.Other));

        UIMng.Instance.Open<ItemInformation>(UIMng.UIName.ItemInformation).Close();
    }
    public void Open(bool isNPCUI)
    {
        m_isShop = isNPCUI;
        if (!transform.GetChild(0).gameObject.activeSelf)
            m_animator.Play("Open");
    }
    public override void Close()
    {
        if (m_isShop)
            UIMng.Instance.GetUI<NPCUI>(UIMng.UIName.NPCUI).ShopUI.Disabled(false);

        for(int i =0; i<m_inventoryList.Count; ++i)
            m_inventoryList[i].Disabled();

        UIMng.Instance.CLOSE = UIMng.UIName.ItemInformation;
        m_animator.Play("Close");
    }
    void OnClickSort()
    {
        if (m_sortTime > 30)
        {
            NetworkMng.Instance.RequestInventorySort();
             m_sortTime = 0;
        }
        else
            SystemMessage.Instance.PushMessage(SystemMessage.MessageType.Sub, "아이템 정렬은 30초마다 할 수 있습니다.");
    }
    void ShowInventory(EShowInventoryType type)
    {
        int i = 0;

        foreach(Item_Base item in ItemMng.Instance.Inventory)
        {
            switch(type)
            {
                case EShowInventoryType.All:
                    break;
                case EShowInventoryType.Equip:
                    if (item is IItemEquipment) break;
                    else continue;
                case EShowInventoryType.Other:
                    if (item.Type == EItemType.Other) break;
                    else continue;
                case EShowInventoryType.Potion:
                    if (item.Type == EItemType.Potion) break;
                    else continue;
                case EShowInventoryType.Scroll:
                    if (item.Type == EItemType.Scroll) break;
                    else continue;
            }
            if (i < m_inventoryList.Count)
            {
                m_inventoryList[i].Enabled(item, m_isShop);
            }
            else
            {
                InventoryBTN btn = Instantiate(Resources.Load<InventoryBTN>("UI/Instance/InventoryBTN"), m_grid).Init();
                btn.Enabled(item, m_isShop);
                m_inventoryList.Add(btn);
            }

            ++i;
        }
        for (int j = i; j < m_inventoryList.Count; ++j)
            m_inventoryList[j].Disabled();
    }
    private void LateUpdate()
    {
        m_sortTime += Time.deltaTime;
        ShowInventory(m_currType);
        m_numberText.text = ItemMng.Instance.Inventory.Count+"/"+ItemMng.Instance.MaxInventorySize;
        m_numberImg.fillAmount = ItemMng.Instance.Inventory.Count/(float)ItemMng.Instance.MaxInventorySize;
        m_goldText.text = PlayerMng.Instance.MainPlayer.Gold.ToString();
        m_cashText.text = PlayerMng.Instance.MainPlayer.Cash.ToString();
    }
}
