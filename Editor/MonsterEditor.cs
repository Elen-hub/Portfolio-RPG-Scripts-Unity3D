using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using SimpleJSON;

public class MonsterEditor : EditorWindow
{
    static EditorWindow m_window;
    static GUIStyle m_guiStyle;
    static float m_windowSize;

    Stat m_stat;
    NormalAttack m_normalAttack;
    int m_handle;
    int m_inputHandle;

    int m_toolbarIndex;
    string[] m_toolbars = { "Information", "StatSystem", "AttackSystem", "Pattern" };

    static StatEdit m_statEdit;
    static PrefabEdit m_prefabEdit;
    static NormalAttackEdit m_normalAttackEdit;

    [MenuItem("CustomEditor/Monster Editor")]
    public static void ShowWindow()
    {
        m_window = GetWindow(typeof(MonsterEditor));
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
                ShowInformation(ref posY);
                break;
            case 1:
                ShowStatSystem(ref posY);
                break;
            case 2:
                ShowAttackSystem(ref posY);
                break;
            case 3:
                ShowPattern(ref posY);
                break;
        }

        posY = (int)m_window.maxSize.y - 110;
        ApplyHandle(ref posY);
        DeleteHandle(ref posY);
        AllSave(ref posY);
    }
    void ShowInformation(ref int posY)
    {
        EditorGUI.LabelField(new Rect(0, posY, m_windowSize / 2, 20), "Monster Grade", m_guiStyle);
        m_stat.MonsterGrade = (EMonsterGrade)EditorGUI.EnumPopup(new Rect(m_windowSize / 2, posY, m_windowSize / 2, 20), m_stat.MonsterGrade);
        posY += 20;
        m_stat = m_statEdit.InputInformation(ref posY, m_stat, m_windowSize, m_guiStyle, "Icon/Monster/");
        m_stat = m_prefabEdit.InputPrefab(ref posY, m_stat, m_windowSize, m_guiStyle, "Assets/AssetBundle_Character/Monster", m_stat.Path);
    }
    void ShowStatSystem(ref int posY)
    {
        m_stat = m_statEdit.InputStat(ref posY, m_stat, m_windowSize, m_guiStyle);
        posY += 10;
        EditorGUI.LabelField(new Rect(0, posY, m_windowSize / 2, 20), "EXP", m_guiStyle);
        m_stat.EXP = EditorGUI.IntField(new Rect(m_windowSize / 2, posY, m_windowSize / 2, 20), m_stat.EXP);
        posY += 20;
        EditorGUI.LabelField(new Rect(0, posY, m_windowSize / 2, 20), "Gold", m_guiStyle);
        m_stat.Gold = EditorGUI.IntField(new Rect(m_windowSize / 2, posY, m_windowSize / 2, 20), m_stat.Gold);
        posY += 20;
        EditorGUI.LabelField(new Rect(0, posY, m_windowSize / 2, 20), "DropItemField", m_guiStyle);
        if (GUI.Button(new Rect(position.width - 20, posY, 20, 20), "+"))
            m_stat.Reword.Add(new SMonsterReword());
        for (int i = 0; i < m_stat.Reword.Count; ++i)
        {
            posY += 20;

            if (GUI.Button(new Rect(position.width - 20, posY, 20, 20), "-"))
            {
                m_stat.Reword.RemoveAt(i);
                return;
            }
            float posX = 0;
            if (EditorDB.ItemDic.ContainsKey(m_stat.Reword[i].Handle))
                EditorGUI.LabelField(new Rect(posX, posY, m_windowSize / 3, 20), EditorDB.ItemDic[m_stat.Reword[i].Handle].Name, m_guiStyle);
            else
                EditorGUI.LabelField(new Rect(posX, posY, m_windowSize / 3, 20), "Handle: ", m_guiStyle);
            posX += m_windowSize / 3;
            m_stat.Reword[i].Handle = EditorGUI.IntField(new Rect(posX, posY, m_windowSize / 8, 20), m_stat.Reword[i].Handle);
            posX += m_windowSize / 8;
            EditorGUI.LabelField(new Rect(posX, posY, m_windowSize / 6, 20), "Value: ", m_guiStyle);
            posX += m_windowSize / 8;
            m_stat.Reword[i].Value = EditorGUI.IntField(new Rect(posX, posY, m_windowSize / 8, 20), m_stat.Reword[i].Value);
            posX += m_windowSize / 8;
            EditorGUI.LabelField(new Rect(posX, posY, m_windowSize / 8, 20), "Percent: ", m_guiStyle);
            posX += m_windowSize / 8;
            m_stat.Reword[i].Percent = EditorGUI.FloatField(new Rect(posX, posY, m_windowSize / 8, 20), m_stat.Reword[i].Percent);
        }
    }
    void ShowAttackSystem(ref int posY)
    {
        m_normalAttack = m_normalAttackEdit.InputNormalAttack(ref posY, m_normalAttack, m_windowSize, m_guiStyle);
    }
    void ShowPattern(ref int posY)
    {
        if (GUI.Button(new Rect(20, posY, position.width-40, 20), "Addition Pattern State"))
            m_stat.Pattern.Add(new EnermyPattern(EEnermyPatternType.Always, m_stat.Pattern.Count+1, ""));

        for (int i = 0; i < m_stat.Pattern.Count; ++i)
        {
            posY += 20;
            EditorGUI.LabelField(new Rect(0, posY, m_windowSize / 2, 20), "------------------------------------------------------------------", m_guiStyle);
            posY += 20;

            EditorGUI.LabelField(new Rect(0, posY, m_windowSize / 4 -15, 20), "Pattern Type", m_guiStyle);
            m_stat.Pattern[i].Type = (EEnermyPatternType)EditorGUI.EnumFlagsField(new Rect(m_windowSize / 4 -15, posY, m_windowSize / 4, 20), m_stat.Pattern[i].Type);
            EditorGUI.LabelField(new Rect(m_windowSize / 2-10, posY, m_windowSize / 4-15, 20), "Order", m_guiStyle);
            int currOrder = m_stat.Pattern[i].Order;
            m_stat.Pattern[i].Order = EditorGUI.IntField(new Rect(3*m_windowSize / 4-25, posY, m_windowSize / 4, 20), m_stat.Pattern[i].Order);
            if(m_stat.Pattern[i].Order != currOrder)
            {
                if (m_stat.Pattern[i].Order > m_stat.Pattern.Count)
                    m_stat.Pattern[i].Order = m_stat.Pattern.Count;
                else
                {
                    m_stat.Pattern[i].Order = currOrder;
                    EnermyPattern pattern = m_stat.Pattern[i];
                    m_stat.Pattern.RemoveAt(i);
                    m_stat.Pattern.Insert(pattern.Order, pattern);
                    for (int j = 0; j < m_stat.Pattern.Count; ++j)
                        m_stat.Pattern[j].Order = j;
                    return;
                    
                }
            }

            if (GUI.Button(new Rect(position.width - 20, posY, 20, 20), "-"))
            {
                m_stat.Pattern.RemoveAt(i);
                return;
            }
            posY += 20;
            EditorGUI.LabelField(new Rect(0, posY, 60, 20), "Skill Key", m_guiStyle);
            m_stat.Pattern[i].SkillKey = EditorGUI.TextField(new Rect(60, posY, m_windowSize / 3, 20), m_stat.Pattern[i].SkillKey);
            if ((m_stat.Pattern[i].Type & EEnermyPatternType.HP) != 0 && (m_stat.Pattern[i].Type & EEnermyPatternType.Time) != 0)
            {
                EditorGUI.LabelField(new Rect(70 + m_windowSize / 3, posY, m_windowSize, 20), "<color=red>Don't use Time & HP Triggers</color>", m_guiStyle);
                return;
            }

            if ((m_stat.Pattern[i].Type & EEnermyPatternType.HP) != 0)
            {
                EditorGUI.LabelField(new Rect(70+m_windowSize/3, posY, 70, 20), "HP Trigger", m_guiStyle);
                m_stat.Pattern[i].TargetHPLow = EditorGUI.FloatField(new Rect(140 + m_windowSize / 3 , posY, (m_windowSize - (140 + m_windowSize / 3)) / 3, 20), m_stat.Pattern[i].TargetHPLow);
                EditorGUI.LabelField(new Rect(1.4f*(m_windowSize - (140 + m_windowSize / 3)) / 3 + 140 + m_windowSize / 3, posY, (m_windowSize - (140 + m_windowSize / 3)) / 3, 20), "~", m_guiStyle);
                m_stat.Pattern[i].TargetHPHigh = EditorGUI.FloatField(new Rect(2*(m_windowSize - (140 + m_windowSize / 3)) / 3 + 140 + m_windowSize / 3, posY, (m_windowSize - (140 + m_windowSize / 3)) / 3, 20), m_stat.Pattern[i].TargetHPHigh);
            }
            else if ((m_stat.Pattern[i].Type & EEnermyPatternType.Time) != 0)
            {
                EditorGUI.LabelField(new Rect(70 + m_windowSize / 3, posY, 80, 20), "Time Trigger", m_guiStyle);
                m_stat.Pattern[i].TargetTime = EditorGUI.FloatField(new Rect(150 + m_windowSize / 3, posY, m_windowSize - 140-m_windowSize/3, 20), m_stat.Pattern[i].TargetTime);
            }
        }
    }
    void InputHandle(ref int posY)
    {
        if (GUI.Button(new Rect(0, posY, m_windowSize, 20), "New handle to last number"))
        {
            m_handle = EditorDB.MonsterStatDic.Count + 1;
            m_inputHandle = m_handle;
        }
        posY += 20;
        m_inputHandle = EditorGUI.IntField(new Rect(m_windowSize / 2 - 20, posY, m_windowSize / 2, 20), m_inputHandle);
        if (GUI.Button(new Rect(m_windowSize - 20, posY, 20, 20), "F"))
            m_handle = m_inputHandle;
    }
    void CheckhandleState(ref int posY)
    {
        if (EditorDB.MonsterStatDic == null)
            return;

        if(m_handle != 0)
        {
            if (EditorDB.MonsterStatDic.ContainsKey(m_handle))
            {
                if (m_stat == null || m_stat.Handle != m_handle)
                {
                    m_statEdit.Reset();
                    m_prefabEdit.Reset();
                    m_normalAttackEdit.Reset();

                    m_stat = EditorDB.MonsterStatDic[m_handle];
                    m_normalAttack = EditorDB.MonsterAttackDic[m_handle];
                    m_statEdit.IconTexture = Resources.Load<Texture2D>(m_stat.Icon);
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
            m_stat = null;
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
        foreach (Stat stat in EditorDB.MonsterStatDic.Values)
        {
            EditorGUI.LabelField(new Rect(0, posY, m_windowSize - 20, 20), "[" + stat.MonsterGrade + "] " + stat.Name + "</color>", m_guiStyle);
            if (GUI.Button(new Rect(m_windowSize - 20, posY, 20, 20), "F"))
            {
                m_handle = stat.Handle;
                m_inputHandle = stat.Handle;
                if (EditorDB.MonsterStatDic.ContainsKey(m_handle))
                {
                    if (m_stat == null || m_stat.Handle != m_handle)
                    {
                        m_statEdit.Reset();
                        m_prefabEdit.Reset();
                        m_normalAttackEdit.Reset();

                        m_stat = EditorDB.MonsterStatDic[m_handle];
                        m_normalAttack = EditorDB.MonsterAttackDic[m_handle];
                        m_statEdit.IconTexture = Resources.Load<Texture2D>(m_stat.Icon);
                        m_prefabEdit.Prefab = UnityEditor.PrefabUtility.LoadPrefabContents("Assets/AssetBundle_Character/Monster/" + m_stat.Path + ".prefab");
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
            if (EditorDB.MonsterStatDic.ContainsKey(m_handle))
                EditorDB.MonsterStatDic[m_handle] = m_stat;
            else
                EditorDB.MonsterStatDic.Add(m_handle, m_stat);
            if (EditorDB.MonsterAttackDic.ContainsKey(m_handle))
                EditorDB.MonsterAttackDic[m_handle] = m_normalAttack;
            else
                EditorDB.MonsterAttackDic.Add(m_handle, m_normalAttack);
        }

        posY += 30;
    }
    void DeleteHandle(ref int posY)
    {
        if (GUI.Button(new Rect(50, posY, m_windowSize - 100, 20), "현재핸들 제거"))
        {
            if (EditorDB.MonsterStatDic.ContainsKey(m_handle))
                EditorDB.MonsterStatDic.Remove(m_handle);
            if (EditorDB.MonsterAttackDic.ContainsKey(m_handle))
                EditorDB.MonsterAttackDic.Remove(m_handle);

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
            m_prefabEdit.Save("Assets/AssetBundle_Character/Monster/");
            UnityEditor.AssetDatabase.Refresh();
        }
    }
    void Save()
    {
        string Path = "Assets/AssetBundle_Database/DB_Monster.json";
        FileStream File = new FileStream(Path, FileMode.Create);
        int i = 0;
        WriteFileToString(File, "{\r");
        foreach (Stat stat in EditorDB.MonsterStatDic.Values)
        {
            WriteFileToString(File, "   \"" + i + "\":" + "{\r");
            WriteFileToString(File, "      \"Handle\" : " + "\"" + stat.Handle + "\",\r");
            WriteFileToString(File, "      \"MonsterGrade\" : " + "\"" + (int)stat.MonsterGrade + "\",\r");
            WriteFileToString(File, "      \"Name\" : " + "\"" + stat.Name + "\",\r");
            WriteFileToString(File, "      \"Explanation\" : " + "\"" + stat.Explanation + "\",\r");
            WriteFileToString(File, "      \"Icon\" : " + "\"" + stat.Icon + "\",\r");
            WriteFileToString(File, "      \"Path\" : " + "\"" + stat.Path + "\",\r");
            WriteFileToString(File, "      \"STR\" : " + "\"" + stat.STR + "\",\r");
            WriteFileToString(File, "      \"DEX\" : " + "\"" + stat.DEX + "\",\r");
            WriteFileToString(File, "      \"INT\" : " + "\"" + stat.INT + "\",\r");
            WriteFileToString(File, "      \"WIS\" : " + "\"" + stat.WIS + "\",\r");
            WriteFileToString(File, "      \"CON\" : " + "\"" + stat.CON + "\",\r");
            WriteFileToString(File, "      \"HP\" : " + "\"" + stat.HP + "\",\r");
            WriteFileToString(File, "      \"RecoveryHP\" : " + "\"" + stat.RecoveryHP + "\",\r");
            WriteFileToString(File, "      \"Resistance\" : " + "\"" + stat.Resistance + "\",\r");
            WriteFileToString(File, "      \"MP\" : " + "\"" + stat.MP + "\",\r");
            WriteFileToString(File, "      \"RecoveryMP\" : " + "\"" + stat.RecoveryMP + "\",\r");
            WriteFileToString(File, "      \"CoolTime\" : " + "\"" + stat.CoolTime + "\",\r");
            WriteFileToString(File, "      \"CriticalPro\" : " + "\"" + stat.CriticalPro + "\",\r");
            WriteFileToString(File, "      \"CriticalDamage\" : " + "\"" + stat.CriticalDamage + "\",\r");
            WriteFileToString(File, "      \"AttackSpeed\" : " + "\" 1 \",\r");
            WriteFileToString(File, "      \"MoveSpeed\" : " + "\"" + stat.MoveSpeed + "\",\r");
            WriteFileToString(File, "      \"MoveSpeedPro\" : " + "\"" + stat.MoveSpeedPro + "\",\r");
            WriteFileToString(File, "      \"AttackDamage\" : " + "\"" + stat.AttackDamage + "\",\r");
            WriteFileToString(File, "      \"AttackDamagePro\" : " + "\"" + stat.AttackDamagePro + "\",\r");
            WriteFileToString(File, "      \"Defence\" : " + "\"" + stat.Defence + "\",\r");
            WriteFileToString(File, "      \"DefencePro\" : " + "\"" + stat.DefencePro + "\",\r");
            WriteFileToString(File, "      \"SkillDamagePro\" : " + "\"" + stat.SkillDamagePro + "\",\r");
            WriteFileToString(File, "      \"EXP\" : " + "\"" + stat.EXP + "\",\r");
            if (stat.Reword.Count != 0)
            {
                string[] val = new string[stat.Reword.Count];
                for (int j = 0; j < stat.Reword.Count; ++j)
                    val[j] = stat.Reword[j].Handle + "," + stat.Reword[j].Value + "," + stat.Reword[j].Percent;
                WriteFileToString(File, "      \"Reword\" : " + "\"" + string.Join("/", val) + "\",\r");
            }
            if (stat.Pattern.Count != 0)
            {
                string[] patternVal = new string[stat.Pattern.Count];
                for (int j = 0; j < stat.Pattern.Count; ++j)
                {
                    patternVal[j] = (int)stat.Pattern[j].Type + "," + stat.Pattern[j].Order + "," + stat.Pattern[j].SkillKey;
                    if ((stat.Pattern[j].Type & EEnermyPatternType.HP) != 0)
                    {
                        patternVal[j] += "," + stat.Pattern[j].TargetHPLow + "," + stat.Pattern[j].TargetHPHigh;
                    }
                    else if ((stat.Pattern[j].Type & EEnermyPatternType.Time) != 0)
                    {
                        patternVal[j] += "," + (int)stat.Pattern[j].TargetTime;
                    }
                }
                WriteFileToString(File, "      \"Pattern\" : " + "\"" + string.Join("/", patternVal) + "\",\r");
            }
            WriteFileToString(File, "      \"Gold\" : " + "\"" + stat.Gold + "\"\r");
            if (i < EditorDB.MonsterStatDic.Count - 1)
                WriteFileToString(File, "   },\r\r");
            else
                WriteFileToString(File, "   }\r");
            ++i;
        }
        WriteFileToString(File, "}");
        File.Close();

        Path = "Assets/AssetBundle_Database/DB_MonsterAttack.json";
        File = new FileStream(Path, FileMode.Create);
        i = 0;
        WriteFileToString(File, "{\r");
        foreach (KeyValuePair<int, NormalAttack> attack in EditorDB.MonsterAttackDic)
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
            WriteFileToString(File, "      \"Angle\" : " + "\"" + Angle + "\",\r");
            WriteFileToString(File, "      \"HitTime\" : " + "\"" + HitTime + "\",\r");
            WriteFileToString(File, "      \"Width\" : " + "\"" + Width + "\"\r");

            if (i < EditorDB.MonsterAttackDic.Count - 1)
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
