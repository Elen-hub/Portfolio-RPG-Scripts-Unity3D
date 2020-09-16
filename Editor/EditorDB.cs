using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;
using System.IO;

#if UNITY_EDITOR
public static class EditorDB
{
    public static bool IsLoad;
    public static Dictionary<int, Stat> CharacterStatDic = new Dictionary<int, Stat>();
    public static Dictionary<int, Stat> MonsterStatDic = new Dictionary<int, Stat>();
    public static Dictionary<int, NPCStat> NPCStatDic = new Dictionary<int, NPCStat>();
    public static Dictionary<int, NormalAttack> NormalAttackDic = new Dictionary<int, NormalAttack>();
    public static Dictionary<int, NormalAttack> MonsterAttackDic = new Dictionary<int, NormalAttack>();
    public static Dictionary<int, Map> MapList = new Dictionary<int, Map>();
    public static Dictionary<int, SkillInfo> SkillList = new Dictionary<int, SkillInfo>();
    public static Dictionary<int, Item_Base> ItemDic = new Dictionary<int, Item_Base>();
    public static Dictionary<int, Quest> QuestDic = new Dictionary<int, Quest>();
    public static Dictionary<int, CoinShopInfo> ShopDic = new Dictionary<int, CoinShopInfo>();
    public static void Init()
    {
        if (IsLoad)
            return;

        LoadCharacterStat();
        LoadMap();
        LoadSkill();
        LoadMonster();
        LoadNPC();
        LoadItem();
        LoadQuest();
        LoadCoinShop();

        IsLoad = true;
    }
    static void LoadCoinShop()
    {

    }
    static void LoadCharacterStat()
    {
        TextAsset Asset = new TextAsset(File.ReadAllText("Assets/AssetBundle_Database/DB_Hero.json"));
        if (Asset == null)
            return;

        JSONNode Node = JSON.Parse(Asset.text);

        for (int i = 0; i < Node.Count; ++i)
        {
            Stat stat = new Stat()
            {
                Handle = int.Parse(Node[i]["Handle"]),
                Class = (ECharacterClass)int.Parse(Node[i]["Class"]),
                Awakening = int.Parse(Node[i]["Awakening"]),
                STR = float.Parse(Node[i]["STR"]),
                DEX = float.Parse(Node[i]["DEX"]),
                INT = float.Parse(Node[i]["INT"]),
                WIS = float.Parse(Node[i]["WIS"]),
                CON = float.Parse(Node[i]["CON"]),
                Name = Node[i]["Name"],
                Explanation = Node[i]["Explanation"],
                Icon = Node[i]["Icon"],
                Path = Node[i]["Path"],
                HP = float.Parse(Node[i]["HP"]),
                RecoveryHP = float.Parse(Node[i]["RecoveryHP"]),
                Resistance = float.Parse(Node[i]["Resistance"]),
                MP = float.Parse(Node[i]["MP"]),
                RecoveryMP = float.Parse(Node[i]["RecoveryMP"]),
                CoolTime = float.Parse(Node[i]["CoolTime"]),
                CriticalPro = float.Parse(Node[i]["CriticalPro"]),
                CriticalDamage = float.Parse(Node[i]["CriticalDamage"]),
                AttackSpeed = float.Parse(Node[i]["AttackSpeed"]),
                MoveSpeed = float.Parse(Node[i]["MoveSpeed"]),
                MoveSpeedPro = float.Parse(Node[i]["MoveSpeedPro"]),
                AttackDamage = float.Parse(Node[i]["AttackDamage"]),
                AttackDamagePro = float.Parse(Node[i]["AttackDamagePro"]),
                Defence = float.Parse(Node[i]["Defence"]),
                DefencePro = float.Parse(Node[i]["DefencePro"]),
                SkillDamagePro = float.Parse(Node[i]["SkillDamagePro"])
            };

            EditorDB.CharacterStatDic.Add(stat.Handle, stat);
        } 
         Asset = new TextAsset(File.ReadAllText("Assets/AssetBundle_Database/DB_NormalAttack.json"));
         Node = JSON.Parse(Asset.text);

        for (int i = 0; i < Node.Count; ++i)
        {
            NormalAttack Attack = new NormalAttack()
            {
                Handle = int.Parse(Node[i]["Handle"]),
                Count = int.Parse(Node[i]["Count"]),
                DurationTime = new float[int.Parse(Node[i]["Count"])],
                CompleteTime = new float[int.Parse(Node[i]["Count"])],
                DamagePro = new float[int.Parse(Node[i]["Count"])],
                Type = new EFindCharacterType[int.Parse(Node[i]["Count"])],
                Range = new float[int.Parse(Node[i]["Count"])],
                Angle = new float[int.Parse(Node[i]["Count"])],
                MissileHandle = new int[int.Parse(Node[i]["Count"])],
                Width = new float[int.Parse(Node[i]["Count"])],
                HitTime = new float[int.Parse(Node[i]["Count"])]

            };

            string[] DurationTime = Node[i]["DurationTime"].Value.Split(',');
            string[] CompleteTime = Node[i]["CompleteTime"].Value.Split(',');
            string[] DamagePro = Node[i]["DamagePro"].Value.Split(',');
            string[] Type = Node[i]["Type"].Value.Split(',');
            string[] Range = Node[i]["Range"].Value.Split(',');
            string[] Angle = Node[i]["Angle"].Value.Split(',');
            string[] Width = Node[i]["Width"].Value.Split(',');
            string[] MissileHandle = Node[i]["MissileHandle"].Value.Split(',');
            string[] HitTime = Node[i]["HitTime"].Value.Split(',');

            for (int j = 0; j < Attack.Count; ++j)
            {
                Attack.DurationTime[j] = float.Parse(DurationTime[j]);
                Attack.CompleteTime[j] = float.Parse(CompleteTime[j]);
                Attack.DamagePro[j] = float.Parse(DamagePro[j]);
                Attack.Type[j] = (EFindCharacterType)int.Parse(Type[j]);
                Attack.Range[j] = float.Parse(Range[j]);
                Attack.Angle[j] = float.Parse(Angle[j]);
                Attack.Width[j] = float.Parse(Width[j]);
                Attack.MissileHandle[j] = int.Parse(MissileHandle[j]);
                Attack.HitTime[j] = float.Parse(HitTime[j]);
            }

            EditorDB.NormalAttackDic.Add(Attack.Handle, Attack);
        }
    }

    static void LoadMap()
    {
        TextAsset Asset = new TextAsset(File.ReadAllText("Assets/AssetBundle_Database/DB_Map.json"));
        if (Asset == null)
            return;

        JSONNode Node = JSON.Parse(Asset.text);
        for (int i = 0; i < Node.Count; ++i)
        {
            Map map = new Map();
            map.Handle = int.Parse(Node[i]["Handle"]);
            map.Type = (EMapType)int.Parse(Node[i]["Area"]);
            map.Group = (EMapAreaGroup)int.Parse(Node[i]["Group"]);
            map.MapName = Node[i]["MapName"];
            map.Information = Node[i]["Information"];
            map.MinLevel = int.Parse(Node[i]["MinLevel"]);
            map.Level = int.Parse(Node[i]["Level"]);
            map.Number = int.Parse(Node[i]["Number"]);
            map.MapImgPath = Node[i]["MapImgPath"];
            map.MapIconPath = Node[i]["MapIconPath"];
            map.SceneName = Node[i]["SceneName"];
            map.BGM = Node[i]["BGM"];
            string[] SizeArr = Node[i]["Size"].Value.Split(',');
            map.Size = new Vector2(float.Parse(SizeArr[0]), float.Parse(SizeArr[1]));
            string[] WayPoint = Node[i]["WayPoint"].Value.Split(',');
            map.WayPoint = new Vector3(float.Parse(WayPoint[0]), float.Parse(WayPoint[1]), float.Parse(WayPoint[2]));
            if (Node[i]["Portal"] != "")
            {
                string[] PortalArr = Node[i]["Portal"].Value.Split('/');
                for (int j = 0; j < PortalArr.Length; ++j)
                {
                    string[] PortalArr2 = PortalArr[j].Split(',');
                    SMapPortal portal = new SMapPortal();
                    portal.Handle = int.Parse(PortalArr2[0]);
                    portal.Coord = new Vector3(float.Parse(PortalArr2[1]), float.Parse(PortalArr2[2]), float.Parse(PortalArr2[3]));
                    portal.DeltaCoord = new Vector3(float.Parse(PortalArr2[4]), float.Parse(PortalArr2[5]), float.Parse(PortalArr2[6]));
                    map.PortalList.Add(portal);
                }
            }
            if (Node[i]["Monster"] != "")
            {
                string[] MonsterArr = Node[i]["Monster"].Value.Split('/');
                for (int j = 0; j < MonsterArr.Length; ++j)
                {
                    string[] MonsterArr2 = MonsterArr[j].Split(',');
                    SMapMonster portal = new SMapMonster();
                    portal.Handle = int.Parse(MonsterArr2[0]);
                    portal.RespawnTime = float.Parse(MonsterArr2[1]);
                    portal.Coord = new Vector3(float.Parse(MonsterArr2[2]), float.Parse(MonsterArr2[3]), float.Parse(MonsterArr2[4]));
                    portal.Angle = float.Parse(MonsterArr2[5]);
                    map.MonsterList.Add(portal);
                }
            }
            if(Node[i]["MatchPortal"] != "")
            {
                string[] PortalArr = Node[i]["MatchPortal"].Value.Split('/');
                for(int j = 0; j<PortalArr.Length; ++j)
                {
                    string[] PortalArr2 = PortalArr[j].Split(',');
                    SMatchPortal portal = new SMatchPortal();
                    portal.Coord = new Vector3(float.Parse(PortalArr2[0]), float.Parse(PortalArr2[1]), float.Parse(PortalArr2[2]));
                    for (int k = 3; k< PortalArr2.Length; ++k)
                        portal.HandleList.Add(int.Parse(PortalArr2[k]));
                    map.MatchPortalList.Add(portal);
                }
            }
            //if (Node[i]["NPC"] != "")
            //{
            //    string[] NPCArr = Node[i]["NPC"].Value.Split('/');
            //    for (int j = 0; j < NPCArr.Length; ++j)
            //    {
            //        string[] NPCArr2 = NPCArr[j].Split(',');
            //        SMapNPC portal = new SMapNPC();
            //        portal.Handle = int.Parse(NPCArr2[0]);
            //        portal.Coord = new Vector3(float.Parse(NPCArr2[1]), float.Parse(NPCArr2[2]), float.Parse(NPCArr2[3]));
            //        map.NPCList.Add(portal);
            //    }
            //}
            EditorDB.MapList.Add(map.Handle, map);
        }
    }

    static void LoadSkill()
    {
        TextAsset Asset = new TextAsset(File.ReadAllText("Assets/AssetBundle_Database/DB_Skill.json"));
        if (Asset == null)
            return;

        JSONNode Node = JSON.Parse(Asset.text);
        for (int i = 0; i < Node.Count; ++i)
        {
            SkillInfo skill = new SkillInfo()
            {
                Handle = int.Parse(Node[i]["Handle"]),
                CharacterClass = (ECharacterClass)int.Parse(Node[i]["CharacterClass"]),
                CharacterAwakening = int.Parse(Node[i]["CharacterAwakening"]),
                SkillPoint = int.Parse(Node[i]["SkillPoint"]),
                Level = int.Parse(Node[i]["Level"]),
                Name = Node[i]["Name"],
                Information = Node[i]["Information"].Value.Replace("//", "\n"),
                Explanation = Node[i]["Explanation"],
                DurationTime = float.Parse(Node[i]["DurationTime"]),
                CompleteTime = float.Parse(Node[i]["CompleteTime"]),
                Icon = Node[i]["Icon"],
                MP = float.Parse(Node[i]["MP"]),
                CoolTime = float.Parse(Node[i]["CoolTime"]),
                Range = float.Parse(Node[i]["Range"]),
                IsAutoAim = bool.Parse(Node[i]["AutoAIM"]),
                Type = (ESkillType)int.Parse(Node[i]["Type"]),
                LinkHandle = int.Parse(Node[i]["LinkHandle"]),
                LinkPossibleTime = float.Parse(Node[i]["LinkPossibleTime"]),
                KeydownTime = float.Parse(Node[i]["KeydownTime"]),
                ChannelingTime = float.Parse(Node[i]["ChannelingTime"]),
            };

            EditorDB.SkillList.Add(skill.Handle, skill);
        }
    }
    static void LoadMonster()
    {
        TextAsset Asset = new TextAsset(File.ReadAllText("Assets/AssetBundle_Database/DB_Monster.json"));
        if (Asset == null)
            return;

        JSONNode Node = JSON.Parse(Asset.text);

        for (int i = 0; i < Node.Count; ++i)
        {
            Stat stat = new Stat()
            {
                Handle = int.Parse(Node[i]["Handle"]),
                MonsterGrade = (EMonsterGrade)int.Parse(Node[i]["MonsterGrade"]),
                STR = float.Parse(Node[i]["STR"]),
                DEX = float.Parse(Node[i]["DEX"]),
                INT = float.Parse(Node[i]["INT"]),
                WIS = float.Parse(Node[i]["WIS"]),
                CON = float.Parse(Node[i]["CON"]),
                Name = Node[i]["Name"],
                Explanation = Node[i]["Explanation"],
                Icon = Node[i]["Icon"],
                Path = Node[i]["Path"],
                HP = float.Parse(Node[i]["HP"]),
                RecoveryHP = float.Parse(Node[i]["RecoveryHP"]),
                Resistance = float.Parse(Node[i]["Resistance"]),
                MP = float.Parse(Node[i]["MP"]),
                RecoveryMP = float.Parse(Node[i]["RecoveryMP"]),
                CoolTime = float.Parse(Node[i]["CoolTime"]),
                CriticalPro = float.Parse(Node[i]["CriticalPro"]),
                CriticalDamage = float.Parse(Node[i]["CriticalDamage"]),
                AttackSpeed = float.Parse(Node[i]["AttackSpeed"]),
                MoveSpeed = float.Parse(Node[i]["MoveSpeed"]),
                MoveSpeedPro = float.Parse(Node[i]["MoveSpeedPro"]),
                AttackDamage = float.Parse(Node[i]["AttackDamage"]),
                AttackDamagePro = float.Parse(Node[i]["AttackDamagePro"]),
                Defence = float.Parse(Node[i]["Defence"]),
                DefencePro = float.Parse(Node[i]["DefencePro"]),
                SkillDamagePro = float.Parse(Node[i]["SkillDamagePro"]),
                EXP = int.Parse(Node[i]["EXP"]),
                Gold = int.Parse(Node[i]["Gold"])
            };
            if(Node[i]["Reword"])
            {
                string[] Reword = Node[i]["Reword"].Value.Split('/');
                for (int j = 0; j < Reword.Length; ++j)
                {
                    string[] Values = Reword[j].Split(',');
                    stat.Reword.Add(new SMonsterReword(int.Parse(Values[0]), float.Parse(Values[2]), int.Parse(Values[1])));
                }
            }
            if(Node[i]["Pattern"])
            {
                string[] Pattern = Node[i]["Pattern"].Value.Split('/');
                for (int j = 0; j < Pattern.Length; ++j)
                {
                    string[] Values = Pattern[j].Split(',');
                    EEnermyPatternType type = (EEnermyPatternType)int.Parse(Values[0]);
                    int order = int.Parse(Values[1]);

                    if(type == EEnermyPatternType.Always)
                        stat.Pattern.Add(new EnermyPattern(type, order, Values[2]));
                    else if ((EEnermyPatternType.HP & type) != 0)
                        stat.Pattern.Add(new EnermyPattern(type, order, Values[2], float.Parse(Values[3]), float.Parse(Values[4])));
                    else if ((EEnermyPatternType.Time & type) != 0)
                        stat.Pattern.Add(new EnermyPattern(type, order, Values[2], float.Parse(Values[5])));
                }
            }
            EditorDB.MonsterStatDic.Add(stat.Handle, stat);
        }

        Asset = new TextAsset(File.ReadAllText("Assets/AssetBundle_Database/DB_MonsterAttack.json"));
        Node = JSON.Parse(Asset.text);

        for (int i = 0; i < Node.Count; ++i)
        {
            NormalAttack Attack = new NormalAttack()
            {
                Handle = int.Parse(Node[i]["Handle"]),
                Count = int.Parse(Node[i]["Count"]),
                DurationTime = new float[int.Parse(Node[i]["Count"])],
                CompleteTime = new float[int.Parse(Node[i]["Count"])],
                DamagePro = new float[int.Parse(Node[i]["Count"])],
                Type = new EFindCharacterType[int.Parse(Node[i]["Count"])],
                Range = new float[int.Parse(Node[i]["Count"])],
                Angle = new float[int.Parse(Node[i]["Count"])],
                Width = new float[int.Parse(Node[i]["Count"])],
                MissileHandle = new int[int.Parse(Node[i]["Count"])],
                HitTime = new float[int.Parse(Node[i]["Count"])],
            };

            string[] DurationTime = Node[i]["DurationTime"].Value.Split(',');
            string[] CompleteTime = Node[i]["CompleteTime"].Value.Split(',');
            string[] DamagePro = Node[i]["DamagePro"].Value.Split(',');
            string[] Type = Node[i]["Type"].Value.Split(',');
            string[] Range = Node[i]["Range"].Value.Split(',');
            string[] Angle = Node[i]["Angle"].Value.Split(',');
            string[] Width = Node[i]["Width"].Value.Split(',');
            string[] MissileHandle = Node[i]["MissileHandle"].Value.Split(',');
            string[] HitTime = Node[i]["HitTime"].Value.Split(',');

            for (int j = 0; j < Attack.Count; ++j)
            {
                Attack.DurationTime[j] = float.Parse(DurationTime[j]);
                Attack.CompleteTime[j] = float.Parse(CompleteTime[j]);
                Attack.DamagePro[j] = float.Parse(DamagePro[j]);
                Attack.Type[j] = (EFindCharacterType)int.Parse(Type[j]);
                Attack.Range[j] = float.Parse(Range[j]);
                Attack.Angle[j] = float.Parse(Angle[j]);
                Attack.Width[j] = float.Parse(Width[j]);
                Attack.MissileHandle[j] = int.Parse(MissileHandle[j]);
                Attack.HitTime[j] = float.Parse(HitTime[j]);
            }
            EditorDB.MonsterAttackDic.Add(Attack.Handle, Attack);
        }
    }
    static void LoadNPC()
    {
        TextAsset Asset = new TextAsset(File.ReadAllText("Assets/AssetBundle_Database/DB_NPC.json"));
        if (Asset == null)
            return;

        JSONNode Node = JSON.Parse(Asset.text);

        for (int i = 0; i < Node.Count; ++i)
        {
            NPCStat stat = new NPCStat();
            stat.Handle = int.Parse(Node[i]["Handle"]);
            stat.Name = Node[i]["Name"];
            stat.Path = Node[i]["Path"];
            stat.Option = (ENpcOption)int.Parse(Node[i]["Option"]);
            stat.MoveSpeed = float.Parse(Node[i]["MoveSpeed"]);
            if(Node[i]["Comment"])
            {
                stat.Comment = new List<string>();
                stat.Comment.AddRange(Node[i]["Comment"].Value.Split(','));
            }
            if(Node[i]["Scripts"])
            {
                stat.Scripts = new List<string>();
                stat.Scripts.AddRange(Node[i]["Scripts"].Value.Split(','));
            }
            if(Node[i]["ShopHandle"])
            {
                stat.ShopHandle = new List<int>();
                string[] ShopHandle = Node[i]["ShopHandle"].Value.Split(',');
                for (int j = 0; j < ShopHandle.Length; ++j)
                    stat.ShopHandle.Add(int.Parse(ShopHandle[j]));
            }
  

            EditorDB.NPCStatDic.Add(stat.Handle, stat);
        }
    }
    static void LoadItem()
    {
        TextAsset Asset = new TextAsset(File.ReadAllText("Assets/AssetBundle_Database/DB_Item.json"));
        if (Asset == null)
            return;

        JSONNode Node = JSON.Parse(Asset.text);

        for (int i = 0; i < Node.Count; ++i)
        {
            Item_Base Item;

            switch ((EItemType)int.Parse(Node[i]["Type"]))
            {
                case EItemType.Other:
                    Item = new Item_Other();
                    break;
                case EItemType.Potion:
                    Item = new Item_Potion();
                    break;
                case EItemType.Scroll:
                    Item = new Item_Scroll();
                    break;
                case EItemType.Weapon:
                case EItemType.Armor:
                case EItemType.Gloves:
                case EItemType.Shoes:
                case EItemType.Ring:
                case EItemType.Necklace:
                    Item = new Item_Equipment();
                    break;
                default:
                    Item = new Item_Base();
                    break;
            }
            Item.Handle = int.Parse(Node[i]["Handle"].Value);
            Item.Name = Node[i]["Name"].Value;
            Item.Explanation = Node[i]["Explanation"].Value;
            Item.Price = int.Parse(Node[i]["Price"].Value);
            Item.Type = (EItemType)int.Parse(Node[i]["Type"]);
            Item.Rarity = (EItemRarity)int.Parse(Node[i]["Rarity"].Value);
            Item.Icon = Node[i]["Icon"].Value;
            IItemLevel Level = Item as IItemLevel;
            if (Level != null)
            {
                Level.Level = int.Parse(Node[i]["Level"]);
            }
            IItemActive Active = Item as IItemActive;
            if (Active != null)
            {
                Active.ActionHandle = (EItemActiveType)int.Parse(Node[i]["ActionHandle"]);
                Active.ActionValue = Node[i]["ActionValue"];
                Active.CoolTime = float.Parse(Node[i]["CoolTime"]);
            }
            IItemEquipment Stat = Item as IItemEquipment;
            if (Stat != null)
            {
                Stat.Stat = new Stat()
                {
                    STR = float.Parse(Node[i]["STR"]),
                    DEX = float.Parse(Node[i]["DEX"]),
                    INT = float.Parse(Node[i]["INT"]),
                    WIS = float.Parse(Node[i]["WIS"]),
                    CON = float.Parse(Node[i]["CON"]),
                    HP = float.Parse(Node[i]["HP"]),
                    RecoveryHP = float.Parse(Node[i]["RecoveryHP"]),
                    Resistance = float.Parse(Node[i]["Resistance"]),
                    MP = float.Parse(Node[i]["MP"]),
                    RecoveryMP = float.Parse(Node[i]["RecoveryMP"]),
                    CoolTime = float.Parse(Node[i]["CoolTime"]),
                    CriticalPro = float.Parse(Node[i]["CriticalPro"]),
                    CriticalDamage = float.Parse(Node[i]["CriticalDamage"]),
                    AttackSpeed = float.Parse(Node[i]["AttackSpeed"]),
                    MoveSpeed = float.Parse(Node[i]["MoveSpeed"]),
                    MoveSpeedPro = float.Parse(Node[i]["MoveSpeedPro"]),
                    AttackDamage = float.Parse(Node[i]["AttackDamage"]),
                    AttackDamagePro = float.Parse(Node[i]["AttackDamagePro"]),
                    Defence = float.Parse(Node[i]["Defence"]),
                    DefencePro = float.Parse(Node[i]["DefencePro"]),
                    SkillDamagePro = float.Parse(Node[i]["SkillDamagePro"])
                };
            }
            EditorDB.ItemDic.Add(Item.Handle, Item);
        }
    }

    static void LoadQuest()
    {
        TextAsset Asset = new TextAsset(File.ReadAllText("Assets/AssetBundle_Database/DB_Quest.json"));
        if (Asset == null)
            return;

        JSONNode Node = JSON.Parse(Asset.text);

        for (int i = 0; i < Node.Count; ++i)
        {
            Quest Quest = new Quest();
            Quest.Handle = int.Parse(Node[i]["Handle"]);
            Quest.Name = Node[i]["Name"];
            Quest.Type = (EQuestType)int.Parse(Node[i]["Type"]);

            for (int j = 2; j < Node[i].Count - 1; ++j)
            {
                string[] Value = Node[i][j].Value.Split(',');
                QuestContent Content = new QuestContent();
                Content.Handle = int.Parse(Value[0]);
                Content.NPCHandle = int.Parse(Value[1]);
                Content.Level = int.Parse(Value[2]);
                Content.ContentName = Value[3];
                Content.Scripts.AddRange(Value[4].Split('/'));
                Content.Type = (EQuestClearType)int.Parse(Value[5]);
                Content.ClearNPCHandle = int.Parse(Value[6]);
                Content.ClearHandle = int.Parse(Value[7]);
                Content.ClearValue = int.Parse(Value[8]);
                string[] Reword = Value[9].Split('/');
                for(int k =0; k<Reword.Length; ++k)
                {
                    if (Reword[k] == "")
                        continue;
                    
                    string[] RewordPair = Reword[k].Split('-');
                    int RewordHandle = int.Parse(RewordPair[0]);
                    int RewordValue = int.Parse(RewordPair[1]);
                    Content.Reword.Add(new SQuestReword(RewordHandle, RewordValue));
                }
                Content.NoneClearScript = Value[10];
                Content.ClearScripts.AddRange(Value[11].Split('/'));
                Content.RewordGold = int.Parse(Value[12]);
                Content.RewordEXP = int.Parse(Value[13]);
                Quest.QuestDic.Add(Content.Handle, Content);
            }

            EditorDB.QuestDic.Add(Quest.Handle, Quest);
        }
    }
}
#endif