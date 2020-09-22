using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NPCUI_ItemProduce : MonoBehaviour
{
    public delegate void DProduceFormulaBTN(ItemProduceFormula produceFormula);

    NPCUI_ItemProduceVisible m_itemProduceVisible;
    List<NPCUI_ItemProduce_FormulaContent> m_formulaContentList = new List<NPCUI_ItemProduce_FormulaContent>();
    Transform m_viewPort;
    public void Init()
    {
        m_itemProduceVisible = transform.GetComponentInChildren<NPCUI_ItemProduceVisible>().Init();
        transform.Find("Exit").GetComponent<Button>().onClick.AddListener(Disabled);
        m_viewPort = GetComponentInChildren<ContentSizeFitter>().transform;
        gameObject.SetActive(false);
    }
    public void Enabled(List<int> produceList)
    {
        for(int i =0; i<produceList.Count; ++i)
        {
            ItemProduceFormula formula = ItemMng.Instance.GetItemProduceFormula(produceList[i]);
            if (i >= m_formulaContentList.Count)
                m_formulaContentList.Add(Instantiate(Resources.Load<NPCUI_ItemProduce_FormulaContent>("UI/Instance/FormulaContent"), m_viewPort).Init(SelectProduceFormula));

            m_formulaContentList[i].Enabled(formula);
        }
        m_itemProduceVisible.Enabled(null);
        gameObject.SetActive(true);
    }
    public void Disabled()
    {
        for (int i = 0; i < m_formulaContentList.Count; ++i)
            m_formulaContentList[i].Disabled();

        m_itemProduceVisible.Disabled();
        gameObject.SetActive(false);
    }
    void SelectProduceFormula(ItemProduceFormula produceFormulaHandle)
    {
        m_itemProduceVisible.Enabled(produceFormulaHandle);
    }
    public void Close()
    {
        m_itemProduceVisible.Disabled();
        gameObject.SetActive(false);
    }
}
