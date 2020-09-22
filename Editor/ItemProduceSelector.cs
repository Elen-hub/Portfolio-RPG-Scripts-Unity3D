using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using NUnit.Framework.Internal.Execution;

public class ItemProduceSelector : EditorWindow
{
    static EditorWindow m_window;
    static GUIStyle m_guiStyle;
    static float m_windowSize;

    static Del_Selection DHandlerSelectionMethod;

    static Dictionary<int, ItemSelector_Content> m_contentList = new Dictionary<int, ItemSelector_Content>();
    public static void ShowWindow(Del_Selection selectionMethod, Vector2 position)
    {
        m_window = GetWindow(typeof(ItemProduceSelector));
        m_window.minSize = new Vector2(400, 50);
        m_window.maxSize = new Vector2(400, 600);
        m_windowSize = m_window.maxSize.x;
        m_guiStyle = new GUIStyle();
        m_guiStyle.richText = true;

        m_window.position = new Rect(position, m_window.minSize);
        DHandlerSelectionMethod = selectionMethod;
    }
    private void OnGUI()
    {
        m_window.Focus();
        SelectionProduceFormula();
    }
    public void SelectionProduceFormula()
    {
        float ySize = 10;
        foreach(ItemProduceFormula fomula in EditorDB.ItemProduceDic.Values)
        {
            if (!EditorDB.ItemDic.ContainsKey(fomula.OutItem.Handle))
                continue;
            
            Rect rect = new Rect(0, ySize, m_windowSize, 50);
            if (!m_contentList.ContainsKey(fomula.OutItem.Handle))
                m_contentList.Add(fomula.OutItem.Handle, new ItemSelector_Content(fomula.OutItem.Handle));

            if(m_contentList[fomula.OutItem.Handle].ShowSelectButton(rect))
            {
                DHandlerSelectionMethod(fomula.Handle);
                m_window.Close();
            }

            ySize += 50;
        }
        m_window.minSize = new Vector2(m_windowSize, ySize);
    }
}
