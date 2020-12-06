using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParseLib : MonoBehaviour
{
    public static void SetPlayerData(ref Player player, int uniqueID, EAllyType type, string infoData, string statusData, string equipData)
    {
        Vector3 pos = Vector3.zero;
        Vector3 angle = Vector3.zero;
        string[] info = infoData.Split(',');
        string[] status = statusData.Split(',');
        string[] equip = equipData.Split('/');
        int STR = 0;
        int DEX = 0;
        int INT = 0;
        int WIS = 0;
        int CON = 0;
        if (infoData != "")
        {
            player.Handle = int.Parse(info[0]);
            player.Name = info[1];
            player.Level = int.Parse(info[2]);
            player.Awakening = int.Parse(info[3]);
            player.Exp = int.Parse(info[4]);
            STR = int.Parse(info[5]);
            DEX = int.Parse(info[6]);
            INT = int.Parse(info[7]);
            WIS = int.Parse(info[8]);
            CON = int.Parse(info[9]);
            player.StatPoint = int.Parse(info[10]);
            player.SkillPoint = int.Parse(info[11]);
            player.Gold = int.Parse(info[12]);
            player.MaxInventoryCount = int.Parse(info[13]);
        }
        if (statusData != "")
        {
            player.CurrMapHandle = int.Parse(status[0]);
            pos = new Vector3(float.Parse(status[1]), float.Parse(status[2]), float.Parse(status[3]));
            if(status.Length > 4)
                angle = new Vector3(float.Parse(status[4]), float.Parse(status[5]), float.Parse(status[6]));
        }
        Dictionary<EItemType, Item_Equipment> EquipList = new Dictionary<EItemType, Item_Equipment>();
        if (equipData != "")
        {
            for (int i = 0; i < equip.Length; ++i)
            {
                string[] parse = equip[i].Split(',');
                int handle = int.Parse(parse[0]);
                int value = int.Parse(parse[1]);
                Item_Equipment Item = ItemMng.Instance.GetItemList[handle].Clone() as Item_Equipment;
                Item.Value = value;
                if (Item != null)
                    EquipList.Add(Item.Type, Item);
            }
        }
        if (player.Character == null)
            player.Character = CharacterMng.Instance.InstantiateHero(player.Handle, uniqueID, type, pos, EquipList, player.Name);

        player.Character.StatSystem.Level = player.Level;
        player.Character.StatSystem.STR = STR;
        player.Character.StatSystem.DEX = DEX;
        player.Character.StatSystem.INT = INT;
        player.Character.StatSystem.WIS = WIS;
        player.Character.StatSystem.CON = CON;
        player.Character.StatSystem.CurrHP = player.Character.StatSystem.GetHP * 1;
        player.Character.StatSystem.CurrMP = player.Character.StatSystem.GetMP * 1;
        player.Character.transform.eulerAngles = angle;
    }
    public static void SetSkillData(ref Player player, string skillData)
    {
        if (skillData != "")
        {
            BaseCharacter character = player.Character;
            string[] skill = skillData.Split('/');
            
            for (int i = 0; i < skill.Length; ++i)
            {
                string[] parse = skill[i].Split(',');
                int handle = int.Parse(parse[0]);
                int level = int.Parse(parse[1]);
                CharacterMng.Instance.AddSkill(character, handle);
            }
        }
    }
    public static string GetPlayerInfoData(Player player)
    {
        string infoData = player.Handle + "," + player.Name + "," + player.Level + "," + player.Awakening + "," + player.Exp + ","
          + player.Character.StatSystem.STR + "," + player.Character.StatSystem.DEX + "," + player.Character.StatSystem.INT + "," + player.Character.StatSystem.WIS + "," + player.Character.StatSystem.CON + ","
          + player.StatPoint + "," + player.SkillPoint + "," + player.Gold + "," + player.MaxInventoryCount;
        return infoData;
    }
    public static string GetStatusData(Player player)
    {
        string statusData = player.CurrMapHandle + "," + player.Character.transform.position.x + "," + player.Character.transform.position.y + "," + player.Character.transform.position.z + ","
            + player.Character.transform.eulerAngles.x + "," + player.Character.transform.eulerAngles.y + "," + player.Character.transform.eulerAngles.z;
        // string statusData = player.CurrMapHandle + "," + player.CurrCoord.x + "," + player.CurrCoord.y + "," + player.CurrCoord.z;
        return statusData;
    }
    public static string GetEquipData(Player player)
    {
        List<string> equipList = new List<string>();
        foreach (Item_Equipment item in player.Character.StatSystem.Equipment.Values)
            equipList.Add(item.Handle + "," + item.Value);
        string equipData = string.Join("/", equipList);
        return equipData;
    }
    public static string GetSkillData(Player player)
    {
        List<string> skillList = new List<string>();
        foreach (BaseSkill skill in PlayerMng.Instance.MainPlayer.Character.AttackSystem.SkillDic.Values)
            if((skill.SkillInfo.Type & ESkillType.NotLearn) == 0)
                skillList.Add(skill.SkillInfo.Handle + "," + skill.SkillInfo.Level);
        string skillData = string.Join("/", skillList);
        return skillData;
    }
    public static string GetClassKorConvert(ECharacterClass val)
    {
        switch(val)
        {
            case ECharacterClass.Warrior:
                return "전사";
            case ECharacterClass.Archer:
                return "아쳐";
            case ECharacterClass.Cleric:
                return "성직자";
            case ECharacterClass.Crusader:
                return "십자군";
        }
        return "";
    }
    public static string GetRairityKorConvert(EItemRarity val)
    {
        switch (val)
        {
            case EItemRarity.Normal:
                return "일반등급 아이템";
            case EItemRarity.Magic:
                return "마법등급 아이템";
            case EItemRarity.Unique:
                return "고급등급 아이템";
            case EItemRarity.Relic:
                return "유물등급 아이템";
            case EItemRarity.Legend:
                return "전설등급 아이템";
            case EItemRarity.Infinity:
                return "절대등급 아이템";
        }
        return "";
    }
}
