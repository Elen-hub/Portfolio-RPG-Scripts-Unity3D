using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Text;
using SimpleJSON;
using System.Linq;
using System.IO;

public enum ESkillPublicType
{
    Active,
    Passive,
    Phase,
    Collect,
}
public class SkillEditor : EditorWindow
{
    List<SkillEditor_Content> m_contentList;

    static EditorWindow m_window;
    string[] m_toolbarOption = { "Public", "Private" };
    ESkillPublicOption m_publicOptionType;
    ECharacterClass m_characterClass = ECharacterClass.Default;
    int m_characterAwakening = -1;

    GUIStyle m_defaultStyle;
    GUIStyle m_selectStyle;

    Rect m_informationRect;
    Vector2 offset = Vector2.zero;
    Vector2 drag;
    Vector2 m_contentSize;

    bool m_useDrag = true;

    [MenuItem("CustomEditor/SkillEditor")]
    static void ShowWindow()
    {
        m_window = GetWindow<SkillEditor>();
        m_window.titleContent = new GUIContent("Skill Editor");
        EditorDB.Init();
    }
    void OnEnable()
    {
        LoadContent();
        m_defaultStyle = new GUIStyle();
        m_defaultStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node1.png") as Texture2D;
        m_defaultStyle.border = new RectOffset(12, 12, 12, 12);

        m_selectStyle = new GUIStyle();
        m_selectStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node1 on.png") as Texture2D;
        m_selectStyle.border = new RectOffset(12, 12, 12, 12);

        m_contentSize = new Vector2(250, 400);
    }
    void OnGUI()
    {
        DrawGrid(20, 0.2f, Color.gray);
        DrawGrid(100, 0.4f, Color.gray);
        DrawInformationWindow();
        DrawNodes();
        ProcessNodeEvents(Event.current);
        ProcessEvents(Event.current);
        SelectType();

        if (GUI.changed) Repaint();
    }
    void SelectType()
    {
        m_publicOptionType = (ESkillPublicOption)GUILayout.Toolbar((int)m_publicOptionType, m_toolbarOption, GUILayout.Height(20), GUILayout.Width(290));

        switch (m_publicOptionType)
        {
            case ESkillPublicOption.Public:
                PublicInput();
                break;
            case ESkillPublicOption.Private:
                PrivateInput();
                break;
        }
    }
    void DrawInformationWindow()
    {
        m_informationRect = new Rect(300, 0, position.width - 300, position.height);
        GUILayout.BeginArea(m_informationRect, EditorStyles.toolbar);
        GUILayout.BeginHorizontal();
        if(GUILayout.Button(new GUIContent("Save"), EditorStyles.toolbarButton, GUILayout.Width(35)))
            Save();
        GUILayout.Space(5);
        m_useDrag = GUILayout.Toggle(m_useDrag, new GUIContent("Drag"), EditorStyles.toolbarButton, GUILayout.Width(35));
        GUILayout.EndHorizontal();
        GUILayout.EndArea();
    }
    void PublicInput()
    {
        int PublicType = m_characterAwakening;
        Rect InputRect = new Rect(0, 20, 300, position.height);
        GUILayout.BeginArea(InputRect, EditorStyles.helpBox);
        GUILayout.BeginVertical();

        for(int i =0; i<4; ++i)
        {
            if (PublicType == i)
            {
                if (GUILayout.Button(new GUIContent(((ESkillPublicType)i).ToString()), EditorStyles.helpBox, GUILayout.Height(20)))
                    PublicType = i;
            }
            else
            {
                if (GUILayout.Button(new GUIContent(((ESkillPublicType)i).ToString()), EditorStyles.toolbarButton, GUILayout.Height(20)))
                    PublicType = i;
            }
        }

        GUILayout.EndHorizontal();
        GUILayout.EndArea();

        if (m_characterClass != ECharacterClass.Default || PublicType != m_characterAwakening)
        {
            m_characterClass = 0;
            m_characterAwakening = PublicType;
            LoadContent();
        }
    }
    void PrivateInput()
    {
        if (m_characterClass == 0)
        {
            m_characterClass = (ECharacterClass)1;
            m_characterAwakening = 0;
            LoadContent();
            return;
        }

        ECharacterClass CharacterClass = m_characterClass;
        int CharacterPhase = m_characterAwakening;

        Rect InputRect = new Rect(0, 20, 300, position.height);
        GUILayout.BeginArea(InputRect, EditorStyles.helpBox);
        GUILayout.BeginVertical();

        for (int i = 1; i < 7; ++i)
        {
            if ((int)CharacterClass == i)
            {
                if (GUILayout.Button(new GUIContent(((ECharacterClass)i).ToString()), EditorStyles.helpBox, GUILayout.Height(20)))
                    CharacterClass = (ECharacterClass)i;
            }
            else
            {
                if (GUILayout.Button(new GUIContent(((ECharacterClass)i).ToString()), EditorStyles.toolbarButton, GUILayout.Height(20)))
                    CharacterClass = (ECharacterClass)i;
            }
        }

        GUILayout.EndHorizontal();
        GUILayout.EndArea();

        if (m_characterClass != CharacterClass || m_characterAwakening != CharacterPhase)
        {
            m_characterClass = CharacterClass;
            m_characterAwakening = CharacterPhase;
            LoadContent();
        }
    }
    void DrawNodes()
    {
        if (m_contentList != null)
            for (int i = 0; i < m_contentList.Count; ++i)
                m_contentList[i].Draw();
    }
    void ProcessEvents(Event e)
    {
        drag = Vector2.zero;
        switch (e.type)
        {
            case EventType.MouseDown:
                if (e.button == 1)
                {
                    if (e.mousePosition.x < 300)
                        break;
                    ProcessContextMenu(e.mousePosition);
                }
                break;
            case EventType.MouseDrag:
                if (e.button == 0 && m_useDrag)
                    OnDrag(e.delta);
                break;
        }
    }
    private void DrawGrid(float gridSpacing, float gridOpacity, Color gridColor)
    {
        int widthDivs = Mathf.CeilToInt(position.width / gridSpacing);
        int heightDivs = Mathf.CeilToInt(position.height / gridSpacing);

        Handles.BeginGUI();
        Handles.color = new Color(gridColor.r, gridColor.g, gridColor.b, gridOpacity);

        drag = Vector2.zero;
        offset = Vector2.zero;

        offset += drag * 0.5f;
        Vector3 newOffset = new Vector3(offset.x % gridSpacing, offset.y % gridSpacing, 0);

        for (int i = 0; i < widthDivs; i++)
        {
            if (gridSpacing * i < 300)
                continue;

            Handles.DrawLine(new Vector3(gridSpacing * i, -gridSpacing, 0) + newOffset, new Vector3(gridSpacing * i, position.height, 0f) + newOffset);
        }
        for (int j = 0; j < heightDivs; j++)
        {
            Handles.DrawLine(new Vector3(300, gridSpacing * j, 0) + newOffset, new Vector3(position.width, gridSpacing * j, 0f) + newOffset);
        }
        Handles.color = Color.white;
        Handles.EndGUI();
    }
    private void ProcessNodeEvents(Event e)
    {
        if (m_contentList != null)
        {
            for (int i = m_contentList.Count - 1; i >= 0; i--)
            {
                bool guiChanged = m_contentList[i].Events(e);

                if (guiChanged)
                    GUI.changed = true;
            }
        }
    }
    void ProcessContextMenu(Vector2 mousePosition)
    {
        GenericMenu genericMenu = new GenericMenu();
        genericMenu.AddItem(new GUIContent("Add node"), false, () => OnClickAddNode(mousePosition));
        genericMenu.ShowAsContext();
    }
    private void OnDrag(Vector2 delta)
    {
        drag = delta;

        if (m_contentList != null)
            for (int i = 0; i < m_contentList.Count; i++)
                m_contentList[i].Drag(delta);

        GUI.changed = true;
    }
    void OnClickAddNode(Vector2 mousePosition)
    {
        SkillInfo Info = new SkillInfo();
        int handle = 0;
        while (true)
        {
            Start:

            foreach(SkillInfo info in EditorDB.SkillList.Values)
            {
                if(info.Handle == handle)
                {
                    ++handle;
                    goto Start;
                }
            }

            Info.Handle = handle;
            Info.CharacterClass = m_characterClass;
            Info.CharacterAwakening = m_characterAwakening;
            EditorDB.SkillList.Add(handle, Info);
            m_contentList.Add(new SkillEditor_Content(mousePosition, m_contentSize.x, m_contentSize.y, EditorStyles.helpBox, EditorStyles.textField, OnClickRemoveNode, Info));
            return;
        }
    }
    private void OnClickRemoveNode(SkillEditor_Content node)
    {
        int handle = node.Skill.Handle;
        EditorDB.SkillList.Remove(handle);
        m_contentList.Remove(node);
    }
    void Save()
    {
        string Path = "Assets/AssetBundle_Database/DB_Skill.json";
        FileStream File = new FileStream(Path, FileMode.Create);
        int i = 0;
        WriteFileToString(File, "{\r");
        foreach (KeyValuePair<int, SkillInfo> skill in EditorDB.SkillList)
        {
            WriteFileToString(File, "   \"" + i + "\":" + "{\r");
            WriteFileToString(File, "      \"Handle\" : " + "\"" + skill.Value.Handle + "\",\r");
            WriteFileToString(File, "      \"CharacterClass\" : " + "\"" + (int)skill.Value.CharacterClass + "\",\r");
            WriteFileToString(File, "      \"CharacterAwakening\" : " + "\"" + skill.Value.CharacterAwakening + "\",\r");
            WriteFileToString(File, "      \"Icon\" : " + "\"" + skill.Value.Icon + "\",\r");
            WriteFileToString(File, "      \"Level\" : " + "\"" + skill.Value.Level + "\",\r");
            WriteFileToString(File, "      \"SkillPoint\" : " + "\"" + skill.Value.SkillPoint + "\",\r");
            WriteFileToString(File, "      \"Name\" : " + "\"" + skill.Value.Name + "\",\r");
            if(skill.Value.Information != null)
                WriteFileToString(File, "      \"Information\" : " + "\"" + skill.Value.Information.Replace("\n", "//") + "\",\r");
            else
                WriteFileToString(File, "      \"Information\" : " + "\"" + skill.Value.Information + "\",\r");
            WriteFileToString(File, "      \"Explanation\" : " + "\"" + skill.Value.Explanation + "\",\r");
            WriteFileToString(File, "      \"DurationTime\" : " + "\"" + skill.Value.DurationTime + "\",\r");
            WriteFileToString(File, "      \"CompleteTime\" : " + "\"" + skill.Value.CompleteTime + "\",\r");
            WriteFileToString(File, "      \"CoolTime\" : " + "\"" + skill.Value.CoolTime + "\",\r");
            WriteFileToString(File, "      \"MP\" : " + "\"" + skill.Value.MP + "\",\r");
            WriteFileToString(File, "      \"Range\" : " + "\"" + skill.Value.Range + "\",\r");
            WriteFileToString(File, "      \"AutoAIM\" : " + "\"" + skill.Value.IsAutoAim + "\",\r");
            WriteFileToString(File, "      \"Type\" : " + "\"" + (int)skill.Value.Type + "\",\r");
            WriteFileToString(File, "      \"LinkHandle\" : " + "\"" + skill.Value.LinkHandle + "\",\r");
            WriteFileToString(File, "      \"LinkPossibleTime\" : " + "\"" + skill.Value.LinkPossibleTime + "\",\r");
            WriteFileToString(File, "      \"KeydownTime\" : " + "\"" + skill.Value.KeydownTime + "\",\r");
            WriteFileToString(File, "      \"ChannelingTime\" : " + "\"" + skill.Value.ChannelingTime + "\"\r");

            if (i < EditorDB.SkillList.Count - 1)
                WriteFileToString(File, "   },\r\r");
            else
                WriteFileToString(File, "   }\r");
            ++i;
        }
        WriteFileToString(File, "}");
        File.Close();
        UnityEditor.AssetDatabase.Refresh();
    }
    void LoadContent()
    {
        m_contentList = new List<SkillEditor_Content>();
        List<SkillInfo> infoList = new List<SkillInfo>();

        foreach(SkillInfo info in EditorDB.SkillList.Values)
        {
            if (info.CharacterClass == m_characterClass && info.CharacterAwakening == m_characterAwakening)
                infoList.Add(info);
        }
        infoList.Sort((x,y) => MathLib.IComparerSkillInfo(x, y));

        float posX = 300;
        float posY = 20;

        for(int i =0; i< infoList.Count; ++i)
        {
            SkillEditor_Content content = new SkillEditor_Content(new Vector2(posX, posY), m_contentSize.x, m_contentSize.y, EditorStyles.helpBox, EditorStyles.textField, OnClickRemoveNode, infoList[i]);
            m_contentList.Add(content);
            posX += m_contentSize.x;
            if (posX > position.width)
            {
                posX = 300;
                posY += m_contentSize.y;
            }
        }
    }
    void WriteFileToString(FileStream file, string str)
    {
        byte[] bytes = System.Text.Encoding.UTF8.GetBytes(str);
        file.Write(bytes, 0, bytes.Length);
    }
}

