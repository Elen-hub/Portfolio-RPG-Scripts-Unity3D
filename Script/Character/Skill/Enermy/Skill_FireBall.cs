using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_FireBall : BaseSkill
{
    int m_splashRange = 2;
    int m_range = 6;
    public override BaseSkill Init(BaseEnermy caster)
    {
        Caster = caster;
        DurationTime = 1.6f;
        CompleteTime = 1.6f;
        CoolTime = 5;
        ElapsedTime = CoolTime;
        return this;
    }
    public override bool RangeCheck()
    {
        if (Vector3.Distance(Caster.transform.position, Caster.Target.transform.position) > 4.5f)
        {
            Caster.MoveSystem.SetMoveToTarget(Caster.Target.transform, 4.5f);
            return false;
        }
        return true;
    }
    public override bool Using()
    {
        if (Caster.AttackSystem.HoldAttack || Caster.State == BaseCharacter.CharacterState.Death)
            return false;
        
        if (!PossibleSkill)
            return false;

        if (Vector3.Distance(Caster.transform.position, Caster.Target.transform.position) > m_range)
            return false;

        return true;
    }
    public override void Use()
    {
        PossibleSkill = false;
        ElapsedTime = 0;

        StartCoroutine(Skill());
    }
    IEnumerator Skill()
    {
        Caster.Animator.Play("FireBall");
        yield return new WaitForSeconds(0.66f);

        if (Caster.Target == null || Caster.State == BaseCharacter.CharacterState.Death)
            yield break;

        EAllyType targetAlly = EAllyType.Hostile;
        if (Caster.AllyType == EAllyType.Hostile)
            targetAlly = EAllyType.Friendly | EAllyType.Player;
        EAttackType type = EAttackType.Normal;
        float damage = Caster.StatSystem.GetNormalCalculateDamage * 3;

        SplashMissile missile = EffectMng.Instance.FindMissile<SplashMissile>("Missile_SkeletonMage_FireBall",0.5f*m_range / Vector3.Distance(Caster.transform.position, Caster.Target.transform.position));
        missile.Enabled(Caster, type, targetAlly, damage, 0.5f, transform.position, Caster.Target.transform.position, m_splashRange, false, Hit);

        yield return null;
    }
    void Hit(BaseCharacter character)
    {
        Vector3 nuckBackPos = (character.transform.position - transform.position).normalized;
        character.Nuckback(nuckBackPos, 0.5f, 2);
    }
}
