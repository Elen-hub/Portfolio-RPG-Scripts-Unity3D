using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public class QuestEditor_Content
{
    public Rect Rect;
    public QuestContent QuestContent;
    public GUIStyle GuiStyle;
    public GUIStyle DefaultStyle;
    public GUIStyle SelectStyle;
    public bool IsDragged;
    public bool IsSelected;
    public Action<QuestEditor_Content> OnRemoveNode;
    public QuestEditor_Content(Vector2 pos, float width, float height, GUIStyle defaultStyle, GUIStyle selectStyle, Action<QuestEditor_Content> onRemoveNode, QuestContent quest)
    {
        QuestContent = quest;
        GuiStyle = defaultStyle;
        DefaultStyle = defaultStyle;
        SelectStyle = selectStyle;
        OnRemoveNode = onRemoveNode;
        Rect = new Rect(pos.x, pos.y, width, height);
    }
    public void Drag(Vector2 delta)
    {
        Rect.position += delta;
    }
    public bool Events(Event e)
    {
        switch (e.type)
        {
            case EventType.MouseDown:
                if (e.button == 0)
                {
                    if (Rect.Contains(e.mousePosition))
                    {
                        IsDragged = true;
                        GUI.changed = true;
                        IsSelected = true;
                        GuiStyle = SelectStyle;
                    }
                    else
                    {
                        GUI.changed = true;
                        IsSelected = false;
                        GuiStyle = DefaultStyle;
                    }
                }
                if (e.button == 1 && IsSelected && Rect.Contains(e.mousePosition))
                {
                    ProcessContextMenu();
                    e.Use();
                }
                break;

            case EventType.MouseUp:
                IsDragged = false;
                break;

            case EventType.MouseDrag:
                if (e.button == 0 && IsDragged)
                {
                    Drag(e.delta);
                    e.Use();
                    return true;
                }
                break;
        }
        return false;
    }
    private void ProcessContextMenu()
    {
        GenericMenu genericMenu = new GenericMenu();
        genericMenu.AddItem(new GUIContent("Remove node"), false, OnClickRemoveNode);
        genericMenu.ShowAsContext();
    }
    private void OnClickRemoveNode()
    {
        if (OnRemoveNode != null)
            OnRemoveNode(this);
    }
    public void Draw()
    {
        Rect rect = Rect;
        rect.height += (QuestContent.ClearScripts.Count + QuestContent.Scripts.Count + QuestContent.Reword.Count*2) * 20;

        if(GUI.Button(new Rect(rect.x+rect.width-20,rect.y,20,20), "-"))
        {
            OnClickRemoveNode();
            return;
        }

        if (QuestContent.Type == EQuestClearType.Talk)
            rect.height -= 40;

        GUI.Box(rect, QuestContent.ContentName, GuiStyle);

        GUIStyle style = EditorStyles.helpBox;
        style.richText = true;
        style.fontSize = 12;
        float posY = 20;

        EditorGUI.LabelField(new Rect(10 + rect.x, rect.y + posY, rect.width, 20), "Chapter: " + QuestContent.Handle);
        posY += 20;

        EditorGUI.LabelField(new Rect(10 + rect.x, rect.y + posY, rect.width/2 - 30, 20), "Name: ");
        QuestContent.ContentName = EditorGUI.TextField(new Rect(rect.x + rect.width / 2 -10, rect.y + posY, rect.width/2, 20), QuestContent.ContentName);
        posY += 25;

        if(EditorDB.NPCStatDic.ContainsKey(QuestContent.NPCHandle))
            EditorGUI.LabelField(new Rect(10 + rect.x, rect.y + posY, rect.width / 2 - 20, 20), "NPC: " + EditorDB.NPCStatDic[QuestContent.NPCHandle].Name);
        else
            EditorGUI.LabelField(new Rect(10 + rect.x, rect.y + posY, rect.width / 2 - 20, 20), "NPC: Not Found");
        QuestContent.NPCHandle = EditorGUI.IntField(new Rect(rect.x + rect.width / 2-10, rect.y + posY, rect.width / 2, 20), QuestContent.NPCHandle);
        posY += 25;

        EditorGUI.LabelField(new Rect(10 + rect.x, rect.y + posY, rect.width / 2 - 20, 20), "Level: ");
        QuestContent.Level = EditorGUI.IntField(new Rect(rect.x + rect.width / 2 - 10, rect.y + posY, rect.width / 2, 20), QuestContent.Level);
        posY += 25;

        EditorGUI.LabelField(new Rect(10 + rect.x, rect.y + posY, rect.width / 2 - 20, 20), "Script");
        if (GUI.Button(new Rect(rect.x + rect.width - 30, rect.y + posY, 20, 20), "+"))
            QuestContent.Scripts.Add("비어있는 스크립트");
        posY += 20;
        for (int i =0; i<QuestContent.Scripts.Count; ++i)
        {
            QuestContent.Scripts[i] = EditorGUI.TextField(new Rect(10 + rect.x, rect.y + posY, rect.width - 40, 20), QuestContent.Scripts[i]);
            if(GUI.Button(new Rect(10+rect.x+rect.width-40, rect.y+posY, 20,20), "-"))
            {
                QuestContent.Scripts.RemoveAt(i);
                return;
            }
            posY += 20;
        }
        posY += 5;

        EditorGUI.LabelField(new Rect(10 + rect.x, rect.y + posY, rect.width / 2 - 20, 20), "Type: ");
        QuestContent.Type = (EQuestClearType)EditorGUI.EnumPopup(new Rect(10 + rect.x, rect.y + posY, rect.width - 40, 20), QuestContent.Type);
        posY += 25;

        if (EditorDB.NPCStatDic.ContainsKey(QuestContent.ClearNPCHandle))
            EditorGUI.LabelField(new Rect(10 + rect.x, rect.y + posY, rect.width / 2 - 20, 20), "CNPC: " + EditorDB.NPCStatDic[QuestContent.NPCHandle].Name);
        else
            EditorGUI.LabelField(new Rect(10 + rect.x, rect.y + posY, rect.width / 2 - 20, 20), "CNPC: Not Found");
        QuestContent.ClearNPCHandle = EditorGUI.IntField(new Rect(rect.x + rect.width / 2 - 10, rect.y + posY, rect.width / 2, 20), QuestContent.ClearNPCHandle);
        posY += 25;

        switch(QuestContent.Type)
        {
            case EQuestClearType.Talk:
                break;
            case EQuestClearType.Collect:
                if (EditorDB.ItemDic.ContainsKey(QuestContent.ClearHandle))
                    EditorGUI.LabelField(new Rect(10 + rect.x, rect.y + posY, rect.width / 2 - 20, 20), "CItem: " + EditorDB.ItemDic[QuestContent.ClearHandle].Name);
                else
                    EditorGUI.LabelField(new Rect(10 + rect.x, rect.y + posY, rect.width / 2 - 20, 20), "CItem: Not Found");
                QuestContent.ClearHandle = EditorGUI.IntField(new Rect(rect.x + rect.width / 2 - 10, rect.y + posY, rect.width / 2, 20), QuestContent.ClearHandle);
                posY += 25;

                EditorGUI.LabelField(new Rect(10 + rect.x, rect.y + posY, rect.width / 2 - 20, 20), "CValue: ");
                QuestContent.ClearValue = EditorGUI.IntField(new Rect(rect.x + rect.width / 2 - 10, rect.y + posY, rect.width / 2, 20), QuestContent.ClearValue);
                posY += 25;
                break;
            case EQuestClearType.Hunt:
                if (EditorDB.MonsterStatDic.ContainsKey(QuestContent.ClearHandle))
                    EditorGUI.LabelField(new Rect(10 + rect.x, rect.y + posY, rect.width / 2 - 20, 20), "CEnermy: " + EditorDB.MonsterStatDic[QuestContent.ClearHandle].Name);
                else
                    EditorGUI.LabelField(new Rect(10 + rect.x, rect.y + posY, rect.width / 2 - 20, 20), "CEnermy: Not Found");
                QuestContent.ClearHandle = EditorGUI.IntField(new Rect(rect.x + rect.width / 2 - 10, rect.y + posY, rect.width / 2, 20), QuestContent.ClearHandle);
                posY += 25;

                EditorGUI.LabelField(new Rect(10 + rect.x, rect.y + posY, rect.width / 2 - 20, 20), "CValue: ");
                QuestContent.ClearValue = EditorGUI.IntField(new Rect(rect.x + rect.width / 2 - 10, rect.y + posY, rect.width / 2, 20), QuestContent.ClearValue);
                posY += 25;
                break;
            case EQuestClearType.Move:
                break;
        }

        EditorGUI.LabelField(new Rect(10 + rect.x, rect.y + posY, rect.width / 2 - 20, 20), "Reword");
        if (GUI.Button(new Rect(rect.x + rect.width - 30, rect.y + posY, 20, 20), "+"))
            QuestContent.Reword.Add(new SQuestReword());

        posY += 20;
        for (int i = 0; i < QuestContent.Reword.Count; ++i)
        {
            if (EditorDB.ItemDic.ContainsKey(QuestContent.Reword[i].Handle))
                EditorGUI.LabelField(new Rect(10 + rect.x, rect.y + posY, rect.width - 20, 20), "Item: " + EditorDB.ItemDic[QuestContent.Reword[i].Handle].Name);
            else
                EditorGUI.LabelField(new Rect(10 + rect.x, rect.y + posY, rect.width - 20, 20), "Not Found");

            posY += 20;

            QuestContent.Reword[i].Handle = EditorGUI.IntField(new Rect(10 + rect.x, rect.y + posY, (rect.width - 40)*0.5f, 20), QuestContent.Reword[i].Handle);
            QuestContent.Reword[i].Value = EditorGUI.IntField(new Rect(10 + rect.x+ (rect.width - 40) * 0.5f, rect.y + posY, (rect.width - 40) * 0.5f, 20), QuestContent.Reword[i].Value);

            if (GUI.Button(new Rect(10 + rect.x + rect.width - 40, rect.y + posY, 20, 20), "-"))
            {
                QuestContent.Reword.RemoveAt(i);
                return;
            }
            posY += 20;
        }
        EditorGUI.LabelField(new Rect(10 + rect.x, rect.y + posY, rect.width / 2 - 20, 20), "RewordGold");
        EditorGUI.LabelField(new Rect(rect.x + rect.width / 2 - 10, rect.y + posY, rect.width / 2 - 20, 20), "RewordEXP");
        posY += 20;
        QuestContent.RewordGold = EditorGUI.IntField(new Rect(10 + rect.x, rect.y + posY, (rect.width - 40) * 0.5f, 20), QuestContent.RewordGold);
        QuestContent.RewordEXP = EditorGUI.IntField(new Rect(rect.x + rect.width / 2 - 10, rect.y + posY, (rect.width - 40) * 0.5f, 20), QuestContent.RewordEXP);
        posY += 25;

        EditorGUI.LabelField(new Rect(10 + rect.x, rect.y + posY, rect.width / 2 - 20, 20), "NoneCScript: ");
        QuestContent.NoneClearScript = EditorGUI.TextField(new Rect(rect.x + rect.width / 2 - 10, rect.y + posY, rect.width / 2, 20), QuestContent.NoneClearScript);
        posY += 25;

        EditorGUI.LabelField(new Rect(10 + rect.x, rect.y + posY, rect.width / 2 - 20, 20), "ClearScript");
        if (GUI.Button(new Rect(rect.x + rect.width - 30, rect.y + posY, 20, 20), "+"))
            QuestContent.ClearScripts.Add("비어있는 스크립트");
        posY += 20;
        for (int i = 0; i < QuestContent.ClearScripts.Count; ++i)
        {
            QuestContent.ClearScripts[i] = EditorGUI.TextField(new Rect(10 + rect.x, rect.y + posY, rect.width - 40, 20), QuestContent.ClearScripts[i]);
            if (GUI.Button(new Rect(10 + rect.x + rect.width - 40, rect.y + posY, 20, 20), "-"))
            {
                QuestContent.ClearScripts.RemoveAt(i);
                return;
            }
            posY += 20;
        }
    }
}
