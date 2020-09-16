using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_Archer_SideWinder : BaseSkill
{
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
        Caster.Animator.Play("Skill_Archer_SideWinder");

        Transform trs = EffectMng.Instance.FindEffect("Skill/Effect_Archer_SideWinderArrow", Caster.AttachSystem.GetAttachPoint(EAttachPoint.SubWeapon).position, transform.eulerAngles, 0.2f).transform;
        trs.SetParent(Caster.AttachSystem.GetAttachPoint(EAttachPoint.SubWeapon));
        trs.localPosition = Vector3.zero;
        trs.localScale = Vector3.one;

        EAllyType targetAlly = EAllyType.Friendly | EAllyType.Player;
        if (Caster.AllyType != EAllyType.Hostile)
            targetAlly = EAllyType.Hostile;

        Transform target = null;

        yield return new WaitForSeconds(SkillInfo.DurationTime * 0.2f);

        int casterID = Caster.UniqueID;
        EAttackType type;
        float damage = 0;
    
        if (Caster.StatSystem.IsCritical)
        {
            type = EAttackType.Critical;
            damage = Caster.StatSystem.GetCriticalCalculateDamage * 4;
        }
        else
        {
            type = EAttackType.Normal;
            damage = Caster.StatSystem.GetNormalCalculateDamage * 4;
        }

        if(target != null)
            transform.LookAt(target);

        Vector3 launchAxis = Caster.AttachSystem.GetAttachPoint(EAttachPoint.Chest).position;
        Vector3 targetPos = transform.position + transform.forward * SkillInfo.Range;
        targetPos.y = launchAxis.y;
        DefaultMissile missile = EffectMng.Instance.FindMissile<DefaultMissile>("Missile_Archer_SideWinderArrow", SkillInfo.Range * 0.25f);
        missile.Enabled(Caster, type, targetAlly, damage, 0.5f, launchAxis, targetPos, HitAction);
        trs.SetParent(EffectMng.Instance.transform);
    }

    void HitAction(BaseCharacter character)
    {
        Vector3 nuckBackPos = (character.transform.position - transform.position).normalized;
        character.Nuckback(nuckBackPos, 0.3f, 2);
    }
}
