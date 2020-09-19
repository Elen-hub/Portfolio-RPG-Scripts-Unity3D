using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;
using UnityEngine.UIElements;

enum EItemProduceType
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
    public System.HashInt Handle;
    public ProduceMaterialHandler OutItem;
    public ProduceMaterialHandler[] InItem;
    public System.HashInt Gold;
    public System.HashFloat ProduceTime;
    public System.HashFloat CoolTime;
    DateTime m_possibleProduceTime;
    public bool PossibleProduceItem()
    {
        if (PlayerMng.Instance.MainPlayer.Gold < Gold)
            return false;

        if ((DateTime.Now - m_possibleProduceTime).TotalSeconds < 0)
            return false;

        for (int i =0; i<InItem.Length; ++i)
        {
            Item_Base item = ItemMng.Instance.FindItemInInventory(InItem[i].Handle);
            if (item == null)
                return false;

            switch (item.Type)
            {
                case EItemType.Potion:
                case EItemType.Other:
                case EItemType.Scroll:
                    IItemNumber itemInterface = item as IItemNumber;
                    if (itemInterface.Number < InItem[i].Number)
                        return false;
                    break;
                case EItemType.Weapon:
                case EItemType.Armor:
                case EItemType.Gloves:
                case EItemType.Necklace:
                case EItemType.Ring:
                case EItemType.Shoes:
                    Item_Equipment equipItem = item as Item_Equipment;
                    if (equipItem.Value < InItem[i].Number)
                        return false;
                    break;
                default:
                    return false;
            }
        }
        return true;
    }
    public void ProduceItem(string materialArray)
    {
        PlayerMng.Instance.MainPlayer.Gold -= Gold;

        string[] materials = materialArray.Split('/');
        for(int i =0; i<materials.Length; ++i)
        {
            string[] material = materials[i].Split(',');
            int inventoryNum = int.Parse(material[0]);
            int number = int.Parse(material[1]);
            if(number ==0)
                ItemMng.Instance.RemoveItem(inventoryNum);
            else
                ItemMng.Instance.RemoveItem(inventoryNum, number);
        }

        m_possibleProduceTime = DateTime.Now;
        m_possibleProduceTime.AddSeconds(CoolTime);

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

    // 조합기능 테스트
#if UNITY_EDITOR
    private void OnGUI()
    {
        if (GUI.Button(new Rect(0, 0, 100, 100), "Test1"))
            NetworkMng.Instance.RequestProduceItem(0);

        //if (GUI.Button(new Rect(100, 0, 100, 100), "Test2"))
        //    Debug.Log(m_itemProduceFormulaDic[0].PossibleProduceItem());
    }
#endif

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
            produceFormula.InItem = new ProduceMaterialHandler[inHandlerArray.Length];
            for (int j = 0; j < inHandlerArray.Length; ++j)
            {
                string[] inHandler = inHandlerArray[j].Split(',');
                produceFormula.InItem[j] = new ProduceMaterialHandler(int.Parse(inHandler[0]), int.Parse(inHandler[1]));
            }
            produceFormula.Gold = int.Parse(Node[i]["Gold"].Value);
            produceFormula.ProduceTime = float.Parse(Node[i]["ProduceTime"].Value);
            produceFormula.CoolTime = float.Parse(Node[i]["CoolTime"].Value);
            m_itemProduceFormulaDic.Add(produceFormula.Handle, produceFormula);
        }
    }
}
