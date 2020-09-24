using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;
using UnityEngine.UIElements;

public enum EItemProduceType
{
    Material,
    Potion,
    Equip,
}
public struct ProduceMaterialHandler
{
    public ProduceMaterialHandler(int handle, int number)
    {
        Handle = handle;
        Number = number;
    }
    public int Handle;
    public int Number;
}
public class ItemProduceFormula
{
    public EItemProduceType ItemProduceType;
    public System.HashInt Handle;
    public ProduceMaterialHandler OutItem;
    public List<ProduceMaterialHandler> InItem = new List<ProduceMaterialHandler>();
    public System.HashInt Level;
    public System.HashInt Gold;
    public System.HashFloat ProduceTime;
    public System.HashFloat CoolTime;
    DateTime m_possibleProduceTime = DateTime.Now;
    public float GetCurrCoolTime{ get { return (float)(DateTime.Now - m_possibleProduceTime).TotalSeconds; } }
    public bool PossibleProduceItem()
    {
        if (PlayerMng.Instance.MainPlayer.Gold < Gold)
        {
            SystemMessage.Instance.PushMessage(SystemMessage.MessageType.Sub, "제작에 필요한 골드가 부족합니다.");
            return false;
        }

        if ((DateTime.Now - m_possibleProduceTime).TotalSeconds < 0)
        {
            SystemMessage.Instance.PushMessage(SystemMessage.MessageType.Sub, "아직 제작 할 수 없습니다.");
            return false;
        }

        for (int i =0; i<InItem.Count; ++i)
        {
            Item_Base item = ItemMng.Instance.FindItemInInventory(InItem[i].Handle);
            if (item == null)
            {
                SystemMessage.Instance.PushMessage(SystemMessage.MessageType.Sub, "제작에 필요한 재료가 부족합니다");
                return false;
            }

            switch (item.Type)
            {
                case EItemType.Potion:
                case EItemType.Other:
                case EItemType.Scroll:
                    IItemNumber itemInterface = item as IItemNumber;
                    if (itemInterface.Number < InItem[i].Number)
                    {
                        SystemMessage.Instance.PushMessage(SystemMessage.MessageType.Sub, "제작에 필요한 재료가 부족합니다");
                        return false;
                    }
                    break;
                case EItemType.Weapon:
                case EItemType.Armor:
                case EItemType.Gloves:
                case EItemType.Necklace:
                case EItemType.Ring:
                case EItemType.Shoes:
                    Item_Equipment equipItem = item as Item_Equipment;
                    if (equipItem.Value < InItem[i].Number)
                    {
                        SystemMessage.Instance.PushMessage(SystemMessage.MessageType.Sub, "제작에 필요한 재료가 부족합니다");
                        return false;
                    }
                    break;
                default:
                    SystemMessage.Instance.PushMessage(SystemMessage.MessageType.Sub, "제작에 필요한 재료가 부족합니다");
                    return false;
            }
        }
        return true;
    }
    public void ProduceItem()
    {
        PlayerMng.Instance.MainPlayer.Gold -= Gold;

        for (int i = 0; i < InItem.Count; ++i)
        {
            Item_Base item = ItemMng.Instance.FindItemInInventory(InItem[i].Handle);
            switch (item.Type)
            {
                case EItemType.Potion:
                case EItemType.Other:
                case EItemType.Scroll:
                    ItemMng.Instance.RemoveItem(item, InItem[i].Number);
                    break;
                case EItemType.Weapon:
                case EItemType.Armor:
                case EItemType.Gloves:
                case EItemType.Necklace:
                case EItemType.Ring:
                case EItemType.Shoes:
                    ItemMng.Instance.RemoveItem(item);
                    break;
                default:
                    break;
            }
        }

        m_possibleProduceTime = DateTime.Now.AddSeconds(CoolTime);

        Item_Base cloneItem = ItemMng.Instance.GetItemList[OutItem.Handle].Clone();
        switch (cloneItem.Type)
        {
            case EItemType.Potion:
            case EItemType.Other:
            case EItemType.Scroll:
                IItemNumber itemInterface = cloneItem as IItemNumber;
                itemInterface.Number = OutItem.Number;
                break;
            case EItemType.Weapon:
            case EItemType.Armor:
            case EItemType.Gloves:
            case EItemType.Necklace:
            case EItemType.Ring:
            case EItemType.Shoes:
                Item_Equipment equipItem = cloneItem as Item_Equipment;
                equipItem.Value = OutItem.Number;
                break;
            default:
                break;
        }
        ItemMng.Instance.AddItem(cloneItem);
    }
}
public partial class ItemMng : TSingleton<ItemMng>
{
    Dictionary<int, ItemProduceFormula> m_itemProduceFormulaDic = new Dictionary<int, ItemProduceFormula>();

    public ItemProduceFormula GetItemProduceFormula(int handle)
    {
        return m_itemProduceFormulaDic[handle];
    }

    void ItemProduceInit()
    {
#if UNITY_EDITOR
        TextAsset Asset = new TextAsset(System.IO.File.ReadAllText("Assets/AssetBundle_Database/DB_ItemProduce.json"));
        JSONNode Node = JSON.Parse(Asset.text);
#else
        JSONNode Node = JSON.Parse(AssetMng.Instance["database"].LoadAsset<TextAsset>("DB_ItemProduce").text);
#endif
        for (int i = 0; i < Node.Count; ++i)
        {
            ItemProduceFormula produceFormula = new ItemProduceFormula();
            produceFormula.Handle = int.Parse(Node[i]["Handle"].Value);
            string[] outHandler = Node[i]["OutItemHandler"].Value.Split(',');
            produceFormula.OutItem = new ProduceMaterialHandler(int.Parse(outHandler[0]), int.Parse(outHandler[1]));
            string[] inHandlerArray = Node[i]["InItemHandler"].Value.Split('/');
            for (int j = 0; j < inHandlerArray.Length; ++j)
            {
                string[] inHandler = inHandlerArray[j].Split(',');
                produceFormula.InItem.Add(new ProduceMaterialHandler(int.Parse(inHandler[0]), int.Parse(inHandler[1])));
            }
            produceFormula.ItemProduceType = (EItemProduceType)int.Parse(Node[i]["Type"].Value);
            produceFormula.Level = int.Parse(Node[i]["Level"].Value);
            produceFormula.Gold = int.Parse(Node[i]["Gold"].Value);
            produceFormula.ProduceTime = float.Parse(Node[i]["ProduceTime"].Value);
            produceFormula.CoolTime = float.Parse(Node[i]["CoolTime"].Value);
            m_itemProduceFormulaDic.Add(produceFormula.Handle, produceFormula);
        }
    }
}
