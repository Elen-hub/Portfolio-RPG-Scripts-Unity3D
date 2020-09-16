using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Hero_Archer : BaseHero
{
    public List<BaseCharacter> KnifeWindTargetList = new List<BaseCharacter>();
    public List<BaseCharacter> WindRainTargetList = new List<BaseCharacter>();
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
        damage *= AttackSystem.NormalAttack.DamagePro[count];
        Vector3 launchPos = AttachSystem.GetAttachPoint(EAttachPoint.Weapon).position;
        Vector3 targetPos = launchPos + transform.forward * AttackSystem.NormalAttack.Range[count];
        targetPos.x += Random.Range(-0.3f, 0.3f);
        targetPos.y += Random.Range(-0.3f, 0.3f);
        targetPos.z += Random.Range(-0.3f, 0.3f);
        EffectMng.Instance.FindMissile<BaseMissile>("Missile_Archer_Arrow", 8).Enabled(this, launchPos, targetPos);
        AttackSystem.SendDamage(count, UniqueID, AllyType, type, damage, "Skill/Effect_Archer_NormalDefaultHit");
    }
}
