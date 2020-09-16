using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Text;
using SimpleJSON;
using System.Linq;
using System.IO;

public class QuestEditor : EditorWindow
{
    List<QuestEditor_Content> m_contentList;

    static EditorWindow m_window;
    EQuestType m_publicOptionType;
    int m_selectHandle = 0;

    GUIStyle m_defaultStyle;
    GUIStyle m_selectStyle;

    Rect m_informationRect;
    Vector2 offset = Vector2.zero;
    Vector2 drag;
    Vector2 m_contentSize;

    bool m_useDrag = true;

    [MenuItem("CustomEditor/QuestEditor")]
    static void ShowWindow()
    {
        m_window = GetWindow<QuestEditor>();
        m_window.titleContent = new GUIContent("Quest Editor");
        EditorDB.Init();
    }
    void OnEnable()
    {
        m_defaultStyle = new GUIStyle();
        m_defaultStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node1.png") as Texture2D;
        m_defaultStyle.border = new RectOffset(120, 0, 15, 15);

        m_selectStyle = new GUIStyle();
        m_selectStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node1 on.png") as Texture2D;
        m_selectStyle.border = new RectOffset(120, 0, 15, 15);

        m_contentSize = new Vector2(300, 400);
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
        EQuestType type = m_publicOptionType;
        int prevHandle = m_selectHandle;
        Rect InputRect = new Rect(0, 20, 300, position.height);
        GUILayout.BeginArea(InputRect, EditorStyles.helpBox);
        GUILayout.BeginVertical();

        for (int i =0; i<5; ++i)
        {
            GUILayout.Space(10);

            if((int)type == i)
            {
                if (GUILayout.Button(new GUIContent(((EQuestType)i).ToString()), EditorStyles.toolbarPopup, GUILayout.Height(20)))
                {
                    type = (EQuestType)0;
                    m_selectHandle = 0;
                }

                var QuestList = from quest in EditorDB.QuestDic
                                where quest.Value.Type == m_publicOptionType
                                select quest.Value;

                foreach(Quest val in QuestList)
                {
                    GUILayout.Space(5);

                    if (m_selectHandle == val.Handle)
                    {
                        if (GUILayout.Button(new GUIContent("                " + val.Name), m_selectStyle, GUILayout.Width(270), GUILayout.Height(20)))
                            m_selectHandle = val.Handle;
                    }
                    else
                    {
                        if (GUILayout.Button(new GUIContent("                " + val.Name), m_defaultStyle, GUILayout.Width(270), GUILayout.Height(20)))
                            m_selectHandle = val.Handle;
                    }
                }
            }
            else
            {
                if (GUILayout.Button(new GUIContent(((EQuestType)i).ToString()), EditorStyles.helpBox, GUILayout.Height(20)))
                {
                    type = (EQuestType)i;
                    m_selectHandle = 0;
                }
            }
        }

        GUILayout.EndVertical();
        GUILayout.EndArea();

        if (m_publicOptionType != type || m_selectHandle != prevHandle)
        {
            m_publicOptionType = type;
            LoadContent();
        }
    }
    void DrawInformationWindow()
    {
        m_informationRect = new Rect(300, 0, position.width - 300, position.height);
        GUILayout.BeginArea(m_informationRect, EditorStyles.toolbar);
        GUILayout.BeginHorizontal();
        if(GUILayout.Button(new GUIContent("Save"), EditorStyles.toolbarButton, GUILayout.Width(40)))
            Save();
        m_useDrag = GUILayout.Toggle(m_useDrag, new GUIContent("Drag"), EditorStyles.toolbarButton, GUILayout.Width(40));
        if (GUILayout.Button(new GUIContent("AddChapter"), EditorStyles.toolbarButton, GUILayout.Width(80)))
            AddContent();
        GUILayout.EndHorizontal();
        GUILayout.EndArea();

        if (m_selectHandle != 0)
        {
            m_informationRect.y = 20;
            GUILayout.BeginArea(m_informationRect, EditorStyles.helpBox);
            GUILayout.BeginHorizontal();

            GUILayout.Label("Handle: " + m_selectHandle, GUILayout.Width(80));
            GUILayout.Space(10);
            EditorDB.QuestDic[m_selectHandle].Name = EditorGUILayout.TextField(EditorDB.QuestDic[m_selectHandle].Name, GUILayout.Width(300));

            GUILayout.EndHorizontal();
            GUILayout.EndArea();
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
                    if (e.mousePosition.x >= 300)
                        break;
                    if(m_publicOptionType != EQuestType.None)
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
    void AddContent()
    {
        if (m_selectHandle == 0)
            return;

        int j = (int)((EditorDB.QuestDic[m_selectHandle].QuestDic.Values.Count * m_contentSize.x) / (position.width - m_contentSize.x));
        float posX = 300 + ((EditorDB.QuestDic[m_selectHandle].QuestDic.Values.Count * m_contentSize.x) % (position.width - m_contentSize.x));
        float posY = 100 + m_contentSize.y * j;

        QuestContent content = new QuestContent();
        content.ContentName = "챕터 이름을 입력해주세요.";
        content.Handle = EditorDB.QuestDic[m_selectHandle].QuestDic.Count;

        EditorDB.QuestDic[m_selectHandle].QuestDic.Add(EditorDB.QuestDic[m_selectHandle].QuestDic.Values.Count, content);
        m_contentList.Add(new QuestEditor_Content(new Vector2(posX, posY), m_contentSize.x, m_contentSize.y, EditorStyles.helpBox, EditorStyles.textField, OnClickRemoveNode, content));
    }
    void OnClickAddNode(Vector2 mousePosition)
    {
        Quest quest = new Quest();
        int handle = 1;
        while (true)
        {
            Start:

            foreach(Quest info in EditorDB.QuestDic.Values)
            {
                if(info.Handle == handle)
                {
                    ++handle;
                    goto Start;
                }
            }

            quest.Handle = handle;
            quest.Type = m_publicOptionType;
            quest.Name = "퀘스트 이름을 입력해주세요.";
            EditorDB.QuestDic.Add(handle, quest);
            return;
        }
    }
    private void OnClickRemoveNode(QuestEditor_Content node)
    {
        int handle = node.QuestContent.Handle;
        EditorDB.QuestDic[m_selectHandle].QuestDic.Remove(handle);
        m_contentList.Remove(node);
    }
    void Save()
    {
        string Path = "Assets/AssetBundle_Database/DB_Quest.json";
        FileStream File = new FileStream(Path, FileMode.Create);
        int i = 0;
        WriteFileToString(File, "{\r");
        foreach (KeyValuePair<int, Quest> quest in EditorDB.QuestDic)
        {
            WriteFileToString(File, "   \"" + i + "\":" + "{\r");
            WriteFileToString(File, "      \"Handle\" : " + "\"" + quest.Value.Handle + "\",\r");
            WriteFileToString(File, "      \"Name\" : " + "\"" + quest.Value.Name + "\",\r");

            foreach (KeyValuePair<int, QuestContent> content in quest.Value.QuestDic)
            {
                List<string> Value = new List<string>();
                Value.Add(content.Value.Handle.ToString());
                Value.Add(content.Value.NPCHandle.ToString());
                Value.Add(content.Value.Level.ToString());
                Value.Add(content.Value.ContentName);
                Value.Add(string.Join("/", content.Value.Scripts));
                Value.Add(((int)content.Value.Type).ToString());
                Value.Add(content.Value.ClearNPCHandle.ToString());
                Value.Add(content.Value.ClearHandle.ToString());
                Value.Add(content.Value.ClearValue.ToString());
                string[] val = new string[content.Value.Reword.Count];
                for(int j= 0; j<val.Length; ++j)
                    val[j] = content.Value.Reword[j].Handle + "-" + content.Value.Reword[j].Value;
                Value.Add(string.Join("/", val));
                Value.Add(content.Value.NoneClearScript);
                Value.Add(string.Join("/", content.Value.ClearScripts));
                Value.Add(content.Value.RewordGold.ToString());
                Value.Add(content.Value.RewordEXP.ToString());
                WriteFileToString(File, "      \"Chapter"+ content.Value.Handle + "\" : " + "\"" + string.Join(",", Value) + "\",\r");
            }

            WriteFileToString(File, "      \"Type\" : " + "\"" + (int)quest.Value.Type + "\"\r");
            if (i < EditorDB.QuestDic.Count - 1)
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
        m_contentList = new List<QuestEditor_Content>();

        float posX = 300;
        float posY = 100;

        if (m_selectHandle == 0)
            return;

        foreach (KeyValuePair<int,QuestContent> val in EditorDB.QuestDic[m_selectHandle].QuestDic)
        {
            QuestEditor_Content content = new QuestEditor_Content(new Vector2(posX, posY), m_contentSize.x, m_contentSize.y, EditorStyles.helpBox, EditorStyles.textField, OnClickRemoveNode, val.Value);
            m_contentList.Add(content);
            posX += m_contentSize.x;
            if(posX > position.width)
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

