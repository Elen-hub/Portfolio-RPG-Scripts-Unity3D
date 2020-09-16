using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;
[Flags]
public enum EAttackType
{
    Normal = 1,
    Critical = 2,
    Burn = 4,
    Absolutely = 8,
}
public class NormalAttack
{
    public int Handle;
    public int Count;

    public float[] DurationTime;
    public float[] CompleteTime;
    public float[] DamagePro;
    public float[] HitTime;

    public EFindCharacterType[] Type;
    public float[] Range;
    public float[] Angle;
    public float[] Width = new float[1];
    public int[] MissileHandle;
}
public struct SReceiveHandle
{
    public EAttackType Type;
    public int UniqueID;
    public float Damage;
    public float HitTime;

    public SReceiveHandle(EAttackType type, int uniqueID, float damage, float hitTime = 0)
    {
        Type = type;
        UniqueID = uniqueID;
        Damage = damage;
        HitTime = hitTime;
    }
}
[Serializable]
public class AttackSystem : MonoBehaviour
{
    BaseCharacter m_character;
    public NormalAttack NormalAttack;
    public Dictionary<int, BaseSkill> SkillDic = new Dictionary<int, BaseSkill>();

    public bool Invincibility = false;
    public bool SuperArmor = false;
    public bool ImmunStun = false;
    public bool HoldAttack = false;
    public bool CompleteAttack = true;
    public int AttackCount = 0;
    
    [SerializeField] float m_durationTime;
    [SerializeField] float m_completeTime;
    public float SetDurationTime { set { m_durationTime = value; } }
    public float SetCompleteTime { set { m_completeTime = value; } }

    public void Init(NormalAttack type)
    {
        m_character = GetComponent<BaseCharacter>();
        NormalAttack = type;
    }
    public void SendDamage(int count, int casterID, EAllyType allyType, EAttackType attackType, float damage, string effectPath = null)
    {
        EAllyType targetAllyType = EAllyType.Hostile;
        if (allyType == EAllyType.Hostile)
            targetAllyType = EAllyType.Player | EAllyType.Friendly;
        List<BaseCharacter> characters;
        switch (NormalAttack.Type[count])
        {
            case EFindCharacterType.OnlyDistance:
                characters = CharacterMng.Instance.GetCharactersToDistance(transform.position, NormalAttack.Range[count]);
                for (int i = 0; i < characters.Count; ++i)
                {
                    if ((targetAllyType & characters[i].AllyType) != 0)
                    {
                        if(effectPath != null)
                            EffectMng.Instance.FindEffect(effectPath, characters[i].AttachSystem.GetAttachPoint(EAttachPoint.Chest).position, characters[i].transform.eulerAngles, 1);

                        if (characters[i].tag == "Player" || transform.tag == "Player")
                            NetworkMng.Instance.NotifyReceiveDamage(attackType, casterID, characters[i].UniqueID, damage, NormalAttack.HitTime[count]);
                    }
                }
                break;
            case EFindCharacterType.Sector:
                characters = CharacterMng.Instance.GetCharacterToDot(transform, NormalAttack.Range[count], NormalAttack.Angle[count]);
                for (int i = 0; i < characters.Count; ++i)
                {
                    if ((targetAllyType & characters[i].AllyType) != 0)
                    {
                        if (effectPath != null)
                            EffectMng.Instance.FindEffect(effectPath, characters[i].AttachSystem.GetAttachPoint(EAttachPoint.Chest).position, characters[i].transform.eulerAngles, 1);

                        if (characters[i].tag == "Player" || transform.tag == "Player")
                            NetworkMng.Instance.NotifyReceiveDamage(attackType, casterID, characters[i].UniqueID, damage, NormalAttack.HitTime[count]);
                    }
                }
                break;
            case EFindCharacterType.Rectangle:
                characters = CharacterMng.Instance.GetCharacterToRectangleRange(transform.position, transform.eulerAngles.y, NormalAttack.Width[count], NormalAttack.Range[count]);
                for (int i = 0; i < characters.Count; ++i)
                {
                    if ((targetAllyType & characters[i].AllyType) != 0)
                    {
                        if (effectPath != null)
                            EffectMng.Instance.FindEffect(effectPath, characters[i].AttachSystem.GetAttachPoint(EAttachPoint.Chest).position, characters[i].transform.eulerAngles, 1);

                        if (characters[i].tag == "Player" || transform.tag == "Player")
                            NetworkMng.Instance.NotifyReceiveDamage(attackType, casterID, characters[i].UniqueID, damage, NormalAttack.HitTime[count]);
                    }
                }
                break;
        }

        //float Damage = damage * NormalAttack.DamagePro[AttackCount - 1];
        //SReceiveHandle Handle = new SReceiveHandle(casterID, Damage, 0.3f);

        //List<BaseCharacter> character = FindTargets(targetType, transform.position, NormalAttack.Range[AttackCount - 1]);

        //for (int i = 0; i < character.Count; ++i)
        //    character[i].ReceiveAttack(Handle);
    }
    public void UseAttack(int uniqueID, float damage)
    {
        // 일반공격 한계카운터 체크 
        if (AttackCount >= NormalAttack.Count)
            return;

        m_character.State = BaseCharacter.CharacterState.Battle;
        m_durationTime = NormalAttack.DurationTime[AttackCount];
        m_completeTime = NormalAttack.CompleteTime[AttackCount];
        CompleteAttack = false;
        HoldAttack = true;
        m_character.Animator.SetInteger("AttackCount", AttackCount);

        if (NormalAttack.Count > 1)
            ++AttackCount;
    }
    public void UseSkill(int handle)
    {
        SkillDic[handle].Use();
        m_durationTime = SkillDic[handle].SkillInfo.DurationTime;
        m_completeTime = SkillDic[handle].SkillInfo.CompleteTime;
        HoldAttack = true;
        CompleteAttack = false;
    }
    public void UseSkill(BaseSkill skill)
    {
        m_durationTime = skill.DurationTime;
        m_completeTime = skill.CompleteTime;
        HoldAttack = true;
        CompleteAttack = false;

        skill.Use();
    }
    void Update()
    {
        // 공격최소시간 체크
        if (HoldAttack)
        {
            m_durationTime -= Time.deltaTime * m_character.StatSystem.GetAttackSpeed;

            if (m_durationTime < 0)
                HoldAttack = false;
        }
        // 공격완전완료시간 체크
        if (!CompleteAttack)
        {
            m_completeTime -= Time.deltaTime * m_character.StatSystem.GetAttackSpeed;

            if (m_completeTime < 0)
            {
                AttackCount = 0;
                CompleteAttack = true;
            }
        }
    }
}
