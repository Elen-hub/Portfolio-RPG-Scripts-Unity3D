using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using NUnit.Framework.Internal.Execution;

public class ItemSelector : EditorWindow
{
    static EditorWindow m_window;
    static GUIStyle m_guiStyle;
    static float m_windowSize;

    static Del_Selection DHandlerSelectionMethod;

    static Dictionary<int, ItemSelector_Content> m_contentList = new Dictionary<int, ItemSelector_Content>();

    static EItemType m_showType;

    static float m_scrollbarValue;
    public static void ShowWindow(Del_Selection selectionMethod, Vector2 position)
    {
        m_window = GetWindow(typeof(ItemSelector));
        m_window.position = new Rect(position, m_window.minSize);
        m_window.minSize = new Vector2(400, 50);
        m_window.maxSize = new Vector2(400, 600);
        m_windowSize = m_window.maxSize.x;
        m_guiStyle = new GUIStyle();
        m_guiStyle.richText = true;

        DHandlerSelectionMethod = selectionMethod;

        m_showType = EItemType.All;
        m_scrollbarValue = 0;
    }
    private void OnGUI()
    {
        m_window.Show();
        SelectionItem();
    }
    public void SelectionItem()
    {
        float ySize = 10;
        ySize += 25f - m_scrollbarValue;
        foreach (Item_Base item in EditorDB.ItemDic.Values)
        {
            Rect rect = new Rect(0, ySize, m_windowSize-20, 50);
            if(!m_contentList.ContainsKey(item.Handle))
                m_contentList.Add(item.Handle, new ItemSelector_Content(item.Handle));

            if (m_contentList[item.Handle].ShowSelectButton(rect))
            {
                DHandlerSelectionMethod(item.Handle);
                m_window.Close();
            }
            ySize += 50;
        }
        if (ySize < 600)
            m_window.minSize = new Vector2(m_windowSize, ySize);
        else
            m_window.minSize = new Vector2(m_windowSize, 600);

        GUI.Box(new Rect(0, 0, m_windowSize, 15), "", EditorStyles.toolbar);
        GUI.Box(new Rect(0, 15, m_windowSize, 15), "", EditorStyles.toolbar);
        m_showType = (EItemType)EditorGUI.EnumPopup(new Rect(0, 10, m_windowSize - 20, 25), m_showType);
        m_scrollbarValue = GUI.VerticalScrollbar(new Rect(m_windowSize - 20, 35, 20, m_window.minSize.y-35), m_scrollbarValue, EditorDB.ItemDic.Count, 0, ySize + 50);
    }
}
