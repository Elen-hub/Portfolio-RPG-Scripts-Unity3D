using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EQuestType
{
    None,
    Normal,
    Main,
    Repeat,
    Special
}
public enum EQuestClearType
{
    Talk,
    Collect,
    Hunt,
    Move,
}
public class SQuestReword
{
    public SQuestReword()
    {

    }
    public SQuestReword(int handle, int value)
    {
        Handle = handle;
        Value = value;
    }
    public int Handle;
    public int Value;
}
public class QuestContent
{
    public int Handle;
    public int NPCHandle;
    public int Level;
    public string ContentName;

    public List<string> Scripts = new List<string>();

    public EQuestClearType Type;
    public int ClearNPCHandle;
    public int ClearHandle;
    public int ClearValue;
    public int CurrValue;

    public List<SQuestReword> Reword = new List<SQuestReword>();
    public int RewordGold;
    public int RewordEXP;
    public string NoneClearScript;
    public List<string> ClearScripts = new List<string>();
}
public class Quest
{
    public int Handle;
    public int Chapter;
    public string Name;
    public bool Accept;
    public bool Clear;
    public EQuestType Type;
    public string ClearDate;
    public Dictionary<int , QuestContent> QuestDic = new Dictionary<int, QuestContent>();
    public QuestContent CurrQuest { get { if (QuestDic.ContainsKey(Chapter)) return QuestDic[Chapter]; else return null; } }
    public bool IsAccept(int level)
    {
        if (CurrQuest != null)
        {
            if (CurrQuest.Level > level)
                return false;

            return true;
        }
        else return false;
    }
    public void Delete()
    {
        Accept = false;
        CurrQuest.CurrValue = 0;
    }
}
