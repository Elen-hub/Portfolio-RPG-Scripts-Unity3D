using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enermy_Normal_CloseAttack : BaseEnermy
{
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
        AttackSystem.SendDamage(count, UniqueID, AllyType, type, damage * AttackSystem.NormalAttack.DamagePro[count]);
    }
}
