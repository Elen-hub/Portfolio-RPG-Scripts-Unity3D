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
    public Vector3 m_initPosition;
    public float RespawnTime;
    protected EnermyPattern m_nextPattern;
    public virtual void Respawn(int uniqueID, EAllyType allyType, Vector3 pos)
    {
        UniqueID = uniqueID;
        transform.position = pos;
        m_initPosition = transform.position;
        StatSystem.CurrHP = StatSystem.GetHP;
        State = CharacterState.Idle;
        gameObject.SetActive(true);
    }
    public virtual void Init(int uniqueID, EAllyType allyType, NormalAttack attack, Stat stat)
    {
        UniqueID = uniqueID;
        AllyType = allyType;
        m_collider = gameObject.AddComponent<SphereCollider>();
        m_collider.radius = GameSystem.NormalMonsterColliderRange;
        m_collider.isTrigger = true;

        m_initPosition = transform.position;
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

        m_actionDic.Add(CharacterState.Idle, Idle);
        m_actionDic.Add(CharacterState.Move, Move);
        m_actionDic.Add(CharacterState.Chase, Chase);
        m_actionDic.Add(CharacterState.Battle, Battle);
        m_actionDic.Add(CharacterState.Death, Death);
        State = CharacterState.Idle;

        List<EnermyPattern> patterns = stat.Pattern;
        for (int i = 0; i < patterns.Count; ++i)
        {
            EnermyPattern pattern = patterns[i].Clone();
            pattern.Init(this);
            m_patternList.Add(pattern);
        }
    }
    protected override void Idle()
    {
        Animator.SetInteger("State", 0);
        if (Target != null)
            State = CharacterState.Chase;
    }
    protected override void Move()
    {
        if (IsHit || IsNuckback || IsStun || !AttackSystem.CompleteAttack)
            return;

        Animator.SetInteger("State", 1);
        StatSystem.RecoveryHP(StatSystem.GetHP * Time.deltaTime * 0.2f);
        MoveSystem.MoveSpeed = StatSystem.GetMoveSpeed * (1 + StatSystem.GetMoveSpeedPro);
        if (MoveSystem.MoveToPosition(m_initPosition, 0))
        {
            RESET();
            StatSystem.RecoveryHP(StatSystem.GetHP);
            State = CharacterState.Idle;
            m_collider.enabled = true;
        }
    }
    protected override void Chase()
    {
        if (IsHit || IsNuckback || IsStun || !AttackSystem.CompleteAttack)
            return;

        if (Target == null)
        {
            State = CharacterState.Move;
            return;
        }

        // 후퇴거리 체크
        float distance = Vector3.Distance(transform.position, Target.transform.position);
        if (distance > GameSystem.NormalMonsterChaseRangeToTarget || Vector3.Distance(transform.position, m_initPosition) > GameSystem.NormalMonsterChaseRangeToSpawn || Target.State == CharacterState.Death)
        {
            Target = null;
            return;
        }

        // 패턴사용 체크
        bool pattern = false;
        for (int i = 0; i < m_patternList.Count; ++i)
        {
            if (m_patternList[i].IsUse())
            {
                pattern = true;
                if (m_patternList[i].RangeCheck())
                {
                    State = CharacterState.Battle;
                    return;
                }
            }
        }
        // 일반공격 체크
        if (!pattern)
        {
            if (distance < AttackSystem.NormalAttack.Range[AttackSystem.AttackCount] * 0.7f)
            {
                State = CharacterState.Battle;
                return;
            }
        }

        Animator.SetInteger("State", 1);
        MoveSystem.MoveSpeed = StatSystem.GetMoveSpeed * (1 + StatSystem.GetMoveSpeedPro);
        MoveSystem.NextFrameChase();
    }
    protected override void Battle()
    {
        MoveSystem.Stop = true;

        if (IsHit || IsNuckback || IsStun || !AttackSystem.CompleteAttack)
            return;

        if (Target == null || Target.State == CharacterState.Death)
        {
            Target = null;
            State = CharacterState.Move;
            return;
        }
        for(int i =0; i<m_patternList.Count; ++i)
        {
            if (m_patternList[i].IsUse())
            {
                if (m_patternList[i].RangeCheck())
                {
                    m_patternList[i].Use();
                    return;
                }
            }
        }
        float distance = Vector3.Distance(transform.position, Target.transform.position);
        if (distance > AttackSystem.NormalAttack.Range[AttackSystem.AttackCount] * 0.7f)
        {
            MoveSystem.SetMoveToTarget(Target.transform, AttackSystem.NormalAttack.Range[AttackSystem.AttackCount] * 0.7f);
            return;
        }
        Animator.SetInteger("State", 0);
        Animator.Play("Attack");
        transform.LookAt(Target.transform);
        AttackSystem.UseAttack(UniqueID, StatSystem.GetAttackDamage);
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
            StartCoroutine(DeathAction());
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
    protected void OnTriggerEnter(Collider other)
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
    protected virtual void RESET()
    {
        for(int i =0; i<m_patternList.Count; ++i)
            m_patternList[i].Reset();

        BuffSystem.Disabled();
    }
    protected IEnumerator DeathAction()
    {
        yield return null;
        BuffSystem.Disabled();
        Target = null;
        MoveSystem.Stop = true;
        Animator.Play("Death");
        StatSystem.CurrHP = 0;

        yield return new WaitForSeconds(3);
        gameObject.SetActive(false);
    }
}
