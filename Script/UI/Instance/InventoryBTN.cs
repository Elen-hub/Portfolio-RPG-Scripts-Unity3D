using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryBTN : MonoBehaviour
{
    Item_Base m_item;
    IItemNumber m_itemNumber;
    Image m_icon;
    Text m_number;
    float m_touchElapsedTime;
    bool m_doubleTouch;
    bool m_isShop;

    public InventoryBTN Init()
    {
        GetComponent<Button>().onClick.AddListener(OnClickUse);
        m_icon = transform.Find("Icon").GetComponent<Image>();
        m_number = GetComponentInChildren<Text>();
        return this;
    }
    public void Enabled(Item_Base item, bool isShop)
    {
        m_isShop = isShop;

        if (item == m_item)
            return;

        m_item = item;
        m_icon.sprite = Resources.Load<Sprite>(m_item.Icon);
        m_icon.material = Resources.Load<Material>("Material/ItemMaterial_" + m_item.Rarity);
        m_itemNumber = item as IItemNumber;
        if (m_itemNumber != null) m_number.text = "x" + m_itemNumber.Number;
        else m_number.text = null;

        gameObject.SetActive(true);
    }
    public void Disabled()
    {
        m_item = null;
        gameObject.SetActive(false);
    }
    public void OnClickUse()
    {
        if (m_doubleTouch)
        {
            m_doubleTouch = false;

            switch(m_item.Type)
            {
                case EItemType.Weapon:
                case EItemType.Armor:
                case EItemType.Gloves:
                case EItemType.Shoes:
                case EItemType.Ring:
                case EItemType.Necklace:
                    NetworkMng.Instance.RequestItemEquip(ItemMng.Instance.Inventory.IndexOf(m_item));
                    UIMng.Instance.Open<ItemInformation>(UIMng.UIName.ItemInformation).Close();
                    break;
                case EItemType.Potion:
                case EItemType.Scroll:
                    NetworkMng.Instance.RequestItemUse(m_item);
                    UIMng.Instance.Open<ItemInformation>(UIMng.UIName.ItemInformation).Close();
                    break;
            }
        }
        else
        {
            if (m_isShop)
            {
                UIMng.Instance.Open<ItemInformation>(UIMng.UIName.ItemInformation).Open(m_item, EItemInformationOption.Sell);
            }
            else
            {
                UIMng.Instance.Open<ItemInformation>(UIMng.UIName.ItemInformation).Open(m_item);
                m_doubleTouch = true;
                m_touchElapsedTime = 0;
            }
        }
    }
    private void LateUpdate()
    {
        if (m_doubleTouch)
        {
            m_touchElapsedTime += Time.deltaTime;
            if (m_touchElapsedTime > GameSystem.DoubleTouchTime)
            {
                m_doubleTouch = false;
                m_touchElapsedTime = 0;
            }
        }

        if (m_itemNumber != null)
            m_number.text = "x" + (m_item as IItemNumber).Number.ToString();
        else
            m_number.text = null;
    }
}
