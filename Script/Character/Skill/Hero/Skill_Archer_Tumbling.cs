using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_Archer_Tumbling : BaseSkill
{
    bool m_isTumbling;
    float m_elapsedTime;
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
        yield return null;
        m_elapsedTime = 0;
        m_isTumbling = true;
        Caster.AttackSystem.Invincibility = true;
        Caster.Animator.Play("Skill_Archer_Tumbling");
        EffectMng.Instance.FindEffect("Skill/Effect_Archer_Tumbling", transform, 0.4f);
    }
    protected override void Update()
    {
        base.Update();

        if (!m_isTumbling)
            return;

        if (m_elapsedTime < 0.55f)
        {
            m_elapsedTime += Time.deltaTime;
            transform.position += transform.forward * Time.deltaTime * Caster.StatSystem.GetMoveSpeed * (1 + Caster.StatSystem.GetMoveSpeedPro) *1.5f;
        }
        else
        {
            Caster.AttackSystem.Invincibility = false;
            m_isTumbling = false;
        }
    }
}
