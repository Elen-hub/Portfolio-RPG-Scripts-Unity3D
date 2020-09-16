using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum EPopupOption
{
    Buy,
    Sell,
}
public class SelectPopup_ItemNumber : SelectPopup_Base
{
    Item_Base m_item;
    protected DoubleIntFunction SuccessFunction;
    protected VoidFunction ExitFunction;
    Image m_itemImg;
    Text m_itemGoldText;
    InputField m_itemNumberInput;
    int m_currNum;
    EPopupOption m_option;

    public override void Init()
    {
        base.Init();

        m_itemNumberInput = transform.GetComponentInChildren<InputField>();
        m_itemNumberInput.onValueChanged.AddListener(OnValueChanged);
        m_itemImg = transform.Find("ItemIcon").GetComponent<Image>();
        m_itemGoldText = transform.Find("ItemGoldText").GetComponent<Text>();
        transform.Find("Success").GetComponent<Button>().onClick.AddListener(Success);
        transform.Find("Exit").GetComponent<Button>().onClick.AddListener(Exit);
        transform.Find("Down").GetComponent<Button>().onClick.AddListener(OnClickDown);
        transform.Find("Up").GetComponent<Button>().onClick.AddListener(OnClickUp);
        gameObject.SetActive(false);
    }
    public void Enabled(DoubleIntFunction SuccessF, string SucceesT, VoidFunction ExitF, string ExitT, string Status, Item_Base Item, EPopupOption Option)
    {
        m_item = Item;
        if (Option == EPopupOption.Sell)
            m_currNum = (m_item as IItemNumber).Number;
        else
            m_currNum = 1;

        m_itemImg.sprite = Resources.Load<Sprite>(Item.Icon);
        SuccessFunction = SuccessF;
        m_successText.text = SucceesT;
        ExitFunction = ExitF;
        m_exitText.text = ExitT;
        m_status.text = Status;
        m_option = Option;
        gameObject.SetActive(true);
        m_itemNumberInput.text = m_currNum.ToString();
    }
    public void Disabled()
    {
        SuccessFunction = null;
        ExitFunction = null;
        m_item = null;

        gameObject.SetActive(false);
    }
    void Success()
    {
        switch(m_option)
        {
            case EPopupOption.Buy:
                SuccessFunction?.Invoke(m_item.Handle, m_currNum);
                break;
            case EPopupOption.Sell:
                SuccessFunction?.Invoke(ItemMng.Instance.Inventory.IndexOf(m_item), m_currNum);
                break;
        }
        UIMng.Instance.CLOSE = UIMng.UIName.SelectPopup;
    }

    void Exit()
    {
        ExitFunction?.Invoke();
        UIMng.Instance.CLOSE = UIMng.UIName.SelectPopup;
    }
    void OnClickUp()
    {
        switch(m_option)
        {
            case EPopupOption.Sell:
                IItemNumber ItemNumber = m_item as IItemNumber;
                if (ItemNumber.Number > m_currNum)
                {
                    m_currNum += 1;
                    m_itemNumberInput.text = m_currNum.ToString();
                }
                break;
            case EPopupOption.Buy:
                if((m_currNum+1) * m_item.Price <= PlayerMng.Instance.MainPlayer.Gold)
                {
                    m_currNum += 1;
                    m_itemNumberInput.text = m_currNum.ToString();
                }
                break;
        }
    }
    void OnClickDown()
    {
        switch(m_option)
        {
            case EPopupOption.Sell:
                if (m_currNum > 1)
                {
                    m_currNum -= 1;
                    m_itemNumberInput.text = m_currNum.ToString();
                }
                break;
            case EPopupOption.Buy:
                if(m_currNum > 1)
                {
                    m_currNum -= 1;
                    m_itemNumberInput.text = m_currNum.ToString();
                }
                break;
        }
    }
    public void OnValueChanged(string arr)
    {
        if (!int.TryParse(arr, out m_currNum))
            return;

        IItemNumber ItemNumber = m_item as IItemNumber;

        switch (m_option)
        {
            case EPopupOption.Sell:
                if (ItemNumber.Number <= m_currNum)
                {
                    m_currNum = ItemNumber.Number;
                }
                else if(m_currNum < 1)
                {
                    m_currNum = 1;
                }
                m_itemGoldText.text = m_item.Price * 0.2 * m_currNum + "골드를 획득합니다.";
                break;
            case EPopupOption.Buy:
                if (m_item.Price * m_currNum > PlayerMng.Instance.MainPlayer.Gold)
                {
                    m_currNum = PlayerMng.Instance.MainPlayer.Gold/m_currNum;
                    if (m_currNum > 999)
                        m_currNum = 999;
                }
                else if (m_currNum < 1)
                {
                    m_currNum = 1;
                }
                m_itemGoldText.text = m_item.Price * m_currNum + "골드가 소모됩니다.";
                break;
        }
    }
}
