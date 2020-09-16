using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text;

public delegate void VoidFunction();
public delegate void IntFunction(int arr);
public delegate void DoubleIntFunction(int arr0, int arr1);
public class SelectPopup : BaseUI
{
    public SelectPopup_Normal NormalPopup;
    public SelectPopup_Item ItemPopup;
    public SelectPopup_ItemNumber ItemNumberPopup;
    public SelectPopup_Reword RewordPopup;
    public SelectPopup_PrivateJoin JoinPopup;
    public SelectPopup_Revive RevivePopup;
    Image m_bg;

    protected override void InitUI()
    {
        NormalPopup = GetComponentInChildren<SelectPopup_Normal>();
        NormalPopup.Init();
        ItemPopup = GetComponentInChildren<SelectPopup_Item>();
        ItemPopup.Init();
        ItemNumberPopup = GetComponentInChildren<SelectPopup_ItemNumber>();
        ItemNumberPopup.Init();
        RewordPopup = GetComponentInChildren<SelectPopup_Reword>();
        RewordPopup.Init();
        JoinPopup = GetComponentInChildren<SelectPopup_PrivateJoin>();
        JoinPopup.Init();
        RevivePopup = GetComponentInChildren<SelectPopup_Revive>();
        RevivePopup.Init();
        m_bg = transform.Find("BG").GetComponent<Image>();

        gameObject.SetActive(false);
    }
    public override void Open()
    {
        m_bg.gameObject.SetActive(false);
        gameObject.SetActive(true);
    }
    public override void Close()
    {
        NormalPopup.Disabled();
        ItemPopup.Disabled();
        ItemNumberPopup.Disabled();
        JoinPopup.Disabled();
        RewordPopup.Disabled();
        RevivePopup.Disabled();

        m_bg.gameObject.SetActive(true);
        gameObject.SetActive(false);
    }
}
