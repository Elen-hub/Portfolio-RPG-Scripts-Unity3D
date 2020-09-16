using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class CoinShopContentBTN : MonoBehaviour
{
    Image m_img;
    Text m_subject;
    Text m_price;
    ECoinShopType m_type;
    int m_number;
    public void Init(ECoinShopType type, int number)
    {
        m_img = transform.Find("Image").GetComponent<Image>();
        m_subject = transform.Find("Subject").GetComponent<Text>();
        m_price = transform.Find("Price").GetComponent<Text>();
        m_type = type;
        m_number = number;
        GetComponent<Button>().onClick.AddListener(OnClickBuy);
    }
    public void OnClickBuy()
    {
        switch(m_type)
        {
            case ECoinShopType.Subscription:
                string code = NetworkMng.Instance.SubscriptionArr[m_number];
                if (NetworkMng.Instance.CurrSubscriptionList.Contains(code))
                {
                    SystemMessage.Instance.PushMessage(SystemMessage.MessageType.Sub, "이미 구독중인 상품입니다.");
                    return;
                }
                UIMng.Instance.Open<SelectPopup>(UIMng.UIName.SelectPopup).NormalPopup.Enabled(() => NetworkMng.Instance.RequestSubscriptionAccount(m_number), "구독", null, "취소", m_subject.text + "를 구독합니다.");
                break;
            case ECoinShopType.Coin:
                break;
            case ECoinShopType.Item:
                break;
        }
        // UIMng.Instance.Open<SelectPopup>(UIMng.UIName.SelectPopup).NormalPopup.Enabled(() => NetworkMng.Instance.RequestShopCode(m_type, m_number), "구매", null, "취소", m_subject.text + "를 구매합니다.");
    }
}
