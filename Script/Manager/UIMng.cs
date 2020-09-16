using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIMng : TSingleton<UIMng>
{
    public enum UIName
    {
        Logo,
        Title,
        SelectPopup,
        FieldUI,
        SystemMessage,
        Game,
        Status,
        Inventory,
        ItemInformation,
        QuestInformation,
        QuickSlot,
        Skill,
        Post,
        PostInformation,
        Option,
        CameraUI,
        NPCUI,
        QuestUI,
        CoinShop,
        SelectStage,
        PartyMenu,
        Loading,
        LoadingScene,
    }

    private Dictionary<UIName, BaseUI> m_uiDic = new Dictionary<UIName, BaseUI>();
    public EventSystem EventSystem;
    public override void Init()
    {
        EventSystem = Instantiate(Resources.Load<EventSystem>("UI/EventSystem"), transform);
        OPEN = UIName.SystemMessage;
        OPEN = UIMng.UIName.FieldUI;
        IsLoad = true;
    }

    public UIName CLOSE  { set  { m_uiDic[value].Close(); } }
    public UIName DESTROY { set { Destroy(m_uiDic[value].gameObject); m_uiDic.Remove(value); } }
    public UIName OPEN { set { Open<BaseUI>(value); } }

    public T Open<T>(UIName uiName) where T : BaseUI
    {
        if (!m_uiDic.ContainsKey(uiName))
        {
            T prefabs = Resources.Load<T>("UI/" + uiName.ToString());

            if (prefabs == null)
                return null;

            T obj = Instantiate<T>(prefabs);
            m_uiDic.Add(uiName, obj);
            obj.Init();
            obj.Open();
            obj.transform.SetParent(transform);

            return obj;
        }
        else
        {
            m_uiDic[uiName].Open();
            return m_uiDic[uiName] as T;
        }
    }
    public bool IsActiveUI(UIName uiName)
    {
        if (m_uiDic.ContainsKey(uiName))
            return m_uiDic[uiName].gameObject.activeSelf;
        else
            return false;
    }
    public T GetUI<T>(UIName uiName) where T : BaseUI
    {
        if (m_uiDic.ContainsKey(uiName))
            return m_uiDic[uiName] as T;

        return null;
    }
}
