using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_Archer_KnifeWind : BaseSkill
{
    new Hero_Archer Caster;
    public override BaseSkill Init(BaseCharacter caster, SkillInfo info)
    {
        base.Init(caster, info);
        Caster = GetComponent<Hero_Archer>();
        return this;
    }
    public override bool Using()
    {
        if (base.Using())
            return true;

        return false;
    }
    public override void Use()
    {
        base.Use();

        
        StartCoroutine(Skill());
    }

    IEnumerator Skill()
    {
        Caster.KnifeWindTargetList.Clear();
        Caster.Animator.Play("Skill_Archer_KnifeWind");
        // 대기시간
        yield return new WaitForSeconds(SkillInfo.DurationTime * 0.6f);
        EffectMng.Instance.FindEffect("Skill/Effect_Archer_KnifeWindBlast", transform.position, transform.eulerAngles, 1);

        Vector3 targetPos = transform.position + transform.forward * (SkillInfo.Range);
        EAllyType targetAlly = EAllyType.Friendly | EAllyType.Player;
        if (Caster.AllyType != EAllyType.Hostile)
            targetAlly = EAllyType.Hostile;

        EAttackType type;
        float damage;

        if (Caster.StatSystem.IsCritical)
        {
            type = EAttackType.Critical;
            damage = Caster.StatSystem.GetCriticalCalculateDamage * 4.5f;
        }
        else
        {
            type = EAttackType.Normal;
            damage = Caster.StatSystem.GetNormalCalculateDamage * 4.5f;
        }

        for (int i =-3; i<4; ++i)
        {
            Vector3 target = MathLib.GetVector3AngleOfPosition(transform.position, targetPos, i * 5);
            DefaultMissile missile = EffectMng.Instance.FindMissile<DefaultMissile>("Missile_Archer_KnifeWindArrow", SkillInfo.Range);
            missile.Enabled(Caster, type, targetAlly, damage, 0.3f, Caster.AttachSystem.GetAttachPoint(EAttachPoint.Chest).position, target, 
                (BaseCharacter character) => character.Nuckback((character.transform.position - transform.position).normalized, 0.15f, 0.5f));
        }
        WaitForSeconds wait = new WaitForSeconds(0.02f);
        for (int i = 0; i < 10; ++i)
        {
            transform.position -= transform.forward * 0.03f;
            yield return wait;
        }

        yield return null;
    }
}
