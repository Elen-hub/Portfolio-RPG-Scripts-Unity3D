using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NPCUI_Shop : MonoBehaviour
{
    List<ShopBTN> m_shopList = new List<ShopBTN>();
    Animator m_animator;
    Transform m_grid;
    public void Init()
    {
        m_animator = GetComponent<Animator>();
        m_grid = GetComponentInChildren<VerticalLayoutGroup>(true).transform;
        transform.Find("Shop").Find("Exit").GetComponent<Button>().onClick.AddListener(() => Disabled(true));
        gameObject.SetActive(false);
    }
    public void Enabled(List<int> shopHandle)
    {
        for (int i = 0; i < shopHandle.Count; ++i)
        {
            if (m_shopList.Count <= i)
                m_shopList.Add(Instantiate(Resources.Load<ShopBTN>("UI/Instance/ShopBTN"), m_grid).Init());

            m_shopList[i].Enabled(shopHandle[i]);
        }
        gameObject.SetActive(true);
        m_animator.Play("Open");
    }
    public void Disabled(bool isInventory)
    {
        for (int i = 0; i < m_shopList.Count; ++i)
            m_shopList[i].Disabled();

        if(isInventory)
            UIMng.Instance.CLOSE = UIMng.UIName.Inventory;

        m_animator.Play("Close");
    }
    public void Close()
    {
        for (int i = 0; i < m_shopList.Count; ++i)
            m_shopList[i].Disabled();

        gameObject.SetActive(false);
    }
}
