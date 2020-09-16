using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using SimpleJSON;
using System.Linq;

public class ItemEditor : EditorWindow
{
    static EditorWindow m_window;
    static GUIStyle m_guiStyle;
    static float m_windowSize;

    Item_Base m_item;
    int m_handle;
    int m_inputHandle;
    EItemType m_type = EItemType.Other;
    EItemType m_currShowType;
    Item_Base[] Items;
    public Texture2D IconTexture;
    string[] m_toolbars = { "Information", "Interface" };
    int m_toolbarIndex;

    [MenuItem("CustomEditor/Item Editor")]
    public static void ShowWindow()
    {
        m_window = GetWindow(typeof(ItemEditor));
        m_window.minSize = new Vector2(400, 800);
        m_window.maxSize = new Vector2(400, 800);
        m_windowSize = m_window.maxSize.x;
        m_guiStyle = new GUIStyle();
        m_guiStyle.richText = true;
        EditorDB.Init();
    }
    private void OnGUI()
    {
        int posY = 0;
        InputHandle(ref posY);
        CheckhandleState(ref posY);

        if (m_handle == 0)
            return;

        GUI.Label(new Rect(0, posY, m_windowSize / 2, 20), "ItemType");
        m_type = (EItemType)EditorGUI.EnumPopup(new Rect(m_windowSize / 2, posY, m_windowSize / 2, 20), m_type);

        if (m_item == null || m_item.Type != m_type)
        {
            switch (m_type)
            {
                case EItemType.Other:
                    m_item = new Item_Other();
                    break;
                case EItemType.Potion:
                    m_item = new Item_Potion();
                    break;
                case EItemType.Scroll:
                    m_item = new Item_Scroll();
                    break;
                case EItemType.Weapon:
                case EItemType.Armor:
                case EItemType.Gloves:
                case EItemType.Shoes:
                case EItemType.Ring:
                case EItemType.Necklace:
                    m_item = new Item_Equipment();
                    IItemEquipment Stat = m_item as IItemEquipment;
                    if (Stat.Stat == null)
                        Stat.Stat = new Stat();
                    break;
            }

            m_item.Handle = m_handle;
            m_item.Type = m_type;
        }
        posY += 30;
        m_toolbarIndex = GUI.Toolbar(new Rect(0, posY, m_windowSize, 20), m_toolbarIndex, m_toolbars);
        posY += 30;
        switch (m_toolbarIndex)
        {
            case 0:
                EditorGUI.LabelField(new Rect(0, posY, m_windowSize / 2, 20), "Name:");
                m_item.Name = EditorGUI.TextField(new Rect(m_windowSize / 2, posY, m_windowSize / 2, 20), m_item.Name);
                posY += 20;
                EditorGUI.LabelField(new Rect(0, posY, m_windowSize / 2, 20), "Explanation:");
                m_item.Explanation = EditorGUI.TextField(new Rect(m_windowSize / 2, posY, m_windowSize / 2, 20), m_item.Explanation);
                posY += 20;
                EditorGUI.LabelField(new Rect(0, posY, m_windowSize / 2, 20), "Rarity:");
                m_item.Rarity = (EItemRarity)EditorGUI.EnumPopup(new Rect(m_windowSize / 2, posY, m_windowSize / 2, 20), m_item.Rarity);
                posY += 20;
                EditorGUI.LabelField(new Rect(0, posY, m_windowSize / 2, 20), "Price:");
                m_item.Price = EditorGUI.IntField(new Rect(m_windowSize / 2, posY, m_windowSize / 2, 20), m_item.Price);
                posY += 20;
                IconTexture = (Texture2D)EditorGUI.ObjectField(new Rect(0, posY + 20, 100, 100), IconTexture, typeof(Texture2D), false);
                if (IconTexture != null)
                    m_item.Icon = AssetDatabase.GetAssetPath(IconTexture).Substring(17, AssetDatabase.GetAssetPath(IconTexture).Length - 21);
                break;
            case 1:
                IItemLevel Level = m_item as IItemLevel;
                if (Level != null)
                {
                    EditorGUI.LabelField(new Rect(0, posY, m_windowSize / 2, 20), "Level:");
                    Level.Level = EditorGUI.IntField(new Rect(m_windowSize / 2, posY, m_windowSize / 2, 20), Level.Level);
                    posY += 30;
                }
                IItemActive Active = m_item as IItemActive;
                if (Active != null)
                {
                    EditorGUI.LabelField(new Rect(0, posY, m_windowSize / 2, 20), "ActiveHandle:");
                    Active.ActionHandle = (EItemActiveType)EditorGUI.EnumPopup(new Rect(m_windowSize / 2, posY, m_windowSize / 2, 20), Active.ActionHandle);
                    posY += 20;
                    EditorGUI.LabelField(new Rect(0, posY, m_windowSize / 2, 20), "ActiveValue:");
                    Active.ActionValue = EditorGUI.TextField(new Rect(m_windowSize / 2, posY, m_windowSize / 2, 20), Active.ActionValue);
                    posY += 30;
                    EditorGUI.LabelField(new Rect(0, posY, m_windowSize / 2, 20), "CoolTime:");
                    Active.CoolTime = EditorGUI.FloatField(new Rect(m_windowSize / 2, posY, m_windowSize / 2, 20), Active.CoolTime);
                    posY += 30;
                }
                IItemEquipment Stat = m_item as IItemEquipment;
                if (Stat != null)
                {
                    EditorGUI.LabelField(new Rect(0, posY, m_windowSize / 2, 20), "STR", m_guiStyle);
                    Stat.Stat.STR = EditorGUI.FloatField(new Rect(m_windowSize / 2, posY, m_windowSize / 2, 20), Stat.Stat.STR);
                    posY += 20;
                    EditorGUI.LabelField(new Rect(0, posY, m_windowSize / 2, 20), "DEX", m_guiStyle);
                    Stat.Stat.DEX = EditorGUI.FloatField(new Rect(m_windowSize / 2, posY, m_windowSize / 2, 20), Stat.Stat.DEX);
                    posY += 20;
                    EditorGUI.LabelField(new Rect(0, posY, m_windowSize / 2, 20), "INT", m_guiStyle);
                    Stat.Stat.INT = EditorGUI.FloatField(new Rect(m_windowSize / 2, posY, m_windowSize / 2, 20), Stat.Stat.INT);
                    posY += 20;
                    EditorGUI.LabelField(new Rect(0, posY, m_windowSize / 2, 20), "WIS", m_guiStyle);
                    Stat.Stat.WIS = EditorGUI.FloatField(new Rect(m_windowSize / 2, posY, m_windowSize / 2, 20), Stat.Stat.WIS);
                    posY += 20;
                    EditorGUI.LabelField(new Rect(0, posY, m_windowSize / 2, 20), "CON", m_guiStyle);
                    Stat.Stat.CON = EditorGUI.FloatField(new Rect(m_windowSize / 2, posY, m_windowSize / 2, 20), Stat.Stat.CON);
                    posY += 30;
                    EditorGUI.LabelField(new Rect(0, posY, m_windowSize / 2, 20), "HP", m_guiStyle);
                    Stat.Stat.HP = EditorGUI.FloatField(new Rect(m_windowSize / 2, posY, m_windowSize / 2, 20), Stat.Stat.HP);
                    posY += 20;
                    EditorGUI.LabelField(new Rect(0, posY, m_windowSize / 2, 20), "RecoveryHP", m_guiStyle);
                    Stat.Stat.RecoveryHP = EditorGUI.FloatField(new Rect(m_windowSize / 2, posY, m_windowSize / 2, 20), Stat.Stat.RecoveryHP);
                    posY += 20;
                    EditorGUI.LabelField(new Rect(0, posY, m_windowSize / 2, 20), "MP", m_guiStyle);
                    Stat.Stat.MP = EditorGUI.FloatField(new Rect(m_windowSize / 2, posY, m_windowSize / 2, 20), Stat.Stat.MP);
                    posY += 20;
                    EditorGUI.LabelField(new Rect(0, posY, m_windowSize / 2, 20), "RecoveryMP", m_guiStyle);
                    Stat.Stat.RecoveryMP = EditorGUI.FloatField(new Rect(m_windowSize / 2, posY, m_windowSize / 2, 20), Stat.Stat.RecoveryMP);
                    posY += 30;

                    EditorGUI.LabelField(new Rect(0, posY, m_windowSize / 2, 20), "AttackDamage", m_guiStyle);
                    Stat.Stat.AttackDamage = EditorGUI.FloatField(new Rect(m_windowSize / 2, posY, m_windowSize / 2, 20), Stat.Stat.AttackDamage);
                    posY += 20;
                    EditorGUI.LabelField(new Rect(0, posY, m_windowSize / 2, 20), "AttackDamagePro", m_guiStyle);
                    Stat.Stat.AttackDamagePro = EditorGUI.FloatField(new Rect(m_windowSize / 2, posY, m_windowSize / 2, 20), Stat.Stat.AttackDamagePro);
                    posY += 20;
                    EditorGUI.LabelField(new Rect(0, posY, m_windowSize / 2, 20), "CriticalPro", m_guiStyle);
                    Stat.Stat.CriticalPro = EditorGUI.FloatField(new Rect(m_windowSize / 2, posY, m_windowSize / 2, 20), Stat.Stat.CriticalPro);
                    posY += 20;
                    EditorGUI.LabelField(new Rect(0, posY, m_windowSize / 2, 20), "CriticalDamage", m_guiStyle);
                    Stat.Stat.CriticalDamage = EditorGUI.FloatField(new Rect(m_windowSize / 2, posY, m_windowSize / 2, 20), Stat.Stat.CriticalDamage);
                    posY += 20;
                    EditorGUI.LabelField(new Rect(0, posY, m_windowSize / 2, 20), "SkillDamagePro", m_guiStyle);
                    Stat.Stat.SkillDamagePro = EditorGUI.FloatField(new Rect(m_windowSize / 2, posY, m_windowSize / 2, 20), Stat.Stat.SkillDamagePro);
                    posY += 20;
                    EditorGUI.LabelField(new Rect(0, posY, m_windowSize / 2, 20), "CoolTime", m_guiStyle);
                    Stat.Stat.CoolTime = EditorGUI.FloatField(new Rect(m_windowSize / 2, posY, m_windowSize / 2, 20), Stat.Stat.CoolTime);
                    posY += 30;

                    EditorGUI.LabelField(new Rect(0, posY, m_windowSize / 2, 20), "Defence", m_guiStyle);
                    Stat.Stat.Defence = EditorGUI.FloatField(new Rect(m_windowSize / 2, posY, m_windowSize / 2, 20), Stat.Stat.Defence);
                    posY += 20;
                    EditorGUI.LabelField(new Rect(0, posY, m_windowSize / 2, 20), "DefencePro", m_guiStyle);
                    Stat.Stat.DefencePro = EditorGUI.FloatField(new Rect(m_windowSize / 2, posY, m_windowSize / 2, 20), Stat.Stat.DefencePro);
                    posY += 20;
                    EditorGUI.LabelField(new Rect(0, posY, m_windowSize / 2, 20), "Resistance", m_guiStyle);
                    Stat.Stat.Resistance = EditorGUI.FloatField(new Rect(m_windowSize / 2, posY, m_windowSize / 2, 20), Stat.Stat.Resistance);
                    posY += 20;
                    EditorGUI.LabelField(new Rect(0, posY, m_windowSize / 2, 20), "MoveSpeed", m_guiStyle);
                    Stat.Stat.MoveSpeed = EditorGUI.FloatField(new Rect(m_windowSize / 2, posY, m_windowSize / 2, 20), Stat.Stat.MoveSpeed);
                    posY += 20;
                    EditorGUI.LabelField(new Rect(0, posY, m_windowSize / 2, 20), "MoveSpeedPro", m_guiStyle);
                    Stat.Stat.MoveSpeedPro = EditorGUI.FloatField(new Rect(m_windowSize / 2, posY, m_windowSize / 2, 20), Stat.Stat.MoveSpeedPro);
                    posY += 20;
                }
                break;
        }
        posY += 30;

        posY = (int)m_window.maxSize.y - 110;
        ApplyHandle(ref posY);
        DeleteHandle(ref posY);
        Save(ref posY);
    }
    void InputHandle(ref int posY)
    {
        if (GUI.Button(new Rect(0, posY, m_windowSize, 20), "New handle to last number"))
        {
            if (m_handle != m_inputHandle)
            {
                m_item = null;
                IconTexture = null;
            }

            m_handle = EditorDB.ItemDic.Count + 1;
            m_inputHandle = m_handle;
            m_type = EItemType.Other;
        }
        posY += 20;
        m_inputHandle = EditorGUI.IntField(new Rect(m_windowSize / 2 - 20, posY, m_windowSize / 2, 20), m_inputHandle);
        if (GUI.Button(new Rect(m_windowSize - 20, posY, 20, 20), "F"))
        {
            if (m_handle != m_inputHandle)
                m_item = null;

            m_handle = m_inputHandle;

            if (EditorDB.ItemDic.ContainsKey(m_handle))
            {
                m_type = EditorDB.ItemDic[m_handle].Type;
                IconTexture = Resources.Load<Texture2D>(EditorDB.ItemDic[m_handle].Icon);
            }
            else
            {
                m_type = EItemType.Other;
                IconTexture = null;
            }
        }
    }
    void CheckhandleState(ref int posY)
    {
        if (EditorDB.ItemDic == null)
            return;

        if(m_handle != 0)
        {
            if (EditorDB.ItemDic.ContainsKey(m_handle))
            {
                if (m_item == null || m_item.Handle != m_handle)
                {
                    m_item = EditorDB.ItemDic[m_handle];
                    m_type = m_item.Type;
                }
                EditorGUI.LabelField(new Rect(0, posY, m_windowSize, 20), "<color=red>Handle: </color>", m_guiStyle);
                posY += 20;
                EditorGUI.LabelField(new Rect(0, posY, m_windowSize, 20), "<color=red>Fixed Character</color>", m_guiStyle);
            }
            else
            {
                EditorGUI.LabelField(new Rect(0, posY, m_windowSize, 20), "<color=blue>Handle: </color>", m_guiStyle);
                posY += 20;
                EditorGUI.LabelField(new Rect(0, posY, m_windowSize, 20), "<color=blue>New Character</color>", m_guiStyle);
            }
        }
        else
        {
            EditorGUI.LabelField(new Rect(0, posY, m_windowSize, 20), "<color=white>Handle: </color>", m_guiStyle);
            posY += 20;
            EditorGUI.LabelField(new Rect(0, posY, m_windowSize, 20), "<color=white>Please input handle</color>.", m_guiStyle);
            posY += 30;
            ShowItemType(ref posY);
        }

        posY += 30;
    }
    void ShowItemType(ref int posY)
    {
        EditorGUI.LabelField(new Rect(0, posY, m_windowSize/2, 20), "<color=white>SearchType: </color>", m_guiStyle);
        EItemType type = (EItemType)EditorGUI.EnumPopup(new Rect(m_windowSize / 2, posY, m_windowSize/2, 20), m_currShowType);
        posY += 20;
        if(Items == null || m_currShowType != type)
        {
            m_currShowType = type;
            var ItemList = from item in EditorDB.ItemDic
                          where item.Value.Type == type
                          select item.Value;

            Items = ItemList.ToArray();
        }
        if(Items != null)
        {
            for(int i =0; i<Items.Length; ++i)
            {
                string output = "";
                switch(Items[i].Rarity)
                {
                    case EItemRarity.Normal:
                        output = "<color=white>";
                        break;
                    case EItemRarity.Magic:
                        output = "<color=cyan>";
                        break;
                    case EItemRarity.Relic:
                        output = "<color=blue>";
                        break;
                    case EItemRarity.Unique:
                        output = "<color=magenta>";
                        break;
                    case EItemRarity.Legend:
                        output = "<color=yellow>";
                        break;
                    case EItemRarity.Infinity:
                        output = "<color=red>";
                        break;
                }
                EditorGUI.LabelField(new Rect(0, posY, m_windowSize -20, 20), output+"[" + Items[i].Handle + "] " + Items[i].Name + "</color>", m_guiStyle);
                if (GUI.Button(new Rect(m_windowSize - 20, posY, 20, 20), "F"))
                {
                    if (m_handle != Items[i].Handle)
                        m_item = null;

                    m_handle = Items[i].Handle;
                    m_inputHandle = Items[i].Handle;
                    if (EditorDB.ItemDic.ContainsKey(m_handle))
                    {
                        m_item = EditorDB.ItemDic[m_handle];
                        m_type = EditorDB.ItemDic[m_handle].Type;
                        IconTexture = Resources.Load<Texture2D>(EditorDB.ItemDic[m_handle].Icon);
                    }
                    else
                    {
                        m_type = EItemType.Other;
                        IconTexture = null;
                    }
                    return;
                }
                posY += 20;
            }
        }
    }
    void ApplyHandle(ref int posY)
    {
        if (GUI.Button(new Rect(50, posY, m_windowSize - 100, 20), "수정사항 핸들값에 적용"))
        {
            if (EditorDB.ItemDic.ContainsKey(m_handle))
                EditorDB.ItemDic[m_handle] = m_item;
            else
                EditorDB.ItemDic.Add(m_handle, m_item);
        }

        posY += 30;
    }
    void DeleteHandle(ref int posY)
    {
        if (GUI.Button(new Rect(50, posY, m_windowSize - 100, 20), "현재핸들 제거"))
        {
            if (EditorDB.ItemDic.ContainsKey(m_handle))
            {
                EditorDB.ItemDic.Remove(m_handle);
                m_item = null;
                m_handle = 0;
                m_inputHandle = 0;
            }
        }

        posY += 30;
    }
    void Save(ref int posY)
    {
        if (GUI.Button(new Rect(50, posY, m_windowSize - 100, 20), "DB 저장"))
        {
            Save();
            UnityEditor.AssetDatabase.Refresh();
        }
    }
    void Save()
    {
        string Path = "Assets/AssetBundle_Database/DB_Item.json";
        FileStream File = new FileStream(Path, FileMode.Create);
        int i = 0;
        WriteFileToString(File, "{\r");
        foreach (KeyValuePair<int, Item_Base> Item in EditorDB.ItemDic)
        {
            WriteFileToString(File, "   \"" + i + "\":" + "{\r");
            WriteFileToString(File, "      \"Handle\" : " + "\"" + Item.Value.Handle + "\",\r");
            WriteFileToString(File, "      \"Name\" : " + "\"" + Item.Value.Name + "\",\r");
            WriteFileToString(File, "      \"Explanation\" : " + "\"" + Item.Value.Explanation + "\",\r");
            WriteFileToString(File, "      \"Price\" : " + "\"" + Item.Value.Price + "\",\r");
            WriteFileToString(File, "      \"Type\" : " + "\"" + (int)Item.Value.Type + "\",\r");
            WriteFileToString(File, "      \"Rarity\" : " + "\"" + (int)Item.Value.Rarity + "\",\r");
            IItemLevel Level = Item.Value as IItemLevel;
            if (Level != null)
            {
                WriteFileToString(File, "      \"Level\" : " + "\"" + Level.Level + "\",\r");
            }
            IItemActive Active = Item.Value as IItemActive;
            if (Active != null)
            {
                WriteFileToString(File, "      \"ActionHandle\" : " + "\"" + (int)Active.ActionHandle + "\",\r");
                WriteFileToString(File, "      \"ActionValue\" : " + "\"" + Active.ActionValue + "\",\r");
                WriteFileToString(File, "      \"CoolTime\" : " + "\"" + Active.CoolTime + "\",\r");
            }
            IItemEquipment Stat = Item.Value as IItemEquipment;
            if(Stat != null)
            {
                WriteFileToString(File, "      \"STR\" : " + "\"" + Stat.Stat.STR + "\",\r");
                WriteFileToString(File, "      \"DEX\" : " + "\"" + Stat.Stat.DEX + "\",\r");
                WriteFileToString(File, "      \"INT\" : " + "\"" + Stat.Stat.INT + "\",\r");
                WriteFileToString(File, "      \"WIS\" : " + "\"" + Stat.Stat.WIS + "\",\r");
                WriteFileToString(File, "      \"CON\" : " + "\"" + Stat.Stat.CON + "\",\r");
                WriteFileToString(File, "      \"HP\" : " + "\"" + Stat.Stat.HP + "\",\r");
                WriteFileToString(File, "      \"RecoveryHP\" : " + "\"" + Stat.Stat.RecoveryHP + "\",\r");
                WriteFileToString(File, "      \"Resistance\" : " + "\"" + Stat.Stat.Resistance + "\",\r");
                WriteFileToString(File, "      \"MP\" : " + "\"" + Stat.Stat.MP + "\",\r");
                WriteFileToString(File, "      \"RecoveryMP\" : " + "\"" + Stat.Stat.RecoveryMP + "\",\r");
                WriteFileToString(File, "      \"CoolTime\" : " + "\"" + Stat.Stat.CoolTime + "\",\r");
                WriteFileToString(File, "      \"CriticalPro\" : " + "\"" + Stat.Stat.CriticalPro + "\",\r");
                WriteFileToString(File, "      \"CriticalDamage\" : " + "\"" + Stat.Stat.CriticalDamage + "\",\r");
                WriteFileToString(File, "      \"AttackSpeed\" : " + "\"" + Stat.Stat.AttackSpeed + "\",\r");
                WriteFileToString(File, "      \"MoveSpeed\" : " + "\"" + Stat.Stat.MoveSpeed + "\",\r");
                WriteFileToString(File, "      \"MoveSpeedPro\" : " + "\"" + Stat.Stat.MoveSpeedPro + "\",\r");
                WriteFileToString(File, "      \"AttackDamage\" : " + "\"" + Stat.Stat.AttackDamage + "\",\r");
                WriteFileToString(File, "      \"AttackDamagePro\" : " + "\"" + Stat.Stat.AttackDamagePro + "\",\r");
                WriteFileToString(File, "      \"Defence\" : " + "\"" + Stat.Stat.Defence + "\",\r");
                WriteFileToString(File, "      \"DefencePro\" : " + "\"" + Stat.Stat.DefencePro + "\",\r");
                WriteFileToString(File, "      \"SkillDamagePro\" : " + "\"" + Stat.Stat.SkillDamagePro + "\",\r");
            }

            WriteFileToString(File, "      \"Icon\" : " + "\"" + Item.Value.Icon + "\"\r");

            if (i < EditorDB.ItemDic.Count - 1)
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
}
