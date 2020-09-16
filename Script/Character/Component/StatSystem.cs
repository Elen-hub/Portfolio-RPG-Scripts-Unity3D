using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SMonsterReword
{
    public SMonsterReword()
    {

    }
    public SMonsterReword(int handle, float percent, int value)
    {
        Handle = handle;
        Percent = percent;
        Value = value;
    }
    public int Handle;
    public float Percent;
    public int Value;
}
public class Stat
{
    public HashInt Handle;
    public ECharacterClass Class;
    public EMonsterGrade MonsterGrade;
    public HashInt Awakening;

    public HashInt Level;
    public HashInt EXP;
    public HashFloat AddEXPPer;

    public string Name;
    public string Explanation;
    public string Icon;
    public string Path;

    public HashFloat STR;
    public HashFloat DEX;
    public HashFloat INT;
    public HashFloat WIS;
    public HashFloat CON;

    public HashFloat HP;
    public HashFloat RecoveryHP;
    public HashFloat Resistance;

    public HashFloat MP;
    public HashFloat RecoveryMP;
    public HashFloat CoolTime;

    public HashFloat CriticalPro;
    public HashFloat CriticalDamage;

    public HashFloat AttackSpeed;

    public HashFloat MoveSpeed;
    public HashFloat MoveSpeedPro;

    public HashFloat AttackDamage;
    public HashFloat AttackDamagePro;

    public HashFloat Reduction;
    public HashFloat Defence;
    public HashFloat DefencePro;

    public HashFloat SkillDamagePro;

    public List<EnermyPattern> Pattern = new List<EnermyPattern>();
    public List<SMonsterReword> Reword = new List<SMonsterReword>();
    public int Gold;
    public Stat()
    {

    }
    public Stat Clone()
    {
        return (Stat)MemberwiseClone();
    }
}

public class StatSystem : MonoBehaviour
{
    public Player Player;
    Dictionary<EItemType, Item_Equipment> m_equipMent = new Dictionary<EItemType, Item_Equipment>();
    public Stat BaseStat;
    public Stat BuffStat;

    public HashInt Level;
    public HashInt Exp;
    HashFloat m_str;
    HashFloat m_dex;
    HashFloat m_int;
    HashFloat m_wis;
    HashFloat m_con;
    public HashFloat STR { 
        get { return m_str; } 
        set {
            CurrHP += (value - m_str) * 10;
            m_str = value;
        }
    }
    public HashFloat DEX
    {
        get { return m_dex; }
        set { m_dex = value; }
    }
    public HashFloat INT
    {
        get { return m_int; }
        set {
            CurrMP += (value - m_int) * 5;
            m_int = value;
        }
    }
    public HashFloat WIS
    {
        get { return m_wis; }
        set {
            CurrMP += (value - m_wis) * 10;
            m_wis = value;
        }
    }
    public HashFloat CON
    {
        get { return m_con; }
        set {
            CurrHP += (value - m_con) * 20;
            m_con = value; 
        }
    }

    public HashFloat GetSTR {
        get {
            HashFloat val = (BaseStat.STR * (10+ Level * 0.1f)) + BuffStat.STR + STR;
            foreach (Item_Equipment item in m_equipMent.Values) val += item.Stat.STR;
            return val;
        }
    }
    public HashFloat GetDEX
    {
        get
        {
            HashFloat val = (BaseStat.DEX * (1 + Level * 0.1f)) + BuffStat.DEX + DEX;
            foreach (Item_Equipment item in m_equipMent.Values) val += item.Stat.DEX;
            return val;
        }
    }
    public HashFloat GetINT
    {
        get
        {
            HashFloat val = (BaseStat.INT * (1 + Level * 0.1f)) + BuffStat.INT + INT;
            foreach (Item_Equipment item in m_equipMent.Values) val += item.Stat.INT;
            return val;
        }
    }
    public HashFloat GetWIS
    {
        get
        {
            HashFloat val = (BaseStat.WIS * (1 + Level * 0.1f)) + BuffStat.WIS + WIS;
            foreach (Item_Equipment item in m_equipMent.Values) val += item.Stat.WIS;
            return val;
        }
    }
    public HashFloat GetCON
    {
        get
        {
            HashFloat val = (BaseStat.CON * (1 + Level*0.1f)) + BuffStat.CON + CON;
            foreach (Item_Equipment item in m_equipMent.Values) val += item.Stat.CON;
            return val;
        }
    }
    public HashFloat GetHP
    {
        get
        {
            HashFloat val = BaseStat.HP + GetSTR * 10 + GetCON * 20;
            foreach (Item_Equipment item in m_equipMent.Values) val += item.Stat.HP;
            return val;
        }
    }
    public HashFloat GetHPRecovery
    {
        get
        {
            HashFloat val = BaseStat.RecoveryHP + BuffStat.RecoveryHP + GetCON;
            foreach (Item_Equipment item in m_equipMent.Values) val += item.Stat.RecoveryHP;
            return val;
        }
    }
    public HashFloat GetMP
    {
        get
        {
            HashFloat val = BaseStat.MP + GetINT * 5 + GetWIS * 10;
            foreach (Item_Equipment item in m_equipMent.Values) val += item.Stat.MP;
            return val;
        }
    }
    public HashFloat GetMPRecovery
    {
        get
        {
            HashFloat val = BaseStat.RecoveryMP + BuffStat.RecoveryMP + GetWIS;
            foreach (Item_Equipment item in m_equipMent.Values) val += item.Stat.RecoveryMP;
            return val;
        }
    }
    public HashFloat GetCriticalPro
    {
        get
        {
            HashFloat val = BaseStat.CriticalPro + BuffStat.CriticalPro + (GetDEX + GetWIS) * 0.005f;
            foreach (Item_Equipment item in m_equipMent.Values) val += item.Stat.CriticalPro;
            return val;
        }
    }
    public HashFloat GetCriticalDamage
    {
        get
        {
            HashFloat val = BaseStat.CriticalDamage + BuffStat.CriticalDamage + (GetDEX + GetINT * 0.5f) * 0.01f;
            foreach (Item_Equipment item in m_equipMent.Values) val += item.Stat.CriticalDamage;
            return val;
        }
    }
    public HashFloat GetAttackSpeed
    {
        get
        {
            HashFloat val = BaseStat.AttackSpeed + BuffStat.AttackSpeed;
            foreach (Item_Equipment item in m_equipMent.Values) val += item.Stat.AttackSpeed;
            return val;
        }
    }
    public HashFloat GetMoveSpeed
    {
        get
        {
            HashFloat val = BaseStat.MoveSpeed + BuffStat.MoveSpeed;
            foreach (Item_Equipment item in m_equipMent.Values) val += item.Stat.MoveSpeed;
            return val;
        }
    }
    public HashFloat GetMoveSpeedPro
    {
        get
        {
            HashFloat val = BaseStat.MoveSpeedPro + BuffStat.MoveSpeedPro + GetDEX * 0.005f;
            foreach (Item_Equipment item in m_equipMent.Values) val += item.Stat.MoveSpeedPro;
            return val;
        }
    }
    public HashFloat GetAttackDamage
    {
        get
        {
            HashFloat val = BaseStat.AttackDamage + BuffStat.AttackDamage;
            foreach (Item_Equipment item in m_equipMent.Values) val += item.Stat.AttackDamage;
            return val;
        }
    }
    public HashFloat GetAttackDamagePro
    {
        get
        {
            HashFloat val = BaseStat.AttackDamagePro + BuffStat.AttackDamagePro + GetSTR * 0.01f;
            foreach (Item_Equipment item in m_equipMent.Values) val += item.Stat.AttackDamagePro;
            return val;
        }
    }
    public HashFloat GetDefence
    {
        get
        {
            HashFloat val = BaseStat.Defence + BuffStat.Defence + GetSTR * 0.01f;
            foreach (Item_Equipment item in m_equipMent.Values) val += item.Stat.Defence;
            return val;
        }
    }
    public HashFloat GetDefencePro
    {
        get
        {
            HashFloat val = BaseStat.DefencePro  + BuffStat.DefencePro + GetCON * 0.01f;
            foreach (Item_Equipment item in m_equipMent.Values) val += item.Stat.DefencePro;
            return val;
        }
    }
    public HashFloat GetSkillDamage
    {
        get
        {
            HashFloat val = BaseStat.SkillDamagePro + BuffStat.SkillDamagePro + GetINT * 0.01f;
            foreach (Item_Equipment item in m_equipMent.Values) val += item.Stat.SkillDamagePro;
            return val;
        }
    }
    public HashFloat GetResistance
    {
        get
        {
            HashFloat val = BaseStat.Resistance + BuffStat.Resistance+ GetCON * 0.005f;
            foreach (Item_Equipment item in m_equipMent.Values) val += item.Stat.Resistance;
            return val;
        }
    }
    public HashFloat GetCoolTime
    {
        get
        {
            HashFloat val = BaseStat.CoolTime + BuffStat .CoolTime+ GetWIS * 0.005f;
            foreach (Item_Equipment item in m_equipMent.Values) val += item.Stat.CoolTime;
            return val;
        }
    }
    public HashFloat GetNormalCalculateDamage
    {
        get
        {
            return (GetAttackDamage * (1 + GetAttackDamagePro)) * (1 + GetSkillDamage);
        }
    }
    public HashFloat GetCriticalCalculateDamage
    {
        get
        {
            return (GetAttackDamage * (1 + GetAttackDamagePro)) * (1 + GetSkillDamage) * (1 + GetCriticalDamage);
        }
    }
    public HashFloat GetReduction
    {
        get
        {
            HashFloat val = BaseStat.Reduction + BuffStat.Reduction;
            foreach (Item_Equipment item in m_equipMent.Values) val += item.Stat.Reduction;
            return val;
        }
    }
    public HashFloat GetAddEXPPer
    {
        get
        {
            HashFloat val = BaseStat.AddEXPPer + BuffStat.AddEXPPer;
            foreach (Item_Equipment item in m_equipMent.Values) val += item.Stat.AddEXPPer;
            return val;
        }
    }
    public bool IsCritical
    {
        get 
        { 
            return GetCriticalPro * 100 >= UnityEngine.Random.Range(0, 100); 
        }
    }
    public float CurrHP;
    public float CurrMP;

    public Item_Equipment RemoveEquipment(EItemType type)
    {
        float HPPer = CurrHP / GetHP;
        float MPPer = CurrMP / GetMP;
        Item_Equipment outItem = null;
        if (m_equipMent.ContainsKey(type))
        {
            outItem = m_equipMent[type];
            m_equipMent.Remove(type);
        }
        CurrHP = HPPer * GetHP;
        CurrMP = MPPer * GetMP;
        return outItem;
    }
    public Item_Equipment SetEquipment(Item_Equipment item)
    {
        float HPPer = CurrHP / GetHP;
        float MPPer = CurrMP / GetMP;
        Item_Equipment outItem = null;

        if (m_equipMent.ContainsKey(item.Type))
        {
            outItem = m_equipMent[item.Type];
            m_equipMent.Remove(item.Type);
        }

        m_equipMent.Add(item.Type, item as Item_Equipment);
        CurrHP = HPPer * GetHP;
        CurrMP = MPPer * GetMP;
        return outItem;
    }
    public Item_Equipment GetEquipment(EItemType type)
    {
        if (m_equipMent.ContainsKey(type))
            return m_equipMent[type];
        else
            return null;
    }
    public Dictionary<EItemType, Item_Equipment> Equipment
    {
        get {
            return m_equipMent; 
        }
        set 
        {
            float HPPer = CurrHP / GetHP;
            float MPPer = CurrMP / GetMP;
            m_equipMent = value;
            CurrHP = HPPer * GetHP;
            CurrMP = MPPer * GetMP;
        }
    }
    public void RecoveryHP(float hp)
    {
        CurrHP = Mathf.Clamp(CurrHP + hp, 0, GetHP);
    }
    public void RecoveryMP(float mp)
    {
        CurrMP = Mathf.Clamp(CurrMP + mp, 0, GetMP);
    }
    public void Init(Stat stat, Dictionary<EItemType, Item_Equipment> equipment)
    {
        BaseStat = stat;
        BuffStat = new Stat();
        m_equipMent = equipment;
        CurrHP = GetHP;
        CurrMP = GetMP;
    }
    public void RecoveryNature()
    {
        RecoveryHP(GetHPRecovery);
        RecoveryMP(GetMPRecovery);
    }
}