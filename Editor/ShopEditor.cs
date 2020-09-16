using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Text;
using SimpleJSON;
using System.Linq;
using System.IO;

public class ShopEditor : EditorWindow
{
    List<ShopEditor_Content> m_contentList;

    static EditorWindow m_window;
    ESkillPublicOption m_publicOptionType;
    ECoinShopType m_coinShopType = ECoinShopType.Coin;

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
        Input();

        if (GUI.changed) Repaint();
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
    void Input()
    {
        ECoinShopType CoinShopType = m_coinShopType;

        Rect InputRect = new Rect(0, 20, 300, position.height);
        GUILayout.BeginArea(InputRect, EditorStyles.helpBox);
        GUILayout.BeginVertical();

        for (int i = 0; i < 2; ++i)
        {
            if ((int)CoinShopType == i)
            {
                if (GUILayout.Button(new GUIContent(((ECoinShopType)i).ToString()), EditorStyles.helpBox, GUILayout.Height(20)))
                    CoinShopType = (ECoinShopType)i;
            }
            else
            {
                if (GUILayout.Button(new GUIContent(((ECoinShopType)i).ToString()), EditorStyles.toolbarButton, GUILayout.Height(20)))
                    CoinShopType = (ECoinShopType)i;
            }
        }

        GUILayout.EndHorizontal();
        GUILayout.EndArea();

        if (m_coinShopType != CoinShopType)
        {
            m_coinShopType = CoinShopType;
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
        CoinShopInfo Info = new CoinShopInfo();
        int handle = 0;
        while (true)
        {
            //foreach(CoinShopInfo info in EditorDB.ShopDic.Values)
            //{
            //    if(info.Handle == handle)
            //    {
            //        ++handle;
            //        continue;
            //    }
            //}

            //Info.Handle = handle;
            //Info.CoinShopType = m_coinShopType;
            //Info.CharacterAwakening = m_characterAwakening;
            EditorDB.ShopDic.Add(handle, Info);
            m_contentList.Add(new ShopEditor_Content(mousePosition, m_contentSize.x, m_contentSize.y, EditorStyles.helpBox, EditorStyles.textField, OnClickRemoveNode, Info));
            return;
        }
    }
    private void OnClickRemoveNode(ShopEditor_Content node)
    {
        //int handle = node.Skill.Handle;
        //EditorDB.ShopDic.Remove(handle);
        //m_contentList.Remove(node);
    }
    void Save()
    {
        string Path = "Assets/AssetBundle_Database/DB_Skill.json";
        FileStream File = new FileStream(Path, FileMode.Create);
        int i = 0;
        WriteFileToString(File, "{\r");
        foreach (KeyValuePair<int, CoinShopInfo> skill in EditorDB.ShopDic)
        {
            //WriteFileToString(File, "   \"" + i + "\":" + "{\r");
            //WriteFileToString(File, "      \"Handle\" : " + "\"" + skill.Value.Handle + "\",\r");
            //WriteFileToString(File, "      \"CoinShopType\" : " + "\"" + (int)skill.Value.CoinShopType + "\",\r");
            //WriteFileToString(File, "      \"CharacterAwakening\" : " + "\"" + skill.Value.CharacterAwakening + "\",\r");
            //WriteFileToString(File, "      \"Icon\" : " + "\"" + skill.Value.Icon + "\",\r");
            //WriteFileToString(File, "      \"Level\" : " + "\"" + skill.Value.Level + "\",\r");
            //WriteFileToString(File, "      \"SkillPoint\" : " + "\"" + skill.Value.SkillPoint + "\",\r");
            //WriteFileToString(File, "      \"Name\" : " + "\"" + skill.Value.Name + "\",\r");
            //if(skill.Value.Information != null)
            //    WriteFileToString(File, "      \"Information\" : " + "\"" + skill.Value.Information.Replace("\n", "//") + "\",\r");
            //else
            //    WriteFileToString(File, "      \"Information\" : " + "\"" + skill.Value.Information + "\",\r");
            //WriteFileToString(File, "      \"Explanation\" : " + "\"" + skill.Value.Explanation + "\",\r");
            //WriteFileToString(File, "      \"DurationTime\" : " + "\"" + skill.Value.DurationTime + "\",\r");
            //WriteFileToString(File, "      \"CompleteTime\" : " + "\"" + skill.Value.CompleteTime + "\",\r");
            //WriteFileToString(File, "      \"CoolTime\" : " + "\"" + skill.Value.CoolTime + "\",\r");
            //WriteFileToString(File, "      \"MP\" : " + "\"" + skill.Value.MP + "\",\r");
            //WriteFileToString(File, "      \"Type\" : " + "\"" + (int)skill.Value.Type + "\",\r");
            //WriteFileToString(File, "      \"LinkHandle\" : " + "\"" + skill.Value.LinkHandle + "\",\r");
            //WriteFileToString(File, "      \"LinkPossibleTime\" : " + "\"" + skill.Value.LinkPossibleTime + "\",\r");
            //WriteFileToString(File, "      \"KeydownTime\" : " + "\"" + skill.Value.KeydownTime + "\",\r");
            //WriteFileToString(File, "      \"ChannelingTime\" : " + "\"" + skill.Value.ChannelingTime + "\"\r");

            if (i < EditorDB.ShopDic.Count - 1)
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
        m_contentList = new List<ShopEditor_Content>();
        List<CoinShopInfo> infoList = new List<CoinShopInfo>();

        foreach(CoinShopInfo info in EditorDB.ShopDic.Values)
        {
            //if (info.CoinShopType == m_coinShopType && info.CharacterAwakening == m_characterAwakening)
            //    infoList.Add(info);
        }
       // infoList.Sort((x,y) => MathLib.IComparerCoinShopInfo(x, y));

        float posX = 300;
        float posY = 20;

        for(int i =0; i< infoList.Count; ++i)
        {
            ShopEditor_Content content = new ShopEditor_Content(new Vector2(posX, posY), m_contentSize.x, m_contentSize.y, EditorStyles.helpBox, EditorStyles.textField, OnClickRemoveNode, infoList[i]);
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

