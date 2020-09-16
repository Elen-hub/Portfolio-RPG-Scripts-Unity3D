using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_Crusader_WillShield : BaseSkill
{
    public Hero_Crusader Crusader;

    public override bool Using()
    {
        if (base.Using())
            return true;
        return false;
    }
    public override void Use()
    {
        base.Use();
        if (Crusader == null)
            Crusader = GetComponent<Hero_Crusader>();
        
        StartCoroutine(Skill());
    }

    IEnumerator Skill()
    {
        Crusader.ShieldHP = Caster.StatSystem.GetHP * 0.2f;
        Crusader.UseShield = true;
        Caster.Animator.SetBool("Guard", Crusader.UseShield);
        Caster.Animator.Play("Shield_Skill_WillShield");
        
        BaseEffect effect = EffectMng.Instance.FindEffect("Skill/Effect_Crusader_WillShield", Crusader.AttachSystem.GetAttachPoint(EAttachPoint.SubWeapon), 6);

        WaitForSeconds wait = new WaitForSeconds(0.1f);

        int count = 0;
        while (IsKeyDown && count < 30)
        {
            if (!Crusader.UseShield)
            {
                IsKeyDown = false;
                effect.DisabledTime();
                Crusader.Animator.Play("Shield_Skill_WillShieldBreak");
                Crusader.ShieldHP = 0;
                Crusader.UseShield = false;
                Crusader.AttackSystem.SetDurationTime = 1.2f;
                Crusader.AttackSystem.SetCompleteTime = 1.2f;
                Crusader.Animator.SetBool("Guard", Crusader.UseShield);
                yield break;
            }

            ++count;
            yield return wait;
        }

        effect.DisabledTime();
        IsKeyDown = false;
        Crusader.AttackSystem.SetDurationTime = 0.2f;
        Crusader.AttackSystem.SetCompleteTime = 0.2f;
        Crusader.ShieldHP = 0;
        Crusader.UseShield = false;
        Crusader.Animator.SetBool("Guard", Crusader.UseShield);
        ElapsedTime += CoolTime *0.5f;

        yield return null;
    }
}
