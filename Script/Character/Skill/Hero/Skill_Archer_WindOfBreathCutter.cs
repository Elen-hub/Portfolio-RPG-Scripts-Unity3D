using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_Archer_WindOfBreathCutter : BaseSkill
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
        Caster.Animator.Play("Bow_WindOfBreathCutter1");
        Caster.Animator.SetBool("WindOfBreathCutter", true);
        Transform arrow = EffectMng.Instance.FindEffect("Skill/Effect_Archer_WindOfBreathArrow", Caster.AttachSystem.GetAttachPoint(EAttachPoint.Weapon).position, transform.eulerAngles, 3).transform;
        arrow.SetParent(Caster.AttachSystem.GetAttachPoint(EAttachPoint.Weapon));
        arrow.localScale = Vector3.one;

        WaitForSeconds wait = new WaitForSeconds(0.11f);
        yield return new WaitForSeconds(0.07f);

        EAllyType targetAlly = EAllyType.Hostile;
        if (Caster.AllyType == EAllyType.Hostile)
            targetAlly = EAllyType.Friendly | EAllyType.Player;

        EAttackType type;
        float damage = 0;

        int count = 0;
        Transform pivot = Caster.AttachSystem.GetAttachPoint(EAttachPoint.Chest);

        while (IsKeyDown && count < 31)
        {
            if (Caster.StatSystem.IsCritical)
            {
                type = EAttackType.Critical;
                damage = Caster.StatSystem.GetCriticalCalculateDamage;
            }
            else
            {
                type = EAttackType.Normal;
                damage = Caster.StatSystem.GetNormalCalculateDamage;
            }
            
            EffectMng.Instance.FindEffect("Skill/Effect_Archer_WindOfBreathShot", transform.position, transform.eulerAngles, 0.2f);
            PenetrationMissile missile = EffectMng.Instance.FindMissile<PenetrationMissile>("Missile_Archer_WindOfBreathArrow", 2);
            missile.Enabled(Caster, type, targetAlly, damage, 0.2f, pivot.position, pivot.position + transform.forward*10 + transform.right * Random.Range(-1f, 1f), 5, damage*0.1f, HitAction);

            ++count;
            yield return wait;
        }
        arrow.gameObject.SetActive(false);
        if (count >= 20)
            yield return new WaitForSeconds(0.1f);

        Caster.Animator.SetBool("WindOfBreathCutter", false);

        yield return new WaitForSeconds(0.3f);
        Caster.AttackSystem.HoldAttack = false;
        Caster.AttackSystem.CompleteAttack = true;
    }

    void HitAction(BaseCharacter character)
    {
        EffectMng.Instance.FindEffect("Skill/Effect_Archer_WindOfBreathHit", character.AttachSystem.GetAttachPoint(EAttachPoint.Chest).position, Vector3.zero, 2);
    }
}
