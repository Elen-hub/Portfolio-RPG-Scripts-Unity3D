using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BaseHero : BaseCharacter
{
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

        m_stateDic.Add(CharacterState.Idle, new State_Idle_Hero(this));
        m_stateDic.Add(CharacterState.Move, new State_Move_Hero(this));
        m_stateDic.Add(CharacterState.Chase, new State_Chase_Hero(this));
        m_stateDic.Add(CharacterState.Battle, new State_Battle_Hero(this));
        m_stateDic.Add(CharacterState.Death, new State_Death_Hero(this));
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
            State = CharacterState.Death;
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
}
