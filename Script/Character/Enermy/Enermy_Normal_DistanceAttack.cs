using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enermy_Normal_DistanceAttack : BaseEnermy
{
    [Header("Default Option")]
    [SerializeField] protected EMissileType m_missileType;
    [SerializeField] protected string m_missilePath;
    [SerializeField] protected float m_missileSpeed;
    [SerializeField] protected Vector3 m_launcherAxis = new Vector3(0, 0.5f, 0);
    [Header("Penetration Option")]
    [SerializeField] protected float m_maxPenetration = 1;
    [SerializeField] protected float m_declineDamage = 0;

    public void AttackEvent(int count)
    {
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

        Vector3 startPos = transform.position + m_launcherAxis;
        Vector3 targetPos = Target.transform.position + (Target.transform.position - transform.position).normalized * 0.5f;

        switch (m_missileType)
        {
            case EMissileType.Default:
                DefaultMissile DefaultMissile = EffectMng.Instance.FindMissile<DefaultMissile>(m_missilePath, m_missileSpeed);
                DefaultMissile.Enabled(this, type, EAllyType.Player | EAllyType.Friendly, damage * AttackSystem.NormalAttack.DamagePro[count], AttackSystem.NormalAttack.HitTime[count], startPos, targetPos);
                break;
            case EMissileType.Penetration:
                PenetrationMissile PenetrationMissile = EffectMng.Instance.FindMissile<PenetrationMissile>(m_missilePath, m_missileSpeed);
                PenetrationMissile.Enabled(this, type, EAllyType.Player | EAllyType.Friendly, damage * AttackSystem.NormalAttack.DamagePro[count], AttackSystem.NormalAttack.HitTime[count], startPos, targetPos, m_maxPenetration, m_declineDamage);
                break;
        }
        // AttackSystem.SendDamage(UniqueID, AllyType, type, damage);
    }
}
