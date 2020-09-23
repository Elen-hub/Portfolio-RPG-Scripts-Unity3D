using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using SimpleJSON;
using UnityEngine.AI;
using System.Linq;

public delegate void Del_Selection(int handle);
public class ItemProduceEditor : EditorWindow
{
    static EditorWindow m_window;
    static GUIStyle m_guiStyle = new GUIStyle();
    static float m_windowSize;
    int m_handle = -1;

    ItemProduceSelector m_itemProduceSelector;

    static Texture2D m_outItemTexture;
    static Texture2D[] m_inItemTexture = new Texture2D[8];

    [MenuItem("CustomEditor/ItemProduce Editor")]
    public static void ShowWindow()
    {
        m_window = GetWindow(typeof(ItemProduceEditor));
        m_window.minSize = new Vector2(400, 400);
        m_window.maxSize = new Vector2(400, 400);
        m_windowSize = m_window.maxSize.x;
        m_guiStyle.richText = true;
        m_outItemTexture = new Texture2D(100, 100);
        for (int i = 0; i < m_inItemTexture.Length; ++i)
            m_inItemTexture[i] = new Texture2D(50, 50);
        EditorDB.Init();
    }
    private void OnGUI()
    {
        float yPos = 0;
        int handle = InputLine(ref yPos);
        if (handle != m_handle && EditorDB.ItemProduceDic.ContainsKey(handle))
            ChangeData(handle);

        if (!EditorDB.ItemProduceDic.ContainsKey(m_handle))
            return;

        InpuData(ref yPos);

        m_window.minSize = new Vector2(m_windowSize, yPos+100);
    }
    int InputLine(ref float yPos)
    {
        float subjectLength = 80f;
        float selectorOpenBtnLength = 60f;
        float saveBtnLength = 40f;
        float newHandlerBtnLength = 40f;
        EditorGUI.LabelField(new Rect(0, 0, subjectLength, 20), "Handle: ");
        int selectionHandle = EditorGUI.IntField(new Rect(subjectLength, 0, m_windowSize-subjectLength - selectorOpenBtnLength - saveBtnLength - newHandlerBtnLength, 20f), m_handle);
        if (GUI.Button(new Rect(m_windowSize - newHandlerBtnLength - selectorOpenBtnLength - saveBtnLength, 0, newHandlerBtnLength, 20), "New"))
            NewHandle();
        if (GUI.Button(new Rect(m_windowSize - newHandlerBtnLength - selectorOpenBtnLength, 0, saveBtnLength, 20), "Save"))
            Save();
        if (GUI.Button(new Rect(m_windowSize - selectorOpenBtnLength, 0, selectorOpenBtnLength, 20), "Find"))
        {
            ItemProduceSelector.ShowWindow(SetHandle, m_window.position.position);
        }

        yPos += 30;
        return selectionHandle;
    }
    void InpuData(ref float yPos)
    {
        ItemProduceFormula produceFomula = EditorDB.ItemProduceDic[m_handle];

        EditorGUI.LabelField(new Rect(0, yPos, m_windowSize, 20), "<color=white>OutItem-----------------------------------------------------------------------------</color>", m_guiStyle);
        yPos += 25;

        string output = "<color=white>" + EditorDB.ItemDic[produceFomula.OutItem.Handle].Name + "</color>";
        switch (EditorDB.ItemDic[produceFomula.OutItem.Handle].Rarity)
        {
            case EItemRarity.Normal:
                output += "<color=white>";
                break;
            case EItemRarity.Magic:
                output += "<color=cyan>";
                break;
            case EItemRarity.Relic:
                output += "<color=blue>";
                break;
            case EItemRarity.Unique:
                output += "<color=magenta>";
                break;
            case EItemRarity.Legend:
                output += "<color=yellow>";
                break;
            case EItemRarity.Infinity:
                output += "<color=red>";
                break;
        }
        output += " (" + ParseLib.GetRairityKorConvert(EditorDB.ItemDic[produceFomula.OutItem.Handle].Rarity) + ") </color>";

        EditorGUI.LabelField(new Rect(150, yPos, m_windowSize-160, 20), output, m_guiStyle);
        yPos += 25;


        GUI.DrawTexture(new Rect(20, yPos, 100, 100), m_outItemTexture);
        EditorGUI.LabelField(new Rect(150, yPos, 60, 20), "Handle: ");
        produceFomula.OutItem.Handle = EditorGUI.IntField(new Rect(210, yPos, m_windowSize - 235, 20), produceFomula.OutItem.Handle);
        yPos += 25;

        EditorGUI.LabelField(new Rect(150, yPos, 60, 20), "Number: ");
        produceFomula.OutItem.Number = EditorGUI.IntField(new Rect(210, yPos, m_windowSize - 235, 20), produceFomula.OutItem.Number);
        yPos += 25;

        EditorGUI.LabelField(new Rect(150, yPos, 100, 20), "Type: ");
        produceFomula.ItemProduceType = (EItemProduceType)EditorGUI.EnumPopup(new Rect(260, yPos, m_windowSize - 285, 20), produceFomula.ItemProduceType);
        yPos += 25;

        EditorGUI.LabelField(new Rect(150, yPos, 100, 20), "Level: ");
        produceFomula.Level = EditorGUI.IntField(new Rect(260, yPos, m_windowSize - 285, 20), produceFomula.Level);
        yPos += 25;

        EditorGUI.LabelField(new Rect(150, yPos, 100, 20), "Gold: ");
        produceFomula.Gold = EditorGUI.IntField(new Rect(260, yPos, m_windowSize - 285, 20), produceFomula.Gold);
        yPos += 25;

        EditorGUI.LabelField(new Rect(150, yPos, 100, 20), "ProduceTime: ");
        produceFomula.ProduceTime = EditorGUI.FloatField(new Rect(260, yPos, m_windowSize - 285, 20), produceFomula.ProduceTime);
        yPos += 25;

        EditorGUI.LabelField(new Rect(150, yPos, 100, 20), "CoolTime: ");
        produceFomula.CoolTime = EditorGUI.FloatField(new Rect(260, yPos, m_windowSize - 285, 20), produceFomula.CoolTime);
        yPos += 25;

        EditorGUI.LabelField(new Rect(0, yPos, m_windowSize, 20), "<color=white>Material-----------------------------------------------------------------------------</color>", m_guiStyle);
        yPos += 25;

        float xPos = 0;
        for(int i = 0; i<m_inItemTexture.Length; ++i)
        {
            if (i == 4)
            {
                yPos += 110;
                xPos = 0;
            }
            if (produceFomula.InItem.Count > i)
            {
                int inHandle = produceFomula.InItem[i].Handle;
                int inNumber = produceFomula.InItem[i].Number;
                EditorGUI.LabelField(new Rect(xPos, yPos + 55f, 100, 20), EditorDB.ItemDic[produceFomula.InItem[i].Handle].Name);
                EditorGUI.LabelField(new Rect(xPos, yPos + 75f, 30, 20), "개수: ");
                inNumber = EditorGUI.IntField(new Rect(35+ xPos, yPos + 75f, 45, 20), produceFomula.InItem[i].Number);
                if(GUI.Button(new Rect(xPos, yPos, 50, 50), m_inItemTexture[i]))
                    ItemSelector.ShowWindow((int handle) => {
                        for(int j =0; j<produceFomula.InItem.Count; ++j)
                        {
                            if(produceFomula.InItem[j].Handle == inHandle)
                            {
                                produceFomula.InItem.RemoveAt(j);
                                produceFomula.InItem.Insert(j, new ProduceMaterialHandler(handle, 0));
                                LoadTexture();
                                break;
                            }
                        }
                    }, m_window.position.position);

                if (inNumber != produceFomula.InItem[i].Number)
                {
                    produceFomula.InItem.RemoveAt(i);
                    produceFomula.InItem.Insert(i, new ProduceMaterialHandler(inHandle, inNumber));
                }
                if (GUI.Button(new Rect(xPos + 50, yPos, 20, 20), "X"))
                {
                    produceFomula.InItem.RemoveAt(i);
                    LoadTexture();
                }
            }
            else if(produceFomula.InItem.Count == i)
            {
                EditorGUI.LabelField(new Rect(xPos, yPos + 55f, 100, 20), "아이템 추가");
                if (GUI.Button(new Rect(10 + xPos, yPos, 50, 50), m_inItemTexture[i]))
                {
                    ItemSelector.ShowWindow((int handle) => 
                    {
                        produceFomula.InItem.Add(new ProduceMaterialHandler(handle, 0));
                        LoadTexture();
                    }, m_window.position.position);
                }
            }
            xPos += m_windowSize / 4 + 10;
        }
    }
    void ChangeData(int changeHandle)
    {
        m_outItemTexture.LoadImage(null);
        ItemProduceFormula produceFomula = EditorDB.ItemProduceDic[changeHandle];
        m_handle = changeHandle;
        m_outItemTexture.LoadImage(File.ReadAllBytes("Assets/Resources/" + EditorDB.ItemDic[produceFomula.OutItem.Handle].Icon + ".png"));
        LoadTexture();
    }
    void LoadTexture()
    {
        ItemProduceFormula produceFomula = EditorDB.ItemProduceDic[m_handle];
        m_outItemTexture.LoadImage(File.ReadAllBytes("Assets/Resources/" + EditorDB.ItemDic[produceFomula.OutItem.Handle].Icon + ".png"));
        for (int i = 0; i < m_inItemTexture.Length; ++i)
        {
            if (produceFomula.InItem.Count > i)
                m_inItemTexture[i].LoadImage(File.ReadAllBytes("Assets/Resources/" + EditorDB.ItemDic[produceFomula.InItem[i].Handle].Icon + ".png"));
            else
                m_inItemTexture[i] = new Texture2D(50, 50);
        }
    }
    void NewHandle()
    {
        ItemSelector.ShowWindow((int handle) =>
        {
            int outHandle;
            if (EditorDB.ItemProduceDic.Count == 0)
                outHandle = 0;
            else
                outHandle = EditorDB.ItemProduceDic.Last().Key + 1;
            ItemProduceFormula newFormula = new ItemProduceFormula();
            newFormula.Handle = outHandle;
            newFormula.OutItem = new ProduceMaterialHandler(handle, 0);
            EditorDB.ItemProduceDic.Add(outHandle, newFormula);
            SetHandle(outHandle);
        }, m_window.position.size);
    }
    void SetHandle(int handle)
    {
        m_handle = handle;
        ChangeData(handle);
    }
    void Save()
    {
        string Path = "Assets/AssetBundle_Database/DB_ItemProduce.json";
        FileStream File = new FileStream(Path, FileMode.Create);
        int i = 0;
        WriteFileToString(File, "{\r");
        foreach (ItemProduceFormula item in EditorDB.ItemProduceDic.Values)
        {
            WriteFileToString(File, "   \"" + i + "\":" + "{\r");
            WriteFileToString(File, "      \"Handle\" : " + "\"" + item.Handle + "\",\r");
            WriteFileToString(File, "      \"OutItemHandler\" : " + "\"" + item.OutItem.Handle + "," + item.OutItem.Number + "\",\r");
            string[] inItemHandler = new string[item.InItem.Count];
            for(int j =0; j<item.InItem.Count; ++j)
                inItemHandler[j] = item.InItem[j].Handle + "," + item.InItem[j].Number;
            WriteFileToString(File, "      \"InItemHandler\" : " + "\"" + string.Join("/", inItemHandler) + "\",\r");
            WriteFileToString(File, "      \"Type\" : " + "\"" + item.Gold + "\",\r");
            WriteFileToString(File, "      \"Level\" : " + "\"" + item.Level + "\",\r");
            WriteFileToString(File, "      \"Gold\" : " + "\"" + item.Gold + "\",\r");
            WriteFileToString(File, "      \"ProduceTime\" : " + "\"" + item.ProduceTime + "\",\r");
            WriteFileToString(File, "      \"CoolTime\" : " + "\"" + item.CoolTime + "\"\r");

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
    void WriteFileToString(FileStream file, string str)
    {
        byte[] bytes = System.Text.Encoding.UTF8.GetBytes(str);
        file.Write(bytes, 0, bytes.Length);
    }
}
