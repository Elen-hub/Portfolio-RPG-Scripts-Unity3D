using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class ItemSelector_Content
{
    int m_handle;
    Rect m_rect;
    Texture2D m_texture;

    public ItemSelector_Content(int handle)
    {
        if (!EditorDB.ItemDic.ContainsKey(handle))
            return;

        m_handle = handle;
        m_texture = new Texture2D(45, 45);
        m_texture.LoadImage(File.ReadAllBytes("Assets/Resources/" + EditorDB.ItemDic[handle].Icon + ".png"));
    }
    public bool ShowSelectButton(Rect rect)
    {
        if (GUI.Button(rect, ""))
            return true;

        GUI.DrawTexture(new Rect(rect.x, rect.y, 45, 45), m_texture);
        EditorGUI.LabelField(new Rect(rect.x + 50, rect.y+5, rect.width - 50, 20), EditorDB.ItemDic[m_handle].Name + " (" + ParseLib.GetRairityKorConvert(EditorDB.ItemDic[m_handle].Rarity) + ")");
        EditorGUI.LabelField(new Rect(rect.x + 50, rect.y+25, rect.width - 50, 20), EditorDB.ItemDic[m_handle].Explanation);
        return false;
    }
}
