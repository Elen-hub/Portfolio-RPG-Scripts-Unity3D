using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class CoinShopContent : MonoBehaviour
{
    List<CoinShopContentBTN> m_contentList = new List<CoinShopContentBTN>();
    public void Init(ECoinShopType type)
    {
        CoinShopContentBTN[] Contens = GetComponentsInChildren<CoinShopContentBTN>(true);
        for(int i =0; i<Contens.Length; ++i)
            Contens[i].Init(type, i);
    }
    public void Open()
    {
        gameObject.SetActive(true);
    }
    public void Close()
    {
        gameObject.SetActive(false);
    }
}
