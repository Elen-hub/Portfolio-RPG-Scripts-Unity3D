using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;

public enum EFindCharacterType
{
    OnlyDistance,
    Sector,
    Missile,
    Rectangle,
}
public enum ECharacterClass
{
    Default,
    Cleric,
    Archer,
    Caster,
    Warrior,
    Crusader,
    Knight,
    Witch,
    Ranger,
    Soldier,
    Beast,
    Assasin,
    Fighter,
    Striker,
}
public class CharacterMng : TSingleton<CharacterMng>
{
    Dictionary<int, Stat> m_characterStat = new Dictionary<int, Stat>();
    Dictionary<int, NormalAttack> m_characterAttackStat = new Dictionary<int, NormalAttack>();
    Dictionary<int, Stat> m_monsterStat = new Dictionary<int, Stat>();
    Dictionary<int, NormalAttack> m_monsterAttackStat = new Dictionary<int, NormalAttack>();
    Dictionary<int, List<BaseEnermy>> m_enermyMemoryDic = new Dictionary<int, List<BaseEnermy>>();
    Dictionary<int, NPCStat> m_npcStat = new Dictionary<int, NPCStat>();

    public List<BaseCharacter> RespawnList = new List<BaseCharacter>();
    public Dictionary<int, BaseCharacter> CurrCharacters = new Dictionary<int, BaseCharacter>();
    public Dictionary<int, Quest> m_questDic = new Dictionary<int, Quest>();
    public Dictionary<int, Quest> ClearQuest { get; private set; } = new Dictionary<int, Quest>();
    public Dictionary<int, Quest> CurrQuest { get; private set; } = new Dictionary<int, Quest>();
    public Dictionary<int, SkillInfo> SkillInfo = new Dictionary<int, SkillInfo>();
    public Stat GetCharacterStat(int handle) { return m_characterStat[handle]; }
    public Stat GetMonsterStat(int handle) { return m_monsterStat[handle]; }
    public bool UseMaterialClamp;
    public void ClearCharacterList()
    {
        foreach(BaseCharacter character in CurrCharacters.Values)
        {
            if (character is BaseEnermy)
            {
                character.gameObject.SetActive(false);
                character.State = BaseCharacter.CharacterState.Idle;
                character.StatSystem.CurrHP = character.StatSystem.GetHP;
                if (!m_enermyMemoryDic.ContainsKey(character.StatSystem.BaseStat.Handle))
                    m_enermyMemoryDic.Add(character.StatSystem.BaseStat.Handle, new List<BaseEnermy>());
                m_enermyMemoryDic[character.StatSystem.BaseStat.Handle].Add(character as BaseEnermy);
            }
        }
        CurrCharacters.Clear();
    }
    public void CompleteQuest(int handle)
    {
        Quest Quest = m_questDic[handle];
        m_npcStat[Quest.CurrQuest.NPCHandle].QuestList.Remove(Quest);
        Quest.Chapter += 1;
        Quest.Accept = false;
        Quest.ClearDate = LogSystem.GetTime().ToString();
        if (Quest.CurrQuest != null)
        {
            m_npcStat[Quest.CurrQuest.NPCHandle].QuestList.Add(Quest);
        }
        else
        {
            Quest.Clear = true;
            CurrQuest.Remove(handle);
            ClearQuest.Add(handle, Quest);
        }
    }
    public bool AcceptCheck(int handle, int level)
    {
        return m_questDic[handle].IsAccept(level);
    }
    public void AcceptQuest(int handle)
    {
        m_npcStat[m_questDic[handle].CurrQuest.NPCHandle].QuestList.Remove(m_questDic[handle]);
        CurrQuest.Add(handle, m_questDic[handle]);
        CurrQuest[handle].Accept = true;
    }
    public void DeleteQuest(int handle)
    {
        m_npcStat[CurrQuest[handle].CurrQuest.NPCHandle].QuestList.Add(CurrQuest[handle]);
        CurrQuest[handle].Delete();
        CurrQuest.Remove(handle);
    }
    public NPCStat GetNPCStat(int handle)
    {
        if (m_npcStat.ContainsKey(handle))
            return m_npcStat[handle];
        else
            return null;
    }
    public BaseSkill AddSkill(BaseCharacter hero, int key)
    {
        BaseSkill skill = null;
        SkillInfo info = SkillInfo[key];
        switch (key)
        {
            case 0:
                skill = hero.gameObject.AddComponent<Skill_Dash>().Init(hero, info);
                break;
            case 4:
                skill = hero.gameObject.AddComponent<Skill_Cleric_Heal>().Init(hero, info);
                break;
            case 5:
                skill = hero.gameObject.AddComponent<Skill_Cleric_Judgement>().Init(hero, info);
                break;
            case 6:
                skill = hero.gameObject.AddComponent<Skill_Cleric_HolyLight>().Init(hero, info);
                break;
            case 7:
                skill = hero.gameObject.AddComponent<Skill_Cleric_Bless>().Init(hero, info);
                break;
            case 8:
                skill = hero.gameObject.AddComponent<Skill_Cleric_PurificationFlame>().Init(hero, info);
                break;
            case 9:
                skill = hero.gameObject.AddComponent<Skill_Cleric_WorldTree>().Init(hero, info);
                break;
            case 10:
                skill = hero.gameObject.AddComponent<Skill_Cleric_Revelation>().Init(hero, info);
                break;
            case 11:
                skill = hero.gameObject.AddComponent<Skill_Cleric_Salvation>().Init(hero, info);
                break;
 
            case 12:
                skill = hero.gameObject.AddComponent<Skill_Archer_SplitWind>().Init(hero, info);
                break;
            case 13:
                skill = hero.gameObject.AddComponent<Skill_Archer_SummonSpritWolf>().Init(hero, info);
                break;
            case 14:
                skill = hero.gameObject.AddComponent<Skill_Archer_Heal>().Init(hero, info);
                break;
            case 15:
                skill = hero.gameObject.AddComponent<Skill_Archer_KnifeWind>().Init(hero, info);
                break;
            case 17:
                skill = hero.gameObject.AddComponent<Skill_Archer_WindOfBreathCutter>().Init(hero, info);
                break;
            case 18:
                skill = hero.gameObject.AddComponent<Skill_Archer_ForestShield>().Init(hero, info);
                break;
            case 19:
                skill = hero.gameObject.AddComponent<Skill_Archer_Tumbling>().Init(hero, info);
                break;
            case 21:
                skill = hero.gameObject.AddComponent<Skill_Archer_WindRain>().Init(hero, info);
                break;
            case 55:
                skill = hero.gameObject.AddComponent<Skill_Archer_SideWinder>().Init(hero, info);
                break;

            case 40:
                skill = hero.gameObject.AddComponent<Skill_Crusader_Judgement>().Init(hero, info);
                break;
            case 41:
                skill = hero.gameObject.AddComponent<Skill_Crusader_WillShield>().Init(hero, info);
                break;
            case 42:
                skill = hero.gameObject.AddComponent<Skill_Crusader_JudgementSlash>().Init(hero, info);
                break;
            case 43:
                skill = hero.gameObject.AddComponent<Skill_Crusader_SwordOfLightwave>().Init(hero, info);
                break;
            case 44:
                skill = hero.gameObject.AddComponent<Skill_Crusader_StarOfBethlehem>().Init(hero, info);
                break;
            case 45:
                skill = hero.gameObject.AddComponent<Skill_Crusader_SkyChain>().Init(hero, info);
                break;
            case 46:
                skill = hero.gameObject.AddComponent<Skill_Crusader_AssultDevilHunt>().Init(hero, info);
                break;
            case 47:
                skill = hero.gameObject.AddComponent<Skill_Crusader_SixWings>().Init(hero, info);
                break;
            case 56:
                skill = hero.gameObject.AddComponent<Skill_Crusader_Advent>().Init(hero, info);
                break;

            case 31:
                skill = hero.gameObject.AddComponent<Skill_Warrior_FullMoomSlash>().Init(hero, info);
                break;
            case 32:
                skill = hero.gameObject.AddComponent<Skill_Warrior_Crescent>().Init(hero, info);
                break;
            case 33:
                skill = hero.gameObject.AddComponent<Skill_Warrior_Step>().Init(hero, info);
                break;
            case 34:
                skill = hero.gameObject.AddComponent<Skill_Warrior_MoonSlash>().Init(hero, info);
                break;
            case 36:
                skill = hero.gameObject.AddComponent<Skill_Warrior_MeteorSmash>().Init(hero, info);
                break;
            case 37:
                skill = hero.gameObject.AddComponent<Skill_Warrior_Asura>().Init(hero, info);
                break;
            case 39:
                skill = hero.gameObject.AddComponent<Skill_Warrior_DrawingSword>().Init(hero, info);
                break;
        }

        hero.AttackSystem.SkillDic.Add(key, skill);
        return skill; 
    }
    public Transform InstantiatePreview(int handle)
    {
#if UNITY_EDITOR
        GameObject obj = UnityEditor.PrefabUtility.LoadPrefabContents("Assets/AssetBundle_Character/Hero/" + m_characterStat[handle].Path + "_Preview.prefab");
#else
        GameObject obj = AssetMng.Instance["character"].LoadAsset<GameObject>(m_characterStat[handle].Path + "_Preview");
#endif
        Transform Character = Instantiate(obj, Vector3.zero, Quaternion.identity).transform;
        return Character;
    }
    public BaseHero InstantiateHero(int handle, int uniqueID, EAllyType allyType, Vector3 position, Dictionary<EItemType, Item_Equipment> equipment, string name)
    {
#if UNITY_EDITOR
        GameObject obj = UnityEditor.PrefabUtility.LoadPrefabContents("Assets/AssetBundle_Character/Hero/" + m_characterStat[handle].Path + ".prefab");
#else
        GameObject obj = AssetMng.Instance["character"].LoadAsset<GameObject>(m_characterStat[handle].Path);
#endif
        obj = Instantiate<GameObject>(obj, position, Quaternion.identity);
        BaseHero Character = obj.GetComponent<BaseHero>();
        Character.Init(uniqueID, allyType, m_characterAttackStat[handle], m_characterStat[handle], equipment);
        Character.Name = name;
        switch (allyType)
        {
            case EAllyType.Player:
                Character.tag = "Player";
                Character.gameObject.layer = LayerMask.NameToLayer("Player");
                DontDestroyOnLoad(Character);
                break;
            case EAllyType.Friendly:
                Character.tag = "Ally";
                Character.gameObject.layer = LayerMask.NameToLayer("Ally");
                UIMng.Instance.GetUI<Game>(UIMng.UIName.Game).SubWindow.MapWindow.SetDynamicIcon(SubWindow_Map_DynamicIcon.EMapIconOption.Ally, Character);
                CurrCharacters.Add(uniqueID, Character);
                break;
            case EAllyType.Hostile:
                Character.tag = "Enermy";
                Character.gameObject.layer = LayerMask.NameToLayer("Enermy");
                UIMng.Instance.GetUI<Game>(UIMng.UIName.Game).SubWindow.MapWindow.SetDynamicIcon(SubWindow_Map_DynamicIcon.EMapIconOption.Enermy, Character);
                CurrCharacters.Add(uniqueID, Character);
                break;
            case EAllyType.Neutral:
                Character.tag = "Hero";
                Character.gameObject.layer = LayerMask.NameToLayer("Hero");
                CurrCharacters.Add(uniqueID, Character);
                break;
        }
        UIMng.Instance.GetUI<FieldUI>(UIMng.UIName.FieldUI).SetNameText(Character, name);
        return Character;
    }

    public BaseEnermy InstantiateMonster(int handle, int uniqueID, EAllyType allyType, Vector3 position, float angle)
    {
        BaseEnermy Character;
        if (m_enermyMemoryDic[handle].Count != 0)
        {
            Character = m_enermyMemoryDic[handle][0];
            m_enermyMemoryDic[handle].Remove(Character);
            Character.Respawn(uniqueID, allyType, position);
        }
        else
        {
#if UNITY_EDITOR
            GameObject obj = UnityEditor.PrefabUtility.LoadPrefabContents("Assets/AssetBundle_Character/Monster/" + m_monsterStat[handle].Path + ".prefab");
#else
            GameObject obj = AssetMng.Instance["character"].LoadAsset<GameObject>(m_monsterStat[handle].Path);
#endif
            obj = Instantiate(obj, position, Quaternion.identity);
            Character = obj.GetComponent<BaseEnermy>();
            Character.transform.eulerAngles = new Vector3(0, angle, 0);
            Character.Init(uniqueID, allyType, m_monsterAttackStat[handle], m_monsterStat[handle]);
            DontDestroyOnLoad(Character);
        }

        CurrCharacters.Add(uniqueID, Character);
        Character.tag = "Enermy";
        Character.gameObject.layer = LayerMask.NameToLayer("Enermy");
        UIMng.Instance.GetUI<FieldUI>(UIMng.UIName.FieldUI).SetNameText(Character, Character.StatSystem.BaseStat.Name);
        UIMng.Instance.GetUI<Game>(UIMng.UIName.Game).SubWindow.MapWindow.SetDynamicIcon(SubWindow_Map_DynamicIcon.EMapIconOption.Enermy, Character);
        return Character;
    }
    public void RemoveMonster(int uniqueID)
    {
        m_enermyMemoryDic[CurrCharacters[uniqueID].StatSystem.BaseStat.Handle].Add(CurrCharacters[uniqueID] as BaseEnermy);
        CurrCharacters.Remove(uniqueID);
    }
    public List<SMonsterReword> GetMonsterReword(int enermyHandle)
    {
        List<SMonsterReword> reword = new List<SMonsterReword>();

        for(int i =0; i< m_monsterStat[enermyHandle].Reword.Count; ++i)
        {
            if(Random.Range(0, 1000) <= m_monsterStat[enermyHandle].Reword[i].Percent*100)
                reword.Add(m_monsterStat[enermyHandle].Reword[i]);
        }
        return reword;
    }
    public List<BaseCharacter> GetCharactersToDistance(Vector3 pivot, float distance)
    {
        List<BaseCharacter> CharacterList = new List<BaseCharacter>();
        foreach (BaseCharacter value in CurrCharacters.Values)
            if (Vector3.Distance(value.transform.position, pivot) < distance)
                CharacterList.Add(value);

        return CharacterList;
    }
    public List<BaseCharacter> GetCharacterToDot(Transform pivot, float distance, float angle)
    {
        List<BaseCharacter> CharacterList = new List<BaseCharacter>();
        Vector3 Position = pivot.position;
        Vector3 Forward = pivot.forward;

       foreach (BaseCharacter value in CurrCharacters.Values)
       {
            if (Vector3.Distance(Position, value.transform.position) <= distance)
            {
                float Angle = Vector3.Angle(Forward, Vector3.Normalize(value.transform.position - Position));
                if (Angle <= angle)
                    CharacterList.Add(value);
            }
       }
        return CharacterList;
    }
    public bool CheckToDot(Vector3 pos, Transform pivot, float distance, float angle)
    {
        if(Vector3.Distance(pos, pivot.position) <= distance)
        {
            // if(Vector3.Angle(pivot.forward, Vector3.Normalize(pos - pivot.transform.position))
            float Angle = Vector3.Angle(Vector3.forward, pos - pivot.transform.position);
            //if (pos.x - pivot.transform.position.x < 0)
            //    Angle *= -1;

            if (Angle <= angle)
                return true;
        }

        return false;
    }
    public List<BaseCharacter> GetCharacterToRectangleRange(Vector3 pivot, float angle, float width, float height)
    {
        float r = Mathf.Deg2Rad * angle;
        List<BaseCharacter> CharacterList = new List<BaseCharacter>();
        foreach (BaseCharacter value in CurrCharacters.Values)
            if(CheckToRectangleRange(value.transform.position, pivot, angle, width, height))
                CharacterList.Add(value);

        return CharacterList;
    }
    public bool CheckToRectangleRange(Vector3 pos, Vector3 pivot, float angle, float width, float height)
    {
        float r = Mathf.Deg2Rad * angle;
        float x = ((pos.x - pivot.x) * Mathf.Cos(r) - (pos.z - pivot.z) * Mathf.Sin(r)) / width;
        float y = (2 * ((pos.x - pivot.x) * Mathf.Sin(r) + (pos.z - pivot.z) * Mathf.Cos(r)) - height) / (2 * height);
        return Mathf.Abs(x - y) + Mathf.Abs(x + y) <= 1; 
    }
    public BaseCharacter GetFilterClosestCharacter(Vector3 pos, EAllyType allyType)
    {
        BaseCharacter character = null;
        float distance = 1000;

        foreach (BaseCharacter value in CurrCharacters.Values)
        {
            if (value.State == BaseCharacter.CharacterState.Death)
                continue;

            if ((value.AllyType & allyType) != 0)
            {
                float ds = Vector3.Distance(pos, value.transform.position);
                if (distance > ds)
                {
                    character = value;
                    distance = ds;
                }
            }
        }
        return character;
    }
    public List<BaseCharacter> GetFilterTargetAlly(ref List<BaseCharacter> characterList, EAllyType allyType)
    {
        for(int i = characterList.Count-1; i>=0; --i)
            if ((characterList[i].AllyType & allyType) == 0)
                characterList.RemoveAt(i);

        return characterList;
    }
    public void RecoveryMP(BaseCharacter character, float mp)
    {
        if (character.StatSystem.GetMP > character.StatSystem.GetMP + mp)
            character.StatSystem.CurrMP = character.StatSystem.GetMP;
        else
            character.StatSystem.CurrMP += mp;

        UIMng.Instance.GetUI<FieldUI>(UIMng.UIName.FieldUI).SetDamageText(character, mp.ToString("F0"), Color.blue);
    }
    public override void Init()
    {
#if UNITY_EDITOR
        TextAsset Asset = new TextAsset(System.IO.File.ReadAllText("Assets/AssetBundle_Database/DB_Hero.json"));
        JSONNode Node = JSON.Parse(Asset.text);
#else
        JSONNode Node = JSON.Parse(AssetMng.Instance["database"].LoadAsset<TextAsset>("DB_Hero").text);
#endif
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

            m_characterStat.Add(stat.Handle, stat);
        }

#if UNITY_EDITOR
        Asset = new TextAsset(System.IO.File.ReadAllText("Assets/AssetBundle_Database/DB_NormalAttack.json"));
        Node = JSON.Parse(Asset.text);
#else
        Node = JSON.Parse(AssetMng.Instance["database"].LoadAsset<TextAsset>("DB_NormalAttack").text);
#endif
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

            m_characterAttackStat.Add(Attack.Handle, Attack);
        }

#if UNITY_EDITOR
        Asset = new TextAsset(System.IO.File.ReadAllText("Assets/AssetBundle_Database/DB_Monster.json"));
        Node = JSON.Parse(Asset.text);
#else
        Node = JSON.Parse(AssetMng.Instance["database"].LoadAsset<TextAsset>("DB_Monster").text);
#endif
        for (int i = 0; i < Node.Count; ++i)
        {
            Stat stat = new Stat()
            {
                Handle = int.Parse(Node[i]["Handle"]),
                MonsterGrade = (EMonsterGrade)int.Parse(Node[i]["MonsterGrade"]),
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
                EXP = int.Parse(Node[i]["EXP"])
            };
            if (Node[i]["Reword"])
            {
                string[] Reword = Node[i]["Reword"].Value.Split('/');
                for (int j = 0; j < Reword.Length; ++j)
                {
                    string[] Values = Reword[j].Split(',');
                    stat.Reword.Add(new SMonsterReword(int.Parse(Values[0]), float.Parse(Values[2]), int.Parse(Values[1])));
                }
            }
            if (Node[i]["Pattern"])
            {
                string[] Pattern = Node[i]["Pattern"].Value.Split('/');
                for (int j = 0; j < Pattern.Length; ++j)
                {
                    string[] Values = Pattern[j].Split(',');
                    EEnermyPatternType type = (EEnermyPatternType)int.Parse(Values[0]);
                    int order = int.Parse(Values[1]);

                    if (type == EEnermyPatternType.Always)
                        stat.Pattern.Add(new EnermyPattern(type, order, Values[2]));
                    else if ((EEnermyPatternType.HP & type) != 0)
                        stat.Pattern.Add(new EnermyPattern(type, order, Values[2], float.Parse(Values[3]), float.Parse(Values[4])));
                    else if ((EEnermyPatternType.Time & type) != 0)
                        stat.Pattern.Add(new EnermyPattern(type, order, Values[2], float.Parse(Values[5])));
                }
            }
            m_enermyMemoryDic.Add(stat.Handle, new List<BaseEnermy>());
            m_monsterStat.Add(stat.Handle, stat);
        }

#if UNITY_EDITOR
        Asset = new TextAsset(System.IO.File.ReadAllText("Assets/AssetBundle_Database/DB_MonsterAttack.json"));
        Node = JSON.Parse(Asset.text);
#else
        Node = JSON.Parse(AssetMng.Instance["database"].LoadAsset<TextAsset>("DB_MonsterAttack").text);
#endif
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

            m_monsterAttackStat.Add(Attack.Handle, Attack);
        }

        // 스킬 불러오기.
#if UNITY_EDITOR
        Asset = new TextAsset(System.IO.File.ReadAllText("Assets/AssetBundle_Database/DB_Skill.json"));
        Node = JSON.Parse(Asset.text);
#else
        Node = JSON.Parse(AssetMng.Instance["database"].LoadAsset<TextAsset>("DB_Skill").text);
#endif
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

            SkillInfo.Add(skill.Handle, skill);
        }

        // NPC 불러오기
#if UNITY_EDITOR
        Asset = new TextAsset(System.IO.File.ReadAllText("Assets/AssetBundle_Database/DB_NPC.json"));
        Node = JSON.Parse(Asset.text);
#else
        Node = JSON.Parse(AssetMng.Instance["database"].LoadAsset<TextAsset>("DB_NPC").text);
#endif
        for (int i = 0; i < Node.Count; ++i)
        {
            NPCStat stat = new NPCStat();
            stat.Handle = int.Parse(Node[i]["Handle"]);
            stat.Name = Node[i]["Name"];
            stat.Path = Node[i]["Path"];
            stat.Option = (ENpcOption)int.Parse(Node[i]["Option"]);
            stat.MoveSpeed = float.Parse(Node[i]["MoveSpeed"]);
            if (Node[i]["Comment"])
            {
                stat.Comment = new List<string>();
                stat.Comment.AddRange(Node[i]["Comment"].Value.Split(','));
            }
            if (Node[i]["Scripts"])
            {
                stat.Scripts = new List<string>();
                stat.Scripts.AddRange(Node[i]["Scripts"].Value.Split(','));
            }
            if (Node[i]["ShopHandle"])
            {
                stat.ShopHandle = new List<int>();
                string[] ShopHandle = Node[i]["ShopHandle"].Value.Split(',');
                for (int j = 0; j < ShopHandle.Length; ++j)
                    stat.ShopHandle.Add(int.Parse(ShopHandle[j]));
            }
            if (Node[i]["ProduceHandle"])
            {
                stat.ProduceList = new List<int>();
                string[] ProduceHandle = Node[i]["ProduceHandle"].Value.Split(',');
                for (int j = 0; j < ProduceHandle.Length; ++j)
                    stat.ProduceList.Add(int.Parse(ProduceHandle[j]));
            }
            m_npcStat.Add(stat.Handle, stat);
        }

        IsLoad = true;
    }

    public void InitQuest(string data)
    {
        m_questDic.Clear();
        ClearQuest.Clear();
        CurrQuest.Clear();

        foreach (NPCStat stat in m_npcStat.Values)
        {
            if (stat.QuestList != null)
                stat.QuestList.Clear();
        }
#if UNITY_EDITOR
        TextAsset Asset = new TextAsset(System.IO.File.ReadAllText("Assets/AssetBundle_Database/DB_Quest.json"));
        JSONNode Node = JSON.Parse(Asset.text);
#else
        JSONNode Node = JSON.Parse(AssetMng.Instance["database"].LoadAsset<TextAsset>("DB_Quest").text);
#endif
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
                for (int k = 0; k < Reword.Length; ++k)
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

            m_questDic.Add(Quest.Handle, Quest);
        }
        if (data != "")
        {
            string[] values = data.Split('/');
            for (int i = 0; i < values.Length; ++i)
            {
                string[] value = values[i].Split(',');
                int handle = int.Parse(value[0]);
                int chapter = int.Parse(value[1]);
                int currValue = int.Parse(value[2]);
                bool isAccept = value[3] == "True";
                bool isClear = value[4] == "True";
                string date = "";
                if(isClear) date = value[5];
                m_questDic[handle].Chapter = chapter;
                m_questDic[handle].Accept = isAccept;
                m_questDic[handle].Clear = isClear;

                if (isAccept)
                {
                    m_questDic[handle].CurrQuest.CurrValue = currValue;
                    CurrQuest.Add(handle, m_questDic[handle]);
                }
                if (isClear)
                {
                    m_questDic[handle].ClearDate = date;
                    ClearQuest.Add(handle, m_questDic[handle]);
                }
            }
        }
        foreach (Quest quest in m_questDic.Values)
        {
            if (!quest.Clear && !quest.Accept)
            {
                int NpcHandle = quest.CurrQuest.NPCHandle;

                if (m_npcStat[NpcHandle].QuestList == null)
                    m_npcStat[NpcHandle].QuestList = new List<Quest>();

                m_npcStat[NpcHandle].QuestList.Add(quest);
            }
        }
    }
}
