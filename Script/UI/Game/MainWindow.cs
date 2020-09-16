using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainWindow : MonoBehaviour
{
    bool m_isOpen;
    Animator m_animator;
    public void Init()
    {
        m_animator = GetComponent<Animator>();
        transform.Find("ActiveButton").GetComponent<Button>().onClick.AddListener(Active);
        Transform main = transform.Find("MainMenuButton");
        main.Find("Status").GetComponent<Button>().onClick.AddListener(OnClickStatus);
        main.Find("Inventory").GetComponent<Button>().onClick.AddListener(OnClickInventory);
        main.Find("Skill").GetComponent<Button>().onClick.AddListener(OnClickSkill);
        main.Find("Post").GetComponent<Button>().onClick.AddListener(OnClickPost);
        main.Find("Option").GetComponent<Button>().onClick.AddListener(OnClickOption);
        main.Find("Quest").GetComponent<Button>().onClick.AddListener(OnClickQuest);
        main.Find("Shop").GetComponent<Button>().onClick.AddListener(OnClickShop);
        Transform sub = transform.Find("SubMenuButton");
        sub.Find("QuickSlot").GetComponent<Button>().onClick.AddListener(OnClickQuickSlot);
        sub.Find("Camera").GetComponent<Button>().onClick.AddListener(OnClickCamera);
        sub.Find("Party").GetComponent<Button>().onClick.AddListener(OnClickParty);
        Disabled();
    }
    public void Enabled()
    {
        m_isOpen = true;
        m_animator.Play("Open");
    }
    public void Disabled()
    {
        m_isOpen = false;
        m_animator.Play("Close");
    }
    void Active()
    {
        if (m_isOpen) Disabled();
        else Enabled();
    }
    void OnClickStatus()
    {
        UIMng.Instance.Open<Status>(UIMng.UIName.Status).Open(PlayerMng.Instance.MainPlayer.Character);
        Disabled();
    }
    void OnClickInventory()
    {
        UIMng.Instance.Open<Inventory>(UIMng.UIName.Inventory).Open(false);
        Disabled();
    }
    void OnClickSkill()
    {
        UIMng.Instance.OPEN = UIMng.UIName.Skill;
        Disabled();
    }
    void OnClickShop()
    {
        UIMng.Instance.OPEN = UIMng.UIName.CoinShop;
        Disabled();
    }
    void OnClickQuickSlot()
    {
        UIMng.Instance.OPEN = UIMng.UIName.QuickSlot;
        Disabled();
    }
    void OnClickPost()
    {
        NetworkMng.Instance.RequestPostData();
        Disabled();
    }
    void OnClickOption()
    {
        UIMng.Instance.OPEN = UIMng.UIName.Option;
        Disabled();
    }
    void OnClickCamera()
    {
        UIMng.Instance.OPEN = UIMng.UIName.CameraUI;
        Disabled();
    }
    void OnClickQuest()
    {
        UIMng.Instance.OPEN = UIMng.UIName.QuestUI;
        Disabled();
    }
    void OnClickParty()
    {
        NetworkMng.Instance.NotifyRequestPartyList();
        Disabled();
    }
}
