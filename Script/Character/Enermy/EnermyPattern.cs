using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Flags]
public enum EEnermyPatternType
{
    Always = 1,
    HP = 2,
    Time = 4,
}
public class EnermyPattern
{
    public EEnermyPatternType Type;

    BaseCharacter m_caster;
    public string SkillKey;
    BaseSkill m_skill;

    public int Order;

    public float TargetHPLow;
    public float TargetHPHigh;
    public bool TargetHPUse;

    public float TargetTime;
    public bool TargetTimeUse;

    float m_timeElapsedTime;
    public EnermyPattern(EEnermyPatternType type, int order, string skillKey)
    {
        Type = type;
        SkillKey = skillKey;
        Order = order;
    }
    public EnermyPattern(EEnermyPatternType type, int order, string skillKey, float hpLow, float hpHigh)
    {
        Type = type;
        SkillKey = skillKey;
        Order = order;
        TargetHPLow = hpLow;
        TargetHPHigh = hpHigh;
    }
    public EnermyPattern(EEnermyPatternType type, int order, string skillKey, float targetTime)
    {
        Type = type;
        SkillKey = skillKey;
        Order = order;
        TargetTime = targetTime;
    }
    public EnermyPattern Init(BaseEnermy caster)
    {
        m_caster = caster;
        switch (SkillKey)
        {
            case "FireBall":
                m_skill = caster.gameObject.AddComponent<Skill_FireBall>().Init(caster);
                break;
            case "FireAttack":
                m_skill = caster.gameObject.AddComponent<Skill_FireAttack>().Init(caster);
                break;
            case "ReturnSoul":
                m_skill = caster.gameObject.AddComponent<Skill_ReturnSoul>().Init(caster);
                break;
            case "FireSlash":
                m_skill = caster.gameObject.AddComponent<Skill_FireSlash>().Init(caster);
                break;
            case "FireWall":
                m_skill = caster.gameObject.AddComponent<Skill_FireWall>().Init(caster);
                break;
            case "HammerStorm":
                m_skill = caster.gameObject.AddComponent<Skill_HammerStorm>().Init(caster);
                break;
            case "HammerStrike": 
                m_skill = caster.gameObject.AddComponent<Skill_HammerStrike>().Init(caster);
                break;
            case "RageAssault":
                m_skill = caster.gameObject.AddComponent<Skill_RageAssault>().Init(caster);
                break;
        }

        return this;
    }
    // 패턴에 필요한 조건식
    public bool IsUse()
    {
        if (!m_skill.Using())
            return false;

        if((Type & EEnermyPatternType.HP) != 0)
        {
            if (!TargetHPUse)
            {
                float hpPer = m_caster.StatSystem.CurrHP / m_caster.StatSystem.GetHP;
                if (TargetHPLow >= hpPer || TargetHPHigh <= hpPer)
                    return false;
            }
            else return false;
        }
        if((Type & EEnermyPatternType.Time) != 0)
        {
            if (!TargetTimeUse)
            {
                if (TargetTime > m_timeElapsedTime)
                    return false;
            }
            else return false;
        }
        return true;
    }
    public bool RangeCheck()
    {
        return m_skill.RangeCheck();
    }
    // 패턴 실행
    public void Use()
    {
        TargetHPUse = (Type & EEnermyPatternType.Always) == 0;
        TargetTimeUse = (Type & EEnermyPatternType.Always) == 0;
        m_caster.AttackSystem.UseSkill(m_skill);
    }
    public void Reset()
    {
        m_skill.ElapsedTime = 0;
        m_timeElapsedTime = 0;
        m_skill.PossibleSkill = false;
        TargetHPUse = false;
        TargetTimeUse = false;
    }
    public EnermyPattern Clone()
    {
        return MemberwiseClone() as EnermyPattern;
    }
}