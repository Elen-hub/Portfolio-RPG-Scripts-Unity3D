using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_RageAssault : BaseSkill
{
    float m_range = 3;
    float m_angle = 180;
    public override BaseSkill Init(BaseEnermy caster)
    {
        Caster = caster;
        DurationTime = 2f;
        CompleteTime = 2f;
        CoolTime = 11;
        ElapsedTime = CoolTime;
        return this;
    }
    public override bool RangeCheck()
    {
        float successDistance = m_range+2;
        if (Vector3.Distance(Caster.transform.position, Caster.Target.transform.position) > successDistance)
        {
            Caster.MoveSystem.SetMoveToTarget(Caster.Target.transform, successDistance);
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

        return true;
    }
    public override void Use()
    {
        PossibleSkill = false;
        ElapsedTime = 0;

        transform.LookAt(Caster.Target.transform);
        Caster.Animator.Play("RageAssault");
        StartCoroutine(IEMovingToFoward(1.5f, 6));
    }
    void OnRageAssaultDamage()
    {
        EAllyType targetAlly = EAllyType.Hostile;
        if (Caster.AllyType == EAllyType.Hostile)
            targetAlly = EAllyType.Friendly | EAllyType.Player;

        List<BaseCharacter> characterList = CharacterMng.Instance.GetCharacterToRectangleRange(transform.position, transform.eulerAngles.y, m_range, m_range);
        for (int i = 0; i < characterList.Count; ++i)
        {
            BaseCharacter character = characterList[i];
            if ((character.AllyType & targetAlly) == 0)
                continue;
            if (character.State == BaseCharacter.CharacterState.Death)
                continue;

            EffectMng.Instance.FindEffect("Effect_DefaultHit_Red", character.AttachSystem.GetAttachPoint(EAttachPoint.Chest).position, character.transform.eulerAngles, 2);
            characterList[i].Nuckback((character.transform.position - transform.position).normalized, 0.3f, 2.5f);
            characterList[i].Stun(2);

            if (character.tag == "Player")
                NetworkMng.Instance.NotifyReceiveDamage(EAttackType.Normal, Caster.UniqueID, character.UniqueID, Caster.StatSystem.GetHP * 0.1f, 0.5f);
        }
    }
    IEnumerator IEMovingToFoward(float time, float distance)
    {
        yield return null;
        float elapsedTime = 0;
        while (true)
        {
            yield return null;
            if (elapsedTime > time)
                yield break;

            elapsedTime += Time.deltaTime;
            transform.position += transform.forward * distance / time * Time.deltaTime;
        }
    }
}
