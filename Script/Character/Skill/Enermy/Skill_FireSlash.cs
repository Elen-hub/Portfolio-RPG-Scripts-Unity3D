using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_FireSlash : BaseSkill
{
    Enermy_SkeletonKing Enermy;
    float m_range = 4.5f;
    float m_angle = 180;
    public override BaseSkill Init(BaseEnermy caster)
    {
        Enermy = caster as Enermy_SkeletonKing;
        DurationTime = 3;
        CompleteTime = 3;
        CoolTime = 7;
        ElapsedTime = CoolTime;
        return this;
    }
    public override bool RangeCheck()
    {
        if (Vector3.Distance(Enermy.transform.position, Enermy.Target.transform.position) > 3.75f)
        {
            Enermy.MoveSystem.SetMoveToTarget(Enermy.Target.transform, 3.75f);
            return false;
        }
        return true;
    }
    public override bool Using()
    {
        if (Enermy.AttackSystem.HoldAttack || Enermy.State == BaseCharacter.CharacterState.Death)
            return false;
        
        if (!PossibleSkill)
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
        yield return null;
        transform.LookAt(Enermy.Target.transform);
        Enermy.Animator.Play("FireSlash");
    }
    void FireSlash()
    {
        EffectMng.Instance.FindEffect("Enermy/Effect_Enermy_FireSlash", transform.position, transform.eulerAngles, 2);

        EAllyType targetAlly = EAllyType.Hostile;
        if (Enermy.AllyType == EAllyType.Hostile)
            targetAlly = EAllyType.Friendly | EAllyType.Player;

        //BaseCharacter character = PlayerMng.Instance.MainPlayer.Character;
        //if(CharacterMng.Instance.CheckToDot(character.transform.position, transform, m_range, m_angle))
        //{
        //    if ((character.AllyType & targetAlly) == 0)
        //        return;
        //    if (character.State == BaseCharacter.CharacterState.Death)
        //        return;

        //    NetworkMng.Instance.NotifyReceiveDamage(EAttackType.Absolutely, Enermy.UniqueID, character.UniqueID, character.StatSystem.GetAttackDamage * 5, 1);
        //}

        List<BaseCharacter> characterList = CharacterMng.Instance.GetCharacterToDot(transform, m_range, m_angle);
        for (int i = 0; i < characterList.Count; ++i)
        {
            BaseCharacter character = characterList[i];
            if ((character.AllyType & targetAlly) == 0)
                continue;
            if (character.State == BaseCharacter.CharacterState.Death)
                continue;

            EffectMng.Instance.FindEffect("Effect_DefaultHit_Red", character.AttachSystem.GetAttachPoint(EAttachPoint.Chest).position, character.transform.eulerAngles, 2);
            characterList[i].Nuckback((character.transform.position - transform.position).normalized, 0.15f, 1);
            characterList[i].Stun(1);

            if (character.tag == "Player")
                NetworkMng.Instance.NotifyReceiveDamage(EAttackType.Absolutely, Enermy.UniqueID, character.UniqueID, Enermy.StatSystem.GetAttackDamage * 5, 0.5f);
        }
    }
    void FireSlashEnd()
    {

    }
}
