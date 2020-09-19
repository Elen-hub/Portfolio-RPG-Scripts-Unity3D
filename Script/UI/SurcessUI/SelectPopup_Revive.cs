using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectPopup_Revive : SelectPopup_Base
{
    Text m_number;
    
    public override void Init()
    {
        base.Init();

        m_number = transform.Find("Number").GetComponent<Text>();
        transform.Find("Revive").GetComponent<Button>().onClick.AddListener(Revive);
        transform.Find("Buy").GetComponent<Button>().onClick.AddListener(Buy);
        transform.Find("Exit").GetComponent<Button>().onClick.AddListener(ReturnToHome);
        gameObject.SetActive(false);
    }
    public void Enabled()
    {
        Item_Base item = ItemMng.Instance.FindItemInInventory(14);

        if (item != null)
            m_number.text = "x" + (item as IItemNumber).Number;
        else
            m_number.text = "x0";

        gameObject.SetActive(true);
    }
    public void Disabled()
    {
        gameObject.SetActive(false);
    }
    void Revive()
    {

    }
    void Buy()
    {

    }
    void ReturnToHome()
    {
        NetworkMng.Instance.RequestExitPrivateMap();
        UIMng.Instance.CLOSE = UIMng.UIName.SelectPopup;
    }
}
