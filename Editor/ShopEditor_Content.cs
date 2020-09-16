using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public class ShopEditor_Content
{
    public Texture2D Texture;
    public Rect Rect;
    public CoinShopInfo Skill;
    public GUIStyle GuiStyle;
    public GUIStyle DefaultStyle;
    public GUIStyle SelectStyle;
    public bool IsDragged;
    public bool IsSelected;
    public Action<ShopEditor_Content> OnRemoveNode;
    public ShopEditor_Content(Vector2 pos, float width, float height, GUIStyle defaultStyle, GUIStyle selectStyle, Action<ShopEditor_Content> onRemoveNode, CoinShopInfo skill)
    {
        Skill = skill;
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
        //Rect rect = Rect;
        //if (Rect.x < 300)
        //{
        //    rect.x = 300;
        //    rect.width = Mathf.Clamp(Rect.width - (300 - Rect.x), 20, 300);
        //}
        //GUI.Box(rect, Skill.Name, GuiStyle);
        //if (Rect.x < 320 - Rect.width)
        //    return;

        //if(Texture == null)
        //    Texture = Resources.Load<Texture2D>(Skill.Icon);

        //GUIStyle style = EditorStyles.helpBox;
        //style.richText = true;
        //style.fontSize = 12;
        //float posY = 20;
        //Texture = (Texture2D)EditorGUI.ObjectField(new Rect(rect.x +rect.width-90, rect.y + posY, 80, 80), Texture, typeof(Texture2D), false);
        //if (Texture != null)
        //{
        //    Skill.Icon = "Icon/Skill/" + Texture.name;
        //    EditorGUI.LabelField(new Rect(10 + rect.x, rect.y + posY + 20, rect.width - 100, 20), Skill.Icon, style);
        //}
        //EditorGUI.LabelField(new Rect(10 + rect.x, rect.y + posY + 40, rect.width - 170, 20), "Level: ");
        //Skill.Level = EditorGUI.IntField(new Rect(80 + rect.x, rect.y + posY + 40, rect.width - 170, 20), Skill.Level);
        //EditorGUI.LabelField(new Rect(10 + rect.x, rect.y + posY + 60, rect.width - 170, 20), "SkillPoint: ");
        //Skill.SkillPoint = EditorGUI.IntField(new Rect(80 + rect.x, rect.y + posY + 60, rect.width - 170, 20), Skill.SkillPoint);
        //EditorGUI.LabelField(new Rect(10+rect.x, rect.y + posY, rect.width - 100, 20), "<b>Handle: " + Skill.Handle.ToString()+ "</b>", style);
        //posY += 85;
        //GUI.Label(new Rect(rect.x + 10, rect.y + posY, rect.width * 0.33f, 20), "Name");
        //Skill.Name = EditorGUI.TextField(new Rect(rect.x + rect.width * 0.33f, rect.y + posY, rect.width * 0.66f - 10, 20), Skill.Name);
        //posY += 25;
        //GUI.Label(new Rect(rect.x + 10, rect.y + posY, rect.width * 0.33f, 20), "D/C Time");
        //Skill.DurationTime = EditorGUI.FloatField(new Rect(rect.x + rect.width * 0.33f, rect.y + posY, rect.width * 0.33f - 10, 20), Skill.DurationTime);
        //GUI.Label(new Rect(rect.x + rect.width * 0.66f-10, rect.y + posY, 10, 20), "-");
        //Skill.CompleteTime = EditorGUI.FloatField(new Rect(rect.x + rect.width * 0.66f, rect.y + posY, rect.width * 0.33f - 10, 20), Skill.CompleteTime);
        //posY += 25;
        //GUI.Label(new Rect(rect.x + 10, rect.y + posY, rect.width * 0.33f, 20), "CoolTime");
        //Skill.CoolTime = EditorGUI.FloatField(new Rect(rect.x + rect.width * 0.33f, rect.y + posY, rect.width * 0.66f - 10, 20), Skill.CoolTime);
        //posY += 25;
        //GUI.Label(new Rect(rect.x + 10, rect.y + posY, rect.width * 0.33f, 20), "MP");
        //Skill.MP = EditorGUI.FloatField(new Rect(rect.x + rect.width * 0.33f, rect.y + posY, rect.width * 0.66f - 10, 20), Skill.MP);
        //posY += 25;
        //GUI.Label(new Rect(rect.x + 10, rect.y + posY, rect.width * 0.33f, 20), "Type");
        //Skill.Type = (ESkillType)EditorGUI.EnumFlagsField(new Rect(rect.x + rect.width * 0.33f, rect.y + posY, rect.width * 0.66f - 10, 20), Skill.Type);
        //posY += 25;
        //if((Skill.Type & ESkillType.Keydown) != 0)
        //{
        //    GUI.Label(new Rect(rect.x + 10, rect.y + posY, rect.width * 0.33f, 20), "KeydownTime");
        //    Skill.KeydownTime = EditorGUI.FloatField(new Rect(rect.x + rect.width * 0.33f, rect.y + posY, rect.width * 0.66f - 10, 20), Skill.KeydownTime);
        //    posY += 25;
        //}
        //if ((Skill.Type & ESkillType.Link) != 0)
        //{
        //    GUI.Label(new Rect(rect.x + 10, rect.y + posY, rect.width * 0.33f, 20), "LinkHandle");
        //    Skill.LinkHandle = EditorGUI.IntField(new Rect(rect.x + rect.width * 0.33f, rect.y + posY, rect.width * 0.66f - 10, 20), Skill.LinkHandle);
        //    posY += 25;
        //}
        //if ((Skill.Type & ESkillType.Link) != 0)
        //{
        //    GUI.Label(new Rect(rect.x + 10, rect.y + posY, rect.width * 0.33f, 20), "LinkPossibleTime");
        //    Skill.LinkPossibleTime = EditorGUI.FloatField(new Rect(rect.x + rect.width * 0.33f, rect.y + posY, rect.width * 0.66f - 10, 20), Skill.LinkPossibleTime);
        //    posY += 25;
        //}
        //if ((Skill.Type & ESkillType.Channeling) != 0)
        //{
        //    GUI.Label(new Rect(rect.x + 10, rect.y + posY, rect.width * 0.33f, 20), "ChannelingTime");
        //    Skill.ChannelingTime = EditorGUI.FloatField(new Rect(rect.x + rect.width * 0.33f, rect.y + posY, rect.width * 0.66f - 10, 20), Skill.ChannelingTime);
        //    posY += 25;
        //}
        //GUI.Label(new Rect(rect.x + 10, rect.y + posY, rect.width * 0.33f, 20), "Information");
        //Skill.Information = EditorGUI.TextField(new Rect(rect.x + rect.width * 0.33f, rect.y + posY, rect.width * 0.66f - 10, 40), Skill.Information);
        //posY += 45;
        //GUI.Label(new Rect(rect.x + 10, rect.y + posY, rect.width * 0.33f, 20), "Explanation");
        //Skill.Explanation = EditorGUI.TextField(new Rect(rect.x + rect.width * 0.33f, rect.y + posY, rect.width * 0.66f - 10, 40), Skill.Explanation);
        //posY += 45;

    }
}
