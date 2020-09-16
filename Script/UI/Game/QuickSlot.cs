using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuickSlot : BaseUI
{
    QuickSlotBTN[] m_quickSlotBTN = new QuickSlotBTN[8];

    public bool IsChange;
    public IQuickSlotable Content;
    public int Handle;
    public bool IsItem;

    protected override void InitUI()
    {
        transform.Find("Exit").GetComponent<Button>().onClick.AddListener(Close);
        Transform QuickSlotGrid = transform.Find("QuickSlot");
        for (int i = 0; i < m_quickSlotBTN.Length; ++i)
            m_quickSlotBTN[i] = QuickSlotGrid.GetChild(i).GetComponent<QuickSlotBTN>().Init(i);
    }
    public void Enabled(IQuickSlotable value, bool isItem, int handle)
    {
        if (value != null)
        {
            Handle = handle;
            Content = value;
            IsItem = isItem;
            IsChange = true;
        }
    }
    public override void Open()
    {
        for (int i = 0; i < m_quickSlotBTN.Length; ++i)
            m_quickSlotBTN[i].Open();

        gameObject.SetActive(true);
    }
    public override void Close()
    {
        Content = null;
        IsChange = false;
        gameObject.SetActive(false);
    }
}
