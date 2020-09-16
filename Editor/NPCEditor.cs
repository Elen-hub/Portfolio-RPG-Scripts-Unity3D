using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using SimpleJSON;

public class NPCEditor : EditorWindow
{
    static EditorWindow m_window;
    static GUIStyle m_guiStyle;
    static float m_windowSize;

    NPCStat m_stat;
    int m_handle;
    int m_inputHandle;

    int m_toolbarIndex;
    string[] m_toolbars = { "Information", "StatSystem", "Scripts" };

    static PrefabEdit m_prefabEdit;

    [MenuItem("CustomEditor/NPC Editor")]
    public static void ShowWindow()
    {
        m_window = GetWindow(typeof(NPCEditor));
        m_window.minSize = new Vector2(400, 800);
        m_guiStyle = new GUIStyle();
        m_guiStyle.richText = true;

        if(m_prefabEdit ==null)
            m_prefabEdit = ScriptableObject.CreateInstance(typeof(PrefabEdit)) as PrefabEdit;

        EditorDB.Init();
    }
    private void OnGUI()
    {
        m_windowSize = position.width;

        int posY = 0;
        InputHandle(ref posY);
        CheckhandleState(ref posY);

        if (m_stat == null)
            return;

        m_toolbarIndex = GUI.Toolbar(new Rect(0, posY, m_windowSize, 20), m_toolbarIndex, m_toolbars);
        posY += 30;

        switch (m_toolbarIndex)
        {
            case 0:
                EditorGUI.LabelField(new Rect(0, posY, m_windowSize / 2, 20), "Name", m_guiStyle);
                m_stat.Name = EditorGUI.TextField(new Rect(m_windowSize / 2, posY, m_windowSize / 2, 20), m_stat.Name);
                posY += 20;
                EditorGUI.LabelField(new Rect(0, posY, m_windowSize / 2, 20), "MoveSpeed", m_guiStyle);
                m_stat.MoveSpeed = EditorGUI.FloatField(new Rect(m_windowSize / 2, posY, m_windowSize / 2, 20), m_stat.MoveSpeed);
                posY += 20;
                m_stat = m_prefabEdit.InputPrefab(ref posY, m_stat, m_windowSize, m_guiStyle, "Assets/AssetBundle_Character/Hero", m_stat.Path);
                break;
            case 1:
                EditorGUI.LabelField(new Rect(0, posY, m_windowSize / 2, 20), "Option", m_guiStyle);
                m_stat.Option = (ENpcOption)EditorGUI.EnumFlagsField(new Rect(m_windowSize / 2, posY, m_windowSize / 2, 20), m_stat.Option);
                posY += 30;
                if((m_stat.Option & ENpcOption.Shop) != 0)
                {
                    if (m_stat.ShopHandle == null)
                        m_stat.ShopHandle = new List<int>();

                    EditorGUI.LabelField(new Rect(30+m_windowSize/2, posY, m_windowSize / 2, 20), "ShopList", m_guiStyle);
                    if (GUI.Button(new Rect(0, posY, 20, 20), "+"))
                        m_stat.ShopHandle.Add(0);

                    posY += 20;
                    for(int i =0; i<m_stat.ShopHandle.Count; ++i)
                    {
                        m_stat.ShopHandle[i] = EditorGUI.IntField(new Rect(0, posY, m_windowSize / 2 - 20, 20), m_stat.ShopHandle[i]);
                        if(EditorDB.ItemDic.ContainsKey(m_stat.ShopHandle[i]))
                            EditorGUI.LabelField(new Rect(m_windowSize / 2-20, posY, m_windowSize / 2, 20), EditorDB.ItemDic[m_stat.ShopHandle[i]].Name, m_guiStyle);
                        else
                            EditorGUI.LabelField(new Rect(m_windowSize / 2 - 20, posY, m_windowSize / 2, 20), "Item is null", m_guiStyle);
                      
                        if (GUI.Button(new Rect(m_windowSize - 20, posY, 20, 20), "-"))
                            m_stat.ShopHandle.RemoveAt(i);

                        posY += 20;
                    }
                }
                break;
            case 2:
                if (m_stat.Comment == null)
                    m_stat.Comment = new List<string>();

                EditorGUI.LabelField(new Rect(30 + m_windowSize / 2, posY, m_windowSize / 2, 20), "Comment", m_guiStyle);
                if (GUI.Button(new Rect(0, posY, 20, 20), "+"))
                    m_stat.Comment.Add("");

                posY += 20;

                for (int i = 0; i < m_stat.Comment.Count; ++i)
                {
                    m_stat.Comment[i] = EditorGUI.TextField(new Rect(30, posY, m_windowSize - 30, 20), m_stat.Comment[i]);

                    if (GUI.Button(new Rect(0, posY, 20, 20), "-"))
                        m_stat.Comment.RemoveAt(i);

                    posY += 20;
                }

                posY += 30;

                if (m_stat.Scripts == null)
                    m_stat.Scripts = new List<string>();

                EditorGUI.LabelField(new Rect(30 + m_windowSize / 2, posY, m_windowSize / 2, 20), "Scripts", m_guiStyle);
                if (GUI.Button(new Rect(0, posY, 20, 20), "+"))
                    m_stat.Scripts.Add("");

                posY += 20;

                for (int i = 0; i < m_stat.Scripts.Count; ++i)
                {
                    m_stat.Scripts[i] = EditorGUI.TextField(new Rect(30, posY, m_windowSize - 30, 20), m_stat.Scripts[i]);

                    if (GUI.Button(new Rect(0, posY, 20, 20), "-"))
                        m_stat.Scripts.RemoveAt(i);

                    posY += 20;
                }
                break;
        }

        posY = (int)position.height - 110;
        ApplyHandle(ref posY);
        DeleteHandle(ref posY);
        AllSave(ref posY);
    }
    void InputHandle(ref int posY)
    {
        if (GUI.Button(new Rect(0, posY, m_windowSize, 20), "New handle to last number"))
        {
            m_handle = EditorDB.NPCStatDic.Count + 1;
            m_inputHandle = m_handle;
        }
        posY += 20;
        m_inputHandle = EditorGUI.IntField(new Rect(m_windowSize / 2 - 20, posY, m_windowSize / 2, 20), m_inputHandle);
        if (GUI.Button(new Rect(m_windowSize - 20, posY, 20, 20), "F"))
            m_handle = m_inputHandle;
    }
    void CheckhandleState(ref int posY)
    {
        if (EditorDB.NPCStatDic == null)
            return;

        if(m_handle != 0)
        {
            if (EditorDB.NPCStatDic.ContainsKey(m_handle))
            {
                if (m_stat == null || m_stat.Handle != m_handle)
                {
                    m_prefabEdit.Reset();

                    m_stat = EditorDB.NPCStatDic[m_handle];
                    m_prefabEdit.Prefab = Resources.Load<GameObject>(m_stat.Path);
                }
                EditorGUI.LabelField(new Rect(0, posY, m_windowSize, 20), "<color=red>Handle: </color>", m_guiStyle);
                posY += 20;
                EditorGUI.LabelField(new Rect(0, posY, m_windowSize, 20), "<color=red>Fixed Character</color>", m_guiStyle);
            }
            else
            {
                if (m_stat == null || m_stat.Handle != m_handle)
                {
                    m_prefabEdit.Reset();

                    m_stat = new NPCStat();
                    m_stat.Handle = m_handle;
                }

                EditorGUI.LabelField(new Rect(0, posY, m_windowSize, 20), "<color=blue>Handle: </color>", m_guiStyle);
                posY += 20;
                EditorGUI.LabelField(new Rect(0, posY, m_windowSize, 20), "<color=blue>New Character</color>", m_guiStyle);
            }
        }
        else
        {
            m_prefabEdit.Reset();

            EditorGUI.LabelField(new Rect(0, posY, m_windowSize, 20), "<color=black>Handle: </color>", m_guiStyle);
            posY += 20;
            EditorGUI.LabelField(new Rect(0, posY, m_windowSize, 20), "<color=black>Please input handle</color>.", m_guiStyle);
            posY += 20;
            SearchList(ref posY);
        }

        posY += 30;
    }
    void SearchList(ref int posY)
    {
        foreach (NPCStat stat in EditorDB.NPCStatDic.Values)
        {
            EditorGUI.LabelField(new Rect(0, posY, m_windowSize - 20, 20), "[" + stat.Handle + "] " + stat.Name + "</color>", m_guiStyle);
            if (GUI.Button(new Rect(m_windowSize - 20, posY, 20, 20), "F"))
            {
                m_handle = stat.Handle;
                m_inputHandle = stat.Handle;
                if (EditorDB.NPCStatDic.ContainsKey(m_handle))
                {
                    if (m_stat == null || m_stat.Handle != m_handle)
                    {
                        m_prefabEdit.Reset();

                        m_stat = EditorDB.NPCStatDic[m_handle];
                        m_prefabEdit.Prefab = Resources.Load<GameObject>(m_stat.Path);
                    }
                }
                return;
            }
            posY += 20;
        }
    }
    void ApplyHandle(ref int posY)
    {
        if (GUI.Button(new Rect(50, posY, m_windowSize - 100, 20), "수정사항 핸들값에 적용"))
        {
            if (EditorDB.NPCStatDic.ContainsKey(m_handle))
                EditorDB.NPCStatDic[m_handle] = m_stat;
            else
                EditorDB.NPCStatDic.Add(m_handle, m_stat);
        }

        posY += 30;
    }
    void DeleteHandle(ref int posY)
    {
        if (GUI.Button(new Rect(50, posY, m_windowSize - 100, 20), "현재핸들 제거"))
        {
            if (EditorDB.NPCStatDic.ContainsKey(m_handle))
                EditorDB.NPCStatDic.Remove(m_handle);
            if (EditorDB.MonsterAttackDic.ContainsKey(m_handle))
                EditorDB.MonsterAttackDic.Remove(m_handle);

            m_handle = 0;
            m_inputHandle = 0;
            m_stat = null;
            m_prefabEdit.Reset();
        }

        posY += 30;
    }
    void AllSave(ref int posY)
    {
        if (GUI.Button(new Rect(50, posY, m_windowSize - 100, 20), "DB 저장"))
        {
            Save();
            m_prefabEdit.Save("Assets/AssetBundle_Character/NPC/");
            UnityEditor.AssetDatabase.Refresh();
        }
    }
    void Save()
    {
        string Path = "Assets/AssetBundle_Database/DB_NPC.json";
        FileStream File = new FileStream(Path, FileMode.Create);
        int i = 0;
        WriteFileToString(File, "{\r");
        foreach (KeyValuePair<int, NPCStat> stat in EditorDB.NPCStatDic)
        {
            WriteFileToString(File, "   \"" + i + "\":" + "{\r");
            WriteFileToString(File, "      \"Handle\" : " + "\"" + stat.Value.Handle + "\",\r");
            WriteFileToString(File, "      \"Name\" : " + "\"" + stat.Value.Name + "\",\r");
            WriteFileToString(File, "      \"Path\" : " + "\"" + stat.Value.Path + "\",\r");
            WriteFileToString(File, "      \"Option\" : " + "\"" + (int)stat.Value.Option + "\",\r");

            if (stat.Value.Comment.Count != 0)
                WriteFileToString(File, "      \"Comment\" : " + "\"" + string.Join(",", stat.Value.Comment) + "\",\r");
            if (stat.Value.Scripts.Count != 0)
                WriteFileToString(File, "      \"Scripts\" : " + "\"" + string.Join(",", stat.Value.Scripts) + "\",\r");
            if ((stat.Value.Option & ENpcOption.Shop) != 0)
                WriteFileToString(File, "      \"ShopHandle\" : " + "\"" + string.Join(",", stat.Value.ShopHandle) + "\",\r");

            WriteFileToString(File, "      \"MoveSpeed\" : " + "\"" + stat.Value.MoveSpeed + "\"\r");

            if (i < EditorDB.NPCStatDic.Count - 1)
                WriteFileToString(File, "   },\r\r");
            else
                WriteFileToString(File, "   }\r");
            ++i;
        }
        WriteFileToString(File, "}");
        File.Close();
    }
    void WriteFileToString(FileStream file, string str)
    {
        byte[] bytes = System.Text.Encoding.UTF8.GetBytes(str);
        file.Write(bytes, 0, bytes.Length);
    }
    private void OnDestroy()
    {
        m_stat = null;
        m_prefabEdit.Reset();
    }
}
