using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Hero_Warrior : BaseHero
{
    public override void AttackEffect(int count)
    {
        EffectMng.Instance.FindEffect("Attack/Effect_Warrior_AttackSlash" + count, transform.position, transform.eulerAngles, 1.5f);
    }
    public override void AttackEvent(int count)
    {
        if (transform.tag != "Player")
            return;

        if (count != 2)
        {
            CameraMng.Instance.GetCamera<PlayerCamera>(CameraMng.CameraStyle.Player).CameraAction_Look(0.9f);
        }
        else
        {
            Nuckback(transform.forward, 0.15f, 0.3f);
            CameraMng.Instance.GetCamera<PlayerCamera>(CameraMng.CameraStyle.Player).CameraAction_Look(0.85f);
        }

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

        AttackSystem.SendDamage(count, UniqueID, AllyType, type, damage, "Attack/Effect_Warrior_AttackHit");
    }
}
