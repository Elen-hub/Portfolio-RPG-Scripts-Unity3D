using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using SimpleJSON;

public class CharacterEditor : EditorWindow
{
    static EditorWindow m_window;
    static GUIStyle m_guiStyle;
    static float m_windowSize;

    Stat m_stat;
    NormalAttack m_normalAttack;
    int m_handle;
    int m_inputHandle;

    int m_toolbarIndex;
    string[] m_toolbars = { "Information", "StatSystem", "AttackSystem" };

    static StatEdit m_statEdit;
    static PrefabEdit m_prefabEdit;
    static NormalAttackEdit m_normalAttackEdit;

    [MenuItem("CustomEditor/Character Editor")]
    public static void ShowWindow()
    {
        m_window = GetWindow(typeof(CharacterEditor));
        m_window.minSize = new Vector2(400, 800);
        m_window.maxSize = new Vector2(400, 800);
        m_windowSize = m_window.maxSize.x;
        m_guiStyle = new GUIStyle();
        m_guiStyle.richText = true;

        if(m_statEdit == null)
            m_statEdit = ScriptableObject.CreateInstance(typeof(StatEdit)) as StatEdit;
        if(m_prefabEdit ==null)
            m_prefabEdit = ScriptableObject.CreateInstance(typeof(PrefabEdit)) as PrefabEdit;
        if(m_normalAttackEdit == null)
            m_normalAttackEdit = ScriptableObject.CreateInstance(typeof(NormalAttackEdit)) as NormalAttackEdit;
        
        EditorDB.Init();
    }
    private void OnGUI()
    {
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
                m_stat = m_statEdit.InputInformation(ref posY, m_stat, m_windowSize, m_guiStyle, "Icon/Hero/");
                m_stat = m_prefabEdit.InputPrefab(ref posY, m_stat, m_windowSize, m_guiStyle, "Assets/AssetBundle_Character/Hero", m_stat.Path + ".prefab");
                break;
            case 1:
                m_stat = m_statEdit.InputHeroStat(ref posY, m_stat, m_windowSize, m_guiStyle);
                m_stat = m_statEdit.InputStat(ref posY, m_stat, m_windowSize, m_guiStyle);
                break;
            case 2:
                m_normalAttack = m_normalAttackEdit.InputNormalAttack(ref posY, m_normalAttack, m_windowSize, m_guiStyle); ;
                break;
        }

        posY = (int)m_window.maxSize.y - 110;
        ApplyHandle(ref posY);
        DeleteHandle(ref posY);
        AllSave(ref posY);
    }
    void InputHandle(ref int posY)
    {
        if (GUI.Button(new Rect(0, posY, m_windowSize, 20), "New handle to last number"))
        {
            m_handle = EditorDB.CharacterStatDic.Count + 1;
            m_inputHandle = m_handle;
        }
        posY += 20;
        m_inputHandle = EditorGUI.IntField(new Rect(m_windowSize / 2 - 20, posY, m_windowSize / 2, 20), m_inputHandle);
        if (GUI.Button(new Rect(m_windowSize - 20, posY, 20, 20), "F"))
            m_handle = m_inputHandle;
    }
    void CheckhandleState(ref int posY)
    {
        if (EditorDB.CharacterStatDic == null)
            return;

        if(m_handle != 0)
        {
            if (EditorDB.CharacterStatDic.ContainsKey(m_handle))
            {
                if (m_stat == null || m_stat.Handle != m_handle)
                {
                    m_statEdit.Reset();
                    m_prefabEdit.Reset();
                    m_normalAttackEdit.Reset();

                    m_stat = EditorDB.CharacterStatDic[m_handle];
                    m_normalAttack = EditorDB.NormalAttackDic[m_handle];
                    m_statEdit.IconTexture = Resources.Load<Texture2D>(m_stat.Icon);
                    m_prefabEdit.Prefab = PrefabUtility.LoadPrefabContents("Assets/AssetBundle_Character/Hero/" + m_stat.Path + ".prefab");
                }
                EditorGUI.LabelField(new Rect(0, posY, m_windowSize, 20), "<color=red>Handle: </color>", m_guiStyle);
                posY += 20;
                EditorGUI.LabelField(new Rect(0, posY, m_windowSize, 20), "<color=red>Fixed Character</color>", m_guiStyle);
            }
            else
            {
                if (m_stat == null || m_stat.Handle != m_handle)
                {
                    m_statEdit.Reset();
                    m_prefabEdit.Reset();
                    m_normalAttackEdit.Reset();

                    m_stat = new Stat();
                    m_stat.Handle = m_handle;
                    m_normalAttack = new NormalAttack();
                    m_normalAttack.Handle = m_handle;
                }

                EditorGUI.LabelField(new Rect(0, posY, m_windowSize, 20), "<color=blue>Handle: </color>", m_guiStyle);
                posY += 20;
                EditorGUI.LabelField(new Rect(0, posY, m_windowSize, 20), "<color=blue>New Character</color>", m_guiStyle);
            }
        }
        else
        {
            m_statEdit.Reset();
            m_prefabEdit.Reset();
            m_normalAttackEdit.Reset();

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
        foreach(Stat stat in EditorDB.CharacterStatDic.Values)
        {
            EditorGUI.LabelField(new Rect(0, posY, m_windowSize - 20, 20),  "[" + ParseLib.GetClassKorConvert(stat.Class) + "] " + stat.Name + "</color>", m_guiStyle);
            if (EditorDB.CharacterStatDic.ContainsKey(m_handle))
            {
                if (m_stat == null || m_stat.Handle != m_handle)
                {
                    m_statEdit.Reset();
                    m_prefabEdit.Reset();
                    m_normalAttackEdit.Reset();

                    m_stat = EditorDB.CharacterStatDic[m_handle];
                    m_normalAttack = EditorDB.NormalAttackDic[m_handle];
                    m_statEdit.IconTexture = Resources.Load<Texture2D>(m_stat.Icon);
                    m_prefabEdit.Prefab = PrefabUtility.LoadPrefabContents("Assets/AssetBundle_Character/Hero/" + m_stat.Path + ".prefab");
                }
            }
            posY += 20;
        }
    }
    void ApplyHandle(ref int posY)
    {
        if (GUI.Button(new Rect(50, posY, m_windowSize - 100, 20), "수정사항 핸들값에 적용"))
        {
            if (EditorDB.CharacterStatDic.ContainsKey(m_handle))
                EditorDB.CharacterStatDic[m_handle] = m_stat;
            else
                EditorDB.CharacterStatDic.Add(m_handle, m_stat);
            if (EditorDB.NormalAttackDic.ContainsKey(m_handle))
                EditorDB.NormalAttackDic[m_handle] = m_normalAttack;
            else
                EditorDB.NormalAttackDic.Add(m_handle, m_normalAttack);
        }

        posY += 30;
    }
    void DeleteHandle(ref int posY)
    {
        if (GUI.Button(new Rect(50, posY, m_windowSize - 100, 20), "현재핸들 제거"))
        {
            if (EditorDB.CharacterStatDic.ContainsKey(m_handle))
                EditorDB.CharacterStatDic.Remove(m_handle);
            if (EditorDB.NormalAttackDic.ContainsKey(m_handle))
                EditorDB.NormalAttackDic.Remove(m_handle);

            m_handle = 0;
            m_inputHandle = 0;
            m_stat = null;
            m_normalAttack = null;
            m_statEdit.Reset();
            m_prefabEdit.Reset();
            m_normalAttackEdit.Reset();
        }

        posY += 30;
    }
    void AllSave(ref int posY)
    {
        if (GUI.Button(new Rect(50, posY, m_windowSize - 100, 20), "DB 저장"))
        {
            Save();
            m_prefabEdit.Save("Assets/AssetBundle_Character/Hero/");
            UnityEditor.AssetDatabase.Refresh();
        }
    }
    void Save()
    {
        string Path = "Assets/AssetBundle_Database/DB_Hero.json";
        FileStream File = new FileStream(Path, FileMode.Create);
        int i = 0;
        WriteFileToString(File, "{\r");
        foreach (KeyValuePair<int, Stat> stat in EditorDB.CharacterStatDic)
        {
            WriteFileToString(File, "   \"" + i + "\":" + "{\r");
            WriteFileToString(File, "      \"Handle\" : " + "\"" + stat.Value.Handle + "\",\r");
            WriteFileToString(File, "      \"Class\" : " + "\"" + (int)stat.Value.Class + "\",\r");
            WriteFileToString(File, "      \"Awakening\" : " + "\"" + stat.Value.Awakening + "\",\r");
            WriteFileToString(File, "      \"Name\" : " + "\"" + stat.Value.Name + "\",\r");
            WriteFileToString(File, "      \"Explanation\" : " + "\"" + stat.Value.Explanation + "\",\r");
            WriteFileToString(File, "      \"Icon\" : " + "\"" + stat.Value.Icon + "\",\r");
            WriteFileToString(File, "      \"Path\" : " + "\"" + stat.Value.Path + "\",\r");
            WriteFileToString(File, "      \"STR\" : " + "\"" + stat.Value.STR + "\",\r");
            WriteFileToString(File, "      \"DEX\" : " + "\"" + stat.Value.DEX + "\",\r");
            WriteFileToString(File, "      \"INT\" : " + "\"" + stat.Value.INT + "\",\r");
            WriteFileToString(File, "      \"WIS\" : " + "\"" + stat.Value.WIS + "\",\r");
            WriteFileToString(File, "      \"CON\" : " + "\"" + stat.Value.CON + "\",\r");
            WriteFileToString(File, "      \"HP\" : " + "\"" + stat.Value.HP + "\",\r");
            WriteFileToString(File, "      \"RecoveryHP\" : " + "\"" + stat.Value.RecoveryHP + "\",\r");
            WriteFileToString(File, "      \"Resistance\" : " + "\"" + stat.Value.Resistance + "\",\r");
            WriteFileToString(File, "      \"MP\" : " + "\"" + stat.Value.MP + "\",\r");
            WriteFileToString(File, "      \"RecoveryMP\" : " + "\"" + stat.Value.RecoveryMP + "\",\r");
            WriteFileToString(File, "      \"CoolTime\" : " + "\"" + stat.Value.CoolTime + "\",\r");
            WriteFileToString(File, "      \"CriticalPro\" : " + "\"" + stat.Value.CriticalPro + "\",\r");
            WriteFileToString(File, "      \"CriticalDamage\" : " + "\"" + stat.Value.CriticalDamage + "\",\r");
            WriteFileToString(File, "      \"AttackSpeed\" : " + "\" 1 \",\r");
            WriteFileToString(File, "      \"MoveSpeed\" : " + "\"" + stat.Value.MoveSpeed + "\",\r");
            WriteFileToString(File, "      \"MoveSpeedPro\" : " + "\"" + stat.Value.MoveSpeedPro + "\",\r");
            WriteFileToString(File, "      \"AttackDamage\" : " + "\"" + stat.Value.AttackDamage + "\",\r");
            WriteFileToString(File, "      \"AttackDamagePro\" : " + "\"" + stat.Value.AttackDamagePro + "\",\r");
            WriteFileToString(File, "      \"Defence\" : " + "\"" + stat.Value.Defence + "\",\r");
            WriteFileToString(File, "      \"DefencePro\" : " + "\"" + stat.Value.DefencePro + "\",\r");
            WriteFileToString(File, "      \"SkillDamagePro\" : " + "\"" + stat.Value.SkillDamagePro + "\"\r");
            if (i < EditorDB.CharacterStatDic.Count - 1)
                WriteFileToString(File, "   },\r\r");
            else
                WriteFileToString(File, "   }\r");
            ++i;
        }
        WriteFileToString(File, "}");
        File.Close();

        Path = "Assets/AssetBundle_Database/DB_NormalAttack.json";
        File = new FileStream(Path, FileMode.Create);
        i = 0;
        WriteFileToString(File, "{\r");
        foreach (KeyValuePair<int, NormalAttack> attack in EditorDB.NormalAttackDic)
        {
            WriteFileToString(File, "   \"" + i + "\":" + "{\r");
            WriteFileToString(File, "      \"Handle\" : " + "\"" + attack.Value.Handle + "\",\r");
            WriteFileToString(File, "      \"Count\" : " + "\"" + attack.Value.Count + "\",\r");

            string DurationTime = "";
            string CompleteTime = "";
            string DamagePro = "";
            string Type = "";
            string Range = "";
            string Angle = "";
            string Width = "";
            string MissileHandle = "";
            string HitTime = "";

            for (int j =0;j<attack.Value.Count; ++j)
            {
                DurationTime += attack.Value.DurationTime[j];
                CompleteTime += attack.Value.CompleteTime[j];
                DamagePro += attack.Value.DamagePro[j];
                Type += (int)attack.Value.Type[j];
                Range += attack.Value.Range[j];
                Angle += attack.Value.Angle[j];
                Width += attack.Value.Width[j];
                MissileHandle += attack.Value.MissileHandle[j];
                HitTime += attack.Value.HitTime[j];

                if (j != attack.Value.Count - 1)
                {
                    DurationTime += ",";
                    CompleteTime += ",";
                    DamagePro += ",";
                    Type += ",";
                    Range += ",";
                    Angle += ",";
                    Width += ",";
                    MissileHandle += ",";
                    HitTime += ",";
                }
            }

            WriteFileToString(File, "      \"DurationTime\" : " + "\"" + DurationTime + "\",\r");
            WriteFileToString(File, "      \"CompleteTime\" : " + "\"" + CompleteTime + "\",\r");
            WriteFileToString(File, "      \"DamagePro\" : " + "\"" + DamagePro + "\",\r");
            WriteFileToString(File, "      \"Type\" : " + "\"" + Type + "\",\r");
            WriteFileToString(File, "      \"Range\" : " + "\"" + Range + "\",\r");
            WriteFileToString(File, "      \"MissileHandle\" : " + "\"" + MissileHandle + "\",\r");
            WriteFileToString(File, "      \"Width\" : " + "\"" + Width + "\",\r");
            WriteFileToString(File, "      \"HitTime\" : " + "\"" + HitTime + "\",\r");
            WriteFileToString(File, "      \"Angle\" : " + "\"" + Angle + "\"\r");

            if (i < EditorDB.NormalAttackDic.Count - 1)
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
        m_normalAttack = null;
        m_normalAttackEdit.Reset();
        m_statEdit.Reset();
        m_prefabEdit.Reset();
    }
}
