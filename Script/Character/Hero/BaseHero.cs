using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BaseHero : BaseCharacter
{
    Collider m_collier;
    public string Name;
    public virtual void Init(int uniqueID, EAllyType allyType, NormalAttack attack, Stat stat, Dictionary<EItemType, Item_Equipment> equipment)
    {
        UniqueID = uniqueID;
        AllyType = allyType;
        Animator = GetComponent<Animator>();
        AttachSystem = gameObject.AddComponent<AttachSystem>();
        AttachSystem.Init();
        BuffSystem = gameObject.AddComponent<BuffSystem>();
        BuffSystem.Init();
        StatSystem = gameObject.AddComponent<StatSystem>();
        StatSystem.Init(stat, equipment);
        MoveSystem = gameObject.AddComponent<MoveSystem>();
        MoveSystem.Init();
        AttackSystem = gameObject.AddComponent<AttackSystem>();
        AttackSystem.Init(attack);
        Outline = gameObject.AddComponent<Outline>();
        Outline.OutlineWidth = 0;
        Outline.OutlineMode = Outline.Mode.OutlineVisible;
        m_collier = GetComponent<Collider>();

        m_actionDic.Add(CharacterState.Idle, Idle);
        m_actionDic.Add(CharacterState.Move, Move);
        m_actionDic.Add(CharacterState.Chase, Chase);
        m_actionDic.Add(CharacterState.Battle, Battle);
        m_actionDic.Add(CharacterState.Death, Death);
        State = CharacterState.Idle;
    }
    public override void SetAngle(float angle)
    {
        if (State == CharacterState.Death)
            return;

        MoveSystem.SetAxis = angle;
    }
    public override void ReceiveAttack(SReceiveHandle handle)
    {
        if (AttackSystem.Invincibility)
            return;

        if (State == CharacterState.Death)
            return;

        float damage = handle.Damage * (1 - StatSystem.GetReduction);
        if ((handle.Type & EAttackType.Absolutely) == 0)
            damage = damage - StatSystem.GetDefence * (1 + StatSystem.GetDefencePro);

        if (damage <= 0)
            return;

        if (StatSystem.CurrHP < damage)
        {
            StartCoroutine(DeathAction());
            return;
        }

        if (transform.tag == "Player")
            CameraMng.Hit(Color.red, Mathf.Clamp(damage / StatSystem.GetHP + 0.3f, 0, 1));
        else
            UIMng.Instance.GetUI<FieldUI>(UIMng.UIName.FieldUI).SetDamageText(this, damage.ToString("F0"), Color.red, (handle.Type & EAttackType.Critical) != 0);

        StatSystem.CurrHP -= damage;

        if (handle.HitTime != 0 && !AttackSystem.SuperArmor && AttackSystem.CompleteAttack)
        {
            float hitTime = handle.HitTime * Mathf.Clamp((1 - StatSystem.GetCON * 0.01f), 0.7f, 1);

            if (hitTime <= 0)
                return;

            SetHit(hitTime);
            Animator.SetFloat("HitTime", hitTime);
            Animator.SetTrigger("Hit");
        }
    }
    protected override void Idle()
    {
        Animator.SetInteger("State", 0);
    }
    protected override void Move()
    {
        if (IsStun || IsHit || IsNuckback || !AttackSystem.CompleteAttack)
        {
            State = CharacterState.Idle;
            return;
        }

        MoveSystem.Stop = true;
        Animator.SetInteger("State", 1);
        if (transform.tag == "Player")
        {
            MoveSystem.MoveSpeed = StatSystem.GetMoveSpeed * (1 + StatSystem.GetMoveSpeedPro);
            MoveSystem.RotateAxis();
            MoveSystem.MoveAxis();
            NetworkMng.Instance.NotifyCharacterState_Move(transform.position, transform.eulerAngles);
        }
    }
    protected override void Chase()
    {
        if (IsStun || IsHit || IsNuckback)
        {
            State = CharacterState.Idle;
            return;
        }

        Animator.SetInteger("State", 1);
        MoveSystem.MoveSpeed = StatSystem.GetMoveSpeed * (1 + StatSystem.GetMoveSpeedPro);
        if (transform.tag == "Player")
        {
            MoveSystem.NextFrameChase();
            NetworkMng.Instance.NotifyCharacterState_Chase(MoveSystem.Target.position, MoveSystem.ChaseDistance);
        }
        else
            MoveSystem.MoveToPosition();
    }
    protected override void Battle()
    {
        //if (transform.tag == "Player")
        //    Debug.Log("");

        MoveSystem.Stop = true;

        if(AttackSystem.CompleteAttack)
        {
            State = CharacterState.Idle;
            return;
        }

        Animator.SetInteger("State", 3);
    }
    protected override void Death()
    {

    }
    protected override void Update()
    {
        if (State != CharacterState.Death)
        {
            m_recoveryElapsedTime += Time.deltaTime;
            if (m_recoveryElapsedTime > 5)
            {
                StatSystem.RecoveryNature();
                m_recoveryElapsedTime = 0;
            }

            // if(!AttackSystem.Hit)
                base.Update();
        }
    }
    public virtual void AttackEffect(int count)
    {

    }
    public virtual void AttackEvent(int count)
    {
        if (transform.tag != "Player")
            return;

        EAttackType type;
        float damage;
        if (StatSystem.IsCritical)
        {
            type = EAttackType.Critical;
            damage = StatSystem.GetCriticalCalculateDamage;
        }
        else
        {
            type = EAttackType.Normal;
            damage = StatSystem.GetNormalCalculateDamage;
        }

        AttackSystem.SendDamage(count, UniqueID, AllyType, type, damage);
    }
    void FootL()
    {

    }
    void FootR()
    {

    }
    void Hit()
    {

    }
    public override void Revival(float hp, float mp)
    {
        base.Revival(hp, mp);
        m_collier.enabled = true;
    }
    protected IEnumerator DeathAction()
    {
        yield return null;
        BuffSystem.Disabled();
        m_collier.enabled = false;
        MoveSystem.Stop = true;
        StatSystem.CurrHP = 0;
        State = CharacterState.Death;
        Animator.Play("Death");
        if (tag == "Player")
        {
            CameraMng.Infrared_ON(3);
            yield return new WaitForSeconds(2f);
            UIMng.Instance.Open<SelectPopup>(UIMng.UIName.SelectPopup).RevivePopup.Enabled();
        }
    }
}
