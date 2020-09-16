using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SubWindow : MonoBehaviour
{
    public enum SubWindowType
    {
        Close,
        Map,
        Party,
        Quest,
    }
    public SubWindow_Map MapWindow;
    public SubWindow_Party PartyWindow;
    public SubWindow_Quest QuestWindow;
    SubWindowType m_type;
    public void Init()
    {
        Transform button = transform.Find("AccessButton");

        button.Find("Close").GetComponent<Button>().onClick.AddListener(() => Enabled(SubWindowType.Close));
        button.Find("Map").GetComponent<Button>().onClick.AddListener(() => Enabled(SubWindowType.Map));
        button.Find("Party").GetComponent<Button>().onClick.AddListener(() => Enabled(SubWindowType.Party));
        button.Find("Quest").GetComponent<Button>().onClick.AddListener(() => Enabled(SubWindowType.Quest));

        MapWindow = GetComponentInChildren<SubWindow_Map>(true).Init();
        PartyWindow = GetComponentInChildren<SubWindow_Party>(true).Init();
        QuestWindow = GetComponentInChildren<SubWindow_Quest>(true).Init();

    }
    public void Enabled(SubWindowType type)
    {
        switch (m_type)
        {
            case SubWindowType.Map:
                MapWindow.Disabled();
                break;
            case SubWindowType.Party:
                PartyWindow.Disabled();
                break;
            case SubWindowType.Quest:
                QuestWindow.Disabled();
                break;
        }
        switch (type)
        {
            case SubWindowType.Map:
                MapWindow.Enabled();
                break;
            case SubWindowType.Party:
                PartyWindow.Enabled();
                break;
            case SubWindowType.Quest:
                QuestWindow.Enabled();
                break;
        }
        m_type = type;
    }
}
