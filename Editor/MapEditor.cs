using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Text;
using SimpleJSON;
using System.Linq;
using System.IO;

public class MapEditor : EditorWindow
{
    static EditorWindow m_window;
    EMapType m_mapArea;

    GUIStyle m_defaultStyle;
    GUIStyle m_selectStyle;

    Rect m_informationRect;
    Vector2 offset = Vector2.zero;
    Vector2 drag;
    int SelectHandle;

    Texture2D MonsterIcon;
    Texture2D NPCIcon;
    Texture2D WayPointIcon;
    Texture2D PortalIcon;
    Texture2D PortalDeltaIcon;
    Texture2D MatchPortalIcon;

    Camera m_captureCam;

    [MenuItem("CustomEditor/MapEditor")]
    static void ShowWindow()
    {
        m_window = GetWindow<MapEditor>();
        m_window.titleContent = new GUIContent("Map Editor");
        EditorDB.Init();
    }
    void OnEnable()
    {
        MonsterIcon = new Texture2D(50, 50);
        NPCIcon = new Texture2D(50, 50);
        PortalIcon = new Texture2D(50, 50);
        WayPointIcon = new Texture2D(50, 50);
        PortalDeltaIcon = new Texture2D(50, 50);
        MatchPortalIcon = new Texture2D(50, 50);
        NPCIcon.LoadImage(File.ReadAllBytes("Assets/Assets_UI/GUIPack-Clean&Minimalist/Sprites/Buttons/Circles/Filled/Yellow-Circle.png"));
        MonsterIcon.LoadImage(File.ReadAllBytes("Assets/Assets_UI/GUIPack-Clean&Minimalist/Sprites/Buttons/Circles/Filled/Red-Circle.png"));
        PortalIcon.LoadImage(File.ReadAllBytes("Assets/Assets_UI/GUIPack-Clean&Minimalist/Sprites/Buttons/Circles/Filled/Blue-Circle.png"));
        WayPointIcon.LoadImage(File.ReadAllBytes("Assets/Assets_UI/GUIPack-Clean&Minimalist/Sprites/Buttons/Circles/Filled/Green-Circle.png"));
        MatchPortalIcon.LoadImage(File.ReadAllBytes("Assets/Assets_UI/GUIPack-Clean&Minimalist/Sprites/Buttons/Circles/Filled/PinkDark-Circle.png"));
        PortalDeltaIcon.LoadImage(File.ReadAllBytes("Assets/Assets_UI/GUIPack-Clean&Minimalist/Sprites/Buttons/Circles/Filled/BlueLight-Circle.png"));

        m_defaultStyle = new GUIStyle();
        m_defaultStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node1.png") as Texture2D;
        m_defaultStyle.border = new RectOffset(120, 0, 15, 15);

        m_selectStyle = new GUIStyle();
        m_selectStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node1 on.png") as Texture2D;
        m_selectStyle.border = new RectOffset(120, 0, 15, 15);
    }
    void OnGUI()
    {
        DrawGrid(20, 0.2f, Color.grey);
        DrawGrid(100, 0.4f, Color.grey);
        DrawInformationWindow();
        ProcessEvents(Event.current);
        SelectArea();
        InputArea();

        if (GUI.changed) Repaint();
    }
    void DrawInformationWindow()
    {
        m_informationRect = new Rect(300, 0, position.width - 300, position.height);
        GUILayout.BeginArea(m_informationRect, EditorStyles.toolbar);
        GUILayout.BeginHorizontal();
        if (GUILayout.Button(new GUIContent("Save"), EditorStyles.toolbarButton, GUILayout.Width(80)))
            Save();
        GUILayout.Space(5);
        if (GUILayout.Button(new GUIContent("Remove"), EditorStyles.toolbarButton, GUILayout.Width(80)))
            Remove();
        GUILayout.Space(5);
        if (GUILayout.Button(new GUIContent("WayPortal"), EditorStyles.toolbarButton, GUILayout.Width(80)))
            AddPortal();
        GUILayout.Space(5);
        if (GUILayout.Button(new GUIContent("MatchPortal"), EditorStyles.toolbarButton, GUILayout.Width(80)))
            AddMatchPortal();
        GUILayout.Space(5);
        if(EditorDB.MapList.ContainsKey(0))
            if(EditorDB.MapList[SelectHandle].Size != Vector2.zero)
                if (GUILayout.Button(new GUIContent("ScreenCapture"), EditorStyles.toolbarButton, GUILayout.Width(100)))
                    InstanceMapCaptureCam(EditorDB.MapList[SelectHandle].Size.x, EditorDB.MapList[SelectHandle].Size.y);

        GUILayout.EndHorizontal();
        GUILayout.EndArea();
    }
    void SelectArea()
    {
        EMapType AreaType = m_mapArea;
        Rect InputRect = new Rect(0, 20, 300, position.height);
        GUILayout.BeginArea(InputRect, EditorStyles.helpBox);
        GUILayout.BeginVertical();

        for (int i = 0; i < 3; ++i)
        {
            GUILayout.Space(10);

            if ((int)AreaType == i)
            {
                if (GUILayout.Button(new GUIContent(((EMapType)i).ToString()), EditorStyles.toolbarPopup, GUILayout.Height(20)))
                    AreaType = (EMapType)0;

                var MapList = from map in EditorDB.MapList
                              where map.Value.Type == m_mapArea
                              select map.Value;


                foreach (Map val in MapList)
                {
                    GUILayout.Space(5);

                    if (SelectHandle == val.Handle)
                    {
                        if (GUILayout.Button(new GUIContent("                " + val.MapName), m_selectStyle, GUILayout.Width(270), GUILayout.Height(20)))
                            SelectHandle = val.Handle;
                    }
                    else
                    {
                        if (GUILayout.Button(new GUIContent("                " + val.MapName), m_defaultStyle, GUILayout.Width(270), GUILayout.Height(20)))
                            SelectHandle = val.Handle;
                    }
                }
            }
            else
            {
                if (GUILayout.Button(new GUIContent(((EMapType)i).ToString()), EditorStyles.helpBox, GUILayout.Height(20)))
                    AreaType = (EMapType)i;
            }
        }

        GUILayout.EndVertical();
        GUILayout.EndArea();

        if (AreaType != m_mapArea)
            m_mapArea = AreaType;
    }
    void InputArea()
    {
        if (!EditorDB.MapList.ContainsKey(SelectHandle) || m_mapArea == EMapType.None)
            return;

        InputMap();
        InputStatus();
        InputMonster();
        // InputNPC();
        InputPortal();
        InputMatchPortal();
    }
    void InputMap()
    {
        Texture2D texture = Resources.Load<Texture2D>(EditorDB.MapList[SelectHandle].MapImgPath);
        if (texture == null)
            return;

        float x, y, w, h;
        x = 320;
        y = 40;
        w = texture.width;
        h = texture.height;

        if (w > h)
        {
            float cons = 512 / w;
            w *= cons;
            h *= cons;
        }
        else
        {
            float cons = 512 / h;
            w *= cons;
            h *= cons;
        }
        x += (512 - w) * 0.5f;
        y += (512 - h) * 0.5f;
        Rect rect = new Rect(x, y, w, h);
        GUI.DrawTexture(rect, texture);
    }
    void InputStatus()
    {
        float posY = 0;
        Rect rect = new Rect(860, 40, 250, 380);
        GUI.Box(rect, EditorDB.MapList[SelectHandle].MapName, EditorStyles.helpBox);
        posY += 20;
        EditorGUI.LabelField(new Rect(rect.x, rect.y + posY, rect.width / 2, 20), "Handle: " + EditorDB.MapList[SelectHandle].Handle);
        posY += 20;
        EditorGUI.LabelField(new Rect(rect.x, rect.y + posY, rect.width / 2, 20), "Area: ");
        EditorDB.MapList[SelectHandle].Group = (EMapAreaGroup)EditorGUI.EnumPopup(new Rect(rect.x + rect.width / 2, rect.y + posY, rect.width / 2, 20), EditorDB.MapList[SelectHandle].Group);
        posY += 30;

        EditorGUI.LabelField(new Rect(rect.x, rect.y + posY, rect.width / 3, 20), "MinLevel: ");
        EditorDB.MapList[SelectHandle].MinLevel = EditorGUI.IntField(new Rect(rect.x + rect.width / 3, rect.y + posY, 2 * rect.width / 3, 20), EditorDB.MapList[SelectHandle].MinLevel);
        posY += 20;

        EditorGUI.LabelField(new Rect(rect.x, rect.y + posY, rect.width / 3, 20), "Level: ");
        EditorDB.MapList[SelectHandle].Level = EditorGUI.IntField(new Rect(rect.x + rect.width / 3, rect.y + posY, 2 * rect.width / 3, 20), EditorDB.MapList[SelectHandle].Level);
        posY += 20;

        EditorGUI.LabelField(new Rect(rect.x, rect.y + posY, rect.width / 3, 20), "Number: ");
        EditorDB.MapList[SelectHandle].Number = EditorGUI.IntField(new Rect(rect.x + rect.width / 3, rect.y + posY, 2 * rect.width / 3, 20), EditorDB.MapList[SelectHandle].Number);
        posY += 20;

        EditorGUI.LabelField(new Rect(rect.x, rect.y + posY, rect.width / 3, 20), "SceneName: ");
        EditorDB.MapList[SelectHandle].SceneName = EditorGUI.TextField(new Rect(rect.x + rect.width / 3, rect.y + posY, 2 * rect.width / 3, 20), EditorDB.MapList[SelectHandle].SceneName);
        posY += 20;

        EditorGUI.LabelField(new Rect(rect.x, rect.y + posY, rect.width / 3, 20), "MapName: ");
        EditorDB.MapList[SelectHandle].MapName = EditorGUI.TextField(new Rect(rect.x + rect.width / 3, rect.y + posY, 2 * rect.width / 3, 20), EditorDB.MapList[SelectHandle].MapName);
        posY += 20;

        EditorGUI.LabelField(new Rect(rect.x, rect.y + posY, rect.width / 3, 60), "Information: ");
        EditorDB.MapList[SelectHandle].Information = EditorGUI.TextField(new Rect(rect.x + rect.width / 3, rect.y + posY, 2 * rect.width / 3, 60), EditorDB.MapList[SelectHandle].Information);
        posY += 60;

        EditorGUI.LabelField(new Rect(rect.x, rect.y + posY, rect.width / 3, 20), "MapPath: ");
        EditorDB.MapList[SelectHandle].MapImgPath = EditorGUI.TextField(new Rect(rect.x + rect.width / 3, rect.y + posY, 2 * rect.width / 3, 20), EditorDB.MapList[SelectHandle].MapImgPath);
        posY += 20;

        EditorGUI.LabelField(new Rect(rect.x, rect.y + posY, rect.width / 3, 20), "IconPath: ");
        EditorDB.MapList[SelectHandle].MapIconPath = EditorGUI.TextField(new Rect(rect.x + rect.width / 3, rect.y + posY, 2 * rect.width / 3, 20), EditorDB.MapList[SelectHandle].MapIconPath);
        posY += 20;

        EditorGUI.LabelField(new Rect(rect.x, rect.y + posY, rect.width / 3, 20), "BGM: ");
        EditorDB.MapList[SelectHandle].BGM = EditorGUI.TextField(new Rect(rect.x + rect.width / 3, rect.y + posY, 2 * rect.width / 3, 20), EditorDB.MapList[SelectHandle].BGM);
        posY += 20;

        EditorDB.MapList[SelectHandle].Size = EditorGUI.Vector3Field(new Rect(rect.x, rect.y + posY, rect.width, 40), "Size: ", EditorDB.MapList[SelectHandle].Size);
        posY += 40;

        EditorDB.MapList[SelectHandle].WayPoint = EditorGUI.Vector3Field(new Rect(rect.x, rect.y + posY, rect.width, 40), "WayPoint: ", EditorDB.MapList[SelectHandle].WayPoint);
        if (EditorDB.MapList[SelectHandle].WayPoint != Vector3.zero)
        {
            float CoordScaleFactorX = 512 / EditorDB.MapList[SelectHandle].Size.x;
            float CoordScaleFactorY = 512 / EditorDB.MapList[SelectHandle].Size.y;
            GUI.DrawTexture(new Rect(315 + 256 + EditorDB.MapList[SelectHandle].WayPoint.x * CoordScaleFactorX, 35 + 256 - EditorDB.MapList[SelectHandle].WayPoint.z * CoordScaleFactorY, 10, 10), WayPointIcon);
        }
    }
    void InputPortal()
    {
        float CoordScaleFactorX = 512 / EditorDB.MapList[SelectHandle].Size.x;
        float CoordScaleFactorY = 512 / EditorDB.MapList[SelectHandle].Size.y; 

        Rect rect = new Rect(320, 40 + 512 + 20, 512 / 2, 150);
        for (int i = 0; i < EditorDB.MapList[SelectHandle].PortalList.Count; ++i)
        {
            GUI.Box(rect, "Portal (" + i + ")", EditorStyles.helpBox);
            float posY = 0;

            if (GUI.Button(new Rect(rect.x + rect.width - 20, rect.y + posY, 20, 20), "-"))
                EditorDB.MapList[SelectHandle].PortalList.RemoveAt(i);

            if (EditorDB.MapList[SelectHandle].PortalList.Count <= i)
                return;

            posY += 20;

            EditorGUI.LabelField(new Rect(rect.x, rect.y + posY, rect.width / 2, 20), "Handle: ");
            EditorDB.MapList[SelectHandle].PortalList[i].Handle = EditorGUI.IntField(new Rect(rect.x + rect.width / 2, rect.y + posY, rect.width / 2, 20), EditorDB.MapList[SelectHandle].PortalList[i].Handle);
            posY += 20; 
            if (EditorDB.MapList.ContainsKey(EditorDB.MapList[SelectHandle].PortalList[i].Handle))
                EditorGUI.LabelField(new Rect(rect.x, rect.y + posY, rect.width, 20), "MapName: " + EditorDB.MapList[EditorDB.MapList[SelectHandle].PortalList[i].Handle].MapName);
            else
                EditorGUI.LabelField(new Rect(rect.x, rect.y + posY, rect.width, 20), "MapName: NULL");
            posY += 20;
            EditorDB.MapList[SelectHandle].PortalList[i].Coord = EditorGUI.Vector3Field(new Rect(rect.x, rect.y + posY, rect.width, 40), "Coord:", EditorDB.MapList[SelectHandle].PortalList[i].Coord);
            posY += 40;
            EditorDB.MapList[SelectHandle].PortalList[i].DeltaCoord = EditorGUI.Vector3Field(new Rect(rect.x, rect.y + posY, rect.width, 40), "DeltaCoord:", EditorDB.MapList[SelectHandle].PortalList[i].DeltaCoord);

            rect.x += 512 / 2 + 10;
            if (rect.x > 320 + 512)
            {
                rect.x = 320;
                rect.y += 150;
            }

            GUI.DrawTexture(new Rect(315 + 256 + EditorDB.MapList[SelectHandle].PortalList[i].DeltaCoord.x * CoordScaleFactorX, 35 + 256 - EditorDB.MapList[SelectHandle].PortalList[i].DeltaCoord.z * CoordScaleFactorY, 10, 10), PortalDeltaIcon);
            GUI.DrawTexture(new Rect(315 + 256 + EditorDB.MapList[SelectHandle].PortalList[i].Coord.x * CoordScaleFactorX, 35 + 256 - EditorDB.MapList[SelectHandle].PortalList[i].Coord.z * CoordScaleFactorY, 10, 10), PortalIcon);
        }
    }
    void InputMatchPortal()
    {
        float CoordScaleFactorX = 512 / EditorDB.MapList[SelectHandle].Size.x;
        float CoordScaleFactorY = 512 / EditorDB.MapList[SelectHandle].Size.y;
        Rect rect = new Rect(860, 430, 250, 110);
        // Rect rect = new Rect(320, 40 + 512 + 20, 512 / 2, 110);
        for (int i = 0; i < EditorDB.MapList[SelectHandle].MatchPortalList.Count; ++i)
        {
            Rect boxRect = rect;
            boxRect.height += EditorDB.MapList[SelectHandle].MatchPortalList[i].HandleList.Count * 20;
            GUI.Box(boxRect, "MatchPortal (" + i + ")", EditorStyles.helpBox);
            float posY = 0;

            if (GUI.Button(new Rect(rect.x + rect.width - 20, rect.y + posY, 20, 20), "-"))
                EditorDB.MapList[SelectHandle].MatchPortalList.RemoveAt(i);

            if (EditorDB.MapList[SelectHandle].MatchPortalList.Count <= i)
                return;

            posY += 20;
            EditorDB.MapList[SelectHandle].MatchPortalList[i].Coord = EditorGUI.Vector3Field(new Rect(rect.x, rect.y + posY, rect.width, 40), "Coord:", EditorDB.MapList[SelectHandle].MatchPortalList[i].Coord);
            posY += 40;

            EditorGUI.LabelField(new Rect(rect.x, rect.y + posY, rect.width / 2, 20), "HandleList: ");
            if (GUI.Button(new Rect(rect.x + rect.width - 20, rect.y + posY, 20, 20), "+"))
                EditorDB.MapList[SelectHandle].MatchPortalList[i].HandleList.Add(0);
            for(int j = 0; j< EditorDB.MapList[SelectHandle].MatchPortalList[i].HandleList.Count; ++j)
            {
                posY += 20;
                EditorDB.MapList[SelectHandle].MatchPortalList[i].HandleList[j] = EditorGUI.IntField(new Rect(rect.x + rect.width / 2, rect.y + posY, rect.width / 2, 20), EditorDB.MapList[SelectHandle].MatchPortalList[i].HandleList[j]);
                if (EditorDB.MapList.ContainsKey(EditorDB.MapList[SelectHandle].MatchPortalList[i].HandleList[j]))
                    EditorGUI.LabelField(new Rect(rect.x, rect.y + posY, rect.width, 20), EditorDB.MapList[EditorDB.MapList[SelectHandle].MatchPortalList[i].HandleList[j]].MapName);
                else
                    EditorGUI.LabelField(new Rect(rect.x, rect.y + posY, rect.width, 20), "MapName: NULL");
            }

            rect.y += rect.height + EditorDB.MapList[SelectHandle].MatchPortalList[i].HandleList.Count * 20;

            GUI.DrawTexture(new Rect(315 + 256 + EditorDB.MapList[SelectHandle].MatchPortalList[i].Coord.x * CoordScaleFactorX, 35 + 256 - EditorDB.MapList[SelectHandle].MatchPortalList[i].Coord.z * CoordScaleFactorY, 10, 10), MatchPortalIcon);
        }
    }
    
    void InputMonster()
    {
        float posY = 0;
        Rect rect = new Rect(1130, 40, 250, 900);
        GUI.Box(rect, "Monster (" + EditorDB.MapList[SelectHandle].MonsterList.Count + ")", EditorStyles.helpBox);
        if (GUI.Button(new Rect(rect.x + rect.width - 20, rect.y + posY, 20, 20), "+"))
            EditorDB.MapList[SelectHandle].MonsterList.Add(new SMapMonster());

        posY += 30;

        float CoordScaleFactorX = 512 / EditorDB.MapList[SelectHandle].Size.x;
        float CoordScaleFactorY = 512 / EditorDB.MapList[SelectHandle].Size.y;

        for (int i = 0; i < EditorDB.MapList[SelectHandle].MonsterList.Count; ++i)
        {
            if (i == 13)
            {
                rect.x = 1400;
                posY = 30;
                GUI.Box(rect, "Monster (" + EditorDB.MapList[SelectHandle].MonsterList.Count + ")", EditorStyles.helpBox);
            }
            if (EditorDB.MonsterStatDic.ContainsKey(EditorDB.MapList[SelectHandle].MonsterList[i].Handle))
            {
                EditorGUI.LabelField(new Rect(rect.x, rect.y + posY, rect.width / 2, 20), "Name: " + EditorDB.MonsterStatDic[EditorDB.MapList[SelectHandle].MonsterList[i].Handle].Name);
            }
            else
            {
                EditorGUI.LabelField(new Rect(rect.x, rect.y + posY, rect.width / 2, 20), "Name: NotFound");
            }
            EditorDB.MapList[SelectHandle].MonsterList[i].Handle = EditorGUI.IntField(new Rect(rect.x + rect.width / 2, rect.y + posY, rect.width / 2 - 20, 20), EditorDB.MapList[SelectHandle].MonsterList[i].Handle);
            if (GUI.Button(new Rect(rect.x + rect.width - 20, rect.y + posY, 20, 20), "-"))
                EditorDB.MapList[SelectHandle].MonsterList.RemoveAt(i);

            if (EditorDB.MapList[SelectHandle].MonsterList.Count <= i)
                return;

            posY += 20;
            EditorGUI.LabelField(new Rect(rect.x, rect.y + posY, rect.width / 2, 20), "RespawnTime: ");
            EditorDB.MapList[SelectHandle].MonsterList[i].RespawnTime = EditorGUI.FloatField(new Rect(rect.x + rect.width / 2, rect.y + posY, rect.width / 2 - 20, 20), EditorDB.MapList[SelectHandle].MonsterList[i].RespawnTime);
            posY += 20;
            EditorGUI.LabelField(new Rect(rect.x, rect.y + posY, rect.width / 3, 20), "Coord: ");
            EditorDB.MapList[SelectHandle].MonsterList[i].Coord = EditorGUI.Vector3Field(new Rect(rect.x + rect.width / 3-40, rect.y + posY, 2 * rect.width / 3 - 20, 20), "", EditorDB.MapList[SelectHandle].MonsterList[i].Coord);
            EditorGUI.LabelField(new Rect(rect.x - 55 + rect.width, rect.y + posY, 10, 20), "A: ");
            EditorDB.MapList[SelectHandle].MonsterList[i].Angle = EditorGUI.FloatField(new Rect(rect.x - 40 + rect.width, rect.y + posY, 35, 20), EditorDB.MapList[SelectHandle].MonsterList[i].Angle);
            posY += 25;

            GUI.DrawTexture(new Rect(315 + 256 + EditorDB.MapList[SelectHandle].MonsterList[i].Coord.x * CoordScaleFactorX, 35 + 256 - EditorDB.MapList[SelectHandle].MonsterList[i].Coord.z * CoordScaleFactorY, 10, 10), MonsterIcon);
        }
    }
    void InputNPC()
    {
        float posY = 0;
        Rect rect = new Rect(860, 350, 250, 590);
        GUI.Box(rect, "NPC", EditorStyles.helpBox);
        if (GUI.Button(new Rect(rect.x + rect.width - 20, rect.y + posY, 20, 20), "+"))
            EditorDB.MapList[SelectHandle].NPCList.Add(new SMapNPC());

        posY += 30;

        float CoordScaleFactorX = 512 / EditorDB.MapList[SelectHandle].Size.x;
        float CoordScaleFactorY = 512 / EditorDB.MapList[SelectHandle].Size.y;

        for (int i = 0; i < EditorDB.MapList[SelectHandle].NPCList.Count; ++i)
        {
            if (EditorDB.MonsterStatDic.ContainsKey(EditorDB.MapList[SelectHandle].NPCList[i].Handle))
            {
                EditorGUI.LabelField(new Rect(rect.x, rect.y + posY, rect.width / 2, 20), "Name: " + EditorDB.MonsterStatDic[EditorDB.MapList[SelectHandle].NPCList[i].Handle].Name);
            }
            else
            {
                EditorGUI.LabelField(new Rect(rect.x, rect.y + posY, rect.width / 2, 20), "Name: NotFound");
            }
            EditorDB.MapList[SelectHandle].NPCList[i].Handle = EditorGUI.IntField(new Rect(rect.x + rect.width / 2, rect.y + posY, rect.width / 2 - 20, 20), EditorDB.MapList[SelectHandle].NPCList[i].Handle);
            if (GUI.Button(new Rect(rect.x + rect.width - 20, rect.y + posY, 20, 20), "-"))
                EditorDB.MapList[SelectHandle].NPCList.RemoveAt(i);
            posY += 20;
            EditorGUI.LabelField(new Rect(rect.x, rect.y + posY, rect.width / 3, 20), "Coord: ");
            EditorDB.MapList[SelectHandle].NPCList[i].Coord = EditorGUI.Vector2Field(new Rect(rect.x + rect.width / 3, rect.y + posY, 2 * rect.width / 3 - 20, 20), "", EditorDB.MapList[SelectHandle].NPCList[i].Coord);
            posY += 25;

            GUI.DrawTexture(new Rect(315 + 256 + EditorDB.MapList[SelectHandle].NPCList[i].Coord.x * CoordScaleFactorX, 35 + 256 - EditorDB.MapList[SelectHandle].NPCList[i].Coord.z * CoordScaleFactorY, 10, 10), NPCIcon);
        }
    }
    void ProcessEvents(Event e)
    {
        drag = Vector2.zero;
        switch (e.type)
        {
            case EventType.MouseDown:
                if (e.button == 1)
                {
                    if (e.mousePosition.x > 300)
                        break;
                    if (m_mapArea != EMapType.None)
                        ProcessContextMenu();
                }
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
    void ProcessContextMenu()
    {
        GenericMenu genericMenu = new GenericMenu();
        genericMenu.AddItem(new GUIContent("Add Map"), false, () => OnClickAddNode());
        genericMenu.ShowAsContext();
    }
    void OnClickAddNode()
    {
        Map Map = new Map();
        int handle = 0;
        while (true)
        {
        Start:

            foreach (Map map in EditorDB.MapList.Values)
            {
                if (map.Handle == handle)
                {
                    ++handle;
                    goto Start;
                }
            }

            Map.MapName = "New Map";
            Map.Handle = handle;
            Map.Type = m_mapArea;

            EditorDB.MapList.Add(handle, Map);
            return;
        }
    }
    void Save()
    {
        string Path = "Assets/AssetBundle_Database/DB_Map.json";
        FileStream File = new FileStream(Path, FileMode.Create);
        int i = 0;
        WriteFileToString(File, "{\r");
        foreach (KeyValuePair<int, Map> map in EditorDB.MapList)
        {
            WriteFileToString(File, "   \"" + i + "\":" + "{\r");
            WriteFileToString(File, "      \"Handle\" : " + "\"" + map.Value.Handle + "\",\r");
            WriteFileToString(File, "      \"Area\" : " + "\"" + (int)map.Value.Type + "\",\r");
            WriteFileToString(File, "      \"Group\" : " + "\"" + (int)map.Value.Group + "\",\r");
            WriteFileToString(File, "      \"MinLevel\" : " + "\"" + map.Value.MinLevel + "\",\r");
            WriteFileToString(File, "      \"Level\" : " + "\"" + map.Value.Level + "\",\r");
            WriteFileToString(File, "      \"Number\" : " + "\"" + map.Value.Number + "\",\r");
            WriteFileToString(File, "      \"MapName\" : " + "\"" + map.Value.MapName + "\",\r");
            WriteFileToString(File, "      \"Information\" : " + "\"" + map.Value.Information + "\",\r");
            WriteFileToString(File, "      \"MapImgPath\" : " + "\"" + map.Value.MapImgPath + "\",\r");
            WriteFileToString(File, "      \"MapIconPath\" : " + "\"" + map.Value.MapIconPath + "\",\r");
            WriteFileToString(File, "      \"SceneName\" : " + "\"" + map.Value.SceneName + "\",\r");
            WriteFileToString(File, "      \"BGM\" : " + "\"" + map.Value.BGM + "\",\r");
            WriteFileToString(File, "      \"Size\" : " + "\"" + map.Value.Size.x+ "," + map.Value.Size.y + "\",\r");
            WriteFileToString(File, "      \"WayPoint\" : " + "\"" + map.Value.WayPoint.x + "," + map.Value.WayPoint.y + "," + map.Value.WayPoint.z + "\",\r");

            string[] Portal = new string[map.Value.PortalList.Count];
            for (int j = 0; j < map.Value.PortalList.Count; ++j)
            {
                Portal[j] = map.Value.PortalList[j].Handle + "," + map.Value.PortalList[j].Coord.x + "," + map.Value.PortalList[j].Coord.y + "," + map.Value.PortalList[j].Coord.z;
                Portal[j] += "," + map.Value.PortalList[j].DeltaCoord.x + "," + map.Value.PortalList[j].DeltaCoord.y + "," + map.Value.PortalList[j].DeltaCoord.z;
            }

            string[] Monster = new string[map.Value.MonsterList.Count];
            for (int j = 0; j < map.Value.MonsterList.Count; ++j)
                Monster[j] = map.Value.MonsterList[j].Handle + "," + map.Value.MonsterList[j].RespawnTime + ","+ map.Value.MonsterList[j].Coord.x 
                    + "," + map.Value.MonsterList[j].Coord.y + "," + map.Value.MonsterList[j].Coord.z + "," + map.Value.MonsterList[j].Angle;

            string[] MatchPortal = new string[map.Value.MatchPortalList.Count];
            for(int j = 0; j<map.Value.MatchPortalList.Count; ++j)
            {
                string handleList = string.Join(",", map.Value.MatchPortalList[j].HandleList);
                MatchPortal[j] = map.Value.MatchPortalList[j].Coord.x + "," + map.Value.MatchPortalList[j].Coord.y + "," + map.Value.MatchPortalList[j].Coord.z;
                if(map.Value.MatchPortalList[j].HandleList.Count != 0)
                    MatchPortal[j] += "," + handleList;
            }

            //string[] NPC = new string[map.Value.NPCList.Count];
            //for (int j = 0; j < map.Value.NPCList.Count; ++j)
            //    NPC[j] = map.Value.NPCList[j].Handle + "," + map.Value.NPCList[j].Coord.x + "," + map.Value.NPCList[j].Coord.y + "," + map.Value.NPCList[j].Coord.z;


            WriteFileToString(File, "      \"Portal\" : " + "\"" + string.Join("/", Portal) + "\",\r");
            WriteFileToString(File, "      \"MatchPortal\" : " + "\"" + string.Join("/", MatchPortal) + "\",\r");
            WriteFileToString(File, "      \"Monster\" : " + "\"" + string.Join("/", Monster) + "\",\r");
            // WriteFileToString(File, "      \"NPC\" : " + "\"" + string.Join("/", NPC) + "\"\r");

            if (i < EditorDB.MapList.Count - 1)
                WriteFileToString(File, "   },\r\r");
            else
                WriteFileToString(File, "   }\r");
            ++i;
        }
        WriteFileToString(File, "}");
        File.Close();

        UnityEditor.AssetDatabase.Refresh();
    }
    void Remove()
    {
        EditorDB.MapList.Remove(SelectHandle);
        SelectHandle = 0;
    }
    void AddPortal()
    {
        if (!EditorDB.MapList.ContainsKey(SelectHandle))
            return;

        EditorDB.MapList[SelectHandle].PortalList.Add(new SMapPortal());
    }
    void AddMatchPortal()
    {
        if (!EditorDB.MapList.ContainsKey(SelectHandle))
            return;

        EditorDB.MapList[SelectHandle].MatchPortalList.Add(new SMatchPortal());
    }
    void InstanceMapCaptureCam(float width, float height)
    {
        if (m_captureCam == null)
            m_captureCam = Editor.Instantiate(PrefabUtility.LoadPrefabContents("Assets/MapCaptureCamera.prefab")).GetComponent<Camera>();

        m_captureCam.rect = new Rect(0, 0, 0.562f * width / height, 1);
        
        if (width == height)
        {
            float hypotenuse = Mathf.Sqrt(width * width + width * width *0.5f*0.5f);
            m_captureCam.transform.position = new Vector3(0, width, 0);
            m_captureCam.fieldOfView = Mathf.Acos(width / hypotenuse) * Mathf.Rad2Deg*2;
        }
        else if (width > height)
        {
            float hypotenuse = Mathf.Sqrt(height * height + width * width*0.5f*0.5f);
            m_captureCam.transform.position = new Vector3(0, width, 0);
            m_captureCam.fieldOfView = Mathf.Acos(height / hypotenuse) * Mathf.Rad2Deg;
        }
        else
        {
            float hypotenuse = Mathf.Sqrt(width * width + height * height * 0.5f * 0.5f);
            m_captureCam.transform.position = new Vector3(0, height, 0);
            m_captureCam.fieldOfView = Mathf.Acos(width/ hypotenuse) * Mathf.Rad2Deg;
        }

        UnityEngine.ScreenCapture.CaptureScreenshot(EditorDB.MapList[SelectHandle].SceneName + ".png");
    }
    public GameObject[] FindAllObject()
    {
        return Resources.FindObjectsOfTypeAll(typeof(GameObject)) as GameObject[];
    }
    void ResetPortalObject()
    {
        GameObject[] Objects = FindAllObject();
        for (int i = 0; i < Objects.Length; ++i)
            if (Objects[i].GetComponent<MatchPortal>() || Objects[i].GetComponent<WayPortal>())
            {
                Debug.Log("TRUE");
                DestroyImmediate(Objects[i]);
            }

        MatchPortal MatchPortal = Resources.Load<MatchPortal>("Content/MatchPortal");
        for (int i = 0; i < EditorDB.MapList[SelectHandle].MatchPortalList.Count; ++i)
        {
            Editor.Instantiate(MatchPortal).Number = i;
        }

        WayPortal WayPortal = Resources.Load<WayPortal>("Content/WayPortal");
        for(int i = 0; i<EditorDB.MapList[SelectHandle].PortalList.Count; ++i)
        {
            Editor.Instantiate(WayPortal).Number = i;
        }
    }
    void WriteFileToString(FileStream file, string str)
    {
        byte[] bytes = System.Text.Encoding.UTF8.GetBytes(str);
        file.Write(bytes, 0, bytes.Length);
    }
    private void OnDisable()
    {
        if(m_captureCam != null)
            DestroyImmediate(m_captureCam.gameObject);
    }
}

