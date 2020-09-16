using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Hero_Crusader : BaseHero
{
    [HideInInspector] public GameObject Wing;
    [Header("Crusader")]
    public bool UseShield;
    public float ShieldHP;
    public override void Init(int uniqueID, EAllyType allyType, NormalAttack attack, Stat stat, Dictionary<EItemType, Item_Equipment> equipment)
    {
        base.Init(uniqueID, allyType, attack, stat, equipment);
        Wing = GetComponentInChildren<Wing>(true).gameObject;
    }
    public override void AttackEffect(int count)
    {
        EffectMng.Instance.FindEffect("Attack/Effect_Crusader_AttackSlash" + count, transform.position, transform.eulerAngles, 1.5f);
    }
    public override void AttackEvent(int count)
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

        AttackSystem.SendDamage(count, UniqueID, AllyType, type, damage, "Attack/Effect_Crusader_AttackHit");
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

        if(UseShield)
        {
            float shield = ShieldHP;
            ShieldHP -= damage;
            if (ShieldHP <= 0)
            {
                damage -= shield;
                UseShield = false;
                UIMng.Instance.GetUI<FieldUI>(UIMng.UIName.FieldUI).SetDamageText(this, "GuardBreak!", Color.red, (handle.Type & EAttackType.Critical) != 0);
            }
            else
                UIMng.Instance.GetUI<FieldUI>(UIMng.UIName.FieldUI).SetDamageText(this, "Guard", Color.red, (handle.Type & EAttackType.Critical) != 0);
        }
        else
        {
            if (handle.HitTime != 0 && !AttackSystem.SuperArmor && AttackSystem.CompleteAttack)
            {
                if (handle.HitTime <= 0)
                    return;

                SetHit(handle.HitTime);
                Animator.SetFloat("HitTime", handle.HitTime);
                Animator.SetTrigger("Hit");
            }
        }

        StatSystem.CurrHP -= damage;
        if (transform.tag == "Player")
            CameraMng.Hit(Color.red, 0.3f);
        else
            UIMng.Instance.GetUI<FieldUI>(UIMng.UIName.FieldUI).SetDamageText(this, damage.ToString("F0"), Color.red, (handle.Type & EAttackType.Critical) != 0);

        if (StatSystem.CurrHP < damage)
        {
            StatSystem.CurrHP = 0;
            if (tag == "Player")
            {
                if (AttackSystem.SkillDic.ContainsKey(56))
                {
                    if (AttackSystem.SkillDic[56].Using())
                    {
                        MoveSystem.Stop = true;
                        AttackSystem.Invincibility = true;
                        MoveSystem.Stop = true;
                        State = CharacterState.Death;
                        Animator.Play("Death");
                        NetworkMng.Instance.NotifyCharacterState_Skill(transform.eulerAngles, 56);
                    }
                }
                else 
                    StartCoroutine(DeathAction());
            }
        }

        //if(transform.tag == "Player")
        //    CameraMng.Hit(Color.red, 0.3f);
        //else
        //    UIMng.Instance.Open<FieldUI>(UIMng.UIName.FieldUI).SetDamageText(this, damage.ToString("F0"), Color.red, (handle.Type & EAttackType.Critical) != 0);

    }
}
