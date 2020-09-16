using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Flags]
public enum EBuffOption
{
    Default = 1,
    Single = 2,
    Interval = 4,
    Continue = 8,
}
public enum EBuffType
{
    Params,
    StrengthATK,
    WeakenATK,
    RecoveryHPPer,
    RecoveryMPPer,
    WeakenDEF,
    StrengthReduction,
    WeakenReduction,
    InvincibilityArmor,
    SuperArmor,
}
public enum EParamsType
{
    Bless,
    Burn,
    Revelation,
    Bethlehem,
    SixWings,
}
public class Buff
{
    protected BaseEffect m_effect;
    public EBuffOption BuffOption;
    public EBuffType BuffType;
    public EParamsType ParamsType;

    BaseCharacter m_caster;
    BaseCharacter m_character;
    public string IconPath;
    protected float m_value;
    protected float[] m_values;

    public float DurationTime;
    public float ElapsedTime;

    protected bool m_isInterval;
    protected float m_intervalElapsedTime;
    protected float m_intervalTime;

    public bool IsStart;

    delegate void BuffAction(bool Start);
    BuffAction m_buff;

    public Buff(BaseCharacter caster, BaseCharacter character, EBuffOption option, EBuffType type, string iconPath, float durationTime, float value, float intervalTime = 0)
    {
        m_caster = caster;
        m_character = character;
        BuffOption = option;
        BuffType = type;
        IconPath = iconPath;
        DurationTime = durationTime;
        m_value = value;
        m_intervalTime = intervalTime;
        m_isInterval = (option & EBuffOption.Interval) != 0;

        m_buff = System.Delegate.CreateDelegate(typeof(BuffAction), this, (type).ToString()) as BuffAction;
    }
    public Buff(BaseCharacter caster, BaseCharacter character, EBuffOption option, EParamsType type, string iconPath, float durationTime, float intervalTime, params float[] value)
    {
        m_caster = caster;
        m_character = character;
        BuffOption = option;
        ParamsType = type;
        IconPath = iconPath;
        DurationTime = durationTime;
        m_values = value;
        m_intervalTime = intervalTime;
        m_isInterval = (option & EBuffOption.Interval) != 0;

        m_buff = System.Delegate.CreateDelegate(typeof(BuffAction), this, (type).ToString()) as BuffAction;
    }
    public void Enabled(BaseEffect effect)
    {
        IsStart = true;

        if(!m_isInterval)
            m_buff(true);

        if (effect != null)
            m_effect = effect;
    }
    public void Continue(BaseEffect effect)
    {
        ElapsedTime = 0;
        if (effect != null)
        {
            m_effect.Disabled();
            m_effect = effect;
        }
    }
    public void Disabled()
    {
        IsStart = false;
        if (m_effect != null)
        {
            m_effect.Disabled();
            m_effect = null;
        }
        m_buff(false);
        m_buff = null;
    }
    public virtual void MoveFrame()
    {
        if (!IsStart)
            return;

        ElapsedTime += Time.deltaTime;
        if (ElapsedTime > DurationTime)
        {
            Disabled();
            return;
        }

        if (m_isInterval)
        {
            m_intervalElapsedTime += Time.deltaTime;
            if(m_intervalElapsedTime>m_intervalTime)
            {
                m_buff(true);
                m_intervalElapsedTime = 0;
            }
        }
    }
    void StrengthATK(bool start)
    {
        if(start)
        {
            m_character.StatSystem.BaseStat.AttackDamage += m_value;
        }
        else
        {
            m_character.StatSystem.BaseStat.AttackDamage -= m_value;
        }
    }
    void RecoveryHPPer(bool start)
    {
        if(start)
            if(m_caster.tag == "Player")
                NetworkMng.Instance.NotifyRecoveryHP(m_caster.UniqueID, m_character.UniqueID, 0, m_value);
    }
    void RecoveryMPPer(bool start)
    {
        if (start)
            if (m_caster.tag == "Player")
                NetworkMng.Instance.NotifyRecoveryMP(m_caster.UniqueID, m_character.UniqueID, 0, m_value);
    }
    void WeakenDEF(bool start)
    {
        if(start)
            m_character.StatSystem.BuffStat.Defence -= m_value;
        else
            m_character.StatSystem.BuffStat.Defence += m_value;
    }
    void StrengthReduction(bool start)
    {
        if (start)
            m_character.StatSystem.BuffStat.Reduction += m_value;
        else
            m_character.StatSystem.BuffStat.Reduction -= m_value;
    }
    void WeakenReduction(bool start)
    {
        if (start)
            m_character.StatSystem.BuffStat.Reduction -= m_value;
        else
            m_character.StatSystem.BuffStat.Reduction += m_value;
    }
    void Bless(bool start)
    {
        if(start)
        {
            m_character.StatSystem.BuffStat.AttackDamage += m_values[0];
            m_character.StatSystem.BuffStat.Defence += m_values[1];
            m_character.StatSystem.BuffStat.AddEXPPer += m_values[2];
        }
        else
        {
            m_character.StatSystem.BuffStat.AttackDamage -= m_values[0];
            m_character.StatSystem.BuffStat.Defence -= m_values[1];
            m_character.StatSystem.BuffStat.AddEXPPer -= m_values[2];
        }
    }
    void Burn(bool start)
    {
        m_character.ReceiveAttack(new SReceiveHandle(EAttackType.Burn, int.Parse(m_values[1].ToString("F0")), m_values[0]));
    }
    void InvincibilityArmor(bool start)
    {
        m_character.AttackSystem.Invincibility = start;
    }
    void SuperArmor(bool start)
    {
        m_character.AttackSystem.SuperArmor = start;
    }
    void Revelation(bool start)
    {
        if (start)
        {
            m_character.StatSystem.BuffStat.SkillDamagePro += m_values[0];
            m_character.StatSystem.BuffStat.Reduction += m_values[1];
            m_character.StatSystem.BuffStat.MoveSpeedPro += m_values[2];
        }
        else
        {
            m_character.StatSystem.BuffStat.SkillDamagePro -= m_values[0];
            m_character.StatSystem.BuffStat.Reduction -= m_values[1];
            m_character.StatSystem.BuffStat.MoveSpeedPro -= m_values[2];
        }
    }
    void Bethlehem(bool start)
    {
        if (start)
        {
            m_character.StatSystem.BuffStat.MoveSpeedPro += m_values[0];
            m_character.StatSystem.BuffStat.Reduction += m_values[1];
        }
        else
        {
            m_character.StatSystem.BuffStat.MoveSpeedPro -= m_values[0];
            m_character.StatSystem.BuffStat.Reduction -= m_values[1];
        }
    }
    void SixWings(bool start)
    {
        if(start)
        {
            m_character.StatSystem.BuffStat.SkillDamagePro += m_values[0];
            m_character.StatSystem.BuffStat.MoveSpeedPro += m_values[1];
        }
        else
        {
            m_character.StatSystem.BuffStat.SkillDamagePro -= m_values[0];
            m_character.StatSystem.BuffStat.MoveSpeedPro -= m_values[1];
        }
    }
}

