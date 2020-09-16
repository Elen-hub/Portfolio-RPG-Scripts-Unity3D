using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum EItemInformationOption
{
    Info,
    Buy,
    Sell,
}
public class ItemInformation : BaseUI
{
    Item_Base m_item;
    Image m_icon;
    Text m_numberText;
    Text m_nameText;
    Text m_goldText;
    Text m_auctionGoldText;
    Text m_rarityText;
    Text m_typeText;
    Text m_explanationText;
    Text m_informationText;

    GameObject m_equipBTN, m_unEquipBTN, m_upgradeBTN, m_magicBTN, m_useBTN, m_quickSlotBTN, m_deleteBTN, m_buyBTN, m_sellBTN;
    protected override void InitUI()
    {
        transform.Find("Exit").GetComponent<Button>().onClick.AddListener(Close);
        m_nameText = transform.Find("ItemName").GetComponent<Text>();
        m_icon = transform.Find("InventoryBTN").Find("Icon").GetComponent<Image>();
        m_numberText = transform.Find("InventoryBTN").GetComponentInChildren<Text>();
        m_goldText = transform.Find("GoldRect").GetComponentInChildren<Text>();
        m_auctionGoldText = transform.Find("AuctionRect").GetComponentInChildren<Text>();
        m_rarityText = transform.Find("Rarity").GetComponent<Text>();
        m_typeText = transform.Find("Type").GetComponent<Text>();
        m_explanationText = transform.Find("Explanation").GetComponent<Text>();
        m_informationText = transform.Find("Information").GetComponent<Text>();
        Transform BTNGroup = transform.Find("BTNGroup");
        m_equipBTN = BTNGroup.Find("Equip").gameObject;
        m_equipBTN.GetComponent<Button>().onClick.AddListener(OnClickEquip);
        m_unEquipBTN = BTNGroup.Find("Unequip").gameObject;
        m_unEquipBTN.GetComponent<Button>().onClick.AddListener(OnClickUnEquip);
        m_upgradeBTN = BTNGroup.Find("Upgrade").gameObject;
        m_upgradeBTN.GetComponent<Button>().onClick.AddListener(OnClickUpgrade);
        m_magicBTN = BTNGroup.Find("Magic").gameObject;
        m_magicBTN.GetComponent<Button>().onClick.AddListener(OnClickMagic);
        m_useBTN = BTNGroup.Find("Use").gameObject;
        m_useBTN.GetComponent<Button>().onClick.AddListener(OnClickUse);
        m_quickSlotBTN = BTNGroup.Find("QuickSlot").gameObject;
        m_quickSlotBTN.GetComponent<Button>().onClick.AddListener(OnClickQuickSlot);
        m_deleteBTN = BTNGroup.Find("Delete").gameObject;
        m_deleteBTN.GetComponent<Button>().onClick.AddListener(OnClickDelete);
        m_sellBTN = BTNGroup.Find("Sell").gameObject;
        m_sellBTN.GetComponent<Button>().onClick.AddListener(OnClickSell);
        m_buyBTN = BTNGroup.Find("Buy").gameObject;
        m_buyBTN.GetComponent<Button>().onClick.AddListener(OnClickBuy);
    }
    public void Open(Item_Base item, EItemInformationOption option = EItemInformationOption.Info)
    {
        m_item = item;
        m_nameText.text = item.Name;
        m_icon.sprite = Resources.Load<Sprite>(item.Icon);
        m_icon.material = Resources.Load<Material>("Material/ItemMaterial_" + item.Rarity);
        m_goldText.text = item.Price.ToString();
        // m_auctionGoldText.text = 거래소 가격;
        m_explanationText.text = item.Explanation;
        m_informationText.text = null;
        m_rarityText.text = ParseLib.GetRairityKorConvert(item.Rarity);
        IItemNumber number = item as IItemNumber;
        switch (option)
        {
            case EItemInformationOption.Info:
                if (number != null) m_numberText.text = "x" + number.Number;
                else m_numberText.text = null;
                switch (item.Type)
                {
                    case EItemType.Weapon:
                    case EItemType.Armor:
                    case EItemType.Gloves:
                    case EItemType.Shoes:
                    case EItemType.Ring:
                    case EItemType.Necklace:
                        m_typeText.text = "장비 아이템";
                        bool IsEquip = PlayerMng.Instance.MainPlayer.Character.StatSystem.GetEquipment(item.Type) == item;
                        m_unEquipBTN.SetActive(IsEquip);
                        m_equipBTN.SetActive(!IsEquip);
                        m_upgradeBTN.SetActive(true);
                        m_magicBTN.SetActive(true);
                        m_useBTN.SetActive(false);
                        m_quickSlotBTN.SetActive(false);
                        m_buyBTN.SetActive(false);
                        m_sellBTN.SetActive(false);
                        m_deleteBTN.SetActive(true);
                        IItemEquipment Item = m_item as IItemEquipment;
                        if (Item.Stat.STR != 0) m_informationText.text += "힘 + " + Item.Stat.STR + " \n";
                        if (Item.Stat.DEX != 0) m_informationText.text += "민첩 + " + Item.Stat.DEX + " \n";
                        if (Item.Stat.INT != 0) m_informationText.text += "지능 + " + Item.Stat.INT + " \n";
                        if (Item.Stat.WIS != 0) m_informationText.text += "지혜 + " + Item.Stat.WIS + " \n";
                        if (Item.Stat.CON != 0) m_informationText.text += "건강 + " + Item.Stat.CON + " \n";
                        if (Item.Stat.HP != 0) m_informationText.text += "체력 + " + Item.Stat.HP + " \n";
                        if (Item.Stat.RecoveryHP != 0) m_informationText.text += "체력 재생력 + " + Item.Stat.RecoveryHP * 100 + "%\n";
                        if (Item.Stat.MP != 0) m_informationText.text += "마력 + " + Item.Stat.MP + " \n";
                        if (Item.Stat.RecoveryMP != 0) m_informationText.text += "마력 재생력 + " + Item.Stat.RecoveryMP * 100 + "%\n";
                        if (Item.Stat.AttackDamage != 0) m_informationText.text += "공격력 + " + Item.Stat.AttackDamage + " \n";
                        if (Item.Stat.AttackDamagePro != 0) m_informationText.text += "공격력 계수 + " + Item.Stat.AttackDamagePro * 100 + "%\n";
                        if (Item.Stat.AttackSpeed != 0) m_informationText.text += "공격속도 + " + Item.Stat.AttackSpeed + " \n";
                        if (Item.Stat.CriticalPro != 0) m_informationText.text += "치명타 확률 + " + Item.Stat.CriticalPro * 100 + "%\n";
                        if (Item.Stat.CriticalDamage != 0) m_informationText.text += "치명타 계수 + " + Item.Stat.CriticalDamage * 100 + "%\n";
                        if (Item.Stat.Defence != 0) m_informationText.text += "방어력 + " + Item.Stat.Defence + " \n";
                        if (Item.Stat.DefencePro != 0) m_informationText.text += "방어력 계수 + " + Item.Stat.DefencePro * 100 + "%\n";
                        if (Item.Stat.MoveSpeed != 0) m_informationText.text += "이동속도 + " + Item.Stat.MoveSpeed + " \n";
                        if (Item.Stat.MoveSpeedPro != 0) m_informationText.text += "이동속도 계수 + " + Item.Stat.MoveSpeedPro * 100 + "%\n";
                        if (Item.Stat.Resistance != 0) m_informationText.text += "저항력 + " + Item.Stat.Resistance * 100 + "%\n";
                        if (Item.Stat.CoolTime != 0) m_informationText.text += "쿨타임 감소률 + " + Item.Stat.CoolTime * 100 + "%\n";
                        if (Item.Stat.SkillDamagePro != 0) m_informationText.text += "스킬 데미지 + " + Item.Stat.SkillDamagePro * 100 + "%\n";
                        break;
                    case EItemType.Potion:
                        m_typeText.text = "소비 아이템";
                        m_equipBTN.SetActive(false);
                        m_unEquipBTN.SetActive(false);
                        m_upgradeBTN.SetActive(false);
                        m_magicBTN.SetActive(false);
                        m_useBTN.SetActive(true);
                        m_quickSlotBTN.SetActive(true);
                        m_buyBTN.SetActive(false);
                        m_sellBTN.SetActive(false);
                        m_deleteBTN.SetActive(true);
                        break;
                    case EItemType.Scroll:
                        m_typeText.text = "스크롤 아이템";
                        m_equipBTN.SetActive(false);
                        m_unEquipBTN.SetActive(false);
                        m_upgradeBTN.SetActive(false);
                        m_magicBTN.SetActive(false);
                        m_useBTN.SetActive(true);
                        m_quickSlotBTN.SetActive(false);
                        m_buyBTN.SetActive(false);
                        m_sellBTN.SetActive(false);
                        m_deleteBTN.SetActive(true);
                        break;
                    case EItemType.Other:
                        m_typeText.text = "재료 아이템";
                        m_equipBTN.SetActive(false);
                        m_unEquipBTN.SetActive(false);
                        m_upgradeBTN.SetActive(false);
                        m_magicBTN.SetActive(false);
                        m_useBTN.SetActive(false);
                        m_quickSlotBTN.SetActive(false);
                        m_buyBTN.SetActive(false);
                        m_sellBTN.SetActive(false);
                        m_deleteBTN.SetActive(true);
                        break;
                        }
                break;
            case EItemInformationOption.Sell:
                if (number != null) m_numberText.text = "x" + number.Number;
                else m_numberText.text = null;
                m_unEquipBTN.SetActive(false);
                m_equipBTN.SetActive(false);
                m_upgradeBTN.SetActive(false);
                m_magicBTN.SetActive(false);
                m_useBTN.SetActive(false);
                m_quickSlotBTN.SetActive(false);
                m_buyBTN.SetActive(false);
                m_sellBTN.SetActive(true);
                m_deleteBTN.SetActive(false);
                break;
            case EItemInformationOption.Buy:
                m_numberText.text = null;
                m_unEquipBTN.SetActive(false);
                m_equipBTN.SetActive(false);
                m_upgradeBTN.SetActive(false);
                m_magicBTN.SetActive(false);
                m_useBTN.SetActive(false);
                m_quickSlotBTN.SetActive(false);
                m_buyBTN.SetActive(true);
                m_sellBTN.SetActive(false);
                m_deleteBTN.SetActive(false);
                break;
        }
    }
    void OnClickEquip()
    {
        NetworkMng.Instance.RequestItemEquip(ItemMng.Instance.Inventory.IndexOf(m_item));
        Close();
    }
    void OnClickUnEquip()
    {
        NetworkMng.Instance.RequestItemUnequip(m_item.Type);
        Close();
    }
    void OnClickUpgrade()
    {

    }
    void OnClickMagic()
    {

    }
    void OnClickUse()
    {
        NetworkMng.Instance.RequestItemUse(m_item);
    }
    void OnClickQuickSlot()
    {
        UIMng.Instance.Open<QuickSlot>(UIMng.UIName.QuickSlot).Enabled(m_item as IQuickSlotable, true, m_item.Handle);
    }
    void OnClickBuy()
    {
        if (m_item.Price > PlayerMng.Instance.MainPlayer.Gold)
        {
            SystemMessage.Instance.PushMessage(SystemMessage.MessageType.Sub, "골드가 부족합니다.");
            return;
        }
        if (!ItemMng.Instance.CheckInventoryCount(m_item))
        {
            SystemMessage.Instance.PushMessage(SystemMessage.MessageType.Sub, "인벤토리 공간이 부족합니다.");
            return;
        }
        if (m_item is IItemNumber)
        {
            UIMng.Instance.Open<SelectPopup>(UIMng.UIName.SelectPopup).ItemNumberPopup.Enabled
            (NetworkMng.Instance.RequestItemBuyNumber, "구매", null, "취소", m_item.Name + "을(를) 구매합니다.", m_item, EPopupOption.Buy);
        }
        else
        {
            UIMng.Instance.Open<SelectPopup>(UIMng.UIName.SelectPopup).ItemPopup.Enabled
            (NetworkMng.Instance.RequestItemBuy, "구매", null, "취소", m_item.Name + "을(를) 구매합니다.", m_item.Handle, m_item.Icon, m_item.Price + "골드가 소모됩니다.");
        }
        Close();
    }
    void OnClickSell()
    {
        if(m_item is IItemNumber)
        {
            UIMng.Instance.Open<SelectPopup>(UIMng.UIName.SelectPopup).ItemNumberPopup.Enabled
            (NetworkMng.Instance.RequestItemSellNumber, "판매", null, "취소",  m_item.Name + "을(를) 판매합니다.", m_item, EPopupOption.Sell);
        }
        else
        {
            UIMng.Instance.Open<SelectPopup>(UIMng.UIName.SelectPopup).ItemPopup.Enabled
            (NetworkMng.Instance.RequestItemSell, "판매", null, "취소", m_item.Name + "을(를) 판매합니다.", ItemMng.Instance.Inventory.IndexOf(m_item), m_item.Icon, m_item.Price * 0.2 + "골드를 획득합니다.");
        }
        Close();
    }
    void OnClickDelete()
    {
        UIMng.Instance.Open<SelectPopup>(UIMng.UIName.SelectPopup).NormalPopup.Enabled(OnClickDelete, "확인", null, "취소", "아이템을 삭제합니다.");
    }
    void DeleteItem()
    {
        NetworkMng.Instance.RequestItemDelete(ItemMng.Instance.Inventory.IndexOf(m_item));
        Close();
    }
    public override void Open()
    {
        gameObject.SetActive(true);
    }
    public override void Close()
    {
        gameObject.SetActive(false);
        m_item = null;
    }
}
