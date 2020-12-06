using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum EMonsterGrade
{
    Normal,
    Elite,
    MediumBoss,
    Boss,
}
public abstract class BaseEnermy : BaseCharacter
{
    protected List<EnermyPattern> m_patternList = new List<EnermyPattern>();
    protected SphereCollider m_collider;
    public Vector3 InitPosition;
    public float RespawnTime;
    protected EnermyPattern m_nextPattern;
    public virtual void Respawn(int uniqueID, EAllyType allyType, Vector3 pos)
    {
        RESET();
        UniqueID = uniqueID;
        transform.position = pos;
        InitPosition = pos;
        StatSystem.CurrHP = StatSystem.GetHP;
        m_stateDic[m_state].OnStateExit();
        m_state = CharacterState.Idle;
        gameObject.SetActive(true);
        MoveSystem.Stop = false;
        m_collider.enabled = true;
    }
    public virtual void Init(int uniqueID, EAllyType allyType, NormalAttack attack, Stat stat)
    {
        UniqueID = uniqueID;
        AllyType = allyType;
        m_collider = transform.Find("AgroCollision").GetComponent<SphereCollider>();
        m_collider.radius = GameSystem.NormalMonsterColliderRange;
        InitPosition = transform.position;
        Animator = GetComponent<Animator>();

        BuffSystem = gameObject.AddComponent<BuffSystem>();
        BuffSystem.Init();
        StatSystem = gameObject.AddComponent<StatSystem>();
        StatSystem.Init(stat, new Dictionary<EItemType, Item_Equipment>());
        MoveSystem = gameObject.AddComponent<MoveSystem>();
        MoveSystem.Init();
        AttackSystem = gameObject.AddComponent<AttackSystem>();
        AttackSystem.Init(attack);
        AttachSystem = gameObject.AddComponent<AttachSystem>();
        AttachSystem.Init();
        Outline = gameObject.AddComponent<Outline>();
        Outline.OutlineWidth = 0;
        Outline.OutlineMode = Outline.Mode.OutlineVisible;

        m_stateDic.Add(CharacterState.Idle, new State_Idle_EnermyDefault(this));
        m_stateDic.Add(CharacterState.Move, new State_Move_EnermyDefault(this, m_collider));
        m_stateDic.Add(CharacterState.Chase, new State_Chase_EnermyDefault(this));
        m_stateDic.Add(CharacterState.Battle, new State_Battle_EnermyDefault(this));
        m_stateDic.Add(CharacterState.Death, new State_Death_EnermyDefault(this));

        List<EnermyPattern> patterns = stat.Pattern;
        for (int i = 0; i < patterns.Count; ++i)
        {
            EnermyPattern pattern = patterns[i].Clone();
            pattern.Init(this);
            m_patternList.Add(pattern);
        }
    }
    public override void ReceiveAttack(SReceiveHandle handle)
    {
        if (State == CharacterState.Death)
            return;

        if (AttackSystem.Invincibility)
            return;

        if (Target == null)
        {
            RESET();
            Target = CharacterMng.Instance.CurrCharacters[handle.UniqueID];
            MoveSystem.SetMoveToTarget(Target.transform, AttackSystem.NormalAttack.Range[AttackSystem.AttackCount]);
            State = CharacterState.Chase;
        }

        float damage = handle.Damage;
        if ((handle.Type & EAttackType.Absolutely) == 0)
            damage = (damage - StatSystem.GetDefence * (1 + StatSystem.GetDefencePro)) * (1 - StatSystem.GetReduction);

        if (damage <= 0)
            return;

        UIMng.Instance.GetUI<FieldUI>(UIMng.UIName.FieldUI).SetDamageText(this, damage.ToString("F0"), Color.red, (handle.Type & EAttackType.Critical) != 0);

        if (StatSystem.CurrHP < damage)
        {
            if(handle.UniqueID == PlayerMng.Instance.MainPlayer.Character.UniqueID)
                NetworkMng.Instance.RequestEnermyKill(UniqueID);

            State = CharacterState.Death;
            return;
        }
        StatSystem.CurrHP -= damage;

        float hitTime = handle.HitTime;
        if (hitTime > 0 && !AttackSystem.SuperArmor)
        {
            SetHit(handle.HitTime);
            Animator.SetFloat("HitTime", hitTime);
            Animator.SetTrigger("Hit");
        }
    }
    public bool CheckBattlePattern()
    {
        for (int i = 0; i < m_patternList.Count; ++i)
        {
            if (m_patternList[i].IsUse())
            {
                if (m_patternList[i].RangeCheck())
                {
                    State = CharacterState.Battle;
                    return true;
                }
            }
        }
        return false;
    }
    public bool UseBattlePattern()
    {
        for (int i = 0; i < m_patternList.Count; ++i)
        {
            if (m_patternList[i].IsUse())
            {
                if (m_patternList[i].RangeCheck())
                {
                    Animator.SetInteger("State", 0);
                    m_patternList[i].Use();
                    return true;
                }
            }
        }
        return false;
    }
    protected virtual void OnTriggerEnter(Collider other)
    {
        if (Target != null)
            return;
        if (State == CharacterState.Death)
            return;

        if (other.tag == "Player" || other.tag == "Ally")
        {
            RESET();
            m_collider.enabled = false;
            Target = other.GetComponent<BaseCharacter>();
            MoveSystem.SetMoveToTarget(Target.transform, AttackSystem.NormalAttack.Range[AttackSystem.AttackCount] * 0.7f);
            State = CharacterState.Chase;
        }
    }
    public virtual void RESET()
    {
        for(int i =0; i<m_patternList.Count; ++i)
            m_patternList[i].Reset();

        BuffSystem.Disabled();
    }
}
