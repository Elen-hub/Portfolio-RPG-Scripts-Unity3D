using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public enum ECoinShopType
{
    Coin,
    Subscription,
    Item,
}
public class CoinShop : BaseUI
{
    ECoinShopType m_currType;
    Dictionary<ECoinShopType, CoinShopContent> m_coinShopDic = new Dictionary<ECoinShopType, CoinShopContent>();
    Image[] m_buttonImgs;
    Text[] m_texts;
    InputField m_couponField;
    protected override void InitUI()
    {
        base.InitUI();

        AddContent(ECoinShopType.Coin);
        AddContent(ECoinShopType.Subscription).Close();
        AddContent(ECoinShopType.Item).Close();

        Transform buttonGrid = transform.Find("SelectTypeButton");
        m_buttonImgs = buttonGrid.GetComponentsInChildren<Image>();
        m_texts = buttonGrid.GetComponentsInChildren<Text>();
        //buttonGrid.Find("Coin").GetComponent<Button>().onClick.AddListener(() => OnClickType(ECoinShopType.Coin));
        //buttonGrid.Find("Subscription").GetComponent<Button>().onClick.AddListener(() => OnClickType(ECoinShopType.Subscription));
        //buttonGrid.Find("Item").GetComponent<Button>().onClick.AddListener(() => OnClickType(ECoinShopType.Item));

        Transform couponGrid = transform.Find("CouponArea");
        m_couponField = couponGrid.GetComponentInChildren<InputField>();
        couponGrid.Find("CouponButton").GetComponent<Button>().onClick.AddListener(OnClickCoupon);

        transform.Find("Exit").GetComponent<Button>().onClick.AddListener(Close);
    }
    CoinShopContent AddContent(ECoinShopType type)
    {
        CoinShopContent Content = GameObject.Find(type.ToString() + "Menu").GetComponent<CoinShopContent>();
        Content.Init(type);
        m_coinShopDic.Add(type, Content);
        transform.Find("SelectTypeButton").Find(type.ToString()).GetComponent<Button>().onClick.AddListener(() => OnClickType(type));
        return Content;
    }

    public override void Open()
    {
        base.Open();
        m_couponField.text = null;
        OnClickType(ECoinShopType.Coin);
        gameObject.SetActive(true);
    }
    public override void Close()
    {
        base.Close();
        gameObject.SetActive(false);
    }
    public void OnClickType(ECoinShopType type)
    {
        m_coinShopDic[m_currType].Close();
        m_buttonImgs[(int)m_currType].color = Color.white;
        m_texts[(int)m_currType].color = new Color(0.5882f, 0.5882f, 0.5882f, 1);
        m_currType = type;
        m_texts[(int)m_currType].color = Color.white;
        m_buttonImgs[(int)m_currType].color = new Color(0, 0, 0, 0.5f);
        m_coinShopDic[m_currType].Open();
    }
    public void OnClickCoupon()
    {
        if (m_couponField.text != null)
            UIMng.Instance.Open<SelectPopup>(UIMng.UIName.SelectPopup).NormalPopup.Enabled(() => { NetworkMng.Instance.RequestCouponUse(m_couponField.text); m_couponField.text = null; }, "확인", null, "취소", "쿠폰을 사용합니다. \n 일련번호: <color=red>" + m_couponField.text + "</color>");
    }
}
