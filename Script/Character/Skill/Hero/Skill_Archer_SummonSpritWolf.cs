using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_Archer_SummonSpritWolf : BaseSkill
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
        Caster.Animator.Play("Skill_Archer_SummonSprit");
        // 대기시간
        // 이펙트
        EffectMng.Instance.FindEffect("Skill/Effect_Archer_SummonSpritWolfReady", Caster.AttachSystem.GetAttachPoint(EAttachPoint.Chest).position, Vector3.zero, 3f);

        EAllyType targetAlly = EAllyType.Hostile;
        if(Caster.AllyType != EAllyType.Hostile)
            targetAlly = EAllyType.Friendly | EAllyType.Player;

        Stat stat = new Stat();
        EffectMng.Instance.FindEffect("Skill/Effect_Archer_SummonSpritWolfTarget", transform.position + transform.forward*2, Vector3.zero, 3f);

        yield return null;
    }
}
